using Microsoft.AspNetCore.Mvc;
using Minishop.Services;

namespace Minishop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiniShopController : ControllerBase
    {
        private readonly MiniShopServices _services;

        public MiniShopController(MiniShopServices services) 
        {
            _services = services;  
        }

        [HttpGet]
        public string Foo()
        {
            return _services.Foo();
        }
    }
}
