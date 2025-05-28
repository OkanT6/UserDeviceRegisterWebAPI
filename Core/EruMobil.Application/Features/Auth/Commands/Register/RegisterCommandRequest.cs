using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandRequest : IRequest<RegisterCommandResponse>
    {
        public RegisterCommandRequest()
        {
            // parametresiz ctor => model binding için gerekli
        }

        public string FullName { get; set; }

        public string TCNo { get; set; }

        public string BusinessIdentifier { get; set; }

        public string UserType { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        //[OnDeserialized]
        //internal void OnDeserializedMethod(StreamingContext context)
        //{
        //    if (!string.IsNullOrWhiteSpace(UserType))
        //    {
        //        if (UserType.ToLower() == "student")
        //            BusinessIdentifier = new BusinessIdentitfierGenerator().CreateStudentNumber();
        //        else if (UserType.ToLower() == "staff")
        //            BusinessIdentifier = new BusinessIdentitfierGenerator().CreateStaffNumber();
        //        else
        //            throw new ArgumentException("Invalid User Type");
        //    }
        //}
    }
}
