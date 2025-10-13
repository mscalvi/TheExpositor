using DeltaDaily.Components.Data;

namespace DeltaDaily.Components.Models
{
    public class ExerciseModel
    {
            public DateTime? Hora { get; set; }
            public TimeOnly? Duracao { get; set; }
            public ExerciseData.Workout? Modalidade { get; set; }
            public ExerciseData.Tired? Cansaco { get; set; }
            public ExerciseData.Happy? Animo { get; set; }
       
    }
}
