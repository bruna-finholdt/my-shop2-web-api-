namespace Minishop.Domain.DTO
{
    public class ProductImageDeleteRequest
    {
        public List<int>? ImageIdsToRemove { get; set; }//Lista de IDs de imagens a serem removidas
    }
}
