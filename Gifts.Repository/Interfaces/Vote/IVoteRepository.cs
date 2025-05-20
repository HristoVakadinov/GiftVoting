using Gifts.Models;
using Gifts.Repository.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gifts.Repository.Interfaces.Vote
{
    public interface IVoteRepository : IBaseRepository<Models.Vote, VoteFilter, VoteUpdate>
    {
        
    }
} 