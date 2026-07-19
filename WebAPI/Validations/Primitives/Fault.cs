namespace WebAPI.Validations.Primitives
{
    public class Fault
    {
        public string Message { get; set; }
        public List<Fault> Details { get; set; }
        public Fault(string message)
        {
            Message = message;
            Details = new List<Fault>();
        }
    }
}
