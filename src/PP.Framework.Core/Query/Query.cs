using Microsoft.AspNetCore.Http;
using OpenTracing;
using PaulPhillips.Framework.Feature.Events.Contracts;
using PaulPhillips.Framework.Feature.Query.Contracts;
using PaulPhillips.Framework.Feature.Validation.Contracts;
using PaulPhillips.Framework.Feature.Validation.Models;
using System.Security.Claims;

namespace PaulPhillips.Framework.Feature.Query;

public abstract class Query : IQuery
{
    public IEventManager? EventManager { get; set; }
    public IQueryCollection Request { get; set; } = new QueryCollection();

    public virtual void LoadIocServices(ISpan tracingSpan) { }
    public virtual async Task LoadData(ISpan tracingSpan) { await Task.CompletedTask; }

    public void SetData(IQueryCollection queryCollection)
    {
        Request = queryCollection;

    }
    public IModelValidation? GetValidation()
    {
        return Request as IModelValidation;
    }


    public abstract Task<dynamic> ProcessAsync(ISpan tracingSpan);
    public virtual ValidationResultModel Validate()
    {
        var vResult = new ValidationResultModel
        {
            Success = true
        };

        return vResult;
    }

    public virtual bool ValidateClaims(IList<Claim> claims, ISpan tracingSpan)
    {
        return true;
    }

}
