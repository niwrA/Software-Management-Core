using CodeGen;
using Moq;
using CodeGenShared;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace CodeGenCoreTests
{
  [Category("CodeGen.CSharp")]
  [TestFixture]
  public class CSharpUpdaterTests
  {
    [Test]
    public void CanAddPropertyToInterface()
    {
      var sut = new CSharpUpdater();

      var settingsBuilder = new SettingsBuilder();
      var settings = settingsBuilder.WithInterface().Build();

      sut.LoadSettings(settings);

      sut.AddProperty("testName", "string", "test", "tests");

      settingsBuilder.DocumentMock.VerifySet(s => s.HasChanged = true, Times.AtLeastOnce);
      settingsBuilder.DocumentMock.Verify(s => s.CreateIfNotExisting("roots", "test", "tests", settings.SolutionRoot), Times.Never);
      settingsBuilder.DocumentMock.Verify(s => s.Update(settings.SolutionRoot, It.Is<string>(r => r.Contains("string testName"))));
    }

    [Test]
    public void CanAddPropertyToClass()
    {
      var sut = new CSharpUpdater();

      var settingsBuilder = new SettingsBuilder();
      var settings = settingsBuilder.WithClass().Build();

      sut.LoadSettings(settings);

      sut.AddProperty("testName", "string", "test", "tests");

      settingsBuilder.DocumentMock.VerifySet(s => s.HasChanged = true, Times.AtLeastOnce);
      settingsBuilder.DocumentMock.Verify(s => s.CreateIfNotExisting("roots", "test", "tests", settings.SolutionRoot), Times.Never);
      settingsBuilder.DocumentMock.Verify(s => s.Update(settings.SolutionRoot, It.Is<string>(r => r.Contains("string testName"))));
    }

    [Test]
    public void CanAddMethodToInterface()
    {
      var sut = new CSharpUpdater();

      var settingsBuilder = new SettingsBuilder();
      var settings = settingsBuilder.WithInterface().Build();

      sut.LoadSettings(settings);

      var parameter = new CustomParameter { Name = "param1", ValueType = "string" };
      var parameters = new List<ICustomParameter> { parameter };
      sut.AddMethod("testName", "bool", parameters, "test", "tests");

      settingsBuilder.DocumentMock.VerifySet(s => s.HasChanged = true, Times.AtLeastOnce);
      settingsBuilder.DocumentMock.Verify(s => s.CreateIfNotExisting("roots", "test", "tests", settings.SolutionRoot), Times.Never);
      settingsBuilder.DocumentMock.Verify(s => s.Update(settings.SolutionRoot, It.Is<string>(r => r.Contains("bool testName(string param1)"))));
    }

    [Test]
    public void CanAddMethodToClass()
    {
      var sut = new CSharpUpdater();

      var settingsBuilder = new SettingsBuilder();
      var settings = settingsBuilder.WithClass().Build();

      sut.LoadSettings(settings);

      var parameter = new CustomParameter { Name = "param1", ValueType = "string" };
      var parameters = new List<ICustomParameter> { parameter };
      sut.AddMethod("testName", "bool", parameters, "test", "tests");

      settingsBuilder.DocumentMock.VerifySet(s => s.HasChanged = true, Times.AtLeastOnce);
      settingsBuilder.DocumentMock.Verify(s => s.CreateIfNotExisting("roots", "test", "tests", settings.SolutionRoot), Times.Never);
      settingsBuilder.DocumentMock.Verify(s => s.Update(settings.SolutionRoot, It.Is<string>(r => r.Contains("bool testName(string param1)"))));
    }
  }

  public class SettingsBuilder
  {
    private string _testFileContents;

    public Mock<ICustomDocument> DocumentMock { get; private set; } = new Mock<ICustomDocument>();
    public Mock<ICustomInterface> InterfaceMock { get; private set; }
    public Mock<ICustomClass> ClassMock { get; private set; }
    public CodeGenSettings Settings { get; private set; } = new CodeGenSettings();

    public CodeGenSettings Build()
    {
      Settings.SolutionRoot = @"c:\";
      var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(_testFileContents));

      DocumentMock.Setup(s => s.GetStream(Settings.SolutionRoot)).Returns(memoryStream);
      DocumentMock.Setup(s => s.HasChanged).Returns(true);
      DocumentMock.Setup(s => s.Name).Returns("Tests.cs");

      Settings.Documents.Add(DocumentMock.Object);
      return Settings;
    }

    public SettingsBuilder WithInterface()
    {
      InterfaceMock = new Mock<ICustomInterface>();
      _testFileContents += "\n" + @"public interface ITest {};";
      InterfaceMock.Setup(s => s.Name).Returns("ITest");
      Settings.Interfaces.Add(InterfaceMock.Object);
      return this;
    }

    public SettingsBuilder WithClass()
    {
      ClassMock = new Mock<ICustomClass>();
      _testFileContents += "\n" + @"public class Test {};";
      ClassMock.Setup(s => s.Name).Returns("Test");
      Settings.Classes.Add(ClassMock.Object);
      return this;
    }
  }
}
