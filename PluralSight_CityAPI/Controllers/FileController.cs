using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace PluralSight_CityAPI.Controllers
{
    [Route("api/files")]
    [Authorize]
    [ApiController]
    public class FileController : ControllerBase
    {
        // Inject Content Type Services
        // readonly modifier: Only get value at declaration or in constructor
        private readonly FileExtensionContentTypeProvider _fileContentType; 

        public FileController(FileExtensionContentTypeProvider fileContentType)
        {
            // Assign injected fileContentType if not null. If fileContentType is null, throw an error
            _fileContentType = fileContentType ?? throw new System.ArgumentNullException(nameof(fileContentType));
        }

        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId) {
            string pathFile = "FileTest.txt";

            // Check exist
            if (!System.IO.File.Exists(pathFile))
            {
                return NotFound();
            }

            // Check and retrieve type
            if (!_fileContentType.TryGetContentType(pathFile, out var contentType)) {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(pathFile);
            return File(bytes, contentType, Path.GetFileName(pathFile));
        }
    }
}
