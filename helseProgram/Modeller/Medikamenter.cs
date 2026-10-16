namespace helseProgram.Modeller;

public class Medikamenter
{
    public int MedikamentId { get; set; }

    public string MedikamentNavn { get; set; } = string.Empty;

    public decimal? Konsentrasjon { get; set; }

    public string KonsentrasjonEnhet { get; set; } = string.Empty;
}