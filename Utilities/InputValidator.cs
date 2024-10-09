
namespace HotelReservas.Utilities
{
    public static class InputValidator
    {
        public static bool ValidarFechas(DateTime fechaEntrada, DateTime fechaSalida)
        {
            return fechaEntrada < fechaSalida && fechaEntrada >= DateTime.Today;
        }
    }
}
