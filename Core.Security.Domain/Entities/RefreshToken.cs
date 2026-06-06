using Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Domain.Entities
{
    public class RefreshToken : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }

        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public string CreatedByIp { get; set; }

        public DateTime? Revoked { get; set; }

        public string? RevokedByIp { get; set; }

        public string? ReplacedByToken { get; set; }

        public string? ReasonRevoked { get; set; }

        public RefreshToken(Guid id, Guid userId, string token, DateTime expires, string createdByIp, DateTime? revoked, string? revokedByIp, string? replacedByToken, string? reasonRevoked) : base(id)
        {
            UserId = userId;
            Token = token;
            Expires = expires;
            CreatedByIp = createdByIp;
            Revoked = revoked;
            RevokedByIp = revokedByIp;
            ReplacedByToken = replacedByToken;
            ReasonRevoked = reasonRevoked;
        }

        public RefreshToken()
        {

        }
    }
}
