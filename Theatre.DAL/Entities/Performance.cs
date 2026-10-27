namespace Theatre.DAL.Entities;

public class Performance
{
    public int Id  { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int TotalSeats { get; set; }
    public decimal TicketPrice { get; set; }

    // Зовнішній ключ до автора
    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}