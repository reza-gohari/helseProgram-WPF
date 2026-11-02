// Azo
using System;
using System.Globalization;
using System.Windows;
using System.Linq;
using helseProgram.Data;
using helseProgram.Modeller;

namespace helseProgram
{
    public partial class PasientInfoWindow : Window
    {
        public PasientInfoWindow()
        {
            InitializeComponent();
            HentPasienter();
        }

        private void HentPasienter()
        {
            try
            {
                using var db = new AppDbContext();

                var pasienter = db.PasientInfo
                    .OrderBy(p => p.Etternavn)
                    .ThenBy(p => p.Fornavn)
                    .Select(p => new
                    {
                        p.PasientId,
                        FulltNavn = p.Fornavn + " " + p.Etternavn
                    })
                    .ToList();

                PasientComboBox.ItemsSource = pasienter;
            }
            catch (Exception ex)
                {
                MessageBox.Show(
                    $"Kunne ikke hente pasienter:\n{ex.Message}",
                    "Databasefeil",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void PasientComboBox_SelectionChanged(
            object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Ingen pasient valgt
            if (PasientComboBox.SelectedValue is not int pasientId)
            {
                SlettPasientKnapp.IsEnabled = false;
                return;
            }

            SlettPasientKnapp.IsEnabled = true;

            try
            {
                using var db = new AppDbContext();

                // Finn riktig pasient
                var pasient = db.PasientInfo
                    .FirstOrDefault(p => p.PasientId == pasientId);

                if (pasient == null)
                {
                    MessageBox.Show("Fant ikke pasienten i databasen.");
                    return;
                }

                // Utfyller tidligere kunde data til text feltene
                FornavnTextBox.Text = pasient.Fornavn;
                EtternavnTextBox.Text = pasient.Etternavn;
                FodselsdatoPicker.SelectedDate = pasient.Fodselsdag;
                VektTextBox.Text = pasient.PasientVekt.ToString();
                HoydeTextBox.Text = pasient.PasientHoyde?
                    .ToString(CultureInfo.GetCultureInfo("nb-NO")) ?? "";
                JournalTextBox.Text = pasient.Journal;
            }

            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Kunne ikke hente pasientinformasjon:\n{ex.Message}",
                    "Databasefeil",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SlettPasientKnapp_Click(object sender, RoutedEventArgs e)
        {
            // Kontroller at en eksisterende pasient er valgt
            if (PasientComboBox.SelectedValue is not int pasientId)
            {
                MessageBox.Show(
                    "Du må velge en eksisterende pasient først.");
                return;
            }

            try
            {
                using var db = new AppDbContext();

                var pasient = db.PasientInfo
                    .FirstOrDefault(p => p.PasientId == pasientId);

                if (pasient == null)
                {
                    MessageBox.Show("Pasienten finnes ikke i databasen.");
                    HentPasienter();
                    return;
                }

                // Kontroller om pasienten har tilknyttede medikamentregistreringer
                bool harRegistreringer = db.MedikamentAdministrering
                    .Any(m => m.PasientId == pasientId);

                if (harRegistreringer)
                {
                    MessageBox.Show(
                        "Pasienten har tilknyttede medikamentregistreringer " +
                        "og kan derfor ikke slettes.",
                        "Sletting blokkert",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Be brukeren bekrefte slettingen
                var svar = MessageBox.Show(
                    $"Er du sikker på at du vil slette " +
                    $"{pasient.Fornavn} {pasient.Etternavn}?\n\n" +
                    "Denne handlingen kan ikke angres.",
                    "Bekreft sletting",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No);

                if (svar != MessageBoxResult.Yes)
                {
                    return;
                }

                // Slett pasienten fra SQLite
                db.PasientInfo.Remove(pasient);
                db.SaveChanges();

                MessageBox.Show(
                    "Pasienten ble slettet.",
                    "Sletting fullført",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Tøm feltene etter sletting
                FornavnTextBox.Clear();
                EtternavnTextBox.Clear();
                FodselsdatoPicker.SelectedDate = null;
                VektTextBox.Clear();
                HoydeTextBox.Clear();
                JournalTextBox.Clear();

                // Oppdater listen over eksisterende pasienter
                HentPasienter();
                PasientComboBox.SelectedIndex = -1;
                SlettPasientKnapp.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Kunne ikke slette pasienten:\n{ex.Message}",
                    "Databasefeil",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LagrePasientKnapp_Click(object sender, RoutedEventArgs e)
        {
            string fornavn = FornavnTextBox.Text.Trim();
            string etternavn = EtternavnTextBox.Text.Trim();
            string journal = JournalTextBox.Text.Trim();

            // Kontroller navn
            if (string.IsNullOrWhiteSpace(fornavn) ||
                string.IsNullOrWhiteSpace(etternavn))
            {
                MessageBox.Show("Fyll ut fornavn og etternavn");
                return;
            }

            // Kontroller fødselsdato
            if (FodselsdatoPicker.SelectedDate is not DateTime fodselsdato ||
                fodselsdato.Date > DateTime.Today)
            {
                MessageBox.Show("Velg en gyldig fødselsdato");
                return;
            }

            // Kontroller vekt
            if (!int.TryParse(VektTextBox.Text, out int vekt) || vekt <= 0)
            {
                MessageBox.Show("Velg gyldig vekt");
                return;
            }

            // Kontroller høyde
            decimal? hoyde = null;

            if (!string.IsNullOrWhiteSpace(HoydeTextBox.Text))
            {
                if (!decimal.TryParse(
                        HoydeTextBox.Text,
                        NumberStyles.Number,
                        CultureInfo.GetCultureInfo("nb-NO"),
                        out decimal verdi) || verdi <= 0)
                {
                    MessageBox.Show("Velg gyldig høyde");
                    return;
                }

                hoyde = verdi;
            }

            // Lagre pasienten
            try
            {
                using var db = new AppDbContext();

                int? pasientId = PasientComboBox.SelectedValue as int?;
                bool erOppdatering = pasientId.HasValue;

                PasientInfo pasient;

                if (pasientId is int valgtPasientId)
                {
                    // Hent eksisterende pasient
                    var eksisterendePasient = db.PasientInfo
                        .FirstOrDefault(p => p.PasientId == valgtPasientId);

                    if (eksisterendePasient == null)
                    {
                        MessageBox.Show(
                            "Fant ikke pasienten i databasen.",
                            "Lagringsfeil",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                pasient = eksisterendePasient;
                }
                    else
                {
                    // Opprett ny pasient
                    pasient = new PasientInfo();
                    db.PasientInfo.Add(pasient);
                }

                // Oppdater pasientens informasjon
                pasient.Fornavn = fornavn;
                pasient.Etternavn = etternavn;
                pasient.Fodselsdag = fodselsdato;
                pasient.PasientVekt = vekt;
                pasient.PasientHoyde = hoyde;
                pasient.Journal = journal;

                // Lagre endringene i SQLite
                db.SaveChanges();

                MessageBox.Show(
                    erOppdatering
                        ? "Pasientinformasjonen ble oppdatert."
                        : "Ny pasient ble opprettet.",
                    "Lagring fullført",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Kunne ikke lagre pasienten:\n{ex.Message}",
                    "Databasefeil",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AvbrytKnapp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}