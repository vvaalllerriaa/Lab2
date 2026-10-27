namespace Theatre.DAL.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public int PerformanceId { get; set; }
        public Performance Performance { get; set; } = null!;
        
        public int SeatNumber { get; set; } 
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsReturned { get; set; } = false;
    }
}