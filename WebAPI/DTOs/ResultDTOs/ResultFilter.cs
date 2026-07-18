namespace WebAPI.DTOs.ResultDTOs
{
    public class ResultFilter
    {
        public string? FileName { get; set; }
        public DateTime? FirstOperationDateAfter { get; set; }
        public DateTime? FirstOperationDateBefore { get; set; }
        public double? MinAverageValue { get; set; }
        public double? MaxAverageValue { get; set; }
        public double? MinAverageExecutionTime { get; set; }
        public double? MaxAverageExecutionTime { get; set; }
    }
}
