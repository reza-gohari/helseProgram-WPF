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

        private readonly List<DoseMarkering> doseMarkeringer = new();
        private int tegnedeMinutter;

        public SimulasjonsTidslinje()
        {
            InitializeComponent();
            Loaded += (_, _) => Nullstill();
        }

        public void Oppdater(TimeSpan simulasjonsTid)
        {
            TidslinjeTidText.Text = simulasjonsTid.ToString(@"hh\:mm\:ss");

            int nodvendigeMinutter = Math.Max(
                MinimumMinutter,
                (int)Math.Ceiling(simulasjonsTid.TotalMinutes / 5.0) * 5 + 5);

            if (nodvendigeMinutter != tegnedeMinutter)
            {
                tegnedeMinutter = nodvendigeMinutter;
                TegnTidslinje(simulasjonsTid);
            }
            else
            {
                FlyttTidsmarkor(simulasjonsTid);
            }
        }

        public void Nullstill()
        {
            doseMarkeringer.Clear();
            tegnedeMinutter = MinimumMinutter;
            TegnTidslinje(TimeSpan.Zero);
            TidslinjeScrollViewer.ScrollToHorizontalOffset(0);
        }

        public void LeggTilDose(string medikamentnavn, TimeSpan tidspunkt)
        {
            doseMarkeringer.Add(new DoseMarkering(medikamentnavn, tidspunkt));

            int nodvendigeMinutter = Math.Max(
                MinimumMinutter,
                (int)Math.Ceiling(tidspunkt.TotalMinutes / 5.0) * 5 + 5);

            if (nodvendigeMinutter > tegnedeMinutter)
                tegnedeMinutter = nodvendigeMinutter;

            TegnTidslinje(tidspunkt);
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
                    Y2 = Bunn,
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

            foreach (DoseMarkering dose in doseMarkeringer)
                TegnDose(dose);

            FlyttTidsmarkor(simulasjonsTid);
        }

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
            Canvas.SetTop(punkt, 172);
            TidslinjeCanvas.Children.Add(punkt);

            var navn = new TextBlock
            {
                Text = dose.Navn,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(25, 74, 54))
            };
            Canvas.SetLeft(navn, x + 7);
            Canvas.SetTop(navn, 168);
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
    }
}



///////////////// simen /////////////////