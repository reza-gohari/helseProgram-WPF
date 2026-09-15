namespace helseProgram.Modeller;

public class SimSesjon
{
    public int SimSesjonsId { get; set; }

    public string SesjonsNavn { get; set; } = string.Empty;

    public DateTime StartTid { get; set; }

    public DateTime? EndingsTid { get; set; } // ? endetid kan være null eller pågående. vil endres når sesjonen faktist tar slutt.

}