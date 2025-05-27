using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Services.DTOs.Vote;
using Gifts.Services.DTOs.VotingSession;
using Gifts.Services.Interfaces.Employee;
using Gifts.Services.Interfaces.Gift;
using Gifts.Services.Interfaces.Vote;
using Gifts.Services.Interfaces.VotingSession;
using Gifts.Web.Attributes;
using Gifts.Web.Models.ViewModels.Voting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gifts.Web.Controllers.Voting
{
    [Authorize]
    public class VotingController : Controller
    {
        private readonly IVotingSessionService _votingSessionService;
        private readonly IVoteService _voteService;
        private readonly IEmployeeService _employeeService;
        private readonly IGiftService _giftService;

        public VotingController(
            IVotingSessionService votingSessionService,
            IVoteService voteService,
            IEmployeeService employeeService,
            IGiftService giftService
        )
        {
            _votingSessionService = votingSessionService;
            _voteService = voteService;
            _employeeService = employeeService;
            _giftService = giftService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId").Value;
            var sessions = await _votingSessionService.GetAllActiveVotingSessionsAsync();

            var viewModel = new VotingSessionListViewModel
            {
                ActiveVotingSessions = new List<VotingSessionViewModel>()
            };

            foreach (var session in sessions.ActiveSessions
            .Where(s => s.BirthdayPersonId != currentUserId))
            {
                var hasVoted = await _voteService.UserHasVotedAsync(new HasUserVotedRequest
                {
                    VotingSessionId = session.VotingSessionId,
                    EmployeeId = currentUserId
                });

                var votes = await _voteService.GetVotesByVotingSessionIdAsync(session.VotingSessionId);
                
                viewModel.ActiveVotingSessions.Add(new VotingSessionViewModel
                {
                    VotingSessionId = session.VotingSessionId,
                    BirthdayPersonName = session.BirthdayPersonName,
                    CreatedDate = session.StartDate,
                    IsActive = session.IsActive,
                    EndDate = session.EndDate,
                    VotesCount = votes.TotalCount,
                    CreatedByName = session.CreatedByFullName,
                    CreatedById = session.CreatedById,
                    IsCreator = session.CreatedById == currentUserId,
                    HasUserVoted = hasVoted.HasVoted
                });
            }

            viewModel.TotalCount = viewModel.ActiveVotingSessions.Count;

            return View(viewModel);
        }
    }
}