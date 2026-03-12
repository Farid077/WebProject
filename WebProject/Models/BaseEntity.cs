namespace WebProject.Models;

public class BaseEntity
{
    public DateOnly CreatedTime { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly? UpdatedTime { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
}
