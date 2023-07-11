using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class PageQueryRequest
    {
        // Permite a entrada de somente números positivos
        [Range(1, int.MaxValue)]
        // Caso não seja passado: o valor padrão é 1 
        public int PaginaAtual { get; set; }

        [Range(0, 50)]
        // Caso não seja passado: o valor padrão é 10
        public int Quantidade { get; set; } = 10;//de itens por página

    }
}
