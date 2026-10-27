using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Theatre.DAL;
using Theatre.DAL.Abstractions;
using Theatre.DAL.Entities;
using Theatre.BLL.Services;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var services = new ServiceCollection();

        services.AddDbContext<TheatreDbContext>(options =>
            options.UseSqlite("Data Source=theatre.db"));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<TheatreService>();
        var serviceProvider = services.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TheatreDbContext>();
            await db.Database.EnsureCreatedAsync();
            await SeedDataAsync(db);
        }

        var theatreService = serviceProvider.GetRequiredService<TheatreService>();

        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("1. Переглянути репертуар вистав");
            Console.WriteLine("2. Купити квиток на виставу");
            Console.WriteLine("3. Повернути квиток (комісія 20%)");
            Console.WriteLine("4. Вихід");
            Console.Write("(1-4): ");
            var choice = Console.ReadLine();
            Console.WriteLine();
            try
            {
                switch (choice)
                {
                    case "1":
                        await ShowPerformancesAsync(theatreService);
                        break;
                    case "2":
                        await BuyTicketFlowAsync(theatreService);
                        break;
                    case "3":
                        await ReturnTicketFlowAsync(theatreService);
                        break;
                    case "4":
                        exit = true;
                        Console.WriteLine("Дякуємо!");
                        break;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Помилка]: {ex.Message}");
                Console.ResetColor();
            }
            if (!exit)
            {
                Console.ReadKey();
            }
        }
    }
    private static async Task SeedDataAsync(TheatreDbContext context)
    {
        if (!await context.Authors.AnyAsync())
        {
            var author = new Author { FullName = "Іван Карпенко-Карий" };
            context.Authors.Add(author);
            await context.SaveChangesAsync();

            context.Performances.Add(new Performance
            {
                Title = "Мартин Боруля",
                Genre = "Комедія",
                Date = DateTime.Now.AddDays(5),
                TotalSeats = 10,
                TicketPrice = 250.00m,
                AuthorId = author.Id
            });

            await context.SaveChangesAsync();
        }
    }
    private static async Task ShowPerformancesAsync(TheatreService service)
    {
        var performances = await service.GetAllPerformancesAsync();
        Console.WriteLine("--- ДОСТУПНІ ВИСТАВИ ---");
        foreach (var p in performances)
        {
            Console.WriteLine($"ID: {p.Id} | Назва: \"{p.Title}\" | Автор: {p.AuthorName}");
            Console.WriteLine($"Жанр: {p.Genre} | Дата: {p.Date:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Ціна: {p.TicketPrice} грн | Вільних місць: {p.AvailableSeats}");
        }
    }
    private static async Task BuyTicketFlowAsync(TheatreService service)
    {
        await ShowPerformancesAsync(service);
        
        Console.Write("Введіть ID вистави: ");
        if (!int.TryParse(Console.ReadLine(), out int perfId)) return;

        Console.Write("Введіть номер місця (наприклад, від 1 до 10): ");
        if (!int.TryParse(Console.ReadLine(), out int seatNum)) return;

        var ticket = await service.BuyTicketAsync(perfId, seatNum);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nКвиток успішно придбано!");
        Console.WriteLine($"Номер квитка: {ticket.Id} | Місце: {ticket.SeatNumber} | Сума: {ticket.PurchasePrice} грн");
        Console.ResetColor();
    }
    private static async Task ReturnTicketFlowAsync(TheatreService service)
    {
        Console.Write("Введіть ID квитка для повернення: ");
        if (!int.TryParse(Console.ReadLine(), out int ticketId)) return;
        decimal refund = await service.ReturnTicketAsync(ticketId);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\nКвиток успішно повернено.");
        Console.WriteLine($"До повернення (з урахуванням 20% комісії): {refund} грн");
        Console.ResetColor();
    }
}