using CommitWhisperer.Data;
using CommitWhisperer.Models;
using CommitWhisperer.Repositories;
using CommitWhisperer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

var serviceProvider = new ServiceCollection()
    .AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlite("Data Source=commits.db");
    })
    .AddSingleton<CommitRepository>()
    .AddSingleton<IGitHubClient>(provider =>
    {
        return new GitHubClient(new ProductHeaderValue("CommitWhisperer"));
    })
    .AddSingleton<GithubService>()
    .BuildServiceProvider();

var githubService = serviceProvider.GetRequiredService<GithubService>();
var commitRepository = serviceProvider.GetRequiredService<CommitRepository>();

Console.WriteLine("Enter GitHub name:");
var username = Console.ReadLine();
Console.WriteLine("Enter repository name:");
var repository = Console.ReadLine();

if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(repository))
{
    Console.WriteLine("Invalid input!");
    return;
}

var commits = await githubService.GetCommitsAsync(username, repository);

if (commits.Count > 0)
{
    var commitInfos = commits.Select(commit => new CommitInfo
    {
        Username = username,
        RepositoryName = repository,
        Sha = commit.Sha,
        Message = commit.Commit.Message,
        Committer = commit.Commit.Committer?.Name ?? "Unknown"
    }).ToList();

    commitRepository.AddCommits(commitInfos);

    Console.WriteLine("Commits:");
    foreach (var commitInfo in commitInfos)
    {
        Console.WriteLine($"{commitInfo.RepositoryName}/{commitInfo.Sha}: {commitInfo.Message} [{commitInfo.Committer}]");
    }
}
else
{
    Console.WriteLine("No commits found.");
}