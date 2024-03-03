using OpenTracing;
using PaulPhillips.Framework.Feature.Events.Contracts;
using System.Security.Claims;


namespace PaulPhillips.Framework.Feature.Core.Contracts
{
    public interface IFeatureCore
    {

        Task LoadData(ISpan tracingSpan);
        void LoadIocServices(ISpan tracingSpan);
        Task<dynamic> ProcessAsync(ISpan tracingSpan);
        bool ValidateClaims(IList<Claim> claims, ISpan tracingSpan);
        IEventManager? EventManager { get; set; }
    }
}
