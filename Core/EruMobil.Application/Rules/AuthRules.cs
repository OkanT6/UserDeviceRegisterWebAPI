using EruMobil.Application.Bases;
using EruMobil.Application.Features.Auth.Exceptions;
using EruMobil.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Rules
{
    public class AuthRules : BaseRules
    {
        public Task UserShouldNotBeExisted(User user, string registerTypeTarget)
        {
            if (user is not null && registerTypeTarget == user.UserType) throw new UserAlreadyExistsException();
            return Task.CompletedTask;
        }

        public Task StudentNumberAndPasswordShouldBeMatched(User user, bool checkPassowrd)
        {
            if (user is null || !checkPassowrd) throw new StudentNumberAndPasswordShouldBeMatched();
            return Task.CompletedTask;
        }

        public Task RefreshTokenShouldNotBeExpired(DateTime? expiryDate)
        {
            if (DateTime.Now >= expiryDate) throw new RefreshTokenShouldNotBeExpiredException();
            return Task.CompletedTask;
        }


    }
}
