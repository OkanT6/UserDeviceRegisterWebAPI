using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommandRequest>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.StudentNumber)
                .NotEmpty().WithMessage("Öğrenci numarası boş olamaz.")
                .Length(10).WithMessage("Öğrenci numarası 10 karakter olmalıdır.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
        }
    }

}
