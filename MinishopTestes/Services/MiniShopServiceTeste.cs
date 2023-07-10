using Minishop.Services;

namespace MinishopTestes.Services
{
    public class MiniShopServiceTeste
    {
        private readonly MiniShopServices _services;

        public MiniShopServiceTeste()
        {
            _services = new MiniShopServices();
        }

        [Fact]
        public void Quando_ForChamado_Deve_Retornar_A_Mensagem()
        {
            var request = _services.Foo();

            var retornoEsperado = "Api minishop";

            Assert.Equal(retornoEsperado, request);
        }
        
    }
}
