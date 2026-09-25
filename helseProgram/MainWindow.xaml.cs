using helseProgram.Simulering;
using System.Windows;
using System.Windows.Threading;


namespace helseProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

            // Subscribe to state changes to update UI (enable/disable buttons)
            stateMachine.TilstandEndret += OnTilstandEndret;

            // Initialize controls according to initial state
            UpdateControls(stateMachine.Tilstand);

            uiTimer = new DispatcherTimer();

            uiTimer.Interval = TimeSpan.FromMilliseconds(100);

            uiTimer.Tick += UiTimer_Tick;
        }

        private void UiTimer_Tick(object? sender, EventArgs e)
        {
            SimulasjonsTidText.Text =
                klokke.SimulasjonsTid.ToString(@"hh\:mm\:ss");
        }
        private void PauseKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (stateMachine.Tilstand == SimulasjonsTilstand.Kjorer)
            {
                stateMachine.Pause();

                klokke.Pause();

                uiTimer.Stop();

                PauseKnapp.Content = "Fortsett";

                // Pause-knappen må fortsatt være aktiv
                PauseKnapp.IsEnabled = true;
                StoppKnapp.IsEnabled = true;
            }
            else if (stateMachine.Tilstand == SimulasjonsTilstand.Pauset)
            {
                stateMachine.Fortsett();

                klokke.Fortsett();

                uiTimer.Start();

                PauseKnapp.Content = "Pause";
            }
        }

        private void StoppKnapp_Click(object sender, RoutedEventArgs e)
        {
            stateMachine.Stopp();

            klokke.Stopp();

            uiTimer.Stop();

            SimulasjonsTidText.Text =
                klokke.SimulasjonsTid.ToString(@"hh\:mm\:ss");
        }

        private void StartKnapp_Click(object sender, RoutedEventArgs e)
        {
            stateMachine.Start();
            klokke.Start();
            uiTimer.Start();
        }

        private void OnTilstandEndret(Simulering.SimulasjonsTilstand tilstand)
        {
            // Ensure UI update happens on UI thread
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => OnTilstandEndret(tilstand));
                return;
            }

            UpdateControls(tilstand);
        }

        private void UpdateControls(Simulering.SimulasjonsTilstand tilstand)
        {
            //StartKnapp, PauseKnapp and StoppKnapp in XAML
            StartKnapp.IsEnabled = tilstand == Simulering.SimulasjonsTilstand.IkkeStartet;
            PauseKnapp.IsEnabled = tilstand == Simulering.SimulasjonsTilstand.Kjorer;
            StoppKnapp.IsEnabled = tilstand == Simulering.SimulasjonsTilstand.Kjorer ||
                                   tilstand == Simulering.SimulasjonsTilstand.Pauset;
        }

    }
}