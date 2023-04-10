using System;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using postcard.Models;
using Microsoft.Data.SqlClient;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage;
using Azure.Storage.Blobs.Specialized;
using System.IO;
using System.IO.Compression;

namespace postcard.Controllers
{
	public class StorageUtility
	{
        static BlobContainerClient blobContainer;

        public List<string> getstorageconnectionstring(IConfiguration configuration)
        {
#if DEBUG
            var connectionString = configuration.GetValue<string>("ConnectionStrings:AzureBlobStorageConnection");
            var blobContainername = configuration.GetValue<string>("StorageBlobs:ContainerName");
#else
            var connectionString = configuration.GetConnectionString("AzureBlobStorageConnection");
            var connectionString = configuration.GetConnectionString("ContainerName");
#endif
            var blobcontainerparams = new List<string>();

            blobcontainerparams.Add(connectionString);
            blobcontainerparams.Add(blobContainername);

            return blobcontainerparams;
        }

        public async Task<List<BlobContainer>> getcontainer(IConfiguration configuration)
        {
            try
            {
                var blobcontainerparams = getstorageconnectionstring(configuration);
                BlobServiceClient blobServiceClient = new BlobServiceClient(blobcontainerparams[0].ToString());
                blobContainer = blobServiceClient.GetBlobContainerClient(blobcontainerparams[1].ToString());

                await blobContainer.CreateIfNotExistsAsync(PublicAccessType.None);

                var container = new List<BlobContainer>();
                foreach (BlobItem blob in blobContainer.GetBlobs())
                {
                    if (blob.Properties.BlobType == BlobType.Block)
                    {
                        container.Add(new BlobContainer { URL = blobContainer.GetBlobClient(blob.Name).Uri.ToString(), name = blob.Name });
                    }
                }

                return container;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Stream> getstreamtodownload(IConfiguration configuration, string filename)
        {
            try
            {
                var blobcontainerparams = getstorageconnectionstring(configuration);
                BlobServiceClient blobServiceClient = new BlobServiceClient(blobcontainerparams[0].ToString());
                blobContainer = blobServiceClient.GetBlobContainerClient(blobcontainerparams[1].ToString());

                var blobBlockClient = blobContainer.GetBlockBlobClient(filename);

                Stream blobStream = blobBlockClient.OpenRead();

                return blobStream;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

