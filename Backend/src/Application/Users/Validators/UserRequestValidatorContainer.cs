using Application.Users.DTOs.Request;
using Application.Users.Validators.Configuration;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Validators
{
    public class UserRequestValidatorContainer
    {
        public  IValidator<UserCreateRequest> UserCreateRequestValidator
        { get; }
        public IValidator<UserUpdateRequest> UserUpdateRequestValidator 
        { get; }
        public IValidator<ChangePasswordRequest > ChangePasswordRequestValidator
        { get; }

        public UserRequestValidatorContainer(
            IValidator<UserCreateRequest> userCreateRequestValidator,
            IValidator<UserUpdateRequest> userUpdateRequestValidator,
            IValidator<ChangePasswordRequest> changePasswordRequestValidator)
        {
            UserCreateRequestValidator = userCreateRequestValidator;
            UserUpdateRequestValidator = userUpdateRequestValidator;
            ChangePasswordRequestValidator = changePasswordRequestValidator;
        }


    }

}
