using System;

namespace Core.Domain
{
    public class BaseEntity<TId>
    {
        public TId Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset? DeletedDate { get; set; }

        public BaseEntity()
        {
            Id = default!;
        }

        public BaseEntity(TId id)
        {
            Id = id;
        }
    }
}
