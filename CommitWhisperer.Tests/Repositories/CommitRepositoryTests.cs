using CommitWhisperer.Data;
using CommitWhisperer.Models;
using CommitWhisperer.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CommitWhisperer.Tests.Repositories
{
    public class CommitRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly CommitRepository _repository;

        public CommitRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new CommitRepository(_context);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CommitRepository(null));
        }

        [Fact]
        public void AddCommits_ShouldThrowArgumentNullException_WhenCommitsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.AddCommits(null));
        }

        [Fact]
        public void AddCommits_ShouldAddNewCommits_WhenNoDuplicatesExist()
        {
            var commits = new List<CommitInfo>
            {
                new CommitInfo { Sha = "sha1", Username = "username1", RepositoryName = "repoName1", Message = "Initial commit", Committer = "committer1" },
                new CommitInfo { Sha = "sha2", Username = "username2", RepositoryName = "repoName2", Message = "Second commit", Committer = "committer2" }
            };

            _repository.AddCommits(commits);

            var savedCommits = _context.Commits.ToList();
            Assert.Equal(2, savedCommits.Count);
            Assert.Contains(savedCommits, c => c.Sha == "sha1");
            Assert.Contains(savedCommits, c => c.Sha == "sha2");
        }

        [Fact]
        public void AddCommits_ShouldNotAddDuplicates_WhenCommitsAlreadyExist()
        {
            var existingCommit = new CommitInfo { Sha = "sha1", Username = "username1", RepositoryName = "repoName1", Message = "Initial commit", Committer = "committer1" };
            _context.Commits.Add(existingCommit);
            _context.SaveChanges();

            var newCommits = new List<CommitInfo>
            {
                new CommitInfo { Sha = "sha1", Username = "username1", RepositoryName = "repoName1", Message = "Duplicate commit", Committer = "committer1" },
                new CommitInfo { Sha = "sha2", Username = "username2", RepositoryName = "repoName2", Message = "New commit", Committer = "committer2" }
            };

            _repository.AddCommits(newCommits);

            var savedCommits = _context.Commits.ToList();
            Assert.Equal(2, savedCommits.Count);
            Assert.Contains(savedCommits, c => c.Sha == "sha1");
            Assert.Contains(savedCommits, c => c.Sha == "sha2");
        }

        [Fact]
        public void AddCommits_ShouldNotSave_WhenAllCommitsAlreadyExist()
        {
            var existingCommits = new List<CommitInfo>
            {
                new CommitInfo { Sha = "sha1", Username = "username1", RepositoryName = "repoName1", Message = "Initial commit", Committer = "committer1" },
                new CommitInfo { Sha = "sha2", Username = "username2", RepositoryName = "repoName2", Message = "Second commit", Committer = "committer2" }
            };

            _context.Commits.AddRange(existingCommits);
            _context.SaveChanges();

            var duplicateCommits = new List<CommitInfo>
            {
                new CommitInfo { Sha = "sha1", Username = "username1", RepositoryName = "repoName1", Message = "Duplicate commit", Committer = "committer1" },
                new CommitInfo { Sha = "sha2", Username = "username2", RepositoryName = "repoName2", Message = "Duplicate commit", Committer = "committer2" }
            };

            _repository.AddCommits(duplicateCommits);

            var savedCommits = _context.Commits.ToList();
            Assert.Equal(2, savedCommits.Count);
        }

        [Fact]
        public void AddCommits_ShouldWorkWithEmptyList()
        {
            var commits = new List<CommitInfo>();

            _repository.AddCommits(commits);

            Assert.Empty(_context.Commits);
        }

        [Fact]
        public void AddCommits_ShouldWorkWithLargeDataset()
        {
            var commits = Enumerable.Range(1, 1000).Select(i => new CommitInfo
            {
                Sha = $"sha{i}",
                Username = "username",
                RepositoryName = "repoName",
                Message = $"Commit message {i}",
                Committer = "committer"
            }).ToList();

            _repository.AddCommits(commits);

            Assert.Equal(1000, _context.Commits.Count());
        }

        [Fact]
        public void AddCommits_ShouldHandleDatabaseException()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            using var context = new AppDbContext(options);

            var mockContext = new Mock<AppDbContext>(options) { CallBase = true };
            mockContext.Setup(c => c.SaveChanges()).Throws(new Exception("Database is not available"));

            var repository = new CommitRepository(mockContext.Object);

            var commits = new List<CommitInfo>
                {
                    new CommitInfo { Sha = "sha1", Username = "username", RepositoryName = "repoName", Message = "Commit 1", Committer = "committer" }
                };

            var exception = Assert.Throws<Exception>(() => repository.AddCommits(commits));
            Assert.Equal("Database is not available", exception.Message);
        }
    }
}