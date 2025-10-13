using System.ComponentModel.DataAnnotations;

namespace DeltaDaily.Components.Data
{
    public class WorkData
    {
        public enum WorkMethod
        {
            Pomodoro,
            Livre
        }
        public enum WorkType
        {
            [Display(Name = "Home-Office")] Dedicada,
            [Display(Name = "Em Paralelo")] Paralela,
            [Display(Name = "Hora Extra")] Extra
        }
        public enum Focus
        {
            Péssimo,
            Baixo,
            Médio,
            Bom,
            Maravilhoso
        }
    }
}
