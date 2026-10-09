using helseProgram.Simulering;
using System;
using System.Windows;
using System.Windows.Threading;


///////////////// simen /////////////////

namespace helseProgram
{
    public partial class MainWindow : Window
    {
        private readonly SimulasjonsStateMachine stateMachine;
        private readonly SimulasjonsKlokke klokke;
        private readonly DispatcherTimer uiTimer;

        public MainWindow()
        {
            InitializeComponent();

            stateMachine = new SimulasjonsStateMachine();
            klokke = new SimulasjonsKlokke();

            uiTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            uiTimer.Tick += UiTimer_Tick;

            stateMachine.TilstandEndret += OnTilstandEndret;
            UpdateControls(stateMachine.Tilstand);
            OppdaterTidsvisning();
        }

        private void StartKnapp_Click(object sender, RoutedEventArgs e)
        {
            stateMachine.Start();
            klokke.Start();
            Tidslinje.Nullstill();
            uiTimer.Start();
            OppdaterTidsvisning();
        }

        private void PauseKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (stateMachine.Tilstand == SimulasjonsTilstand.Kjorer)
            {
                stateMachine.Pause();
                klokke.Pause();
                uiTimer.Stop();
            }
            else if (stateMachine.Tilstand == SimulasjonsTilstand.Pauset)
            {
                stateMachine.Fortsett();
                klokke.Fortsett();
                uiTimer.Start();
            }

            OppdaterTidsvisning();
        }

        private void StoppKnapp_Click(object sender, RoutedEventArgs e)
        {
            stateMachine.Stopp();
            klokke.Stopp();
            uiTimer.Stop();
            OppdaterTidsvisning();
        }

        private void OpprettDoseKnapp_Click(object sender, RoutedEventArgs e)
        {
            ApneDoseRegistrering();
        }

        private void ApneDoseRegistrering()
        {
            var window = new DoseRegistrationWindow
            {
                Owner = this
            };

            window.ShowDialog();

            // Når DoseRegistrationWindow returnerer den registrerte dosen,
            // kan den plasseres på tidslinjen slik:
            // Tidslinje.LeggTilDose("Medikamentnavn", klokke.SimulasjonsTid);
        }

        private void UiTimer_Tick(object? sender, EventArgs e)
        {
            OppdaterTidsvisning();
        }

        private void OppdaterTidsvisning()
        {
            TimeSpan simulasjonsTid = klokke.SimulasjonsTid;
            SimulasjonsTidText.Text = simulasjonsTid.ToString(@"hh\:mm\:ss");
            Tidslinje.Oppdater(simulasjonsTid);
        }

        private void OnTilstandEndret(SimulasjonsTilstand tilstand)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => OnTilstandEndret(tilstand));
                return;
            }

            UpdateControls(tilstand);
        }

        private void UpdateControls(SimulasjonsTilstand tilstand)
        {
            StartKnapp.IsEnabled = tilstand == SimulasjonsTilstand.IkkeStartet;

            // Den samme knappen brukes til både Pause og Fortsett.
            PauseKnapp.IsEnabled = tilstand == SimulasjonsTilstand.Kjorer ||
                                   tilstand == SimulasjonsTilstand.Pauset;

            StoppKnapp.IsEnabled = tilstand == SimulasjonsTilstand.Kjorer ||
                                   tilstand == SimulasjonsTilstand.Pauset;

            // Hele simuleringen er fryst under pause.
            OpprettDoseKnapp.IsEnabled = tilstand == SimulasjonsTilstand.Kjorer;

            PauseKnapp.Content = tilstand == SimulasjonsTilstand.Pauset
                ? "▶  Fortsett"
                : "Ⅱ  Pause";

            TilstandText.Text = tilstand switch
            {
                SimulasjonsTilstand.IkkeStartet => "Ikke startet",
                SimulasjonsTilstand.Kjorer => "Pågår",
                SimulasjonsTilstand.Pauset => "Pauset",
                SimulasjonsTilstand.Avsluttet => "Avsluttet",
                _ => "Ukjent"
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            uiTimer.Stop();
            stateMachine.TilstandEndret -= OnTilstandEndret;
            base.OnClosed(e);
        }

        private void Tidslinje_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

///////////////// simen /////////////////