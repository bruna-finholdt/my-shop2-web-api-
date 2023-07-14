using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minishop.Domain.DTO;
using Minishop.Domain.DTO.Validation;
using Minishop.Services;

namespace Minishop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _service;

        public OrdersController(IOrdersService service)
        {
            _service = service;
        }

        [HttpGet("contagem")]
        public async Task<ActionResult<ItemCountResponse>> ObterContagem()//Contagem Orders

        {

            var retorno = await _service.Contar();

            var response = new ItemCountResponse
            {
                ItemCount = retorno
            };

            return Ok(response);
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
        public async Task<IActionResult> GetById([Id(ErrorMessage = "Valor de Id não condiz com formato esperado")] string id)
        {
            //Validação modelo de entrada
            var retorno = await _service.PesquisaPorId(int.Parse(id));

            if (retorno.Sucesso)
                return Ok(retorno.Conteudo);
            else
                return NotFound(retorno);
        }
    }
}
