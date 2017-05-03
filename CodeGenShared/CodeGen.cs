using CommandsShared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// todo: warp all CodeAnalysis in an Interface
namespace CodeGenShared
{
    public interface IUpdateAction
    {
        string EntityName { get; set; }
        string EntitiesName { get; set; }
    }
    public interface ICodeGenService: ICommandProcessor
    {
        void loadSettings(Settings settings);
        void ProcessActions(IEnumerable<IUpdateAction> updateActions);
        void AddProperty(string name, string typeName, string entityName, string entitiesName);
    }
    public class UpdateActionBase : IUpdateAction
    {
        private string _entityName;
        public string EntityName
        {
            get { return _entityName; }
            set
            {
                _entityName = value;
                Pluralize(value);
            }
        }

        private void Pluralize(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.EndsWith("y"))
                {
                    EntitiesName = value.Substring(0, value.Length - 1) + "ies";
                }
                else
                {
                    EntitiesName = value + 's';
                }
            }
        }

        public string EntitiesName { get; set; }
    }
    public class UpdateProperty : UpdateActionBase
    {
        public CustomProperty CustomProperty { get; set; }
    }
    public class UpdateMethod : UpdateActionBase
    {
        public CustomMethod CustomMethod { get; set; }
    }
    public class CustomProperty
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
    }
    public class CustomMethod
    {
        public CustomMethod()
        {
            Parameters = new List<CustomParameter>();
        }
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public IList<CustomParameter> Parameters { get; set; }
    }
    public class CustomParameter
    {
        public string Name { get; set; }
        public string ValueType { get; set; }
        public string InputType { get; set; }
    }
    public class CustomDocument
    {
        public string Name { get; set; }
        public bool CreateIfNonExisting { get; set; }
        public string TemplateName { get; set; }
    }
    public class Settings
    {
        public Settings(string entityName, string entitiesName)
        {
            Documents = new List<CustomDocument>();
            Documents.Add(new CustomDocument { Name = $@"{entitiesName}Shared\{entitiesName}.cs" });
            Documents.Add(new CustomDocument { Name = $@"SoftwareManagementCoreApi\Controllers\{entitiesName}Controller.cs" });
            Documents.Add(new CustomDocument { Name = $@"SoftwareManagementEventSourcedRepository\MainEventSourceRepository.cs" });
            Documents.Add(new CustomDocument { Name = $@"SoftwareManagementMongoDbCoreRepository\MainRepository.cs" });
            Documents.Add(new CustomDocument { Name = $@"SoftwareManagementEFCoreRepository\MainRepository.cs" });
            Documents.Add(new CustomDocument { Name = $@"SoftwareManagementCoreTests\{entitiesName}\Fakes\{entityName}State.cs" });

            Interfaces = new List<CustomInterface>();
            Interfaces.Add(new CustomInterface { Name = $"I{entityName}" });
            Interfaces.Add(new CustomInterface { Name = $"I{entityName}State" });

            Classes = new List<CustomClass>();
            Classes.Add(new CustomClass { Name = $"{entityName}" });
            Classes.Add(new CustomClass { Name = $"{entityName}State" });
            Classes.Add(new CustomClass { Name = $"{entityName}Dto" });
        }

        public string SolutionRoot = @"C:\PROJECTS\Software-Management-Core\";
        public string TemplateEntityName = "Company";
        public string TemplateEntitiesName = "Companies";
        public List<CustomDocument> Documents { get; set; }
        public IList<CustomInterface> Interfaces { get; set; }
        public IList<CustomClass> Classes { get; set; }
    }

    public class CustomClass
    {
        public string Name { get; set; }
        public bool IsState
        {
            get
            {
                return Name.EndsWith("State") || Name.EndsWith("Dto");
            }
        }
    }

    public class CustomInterface
    {
        public string Name { get; set; }
        public bool IsState
        {
            get
            {
                return Name.EndsWith("State");
            }
        }
    }

    public class CodeGen
    {
        public CodeGen(ICodeGenService updater)
        {
            _updater = updater;
        }
        private ICodeGenService _updater;
        public void ProcessActions(IEnumerable<IUpdateAction> updateActions)
        {
            _updater.ProcessActions(updateActions);
        }
    }
}

