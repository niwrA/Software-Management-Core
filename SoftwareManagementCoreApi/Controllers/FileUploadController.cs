using System;
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

        public FileUploadController()
        {

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
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                contentTypes.Add(file.ContentType);

                names.Add(fileName);
                var folderName = @"uploads/" + forEntityType + @"/" + forEntityGuid;
                await SaveAsync(file, folderName, fileName);
                // todo: post command for saving a file record to the database
                var createFileCommand = new CreateFileCommand();
                createFileCommand.ForGuid = forEntityGuid;
                createFileCommand.EntityGuid = Guid.NewGuid();
                createFileCommand.Name = fileName;
                createFileCommand.FolderName = folderName;
            }
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
