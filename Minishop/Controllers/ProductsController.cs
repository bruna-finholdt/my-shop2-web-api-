using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minishop.Services;
using Minishop.Domain.DTO;

namespace Minishop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _service;

        public ProductsController(ProductsService service)
        {
            _service = service;
        }

        [HttpGet("contagem")]
        public async Task<ActionResult<ItemCountResponse>> ObterContagem()//Contagem Products

        {
            ProductCountResponse retorno = await _service.Contar();

            return Ok(retorno);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PageQueryRequest queryResquest)
        {
            //Validação modelo de entrada
            var retorno = await _service.Pesquisar(queryResquest);
            if (retorno.Sucesso)
                return Ok(retorno);
            else
                return BadRequest(retorno);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //Validação modelo de entrada
            var retorno = await _service.PesquisaPorId(id);

            if (retorno.Sucesso)
                return Ok(retorno.Conteudo);
            else
                return NotFound(retorno);
        }
    }
}
