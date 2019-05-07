using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WPF_Ticketsystem
{

    // Dialogmode (d.h. INSERT oder UPDATE)
    public enum DialogMode { Neu, Aendern };

    /// <summary>
    /// Interaktionslogik für TicketDialog.xaml
    /// </summary>
    public partial class TicketDialog : Window
    {

        private DialogMode _mode;

        public TicketDialog(DialogMode m)
        {
            InitializeComponent();

            this._mode = m;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this._mode == DialogMode.Neu)
            {
                this.Title = "Neues Ticket anlegen";
                this.tbBenutzer.IsReadOnly = false;
                this.cbStatus.IsEnabled = false;
            }
            else
            {
                this.Title = "Ticket bearbeiten";
            }
        }

        private bool Pruefen()
        {
            if (this.tbBenutzer.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Der Benutzername darf nicht leer sein!");
                return false;
            }

            if (this.tbMeldungstext.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Der Meldungstext darf nicht leer sein!");
                return false;
            }

            if (this.cbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Wählen Sie einen Status aus!");
                return false;
            }

            return true;
        }

        private void btAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btOk_Click_v1(object sender, RoutedEventArgs e)
        {
            if (Pruefen())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            MainWindowModel viewmodel = FindResource("mwmodel") as MainWindowModel;

            try
            {

                if (_mode == DialogMode.Aendern)
                    viewmodel.TicketBearbeitung.Update();
                else
                    viewmodel.TicketBearbeitung.Insert();

                this.DialogResult = true;

                this.Close();
            }
            catch (MultiAccessException)
            {
                MessageBoxResult r = MessageBox.Show("Der Datensatz wurde bereits von einem Benutzer verändert!\n\n" +
                  "Sollen die Daten aktualisiert werden?", "Mehrbenutzerfehler", MessageBoxButton.YesNo,
                  MessageBoxImage.Exclamation);

                if (r == MessageBoxResult.Yes)
                {
                    // Ticket neu aus der DB lesen
                    viewmodel.TicketBearbeitung = new Ticket(viewmodel.TicketBearbeitung.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.WriteLine(ex.Message);
            }
        }
    }
}
