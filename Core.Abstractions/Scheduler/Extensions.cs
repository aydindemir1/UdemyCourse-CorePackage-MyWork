using Core.Abstractions.Cqrs;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Scheduler
{
    public static class Extensions
    {
        public static async Task SendSchedulerObject(this ICqrsProcessor cqrsProcessor, SchedulerSerializedObject schedulerSerializedObject)
        {
            var type = schedulerSerializedObject.GetPayloadType();

            dynamic? command = Newtonsoft.Json.JsonConvert.DeserializeObject(schedulerSerializedObject.Data, type);

            if (command != null)
                await cqrsProcessor.SendAsync(command);

        }

        private static Type GetPayloadType(this SchedulerSerializedObject obj)
        {
            var type = Type.GetType($"{obj.TypeName},{obj.AssemblyName}");
            if (type == null)
                throw new InvalidOperationException($"SchedulerSerializedObject : type could not be resolved {obj.TypeName}");
            return type;
        }
    }
}
