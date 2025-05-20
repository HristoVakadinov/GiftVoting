using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Services.DTOs.VotingSession;

namespace Gifts.Services.Interfaces.VotingSession
{
    public interface IVotingSessionService
    {
        Task<VotingSessionDto> GetVotingSessionByIdAsync(int votingSessionId);
        Task<IEnumerable<VotingSessionDto>> GetAllActiveVotingSessionsAsync();
        Task<VotingSessionDto> CreateVotingSessionAsync(CreateVotingSessionRequest request);
        Task<bool> EndVotingSessionAsync(int votingSessionId, int requestorId );
        Task<VotingSessionDto> GetActiveSessionForEmployeeAsync(int employeeId);
    }
}