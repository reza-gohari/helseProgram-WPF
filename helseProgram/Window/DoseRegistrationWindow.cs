using helseProgram.menu;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace helseProgram;

public partial class DoseRegistrationWindow : Window
{
    private const string ChooseMedicationText = "Velg medikament...";

    // Listen over registrerte doser.
    private readonly ObservableCollection<RegisteredDose> registeredDoses = [];

    private string locationTextBeforeTablet = string.Empty;
    private bool locationIsLocked;
    private bool isUpdatingDoseText;

    public DoseRegistrationWindow()
    {
        InitializeComponent();
        PopulateMedicationList();
        RegisteredDosesGrid.ItemsSource = registeredDoses;
    }

    // Fyller nedtrekkslisten med medikamenter.
    private void PopulateMedicationList()
    {
        MedicationComboBox.Items.Add(ChooseMedicationText);

        foreach (var medicationName in MedikamentListe.All)
        {
            MedicationComboBox.Items.Add(medicationName);
        }

        MedicationComboBox.SelectedIndex = 0;
    }

    // Låser lokalisasjon når doseformen er tablett.
    private void DoseFormComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (LocationTextBox is null)
        {
            return;
        }

        var isTablet = string.Equals(
            GetSelectedComboBoxText(DoseFormComboBox),
            "Tablett",
            StringComparison.OrdinalIgnoreCase);

        if (isTablet)
        {
            if (!locationIsLocked)
            {
                locationTextBeforeTablet = LocationTextBox.Text;
            }

            locationIsLocked = true;
            LocationTextBox.Text = "--";
            LocationTextBox.IsReadOnly = true;
            LocationTextBox.IsHitTestVisible = false;
            LocationTextBox.Focusable = false;
            LocationTextBox.Foreground = Brushes.Gray;
            LocationTextBox.Background = Brushes.LightGray;
            return;
        }

        if (!locationIsLocked)
        {
            return;
        }

