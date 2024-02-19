namespace PaulPhillips.Framework.Feature.Validation.Models
{
    public class ValidationResultModel
    {
        public bool Success { get; set; } = false;
        public List<string> ValidationMessages { get; set; } = [];
    }
}
