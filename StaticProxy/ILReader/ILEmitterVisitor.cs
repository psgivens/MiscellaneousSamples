using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace ClrTest.Reflection
{
    public class ILEmitterVisitor : ILInstructionVisitor
    {
        private readonly IILBranchManager collector;
        private readonly ILGenerator ilGenerator;

        public ILEmitterVisitor(MethodBuilder methodBuilder)
            : this(methodBuilder, methodBuilder.GetILGenerator())
        {
        }

        public ILEmitterVisitor(MethodBuilder methodBuilder, ILGenerator ilGenerator)
        {
            this.ilGenerator = ilGenerator;
            this.collector = new ILEmitCollector(ilGenerator);
            Type returnType = methodBuilder.ReturnType;
            if (returnType != typeof(void))
                ilGenerator.DeclareLocal(returnType);
        }

        public override void VisitInlineBrTargetInstruction(InlineBrTargetInstruction inlineBrTargetInstruction)
        {
            collector.Process(inlineBrTargetInstruction, inlineBrTargetInstruction.TargetOffset);
            ilGenerator.Emit(inlineBrTargetInstruction.OpCode, inlineBrTargetInstruction.TargetOffset);
        }

        public override void VisitInlineFieldInstruction(InlineFieldInstruction inlineFieldInstruction)
        {
            FieldInfo field = inlineFieldInstruction.Field;
            collector.Process(inlineFieldInstruction, field);
            ilGenerator.Emit(inlineFieldInstruction.OpCode, field);
        }

        public override void VisitInlineIInstruction(InlineIInstruction inlineIInstruction)
        {
            collector.Process(inlineIInstruction, inlineIInstruction.Int32);
            ilGenerator.Emit(inlineIInstruction.OpCode, inlineIInstruction.Int32);
        }

        public override void VisitInlineI8Instruction(InlineI8Instruction inlineI8Instruction)
        {
            collector.Process(inlineI8Instruction, inlineI8Instruction.Int64);
            ilGenerator.Emit(inlineI8Instruction.OpCode, inlineI8Instruction.Int64);
        }

        public override void VisitInlineMethodInstruction(InlineMethodInstruction inlineMethodInstruction)
        {
            MethodBase method = inlineMethodInstruction.Method;
            collector.Process(inlineMethodInstruction, method);
            ilGenerator.Emit(inlineMethodInstruction.OpCode, (MethodInfo)method);
        }

        public override void VisitInlineNoneInstruction(InlineNoneInstruction inlineNoneInstruction)
        {
            collector.Process(inlineNoneInstruction, null);
            ilGenerator.Emit(inlineNoneInstruction.OpCode);
        }

        public override void VisitInlineRInstruction(InlineRInstruction inlineRInstruction)
        {
            collector.Process(inlineRInstruction, inlineRInstruction.Double);
            ilGenerator.Emit(inlineRInstruction.OpCode, inlineRInstruction.Double);
        }

        public override void VisitInlineSigInstruction(InlineSigInstruction inlineSigInstruction)
        {
            collector.Process(inlineSigInstruction, inlineSigInstruction.Signature);
            //ilGenerator.Emit(inlineSigInstruction.OpCode, inlineSigInstruction.Signature);
            System.Diagnostics.Debugger.Break();
        }

        public override void VisitInlineStringInstruction(InlineStringInstruction inlineStringInstruction)
        {
            collector.Process(inlineStringInstruction, inlineStringInstruction.String);
            ilGenerator.Emit(inlineStringInstruction.OpCode, inlineStringInstruction.String);
        }

        public override void VisitInlineSwitchInstruction(InlineSwitchInstruction inlineSwitchInstruction)
        {
            collector.Process(inlineSwitchInstruction, inlineSwitchInstruction.TargetOffsets);
            //ilGenerator.Emit(inlineSwitchInstruction.OpCode, inlineSwitchInstruction.TargetOffsets);
            System.Diagnostics.Debugger.Break();
        }

        public override void VisitInlineTokInstruction(InlineTokInstruction inlineTokInstruction)
        {
            MemberInfo member = inlineTokInstruction.Member;
            collector.Process(inlineTokInstruction, member);
            //ilGenerator.Emit(inlineTokInstruction.OpCode, member);
            System.Diagnostics.Debugger.Break();
        }

        public override void VisitInlineTypeInstruction(InlineTypeInstruction inlineTypeInstruction)
        {
            Type type = inlineTypeInstruction.Type;
            collector.Process(inlineTypeInstruction, type);
            ilGenerator.Emit(inlineTypeInstruction.OpCode, type);
        }

        public override void VisitInlineVarInstruction(InlineVarInstruction inlineVarInstruction)
        {
            collector.Process(inlineVarInstruction, inlineVarInstruction.Ordinal);
            ilGenerator.Emit(inlineVarInstruction.OpCode, inlineVarInstruction.Ordinal);
        }

        public override void VisitShortInlineBrTargetInstruction(ShortInlineBrTargetInstruction shortInlineBrTargetInstruction)
        {
            collector.Process(shortInlineBrTargetInstruction, shortInlineBrTargetInstruction.TargetOffset);
            ilGenerator.Emit(shortInlineBrTargetInstruction.OpCode, collector.CreateLabel(shortInlineBrTargetInstruction.TargetOffset));
        }

        public override void VisitShortInlineIInstruction(ShortInlineIInstruction shortInlineIInstruction)
        {
            collector.Process(shortInlineIInstruction, shortInlineIInstruction.Byte);
            ilGenerator.Emit(shortInlineIInstruction.OpCode, shortInlineIInstruction.Byte);
        }

        public override void VisitShortInlineRInstruction(ShortInlineRInstruction shortInlineRInstruction)
        {
            collector.Process(shortInlineRInstruction, shortInlineRInstruction.Single);
            ilGenerator.Emit(shortInlineRInstruction.OpCode, shortInlineRInstruction.Single);
        }

        public override void VisitShortInlineVarInstruction(ShortInlineVarInstruction shortInlineVarInstruction)
        {
            collector.Process(shortInlineVarInstruction, shortInlineVarInstruction.Ordinal);
            ilGenerator.Emit(shortInlineVarInstruction.OpCode, shortInlineVarInstruction.Ordinal);
        }
    }

    public interface IILBranchManager
    {
        Label CreateLabel(int offset);
        void Process(ILInstruction ilInstruction, object operand);
    }

    public class ILEmitCollector : IILBranchManager
    {
        private readonly Dictionary<int, Label> labels
            = new Dictionary<int, Label>();
        private readonly ILGenerator ilGenerator;

        public ILEmitCollector(ILGenerator ilGenerator)
        {
            this.ilGenerator = ilGenerator;
        }

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

        public virtual void Process(ILInstruction ilInstruction, object operand)
        {
            Label label;
            int offset = ilInstruction.Offset;
            if (labels.TryGetValue(offset, out label))
            {
                ilGenerator.MarkLabel(label);
                labels.Remove(offset);
            }
        }
    }
}
