using helseProgram.Simulering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using helseProgram.Modeller;


///////////////// simen /////////////////

namespace helseProgram
{
    public partial class MainWindow : Window
    {
        private readonly SimulasjonsStateMachine stateMachine;
        private readonly SimulasjonsKlokke klokke;
        private readonly DispatcherTimer uiTimer;

        /////Reza, Doseliste for medikamenttabellen.
        ///Ado, oppdaterte listen fra DoseRegistration til MedikamentAdministerig
        ///fra modeller. 
        private readonly List<MedikamentAdministrering> registrerteDoser = new();
        
        public MainWindow()
        {
            InitializeComponent();
            // REZA: MainWindow lytter etter nye hendelser fra tidslinjen.
            Tidslinje.HendelseRegistrert += Tidslinje_HendelseRegistrert;

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

        // Azo Start
        private void PasientInfoKnapp_Click(object sender, RoutedEventArgs e)
        {
            var pasientVindu = new PasientInfoWindow
            {
                Owner = this
            };
            pasientVindu.ShowDialog();
        }
        //Azo Slutt

        /////Reza, Åpner registrering og viser doser


        private void ApneDoseRegistrering()
        {
            var window = new DoseRegistrationWindow
            {
                Owner = this
            };

            window.ShowDialog();

            foreach (var dose in window.RegistrerteDoser)
            {
                TimeSpan tid = klokke.SimulasjonsTid;

                dose.SimulasjonstidTicks = tid.Ticks;
                registrerteDoser.Add(dose);

                // Kontinuerlig infusjon registreres separat fra vanlig dose.
                if (dose.AdministreringsType == "Infusjon" &&
                    dose.ErKontinuerlig)
                {
                    Tidslinje.LeggTilInfusjon(
                        dose.Medikament.MedikamentNavn,
                        tid);
                }
                else
                {
                    Tidslinje.LeggTilDose(
                        dose.Medikament.MedikamentNavn,
                        tid);
                }
            }

            MedikamenterDataGrid.ItemsSource = null;

            MedikamenterDataGrid.ItemsSource = registrerteDoser.Select(d => new
            {
                Simulasjonstid = TimeSpan.FromTicks(
                    d.SimulasjonstidTicks.GetValueOrDefault())
                    .ToString(@"hh\:mm\:ss"),

                Medication = d.Medikament.MedikamentNavn,

                Dose =
                    $"{d.Dose.ToString(CultureInfo.GetCultureInfo("nb-NO"))} {d.Enhet}",

                DoseForm = d.AdministreringsType
            }).ToList();
        }
        /////Reza, Viser hendelsen i tabell og tidslinje

        private void Tidslinje_HendelseRegistrert(string hendelse)
        {
            TimeSpan tid = klokke.SimulasjonsTid;

            Tidslinje.LeggTilHendelse(hendelse, tid);

            var liste = HendelserDataGrid.ItemsSource as List<object>
                        ?? new List<object>();

            liste.Add(new
            {
                Tidspunkt = tid.ToString(@"hh\:mm\:ss"),
                Beskrivelse = hendelse
            });

            HendelserDataGrid.ItemsSource = null;
            HendelserDataGrid.ItemsSource = liste;
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