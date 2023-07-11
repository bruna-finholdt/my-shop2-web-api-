namespace Minishop.Services.Base
{
    public class ServicePagedResponse<T> : ServiceResponse<IEnumerable<T>>
    {
        /// <summary>
        /// Cria um service response com paginação - caso de sucesso
        /// </summary>
        /// <remarks>Chama o construtor da classe pai através do 'base(lista)'</remarks>
        /// <see cref=""/>
        /// <param name="lista">Resultado da consulta</param>
        /// <param name="contagem">Quantidade de itens existentes no banco</param>
        /// <param name="paginaAtual">Pagina que atualmente foi consultada</param>
        /// <param name="qtdPagina">Quantidade de itens existentes na página</param>
        public ServicePagedResponse(IEnumerable<T> lista, int contagem, int paginaAtual, int qtdPagina)
            : base(lista)
        {
            PaginaAtual = paginaAtual;
            // Se houverem 95 itens e consultamos 10 itens p/ página, existirão portanto 10 página
            //  Math.Ceiling -> arredonda o atual número decimal para cima. ex: 9.5 => 10.0 | 10 => 10.0
            //  (int) -> transforma nosso número decimal em número inteiro
            //  (double) -> força que o resultado da divisão seja do tipo double, uma vez que divisão pode retornar decimal ou double
            TotalPaginas = (int)Math.Ceiling(contagem / (double)qtdPagina);
        }

        /// <summary>
        /// Cria um service response com paginação - caso de sucesso
        /// </summary>
        /// <remarks>Chama o construtor da classe pai através do 'base(mensagemDeErro)'</remarks>
        /// <param name="mensagemDeErro">Mensagem de erro da execução</param>
        public ServicePagedResponse(string mensagemDeErro) : base(mensagemDeErro)
        {
        }

        public int PaginaAtual { get; private set; }
        public int TotalPaginas { get; private set; }
    }
}
