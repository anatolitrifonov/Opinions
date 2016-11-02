using BestFor.Dto;

namespace BestFor.Services.Blobs
{
    public interface IBlobService
    {
        void SaveUserProfilePicture(string userName, BlobDataDto blobData);
    }
}
