using Theatre.DAL.Abstractions;
using Theatre.DAL.Entities;
using Theatre.BLL.DTOs;

namespace Theatre.BLL.Services;

public class TheatreService
{
    private readonly IUnitOfWork _unitOfWork;

    public TheatreService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
        public async Task<IEnumerable<PerformanceDto>> GetAllPerformancesAsync()
    {
        var performanceRepo = _unitOfWork.GetRepository<Performance>();
        var authorRepo = _unitOfWork.GetRepository<Author>();
        var ticketRepo = _unitOfWork.GetRepository<Ticket>();

        var performances = await performanceRepo.GetAllAsync();
        var authors = await authorRepo.GetAllAsync();
        var tickets = await ticketRepo.GetAllAsync();

        var result = new List<PerformanceDto>();

        foreach (var p in performances)
        {
            var author = authors.FirstOrDefault(a => a.Id == p.AuthorId);
            var soldTicketsCount = tickets.Count(t => t.PerformanceId == p.Id && !t.IsReturned);

            result.Add(new PerformanceDto
            {
                Id = p.Id,
                Title = p.Title,
                Genre = p.Genre,
                AuthorName = author?.FullName ?? "Невідомий",
                Date = p.Date,
                TicketPrice = p.TicketPrice,
                AvailableSeats = p.TotalSeats - soldTicketsCount
            });
        }

        return result;
    }
        public async Task<TicketDto> BuyTicketAsync(int performanceId, int seatNumber)
    {
        var performanceRepo = _unitOfWork.GetRepository<Performance>();
        var ticketRepo = _unitOfWork.GetRepository<Ticket>();

        var performance = await performanceRepo.GetByIdAsync(performanceId);
        if (performance == null)
            throw new Exception("Виставу не знайдено.");
        var existingTickets = await ticketRepo.FindAsync(t => t.PerformanceId == performanceId && t.SeatNumber == seatNumber && !t.IsReturned);
        if (existingTickets.Any())
            throw new Exception("Це місце вже зайняте!");

        if (seatNumber < 1 || seatNumber > performance.TotalSeats)
            throw new Exception($"Некоректний номер місця. Доступно місць: від 1 до {performance.TotalSeats}");

        var ticket = new Ticket
        {
            PerformanceId = performanceId,
            PurchasePrice = performance.TicketPrice,
            PurchaseDate = DateTime.Now,
            SeatNumber = seatNumber,
            IsReturned = false
        };

        await ticketRepo.AddAsync(ticket);
        await _unitOfWork.SaveChangesAsync();

        return new TicketDto
        {
            Id = ticket.Id,
            PerformanceTitle = performance.Title,
            PerformanceDate = performance.Date,
            PurchasePrice = ticket.PurchasePrice,
            PurchaseDate = ticket.PurchaseDate,
            IsReturned = ticket.IsReturned,
            SeatNumber = ticket.SeatNumber
        };
    }
    public async Task<decimal> ReturnTicketAsync(int ticketId)
    {
        var ticketRepo = _unitOfWork.GetRepository<Ticket>();
        var performanceRepo = _unitOfWork.GetRepository<Performance>();

        var ticket = await ticketRepo.GetByIdAsync(ticketId);
        if (ticket == null || ticket.IsReturned)
            throw new Exception("Квиток не знайдено або він уже був повернений.");

        var performance = await performanceRepo.GetByIdAsync(ticket.PerformanceId);
        if (performance == null)
            throw new Exception("Пов'язану виставу не знайдено.");

        // Перевірка терміну: до вистави має бути не менше 2 днів
        var timeDifference = performance.Date - DateTime.Now;
        if (timeDifference.TotalDays < 2)
        {
            throw new Exception("Повернення неможливе: до початку вистави залишилося менше ніж 2 дні.");
        }
        decimal refundAmount = ticket.PurchasePrice * 0.8m;

        ticket.IsReturned = true;
        ticketRepo.Update(ticket);
        await _unitOfWork.SaveChangesAsync();

        return refundAmount;
    }
}