        locationIsLocked = false;
        LocationTextBox.Text = locationTextBeforeTablet;
        LocationTextBox.IsReadOnly = false;
        LocationTextBox.IsHitTestVisible = true;
        LocationTextBox.Focusable = true;
        LocationTextBox.Foreground = Brushes.Black;
        LocationTextBox.Background = Brushes.White;
    }

    // Kontrollerer feltene og registrerer dosen.
    private void CreateDoseButton_Click(object sender, RoutedEventArgs e)
    {
        var medication = GetSelectedComboBoxText(MedicationComboBox);
        var doseForm = GetSelectedComboBoxText(DoseFormComboBox);
        var doseText = DoseTextBox.Text.Trim();
        var startTimeText = StartTimeTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(medication) ||
            medication == ChooseMedicationText)
        {
            ShowInvalidField(
                "Ugyldig forordningsmiddel.",
                MedicationComboBox);

            return;
        }

        if (string.IsNullOrWhiteSpace(doseForm) ||
            doseForm == "Velg...")
        {
            ShowInvalidField(
                "Ugyldig doseform.",
                DoseFormComboBox);

            return;
        }

        if (!TryParsePositiveDose(doseText, out _))
        {
            ShowInvalidField("Ugyldig dose.", DoseTextBox);
            return;
        }

        if (!DateTime.TryParseExact(
                startTimeText,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var startTime))
        {
            ShowInvalidField(
                "Ugyldig starttid.",
                StartTimeTextBox);

            return;
        }

        if (StartDatePicker.SelectedDate is not DateTime startDate)
        {
            ShowInvalidField(
                "Ugyldig startdato.",
                StartDatePicker);

            return;
        }

        var unit = GetSelectedComboBoxText(DoseUnitComboBox);
        var administration =
            GetSelectedComboBoxText(AdministrationRouteComboBox);

        var comment = CommentTextBox.Text.Trim();

        registeredDoses.Add(new RegisteredDose(
            Medication: medication,
            Amount: "--",
            Dose: $"{doseText} {unit}",
            Concentration: "--",
            Administration:
                administration == "Velg..."
                    ? "--"
                    : administration,
            StartTime:
                startTime.ToString(
                    "HH:mm",
                    CultureInfo.InvariantCulture),
            StartDate: startDate.Date,
            Location:
                locationIsLocked ||
                string.IsNullOrWhiteSpace(LocationTextBox.Text)
                    ? "--"
                    : LocationTextBox.Text.Trim(),
            Comment:
                string.IsNullOrWhiteSpace(comment)
                    ? "--"
                    : comment));

        // Nullstiller feltene etter registrering.
        RegisteredDosesGrid.ScrollIntoView(registeredDoses[^1]);
        ResetDoseInput();
        CommentTextBox.Clear();
        DoseTextBox.Focus();
    }

    // Tillater bare tall i dosefeltet.
    private void DoseTextBox_PreviewTextInput(
        object sender,
        TextCompositionEventArgs e)
    {
        e.Handled = true;

        if (e.Text.Length != 1 || !char.IsDigit(e.Text[0]))
        {
            return;
        }

        ReplaceDoseDigit(e.Text[0]);
    }

    // Styrer tastene som kan brukes i dosefeltet.
    private void DoseTextBox_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
        {
            e.Handled = true;
            return;
        }

        switch (e.Key)
        {
            case Key.Left:
            case Key.Home:
                SetDoseCaret(0);
                e.Handled = true;
                break;

            case Key.Right:
            case Key.End:
                SetDoseCaret(2);
                e.Handled = true;
                break;

            case Key.Back:
            case Key.Delete:
                ReplaceDoseDigit('0');
                e.Handled = true;
                break;

            case Key.Space:
            case Key.Decimal:
            case Key.OemPeriod:
            case Key.OemComma:
                e.Handled = true;
                break;
        }
    }

    private void DoseTextBox_PreviewMouseDown(
        object sender,
        MouseButtonEventArgs e)
    {
        e.Handled = true;
        DoseTextBox.Focus();

        var mousePosition = e.GetPosition(DoseTextBox);

        SetDoseCaret(
            mousePosition.X < DoseTextBox.ActualWidth / 2
                ? 0
                : 2);
    }

    private void DoseTextBox_GotKeyboardFocus(
        object sender,
        KeyboardFocusChangedEventArgs e)
    {
        SetDoseCaret(0);
    }

    private void DoseTextBox_SelectionChanged(
        object sender,
        RoutedEventArgs e)
    {
        if (isUpdatingDoseText || sender is not TextBox textBox)
        {
            return;
        }

        SetDoseCaret(NormalizeDoseCaret(textBox.CaretIndex));
    }

    // Erstatter tallet før eller etter punktum.
    private void ReplaceDoseDigit(char digit)
    {
        var currentText = DoseTextBox.Text;

        if (currentText.Length != 3 || currentText[1] != '.')
        {
            currentText = "0.0";
        }

        var digitIndex =
            NormalizeDoseCaret(DoseTextBox.CaretIndex);

        var characters = currentText.ToCharArray();
        characters[digitIndex] = digit;

        isUpdatingDoseText = true;
        DoseTextBox.Text = new string(characters);
        DoseTextBox.CaretIndex = digitIndex;
        DoseTextBox.SelectionLength = 0;
        isUpdatingDoseText = false;
    }

    private void ResetDoseInput()
    {
        isUpdatingDoseText = true;
        DoseTextBox.Text = "0.0";
        DoseTextBox.CaretIndex = 0;
        DoseTextBox.SelectionLength = 0;
        isUpdatingDoseText = false;
    }

    private void SetDoseCaret(int requestedIndex)
    {
        if (DoseTextBox is null)
        {
            return;
        }

        isUpdatingDoseText = true;
        DoseTextBox.CaretIndex =
            NormalizeDoseCaret(requestedIndex);

        DoseTextBox.SelectionLength = 0;
        isUpdatingDoseText = false;
    }

    // Markøren kan bare stå før eller etter punktum.
    private static int NormalizeDoseCaret(int requestedIndex)
    {
        return requestedIndex <= 1 ? 0 : 2;
    }

    // Sjekker at dosen er et positivt tall.
    private static bool TryParsePositiveDose(
        string text,
        out decimal value)
    {
        var norwegianCulture =
            CultureInfo.GetCultureInfo("nb-NO");

        if (decimal.TryParse(
                text,
                NumberStyles.Number,
                norwegianCulture,
                out value) &&
            value > 0)
        {
            return true;
        }

        return decimal.TryParse(
                   text,
                   NumberStyles.Number,
                   CultureInfo.InvariantCulture,
                   out value) &&
               value > 0;
    }

    private void ShowInvalidField(
        string message,
        FrameworkElement field)
    {
        MessageBox.Show(
            this,
            message,
            "Feil",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);

        field.Focus();
    }

    // Henter teksten fra en ComboBox.
    private static string GetSelectedComboBoxText(
        ComboBox comboBox)
    {
        return comboBox.SelectedItem switch
        {
            ComboBoxItem item =>
                item.Content?.ToString() ?? string.Empty,

            object item =>
                item.ToString() ?? string.Empty,

            null =>
                string.Empty
        };
    }

    private void CloseButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}

// Informasjon om én registrert dose.
public sealed record RegisteredDose(
    string Medication,
    string Amount,
    string Dose,
    string Concentration,
    string Administration,
    string StartTime,
    DateTime StartDate,
    string Location,
    string Comment);