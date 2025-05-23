using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Repository.Interfaces.Employee;
using Gifts.Repository.Interfaces.Gift;
using Gifts.Repository.Interfaces.Vote;
using Gifts.Repository.Interfaces.VotingSession;
using Gifts.Services.DTOs.Vote;
using Gifts.Services.Interfaces.Vote;

namespace Gifts.Services.Implementations.Vote
{
    public class VoteService : IVoteService
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IGiftRepository _giftRepository;
        private readonly IVotingSessionRepository _votingSessionRepository;

        public VoteService(IVoteRepository voteRepository, IEmployeeRepository employeeRepository, IGiftRepository giftRepository, IVotingSessionRepository votingSessionRepository)
        {
            _voteRepository = voteRepository;
            _employeeRepository = employeeRepository;
            _giftRepository = giftRepository;
            _votingSessionRepository = votingSessionRepository;
        }

        private async Task<VoteInfo> MapToVoteInfoAsync(Models.Vote vote)
        {
            var voter = await _employeeRepository.RetrieveAsync(vote.VoterId);
            var gift = await _giftRepository.RetrieveAsync(vote.GiftId);
            return new VoteInfo
            {
                VoteId = vote.VoteId,
                VotingSessionId = vote.VotingSessionId,
                VoterId = vote.VoterId,
                VoterName = voter.FullName,
                GiftId = vote.GiftId,
                GiftName = gift.Name,
                VoteDate = vote.VoteDate
            };
        }

        public async Task<CreateVoteResponse> CreateVoteAsync(CreateVoteRequest request)
        {
            var session = await _votingSessionRepository.RetrieveAsync(request.VotingSessionId);
            if (session == null)
            {
                return new CreateVoteResponse()
                {
                    Success = false,
                    Message = "Voting session not found"
                };
            }
            if (!session.IsActive)
            {
                return new CreateVoteResponse()
                {
                    Success = false,
                    Message = "Voting session is not active"
                };
            }

            if (request.VoterId == session.BirthdayPersonId)
            {
                return new CreateVoteResponse()
                {
                    Success = false,
                    Message = "Birthday person cannot vote in their own session"
                };
            }

            var hasVoted = await UserHasVotedAsync(new HasUserVotedRequest() {
                EmployeeId = request.VoterId, 
                VotingSessionId = request.VotingSessionId
                });
            if (hasVoted.HasVoted)
            {
                return new CreateVoteResponse()
                {
                    Success = false,
                    Message = "User has already voted in this session"
                };
            }

            var gift = await _giftRepository.RetrieveAsync(request.GiftId);
            if (gift == null)
            {
                return new CreateVoteResponse()
                {
                    Success = false,
                    Message = "Gift not found"
                };
            }

            var vote = new Models.Vote
            {
                VotingSessionId = request.VotingSessionId,
                VoterId = request.VoterId,
                GiftId = request.GiftId,
                VoteDate = DateTime.Now
            };

            int voteId = await _voteRepository.CreateAsync(vote);

            vote.VoteId = voteId;
            var voteInfo = await MapToVoteInfoAsync(vote);
            return new CreateVoteResponse
            {
                VoteId = voteInfo.VoteId,
                VotingSessionId = voteInfo.VotingSessionId,
                VoterId = voteInfo.VoterId,
                VoterName = voteInfo.VoterName,
                GiftId = voteInfo.GiftId,
                GiftName = voteInfo.GiftName,
                VoteDate = voteInfo.VoteDate,
                Success = true
            };
        }

        public async Task<GetAllVotesResponse> GetVotesByVotingSessionIdAsync(int votingSessionId)
        {
            var filter = new VoteFilter { VotingSessionId = new SqlInt32(votingSessionId) };
            var votes = await _voteRepository.RetrieveCollectionAsync(filter).ToListAsync();

            var allVotes = new GetAllVotesResponse()
            {
                Votes = new List<VoteInfo>(),
                TotalCount = votes.Count
            };
            foreach (var vote in votes)
            {
                allVotes.Votes.Add(await MapToVoteInfoAsync(vote));
            }
            return allVotes;
        }

        public async Task<HasUserVotedResponse> UserHasVotedAsync(HasUserVotedRequest request)
        {
            var filter = new VoteFilter
            {
                VoterId = new SqlInt32(request.EmployeeId),
                VotingSessionId = new SqlInt32(request.VotingSessionId)
            };

            var votes = await _voteRepository.RetrieveCollectionAsync(filter).ToListAsync();
            var vote = votes.SingleOrDefault();

            if (vote == null)
            {
                return new HasUserVotedResponse
                {
                    HasVoted = false,
                    VotedAt = null,
                    GiftName = null
                };
            }

            var gift = await _giftRepository.RetrieveAsync(vote.GiftId);

            return new HasUserVotedResponse
            {
                HasVoted = true,
                VotedAt = vote.VoteDate,
                GiftName = gift.Name
            };

        }
    }
}