using System.Data;
using System.Data.SqlClient;
using HotelReservas.Utilities;

namespace HotelReservas.Services
{
    public class ReservaService(DatabaseService dbService)
    {
        private readonly DatabaseService _dbService = dbService;

        public void RegistrarReserva(int clienteId, int habitacionId, DateTime fechaEntrada, DateTime fechaSalida)
        {
            if(!InputValidator.ValidarFechas(fechaEntrada, fechaSalida))
            {
                Console.WriteLine("Las fechas ingresadas no son válidas");
                return;
            }

            if (!ExisteCliente(clienteId))
            {
                Console.WriteLine("El cliente ingresado no existe");
                return;
            }

            if (!ExisteHabitacion(habitacionId))
            {
                Console.WriteLine("La habitación ingresada no existe");
                return;
            }

            if (!HabitacionDisponible(habitacionId, fechaEntrada, fechaSalida))
            {
                Console.WriteLine("La habitación ya está reservada en el rango de fechas especificado");
                return;
            }

            using var conn = _dbService.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("RegistrarReserva", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ClienteID", clienteId);
            cmd.Parameters.AddWithValue("@HabitacionID", habitacionId);
            cmd.Parameters.AddWithValue("@FechaEntrada", fechaEntrada);
            cmd.Parameters.AddWithValue("@FechaSalida", fechaSalida);

            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Reserva registrada correctamente");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al registrar la reserva: " + ex.Message);
            }
        }

        private bool ExisteCliente(int clienteId)
        {
            using var conn = _dbService.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM Clientes WHERE ClienteID = @ClienteID", conn);
            cmd.Parameters.AddWithValue("@ClienteID", clienteId);

            try
            {
                return (int) cmd.ExecuteScalar() > 0;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al consultar la existencia del cliente " + ex.Message);
                return false;
            }
        }

        private bool ExisteHabitacion(int habitacionId)
        {
            using var conn = _dbService.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM Habitaciones WHERE HabitacionID = @HabitacionID", conn);
            cmd.Parameters.AddWithValue("@HabitacionID", habitacionId);

            try
            {
                return (int) cmd.ExecuteScalar() > 0;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al consultar la existencia de la habitación " + ex.Message);
                return false;
            }
        }

        private bool HabitacionDisponible(int habitacionId, DateTime fechaEntrada, DateTime fechaSalida)
        {
            using var conn = _dbService.GetConnection();
            conn.Open();
            string query = @"SELECT COUNT(1) 
                             FROM Reservas 
                             WHERE HabitacionID = @HabitacionID
                             AND (@FechaEntrada < FechaSalida AND @FechaSalida > FechaEntrada)";
            using var cmd = new SqlCommand(query, conn);
            
            cmd.Parameters.AddWithValue("@HabitacionID", habitacionId);
            cmd.Parameters.AddWithValue("@FechaEntrada", fechaEntrada);
            cmd.Parameters.AddWithValue("@FechaSalida", fechaSalida);

            try
            {
                return (int) cmd.ExecuteScalar() == 0;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al consultar la disponibilidad de la habitación " + ex.Message);
                return false;
            }
        }

        public void ConsultarReservasActivasDeCliente(int clienteId)
        {
            using var conn = _dbService.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(@"SELECT R.ReservaID, H.TipoHabitacion, R.FechaEntrada, R.FechaSalida 
                                                  FROM Reservas R
                                                  JOIN Habitaciones H ON R.HabitacionID = H.HabitacionID
                                                  WHERE R.ClienteID = @ClienteID AND R.FechaSalida > GETDATE()", conn);

            cmd.Parameters.AddWithValue("@ClienteID", clienteId);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("\n--- Reservas activas del cliente ---");
                while (reader.Read())
                {
                    Console.WriteLine($"ReservaID: {reader["ReservaID"]}, Habitación: {reader["TipoHabitacion"]}, Entrada: {reader["FechaEntrada"]}, Salida: {reader["FechaSalida"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener las reservas activas del cliente: " + ex.Message);
            }
        }
    }
}
