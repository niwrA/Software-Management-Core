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
  public interface IFileIO
  {
    string GetContent(string path);
    Stream GetStream(string path);
    void Save(string path, string content);
    bool Exists(string path);
  }
  public class FileIO : IFileIO
  {
    public FileIO(string solutionRoot)
    {
      SolutionRoot = solutionRoot;
    }

    public string SolutionRoot { get; private set; }

    public string GetContent(string path)
    {
      if (File.Exists(path))
      {
        return File.ReadAllText(path);
      }
      return null;
    }

    public Stream GetStream(string path)
    {
      if (File.Exists(path))
      {
        return File.OpenRead(path);
      }
      return null;
    }
    public bool Exists(string path)
    {
      return File.Exists(path);
    }

    public void Save(string path, string content)
    {
      if (!string.IsNullOrWhiteSpace(content))
      {
        try
        {
          string fullPath = Path.GetDirectoryName(path);
          Directory.CreateDirectory(fullPath);
          File.WriteAllText(path, content, Encoding.UTF8); // todo: use original encoding?
                                                           //using (var stream = File.OpenWrite(path))
                                                           //{
                                                           //    File.WriteAllText(path, content, Encoding.UTF8); // todo: use original encoding?
                                                           //    using (var writer = new StreamWriter(stream))
                                                           //    {
                                                           //        writer.Write(content,);
                                                           //    }
                                                           //};
          Console.WriteLine($"Saved to {path}.");
        }
        catch (UnauthorizedAccessException UAEx)
        {
          Console.WriteLine($"Could not create file at {path}.");
          Console.WriteLine(UAEx.Message);
        }
      }
    }
  }
  public interface IUpdateAction
  {
    string EntityName { get; set; }
    string EntitiesName { get; set; }
    string RootEntitiesName { get; set; }
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
    private string _rootEntitiesName;
    public string EntityName
    {
      get { return _entityName; }
      set
      {
        _entityName = value;
        Pluralize(value);
      }
    }
    public string RootEntitiesName
    {
      get { return _rootEntitiesName; }
      set
      {
        _rootEntitiesName = value;
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
  public class CustomParameter : ICustomParameter
  {
    public string Name { get; set; }
    public string ValueType { get; set; }
    public string InputType { get; set; }
  }
  public interface ICustomDocument
  {
    string Name { get; set; }
    string NameTemplate { get; set; }
    string Body { get; set; }
    bool IsCreateIfNotExisting { get; set; }
    string TemplateName { get; set; }
    string TemplateEntityName { get; set; }
    string TemplateEntitiesName { get; set; }
    string TemplateRootEntitiesName { get; set; }
    bool HasChanged { get; set; }
    Stream GetStream(string solutionRoot);
    bool CreateIfNotExisting(string rootEntitiessName, string entityName, string entitiesName, string solutionRoot);
    void Update(string solutionRoot, string content);
    bool IsRootOnly { get; set; }
  }
  public class CustomDocument : ICustomDocument
  {
    private IFileIO _fileIO;

    public CustomDocument(IFileIO fileIO)
    {
      _fileIO = fileIO;
      Body = _fileIO.GetContent(Name);
    }
    public string Name { get; set; }
    public string TemplateName { get; set; }
    public string Body { get; set; }
    public bool IsCreateIfNotExisting { get; set; } = true;
    public string TemplateEntityName { get; set; }
    public string TemplateEntitiesName { get; set; }
    public string TemplateRootEntitiesName { get; set; }
    public bool HasChanged { get; set; }
    public string NameTemplate { get; set; }
    public bool IsRootOnly { get; set; } = false;

    public Stream GetStream(string solutionRoot)
    {
      string path = Path.Combine(solutionRoot, Name);
      return _fileIO.GetStream(path);
    }

    public bool CreateIfNotExisting(string rootEntitiesName, string entityName, string entitiesName, string solutionRoot)
    {
      var path = Path.Combine(solutionRoot, Name);
      if (!_fileIO.Exists(path))
      {
        Console.WriteLine($"{path} not found.");
        var templatePath = Path.Combine(solutionRoot, TemplateName);
        var text = _fileIO.GetContent(templatePath);
        // File.Copy(templatePath, path, false);
        var textReplaced = "";
        //string entitiesName = updateActions.First().EntitiesName;
        textReplaced = text.Replace(TemplateEntitiesName, entitiesName);
        Console.WriteLine($"Replaced {TemplateEntitiesName} with {entitiesName}");
        //string entityName = updateActions.First().EntityName;
        textReplaced = textReplaced.Replace(TemplateEntityName, entityName);
        Console.WriteLine($"Replaced {TemplateEntityName} with {entityName}");
        // todo: move to 'PersistChanges()'?
        textReplaced = textReplaced.Replace(TemplateRootEntitiesName, rootEntitiesName);
        Console.WriteLine($"Replaced {TemplateRootEntitiesName} with {rootEntitiesName}");

        Body = textReplaced;
        return true;
      }
      return false;
    }

    public void Update(string solutionRoot, string content)
    {
      Body = content;
      // todo: move to 'PersistChanges()'?
      var path = Path.Combine(solutionRoot, Name);
      _fileIO.Save(path, Body);
    }
  }
  public class CodeGenSettings
  {
    public CodeGenSettings()
    {
      Documents = new List<ICustomDocument>();
      Interfaces = new List<ICustomInterface>();
      Classes = new List<ICustomClass>();
    }
    public CodeGenSettings(string rootEntitiesName, string entityName, string entitiesName)
    {
      var fileIO = new FileIO(@"C:\PROJECTS\Software-Management-Core");
      // todo: make settings file. These are development / test settings
      Documents = new List<ICustomDocument>();
      Documents.Add(new CustomDocument(fileIO) { NameTemplate = @"{rootEntitiesName}Shared\{entitiesName}.cs" });
      Documents.Add(new CustomDocument(fileIO) { NameTemplate = @"{rootEntitiesName}Shared\{entityName}Commands.cs" });
      Documents.Add(new CustomDocument(fileIO) { NameTemplate = @"SoftwareManagementCoreApi\Controllers\{entitiesName}Controller.cs", IsRootOnly = true });
      Documents.Add(new CustomDocument(fileIO) { NameTemplate = @"SoftwareManagementEventSourcedRepository\MainEventSourceRepository.cs" });
      Documents.Add(new CustomDocument(fileIO) { NameTemplate = @"SoftwareManagementMongoDbCoreRepository\{entityName}StateRepository.cs", IsRootOnly = true });
      Documents.Add(new CustomDocument(fileIO) { NameTemplate = @"SoftwareManagementMongoDbCoreRepositoryTests\{entityName}StateRepositoryTests.cs", IsRootOnly = true });
      Documents.Add(new CustomDocument(fileIO) { NameTemplate = @"SoftwareManagementEFCoreRepository\MainRepository.cs" });
      Documents.Add(new CustomDocument(fileIO) { NameTemplate = @"SoftwareManagementCoreTests\{entitiesName}\Fakes\{entityName}State.cs" });
      Documents.Add(new CustomDocument(fileIO) { NameTemplate = @"SoftwareManagementCoreTests\{entitiesName}\{entityName}CommandsTests.cs" });
      Documents.Add(new CustomDocument(fileIO) { NameTemplate = @"SoftwareManagementCoreTests\{entitiesName}\{entitiesName}Tests.cs" });

      // todo: make 'clean' template entity, or get it from the UI
      foreach (var document in Documents)
      {
        document.Name = GetDocumentName(document, rootEntitiesName, entityName, entitiesName);
        document.TemplateEntityName = this.TemplateEntityName;
        document.TemplateEntitiesName = this.TemplateEntitiesName;
        document.TemplateName = GetDocumentName(document, TemplateRootEntitiesName, this.TemplateEntityName, this.TemplateEntitiesName);
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

    // todo: factor out this public property and use the one in fileIO
    public string SolutionRoot = @"C:\development\SoftwareManagement\Software-Management-Core";
    public string TemplateEntityName = "Link";
    public string TemplateEntitiesName = "Links";
    public string TemplateRootEntitiesName = "Links";
    public IList<ICustomDocument> Documents { get; set; }
    public IList<ICustomInterface> Interfaces { get; set; }
    public IList<ICustomClass> Classes { get; set; }
    private string GetDocumentName(ICustomDocument doc, string rootEntitiesName, string entityName, string entitiesName)
    {
      var document = doc.NameTemplate.Replace(@"{entityName}", entityName);
      document = document.Replace(@"{entitiesName}", entitiesName);
      document = document.Replace(@"{rootEntitiesName}", rootEntitiesName);
      return document;
    }
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

