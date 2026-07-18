namespace WebAPI.Database.Models
{
    public class ValueEntity
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public double ExecutionTime { get; set; }
        public double Value { get; set; }
        public Guid ResultId {  get; set; }
        public ResultEntity Result { get; set; }
    }
}
