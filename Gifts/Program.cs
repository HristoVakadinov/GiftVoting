using System.Text;
using Gifts.Models;
using Gifts.Repository.Helpers;
using Gifts.Repository.Interfaces.Employee;
using Gifts.Repository.Interfaces.Gift;
using Gifts.Repository.Interfaces.Vote;
using Gifts.Repository.Interfaces.VotingSession;
using Gifts.Repository;
using Microsoft.Extensions.Configuration;
using Gifts.Repository.Implementations.Employee;
using Gifts.Repository.Implementations.Gift;
using Gifts.Repository.Implementations.Vote;
using Gifts.Repository.Implementations.VotingSession;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Gifts.Services.Interfaces.Employee;
using Gifts.Services.Implementations.Employee;
using Gifts.Services.Interfaces.Gift;
using Gifts.Services.Implementations.Gift;
using Gifts.Services.DTOs.Employee;
using Gifts.Services.DTOs.Gift;
using Gifts.Services.DTOs.Authentication;
using Gifts.Services.Interfaces.Authentication;
using Gifts.Services.Implementations.Authentication;
using Gifts.Services.Interfaces.Vote;
using Gifts.Services.Implementations.Vote;
using Gifts.Services.Interfaces.VotingSession;
using Gifts.Services.Implementations.VotingSession;
using Gifts.Services.DTOs.Vote;
using Gifts.Services.DTOs.VotingSession;

namespace Gifts.Services
{
    public class Program
    {
        static async Task<LoginResponce> PerformLogin(IAuthenticationService authService)
        {
            Console.WriteLine("\nLogin:");
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();

            var loginResult = await authService.LoginAsync(new LoginRequest 
            { 
                Username = username, 
                Password = password 
            });

            if (!loginResult.Success)
            {
                Console.WriteLine($"Login failed: {loginResult.Message}");
                return null;
            }

            Console.WriteLine($"\nSuccessfully logged in as: {loginResult.FullName}");
            return loginResult;
        }

        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            ConnectionFactory.Initialize(connectionString);

            // Setup dependency injection
            var services = new ServiceCollection();
            
            // Register repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IGiftRepository, GiftRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<IVotingSessionRepository, VotingSessionRepository>();

            // Register services
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IGiftService, GiftService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<IVotingSessionService, VotingSessionService>();

            var serviceProvider = services.BuildServiceProvider();

            // Get service instances
            var employeeService = serviceProvider.GetRequiredService<IEmployeeService>();
            var giftService = serviceProvider.GetRequiredService<IGiftService>();
            var authService = serviceProvider.GetRequiredService<IAuthenticationService>();
            var voteService = serviceProvider.GetRequiredService<IVoteService>();
            var votingSessionService = serviceProvider.GetRequiredService<IVotingSessionService>();

