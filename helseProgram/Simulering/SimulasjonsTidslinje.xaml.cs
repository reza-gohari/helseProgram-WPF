using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


///////////////// simen /////////////////

namespace helseProgram.Simulering
{
    public partial class SimulasjonsTidslinje : UserControl
    {
        private const double PikslerPerMinutt = 25;
        private const double VenstreMarg = 25;
        private const double SkalaTopp = 42;
        private const double Bunn = 215;
        private const int MinimumMinutter = 60;
        private TimeSpan gjeldendeSimulasjonsTid;

        private readonly List<DoseMarkering> doseMarkeringer = new();
        private int tegnedeMinutter = 120;

        /// Redigert av REZA:Sender hendelser videre. 
        public event Action<string>? HendelseRegistrert;

        public SimulasjonsTidslinje()
        {
            InitializeComponent();
            Loaded += (_, _) => Nullstill();
        }
        public void Oppdater(TimeSpan simulasjonsTid)
        {
            ///Redigert av REZA,Husker siste simulasjonstid.
            gjeldendeSimulasjonsTid = simulasjonsTid;

            TidslinjeTidText.Text =
                simulasjonsTid.ToString(@"hh\:mm\:ss");

            bool aktivInfusjon = false;

            foreach (var infusjon in infusjoner)
            {
                if (infusjon.SluttTid == null)
                {
                    aktivInfusjon = true;
                    break;
                }
            }

            if (aktivInfusjon)
                TegnTidslinje(simulasjonsTid);
            else
                FlyttTidsmarkor(simulasjonsTid);
        }
        // REZA: Åpner hendelsenvinduet.
        private void NyHendelseKnapp_Click(object sender, RoutedEventArgs e)
        {
            var vindu = new EventRegistrationWindow();

            if (vindu.ShowDialog() == true)
                HendelseRegistrert?.Invoke(vindu.RegistrertHendelse);
        }

        public void Nullstill()
        {
            doseMarkeringer.Clear();
            hendelseMarkeringer.Clear();

            tegnedeMinutter = 120;
            TegnTidslinje(TimeSpan.Zero);
            TidslinjeScrollViewer.ScrollToHorizontalOffset(0);
        }

        public void LeggTilDose(string medikamentnavn, TimeSpan tidspunkt)
        {
            doseMarkeringer.Add(new DoseMarkering(medikamentnavn, tidspunkt));

            tegnedeMinutter = 120;

            TegnTidslinje(tidspunkt);
        }

        //////Reza ,Legger til infusjon 
        public void LeggTilInfusjon(string navn, TimeSpan startTid)
        {
            InfusjonMarkering infusjon = new InfusjonMarkering();

            infusjon.Navn = navn;
            infusjon.StartTid = startTid;
            infusjon.SluttTid = null;

            infusjoner.Add(infusjon);

            InfusjonComboBox.Items.Add(navn);

            VisAktiveInfusjoner();

            TegnTidslinje(startTid);
        }

       
        private void TegnTidslinje(TimeSpan simulasjonsTid)
        {
            TidslinjeCanvas.Children.Clear();
            TidslinjeCanvas.Width = VenstreMarg * 2 + tegnedeMinutter * PikslerPerMinutt;

            TegnRadBakgrunn(65, 75, Color.FromRgb(248, 250, 251));
            TegnRadBakgrunn(140, 75, Color.FromRgb(233, 241, 245));

            for (int minutt = 0; minutt <= tegnedeMinutter; minutt++)
            {
                double x = VenstreMarg + minutt * PikslerPerMinutt;
                bool hovedstrek = minutt % 5 == 0;

                var linje = new Line
                {
                    X1 = x,
                    X2 = x,
                    Y1 = hovedstrek ? 30 : SkalaTopp,
                    Y2 = TidslinjeCanvas.Height,
                    Stroke = hovedstrek
                        ? new SolidColorBrush(Color.FromRgb(142, 158, 166))
                        : new SolidColorBrush(Color.FromRgb(218, 225, 229)),
                    StrokeThickness = hovedstrek ? 1.1 : 0.6
                };
                TidslinjeCanvas.Children.Add(linje);

                if (hovedstrek)
                {
                    var etikett = new TextBlock
                    {
                        Text = TimeSpan.FromMinutes(minutt).ToString(@"hh\:mm"),
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Color.FromRgb(36, 62, 73))
                    };
                    Canvas.SetLeft(etikett, x - 17);
                    Canvas.SetTop(etikett, 8);
                    TidslinjeCanvas.Children.Add(etikett);
                }
            }

