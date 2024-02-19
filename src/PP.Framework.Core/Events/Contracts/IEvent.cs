using OpenTracing;

namespace PaulPhillips.Framework.Feature.Events.Contracts
{
    public interface IEvent
    {
        void LoadData(string data);
        bool Process(ISpan tracingSpan);
    }
}
