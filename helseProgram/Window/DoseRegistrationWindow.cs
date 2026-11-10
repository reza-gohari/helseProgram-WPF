using helseProgram.Data;
using helseProgram.menu;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using helseProgram.Modeller;
using Microsoft.EntityFrameworkCore;
using System.Linq;

/////////////////Ado////////////////////
namespace helseProgram;

public partial class DoseRegistrationWindow : Window
{
    private const string velgLegemiddel = "Velg legemiddel...";

    private readonly ObservableCollection<MedikamentAdministrering> registrerteDoser = new();

    /////Reza, Gir MainWindow tilgang til dosene.
    public IReadOnlyList<MedikamentAdministrering> RegistrerteDoser => registrerteDoser;

    private string stedForTablett = string.Empty;
    private bool stedErLast;

    public DoseRegistrationWindow()
    {
        InitializeComponent();
        FyllLegemiddelliste();
        FyllVarighetslister();
        OppdaterVarighet();

        DoseTextBox.Clear();
    }

    private void FyllLegemiddelliste()
    {
        using var db = new AppDbContext();

        MedicationComboBox.ItemsSource = db.Medikamenter
            .AsNoTracking()
            .Include(m => m.MedikamentTyper)
            .ThenInclude(mt => mt.Virkestoffer)
            .OrderBy(m => m.MedikamentNavn)
            .ToList();

        MedicationComboBox.SelectedIndex = -1;
    }

    private void FyllVarighetslister()
    {
        for (var time = 0; time <= 24; time++)
        {
            VarighetTime.Items.Add(time);
        }

        for (var minutt = 0; minutt < 60; minutt += 1)
        {
            VarighetMinutt.Items.Add(minutt.ToString("00"));
        }

        VarighetTime.SelectedIndex = 0;
        VarighetMinutt.SelectedIndex = 0;
    }

    private void DoseFormComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (LocationTextBox is null)
        {
            return;
        }

        var erTablett = string.Equals(
            HentValgtTekst(DoseFormComboBox),
            "Tablett",
            StringComparison.OrdinalIgnoreCase);

        // Sted er ikke relevant når doseformen er tablett
        if (erTablett)
        {
            if (!stedErLast)
            {
                stedForTablett = LocationTextBox.Text;
            }

            stedErLast = true;
            LocationTextBox.Text = "--";
            LocationTextBox.IsReadOnly = true;
            LocationTextBox.IsHitTestVisible = false;
            LocationTextBox.Focusable = false;
            LocationTextBox.Foreground = Brushes.Gray;
            LocationTextBox.Background = Brushes.LightGray;
        }
        else if (stedErLast)
        {
            stedErLast = false;
            LocationTextBox.Text = stedForTablett;
            LocationTextBox.IsReadOnly = false;
            LocationTextBox.IsHitTestVisible = true;
            LocationTextBox.Focusable = true;
            LocationTextBox.Foreground = Brushes.Black;
            LocationTextBox.Background = Brushes.White;
        }

