using Application.DTOs.Users.Request;
using Application.DTOs.Users.Response;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.User
{
    public interface IUserService
    {
        Task<Result<UserReadResponse>> FindByIdAsync(int id
            , CancellationToken cancellationToken);
        Task<Result<IEnumerable<UserReadResponse>>> GetAllAsync(
            CancellationToken cancellationToken);

        Task<Result> UpdateAsync(UserUpdateRequest request
            , CancellationToken cancellationToken);
        Task<Result<UserReadResponse>> AddAsync(UserCreateRequest request,
            CancellationToken cancellationToken);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<Result> ActivateAsync(int id , CancellationToken cancellationToken);
        Task<Result> DesActivateAsync(int id, CancellationToken cancellationToken);

        Task<Result> ChangePasswordAsync(ChangePasswordRequest request,
            CancellationToken cancellationToken);
       
    }
}
