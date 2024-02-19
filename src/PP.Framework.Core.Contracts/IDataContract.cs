namespace PaulPhillips.Framework.Feature.Contracts;

// NOTE: WIP, Need to add more methods etc to support required 
// functionality.
// Key goal to provide comment ground to working with external 
// databases.
public interface IDataContract
{
    Task<bool> RecordExistsAsync<T>(string tableName, string field, T value);
}
