using Core.Domain;
using Core.Security.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Domain.Entities
{
    public class User : BaseEntity<Guid>
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool Status { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public AuthenticatorType AuthenticatorType { get; set; }

        public User(Guid id, string email, string passwordHash, bool status, int accessFailedCount, bool isLockedOut, DateTime? lockoutEnd, AuthenticatorType authenticatorType) : base(id)
        {
            Email = email;
            PasswordHash = passwordHash;
            Status = status;
            AccessFailedCount = accessFailedCount;
            IsLockedOut = isLockedOut;
            LockoutEnd = lockoutEnd;
            AuthenticatorType = authenticatorType;
        }

        public User()
        {

        }
    }
}
