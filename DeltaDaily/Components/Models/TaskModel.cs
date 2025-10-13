namespace DeltaDaily.Components.Models
{
    public class TaskModel
    {
        public string Nome { get; set; }
        public DateOnly Criacao { get; set; }
        public DateOnly? Finalizada { get; set; }
        public bool? Completa { get; set; }
        public string? Comentario { get; set; }
    }
}
