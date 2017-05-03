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
    public interface ICodeGenService : ICommandProcessor
    {
        void loadSettings(CodeGenSettings settings);
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
    public interface ICustomProperty
    {
        string Name { get; set; }
        string TypeName { get; set; }
    }
    public class UpdateProperty : UpdateActionBase
    {
        public CustomProperty CustomProperty { get; set; }
    }
    public class UpdateMethod : UpdateActionBase
    {
        public CustomMethod CustomMethod { get; set; }
    }
    public class CustomProperty : ICustomProperty
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
    }
    public class CustomMethod
    {
        public CustomMethod()
        {
            Parameters = new List<ICustomParameter>();
        }
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public IList<ICustomParameter> Parameters { get; set; }
    }
    public interface ICustomParameter
    {
        string Name { get; set; }
        string ValueType { get; set; }
        string InputType { get; set; }
    }
    public class CustomParameter: ICustomParameter
    {
        public string Name { get; set; }
        public string ValueType { get; set; }
        public string InputType { get; set; }
    }
    public interface ICustomDocument
    {
        string Name { get; set; }
        bool IsCreateIfNotExisting { get; set; }
        string TemplateEntityName { get; set; }
        string TemplateEntitiesName { get; set; }
        bool HasChanged { get; set; }
        Stream GetStream(string solutionRoot);
        void CreateIfNotExisting(string entityName, string entitiesName, string solutionRoot);
        void Update(string solutionRoot, string content);
        string UpdatedContent { get; set; }
    }
    public class CustomDocument : ICustomDocument
    {
        public string Name { get; set; }
        public bool IsCreateIfNotExisting { get; set; }
        public string TemplateEntityName { get; set; }
        public string TemplateEntitiesName { get; set; }
        public bool HasChanged { get; set; }

        public Stream GetStream(string solutionRoot)
        {
            string path = Path.Combine(solutionRoot, Name);
            if (File.Exists(path))
            {
                return File.OpenRead(path);
            }
            return null;
        }
        public void CreateIfNotExisting(string entityName, string entitiesName, string solutionRoot)
        {
            var path = Path.Combine(solutionRoot, Name);
            if (!File.Exists(path))
            {
                Console.WriteLine($"{path} not found.");
                var templatePath = Path.Combine(solutionRoot, Name);
                if (File.Exists(templatePath))
                {
                    // File.Copy(templatePath, path, false);
                    var textReplaced = "";
                    using (var stream = File.OpenRead(templatePath))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var text = reader.ReadToEnd();
                            //string entitiesName = updateActions.First().EntitiesName;
                            textReplaced = text.Replace(TemplateEntitiesName, entitiesName);
                            Console.WriteLine($"Replaced {TemplateEntitiesName} with {entitiesName}");
                            //string entityName = updateActions.First().EntityName;
                            textReplaced = textReplaced.Replace(TemplateEntityName, entityName);
                            Console.WriteLine($"Replaced {TemplateEntityName} with {entityName}");
                        }
                    }
                    // todo: move to 'PersistChanges()'?
                    if (!string.IsNullOrWhiteSpace(textReplaced))
                    {
                        try
                        {
                            string fullPath = Path.GetDirectoryName(path);
                            Directory.CreateDirectory(fullPath);
                            using (var stream = File.OpenWrite(path))
                            {
                                using (var writer = new StreamWriter(stream))
                                {
                                    writer.Write(textReplaced);
                                }
                            };
                            Console.WriteLine($"{path} created from {templatePath}.");
                        }
                        catch (UnauthorizedAccessException UAEx)
                        {
                            Console.WriteLine($"Could not create file at {path}.");
                            Console.WriteLine(UAEx.Message);
                        }
                    }
                }
            }
        }

        public void Update(string solutionRoot, string content)
        {
            UpdatedContent = content;
            // todo: move to 'PersistChanges()'?
            var path = Path.Combine(solutionRoot, Name);
            File.WriteAllText(path, content, Encoding.UTF8); // todo: use original encoding?
        }
        public string UpdatedContent { get; set; }
    }
    public class CodeGenSettings
    {
        public CodeGenSettings()
        {
            Documents = new List<ICustomDocument>();
            Interfaces = new List<ICustomInterface>();
            Classes = new List<ICustomClass>();
        }
        public CodeGenSettings(string entityName, string entitiesName)
        {
            // todo: make settings file. These are development / test settings
            Documents = new List<ICustomDocument>();
            Documents.Add(new CustomDocument { Name = $@"{entitiesName}Shared\{entitiesName}.cs" });
            Documents.Add(new CustomDocument { Name = $@"SoftwareManagementCoreApi\Controllers\{entitiesName}Controller.cs" });
            Documents.Add(new CustomDocument { Name = $@"SoftwareManagementEventSourcedRepository\MainEventSourceRepository.cs" });
            Documents.Add(new CustomDocument { Name = $@"SoftwareManagementMongoDbCoreRepository\MainRepository.cs" });
            Documents.Add(new CustomDocument { Name = $@"SoftwareManagementEFCoreRepository\MainRepository.cs" });
            Documents.Add(new CustomDocument { Name = $@"SoftwareManagementCoreTests\{entitiesName}\Fakes\{entityName}State.cs" });

            // todo: make 'clean' template entity, or get it from the UI
            foreach (var document in Documents)
            {
                document.TemplateEntityName = "Contact";
                document.TemplateEntitiesName = "Contact";
            }

            Interfaces = new List<ICustomInterface>();
            Interfaces.Add(new CustomInterface { Name = $"I{entityName}" });
            Interfaces.Add(new CustomInterface { Name = $"I{entityName}State" });

            Classes = new List<ICustomClass>();
            Classes.Add(new CustomClass { Name = $"{entityName}" });
            Classes.Add(new CustomClass { Name = $"{entityName}State" });
            Classes.Add(new CustomClass { Name = $"{entityName}Dto" });
        }
        // todo: this is user and machine specific! either update in repository or neatly find on this machine
        // somehow. Or perhaps a command line needs to update the solution from the solution folder?
//        public string SolutionRoot = @"C:\PROJECTS\Software-Management-Core\";
        public string SolutionRoot = @"C:\Users\Arwin\Documents\GitHub\Software-Management-Core";
        public string TemplateEntityName = "Company";
        public string TemplateEntitiesName = "Companies";
        public IList<ICustomDocument> Documents { get; set; }
        public IList<ICustomInterface> Interfaces { get; set; }
        public IList<ICustomClass> Classes { get; set; }
    }
    public interface ICustomClass
    {
        string Name { get; set; }
        bool IsState { get; }
    }
    public class CustomClass : ICustomClass
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
    public interface ICustomInterface
    {
        string Name { get; set; }
        bool IsState { get; }
    }
    public class CustomInterface : ICustomInterface
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

