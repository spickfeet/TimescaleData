namespace WebAPI.Validations.Primitives
{
    public class Fault
    {
        public string Error { get; set; }
        public List<Fault> Details { get; set; }
        public Fault(string error)
        {
            Error = error;
            Details = new List<Fault>();
        }
    }
}
