using Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Domain.Entities
{
    public class UserOperationClaim : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid OperationClaimId { get; set; }

        public UserOperationClaim(Guid id, Guid userId, Guid operationClaimId) : base(id)
        {
            UserId = userId;
            OperationClaimId = operationClaimId;
        }

        public UserOperationClaim()
        {

        }
    }
}
