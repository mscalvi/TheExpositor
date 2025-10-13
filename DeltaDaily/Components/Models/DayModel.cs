using DeltaDaily.Components.Data;
using System.ComponentModel.DataAnnotations;

namespace DeltaDaily.Components.Models;

public class DayModel
{
    public DateTime Data { get; set; } = DateTime.Now;
    public bool DiaFechado { get; set; }

    // Bloco 1
    public TimeOnly PrimeiroDespertar { get; set; }
    public TimeOnly SairDaCama { get; set; }

    public MedicineData.Medicine? Remedio { get; set; }
    public MedicineData.Dose? DoseRemedio { get; set; }
    public TimeOnly HorarioRemedio { get; set; }

    public TimeOnly? LogoutHora { get; set; }
    public TimeOnly DeitarHora { get; set; }

    // Bloco 2
    public WorkModel? Trabalho { get; set; }
    public List<WorkModel>? Trabalhos { get; set; } = new();
    public int TotalMinutosDedicados { get; set; }
    public int TotalMinutosParalelos { get; set; }
    public int TotalMinutosExtras { get; set; }

    // Bloco 3
    public SleepData.SleepQuality? QualidadeSono { get; set; }
    public SleepData.DreamType? TipoSonhos { get; set; }
    public string? NotasSonhos { get; set; }

    public HumorData.Humor? HumorMedio { get; set; }
    public string? VariacaoHumorNotas { get; set; }
    public ExerciseModel? Exercicio { get; set; }
    public List<ExerciseModel>? Exercicios { get; set; } = new();
    public SnackModel? Refeicao { get; set; }
    public List<SnackModel>? Refeicoes { get; set; } = new();
}
