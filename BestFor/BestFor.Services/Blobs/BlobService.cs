using BestFor.Common;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Services.Cache;
using ImageResizer;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BestFor.Services.Blobs
{
    /// <summary>
    /// Contains the code that saves the file to blob.
    /// Contains the logic that forms the path for files per entity type.
    /// Entity type is passed through the method name.
    /// All images are stored as png.
    /// 
    /// All files are stored in one container until we know better.
    /// </summary>
    public class BlobService : IBlobService
    {
        private const string USER_IMAGES_PATH = "user_";
        private const string USER_IMAGES_EXTENSION = ".png";
        private const string USER_IMAGES_EXTENSION_SMALL = "_s.png";
        private const string USER_IMAGES_FORMAT = "png";

        /// <summary>
        /// Connection to blob storage from app settings
        /// </summary>
        private string _blobServiceConnectionString;

        /// <summary>
        /// Default images container name from app settings
        /// </summary>
        private string _blobServiceContainerName;

        /// <summary>
        /// Default avatar width from app settings
        /// </summary>
        private int _avatarWidth;

        /// Default avatar height from app settings
        private int _avatarHeight;

        /// <summary>
        /// Injected cache manager.
        /// </summary>
        private ICacheManager _cacheManager;

        private string _blobServiceContainerUrl;

        public BlobService(IOptions<AppSettings> appSettings, ICacheManager cacheManager)
        {
            _blobServiceConnectionString = appSettings.Value.AzureBlobsConnectionString;
            _blobServiceContainerName = appSettings.Value.AzureBlobsContainerName;
            _blobServiceContainerUrl = appSettings.Value.AzureBlobsContainerUrl;
            _avatarWidth = appSettings.Value.AvatarWidth;
            _avatarHeight = appSettings.Value.AvatarHeight;
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

        #region IBlobService implementation
        /// <summary>
        /// Upload user's avatar picture to blobs
        /// FileName will be taken only for extension.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="blobData"></param>
        public void SaveUserProfilePicture(string userName, BlobDataDto blobData)
        {
            // Todo check username for slashes
            // use image resizer to resize to a small avatar.

            // main avatar
            var path = USER_IMAGES_PATH + userName + USER_IMAGES_EXTENSION;
            var resizedImageStream = ResizeToAvatar(blobData.Stream, _avatarWidth, _avatarHeight);
            resizedImageStream.Position = 0;
            SaveFileToBlob(path, resizedImageStream);

            //blobData.Stream.Position = 0;

            // half avatar
            var pathSmall = USER_IMAGES_PATH + userName + USER_IMAGES_EXTENSION_SMALL;
            resizedImageStream.Position = 0;
            var resizedImageStreamSmall = ResizeToAvatar(resizedImageStream, _avatarWidth / 2, _avatarHeight / 2);
            resizedImageStreamSmall.Position = 0;
            SaveFileToBlob(pathSmall, resizedImageStreamSmall);
        }

        /// <summary>
        /// Return user's umage url if it has one.
        /// Methos also caches the image path in user object.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks>Images are sparate users. This service helps tying then together.</remarks>
        public string GetUserImagUrl(ApplicationUser user)
        {
            // return data if already cached.
            if (user.IsImageCached) return user.ImageUrl;

            // Go to blob service and search
            var hasImage = DoesUserProfileHasPicture(user.UserName);

            // Populate the image url if found and mark user image as cached.
            SetUserImageCached(user, hasImage);

            return user.ImageUrl;
        }

        /// <summary>
        /// Set the fact that we already checked user's image existence.
        /// Set the expected url if user has an image.
        /// User's image url is predefined.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="hasImage"></param>
        public void SetUserImageCached(ApplicationUser user, bool hasImage)
        {
            // found or not it is cached.
            user.IsImageCached = true;

            // set the image url
            user.ImageUrl = hasImage ? _blobServiceContainerUrl + GetUserProfilePictureName(user.UserName) : null;
            user.ImageUrlSmall = hasImage ? _blobServiceContainerUrl + GetUserProfilePictureNameSmall(user.UserName) : null;
        }

        public void DeleteUserProfilePicture(string userName)
        {
            // Nothing to do -> exit.
            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName)) return;

            // Get a reference to our container.
            var container = GetContainer();

            // form the user image path
            var path = GetUserProfilePictureName(userName);

            // Verified that ListBlobs does not return null if not found
            // There will be no null pointer exception here.
            var blob = container.ListBlobs(path, false).FirstOrDefault();
            if (blob == null) return;

            if (blob.GetType() == typeof(CloudBlockBlob))
            {
                var item = (CloudBlockBlob)blob;
                item.Delete();
            }
        }
        #endregion

        public string GetUserProfilePictureName(string userName)
        {
            return USER_IMAGES_PATH + userName + USER_IMAGES_EXTENSION;
        }

        public string GetUserProfilePictureNameSmall(string userName)
        {
            return USER_IMAGES_PATH + userName + USER_IMAGES_EXTENSION_SMALL;
        }

        public bool DoesUserProfileHasPicture(string userName)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
                throw new ServicesException("Null or blank path parameter IsUserProfileHasPicture(userName)");
            if (userName.Contains("\\"))
                throw new ServicesException("Path parameter contains slash IsUserProfileHasPicture(userName)");

            // Get a reference to our container.
            var container = GetContainer();

            // Verified that ListBlobs does not return null if not found
            // There will be no null pointer exception here.
            //var blob = container.ListBlobs(USER_IMAGES_PATH + userName, false).FirstOrDefault();
            var blob = container.ListBlobs(GetUserProfilePictureName(userName), false).FirstOrDefault();
            return blob != null;
        }

        /// <summary>
        /// Resize input image stream to width and height and to png format.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Stream ResizeToAvatar(Stream input, int width, int height)
        {
            // Instructions on how to resize.
            var settings = new Instructions()
            {
                Width = width,
                Height = height,
                Format = USER_IMAGES_FORMAT
            };
            // Will return a memory stream
            var result = new MemoryStream();
            // Define the "job" to do and run it.
            var job = new ImageJob(input, result, settings);
            ImageBuilder.Current.Build(job);
            // Reset position to zero otherwise whoever is going to write it to disk or anywhere
            // Will write zero bytes
            result.Position = 0;
            return result;
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
                    //container.DeleteIfExists();
                    container.CreateIfNotExists();
                    var perm = container.GetPermissions();
                    perm.PublicAccess = BlobContainerPublicAccessType.Blob;
                    // set all blobs to anonymous read only
                    // no one can list all blobs though.
                    // We will generate direct link to user's image.
                    container.SetPermissions(perm);
                }
                catch (StorageException ex)
                {
                    throw new ServicesException("StorageException was thrown with message: " + ex.Message, ex);
                }
            }

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
