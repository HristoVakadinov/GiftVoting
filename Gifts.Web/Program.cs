using Gifts.Repository;
using Gifts.Repository.Implementations.Employee;
using Gifts.Repository.Implementations.Gift;
using Gifts.Repository.Implementations.Vote;
using Gifts.Repository.Implementations.VotingSession;
using Gifts.Repository.Interfaces.Employee;
using Gifts.Repository.Interfaces.Gift;
using Gifts.Repository.Interfaces.Vote;
using Gifts.Repository.Interfaces.VotingSession;
using Gifts.Services.Interfaces.Authentication;
using Gifts.Services.Implementations.Authentication;
using Gifts.Services.Interfaces.Employee;
using Gifts.Services.Implementations.Employee;
using Gifts.Services.Interfaces.Gift;
using Gifts.Services.Implementations.Gift;
using Gifts.Services.Interfaces.Vote;
using Gifts.Services.Implementations.Vote;
using Gifts.Services.Interfaces.VotingSession;
using Gifts.Services.Implementations.VotingSession;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<IVotingSessionService, VotingSessionService>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<IVotingSessionRepository, VotingSessionRepository>();

ConnectionFactory.Initialize(
    builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
