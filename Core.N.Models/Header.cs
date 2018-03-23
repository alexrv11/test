namespace Core.N.Models
{
    public class Header
    {
        public string IdOperation { get; set; } 
        public Status Status { get; set; }
        public string ErrorCode { get; set; }
        public string Description { get;set; }
    }

    public enum Status {
        Ok,
        Error,
        Info,
        Warning
    }
}
