namespace WebProject.Models;

public enum PageAccess
{
    Read = 1,
    Read_Write = 1 | 2,
}
