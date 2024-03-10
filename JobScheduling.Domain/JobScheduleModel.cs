namespace JobScheduling.Domain;

public class JobScheduleModel
{
    public DateTimeOffset? Start { get; set; }
    public DateTimeOffset? End { get; set; }
    public string? Description { get; set; }
}
