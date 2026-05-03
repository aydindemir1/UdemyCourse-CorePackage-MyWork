using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events
{
    public enum EventPublishingStrategy
    {
        Transactional,
        Volatile
    }
}
