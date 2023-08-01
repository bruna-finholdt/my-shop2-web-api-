namespace Minishop.Services
{
    public interface IStorageService
    {
        Task<string> UploadFile(IFormFile file, int productId);
    }
}

