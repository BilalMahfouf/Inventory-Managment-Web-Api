using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Users.Response
{
    public sealed record UserReadResponse()
    {
        public int Id { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string Email {  get; init; } = string.Empty;
        public string FullName {  get; init; } = string.Empty;
        public int RoleId { get; init; }
        public string RoleName { get; init; }= string.Empty;
        public bool IsActive { get; init; }
        public bool IsEmailConfirmed { get; init; }
        public bool? IsDeleted { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public DateTime? DeletedAt { get; init; }

        public int? CreatedByUserId { get; init; }
        public string? CreatedByUserName { get; init; }

        public int? UpdatedByUserId { get;init; }
        public string? UpdatedByUserName { get; init; }

        public int? DeletedByUserId { get;init;}
        public string? DeletedByUserName { get; init; }


    }
}
