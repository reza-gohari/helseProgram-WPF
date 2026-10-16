namespace helseProgram.Modeller;

public class Medikamenter
{
    public int MedikamentId { get; set; }

    public string MedikamentNavn { get; set; } = string.Empty;

    public ICollection<MedikamentType> MedikamentTyper { get; set; }
        = new List<MedikamentType>();
}