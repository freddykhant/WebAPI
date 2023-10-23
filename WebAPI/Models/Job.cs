namespace WebAPI.Models
{
    public class Job
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Status { get; set; }
        public DateTime CompletionTime { get; set; }
    }
}
