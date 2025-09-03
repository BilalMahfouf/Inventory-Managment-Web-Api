using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Users.Response
{
    public sealed record UserRolesReadResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description {  get; init; } = string.Empty;
        public bool IsDeleted { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime LastUpdatedAt { get; init; }
        public DateTime DeletedAt { get; init; }

        public int CreatedByUserId { get; init; }
        public string CreatedByUserName { get; init; }= string.Empty;

        public int UpdatedByUserId { get; init; }
        public string UpdatedByUserName { get;init; }= string.Empty;

        public int DeleteByUserId { get; init; }
        public string DeleteByUserName { get; init;}= string.Empty;

    }
}
