using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Users.Entities;
    public class ConfirmEmailToken : Entity
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool IsLocked { get; set; }
        public virtual User User { get; set; } = null!;

        public void LockToken()
        {
            IsLocked = true;
        }
    }
