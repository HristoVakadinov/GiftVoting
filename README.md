# GiftVoting
# 🎁 Birthday Gift Voting App

This is a demo project developed as part of a coding assignment for a **.NET Developer Internship**. The application enables employees to collaboratively select birthday gifts for their colleagues via a structured voting process.

## 📝 Description

The **Birthday Gift Voting App** allows users within an organization to initiate and participate in gift selection for upcoming employee birthdays. Employee profiles and gift options are predefined in the system's database.

### Core Features

- **Initiate a Gift Vote**  
  Any employee can start a vote for a colleague's birthday gift. The vote is visible to all users except the birthday person. Only one active vote per person per birthday/year is allowed.

- **Vote on Gift Suggestions**  
  Employees (excluding the birthday person) can vote once, choosing one of the predefined gift options.

- **End Vote and Reveal Results**  
  The initiator can end the vote at any time. Once ended, results are visible to all (except the birthday person), including vote distribution and participation.

## 🏗️ Architecture & Technologies

- 💻 **.NET Stack**
  - ASP.NET Core MVC (preferred architectural pattern)
  - Entity Framework Core
  - Microsoft SQL Server
- 🎯 **3-Tier Architecture**
  - **Presentation Layer**: Razor Views / Blazor (if applicable)
  - **Business Logic Layer**: Services & Managers
  - **Data Access Layer**: Repository Pattern
- 🛠️ Dependency Injection, Authentication (basic), and simple UI design

## 🚀 Getting Started

To run the project locally:

1. Clone the repository
2. Set up the .NET SDK (>= 6.0)
3. Configure the database (with seed data for employees & gifts)
4. Run the application with `dotnet run`
5. Open in your browser at `https://localhost:5001`


