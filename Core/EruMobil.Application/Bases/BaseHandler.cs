using EruMobil.Application.Interfaces.AutoMapper;
using EruMobil.Application.Interfaces.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Bases
{
    public class BaseHandler
    {
        protected readonly IMapper mapper;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly Guid userId;

        public BaseHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.httpContextAccessor = httpContextAccessor;
            userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) != null ?
                Guid.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)) : Guid.Empty;
        }
    }
}
