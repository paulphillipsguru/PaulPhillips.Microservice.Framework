using Microsoft.AspNetCore.Http;
using PaulPhillips.Framework.Feature.Core.Contracts;
using PaulPhillips.Framework.Feature.Validation.Models;

namespace PaulPhillips.Framework.Feature.Query.Contracts;

public interface IQuery : IFeatureCore
{
    void SetData(IQueryCollection data);
    ValidationResultModel Validate();
}
