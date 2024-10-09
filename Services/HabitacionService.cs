using System.Data.SqlClient;

namespace HotelReservas.Services
{
    public class HabitacionService(DatabaseService dbService)
    {
        private readonly DatabaseService _dbService = dbService;

        public void ListarHabitaciones()
        {
            using var conn = _dbService.GetConnection();
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT HabitacionID, TipoHabitacion, PrecioPorNoche FROM Habitaciones", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\n--- Lista de Habitaciones ---");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["HabitacionID"]}, Tipo: {reader["TipoHabitacion"]}, Precio: {reader["PrecioPorNoche"]}");
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Error al listar habitaciones " + ex.Message);
            }
        }
    }
}
