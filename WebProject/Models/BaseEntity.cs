namespace WebProject.Models;

public class BaseEntity
{
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime? UpdatedTime { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
}