            // Tegner registrerte medikamenter.
            foreach (DoseMarkering dose in doseMarkeringer)
                TegnDose(dose);

            ////Reza ,Tegner registrerte infusjoner.
            foreach (InfusjonMarkering infusjon in infusjoner)
            {
                TegnInfusjon(infusjon, simulasjonsTid);
            }

            ////Reza, Tegner registrerte hendelser.
            foreach (HendelseMarkering hendelse in hendelseMarkeringer)
                TegnHendelse(hendelse);

            // Oppdaterer den gule tidsmarkøren.
            FlyttTidsmarkor(simulasjonsTid);
        }
        ///// Reza ,Data om hendelsen.
        private class HendelseMarkering
        {
            public TimeSpan Tidspunkt { get; set; }
            public string Beskrivelse { get; set; } = "";
        }
        /// /Reza Beholder hendelsene når tidslinjen tegnes på nytt.
        private readonly List<HendelseMarkering> hendelseMarkeringer = new();


        private void TegnRadBakgrunn(double topp, double hoyde, Color farge)
        {
            var bakgrunn = new Rectangle
            {
                Width = TidslinjeCanvas.Width,
                Height = hoyde,
                Fill = new SolidColorBrush(farge),
                Stroke = new SolidColorBrush(Color.FromRgb(198, 210, 216)),
                StrokeThickness = 0.5
            };
            Canvas.SetLeft(bakgrunn, 0);
            Canvas.SetTop(bakgrunn, topp);
            TidslinjeCanvas.Children.Add(bakgrunn);
        }

