using Gifts.Services.DTOs.Vote;

namespace Gifts.Services.Interfaces.Vote
{
    public interface IVoteService
    {
        Task<VoteDto> CreateVoteAsync(CreateVoteRequest request);
        Task<IEnumerable<VoteDto>> GetVotesByVotingSessionIdAsync(int votingSessionId);
        Task<bool> UserHasVotedAsync(int votingSessionId, int employeeId);
    }
}