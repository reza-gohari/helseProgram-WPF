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
    /////Reza, Vindu for hendelsesregistrering.
    public partial class EventRegistrationWindow : System.Windows.Window
    {
        public EventRegistrationWindow()
        {
            InitializeComponent();
        }
        /////Reza, Teksten som er registrert
        public string RegistrertHendelse { get; private set; } = "";

        /////Reza, Sjekker tekst og bekrefter registreringen.
        private void RegistrerHendelseKnapp_Click(object sender, RoutedEventArgs e)
        {
            string hendelse = HendelseTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(hendelse))
            {
                MessageBox.Show("Du må skrive en hendelse.");
                return;
            }

            
            RegistrertHendelse = hendelse;

            
            DialogResult = true;
        }
    }
}
