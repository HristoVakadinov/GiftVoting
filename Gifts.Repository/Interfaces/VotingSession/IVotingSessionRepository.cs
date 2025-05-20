using Gifts.Models;
using Gifts.Repository.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gifts.Repository.Interfaces.VotingSession
{
    public interface IVotingSessionRepository : IBaseRepository<Models.VotingSession, VotingSessionFilter, VotingSessionUpdate>
    {

    }
} 