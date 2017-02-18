using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using ClrTest.Reflection;
using System.Reflection;
using System.IO;
namespace ILReader.Tests
{
    public class Readfrom_ILReader
    {
        [Fact]
        public void Readable_SampleMethod()
        {
            MethodInfo method = GetType().GetMethod("SampleMethod", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var memoryStream = new MemoryStream(500);
            var streamWriter = new StreamWriter(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            var v = new ReadableILStringVisitor(new ReadableILStringToTextWriter(streamWriter));
            var reader = new ClrTest.Reflection.ILReader(method);
            reader.Accept(v);
            streamWriter.Flush();

            memoryStream.Seek(0, SeekOrigin.Begin);
            string line = string.Empty;

            string[] values = new[]{
            "IL_0000: nop",        
            "IL_0001: ldc.i4.s   19",
            "IL_0003: stloc.0",    
            "IL_0004: ldc.i4.s   23",
            "IL_0006: stloc.1",    
            "IL_0007: ldloc.0",    
            "IL_0008: ldloc.1",    
            "IL_0009: add",        
            "IL_000a: stloc.2",    
            "IL_000b: br.s       IL_000d",
            "IL_000d: ldloc.2",    
            "IL_000e: ret"
            };

            foreach (string value in values)
                Assert.Contains(value, streamReader.ReadLine(), StringComparison.InvariantCulture);
        }

        [Fact]
        public void Raw_SampleMethod()
        {
            MethodInfo method = GetType().GetMethod("SampleMethod", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var memoryStream = new MemoryStream(500);
            var streamWriter = new StreamWriter(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            var v = new ReadableILStringVisitor(new RawILStringToTextWriter(streamWriter));
            var reader = new ClrTest.Reflection.ILReader(method);
            reader.Accept(v);
            streamWriter.Flush();

            memoryStream.Seek(0, SeekOrigin.Begin);
            string line = string.Empty;

            string[] values = new[]{
                "IL_0000: 00  |",         
                "IL_0001: 1f  | 19",      
                "IL_0003: 0a  |",         
                "IL_0004: 1f  | 23",      
                "IL_0006: 0b  |",         
                "IL_0007: 06  |",         
                "IL_0008: 07  |",         
                "IL_0009: 58  |",         
                "IL_000a: 0c  |",         
                "IL_000b: 2b  | IL_000d", 
                "IL_000d: 08  |",         
                "IL_000e: 2a  |"};

            foreach (string value in values)
                Assert.Contains(value, streamReader.ReadLine(), StringComparison.InvariantCulture);
        }

        [Fact]
        public void Readable_ComplexMethod()
        {
            MethodInfo method = GetType().GetMethod("ComplexMethod", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var memoryStream = new MemoryStream(500);
            var streamWriter = new StreamWriter(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            var v = new ReadableILStringVisitor(new ReadableILStringToTextWriter(streamWriter));
            var reader = new ClrTest.Reflection.ILReader(method);
            reader.Accept(v);
            streamWriter.Flush();

            memoryStream.Seek(0, SeekOrigin.Begin);
            string line = string.Empty;

            string[] values = new[]{
                "IL_0000: nop",        
                "IL_0001: ldstr      \"Hello world\"",
                "IL_0006: call       Void WriteLine(System.String)/System.Console",
                "IL_000b: nop",        
                "IL_000c: ldarg.0",    
                "IL_000d: call       Int32 SampleMethod()/ILReader.Tests.Readfrom_ILReader",
                "IL_0012: pop",        
                "IL_0013: ret"};

            foreach (string value in values)
                Assert.Contains(value, streamReader.ReadLine(), StringComparison.InvariantCulture);

            //while ((line = streamReader.ReadLine()) != null)
            //{
            //    Console.WriteLine(line);
            //}
        }


        [Fact]
        public void Raw_ComplexMethod()
        {
            MethodInfo method = GetType().GetMethod("ComplexMethod", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var memoryStream = new MemoryStream(500);
            var streamWriter = new StreamWriter(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            var v = new ReadableILStringVisitor(new RawILStringToTextWriter(streamWriter));
            var reader = new ClrTest.Reflection.ILReader(method);
            reader.Accept(v);
            streamWriter.Flush();

            memoryStream.Seek(0, SeekOrigin.Begin);
            string line = string.Empty;

            string[] values = new[]{            
                "IL_0000: 00  |",
                "IL_0001: 72  | \"Hello world\"",
                "IL_0006: 28  | Void WriteLine(System.String)/System.Console",
                "IL_000b: 00  |",         
                "IL_000c: 02  |",         
                "IL_000d: 28  | Int32 SampleMethod()/ILReader.Tests.Readfrom_ILReader",
                "IL_0012: 26  |",         
                "IL_0013: 2a  |"};

            foreach (string value in values)
                Assert.Contains(value, streamReader.ReadLine(), StringComparison.InvariantCulture);

            //while ((line = streamReader.ReadLine()) != null)
            //{
            //    Console.WriteLine(line);
            //}
        }

        private int SampleMethod()
        {
            int nineteen = 19;
            int twentyThree = 23;
            return nineteen + twentyThree;
        }

        private void ComplexMethod()
        {
            Console.WriteLine("Hello world");
            SampleMethod();
        }
    }
}
