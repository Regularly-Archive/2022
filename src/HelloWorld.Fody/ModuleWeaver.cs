using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        private MethodInfo _writeLineMethod  => typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
        public override void Execute()
        {
            foreach (var type in ModuleDefinition.Types)
            {
                foreach (var method in type.Methods)
                {
                    var customerAttribute = method.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == nameof(HelloWorldAttribute));
                    if (customerAttribute != null)
                    {
                        ProcessMethod(method);
                    }
                }
            }
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "mscorlib";
            yield return "System";
        }

        private void ProcessMethod(MethodDefinition method)
        {
            // 获取当前方法体中的第一个 IL 指令
            var processor = method.Body.GetILProcessor();
            var current = method.Body.Instructions.First();

            // 插入一个 Nop 指令，表示什么都不做
            var first = Instruction.Create(OpCodes.Nop);
            processor.InsertBefore(current, first);
            current = first;

            // 构造 Console.WriteLine("Hello World")
            foreach (var instruction in GetInstructions(method))
            {
                processor.InsertAfter(current, instruction);
                current = instruction;
            }
        }

        private IEnumerable<Instruction> GetInstructions(MethodDefinition method)
        {
            yield return Instruction.Create(OpCodes.Nop);
            yield return Instruction.Create(OpCodes.Ldstr, "Hello World.");
            yield return Instruction.Create(OpCodes.Call, ModuleDefinition.ImportReference(_writeLineMethod));
        }
    }
}
