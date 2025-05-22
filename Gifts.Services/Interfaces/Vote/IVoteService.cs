using Gifts.Services.DTOs.Vote;

namespace Gifts.Services.Interfaces.Vote
{
    public interface IVoteService
    {
        Task<CreateVoteResponse> CreateVoteAsync(CreateVoteRequest request);
        Task<GetAllVotesResponse> GetVotesByVotingSessionIdAsync(int votingSessionId);
        Task<HasUserVotedResponse> UserHasVotedAsync(HasUserVotedRequest request);
    }
}