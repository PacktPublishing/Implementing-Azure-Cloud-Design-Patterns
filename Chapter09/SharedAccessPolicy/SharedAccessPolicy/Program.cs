using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAccessPolicy
{
    class Program
    {
        static void Main(string[] args)
        {
            //Read the Storage Account connection string from App.config
            CloudStorageAccount storageAccount =
    CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureStorageConnectionString"));

            //Create the blob service client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Reference to storage account container
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("demo");
            blobContainer.CreateIfNotExists();

            //Generate a Shared Access Signature URI for the storage account Blob container                 
            Console.WriteLine("Generated Container SAS URI: " +
                             GenerateBlobContainerSASUri(blobContainer));

            //Shared Access Signature generation for a specific blob inside a Blob Storage container
            //Console.WriteLine("Blob specific SAS URI: " + GenerateBlobSASUri(blobContainer));

            Console.ReadLine();
        }

        //Shared Access Signature generation
        static string GenerateBlobContainerSASUri(CloudBlobContainer blobContainer)
        {
            //SAS policy with constraints and time-limit setup
            SharedAccessBlobPolicy sasPolicy = new SharedAccessBlobPolicy();
            sasPolicy.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(12);
            sasPolicy.Permissions = SharedAccessBlobPermissions.List |
                                         SharedAccessBlobPermissions.Write;

            //SAS token creation
            string sasToken = blobContainer.GetSharedAccessSignature(sasPolicy);

            //Final SAS URI
            return blobContainer.Uri + sasToken;
        }

        static string GenerateBlobSASUri(CloudBlobContainer blobContainer)
        {
            //Sets a reference to a blob inside the container in input
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference("demo.txt");

            //Creates a blob file (or updates it if existing)
            string blobContent = "Demo Blob file created for Packt Publishing.";
            blob.UploadText(blobContent);

            //SAS policy setup for the blob file (permissions and time-limits)
            SharedAccessBlobPolicy sasPolicy = new SharedAccessBlobPolicy();
            sasPolicy.SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-3);
            sasPolicy.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(12);
            sasPolicy.Permissions = SharedAccessBlobPermissions.Read |
                                         SharedAccessBlobPermissions.Write;
            //SAS Token generation
            string sasToken = blob.GetSharedAccessSignature(sasPolicy);

            //Final SAS URI
            return blob.Uri + sasToken;
        }

        //Function for testing the usage of a SAS key for a blob
        static void TestBlobSAS(string SASuri)
        {
            //Gets a blob reference from the SAS uri
            CloudBlockBlob blob = new CloudBlockBlob(new Uri(SASuri));

            //WRITE operation test
            try
            {
                string blobContent = "Write test with SAS token for Packt.";
                MemoryStream msWrite = new MemoryStream(Encoding.UTF8.GetBytes(blobContent));
                msWrite.Position = 0;
                using (msWrite)
                {
                    blob.UploadFromStream(msWrite);
                }
                Console.WriteLine("Write operation completed successfully");
            }
            catch (StorageException e)
            {
                Console.WriteLine("Write operation failed: " + e.Message);
            }

            //READ operation test
            try
            {
                MemoryStream msRead = new MemoryStream();
                using (msRead)
                {
                    blob.DownloadToStream(msRead);
                    msRead.Position = 0;
                    using (StreamReader reader = new StreamReader(msRead, true))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
                Console.WriteLine("Read operation completed successfully.");
            }
            catch (StorageException e)
            {
                Console.WriteLine("Read operation failed: " + e.Message);
            }

            //DELETE operation test
            try
            {
                blob.Delete();
                Console.WriteLine("Delete operation completed successfully");
            }
            catch (StorageException e)
            {
                Console.WriteLine("Delete operation failed: " + e.Message);
            }
        }

    }
}
