namespace Minishop.Domain.DTO
{
    public class ProductImageUpdateRequest
    {
        public List<IFormFile>? NewImages { get; set; }//Lista de novas imagens a serem adicionadas
        public List<int>? ImageIdsToRemove { get; set; }//Lista de IDs de imagens a serem removidas
    }
}
