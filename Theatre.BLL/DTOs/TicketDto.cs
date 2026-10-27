public class TicketDto
{
    public int Id { get; set; }
    public string PerformanceTitle { get; set; } = string.Empty;
    public DateTime PerformanceDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public bool IsReturned { get; set; }
    public int SeatNumber { get; set; }
}