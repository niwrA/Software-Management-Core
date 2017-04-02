using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Threading;

// added for research as I came across it, not yet in use
namespace SoftwareManagementCoreApi.Controllers
{
    public class AllUploadedFiles
    {
        public List<FileDescriptionShort> FileShortDescriptions { get; set; }
    }
    public class FileDescriptionShort
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public ICollection<IFormFile> File { get; set; }
    }

    public class FileUploadController : Controller
    {
        private int DefaultBufferSize = 80 * 1024;

        public FileUploadController()
        {

        }
        // todo: move to injected class
        private async Task SaveAsync(IFormFile formFile, string filename, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (formFile == null)
            {
                throw new ArgumentNullException(nameof(formFile));
            }

            using (var fileStream = new FileStream(filename, FileMode.Create))
            {
                var inputStream = formFile.OpenReadStream();
                await inputStream.CopyToAsync(fileStream, DefaultBufferSize, cancellationToken);
            }
        }

        [Route("files")]
        [HttpPost]
        public async Task UploadFiles(FileDescriptionShort fileDescriptionShort)
        {
            var names = new List<string>();
            var contentTypes = new List<string>();
            if (ModelState.IsValid)
            {
                // http://www.mikesdotnetting.com/article/288/asp-net-5-uploading-files-with-asp-net-mvc-6
                // http://dotnetthoughts.net/file-upload-in-asp-net-5-and-mvc-6/
                foreach (var file in fileDescriptionShort.File)
                {
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        contentTypes.Add(file.ContentType);

                        names.Add(fileName);
                        
                        await SaveAsync(file, Path.Combine("", fileName));
                    }
                }
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
