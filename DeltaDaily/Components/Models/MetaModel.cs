namespace DeltaDaily.Components.Models
{
    public class MetaModel
    {
        public string Nome { get; set; }
        public string Objetivo { get; set; }
        public DateTime DataCriacao { get; set; }
        public int TarefasCriacao { get; set; }
        public int TarefasTardias { get; set; }
        public int TarefasCompletas { get; set; }
        public int TarefasTotais { get; set; }
        public DateOnly? DataFechamento { get; set; }
        public bool Ativa { get; set; }
        public bool Completa { get; set; }
        public List<TaskModel> Tasks { get; set; }
    }
}
