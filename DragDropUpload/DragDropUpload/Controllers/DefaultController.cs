using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragDropUpload.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        public DefaultController(IHostingEnvironment _hostingEnvironment)
        {
            this.hostingEnvironment = _hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFileAsync(List<IFormFile> files)
        {
            string uploadFiles = $"{this.hostingEnvironment.WebRootPath}\\uploads";
            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                string newfileName = "";
                for (int i = 1; i < fileName.Length - 1; i++)
                    newfileName += fileName[i];
                string extension = Path.GetExtension(newfileName);
                newfileName = GetFileName(file) + extension;
                if (fileName.Contains("\\"))
                    fileName.Trim('"').Substring(fileName.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                else fileName.Trim('"');

                string fullPath = Path.Combine(uploadFiles, newfileName);
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return this.Ok();
        }

        public string GetFileName(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                StringBuilder builder = new StringBuilder();
                file.CopyTo(stream);
                byte[] fileBytes = stream.ToArray();
                var fileHash = GetMD5Hash(fileBytes);
                for (int i = 0; i < fileHash.Length; i++)
                {
                    builder.Append(fileHash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public byte[] GetMD5Hash(byte[] input)
        {
            byte[] fileHash;
            using (SHA1 sha256Hasher = SHA1.Create())
            {
                fileHash = sha256Hasher.ComputeHash(input);
            }
            return fileHash;
        }
    }
}