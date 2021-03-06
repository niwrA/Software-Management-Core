﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using FilesShared;
using niwrA.CommandManager;
using niwrA.CommandManager.Contracts;

// added for research as I came across it, not yet in use
namespace SoftwareManagementCoreApi.Controllers
{
  public class AllUploadedFiles
  {
    public List<FileDescriptionShort> FileShortDescriptions { get; set; }
  }
  public class FileDescriptionShort
  {
    public string Description { get; set; }
    public string Name { get; set; }
    public ICollection<IFormFile> Files { get; set; }
  }

  public class fileUpload
  {
    public Guid ForEntityGuid { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string FileName { get; set; }
    public string Url { get; set; }
    public IFormFile File { get; set; }
  }

  [EnableCors("SiteCorsPolicy")]
  [Route("api/[controller]")]
  [AllowAnonymous]
  public class FileUploadController : Controller
  {
    private int DefaultBufferSize = 80 * 1024;
    private ICommandManager _commandManager;
    private IFileService _fileService;

    // todo: inject fileio service
    public FileUploadController(ICommandManager commandManager, IFileService fileService)
    {
      _commandManager = commandManager;
      _fileService = fileService;

      var processorConfigs = new List<IProcessorConfig>
      {
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace: "FilesShared", entityRoot: "File", processor: fileService )
      };

      _commandManager.AddProcessorConfigs(processorConfigs);
    }
    // todo: move to injected class
    private async Task SaveAsync(IFormFile formFile, string folderName, string fileName, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (formFile == null)
      {
        throw new ArgumentNullException(nameof(formFile));
      }
      if (!Directory.Exists(folderName))
      {
        Directory.CreateDirectory(folderName);
      }
      var filePath = Path.Combine(folderName, fileName);
      using (var fileStream = new FileStream(filePath, FileMode.Create))
      {
        var inputStream = formFile.OpenReadStream();
        await inputStream.CopyToAsync(fileStream, DefaultBufferSize, cancellationToken);
      }
    }

    [Route("{forEntityType}/{forEntityGuid}")]
    [HttpPost]
    public async Task UploadFiles(fileUpload fileDescription, string forEntityType, Guid forEntityGuid)
    {
      var names = new List<string>();
      var contentTypes = new List<string>();
      var file = fileDescription.File;
      if (file.Length > 0)
      {
        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim().Value;
        contentTypes.Add(file.ContentType);
        fileName = fileName.Trim('"'); // https://github.com/aspnet/HttpAbstractions/issues/446
        names.Add(fileName);
        // todo: this logic needs to match the command's logic. Or do we replace
        // foldername with forEntityType in both?
        var folderName = forEntityType + @"/" + forEntityGuid;
        // todo: make a global setting for the uploads folder
        // hosting them should eventually be from the controller
        await SaveAsync(file, @"wwwroot/uploads/" + folderName, fileName);
        // in progress: moving this to the frontend, where it probably belongs
        // PostCreateFileCommand(forEntityGuid, fileName, folderName);
      }
    }

    private void PostCreateFileCommand(Guid forEntityGuid, string fileName, string folderName)
    {
      var type = System.IO.Path.GetExtension(fileName).ToLower();
      // todo: make a typed path (adjust commands to contain Entity name and auto-serialize the additional properties into ParametersJson)
      var commandDto = new CommandDto { Entity = "File", EntityGuid = Guid.NewGuid().ToString(), Guid = Guid.NewGuid(), CreatedOn = DateTime.Now, Command = "Create" };
      commandDto.ParametersJson = $@"{{'ForGuid':'{forEntityGuid}', 'Name': '{fileName}', 'FileName': '{fileName}', 'FolderName':'{folderName}', 'Type':'{type}'}}";
      _commandManager.ProcessCommands( new List<CommandDto> { commandDto });
      _fileService.PersistChanges();
      _commandManager.PersistChanges();
    }

    //[Route("download/{id}")]
    //[HttpGet]
    //public FileStreamResult Download(int id)
    //{
    // todo: CQRS => read section?
    //var fileDescription = _fileRepository.GetFileDescription(id);

    //var path = "" + "\\" + fileDescription.FileName;
    //var stream = new FileStream(path, FileMode.Open);
    //return File(stream, fileDescription.ContentType);
    //}
  }
}
