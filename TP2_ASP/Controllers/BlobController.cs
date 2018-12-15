using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TP2_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class BlobController : ControllerBase
    {
        // GET: api/Blob
        [HttpGet]
        [Authorize]
        public IEnumerable<IListBlobItem> Get()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            List<IListBlobItem> blobs = new List<IListBlobItem>();
            BlobResultSegment resultSegment = container.ListBlobsSegmentedAsync(null).Result;
            foreach (IListBlobItem item in resultSegment.Results)
            {
                blobs.Add(item);
            }

            return blobs;
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

        //[HttpPost]
        //public async Task Post([FromForm]IFormFile asset)
        //{
        //    CloudStorageAccount storageAccount = null;
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json");
        //    IConfigurationRoot Configuration = builder.Build();
        //    if (CloudStorageAccount.TryParse(Configuration["ConnectionStrings:AzureStorageConnectionString-1"], out storageAccount))
        //    {
        //        var client = storageAccount.CreateCloudBlobClient();
        //        var container = client.GetContainerReference("blob-image-exer7");
        //        await container.CreateIfNotExistsAsync();

        //        var blob = await container.GetBlobReferenceFromServerAsync(asset.FileName);
        //        await blob.UploadFromStreamAsync(asset.OpenReadStream());

        //    }

        //}


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize]
        public void Delete(string name)
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(name);
            blob.DeleteAsync().Wait();
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot Configuration = builder.Build();
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(Configuration["ConnectionStrings:AzureStorageConnectionString-1"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("blob-image-exer7");
            return container;
        }
    }
}