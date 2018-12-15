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
        [Authorize]
        public FileStreamResult Get(string name)
        {
            CloudBlobContainer container = GetCloudBlobContainer();

            // va chercher la reference vers le blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);

            BlobResultSegment resultSegment = container.ListBlobsSegmentedAsync(null).Result;
            foreach (IListBlobItem blobItem in resultSegment.Results)
            {
                if (blobItem.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)blobItem;
                    if (blob.Name == name)
                    {
                        Stream stream = new MemoryStream();

                        blob.DownloadToStreamAsync(stream);
                        stream.Position = 0;

                        return new FileStreamResult(stream, blob.Properties.ContentType);
                    }
                }
            }
            return null;
        }

        // POST: api/Blob
        [HttpPost]
        [Authorize]
        async public Task Post([FromBody] IFormFile file)
        {
            CloudBlobContainer container = GetCloudBlobContainer();

            // va chercher la reference vers le blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

            // Creer ou ecrase le blob
            using (var fileStream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
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