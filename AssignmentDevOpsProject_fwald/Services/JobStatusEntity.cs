using Azure.Data.Tables;
using Azure;

public class JobStatusEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string JobId { get; set; }
    public string Status { get; set; }

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public JobStatusEntity() { }

    public JobStatusEntity(string jobId)
    {
        PartitionKey = "JobStatus";
        RowKey = jobId;
        JobId = jobId;
        Status = "Pending";
    }
}
