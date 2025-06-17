using EruMobil.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EruMobil.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] RegisterCommandRequest request)
        {
            //obisisAPI(request.token);
            request.UserType = "student"; // UserType'ı student olarak ayarla
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] RegisterCommandRequest request)
        {
            request.UserType = "staff"; // UserType'ı staff olarak ayarla
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Test()
        {
            return Ok("Controller çalışıyor!");
        }

        //[HttpPost]
        //public async Task<IActionResult> RegisterDevice([FromBody] LoginCommandRequest request)
        //{
        //    var result = await mediator.Send(request);
        //    return Ok(result);
        //}

        //[HttpPost]
        //public async Task<IActionResult> LogoutStudent([FromBody] RevokeCommandRequest request)
        //{
        //    var result = await mediator.Send(request);
        //    return Ok(result);
        //}



        //[HttpPost]
        //public async Task<IActionResult> Register2([FromBody] RegisterCommandRequest request)
        //{
        //    try
        //    {
        //        // Debug için request'i kontrol et
        //        if (request == null)
        //        {
        //            return BadRequest("Request is null");
        //        }

        //        if (string.IsNullOrEmpty(request.UserType))
        //        {
        //            return BadRequest("UserType is required");
        //        }

        //        // Model validation kontrolü
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var result = await mediator.Send(request);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}
    }
}
