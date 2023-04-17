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
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;
using Azure;

namespace postcard.Controllers
{
	public class StorageUtility
    {
        static BlobContainerClient blobContainer;

        public List<string> getstorageconnectionstring(IConfiguration configuration)
        {
            var blobContainername = configuration.GetValue<string>("StorageBlobs:ContainerName");
            var tableName = configuration.GetValue<string>("StorageBlobs:TableName");
            var queueName = configuration.GetValue<string>("StorageBlobs:QueueName");
            var fileshareName = configuration.GetValue<string>("StorageBlobs:FileShareName");

//#if DEBUG
            var connectionString = configuration.GetValue<string>("ConnectionStrings:AzureBlobStorageConnection");
//#else
//            var connectionString = configuration.GetConnectionString("AzureBlobStorageConnection");
//#endif
            var blobcontainerparams = new List<string>();

            blobcontainerparams.Add(connectionString);
            blobcontainerparams.Add(blobContainername);
            blobcontainerparams.Add(tableName);
            blobcontainerparams.Add(queueName);
            blobcontainerparams.Add(fileshareName);

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

        public async Task<TableClient> gettable(IConfiguration configuration)
        {
            var blobcontainerparams = getstorageconnectionstring(configuration);
            var serviceClient = new TableServiceClient(blobcontainerparams[0].ToString());
            var tableClient = serviceClient.GetTableClient(blobcontainerparams[2].ToString());

            await tableClient.CreateIfNotExistsAsync();

            return tableClient;
        }

        public async Task<FeatureToggle> getfeaturetoggle(IConfiguration configuration, string uf, string estado)
        {
            var tableClient = await gettable(configuration);

            return await tableClient.GetEntityAsync<FeatureToggle>(uf, estado);
        }

        public void SendMessageToQueue(IConfiguration configuration, string message)
        {
            var blobcontainerparams = getstorageconnectionstring(configuration);
            string connectionString = blobcontainerparams[0].ToString();
            var queueName = blobcontainerparams[3].ToString();

            var queueClient = new QueueClient(connectionString, queueName, new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
            queueClient.CreateIfNotExists();
            if (queueClient.Exists())
            {
                queueClient.SendMessage(message);
            }
        }

        public void uploadfiletoshare(IConfiguration configuration, string filename, FileStream stream)
        {
            var blobcontainerparams = getstorageconnectionstring(configuration);
            ShareClient share = new ShareClient(blobcontainerparams[0].ToString(), blobcontainerparams[4].ToString());

            var directory = share.GetDirectoryClient("");
            var file = directory.GetFileClient(filename);

            file.Create(stream.Length);
            stream.Position = 0;
            file.UploadRange(new HttpRange(0, stream.Length), stream);
        }
    }
}

