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
        //[Authorize]
        public async Task<IActionResult> GetById([Id(ErrorMessage = "Valor de Id não condiz com formato esperado")] string id)
        {
            // Validação modelo de entrada
            var retornoCompleto = await _service.PesquisaPorIdCompleto(int.Parse(id));

            if (!retornoCompleto.Sucesso)
                return NotFound(retornoCompleto);

            return Ok(retornoCompleto.Conteudo);
        }

        [HttpPost("Produto")]
        //[Authorize(Roles = "1, 3")]
        public async Task<IActionResult> Post([FromBody] ProductCreateRequest model)
        {
            if (ModelState.IsValid)
            {
                var produtoResponse = await _service.Cadastrar(model);

                if (!produtoResponse.Sucesso)
                    return BadRequest(produtoResponse);

                return Ok(produtoResponse);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("ImagemdoProduto")]
        //[Authorize(Roles = "1, 3")]
        public async Task<IActionResult> Post([FromForm] int productId, [FromForm] List<IFormFile> files)
        {
            //Valida o formato do arquivo (PNG, JPG ou JPEG)
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
            if (files != null && files.Any())//se tiver algum arquivo de imagem...
            {
                foreach (IFormFile file in files)//percorrer sobre eles...
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))//e validar o seu tipo
                    {
                        return BadRequest("Formato de arquivo não suportado. Apenas arquivos PNG, JPG e JPEG são permitidos.");
                        //usar String.Join nessa msg
                    }
                }
            }

            if (ModelState.IsValid)
            {
                List<ProductImageResponse> imagemResponses = new List<ProductImageResponse>();
                if (files != null && files.Any())//se tiver algum arquivo de imagem...
                {
                    foreach (IFormFile file in files)//percorrer sobre eles...
                    {
                        var retornoImagem = await _service.CadastrarImagem(file, productId);
                        if (!retornoImagem.Sucesso)
                        {
                            return BadRequest(retornoImagem);
                        }
                        imagemResponses.Add(retornoImagem.Conteudo);//e cadastrar cada um deles
                    }
                }
                return Ok(imagemResponses);//response com as imagens cadastradas
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("Produto/{productId}")]
        //[Authorize(Roles = "1, 3")]
        public async Task<IActionResult> Put(int productId, [FromBody] ProductUpdateRequest model)
        {
            if (ModelState.IsValid)
            {
                var updatedProduct = await _service.Editar(productId, model);

                if (updatedProduct == null)
                    return NotFound("Produto não encontrado");

                return Ok(updatedProduct);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("ImagemdoProduto/{productId}")]
        //[Authorize(Roles = "1, 3")]
        public async Task<IActionResult> Put(int productId, [FromForm] ProductImageUpdateRequest model)
        {

            if (ModelState.IsValid)
            {
                var response = await _service.EditarImagem(productId, model);

                if (!response.Sucesso)
                    return BadRequest(response);

                return Ok(response);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("ImagemdoProduto/{productId}/AlterarOrdem")]
        //[Authorize(Roles = "1, 3")]
        public async Task<IActionResult> Put(int productId, [FromBody] ProductImageOrderUpdateRequest model)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.AlterarOrdemImagens(productId, model);

                if (!response.Sucesso)
                    return BadRequest(response);

                return Ok(response);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        //bla
    }
}
