namespace Minishop.Services
{
    public interface IStorageService
    {
        Task<string> UploadFile(IFormFile file, int productId);
        Task<bool> RemoveImageFromBucket(string key);
    }
}

