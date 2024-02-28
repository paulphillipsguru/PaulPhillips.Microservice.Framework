using OpenTracing;
using PaulPhillips.Feature.BasicExample.Features.FeatureDemo.Models;
using PaulPhillips.Framework.Feature.Commands;

namespace PaulPhillips.Feature.BasicExample.Features.FeatureDemo
{
    public class CreateCustomerFeature : Command<RequestModel,ResponseModel>
    {
        public override  async Task<dynamic> ProcessAsync(ISpan tracingSpan)
        {
            if (!string.IsNullOrWhiteSpace(Request?.FullName))
            {
                var nameToSplit = Request.FullName.Split(" ");

                var responseModel = new ResponseModel { FirstName = nameToSplit[0] };

                return await Task.FromResult(responseModel);
            }

            return await Task.FromResult(new ResponseModel());

        }
    }
}
