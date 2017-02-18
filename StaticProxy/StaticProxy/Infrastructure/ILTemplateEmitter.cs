using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using ClrTest.Reflection;
using PhillipScottGivens.StaticProxy.Templates;


namespace PhillipScottGivens.StaticProxy.Infrastructure
{
    public class ILTemplateEmitter : ILEmitterVisitor
    {
        #region Template Place Holders
        private static readonly MethodInfo Call_Base;
        private static readonly MethodInfo Return_Parameter;
        #endregion

        private readonly MethodInfo methodToOverride;
        private readonly MethodInfo templateMethod;
        private readonly ILGenerator ilGenerator;

        private readonly ParameterInfo[] parameterInfos;
        private readonly Type returnType;
        private readonly Label returnLabel;
        private readonly IList<LocalVariableInfo> localVariables;
        private readonly IList<ExceptionHandlingClause> exceptionClauses;

        private readonly Dictionary<int, Label> labels = new Dictionary<int, Label>();
        private readonly Stack<ExceptionHandlingClause> handlingExceptions
            = new Stack<ExceptionHandlingClause>();

        private int currentExceptionBlock = 0;

        static ILTemplateEmitter()
        {
            var template = typeof(Proxy);

            // Template Parameters.
            Call_Base = template.GetMethod("Call_Base", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            Return_Parameter = template.GetMethod("Return_Parameter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        public ILTemplateEmitter(
            ILGenerator ilGenerator,
            MethodInfo templateMethod,
            MethodInfo methodToOverride,
            MethodBuilder methodBuilder)
            : base(methodBuilder, ilGenerator)
        {
            this.methodToOverride = methodToOverride;
            this.templateMethod = templateMethod;

            var templateBody = templateMethod.GetMethodBody();

            this.exceptionClauses = templateBody.ExceptionHandlingClauses;
            this.localVariables = templateBody.LocalVariables;

            this.ilGenerator = ilGenerator;
            returnType = methodBuilder.ReturnType;

            foreach (var local in localVariables)
                ilGenerator.DeclareLocal(local.LocalType);

            if (returnType != typeof(void))
            {
                ilGenerator.DeclareLocal(returnType);
                returnLabel = ilGenerator.DefineLabel();
            }
            parameterInfos = methodToOverride.GetParameters();
        }


        public virtual bool Process(ILInstruction ilInstruction, object operand)
        {
            bool skipInstruction = false;
            Label label;
            int offset = ilInstruction.Offset;
            if (labels.TryGetValue(offset, out label))
            {
                ilGenerator.MarkLabel(label);
                labels.Remove(offset);
            }
            if (currentExceptionBlock < exceptionClauses.Count
                && offset == exceptionClauses[currentExceptionBlock].TryOffset)
            {
                ilGenerator.BeginExceptionBlock();
                handlingExceptions.Push(exceptionClauses[currentExceptionBlock++]);
            }
            Queue<ExceptionHandlingClause> finallyClauses = new Queue<ExceptionHandlingClause>();
            foreach (var exceptionHandler in handlingExceptions)
            {
                if (ilInstruction.OpCode == OpCodes.Leave
                    && ilInstruction.Offset == exceptionHandler.HandlerOffset - 5)
                {
                    skipInstruction = true;
                    continue;
                }
                if (ilInstruction.OpCode == OpCodes.Leave_S)
                {
                    if (offset == exceptionHandler.HandlerOffset - 2)
                    {
                        //ilInstruction.OpCode = OpCodes.Leave;
                        skipInstruction = true;
                        continue;
                    }
                }

                if (offset == exceptionHandler.HandlerOffset)
                {
                    if (exceptionHandler.Flags == ExceptionHandlingClauseOptions.Clause)
                    {
                        ilGenerator.BeginCatchBlock(exceptionHandler.CatchType);
                    }
                    else
                    {
                        ilGenerator.BeginFinallyBlock();
                    }
                }
                else if (offset == exceptionHandler.HandlerOffset + exceptionHandler.HandlerLength)
                {
                    ilGenerator.EndExceptionBlock();
                    finallyClauses.Enqueue(exceptionHandler);
                }
            }
            foreach (var exceptionClause in finallyClauses)
                if (exceptionClause != handlingExceptions.Pop())
                    throw new InvalidOperationException();

            return skipInstruction;
        }

        #region Special Processing Visitor Instructions
        public override void VisitInlineMethodInstruction(InlineMethodInstruction inlineMethodInstruction)
        {
            MethodBase method = inlineMethodInstruction.Method;
            Process(inlineMethodInstruction, method);

            if (method == Call_Base)
            {
                inlineMethodInstruction.Method = method;

                for (int i = 0; i <= parameterInfos.Length; i++)
                    LoadReference(ilGenerator, i);

                ilGenerator.Emit(OpCodes.Call, methodToOverride);   // call the method non-virtually

                return;
            }
            else if (method == Return_Parameter)
            {

                if (returnType == typeof(void))
                {
                    ilGenerator.Emit(OpCodes.Pop);
                }
                else
                {
                    if (returnType.IsValueType)
                    {
                        ilGenerator.Emit(OpCodes.Unbox, returnType);
                        ilGenerator.Emit(OpCodes.Ldobj, returnType);
                    }
                    else
                    {
                        ilGenerator.Emit(OpCodes.Castclass, returnType);
                    }

                    StoreLocation(ilGenerator, localVariables.Count);

                    if (this.handlingExceptions.Count > 0)
                    {
                        ilGenerator.Emit(OpCodes.Leave, returnLabel);
                    }
                    else
                    {
                        ilGenerator.Emit(OpCodes.Br, returnLabel);
                    }
                }
                return;
            }

            ilGenerator.Emit(inlineMethodInstruction.OpCode, (MethodInfo)method);
        }

        public override void VisitInlineNoneInstruction(InlineNoneInstruction inlineNoneInstruction)
        {
            Process(inlineNoneInstruction, null);

            if (inlineNoneInstruction.OpCode == OpCodes.Ret)
            {
                if (returnType != typeof(void))
                {
                    ilGenerator.MarkLabel(returnLabel);
                    LoadLocation(ilGenerator, localVariables.Count);
                }
            }

            ilGenerator.Emit(inlineNoneInstruction.OpCode);
        }

        public override void VisitShortInlineBrTargetInstruction(ShortInlineBrTargetInstruction shortInlineBrTargetInstruction)
        {
            if (!Process(shortInlineBrTargetInstruction, shortInlineBrTargetInstruction.TargetOffset))
                ilGenerator.Emit(shortInlineBrTargetInstruction.OpCode, CreateLabel(shortInlineBrTargetInstruction.TargetOffset));
        }
        #endregion

        #region Process and Emit Visitor Instructions
        public override void VisitInlineBrTargetInstruction(InlineBrTargetInstruction inlineBrTargetInstruction)
        {
            Process(inlineBrTargetInstruction, inlineBrTargetInstruction.TargetOffset);
            ilGenerator.Emit(inlineBrTargetInstruction.OpCode, inlineBrTargetInstruction.TargetOffset);
        }

        public override void VisitInlineFieldInstruction(InlineFieldInstruction inlineFieldInstruction)
        {
            FieldInfo field = inlineFieldInstruction.Field;
            Process(inlineFieldInstruction, field);
            ilGenerator.Emit(inlineFieldInstruction.OpCode, field);
        }

        public override void VisitInlineIInstruction(InlineIInstruction inlineIInstruction)
        {
            Process(inlineIInstruction, inlineIInstruction.Int32);
            ilGenerator.Emit(inlineIInstruction.OpCode, inlineIInstruction.Int32);
        }

        public override void VisitInlineI8Instruction(InlineI8Instruction inlineI8Instruction)
        {
            Process(inlineI8Instruction, inlineI8Instruction.Int64);
            ilGenerator.Emit(inlineI8Instruction.OpCode, inlineI8Instruction.Int64);
        }


        public override void VisitInlineRInstruction(InlineRInstruction inlineRInstruction)
        {
            Process(inlineRInstruction, inlineRInstruction.Double);
            ilGenerator.Emit(inlineRInstruction.OpCode, inlineRInstruction.Double);
        }

        public override void VisitInlineSigInstruction(InlineSigInstruction inlineSigInstruction)
        {
            Process(inlineSigInstruction, inlineSigInstruction.Signature);
            //ilGenerator.Emit(inlineSigInstruction.OpCode, inlineSigInstruction.Signature);
            System.Diagnostics.Debugger.Break();
        }

        public override void VisitInlineStringInstruction(InlineStringInstruction inlineStringInstruction)
        {
            Process(inlineStringInstruction, inlineStringInstruction.String);
            ilGenerator.Emit(inlineStringInstruction.OpCode, inlineStringInstruction.String);
        }

        public override void VisitInlineSwitchInstruction(InlineSwitchInstruction inlineSwitchInstruction)
        {
            System.Diagnostics.Debugger.Break();
            throw new NotSupportedException("VisitInlineSwitchInstruction");
            //Process(inlineSwitchInstruction, inlineSwitchInstruction.TargetOffsets);
            //ilGenerator.Emit(inlineSwitchInstruction.OpCode, inlineSwitchInstruction.TargetOffsets);            
        }

        public override void VisitInlineTokInstruction(InlineTokInstruction inlineTokInstruction)
        {
            System.Diagnostics.Debugger.Break();
            throw new NotSupportedException("VisitInlineTokInstruction");
            //MemberInfo member = inlineTokInstruction.Member;
            //Process(inlineTokInstruction, member);
            //ilGenerator.Emit(inlineTokInstruction.OpCode, member);

        }

        public override void VisitInlineTypeInstruction(InlineTypeInstruction inlineTypeInstruction)
        {
            Type type = inlineTypeInstruction.Type;
            Process(inlineTypeInstruction, type);
            ilGenerator.Emit(inlineTypeInstruction.OpCode, type);
        }

        public override void VisitInlineVarInstruction(InlineVarInstruction inlineVarInstruction)
        {
            Process(inlineVarInstruction, inlineVarInstruction.Ordinal);
            ilGenerator.Emit(inlineVarInstruction.OpCode, inlineVarInstruction.Ordinal);
        }

        public override void VisitShortInlineIInstruction(ShortInlineIInstruction shortInlineIInstruction)
        {
            Process(shortInlineIInstruction, shortInlineIInstruction.Byte);
            ilGenerator.Emit(shortInlineIInstruction.OpCode, shortInlineIInstruction.Byte);
        }

        public override void VisitShortInlineRInstruction(ShortInlineRInstruction shortInlineRInstruction)
        {
            Process(shortInlineRInstruction, shortInlineRInstruction.Single);
            ilGenerator.Emit(shortInlineRInstruction.OpCode, shortInlineRInstruction.Single);
        }

        public override void VisitShortInlineVarInstruction(ShortInlineVarInstruction shortInlineVarInstruction)
        {
            Process(shortInlineVarInstruction, shortInlineVarInstruction.Ordinal);
            ilGenerator.Emit(shortInlineVarInstruction.OpCode, shortInlineVarInstruction.Ordinal);
        }
        #endregion

        #region Helper Method
        public Label CreateLabel(int offset)
        {
            Label label;
            if (!labels.TryGetValue(offset, out label))
            {
                label = ilGenerator.DefineLabel();
                labels.Add(offset, label);
            }
            return label;
        }

        private void LoadReference(ILGenerator gen, int position)
        {
            if (position == -1)
            {
                throw new ProxyGenerationException("ArgumentReference unitialized");
            }
            switch (position)
            {
                case 0:
                    gen.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    gen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    gen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    gen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    gen.Emit(OpCodes.Ldarg_S, position);
                    break;
            }
        }

        private void StoreLocation(ILGenerator gen, int position)
        {
            if (position == -1)
            {
                throw new ProxyGenerationException("ArgumentReference unitialized");
            }
            switch (position)
            {
                case 0:
                    gen.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    gen.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    gen.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    gen.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    gen.Emit(OpCodes.Stloc_S, position);
                    break;
            }
        }


        private void LoadLocation(ILGenerator gen, int position)
        {
            if (position == -1)
            {
                throw new ProxyGenerationException("ArgumentReference unitialized");
            }
            switch (position)
            {
                case 0:
                    gen.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    gen.Emit(OpCodes.Ldloc_0);
                    break;
                case 2:
                    gen.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    gen.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    gen.Emit(OpCodes.Ldloc_S, position);
                    break;
            }
        }
        #endregion
    }
}