            try
            {
                Console.WriteLine("Successfully connected to database!");

                var loginResult1 = await PerformLogin(authService);
                if (loginResult1 == null) return;

                while (true)
                {
                    Console.WriteLine("\nMain Menu:");
                    Console.WriteLine("1. List Employees");
                    Console.WriteLine("2. List Gifts");
                    Console.WriteLine("3. Create Voting Session");
                    Console.WriteLine("4. View Active Voting Sessions");
                    Console.WriteLine("5. Vote in Session");
                    Console.WriteLine("6. View Session Votes");
                    Console.WriteLine("7. End Voting Session");
                    Console.WriteLine("8. Change User");
                    Console.WriteLine("9. Exit");
                    Console.Write("\nSelect an option: ");

                    var choice = Console.ReadLine();
                    Console.WriteLine();

                    switch (choice)
                    {
                        case "1":
                            // List Employees
                            var employeesResponse = await employeeService.GetAllEmployeesAsync();
                            if (employeesResponse?.Employees == null || employeesResponse.Employees.Count == 0)
                            {
                                Console.WriteLine("No employees found.");
                                break;
                            }
                            Console.WriteLine("Available employees:");
                            foreach (var emp in employeesResponse.Employees)
                            {
                                Console.WriteLine($"ID: {emp.EmployeeId}, Name: {emp.FullName}, Days till birthday: {emp.DaysTillNextBirthday}");
                            }
                            break;

                        case "2":
                            // List Gifts
                            var giftsResponse = await giftService.GetAllGiftsAsync();
                            if (giftsResponse?.Gifts == null || giftsResponse.Gifts.Count == 0)
                            {
                                Console.WriteLine("No gifts found.");
                                break;
                            }
                            Console.WriteLine("Available gifts:");
                            foreach (var gift in giftsResponse.Gifts)
                            {
                                Console.WriteLine($"ID: {gift.GiftId}, Name: {gift.Name}, Price: {gift.Price}");
                            }
                            break;

                        case "3":
                            // Create Voting Session
                            Console.Write("Enter birthday person ID: ");
                            if (int.TryParse(Console.ReadLine(), out int birthdayPersonId))
                            {
                                try
                                {
                                    var createSessionRequest = new CreateVotingSessionRequest
                                    {
                                        BirthdayPersonId = birthdayPersonId,
                                        CreatedById = loginResult1.UserId.Value
                                    };

                                    var session = await votingSessionService.CreateVotingSessionAsync(createSessionRequest);
                                    if (!session.Success)
                                    {
                                        Console.WriteLine($"Failed to create session: {session.Message}");
                                        break;
                                    }
                                    Console.WriteLine($"Created voting session with ID: {session.VotingSessionId} for {session.BirthdayPersonName}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error creating session: {ex.Message}");
                                }
                            }
                            break;

                        case "4":
                            // View Active Sessions
                            var activeSessions = await votingSessionService.GetAllActiveVotingSessionsAsync();
                            if (activeSessions?.ActiveSessions == null || activeSessions.ActiveSessions.Count == 0)
                            {
                                Console.WriteLine("No active voting sessions found.");
                                break;
                            }
                            Console.WriteLine("Active voting sessions:");
                            var hasAnySessions = false;
                            foreach (var session in activeSessions.ActiveSessions.Where(s => s.BirthdayPersonId != loginResult1.UserId))
                            {
                                hasAnySessions = true;
                                Console.WriteLine($"Session ID: {session.VotingSessionId}, Birthday Person: {session.BirthdayPersonName}, Created By: {session.CreatedByFullName}");
                            }
                            if (!hasAnySessions)
                            {
                                Console.WriteLine("No active voting sessions available for you.");
                            }
                            break;

                        case "5":
                            // Vote in Session
                            Console.Write("Enter voting session ID: ");
                            if (int.TryParse(Console.ReadLine(), out int sessionId))
                            {
                                try
                                {
                                    // First check if this is the user's birthday session
                                    var session = await votingSessionService.GetVotingSessionByIdAsync(sessionId);
                                    if (session == null)
                                    {
                                        Console.WriteLine("Session not found.");
                                        break;
                                    }
                                    if (session.BirthdayPersonId == loginResult1.UserId)
                                    {
                                        Console.WriteLine("You cannot vote in your own birthday session!");
                                        break;
                                    }

                                    Console.Write("Enter gift ID to vote for: ");
                                    if (int.TryParse(Console.ReadLine(), out int giftId))
                                    {
                                        var voteRequest = new CreateVoteRequest
                                        {
                                            VotingSessionId = sessionId,
                                            VoterId = loginResult1.UserId.Value,
                                            GiftId = giftId
                                        };

                                        var vote = await voteService.CreateVoteAsync(voteRequest);
                                        if (!vote.Success)
                                        {
                                            Console.WriteLine($"Failed to create vote: {vote.Message}");
                                            break;
                                        }
                                        Console.WriteLine($"Vote registered successfully! Voted for gift: {vote.GiftName}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error voting: {ex.Message}");
                                }
                            }
                            break;

                        case "6":
                            // View Session Votes
                            Console.Write("Enter voting session ID: ");
                            if (int.TryParse(Console.ReadLine(), out int voteSessionId))
                            {
                                try
                                {
                                    // Check if this is the user's birthday session
                                    var voteSession = await votingSessionService.GetVotingSessionByIdAsync(voteSessionId);
                                    if (voteSession == null)
                                    {
                                        Console.WriteLine("Session not found.");
                                        break;
                                    }
                                    if (voteSession.BirthdayPersonId == loginResult1.UserId)
                                    {
                                        Console.WriteLine("You cannot view votes for your own birthday session!");
                                        break;
                                    }

                                    var votesResponse = await voteService.GetVotesByVotingSessionIdAsync(voteSessionId);
                                    if (votesResponse?.Votes == null || votesResponse.Votes.Count == 0)
                                    {
                                        Console.WriteLine("No votes found for this session.");
                                        break;
                                    }
                                    Console.WriteLine($"\nVotes in session {voteSessionId}:");
                                    foreach (var vote in votesResponse.Votes)
                                    {
                                        Console.WriteLine($"Voter: {vote.VoterName}, Gift: {vote.GiftName}, Vote Date: {vote.VoteDate}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error retrieving votes: {ex.Message}");
                                }
                            }
                            break;

                        case "7":
                            // End Voting Session
                            Console.Write("Enter voting session ID to end: ");
                            if (int.TryParse(Console.ReadLine(), out int endSessionId))
                            {
                                try
                                {
                                    var endSessionRequest = new EndVotingSessionRequest
                                    {
                                        VotingSessionId = endSessionId,
                                        EndedById = loginResult1.UserId.Value
                                    };
                                    
                                    var result = await votingSessionService.EndVotingSessionAsync(endSessionRequest);
                                    if (result.Success)
                                    {
                                        Console.WriteLine("Voting session ended successfully!");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Failed to end voting session: {result.Message}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error ending session: {ex.Message}");
                                }
                            }
                            break;

                        case "8":
                            // Change User
                            loginResult1 = await PerformLogin(authService);
                            if (loginResult1 == null) return;
                            break;

                        case "9":
                            Console.WriteLine("Goodbye!");
                            return;

                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError occurred:");
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception:");
                    Console.WriteLine(ex.InnerException.Message);
                }
            }
        }
    }
}
