using System.ComponentModel.DataAnnotations;

namespace DeltaDaily.Components.Data
{
    public class SleepData
    {
        public enum SleepQuality
        {
            [Display(Name = "Normal")] Normal,
            [Display(Name = "Ótimo")] Otimo,
            [Display(Name = "Muitos Sonhos")] MuitosSonhos,
            [Display(Name = "Muito Calor")] MuitoCalor,
            [Display(Name = "Muito Frio")] MuitoFrio,
            [Display(Name = "Insônia")] Insonia
        }
        public enum DreamType
        {
            [Display(Name = "Normais")] Normais,
            [Display(Name = "Agitados")] Agitados,
            [Display(Name = "Pesadelos com Trabalho")] PesadelosTrabalho,
            [Display(Name = "Pesadelos com Vi")] PesadelosVi,
            [Display(Name = "Pesadelos com Família")] PesadelosFamilia,
            [Display(Name = "Pesadelos Diferentes")] PesadelosDiferentes,
            [Display(Name = "Sexuais")] Sexuais
        }
    }
}
