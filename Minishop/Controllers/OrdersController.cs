﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minishop.Domain.DTO;
using Minishop.Services;

namespace Minishop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService _service;

        public OrdersController(OrdersService service)
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
    }
}