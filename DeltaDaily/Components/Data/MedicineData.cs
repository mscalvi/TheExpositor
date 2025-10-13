using System.ComponentModel.DataAnnotations;

namespace DeltaDaily.Components.Data
{
    public class MedicineData
    {
        public enum Medicine {
            Nenhum,
            Juneve
        }

        public enum Dose
        {
            [Display(Name = "10mg")] Dez,
            [Display(Name = "20mg")] Vinte,
            [Display(Name = "30mg")] Trinta,
            [Display(Name = "40mg")] Quartena,
            [Display(Name = "50mg")] Cinquenta,
            [Display(Name = "60mg")] Sessenta,
            [Display(Name = "70mg")] Setenta
        }
    }
}
