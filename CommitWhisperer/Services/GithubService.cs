using Octokit;

namespace CommitWhisperer.Services
{
    public class GithubService
    {
        private readonly IGitHubClient _client;

        public GithubService(IGitHubClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IReadOnlyList<GitHubCommit>> GetCommitsAsync(string username, string repository)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(repository))
            {
                throw new ArgumentNullException(nameof(repository), "Repository name cannot be null or empty");
            }

            try
            {
                return await _client.Repository.Commit.GetAll(username, repository);
            }
            catch (NotFoundException)
            {
                Console.WriteLine("Repository or user not found");
                return Array.Empty<GitHubCommit>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving commits: {ex.Message}");
                return Array.Empty<GitHubCommit>();
            }
        }
    }
}