        OppdaterVarighet();
    }

    private void KontinuerligCheckBox_Endret(
        object sender,
        RoutedEventArgs e)
    {
        OppdaterVarighet();
    }

    private void OppdaterVarighet()
    {
        if (VarighetTime is null ||
            VarighetMinutt is null ||
            KontinuerligCheckBox is null)
        {
            return;
        }

        var erInfusjon = string.Equals(
            HentValgtTekst(DoseFormComboBox),
            "Infusjon",
            StringComparison.OrdinalIgnoreCase);

        KontinuerligCheckBox.IsEnabled = erInfusjon;

        if (!erInfusjon)
        {
            KontinuerligCheckBox.IsChecked = false;
        }

        var erKontinuerlig =
            KontinuerligCheckBox.IsChecked == true;

        VarighetTime.IsEnabled = erInfusjon && !erKontinuerlig;
        VarighetMinutt.IsEnabled = erInfusjon && !erKontinuerlig;
    }

    private void CreateDoseButton_Click(object sender, RoutedEventArgs e)
    {
        if (MedicationComboBox.SelectedItem is not Medikamenter valgtMedikament)
        {
            VisFeil("Velg et legemiddel.", MedicationComboBox);
            return;
        }

        var doseform = HentValgtTekst(DoseFormComboBox);
        var dosetekst = DoseTextBox.Text.Trim();
        var klokkeslettTekst = StartTimeTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(doseform) || doseform == "Velg...")
        {
            VisFeil("Velg en doseform.", DoseFormComboBox);
            return;
        }

        if (!ErGyldigDose(dosetekst, out var dose))
        {
            VisFeil(
                "Skriv en gyldig dose større enn 0, for eksempel 1,5.",
                DoseTextBox);
            return;
        }

        if (!DateTime.TryParseExact(
                klokkeslettTekst,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var klokkeslett))
        {
            VisFeil(
                "Skriv starttid som tt:mm, for eksempel 14:30.",
                StartTimeTextBox);
            return;
        }

        if (StartDatePicker.SelectedDate is not DateTime dato)
        {
            VisFeil("Velg en startdato.", StartDatePicker);
            return;
        }

        var enhet = HentValgtTekst(DoseUnitComboBox);
        var administreringsmate =
            HentValgtTekst(AdministrationRouteComboBox);

        var startTid = dato.Date.Add(klokkeslett.TimeOfDay);

        var erKontinuerlig =
            doseform == "Infusjon" &&
            KontinuerligCheckBox.IsChecked == true;

        DateTime? endeTid = null;

        if (doseform == "Infusjon" && !erKontinuerlig)
        {
            var timer = (int)VarighetTime.SelectedItem;
            var minutter = int.Parse(HentValgtTekst(VarighetMinutt));
            var varighet = TimeSpan.FromMinutes(timer * 60 + minutter);

            if (varighet > DateTime.MaxValue - startTid)
            {
                VisFeil("Velg en tidligere startdato.", StartDatePicker);
                return;
            }

            endeTid = startTid.Add(varighet);
        }

        var administrering = new MedikamentAdministrering
        {
            MedikamentId = valgtMedikament.MedikamentId,
            Medikament = valgtMedikament,
            Dose = dose,
            Enhet = enhet,
            AdministreringsType = doseform,
            AdministreringsMetode = administreringsmate,
            StartTid = startTid,
            EndeTid = endeTid,
            ErKontinuerlig = erKontinuerlig,
            Lokalisasjon = stedErLast
                ? string.Empty
                : LocationTextBox.Text.Trim(),
            Kommentar = CommentTextBox.Text.Trim()
        };

        registrerteDoser.Add(administrering);

        RegisteredDosesGrid.ItemsSource = registrerteDoser.Select(d =>
        {
            var varighetTekst = "--";

            if (d.ErKontinuerlig)
            {
                varighetTekst = "Kontinuerlig";
            }
            else if (d.EndeTid is DateTime slutt)
            {
                var varighet = slutt - d.StartTid;

                varighetTekst =
                    $"{(int)varighet.TotalHours} t {varighet.Minutes:00} min";
            }

            return new
            {
                Medication = d.Medikament.MedikamentNavn,

                Dose =
                    $"{d.Dose.ToString(CultureInfo.GetCultureInfo("nb-NO"))} {d.Enhet}",

                Concentration = "--",

                Administration = d.AdministreringsMetode == "Velg..."
                    ? "--"
                    : d.AdministreringsMetode,

                Duration = varighetTekst,
                StartTime = d.StartTid.ToString("HH:mm"),

                Comment = string.IsNullOrWhiteSpace(d.Kommentar)
                    ? "--"
                    : d.Kommentar
            };
        }).ToList();

        RegisteredDosesGrid.ScrollIntoView(
            RegisteredDosesGrid.Items[RegisteredDosesGrid.Items.Count - 1]);

        NullstillDose();
        KontinuerligCheckBox.IsChecked = false;
        VarighetTime.SelectedIndex = 0;
        VarighetMinutt.SelectedIndex = 0;
        CommentTextBox.Clear();
        DoseTextBox.Focus();
    }

    private void NullstillDose()
    {
        DoseTextBox.Clear();
    }

    private static bool ErGyldigDose(string tekst, out decimal verdi)
    {
        // Godta både komma og punktum som desimaltegn.
        tekst = tekst.Replace('.', ',');
        var norsk = CultureInfo.GetCultureInfo("nb-NO");

        return decimal.TryParse(
            tekst,
            NumberStyles.AllowDecimalPoint,
            norsk,
            out verdi) && verdi > 0;
    }

    private void VisFeil(string melding, FrameworkElement felt)
    {
        MessageBox.Show(
            this,
            melding,
            "Feil",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        felt.Focus();
    }

    private static string HentValgtTekst(ComboBox liste)
    {
        if (liste.SelectedItem is ComboBoxItem valg)
        {
            return valg.Content?.ToString() ?? "";
        }

        return liste.SelectedItem?.ToString() ?? "";
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }


    private void DoseTextBox_PreviewTextInput(
        object sender,
        TextCompositionEventArgs e)
    {
    }

    private void DoseTextBox_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
    }

    private void DoseTextBox_PreviewMouseDown(
        object sender,
        MouseButtonEventArgs e)
    {
    }

    private void DoseTextBox_GotKeyboardFocus(
        object sender,
        KeyboardFocusChangedEventArgs e)
    {
    }

    private void DoseTextBox_SelectionChanged(
        object sender,
        RoutedEventArgs e)
    {
    }
}