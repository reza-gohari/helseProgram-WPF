using helseProgram.Modeller;

public class MedikamentVirkestoff
{
    public int MedikamentVirkestoffId { get; set; } // PK

    public int MedikamentTypeId { get; set; } // FK

    public string VirkestoffNavn { get; set; } = string.Empty;

    public decimal? Konsentrasjon { get; set; }

    public string KonsentrasjonEnhet { get; set; } = string.Empty; // mg/ml, mcg/ml

    public MedikamentType MedikamentType { get; set; } = null!; // relasjon

}