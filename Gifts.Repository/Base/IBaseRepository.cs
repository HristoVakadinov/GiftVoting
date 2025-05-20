using System;
using System.Collections.Generic;

namespace Gifts.Repository.Base
{
    public interface IBaseRepository<TObj, TFilter, TUpdate>
    {
        Task<int> CreateAsync(TObj entity);
        Task<TObj> RetrieveAsync(int objectId);
        IAsyncEnumerable<TObj> RetrieveCollectionAsync(TFilter filter);
        Task<bool> UpdateAsync(int objectId, TUpdate update);
        Task<bool> DeleteAsync(int objectId);
    }
}
