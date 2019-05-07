using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Ticketsystem
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel _viewmodel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)

        {
            // In XAML definiert Ressource im Code behind verfügbar machen
            this._viewmodel = FindResource("mwmodel") as MainWindowModel;
        }

        private void btNeu_Click(object sender, RoutedEventArgs e)
        {
            // neues Ticket erstellen
            _viewmodel.TicketBearbeitung = new Ticket();

            TicketDialog dlg = new TicketDialog(DialogMode.Neu);
            // modaler Dialog: aufrufendes Fenster ist blockiert, bis Dialog geschlossen wird
            bool? result = dlg.ShowDialog();

            if (result == true) // Dialog wurde mit OK geschlossen
            {
                // tickets von der DB neu einlesen (MultiUser !!)
                _viewmodel.LstTickets = Ticket.AlleLesen();

            }
        }

        private void btBearbeiten_Click(object sender, RoutedEventArgs e)
        {
            if (this._viewmodel.SelTicket == null)
                return;

            TicketDialog dlg = new TicketDialog(DialogMode.Aendern);

            
            //_viewmodel.TicketBearbeitung = _viewmodel.SelTicket.Clone();

            // _viewmodel.TicketBearbeitung = _viewmodel.SelTicket;

            // Ticket neu lesen wegen Multiuser
            _viewmodel.TicketBearbeitung = new Ticket(_viewmodel.SelTicket.Id);

            bool? result = dlg.ShowDialog();

            if (result == true) // Dialog wurde mit OK geschlossen
            {
                _viewmodel.LstTickets = Ticket.AlleLesen();
            }
        }

        private void btEnde_Click(object sender, RoutedEventArgs e)
        {
            //  MessageBox.Show("Open :" + DBZugriff.ZOpen + " Close: " + DBZugriff.ZClose + " Dispose Close: " + DBZugriff.ZDClose + " Fehler:" + DBZugriff.ZFehler);
            this.Close();
        }

        private void btDel_Click(object sender, RoutedEventArgs e)
        {
            if (this._viewmodel.SelTicket != null)
            {
                MessageBoxResult r = MessageBox.Show("Wollen Sie das Ticket ganz, ganz echt löschen?", "Sind Sie sicher?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                {
                    this._viewmodel.SelTicket.Loeschen();

                    _viewmodel.LstTickets = Ticket.AlleLesen();
                }
            }
        }

        private void btProtokoll_Click(object sender, RoutedEventArgs e)
        {
            _viewmodel.LstProt = Protokoll.AlleLesen();

            ProtokollDialog dlg = new ProtokollDialog();

            dlg.ShowDialog();
        }
    }
}
