using Newtonsoft.Json;
using OpenTracing;
using PaulPhillips.Framework.Feature.Events.Contracts;

namespace PaulPhillips.Framework.Feature.Events
{
    public abstract class Event<T> : IEvent
    {
        public T? Request { get; set; }
        public void LoadData(string data) { 
            Request = JsonConvert.DeserializeObject<T>(data);
        }

        public abstract bool Process(ISpan tracingSpan);
    }
}
