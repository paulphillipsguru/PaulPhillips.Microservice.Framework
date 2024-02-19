using PaulPhillips.Framework.Feature.Validation.Models;

namespace PaulPhillips.Framework.Feature.Models
{
    public class ResponseModel
    {
        public ValidationResultModel? ValidationResult { get; set; }
        public dynamic? Response { get; set; }
    }
}
