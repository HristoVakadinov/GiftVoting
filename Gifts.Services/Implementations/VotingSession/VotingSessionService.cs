using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Repository.Interfaces.Employee;
using Gifts.Repository.Interfaces.VotingSession;
using Gifts.Services.DTOs.VotingSession;
using Gifts.Services.Interfaces.VotingSession;

namespace Gifts.Services.Implementations.VotingSession
{
    public class VotingSessionService : IVotingSessionService
    {
        private readonly IVotingSessionRepository _votingSessionRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public VotingSessionService(IVotingSessionRepository votingSessionRepository, IEmployeeRepository employeeRepository)
        {
            _votingSessionRepository = votingSessionRepository;
            _employeeRepository = employeeRepository;
        }

        private async Task<VotingSessionInfo> MapToVotingSessionInfoAsync(Models.VotingSession votingSession)
        {
            var birthdayPerson = await _employeeRepository.RetrieveAsync(votingSession.BirthdayPersonId);
            var createdBy = await _employeeRepository.RetrieveAsync(votingSession.CreatedById);
            return new VotingSessionInfo
            {
                VotingSessionId = votingSession.VotingSessionId,
                BirthdayPersonId = votingSession.BirthdayPersonId,
                BirthdayPersonName = birthdayPerson.FullName,
                CreatedById = votingSession.CreatedById,
                CreatedByFullName = createdBy.FullName,
                StartDate = votingSession.StartDate,
                EndDate = votingSession.EndDate,
                IsActive = votingSession.IsActive,
                BirthYear = votingSession.BirthYear
            };
        }

        public async Task<CreateVotingSessionResponse> CreateVotingSessionAsync(CreateVotingSessionRequest request)
        {
            if (request.CreatedById == request.BirthdayPersonId)
            {
                return new CreateVotingSessionResponse()
                {
                    Success = false,
                    Message = "You cannot create a voting session for yourself"
                };
            }

            var activeSessionFilter = new VotingSessionFilter
            {
                BirthdayPersonId = new SqlInt32(request.BirthdayPersonId),
                IsActive = SqlBoolean.True
            };

            var hasActiveSession = await _votingSessionRepository
                .RetrieveCollectionAsync(activeSessionFilter)
                .AnyAsync();

            if (hasActiveSession)
            {
                return new CreateVotingSessionResponse()
                {
                    Success = false,
                    Message = "There is already an active voting session for this person"
                };
            }

            var currentYear = DateTime.Now.Year;
            var yearSessionFilter = new VotingSessionFilter
            {
                BirthdayPersonId = new SqlInt32(request.BirthdayPersonId)
            };

            var yearSessions = await _votingSessionRepository
                .RetrieveCollectionAsync(yearSessionFilter)
                .ToListAsync();

            if (yearSessions.Any(s => s.BirthYear == currentYear))
            {
                return new CreateVotingSessionResponse()
                {
                    Success = false,
                    Message = "There is already a voting session for this person this year"
                };
            }

            var session = new Models.VotingSession
            {
                BirthdayPersonId = request.BirthdayPersonId,
                CreatedById = request.CreatedById,
                StartDate = DateTime.Now,
                IsActive = true,
                BirthYear = currentYear
            };

            int sessionId = await _votingSessionRepository.CreateAsync(session);

            session.VotingSessionId = sessionId;
            var sessionInfo = await MapToVotingSessionInfoAsync(session);
            return new CreateVotingSessionResponse
            {
                VotingSessionId = sessionInfo.VotingSessionId,
                BirthdayPersonId = sessionInfo.BirthdayPersonId,
                BirthdayPersonName = sessionInfo.BirthdayPersonName,
                CreatedById = sessionInfo.CreatedById,
                CreatedByFullName = sessionInfo.CreatedByFullName,
                StartDate = sessionInfo.StartDate,
                EndDate = sessionInfo.EndDate,
                IsActive = sessionInfo.IsActive,
                BirthYear = sessionInfo.BirthYear,
                Success = true
            };
        }

        public async Task<EndVotingSessionResponse> EndVotingSessionAsync(EndVotingSessionRequest request)
        {
            var session = await _votingSessionRepository.RetrieveAsync(request.VotingSessionId);

            if (session.CreatedById != request.EndedById)
            {
                return new EndVotingSessionResponse()
                {
                    Success = false,
                    Message = "Only the creator can end the voting session"
                };
            }

            var update = new VotingSessionUpdate
            {
                IsActive = SqlBoolean.False,
                EndDate = new SqlDateTime(DateTime.Now)
            };

            var isUpdated =  await _votingSessionRepository.UpdateAsync(request.VotingSessionId, update);

            if (isUpdated)
            {
                return new EndVotingSessionResponse()
                {
                    Success = true,
                    EndedAt = (DateTime)update.EndDate
                };
            }
            else
            {
                return new EndVotingSessionResponse()
                {
                    Success = false,
                    Message = "Unable to end the voting session"
                };
            }

        }

        public async Task<GetVotingSessionsResponse> GetActiveSessionForEmployeeAsync(int employeeId)
        {
            var filter = new VotingSessionFilter { BirthdayPersonId = new SqlInt32(employeeId), IsActive = SqlBoolean.True };
            var sessions = await _votingSessionRepository.RetrieveCollectionAsync(filter).ToListAsync();
            var session = sessions.FirstOrDefault();
            if (session == null) return null;

            return (GetVotingSessionsResponse)await MapToVotingSessionInfoAsync(session);
        }

        public async Task<GetActiveVotingSessionsResponse> GetAllActiveVotingSessionsAsync()
        {
            var filter = new VotingSessionFilter { IsActive = SqlBoolean.True };
            var sessions = await _votingSessionRepository.RetrieveCollectionAsync(filter).ToListAsync();
            var response = new GetActiveVotingSessionsResponse
            {
                ActiveSessions = new List<VotingSessionInfo>(),
                TotalCount = sessions.Count
            };
            foreach (var session in sessions)
            {
                response.ActiveSessions.Add(await MapToVotingSessionInfoAsync(session));
            }
            return response;
        }

        public async Task<GetVotingSessionsResponse> GetVotingSessionByIdAsync(int sessionId)
        {
            var session = await _votingSessionRepository.RetrieveAsync(sessionId);
            if (session == null) return null;
            
            var sessionInfo = await MapToVotingSessionInfoAsync(session);
            return new GetVotingSessionsResponse
            {
                VotingSessionId = sessionInfo.VotingSessionId,
                BirthdayPersonId = sessionInfo.BirthdayPersonId,
                BirthdayPersonName = sessionInfo.BirthdayPersonName,
                CreatedById = sessionInfo.CreatedById,
                CreatedByFullName = sessionInfo.CreatedByFullName,
                StartDate = sessionInfo.StartDate,
                EndDate = sessionInfo.EndDate,
                IsActive = sessionInfo.IsActive,
                BirthYear = sessionInfo.BirthYear
            };
        }
    }
}