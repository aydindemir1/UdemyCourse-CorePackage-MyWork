using Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Security.Domain.Entities
{
    public class OperationClaim : BaseEntity<Guid>
    {
        public string Name { get; set; }

        public OperationClaim(Guid id, string name) : base(id)
        {
            Name = name;
        }
        public OperationClaim()
        {

        }
    }
}
