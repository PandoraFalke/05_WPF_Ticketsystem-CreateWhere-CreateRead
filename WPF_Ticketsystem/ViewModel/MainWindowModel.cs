using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPF_Ticketsystem
{
  public class MainWindowModel : BaseModel
  {

    private ObservableCollection<Protokoll> _lstProt = new ObservableCollection<Protokoll>();

    private ObservableCollection<Ticket> _lstTickets = new ObservableCollection<Ticket>();
    public ObservableCollection<Ticket> LstTickets
    {
      get
      {
        return this._lstTickets;
      }
      set
      {
        _lstTickets = value;
        onPropertyChanged("LstTickets");
      }
    }

    private Ticket _selTicket;
    public Ticket SelTicket
    {
      get
      {
        return this._selTicket;
      }
      set
      {   
        _selTicket = value;
       
        onPropertyChanged("SelTicket");
        // folgende Property ändert sich (möglicherweise)
        // indirekt mit
        onPropertyChanged("ButtonsEnabled");
      }
    }


    private Ticket _ticketBearbeitung = null;
    public Ticket TicketBearbeitung
    {
      get
      {
        return this._ticketBearbeitung;
      }
      set
      {
       
        _ticketBearbeitung = value;
        onPropertyChanged("TicketBearbeitung");
      }
    }

    

    private List<TicketStatus> _lstStatus = new List<TicketStatus>();

    public List<TicketStatus> LstStatus
    {
      get
      {
        return this._lstStatus;
      }

      set
      {
        _lstStatus = value;
        onPropertyChanged("LstStatus");
      }
    }

    public bool ButtonsEnabled
    {
      get
      {
        //if (this.SelTicket != null)
        //  return true;
        //else
        //  return false;

        return this.SelTicket != null;
      }
    }

        public ObservableCollection<Protokoll> LstProt
        {
            get
            {
                return _lstProt;
            }

            set
            {
                _lstProt = value;
                onPropertyChanged("LstProt");
            }
        }

      

     public MainWindowModel()
    {
      this.FillModel();
    }

    public void FillModel()
    {
      this.LstTickets = new ObservableCollection<Ticket>(Ticket.AlleLesen());

      //this.LstStatus.Add(TicketStatus.Bearbeitung);
      //this.LstStatus.Add(TicketStatus.Gelöst);
      //this.LstStatus.Add(TicketStatus.Gelöscht);

      // kurz und trickreich:
      foreach (TicketStatus s in Enum.GetValues(typeof(TicketStatus)))
      {
        this.LstStatus.Add(s);
      }
    }
  }
}
