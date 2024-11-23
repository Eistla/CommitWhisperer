using System.Net;
using Moq;
using Octokit;

namespace CommitWhisperer.Services.Tests
{
    public class GithubServiceTests
    {
        private readonly Mock<IGitHubClient> _mockGitHubClient;
        private readonly GithubService _githubService;

        public GithubServiceTests()
        {
            _mockGitHubClient = new Mock<IGitHubClient>();
            var mockClient = _mockGitHubClient.Object;

            _githubService = new GithubService(mockClient);
        }

        [Fact]
        public async Task GetCommitsAsync_ShouldThrowArgumentNullException_WhenUsernameIsNullOrEmpty()
        {
            var repository = "test-repo";

            await Assert.ThrowsAsync<ArgumentNullException>(() => _githubService.GetCommitsAsync(null, repository));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _githubService.GetCommitsAsync("", repository));
        }

        [Fact]
        public async Task GetCommitsAsync_ShouldThrowArgumentNullException_WhenRepositoryIsNullOrEmpty()
        {
            var username = "test-user";

            await Assert.ThrowsAsync<ArgumentNullException>(() => _githubService.GetCommitsAsync(username, null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _githubService.GetCommitsAsync(username, ""));
        }

        [Fact]
        public async Task GetCommitsAsync_ShouldReturnEmptyList_WhenRepositoryOrUserNotFound()
        {
            var username = "test-user";
            var repository = "test-repo";

            _mockGitHubClient
                .Setup(client => client.Repository.Commit.GetAll(username, repository))
                .ThrowsAsync(new NotFoundException("Not Found", HttpStatusCode.NotFound));

            var result = await _githubService.GetCommitsAsync(username, repository);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCommitsAsync_ShouldReturnEmptyList_OnGeneralException()
        {
            var username = "test-user";
            var repository = "test-repo";

            _mockGitHubClient
                .Setup(client => client.Repository.Commit.GetAll(username, repository))
                .ThrowsAsync(new Exception("General exception"));

            var result = await _githubService.GetCommitsAsync(username, repository);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}