using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Users.Request
{
    public record ChangePasswordRequest(string OldPassword,string NewPassword
        ,string ConfirmNewPassword);
}
