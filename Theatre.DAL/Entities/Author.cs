namespace Theatre.DAL.Entities;

public class Author
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public ICollection<Performance> Performances { get; set; } = new List<Performance>();
}