CommitWhisperer is a console application written in C# that fetches commit information from a selected GitHub repository and stores it in a local SQLite database. It demonstrates the use of design patterns, integration with external APIs, and basic database operations.

Features
Fetch GitHub Commits:
- Users provide a GitHub username and repository name.
- The app retrieves commit data using the GitHub API.
  
Save Commits to SQLite Database:
- Commits are saved in a table (Commits) while avoiding duplicates (checked by SHA).
  
Display Commits:
- Prints retrieved commits with their message, author, and repository name in the console.
  
Main Libraries
Octokit:
- Used for GitHub API integration.
- Provides easy access to GitHub commit data.
  
Entity Framework Core:
- ORM for managing the SQLite database.
- Simplifies database operations.
  
SQLite:
- Lightweight, file-based database for storing commit data.
  
Microsoft.Extensions.DependencyInjection:
- Handles dependency injection for modular and testable code.
  
xUnit and Moq:
- Used for unit testing application logic.
  
Project Structure:
- CommitWhisperer.Data: Contains AppDbContext for database setup.
- CommitWhisperer.Models: Defines the CommitInfo model for commit data.
- CommitWhisperer.Repositories: CommitRepository handles database operations.
- CommitWhisperer.Services: GithubService integrates with GitHub API.
- CommitWhisperer.Tests: Includes unit tests for database and API logic.
  
How to Run
Clone the repository:
git clone https://github.com/Eistla/CommitWhisperer.git

Navigate to the project directory:
cd CommitWhisperer

Install dependencies:
dotnet restore

Run the application:
dotnet run --project CommitWhisperer

Run unit tests:
dotnet test

Why These Technologies?
- Octokit: Simplifies GitHub API integration.
- EF Core + SQLite: Lightweight, no need for external database servers.
- Dependency Injection: Ensures modularity and testability.
- xUnit + Moq: Popular and easy-to-use tools for unit testing.

Example Workflow
- User inputs a GitHub username and repository name.
- The app fetches commits from the repository.
- Commits are stored in commits.db (local SQLite file).
- Commits are displayed in the console.
