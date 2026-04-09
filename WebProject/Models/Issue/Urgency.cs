namespace WebProject.Models;

public class Urgency
{
    public int Id { get; set; }
    public string Name { get; set; }

    /// <summary>
    /// time the issue should be solved (by minutes)
    /// </summary>
    public int Time { get; set; } = 0;
    public ICollection<Issue> Issues { get; set; } = [];
}
