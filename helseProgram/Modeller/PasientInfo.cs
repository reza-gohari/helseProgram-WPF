namespace helseProgram.Modeller;

public class PasientInfo //Til senere bruk for når og hvis vi rekker å sette opp for pasient info.
{
    public int PasientId { get; set; }

    public string Fornavn { get; set; } = string.Empty;

    public string Etternavn { get; set; } = string.Empty;

    public DateTime Fodselsdag { get; set; }

    public int PasientVekt { get; set; }
}