namespace DutyAssignment.Models;

public class DutyAssignmentDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string DutyCollectionName { get; set; } = null!;
}