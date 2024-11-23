using CommitWhisperer.Data;
using CommitWhisperer.Models;

namespace CommitWhisperer.Repositories
{
    public class CommitRepository
    {
        private readonly AppDbContext _context;

        public CommitRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null!");
            _context.Database.EnsureCreated();
        }

        public void AddCommits(IEnumerable<CommitInfo> commits)
        {
            if (commits == null)
            {
                throw new ArgumentNullException(nameof(commits), "Commits collection cannot be null!");
            }

            var newCommits = commits.Where(commit => !_context.Commits.Any(c => c.Sha == commit.Sha)).ToList();

            if (newCommits.Any())
            {
                _context.Commits.AddRange(newCommits);
                _context.SaveChanges();
            }
        }
    }
}