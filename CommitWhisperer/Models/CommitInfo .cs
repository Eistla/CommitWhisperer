namespace CommitWhisperer.Models
{
    public class CommitInfo
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string RepositoryName { get; set; } = string.Empty;
        public string Sha { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Committer { get; set; } = string.Empty;
    }
}
