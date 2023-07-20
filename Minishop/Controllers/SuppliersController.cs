using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minishop.Domain.DTO;
using Minishop.Domain.DTO.Validation;
using Minishop.Services;

namespace Minishop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SuppliersController(ISupplierService service)
        {
            _service = service;
        }

        [HttpGet("contagem")]
        public async Task<ActionResult<ItemCountResponse>> ObterContagem()//Contagem Suppliers

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

        [HttpPost]
        // FromBody para indicar de o corpo da requisição deve ser mapeado para o modelo
        public async Task<IActionResult> Post([FromBody] SupplierCreateRequest postModel)
        {
            //Validação modelo de entrada
            if (ModelState.IsValid)
            {
                var retorno = await _service.Cadastrar(postModel);
                if (!retorno.Sucesso)
                    return BadRequest(retorno);
                else
                    return Ok(retorno.Conteudo);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SupplierUpdateRequest updateModel)
        {
            if (ModelState.IsValid)
            {
                var retorno = await _service.Editar(id, updateModel);
                if (!retorno.Sucesso)
                {
                    return BadRequest(retorno.Mensagem);
                }
                else
                {
                    return Ok(retorno.Conteudo);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
