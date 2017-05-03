using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeGen;
using Moq;
using CodeGenShared;
using System.IO;
using System.Text;

namespace CodeGenCoreTests
{
    [TestClass]
    public class CSharpUpdaterTests
    {
        [TestMethod]
        public void CanAddProperty()
        {
            var sut = new CSharpUpdater();

            var settings = new CodeGenSettings();
            var documentMock = new Mock<ICustomDocument>();
            var interfaceMock = new Mock<ICustomInterface>();

            var solutionRoot = @"c:\";
            var testInterface = "public interface ITest {};";
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(testInterface));

            documentMock.Setup(s => s.GetStream(solutionRoot)).Returns(memoryStream);
                
            settings.Documents.Add(documentMock.Object);
            settings.Interfaces.Add(interfaceMock.Object);
            sut.loadSettings(settings);
            
            sut.AddProperty("testName", "string", "test", "tests");

            documentMock.Verify(s => s.CreateIfNotExisting("test", "tests", solutionRoot), Times.Once);
            documentMock.Verify(s => s.Update(solutionRoot, It.Is<string>(r => r.Contains("string testName"))));
        }
    }
}
