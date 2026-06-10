using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Scheduler
{
    public class SchedulerSerializedObject
    {
        public string TypeName { get; set; }
        public string AssemblyName { get; set; }
        public string Data { get; set; }
        public string Description { get; set; }

        public SchedulerSerializedObject(string typeName, string assemblyName, string data, string description)
        {
            TypeName = typeName;
            AssemblyName = assemblyName;
            Data = data;
            Description = description;
        }

        public override string ToString()
        {
            var commandName = TypeName.Split('.').Last();
            return $"{commandName}";
        }
    }
}
