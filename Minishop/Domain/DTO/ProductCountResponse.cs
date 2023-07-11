namespace Minishop.Domain.DTO
{
    public class ProductCountResponse : ItemCountResponse
    {
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
    }
}
