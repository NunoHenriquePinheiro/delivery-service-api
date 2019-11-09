using AutoMapper;
using DeliveryServiceApp.Helpers.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryServiceApp.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly IMapper _mapper;
        protected readonly AppSettings _appSettings;

        public BaseController(
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        protected IActionResult OkOptionalContent<T>(T obj)
        {
            if (obj != null)
            {
                return Ok(obj);
            }
            return NoContent();
        }

        protected IActionResult OkOptionalContent<T>(IEnumerable<T> objCollection)
        {
            if (objCollection?.Any() == true)
            {
                return Ok(objCollection);
            }
            return NoContent();
        }
    }
}
