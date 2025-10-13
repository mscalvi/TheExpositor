using DeltaDaily.Components.Data;

namespace DeltaDaily.Components.Models
{
    public class SnackModel
    {
        public SnackData.Snack? Tipo { get; set; }
        public string? Alimentos { get; set; }
        public TimeOnly? Horario { get; set; }
        public string? Calorias { get; set; }
    }
}
