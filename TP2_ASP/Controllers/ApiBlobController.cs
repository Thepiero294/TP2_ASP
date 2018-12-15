using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TP2_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBlobController : ControllerBase
    {
        // GET: api/Blob
        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            BlobClass blob = new BlobClass();

            return blob.Get();


        }

        // GET: api/Blob/fileName
        [HttpGet("{name}", Name = "Get")]
        [AllowAnonymous]
        async public Task<FileStreamResult> Get(string name)
        {
            BlobClass blob = new BlobClass();

            MemoryStream stream = new MemoryStream();
            CloudBlockBlob cloudBlob = blob.Get(name);
            await cloudBlob.DownloadToStreamAsync(stream);
            stream.Position = 0;
            // fill stream from blob
            return new FileStreamResult(stream, cloudBlob.Properties.ContentType);

        }

        // POST: api/Blob
        [HttpPost]
        [Authorize]
        public async Task Post([FromBody] IFormFile file)
        {
            BlobClass blob = new BlobClass();

            try
            {
                await blob.Post(file);
            }
            catch
            {
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize]
        public void Delete(string name)
        {
            BlobClass blob = new BlobClass();

            try
            {
                blob.Delete(name);
            }
            catch
            {

            }
        }
    }
}