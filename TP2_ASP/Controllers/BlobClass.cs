using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace TP2_ASP.Controllers
{
    public class BlobClass
    {
        // GET: api/Blob
        public List<string> Get()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            List<string> blobs = new List<string>();
            BlobResultSegment resultSegment = container.ListBlobsSegmentedAsync(null).Result;
            foreach (IListBlobItem item in resultSegment.Results)
            {
                CloudBlockBlob blob = (CloudBlockBlob)item;
                blobs.Add(blob.Name);
            }

            return blobs;
        }


        // GET: api/Blob/fileName
        public CloudBlockBlob Get(string name)
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
                        return blob;
                    }
                }
            }
            return null;
        }

        // POST: api/Blob

        // POST: api/Blob
        async public Task Post([FromBody] IFormFile file)
        {
            // CloudBlobContainer container = GetCloudBlobContainer();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot Configuration = builder.Build();
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(Configuration["ConnectionStrings:AzureStorageConnectionString-1"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("blob-image-exer7");

            // va chercher la reference vers le blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

            // Creer ou ecrase le blob
            using (var fileStream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }
        }

        // DELETE: api/ApiWithActions/5
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
