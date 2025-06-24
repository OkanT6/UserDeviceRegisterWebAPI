using EruMobil.Application.Features.Devices.Commands.RegisterDevice;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EruMobil.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IMediator mediator;

        public DeviceController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [EnableRateLimiting("IPPolicy")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceCommandRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }
    }
}
