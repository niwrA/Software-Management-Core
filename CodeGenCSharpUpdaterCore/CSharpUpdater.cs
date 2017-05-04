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
using CodeGenShared;
namespace CodeGen
{
    public class CSharpUpdater : ICodeGenService
    {
        private CodeGenSettings _settings;
        // todo: load settings
        public void loadSettings(CodeGenSettings settings)
        {
            _settings = settings;
        }
        public void ProcessActions(IEnumerable<IUpdateAction> updateActions)
        {
            // current setup requires all actions to target the same Entity
            var solutionRoot = _settings.SolutionRoot;
            if (_settings == null)
            {
                _settings = new CodeGenSettings(updateActions.First().EntityName, updateActions.First().EntitiesName);
            }
            foreach (var doc in _settings.Documents)
            {
                string path = GetDocumentPath(updateActions, solutionRoot, doc);
                var entitiesName = updateActions.First().EntitiesName;
                var entityName = updateActions.First().EntityName;
                CreateIfNecessary(entityName, entitiesName, solutionRoot, doc, path);
                UpdateAndSave(updateActions, path);
            }
        }

        // todo: extract file-io to interface
        private void UpdateAndSave(IEnumerable<IUpdateAction> updateActions, string path)
        {
            SyntaxNode newRoot;
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path);
                    newRoot = syntaxTree.GetRoot();
                    UpdateInterfaces(updateActions, ref newRoot);
                    UpdateClasses(updateActions, ref newRoot);
                }
                if (newRoot != null)
                {
                    File.WriteAllText(path, newRoot.ToFullString());
                }
            }
        }

        private void CreateIfNecessary(string entityName, string entitiesName, string solutionRoot, ICustomDocument doc, string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"{path} not found.");
                var templatePath = GetDocumentTemplatePath(solutionRoot, doc, _settings.TemplateEntityName, _settings.TemplateEntitiesName);
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
                            textReplaced = text.Replace(_settings.TemplateEntitiesName, entitiesName);
                            Console.WriteLine($"Replaced {_settings.TemplateEntitiesName} with {entitiesName}");
                            //string entityName = updateActions.First().EntityName;
                            textReplaced = textReplaced.Replace(_settings.TemplateEntityName, entityName);
                            Console.WriteLine($"Replaced {_settings.TemplateEntityName} with {entityName}");
                        }
                    }
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

        private string GetDocumentPath(IEnumerable<IUpdateAction> updateActions, string solutionRoot, ICustomDocument doc)
        {
            var document = doc.Name.Replace(@"{EntityName}", updateActions.First().EntityName);
            document = document.Replace(@"{EntitiesName}", updateActions.First().EntitiesName);
            var path = Path.Combine(solutionRoot, document);
            return path;
        }
        private string GetDocumentTemplatePath(string solutionRoot, ICustomDocument doc, string templateEntityName, string templateEntitiesName)
        {
            var document = doc.Name.Replace(@"{EntityName}", templateEntityName);
            document = document.Replace(@"{EntitiesName}", templateEntitiesName);
            var path = Path.Combine(solutionRoot, document);
            return path;
        }

        public void AddProperty(string name, string typeName, string entityName, string entitiesName)
        {
            var solutionRoot = _settings.SolutionRoot;
            foreach (var doc in _settings.Documents)
            {
                var hasChanged = false;
                SyntaxNode newRoot = null;
                string path = Path.Combine(solutionRoot, doc.Name);
                doc.CreateIfNotExisting(entityName, entitiesName, solutionRoot);
                //                CreateIfNecessary(entityName, entitiesName, solutionRoot, doc, path);
                using (var stream = doc.GetStream(solutionRoot))
                {
                    if (stream != null)
                    {
                        var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path);
                        newRoot = syntaxTree.GetRoot();
                        newRoot = AddPropertyToInterfaces(_settings.Interfaces, name, typeName, newRoot, ref hasChanged);
                        newRoot = AddPropertyToClasses(_settings.Classes, name, typeName, newRoot, ref hasChanged);
                    }
                }
                // todo: move save to document class?
                if (newRoot != null && hasChanged)
                {
                    doc.HasChanged = hasChanged;
                    doc.Update(solutionRoot, newRoot.ToFullString());
                }
            }
        }

        public void AddMethod(string name, string typeName, IList<ICustomParameter> customParameters, string entityName, string entitiesName)
        {
            var solutionRoot = _settings.SolutionRoot;
            foreach (var doc in _settings.Documents)
            {
                var hasChanged = false;
                SyntaxNode newRoot = null;
                string path = Path.Combine(solutionRoot, doc.Name);
                doc.CreateIfNotExisting(entityName, entitiesName, solutionRoot);
                using (var stream = doc.GetStream(solutionRoot))
                {
                    if (stream != null)
                    {
                        var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path);
                        newRoot = syntaxTree.GetRoot();
                        newRoot = AddMethodToInterfaces(_settings.Interfaces, name, typeName, customParameters, newRoot, ref hasChanged);
                        newRoot = AddMethodToClasses(_settings.Classes, name, typeName, customParameters, newRoot, ref hasChanged);
                    }
                }
                if (newRoot != null && hasChanged)
                {
                    doc.HasChanged = hasChanged;
                    string content = newRoot.ToFullString();
                    doc.Update(solutionRoot, content);
                }
            }
        }


        private SyntaxNode AddPropertyToClasses(IEnumerable<ICustomClass> customClasses, string name, string typeName, SyntaxNode newRoot, ref bool hasChanged)
        {
            foreach (var customClass in customClasses)
            {
                var updateClass = GetClass(customClass.Name, ref newRoot);
                if (updateClass != null)
                {
                    var updatedClass = AddPropertyToClass(updateClass, name, typeName, customClass.IsState);
                    if (updatedClass != null)
                    {
                        hasChanged = true;
                        newRoot = newRoot.ReplaceNode(updateClass, updatedClass);
                        updateClass = updatedClass;
                    }
                }
            }

            return newRoot;
        }

        private SyntaxNode AddPropertyToInterfaces(IEnumerable<ICustomInterface> customInterfaces, string name, string typeName, SyntaxNode newRoot, ref bool hasChanged)
        {
            foreach (var customInterface in customInterfaces)
            {
                var updateInterface = GetInterface(customInterface.Name, ref newRoot);
                if (updateInterface != null)
                {
                    var updatedInterface = AddPropertyToInterface(updateInterface, name, typeName, customInterface.IsState);
                    if (updatedInterface != null)
                    {
                        hasChanged = true;
                        newRoot = newRoot.ReplaceNode(updateInterface, updatedInterface);
                        updateInterface = updatedInterface;
                    }
                }
            }

            return newRoot;
        }
        private SyntaxNode AddMethodToClasses(IEnumerable<ICustomClass> customClasses, string name, string typeName, IList<ICustomParameter> customParameters, SyntaxNode newRoot, ref bool hasChanged)
        {
            var updateMethod = new UpdateMethod { CustomMethod = new CustomMethod { Name = name, ReturnType = typeName, Parameters = customParameters } };
            foreach (var customClass in customClasses)
            {
                var updateClass = GetClass(customClass.Name, ref newRoot);
                var updatedClass = AddMethodToClass(updateClass, updateMethod);
                if (updatedClass != null)
                {
                    hasChanged = true;
                    newRoot = newRoot.ReplaceNode(updateClass, updatedClass);
                    updateClass = updatedClass;
                }
            }

            return newRoot;
        }
        private SyntaxNode AddMethodToInterfaces(IEnumerable<ICustomInterface> customInterfaces, string name, string returnType, IList<ICustomParameter> parameters, SyntaxNode newRoot, ref bool hasChanged)
        {
            var updateMethod = new UpdateMethod { CustomMethod = new CustomMethod { Name = name, Parameters = parameters, ReturnType = returnType } };
            foreach (var customInterface in customInterfaces)
            {
                var updateInterface = GetInterface(customInterface.Name, ref newRoot);
                var updatedInterface = AddMethodToInterface(updateInterface, updateMethod);
                if (updatedInterface != null)
                {
                    hasChanged = true;
                    newRoot = newRoot.ReplaceNode(updateInterface, updatedInterface);
                    updateInterface = updatedInterface;
                }
            }

            return newRoot;
        }

        private void UpdateInterfaces(IEnumerable<IUpdateAction> updateActions, ref SyntaxNode newRoot)
        {
            // todo: reverse interfacesToUpdate loop with updateAction loop?
            foreach (var updateAction in updateActions)
            {
                var interfacesToUpdate = new List<string> { "I" + updateAction.EntityName + "State", "I" + updateAction.EntityName };

                foreach (var interfaceName in interfacesToUpdate)
                {
                    var updateInterface = GetInterface(interfaceName, ref newRoot);
                    var updatedInterface = updateInterface;
                    if (updateInterface != null)
                    {
                        var readOnly = !interfaceName.EndsWith("State") && !interfaceName.EndsWith("Dto");
                        if (updateAction is UpdateProperty updateProperty)
                        {
                            updatedInterface = AddPropertyToInterface(updatedInterface, updateProperty.CustomProperty.Name, updateProperty.CustomProperty.TypeName, readOnly);
                        }
                        else if (updateAction is UpdateMethod updateMethod && readOnly)
                        {
                            updatedInterface = AddMethodToInterface(updatedInterface, updateMethod);
                        }
                    }
                    if (updatedInterface != null)
                    {
                        newRoot = newRoot.ReplaceNode(updateInterface, updatedInterface);
                        updateInterface = updatedInterface;
                    }
                }
            }
        }

        private void UpdateClasses(IEnumerable<IUpdateAction> updateActions, ref SyntaxNode newRoot)
        {
            // todo: revert loops
            foreach (var updateAction in updateActions)
            {
                var classesToUpdate = new List<string> { updateAction.EntityName, updateAction.EntityName + "State", updateAction.EntityName + "Dto" };

                foreach (var className in classesToUpdate)
                {
                    var updateClass = GetClass(className, ref newRoot);
                    var updatedClass = updateClass;
                    if (updateClass != null)
                    {
                        // Open the solution within the workspace.
                        var isStateClass = className.EndsWith("State") || className.EndsWith("Dto");
                        if (updateAction is UpdateProperty)
                        {
                            var updateProperty = (UpdateProperty)updateAction;
                            updatedClass = AddPropertyToClass(updatedClass, updateProperty.CustomProperty.Name, updateProperty.CustomProperty.TypeName, isStateClass);
                        }
                        else if (updateAction is UpdateMethod && !isStateClass)
                        {
                            var updateMethod = (UpdateMethod)updateAction;
                            updatedClass = AddMethodToClass(updatedClass, updateMethod);
                        }
                    }
                    if (updatedClass != null)
                    {
                        newRoot = newRoot.ReplaceNode(updateClass, updatedClass);
                    }
                }
            }
        }

        private ClassDeclarationSyntax GetClass(string className, ref SyntaxNode newRoot)
        {
            var nameSpace = newRoot.ChildNodes().FirstOrDefault(node => node.IsKind(SyntaxKind.NamespaceDeclaration));
            var searchRoot = nameSpace ?? newRoot;
            var classDeclarations = searchRoot.ChildNodes().OfType<ClassDeclarationSyntax>();
            var updateClass = classDeclarations.SingleOrDefault(s => s.Identifier.Text == className);

            return updateClass;
        }

        private InterfaceDeclarationSyntax GetInterface(string interfaceName, ref SyntaxNode newRoot)
        {
            var nameSpace = newRoot.ChildNodes().FirstOrDefault(node => node.IsKind(SyntaxKind.NamespaceDeclaration));
            var searchRoot = nameSpace ?? newRoot;
            var interfaceDeclarations = searchRoot.ChildNodes().OfType<InterfaceDeclarationSyntax>();
            var updateInterface = interfaceDeclarations.SingleOrDefault(s => s.Identifier.Text == interfaceName);

            return updateInterface;
        }

        private InterfaceDeclarationSyntax AddPropertyToInterface(InterfaceDeclarationSyntax updateInterface, string propertyName, string typeName, bool readOnly)
        {
            var newInterfaceDeclaration = updateInterface;
            var propertyDeclarations = newInterfaceDeclaration.ChildNodes().OfType<PropertyDeclarationSyntax>();
            var updateProperty = propertyDeclarations.SingleOrDefault(s => s.Identifier.Text == propertyName);
            if (updateProperty == null)
            {
                var propertyToInsert = GetPropertyDeclarationSyntax(typeName: typeName,
                  propertyName: propertyName, readOnly: readOnly, asPublic: false, isStateClass: false);
                var oldInterfaceDeclaration = newInterfaceDeclaration;
                newInterfaceDeclaration = newInterfaceDeclaration.AddMembers(propertyToInsert.NormalizeWhitespace());
                // newRoot = newRoot.ReplaceNode(oldInterfaceDeclaration, newInterfaceDeclaration);

                Console.WriteLine($"Property {propertyName} added to interface with type {typeName}!");
                return newInterfaceDeclaration;
            }
            return null;
        }

        private ClassDeclarationSyntax AddPropertyToClass(ClassDeclarationSyntax updateClass, string propertyName, string typeName, bool isStateClass)
        {
            var propertyDeclarations = updateClass.ChildNodes().OfType<PropertyDeclarationSyntax>();
            var updateProperty = propertyDeclarations.SingleOrDefault(s => s.Identifier.Text == propertyName);
            if (updateProperty == null)
            {
                var propertyToInsert = GetPropertyDeclarationSyntax(typeName: typeName,
                  propertyName: propertyName, readOnly: true, asPublic: true, isStateClass: isStateClass);
                var updatedClass = updateClass.AddMembers(propertyToInsert.NormalizeWhitespace());
                // newRoot = newRoot.ReplaceNode(updateClass, updatedClass);
                Console.WriteLine($"Property {propertyName} added to class with type {typeName}!");
                return updatedClass;
            }
            return null;
        }

        private InterfaceDeclarationSyntax AddMethodToInterface(InterfaceDeclarationSyntax updateInterface, UpdateMethod updateMethod)
        {
            var methodName = updateMethod.CustomMethod.Name;

            var methodDeclarations = updateInterface.ChildNodes().OfType<MethodDeclarationSyntax>();
            var methodToUpdate = methodDeclarations.SingleOrDefault(s => s.Identifier.Text == methodName);
            var parameterTypes = updateMethod.CustomMethod.Parameters.Select(s => s.ValueType).ToList();
            var parameterNames = updateMethod.CustomMethod.Parameters.Select(s => s.Name).ToList();

            if (methodToUpdate == null)
            {
                var methodToInsert = GetMethodDeclarationSyntax(returnTypeName: updateMethod.CustomMethod.ReturnType,
                  methodName: methodName,
                  parameterTypes: parameterTypes.ToArray(),
                  parameterNames: parameterNames.ToArray());
                var updatedInterface = updateInterface.AddMembers(methodToInsert);

                // newRoot = newRoot.ReplaceNode(updateInterface, updatedInterface);
                Console.WriteLine($"Method {methodName} added to interface");
                return updatedInterface;
            }
            return null;
        }
        private ClassDeclarationSyntax AddMethodToClass(ClassDeclarationSyntax updateClass, UpdateMethod updateMethod)
        {
            var methodName = updateMethod.CustomMethod.Name;
            var methodDeclarations = updateClass.ChildNodes().OfType<MethodDeclarationSyntax>();
            var methodToUpdate = methodDeclarations.SingleOrDefault(s => s.Identifier.Text == methodName);
            var parameterTypes = updateMethod.CustomMethod.Parameters.Select(s => s.ValueType).ToList();
            var parameterNames = updateMethod.CustomMethod.Parameters.Select(s => s.Name).ToList();

            if (methodToUpdate == null)
            {
                var methodToInsert = GetMethodDeclarationSyntax(returnTypeName: updateMethod.CustomMethod.ReturnType,
                  methodName: methodName,
                  parameterTypes: parameterTypes.ToArray(),
                  parameterNames: parameterNames.ToArray());
                var updatedClass = updateClass.AddMembers(methodToInsert);

                // newRoot = newRoot.ReplaceNode(updateInterface, updatedInterface);
                Console.WriteLine($"Method {methodName} added to class");
                return updatedClass;
            }
            return null;
        }


        public PropertyDeclarationSyntax GetPropertyDeclarationSyntax(string typeName, string propertyName, bool readOnly, bool asPublic, bool isStateClass)
        {
            var declaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(typeName), propertyName);

            var accessorList = SyntaxFactory.AccessorList();
            var accessors = new List<AccessorDeclarationSyntax>();
            var get = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
            if (asPublic && !isStateClass)
            {
                var statement = SyntaxFactory.ParseStatement(@"return _state." + propertyName + ";");
                var block = SyntaxFactory.Block(new List<StatementSyntax> { statement });
                get = get.WithBody(block);
            }
            else
            {
                get = get.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }
            accessors.Add(get);
            if (!readOnly || isStateClass)
            {
                var set = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                accessors.Add(set);
            }
            var accessorsArray = accessors.ToArray();
            accessorList = accessorList.AddAccessors(accessorsArray);
            declaration = declaration.WithAccessorList(accessorList);
            if (asPublic)
            {
                declaration = declaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            }
            return declaration;
        }
        public MethodDeclarationSyntax GetMethodDeclarationSyntax(string returnTypeName, string methodName, string[] parameterTypes, string[] parameterNames)
        {
            var parameterList = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(GetParametersList(parameterTypes, parameterNames)));
            var declaration = SyntaxFactory.MethodDeclaration(attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                          modifiers: SyntaxFactory.TokenList(),
                          returnType: SyntaxFactory.ParseTypeName(returnTypeName),
                          explicitInterfaceSpecifier: null,
                          identifier: SyntaxFactory.Identifier(methodName),
                          typeParameterList: null,
                          parameterList: parameterList,
                          constraintClauses: SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
                          body: null,
                          semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            //.WithAdditionalAnnotations(Formatter.Annotation);

            declaration = declaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            return declaration.NormalizeWhitespace();
        }

        private IEnumerable<ParameterSyntax> GetParametersList(string[] parameterTypes, string[] paramterNames)
        {
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                yield return SyntaxFactory.Parameter(attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                                                         modifiers: SyntaxFactory.TokenList(),
                                                         type: SyntaxFactory.ParseTypeName(parameterTypes[i]),
                                                         identifier: SyntaxFactory.Identifier(paramterNames[i]),
                                                         @default: null);
            }
        }
    }
}
