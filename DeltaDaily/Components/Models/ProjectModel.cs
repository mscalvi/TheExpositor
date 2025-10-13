using DeltaDaily.Components.Data;

namespace DeltaDaily.Components.Models
{
    public class ProjectModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        // Geral
        public string Nome { get; set; } = "";
        public string Resumo { get; set; } = "";
        public ProjectData.Phase Fase { get; set; } = ProjectData.Phase.Feira;
        public DateTime? DataAlteracao { get; set; } = DateTime.Now;
        public DateTime? DataCriacao { get; set; } = DateTime.Now;
        public string Descricao { get; set; } = "";
        public bool Ativo { get; set; } = false;

        // Meta
        public string MetaAtual { get; set; } = "";
        public int TarefasAtuais { get; set; }
        public List<MetaModel> Metas { get; set; } = new();

        // Ideias
        public List<TaskModel> Ideias { get; set; } = new();
    }
}
