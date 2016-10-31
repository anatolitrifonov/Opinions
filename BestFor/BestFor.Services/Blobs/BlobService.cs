using BestFor.Common;
using BestFor.Dto;
using Microsoft.Extensions.Options;
using BestFor.Services.Cache;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestFor.Services.Blobs
{
    /// <summary>
    /// Contains the code that saves the file to blob.
    /// Contains the logic that forms the path for files per entity type.
    /// Entity type is passed through the method name.
    /// 
    /// All files are stored in one container until we know better.
    /// </summary>
    public class BlobService : IBlobService
    {
        private const string USER_IMAGES_PATH = "user_";

        private string _blobServiceConnectionString;

        private string _blobServiceContainerName;

        /// <summary>
        /// Injected cache manager.
        /// </summary>
        private ICacheManager _cacheManager;

        public BlobService(IOptions<AppSettings> appSettings, ICacheManager cacheManager)
        {
            _blobServiceConnectionString = appSettings.Value.AzureBlobsConnectionString;
            _blobServiceContainerName = appSettings.Value.AzureBlobsContainerName;
            _cacheManager = cacheManager;
        }

        public bool IsDevelopment
        {
            get
            {
                if (string.IsNullOrEmpty(_blobServiceConnectionString) || string.IsNullOrWhiteSpace(_blobServiceConnectionString))
                    throw new ServicesException("Uninitialized BlobService.");
                return _blobServiceConnectionString.Contains("UseDevelopmentStorage");
            }
        }

        /// <summary>
        /// Upload user's avatar picture to blobs
        /// FileName will be taken only for extension.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        public void SaveUserProfilePicture(string userName, BlobDataDto blobData)
        {
            // Todo check username for slashes

            // Not sure how to save this just yet.
            var path = USER_IMAGES_PATH + userName + "_" + blobData.FileName;

            SaveFileToBlob(path, blobData.Stream);
        }

        public BlobDataDto FindUserProfilePicture(string userName)
        {
            // Not sure how to save this just yet.
            var path = USER_IMAGES_PATH + userName;
            return FindBlob(path);
        }

        /// <summary>
        /// Returns the first blob search by passes pattern.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public BlobDataDto FindBlob(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                throw new ServicesException("Null or blank path parameter FindBlob(path)");
            if (path.Contains("\\"))
                throw new ServicesException("Path parameter contains slash FindBlob(path)");

            // Get a reference to our container.
            var container = GetContainer();

            // Verified that ListBlobs does not return null if not found
            // There will be no null pointer exception here.
            var blob = container.ListBlobs(path, false).FirstOrDefault();
            if (blob == null) return null;

            // Convert to blob, read to stream and return.
            CloudBlockBlob blockBlob = (CloudBlockBlob)blob;
            var result = new BlobDataDto() { Stream = new MemoryStream(), FileName = blockBlob.Name };
            blockBlob.DownloadToStream(result.Stream);
            return result;
        }

        /// <summary>
        /// Blindly saves stream into a path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <remarks>Will error out on path containing slash.</remarks>
        public void SaveFileToBlob(string path, Stream stream)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                throw new ServicesException("Null or blank path parameter SaveFileToBlob(path, stream)");
            if (path.Contains("\\"))
                throw new ServicesException("Path parameter contains slash SaveFileToBlob(path, stream)");
            if (stream == null)
                throw new ServicesException("Invalid stream parameter SaveFileToBlob(path, stream)");
            if (!stream.CanRead)
                throw new ServicesException("Unreadable stream parameter SaveFileToBlob(path, stream)");

            // Get a reference to our container.
            var container = GetContainer();

            // Create the container if it doesn't already exist only on development environment
            if (IsDevelopment)
            {
                try
                {
                    container.CreateIfNotExists();
                }
                catch (StorageException ex)
                {
                    throw new ServicesException("StorageException was thrown with message: " + ex.Message, ex);
                }
            }

            //container.SetPermissions(
            //  new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // Retrieve reference to the blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);

            // Create or overwrite the blob with content
            blockBlob.UploadFromStream(stream);
        }

        public List<string> ListAllBlobs()
        {
            // Get a reference to out container.
            var container = GetContainer();

            var result = new List<string>();

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    result.Add(string.Format("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri));

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;

                    result.Add(string.Format("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri));

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;

                    result.Add(string.Format("Directory: {0}", directory.Uri));
                }
            }

            return result;
        }

        public List<string> ClearAllBlobs()
        {
            // Get a reference to out container.
            var container = GetContainer();

            var result = new List<string>();

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    blob.Delete();

                    result.Add(string.Format("Deleted block blob of length {0}: {1}", blob.Properties.Length, blob.Uri));

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;
                    pageBlob.Delete();

                    result.Add(string.Format("Deleted page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri));

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;
                    var d = container.GetDirectoryReference(directory.Uri.ToString());

                    result.Add(string.Format("Deleted directory: {0}", directory.Uri));
                }
            }

            return result;
        }

        /// <summary>
        /// return extension if file name ends on one of the known extensions.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFileExtension(string fileName)
        {
            const string ACCEPTABLE_EXTENSIONS = ",jpg,jpeg,png,gif,";

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
                return null;
            if (!fileName.Contains('.')) return null;
            var extension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
            return ACCEPTABLE_EXTENSIONS.Contains("," + extension + ",") ? extension : null;
        }

        private CloudBlobContainer GetContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_blobServiceConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(_blobServiceContainerName);

            return container;
        }
    }
}
