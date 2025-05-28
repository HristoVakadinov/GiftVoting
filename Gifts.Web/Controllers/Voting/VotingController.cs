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
using Gifts.Web.Models.ViewModels.Gift;
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

        [HttpGet]
        public async Task<IActionResult> CreateSession(int birthdayPersonId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(birthdayPersonId);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee not found.";
                return RedirectToAction("Index", "Employee");
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId").Value;
            
            // Check if trying to create session for themselves
            if (birthdayPersonId == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot create a voting session for yourself.";
                return RedirectToAction("Index", "Employee");
            }

            // Check if there's already an active session for this person
            var activeSession = await _votingSessionService.GetActiveSessionForEmployeeAsync(birthdayPersonId);
            if (activeSession != null)
            {
                TempData["ErrorMessage"] = "There is already an active voting session for this person.";
                return RedirectToAction("Index", "Employee");
            }

            var viewModel = new CreateVotingSessionViewModel
            {
                BirthdayPersonId = birthdayPersonId,
                BirthdayPersonName = employee.FullName
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession(CreateVotingSessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    var employee = await _employeeService.GetEmployeeByIdAsync(model.BirthdayPersonId);
                    model.BirthdayPersonName = employee?.FullName ?? "Unknown";
                }
                catch
                {
                    model.BirthdayPersonName = "Unknown";
                }
                return View(model);
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId").Value;

            var request = new CreateVotingSessionRequest
            {
                BirthdayPersonId = model.BirthdayPersonId,
                CreatedById = currentUserId
            };

            var result = await _votingSessionService.CreateVotingSessionAsync(request);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                try
                {
                    var employee = await _employeeService.GetEmployeeByIdAsync(model.BirthdayPersonId);
                    model.BirthdayPersonName = employee?.FullName ?? "Unknown";
                }
                catch
                {
                    model.BirthdayPersonName = "Unknown";
                }
                return View(model);
            }

            TempData["SuccessMessage"] = $"Voting session created successfully for {result.BirthdayPersonName}!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Vote(int sessionId)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId").Value;
            
            // Get the voting session
            var session = await _votingSessionService.GetVotingSessionByIdAsync(sessionId);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Voting session not found.";
                return RedirectToAction("Index");
            }

            // Check if session is active
            if (!session.IsActive)
            {
                TempData["ErrorMessage"] = "This voting session is no longer active.";
                return RedirectToAction("Index");
            }

            // Check if user is the birthday person
            if (session.BirthdayPersonId == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot vote in your own birthday session.";
                return RedirectToAction("Index");
            }

            // Check if user has already voted
            var hasVoted = await _voteService.UserHasVotedAsync(new HasUserVotedRequest
            {
                VotingSessionId = sessionId,
                EmployeeId = currentUserId
            });

            if (hasVoted.HasVoted)
            {
                TempData["ErrorMessage"] = "You have already voted in this session.";
                return RedirectToAction("Index");
            }

            // Get available gifts
            var gifts = await _giftService.GetAllGiftsAsync();

            var viewModel = new VoteViewModel
            {
                VotingSessionId = sessionId,
                BirthdayPersonName = session.BirthdayPersonName,
                AvailableGifts = gifts.Gifts.Select(g => new GiftViewModel
                {
                    GiftId = g.GiftId,
                    GiftName = g.Name,
                    Description = g.Description,
                    Price = g.Price
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Vote(int sessionId, int giftId)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId").Value;

            var request = new CreateVoteRequest
            {
                VotingSessionId = sessionId,
                VoterId = currentUserId,
                GiftId = giftId
            };

            var result = await _voteService.CreateVoteAsync(request);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Vote", new { sessionId });
            }

            TempData["SuccessMessage"] = $"Your vote for {result.GiftName} has been registered successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EndSession(int sessionId)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId").Value;

            var request = new EndVotingSessionRequest
            {
                VotingSessionId = sessionId,
                EndedById = currentUserId
            };

            var result = await _votingSessionService.EndVotingSessionAsync(request);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Voting session ended successfully!";
            }

            return RedirectToAction("Index");
        }
    }
}