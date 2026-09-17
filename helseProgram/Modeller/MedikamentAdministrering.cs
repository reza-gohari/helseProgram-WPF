namespace helseProgram.Modeller;

public class MedikamentAdministrering
{
    public int MedikamentAdministreringId { get; set; } // selvforklarende

    public int MedikamentId { get; set; } // hvilke mediment som blir brukt

    public int PasientId { get; set; } // pasient ID

    public int SimSesjonsId { get; set; } // Hvilke simulering det potensielt tilhører

    public string AdministreringsType { get; set; } = string.Empty; // bolus eller intravenøst

    public string AdministreringsMetode { get; set; } = string.Empty; // IV,IM osv, hvordan pasienten får dosen

    public decimal Dose { get; set; } //mengde 

    public string Enhet { get; set; } = string.Empty; // hvilke form kommer enheten mg, mcl, ml...

    public DateTime StartTid { get; set; }

    public DateTime? EndeTid { get; set; }

    public string Kommentar { get; set; } = string.Empty;

    public Medikamenter Medikament { get; set; } = null!;

    public PasientInfo Pasient { get; set; } = null!;

    public SimSesjon SimSesjon { get; set; } = null!;
}