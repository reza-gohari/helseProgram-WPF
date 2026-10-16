namespace helseProgram.Modeller;

public class MedikamentType
{
    
    public int MedikamentTypeId { get; set; } //PK 
    public int MedikamentId { get;set; } //FK til medikamenter

    public string Beskrivelse { get; set; } = string.Empty; // denne er forsåvidt valgfri og muligens obselete

    public Medikamenter Medikamenter { get; set; } = null!; // relasjon

    public ICollection<MedikamentVirkestoff> Virkestoffer { get; set; } //kan ha flere virkestoffer
        = new List<MedikamentVirkestoff>();
}