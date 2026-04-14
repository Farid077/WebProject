namespace WebProject.Models;

public class BaseEntity
{
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow.AddHours(4);
    public DateTime? UpdatedTime { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
}
