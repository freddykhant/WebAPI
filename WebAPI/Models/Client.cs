namespace WebAPI.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public Job Job { get; set; }
        public int CompletedJobsCount { get; set; }
    }
}
