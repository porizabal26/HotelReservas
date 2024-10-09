using HotelReservas.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReservas
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = BuildConfiguration();
            var serviceProvider = BuildServiceProvider(configuration);
            
            var reservaService = serviceProvider.GetService<ReservaService>();
            var habitacionService = serviceProvider.GetService<HabitacionService>();

            while (true)
            {
                Console.WriteLine("\n--- Menú de gestión de reservas de hotel ---");
                Console.WriteLine("1. Listar habitaciones disponibles");
                Console.WriteLine("2. Registrar nueva reserva");
                Console.WriteLine("3. Consultar reservas de un cliente");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccionar una opción: ");
                string opcion = Console.ReadLine();

                switch(opcion)
                {
                    case "1":
                        habitacionService.ListarHabitaciones();
                        break;
                    case "2":
                        RegistrarReserva(reservaService);
                        break;
                    case "3":
                        ConsultarReservasDeCliente(reservaService);
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine($"Opción: {opcion} no válida.");
                        break;
                }
            }
        }

        static void RegistrarReserva(ReservaService reservaService)
        {
            Console.Write("\nIngrese el ID del cliente: ");
            int clienteId = int.Parse(Console.ReadLine());

            Console.Write("Ingrese el ID de la habitación: ");
            int habitacionId = int.Parse(Console.ReadLine());

            Console.Write("Ingrese la fecha de entrada (yyyy-MM-dd): ");
            DateTime fechaEntrada = DateTime.Parse(Console.ReadLine());

            Console.Write("Ingrese la fecha de salida (yyyy-MM-dd): ");
            DateTime fechaSalida = DateTime.Parse(Console.ReadLine());

            reservaService.RegistrarReserva(clienteId, habitacionId, fechaEntrada, fechaSalida);
        }

        static void ConsultarReservasDeCliente(ReservaService reservaService)
        {
            Console.Write("\nIngrese el ID del cliente: ");
            int clienteId = int.Parse(Console.ReadLine());
            reservaService.ConsultarReservasActivasDeCliente(clienteId);
        }

        static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        static ServiceProvider BuildServiceProvider(IConfiguration configuration)
        {
            return new ServiceCollection()
                .AddSingleton(configuration)
                .AddSingleton<DatabaseService>()
                .AddSingleton<HabitacionService>()
                .AddSingleton<ReservaService>()
                .BuildServiceProvider();
        }
    }
}
