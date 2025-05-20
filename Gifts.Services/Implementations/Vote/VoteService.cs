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

        private async Task<VoteDto> MapToDtoAsync(Models.Vote vote)
        {
            var voter = await _employeeRepository.RetrieveAsync(vote.VoterId);
            var gift = await _giftRepository.RetrieveAsync(vote.GiftId);

            return new VoteDto
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

        public async Task<VoteDto> CreateVoteAsync(CreateVoteRequest request)
        {
            var session = await _votingSessionRepository.RetrieveAsync(request.VotingSessionId);
            if (session == null)
            {
                throw new ValidationException("Voting session not found");
            }
            if (!session.IsActive)
            {
                throw new ValidationException("Voting session is not active");
            }
            if (session.EndDate < DateTime.UtcNow)
            {
                throw new ValidationException("Voting session has expired");
            }
            if (request.VoterId == session.BirthdayPersonId)
            {
                throw new ValidationException("Birthday person cannot vote for themselves");
            }
            
            var hasVoted = await UserHasVotedAsync(request.VotingSessionId, request.VoterId);
            if (hasVoted)
            {
                throw new ValidationException("Employee has already voted");
            }
            var vote = new Models.Vote
            {
                VotingSessionId = request.VotingSessionId,
                VoterId = request.VoterId,
                GiftId = request.GiftId,
                VoteDate = DateTime.Now
            };
            var voteId = await _voteRepository.CreateAsync(vote);
            return await GetVoteByIdAsync(voteId);
        }

        private async Task<VoteDto> GetVoteByIdAsync(int voteId)
        {
            var vote = await _voteRepository.RetrieveAsync(voteId);
            return await MapToDtoAsync(vote);
        }

        public async Task<IEnumerable<VoteDto>> GetVotesByVotingSessionIdAsync(int votingSessionId)
        {
            var filter = new VoteFilter
            {
                VotingSessionId = new SqlInt32(votingSessionId)
            };
            var votes = await _voteRepository.RetrieveCollectionAsync(filter).ToListAsync();

            var voteDtos = new List<VoteDto>();
            foreach (var vote in votes)
            {
                voteDtos.Add(await MapToDtoAsync(vote));
            }
            return voteDtos;
        }

        public async Task<bool> UserHasVotedAsync(int votingSessionId, int employeeId)
        {
            var filter = new VoteFilter
            {
                VoterId = new SqlInt32(employeeId),
                VotingSessionId = new SqlInt32(votingSessionId)
            };
            return await _voteRepository.RetrieveCollectionAsync(filter).AnyAsync();
        }
    }
}