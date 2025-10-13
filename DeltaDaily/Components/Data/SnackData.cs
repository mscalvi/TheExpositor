using System.ComponentModel.DataAnnotations;

namespace DeltaDaily.Components.Data
{
    public class SnackData
    {
        public enum Snack
        {
            [Display(Name = "Café da Manhã")] Café,
            [Display(Name = "Almoço")] Almoço,
            [Display(Name = "Lanche")] Lanche,
            [Display(Name = "Janta")] Janta
        }
    }
}
