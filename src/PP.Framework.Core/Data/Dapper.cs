using PaulPhillips.Framework.Feature.Contracts;

namespace PaulPhillips.Framework.Feature.Infrastructure.Dapper
{
    // NOTE: WIP, need to implement real dapper logic.
    public class Dapper : IDataContract
    {
        public async Task<bool> RecordExistsAsync<T>(string tableName, string field, T value)
        {
            
            return await Task.FromResult(false);
        }
    }
}
