using Gifts.Services.Implementations.Vote;
using Gifts.Services.Interfaces.Vote;
using Gifts.Services.Interfaces.VotingSession;
using Gifts.Web.Attributes;
using Gifts.Web.Models.ViewModels.Voting;
using Microsoft.AspNetCore.Mvc;

namespace Gifts.Web.Controllers.Home
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IVotingSessionService _votingSessionService;
        private readonly IVoteService _voteService;

        public HomeController(
            IVotingSessionService votingSessionService,
            IVoteService voteService
        )
        {
            _votingSessionService = votingSessionService;
            _voteService = voteService;
        }

        public async Task<IActionResult> Index()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var completedSessions = await _votingSessionService.GetCompletedSessionsAsync();
            var viewModel = new List<VotingSessionViewModel>();

            foreach (var session in completedSessions.ActiveSessions)
            {
                var votes = await _voteService.GetVotesByVotingSessionIdAsync(session.VotingSessionId);
                viewModel.Add(new VotingSessionViewModel
                {
                    VotingSessionId = session.VotingSessionId,
                    BirthdayPersonName = session.BirthdayPersonName,
                    CreatedDate = session.StartDate,
                    IsActive = session.IsActive,
                    EndDate = session.EndDate,
                    VotesCount = votes.Votes.Count,
                    CreatedByName = session.CreatedByFullName,
                });
            }

            return View(viewModel.OrderByDescending(v => v.EndDate));
        }

        public async Task<IActionResult> SessionResults(int sessionId)
        {
            var session = await _votingSessionService.GetVotingSessionByIdAsync(sessionId);
            var votes = await _voteService.GetVotesByVotingSessionIdAsync(sessionId);

            var voteResults = votes.Votes
            .GroupBy(v => new {v.GiftId, v.GiftName})
            .Select(g => new VoteResultViewModel
            {
                GiftName = g.Key.GiftName,
                VotesCount = g.Count(),
                Percentage = (double)g.Count() / votes.Votes.Count * 100
            })
            .OrderByDescending(v => v.VotesCount)
            .ToList();

            var viewModel = new SessionResultViewModel
            {
                SessionId = sessionId,
                BirthdayPersonName = session.BirthdayPersonName,
                CreatedByName = session.CreatedByFullName,
                CreatedDate = session.StartDate,
                EndDate = session.EndDate.Value,
                TotalVotes = votes.Votes.Count,
                VoteResults = voteResults
            };

            return View(viewModel);
        }
    }
}