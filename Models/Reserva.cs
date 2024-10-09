
namespace HotelReservas.Models
{
    public class Reserva
    {
        public int ReservaID { get; set; }
        public int ClienteID { get; set; }
        public int HabitacionID { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public decimal Total {  get; set; }
    }
}
