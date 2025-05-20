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

                // First user login
                Console.WriteLine("\nLogin as first user:");
                Console.Write("Username: ");
                var username1 = Console.ReadLine();
                Console.Write("Password: ");
                var password1 = Console.ReadLine();

                var loginResult1 = await authService.LoginAsync(new LoginRequest 
                { 
                    Username = username1, 
                    Password = password1 
                });

                if (!loginResult1.Success)
                {
                    Console.WriteLine($"Login failed: {loginResult1.Message}");
                    return;
                }

                Console.WriteLine($"\nSuccessfully logged in as: {loginResult1.FullName}");

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
                    Console.WriteLine("8. Exit");
                    Console.Write("\nSelect an option: ");

                    var choice = Console.ReadLine();
                    Console.WriteLine();

                    switch (choice)
                    {
                        case "1":
                            // List Employees
                            var employees = await employeeService.GetAllEmployeesAsync();
                            Console.WriteLine("Available employees:");
                            foreach (var emp in employees)
                            {
                                Console.WriteLine($"ID: {emp.EmployeeId}, Name: {emp.FullName}, Username: {emp.Username}");
                            }
                            break;

                        case "2":
                            // List Gifts
                            var gifts = await giftService.GetAllGiftsAsync();
                            Console.WriteLine("Available gifts:");
                            foreach (var gift in gifts)
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
                            var filteredSessions = activeSessions.Where(s => s.BirthdayPersonId != loginResult1.UserId);
                            
                            Console.WriteLine("Active voting sessions:");
                            var hasAnySessions = false;
                            foreach (var session in filteredSessions)
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
                                    if (voteSession.BirthdayPersonId == loginResult1.UserId)
                                    {
                                        Console.WriteLine("You cannot view votes for your own birthday session!");
                                        break;
                                    }

                                    var votes = await voteService.GetVotesByVotingSessionIdAsync(voteSessionId);
                                    Console.WriteLine($"\nVotes in session {voteSessionId}:");
                                    foreach (var vote in votes)
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
                                    var result = await votingSessionService.EndVotingSessionAsync(endSessionId, loginResult1.UserId.Value);
                                    if (result)
                                    {
                                        Console.WriteLine("Voting session ended successfully!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to end voting session.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error ending session: {ex.Message}");
                                }
                            }
                            break;

                        case "8":
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
