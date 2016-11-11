using BestFor.Dto;
using BestFor.Domain.Entities;

namespace BestFor.Services.Blobs
{
    public interface IBlobService
    {
        void SaveUserProfilePicture(string userName, BlobDataDto blobData);

        string GetUserProfilePictureName(string userName);

        bool DoesUserProfileHasPicture(string userName);

        /// <summary>
        /// Return user's umage url if it has one.
        /// Methos also caches the image path in user object.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks>Images are sparate users. This service helps tying then together.</remarks>
        string GetUserImagUrl(ApplicationUser user);

        /// <summary>
        /// Set the fact that we already checked user's image existence.
        /// Set the expected url if user has an image.
        /// User's image url is predefined.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="hasImage"></param>
        void SetUserImageCached(ApplicationUser user, bool hasImage);
    }
}
