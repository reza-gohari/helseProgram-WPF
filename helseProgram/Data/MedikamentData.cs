
using System.Collections.Generic;
using System.Linq;
using helseProgram.Modeller;
using Microsoft.EntityFrameworkCore;

namespace helseProgram.Data;

public static class MedikamentData
{
    public static void Seed(AppDbContext db)
    {
        var medikamenter = new List<Medikamenter>
        {
            LagMedikament("Atropin", ("Atropin", 1m, "mg/ml")),
            LagMedikament("Efedrin", ("Efedrin", 50m, "mg/ml")),
            LagMedikament("Fenylefrin", ("Fenylefrin", 0.1m, "mg/ml")),
            LagMedikament("Noradrenalin", ("Noradrenalin", 1m, "mg/ml")),

            LagMedikament("Adrenalin",
                ("Adrenalin", 10m, "mcg/ml"),
                ("Adrenalin", 1m, "mg/ml")),

            LagMedikament("Propofol", ("Propofol", 10m, "mg/ml")),
            LagMedikament("Tiopental", ("Tiopental", 25m, "mg/ml")),

            LagMedikament("Ketamin",
                ("Ketamin", 50m, "mg/ml"),
                ("Ketamin", 10m, "mg/ml")),

            LagMedikament("Remifentanil", ("Remifentanil", 50m, "mcg/ml")),
            LagMedikament("Alfentanil", ("Alfentanil", 0.5m, "mg/ml")),
            LagMedikament("Fentanyl", ("Fentanyl", 50m, "mcg/ml")),
            LagMedikament("Midazolam", ("Midazolam", 1m, "mg/ml")),

            LagMedikament("Suxamethonium", ("Suxamethonium", 50m, "mg/ml")),
            LagMedikament("Rokuronium", ("Rokuronium", 10m, "mg/ml")),

            LagMedikament("NaCl", ("Natriumklorid", 9m, "mg/ml")),

            LagMedikament("Plasmalyte"),
            LagMedikament("Ringer Acetat"),

            LagMedikament("Naloxon", ("Naloxon", 0.4m, "mg/ml")),
            LagMedikament("Flumazenil", ("Flumazenil", 0.1m, "mg/ml")),

            LagMedikament("Robinol-Neostigmin",
                ("Glykopyrronium", 0.5m, "mg/ml"),
                ("Neostigmin", 2.5m, "mg/ml")),

            LagMedikament("Sugammadex", ("Sugammadex", 100m, "mg/ml")),
            LagMedikament("Metokloperamid", ("Metokloperamid", 5m, "mg/ml")),
            LagMedikament("Ondansetron", ("Ondansetron", 4m, "mg/ml")),
            LagMedikament("Dexametason", ("Dexametason", 4m, "mg/ml")),
            LagMedikament("Klonidin", ("Klonidin", 150m, "mcg/ml")),
            LagMedikament("Ketorolax", ("Ketorolax", 30m, "mg/ml")),
            LagMedikament("Tranexamsyre", ("Tranexamsyre", 100m, "mg/ml")),
            LagMedikament("Paracetamol", ("Paracetamol", 10m, "mg/ml")),
            LagMedikament("Ibuprofen", ("Ibuprofen", 4m, "mg/ml"))
        };

        var eksisterende = db.Medikamenter
            .Include(m => m.MedikamentTyper)
            .ThenInclude(mt => mt.Virkestoffer)
            .ToList()
            .ToDictionary(m => m.MedikamentNavn);

        foreach (var medikament in medikamenter)
        {
            if (!eksisterende.TryGetValue(
                    medikament.MedikamentNavn,
                    out var lagretMedikament))
            {
                db.Medikamenter.Add(medikament);
                continue;
            }

            foreach (var variant in medikament.MedikamentTyper)
            {
                bool finnes = lagretMedikament.MedikamentTyper.Any(mt =>
                    mt.Virkestoffer.Count == variant.Virkestoffer.Count &&
                    mt.Virkestoffer.All(v =>
                        variant.Virkestoffer.Any(ny =>
                            ny.VirkestoffNavn == v.VirkestoffNavn &&
                            ny.Konsentrasjon == v.Konsentrasjon &&
                            ny.KonsentrasjonEnhet == v.KonsentrasjonEnhet)));

                if (!finnes)
                {
                    lagretMedikament.MedikamentTyper.Add(variant);
                }
            }
        }

        db.SaveChanges();
    }

    private static Medikamenter LagMedikament(
        string navn,
        params (string virkestoff, decimal konsentrasjon, string enhet)[] verdier)
    {
        var medikament = new Medikamenter
        {
            MedikamentNavn = navn
        };

        // Hver oppføring representerer én medikamentvariant.
        // Kombinasjonspreparater behandles separat nedenfor.
        if (navn == "Robinol-Neostigmin")
        {
            var variant = new MedikamentType();

            foreach (var (virkestoff, konsentrasjon, enhet) in verdier)
            {
                variant.Virkestoffer.Add(new MedikamentVirkestoff
                {
                    VirkestoffNavn = virkestoff,
                    Konsentrasjon = konsentrasjon,
                    KonsentrasjonEnhet = enhet
                });
            }

            medikament.MedikamentTyper.Add(variant);
        }
        else
        {
            foreach (var (virkestoff, konsentrasjon, enhet) in verdier)
            {
                medikament.MedikamentTyper.Add(new MedikamentType
                {
                    Virkestoffer = new List<MedikamentVirkestoff>
                    {
                        new MedikamentVirkestoff
                        {
                            VirkestoffNavn = virkestoff,
                            Konsentrasjon = konsentrasjon,
                            KonsentrasjonEnhet = enhet
                        }
                    }
                });
            }
        }

        return medikament;
    }
}
