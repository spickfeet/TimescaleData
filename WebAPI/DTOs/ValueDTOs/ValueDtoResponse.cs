namespace WebAPI.DTOs.ValueDTOs
{
    public class ValueDtoResponse
    {
        public Guid Id { get; set; }
        public string FileName {  get; set; }
        public DateTime Date { get; set; }
        public double ExecutionTime { get; set; }
        public double Value { get; set; }
    }
}
