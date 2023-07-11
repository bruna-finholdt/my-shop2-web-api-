namespace Minishop.Services.Base
{
    public class ServiceResponse<T>
    {
        public ServiceResponse(T objeto)
        {
            Sucesso = true;
            Mensagem = string.Empty;
            Conteudo = objeto;
        }

        public ServiceResponse(string mensagemDeErro)
        {
            Sucesso = false;
            Mensagem = mensagemDeErro;
            Conteudo = default;
        }

        public bool Sucesso { get; private set; }
        public string Mensagem { get; private set; }
        public T? Conteudo { get; private set; }
    }
}
