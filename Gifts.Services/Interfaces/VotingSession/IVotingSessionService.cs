using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Services.DTOs.VotingSession;

namespace Gifts.Services.Interfaces.VotingSession
{
    public interface IVotingSessionService
    {
        Task<GetVotingSessionsResponse> GetVotingSessionByIdAsync(int sessionId);
        Task<GetActiveVotingSessionsResponse> GetAllActiveVotingSessionsAsync();
        Task<GetActiveVotingSessionsResponse> GetCompletedSessionsAsync();
        Task<CreateVotingSessionResponse> CreateVotingSessionAsync(CreateVotingSessionRequest request);
        Task<EndVotingSessionResponse> EndVotingSessionAsync(EndVotingSessionRequest request);
        Task<GetVotingSessionsResponse> GetActiveSessionForEmployeeAsync(int employeeId);
    }
}