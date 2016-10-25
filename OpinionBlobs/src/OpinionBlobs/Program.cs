using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types


namespace OpinionBlobs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // http://127.0.0.1:10000/<account-name>/<resource-path>


            // <add key="StorageConnectionString" value="DefaultEndpointsProtocol=https;AccountName=storagesample;AccountKey=nYV0gln6fT7mvY+rxu2iWAEyzPKITGkhM88J8HUoyofvK7C6fHcZc2kRZp6cKgYRUM74lHI84L50Iau1+9hPjB==" />

            // To target the storage emulator, you can use a shortcut
            // that maps to the well-known account name and key.
            string connString = "UseDevelopmentStorage=true;";

            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            //    CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            //container.SetPermissions(
            //  new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@"C:\Users\atrifono\Pictures\20161023 Salmon\IMG_0015.JPG"))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;

                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;

                    Console.WriteLine("Directory: {0}", directory.Uri);
                }
            }

            // Retrieve reference to a blob named "photo1.jpg".
            //CloudBlockBlob blockBlob = container.GetBlockBlobReference("photo1.jpg");

            // Save blob contents to a file.
            using (var fileStream = System.IO.File.OpenWrite(@"C:\Users\atrifono\Pictures\20161023 Salmon\z.JPG"))
            {
                blockBlob.DownloadToStream(fileStream);
            }


            Console.WriteLine("Done.");

            //// Retrieve reference to a blob named "myblob.txt".
            //CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob.txt");

            //// Delete the blob.
            //blockBlob.Delete();

            Console.ReadLine();
             

        }
    }
}

