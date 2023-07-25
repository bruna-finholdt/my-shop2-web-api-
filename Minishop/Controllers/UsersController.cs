using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minishop.Domain.DTO.Validation;
using Minishop.Domain.DTO;
using Minishop.Services;
using Microsoft.AspNetCore.Authorization;

namespace Minishop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;
        private readonly ITokenService _tokenService;

        public UsersController(IUsersService service, ITokenService tokenService)
        {
            _service = service;
            _tokenService = tokenService;
        }

        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Get([FromQuery] PageQueryRequest queryRequest)
        {
            //Validação modelo de entrada
            var retorno = await _service.Pesquisar(queryRequest);
            if (retorno.Sucesso)
                return Ok(retorno);
            else
                return BadRequest(retorno);
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetById([Id(ErrorMessage = "Valor de Id não condiz com formato esperado")] string id)
        {
            //Validação modelo de entrada
            var retorno = await _service.PesquisaPorId(int.Parse(id));

            if (retorno.Sucesso)
                return Ok(retorno.Conteudo);
            else
                return NotFound(retorno);
        }

        [HttpGet("GetByUserName/{userName}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            var response = await _service.PesquisaPorNome(userName);
            if (!response.Sucesso)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> AuthenticateAsync([FromBody] UserLoginRequest model)
        {
            //Validação modelo de entrada
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var retorno = await _service.Logar(model);
            if (!retorno.Sucesso)
                return BadRequest(retorno);

            string token = ""; //oculta a senha

            if (retorno.Conteudo != null)
            {
                token = _tokenService.GenerateToken(retorno.Conteudo);
            }

            return new //retorna os dados
            {
                user = retorno.Conteudo,
                token = token,
            };
        }

        [HttpPost]
        [Route("Cadastro")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Post([FromBody] UserCreateRequest postModel)
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
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Put(int id, [FromBody] UserUpdateRequest updateModel)
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

