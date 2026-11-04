using helseProgram.Data;
using helseProgram.menu;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using helseProgram.Data;
using helseProgram.Modeller;
using Microsoft.EntityFrameworkCore;
using System.Linq;

/////////////////Ado////////////////////
namespace helseProgram;

public partial class DoseRegistrationWindow : Window
{
    private const string velgLegemiddel = "Velg legemiddel...";

    private readonly ObservableCollection<RegisteredDose> registrerteDoser = new();

    /////Reza, Gir MainWindow tilgang til dosene.
    public IReadOnlyList<RegisteredDose> RegistrerteDoser => registrerteDoser;

    private string stedForTablett = string.Empty;
    private bool stedErLast;

    public DoseRegistrationWindow()
    {
        InitializeComponent();
        FyllLegemiddelliste();
        FyllVarighetslister();
        OppdaterVarighet();

        RegisteredDosesGrid.ItemsSource = registrerteDoser;
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

        var legemiddel = valgtMedikament.MedikamentNavn;
        var medikamentId = valgtMedikament.MedikamentId;

        var doseform = HentValgtTekst(DoseFormComboBox);
        var dosetekst = DoseTextBox.Text.Trim();
        var klokkeslettTekst = StartTimeTextBox.Text.Trim();

        

        if (string.IsNullOrWhiteSpace(doseform) || doseform == "Velg...")
        {
            VisFeil("Velg en doseform.", DoseFormComboBox);
            return;
        }

        if (!ErGyldigDose(dosetekst, out _))
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
        var administreringsmate = HentValgtTekst(AdministrationRouteComboBox);
        var timer = HentValgtTekst(VarighetTime);
        var minutter = HentValgtTekst(VarighetMinutt);
        var varighet = doseform == "Infusjon"
            ? KontinuerligCheckBox.IsChecked == true
                ? "Kontinuerlig"
                : $"{timer} t {minutter} min"
            : "--";
        var kommentar = CommentTextBox.Text.Trim();

        /////Reza, Simulasjonstiden settes i MainWindow.
        registrerteDoser.Add(new RegisteredDose(
            Medication: legemiddel,
            DoseForm: doseform,
            Simulasjonstid: "",
                    
                    Amount: "--",
            Dose: $"{dosetekst} {enhet}",
            Concentration: "--",
            Administration: administreringsmate == "Velg..."
                ? "--"
                : administreringsmate,
            Duration: varighet,
            StartTime: klokkeslett.ToString(
                "HH:mm",
                CultureInfo.InvariantCulture),
            StartDate: dato.Date,
            Location: stedErLast ||
                string.IsNullOrWhiteSpace(LocationTextBox.Text)
                    ? "--"
                    : LocationTextBox.Text.Trim(),
            Comment: string.IsNullOrWhiteSpace(kommentar)
                ? "--"
                : kommentar));

        RegisteredDosesGrid.ScrollIntoView(
            registrerteDoser[registrerteDoser.Count - 1]);

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

public sealed record RegisteredDose(

    string Medication,
    string DoseForm,/////Reza, Infusjon, bolus eller tablett.
    string Amount,
    string Dose,
    string Concentration,
    string Administration,
    string Duration,
    string StartTime,
    DateTime StartDate,
    string Location,
    /////Reza, Simulasjonstid for visning.
    string Simulasjonstid,
    
    string Comment);

/////////////////////Ado////////////////////////