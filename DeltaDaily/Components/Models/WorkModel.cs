using DeltaDaily.Components.Data;

namespace DeltaDaily.Components.Models
{
    public class WorkModel
        {
        public string? ProjetoId { get; set; }
        public TimeOnly? Duracao { get; set; }
        public WorkData.WorkType? Tipo { get; set; }
        public WorkData.Focus? Foco { get; set; }
        public WorkData.WorkMethod? Metodo { get; set; }
    }

}
