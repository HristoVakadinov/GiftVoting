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

        private async Task<VotingSessionDto> MapToDtoAsync(Models.VotingSession votingSession)
        {
            var birthdayPerson = await _employeeRepository.RetrieveAsync(votingSession.BirthdayPersonId);
            var createdBy = await _employeeRepository.RetrieveAsync(votingSession.CreatedById);

            return new VotingSessionDto
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

        public async Task<VotingSessionDto> CreateVotingSessionAsync(CreateVotingSessionRequest request)
        {
            if (request.CreatedById == request.BirthdayPersonId)
            {
                throw new ValidationException("You cannot create a voting session for yourself");
            }

            var filter = new VotingSessionFilter
            {
                BirthdayPersonId = new SqlInt32(request.BirthdayPersonId),
                IsActive = new SqlBoolean(true)
            };

            var existingSession = await _votingSessionRepository.RetrieveCollectionAsync(filter).AnyAsync();
            if (existingSession)
            {
                throw new ValidationException("A voting session already exists for this birthday person");
            }

            var currYear = DateTime.Now.Year;
            var yearSessionFilter = new VotingSessionFilter
            {
                BirthdayPersonId = new SqlInt32(request.BirthdayPersonId),
            };

            var sessionYear = await _votingSessionRepository.RetrieveCollectionAsync(yearSessionFilter).ToListAsync();
            if (sessionYear.Any(x => x.BirthYear == currYear))
            {
                throw new ValidationException("A voting session already exists for this birthday person this year");
            }

            var votingSession = new Models.VotingSession
            {
                BirthdayPersonId = request.BirthdayPersonId,
                CreatedById = request.CreatedById,
                BirthYear = currYear,
                StartDate = DateTime.Now,
                IsActive = true
            };

            var sessionId = await _votingSessionRepository.CreateAsync(votingSession);
            return await GetVotingSessionByIdAsync(sessionId);            
        }

        public async Task<bool> EndVotingSessionAsync(int votingSessionId, int requestorId)
        {
            var session = await _votingSessionRepository.RetrieveAsync(votingSessionId);
            if (session == null)
            {
                throw new ValidationException("Voting session not found");
            }
            
            if(session.CreatedById != requestorId)
            {
                throw new ValidationException("You are not authorized to end this voting session! Only the creator can end the session.");
            }
            var updatedSession = new VotingSessionUpdate
            {
                IsActive = SqlBoolean.False,
                EndDate = new SqlDateTime(DateTime.Now)
            };

            return await _votingSessionRepository.UpdateAsync(votingSessionId, updatedSession);
        }

        public async Task<VotingSessionDto> GetActiveSessionForEmployeeAsync(int employeeId)
        {
            var filter = new VotingSessionFilter
            {
                BirthdayPersonId = new SqlInt32(employeeId),
                IsActive = SqlBoolean.True
            };

            var sessions = await _votingSessionRepository.RetrieveCollectionAsync(filter).ToListAsync();
            if (sessions == null)
            {
                throw new ValidationException("No active voting session found for this employee");
            }

           var session = sessions.FirstOrDefault();
           if (session == null) return null;
           return await MapToDtoAsync(session);
        }

        public async Task<IEnumerable<VotingSessionDto>> GetAllActiveVotingSessionsAsync()
        {
            var filter = new VotingSessionFilter
            {
                IsActive = new SqlBoolean(true)
            };

            var sessions = await _votingSessionRepository.RetrieveCollectionAsync(filter).ToListAsync();
            var sessionDtos = new List<VotingSessionDto>();
            foreach (var session in sessions)
            {
                sessionDtos.Add(await MapToDtoAsync(session));
            }
            return sessionDtos;
        }

        public async Task<VotingSessionDto> GetVotingSessionByIdAsync(int votingSessionId)
        {
            var session = await _votingSessionRepository.RetrieveAsync(votingSessionId);
            if (session == null)
            {
                throw new ValidationException("Voting session not found");
            }

            return await MapToDtoAsync(session);
        }
    }
}