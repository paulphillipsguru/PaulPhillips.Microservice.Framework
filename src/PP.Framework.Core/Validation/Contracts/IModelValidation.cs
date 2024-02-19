using PaulPhillips.Framework.Feature.Validation.Models;
namespace PaulPhillips.Framework.Feature.Validation.Contracts;

public interface IModelValidation
{
    ValidationResultModel Validate();
}
