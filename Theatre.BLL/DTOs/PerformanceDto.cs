namespace Theatre.BLL.DTOs;
public class PerformanceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string AuthorName {  get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal TicketPrice { get; set; }
    public int AvailableSeats { get; set; }
}
