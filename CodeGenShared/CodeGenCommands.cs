using CodeGen;
using CommandsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenShared
{
    public abstract class PropertyCodeGenCommand : CommandBase
    {
        public PropertyCodeGenCommand() : base() { }
//        public PropertyCodeGenCommand(ICommandStateRepository repo) : base(repo) { }
    }

    public class CreatePropertyCodeGenCommand : PropertyCodeGenCommand
    {
        public CreatePropertyCodeGenCommand() : base() { }
//        public CreatePropertyCodeGenCommand(ICommandStateRepository repo) : base(repo) { }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string EntityName { get; set; }
        public string EntitiesName { get; set; }
        public override void Execute()
        {
            ICodeGenService codeGenService = ((ICodeGenService)base.CommandProcessor);
            // todo: configurable externally?
            codeGenService.loadSettings(new CodeGenSettings(this.EntityName, this.EntitiesName));
            codeGenService.AddProperty(this.Name, this.TypeName, this.EntityName, this.EntitiesName);
            base.Execute();
        }
    }
}
