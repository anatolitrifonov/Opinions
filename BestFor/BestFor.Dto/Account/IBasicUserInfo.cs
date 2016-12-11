namespace BestFor.Dto.Account
{
    /// <summary>
    /// Basic interface for grouping common properties related to user's image
    /// </summary>
    public interface IBasicUserInfo
    {
        int Level { get; set; }

        string UserImageUrl { get; set; }

        string UserImageUrlSmall { get; set; }
    }
}
