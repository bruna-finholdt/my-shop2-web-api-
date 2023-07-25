using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minishop.Services;
using Minishop.Domain.DTO;
using Minishop.Domain.DTO.Validation;
using Microsoft.AspNetCore.Authorization;

namespace Minishop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _service;


        public ProductsController(IProductsService service)
        {
            _service = service;
        }

        [HttpGet("contagem")]
        [AllowAnonymous]
        public async Task<ActionResult<ItemCountResponse>> ObterContagem()//Contagem Products

        {
            ProductCountResponse retorno = await _service.Contar();

            return Ok(retorno);
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
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
        [Authorize(Roles = "1, 3")]
        // FromBody para indicar de o corpo da requisição deve ser mapeado para o modelo
        public async Task<IActionResult> Post([FromBody] ProductCreateRequest postModel)
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
        [Authorize(Roles = "1, 3")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductUpdateRequest updateModel)
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
