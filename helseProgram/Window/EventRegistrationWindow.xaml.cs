using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace helseProgram
{
    /// <summary>
    /// Interaction logic for EventRegistrationWindow.xaml
    /// </summary>
    public partial class EventRegistrationWindow : System.Windows.Window
    {
        public EventRegistrationWindow()
        {
            InitializeComponent();
        }
        // REZA: Lagrer teksten som brukeren registrerer.
        public string RegistrertHendelse { get; private set; } = "";


        // REZA: Kontrollerer at brukeren har skrevet en hendelse.
        private void RegistrerHendelseKnapp_Click(object sender, RoutedEventArgs e)
        {
            string hendelse = HendelseTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(hendelse))
            {
                MessageBox.Show("Du må skrive en hendelse.");
                return;
            }

            // Gjør hendelsen tilgjengelig for tidslinjen.
            RegistrertHendelse = hendelse;

            // Lukker vinduet og bekrefter registreringen.
            DialogResult = true;
        }
    }
}