        private void FlyttTidsmarkor(TimeSpan simulasjonsTid)
        {
            FjernElementerMedTag("Tidsmarkor");

            double x = VenstreMarg + simulasjonsTid.TotalMinutes * PikslerPerMinutt;

            var markor = new Line
            {
                X1 = x,
                X2 = x,
                Y1 = 27,
                Y2 = Bunn,
                Stroke = Brushes.Gold,
                StrokeThickness = 3,
                Tag = "Tidsmarkor"
            };
            TidslinjeCanvas.Children.Add(markor);

            var pil = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(x - 6, 27),
                    new Point(x + 6, 27),
                    new Point(x, 37)
                },
                Fill = Brushes.Gold,
                Tag = "Tidsmarkor"
            };
            TidslinjeCanvas.Children.Add(pil);

            if (x > TidslinjeScrollViewer.HorizontalOffset +
                    TidslinjeScrollViewer.ViewportWidth - 80)
            {
                TidslinjeScrollViewer.ScrollToHorizontalOffset(
                    Math.Max(0, x - TidslinjeScrollViewer.ViewportWidth + 80));
            }
        }
        /////Reza ,Tegner dose med punkt og navn.
        private void TegnDose(DoseMarkering dose)
        {
            double x = VenstreMarg + dose.Tidspunkt.TotalMinutes * PikslerPerMinutt;

            var punkt = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(Color.FromRgb(32, 126, 87)),
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
            Canvas.SetLeft(punkt, x - 5);
            Canvas.SetTop(punkt, 143);
            TidslinjeCanvas.Children.Add(punkt);

            var navn = new TextBlock
            {
                Text = dose.Navn,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(25, 74, 54))
            };
            Canvas.SetLeft(navn, x + 7);
            Canvas.SetTop(navn,140);
            TidslinjeCanvas.Children.Add(navn);
        }
        ///// Reza ,Tegner infusjon med linje og navn.
        private void TegnInfusjon(
            InfusjonMarkering infusjon,
            TimeSpan simulasjonsTid)
        {
            double startX = VenstreMarg +
                infusjon.StartTid.TotalMinutes * PikslerPerMinutt;

            TimeSpan sluttTid;

            if (infusjon.SluttTid != null)
            {
                sluttTid = infusjon.SluttTid.Value;
            }
            else
            {
                sluttTid = simulasjonsTid;
            }

            double sluttX = VenstreMarg +
                sluttTid.TotalMinutes * PikslerPerMinutt;

            if (sluttX < startX)
            {
                sluttX = startX;
            }

            int nummer = infusjoner.IndexOf(infusjon);
            
            double hoyde = 180 + nummer * 25;

            Line linje = new Line
            {
                X1 = startX,
                Y1 = hoyde,
                X2 = sluttX,
                Y2 = hoyde,
                Stroke = Brushes.Green,
                StrokeThickness = 4
            };

            TidslinjeCanvas.Children.Add(linje);

            TextBlock navn = new TextBlock
            {
                Text = infusjon.Navn,
                FontSize = 11,
                Foreground = Brushes.DarkGreen
            };

            Canvas.SetLeft(navn, startX + 5);
            Canvas.SetTop(navn, hoyde + 5 );

            TidslinjeCanvas.Children.Add(navn);
        }
        ///// Reza//// Legger til hendelse.
        public void LeggTilHendelse(string beskrivelse, TimeSpan tidspunkt)
        {
            
            hendelseMarkeringer.Add(new HendelseMarkering
            {
                Tidspunkt = tidspunkt,
                Beskrivelse = beskrivelse
            });

            TegnTidslinje(tidspunkt);
        }
        ///// Reza ,Tegner hendelse med punkt og tekst.
        private void TegnHendelse(HendelseMarkering hendelse)
        {
            double x = VenstreMarg
                     + hendelse.Tidspunkt.TotalMinutes * PikslerPerMinutt;

            var punkt = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(Color.FromRgb(230, 110, 40)),
                Stroke = Brushes.White,
                StrokeThickness = 1
            };

            Canvas.SetLeft(punkt, x - 5);
            Canvas.SetTop(punkt, 100);
            TidslinjeCanvas.Children.Add(punkt);
            
            int nummer = hendelseMarkeringer.IndexOf(hendelse);
            double y = 85;
            if (nummer % 2 == 1)
            {
                y = 120;
            }

            var navn = new TextBlock
            {
                Text = hendelse.Beskrivelse,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(160, 70, 20))
            };

            Canvas.SetLeft(navn, x + 15);
            Canvas.SetTop(navn, y-4);
            TidslinjeCanvas.Children.Add(navn);
        }
        

        private void FjernElementerMedTag(string tag)
        {
            for (int i = TidslinjeCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (TidslinjeCanvas.Children[i] is FrameworkElement element &&
                    Equals(element.Tag, tag))
                {
                    TidslinjeCanvas.Children.RemoveAt(i);
                }
            }
        }
        /// Reza ,Infusjonsliste.
        private readonly List<InfusjonMarkering> infusjoner = new();

        ///// Reza//// Data om dosen
        private sealed class DoseMarkering
        {
            public DoseMarkering(string navn, TimeSpan tidspunkt)
            {
                Navn = navn;
                Tidspunkt = tidspunkt;
            }

            public string Navn { get; }
            public TimeSpan Tidspunkt { get; }
        }
        ///// Reza ,Stopper valgt infusjon.
        private void StoppInfusjonKnapp_Click(object sender, RoutedEventArgs e)
        {
            string? valgtNavn = InfusjonComboBox.SelectedItem as string;

            foreach (var infusjon in infusjoner)
            {
                if (infusjon.Navn == valgtNavn &&
                    infusjon.SluttTid == null)
                {
                    infusjon.SluttTid = gjeldendeSimulasjonsTid;

                    TegnTidslinje(gjeldendeSimulasjonsTid);

                    InfusjonComboBox.Items.Remove(valgtNavn);
                    return;
                }
            }

            MessageBox.Show("Velg en aktiv infusjon.");
        }
        ///// Reza ,Lagrer navn, starttid og sluttid for infusjon.
        private class InfusjonMarkering
        {
            public string Navn { get; set; } = "";

            
            public TimeSpan StartTid { get; set; }

            
            public TimeSpan? SluttTid { get; set; }
        }
        ///// Reza ,Viser stoppknapper for aktive infusjoner.
        private void VisAktiveInfusjoner()
        {
            // Fjerner gamle knapper.
            AktiveInfusjonerPanel.Children.Clear();

            foreach (var infusjon in infusjoner)
            {
                if (infusjon.SluttTid == null)
                {
                    Button knapp = new Button();

                    knapp.Content = "Stopp " + infusjon.Navn;
                    knapp.Height = 25;
                    knapp.Margin = new Thickness(0, 2, 0, 2);

                    AktiveInfusjonerPanel.Children.Add(knapp);
                }
            }
        }


    }
}



///////////////// simen /////////////////