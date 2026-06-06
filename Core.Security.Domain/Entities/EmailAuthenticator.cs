using Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Domain.Entities
{
    public class EmailAuthenticator : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public string? ActivationKey { get; set; }
        public bool IsVerified { get; set; }

        public EmailAuthenticator()
        {

        }

        public EmailAuthenticator(Guid id, Guid userId, string? activationKey, bool isVerified) : base(id)
        {
            UserId = userId;
            ActivationKey = activationKey;
            IsVerified = isVerified;
        }
    }
}
