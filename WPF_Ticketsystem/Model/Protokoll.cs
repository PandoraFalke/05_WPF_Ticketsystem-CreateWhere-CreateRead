using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Ticketsystem
{
  public class Protokoll
  {
    public int Id { get; set; }
    public DateTime Zeitpunkt { get; set; }

    public Ticket Ticket { get; set; }

    public TicketStatus Status { get; set; }

    public String Bearbeiter { get; set; }


    public Protokoll()
    {
    }

    public Protokoll(Ticket t)
    {
      this.Ticket = t;
      this.Status = t.Status;
      this.Bearbeiter = Environment.UserName;
      this.Zeitpunkt = DateTime.Now;
    }

    public static ObservableCollection<Protokoll> AlleLesen()
    {
      return DBProtokoll.AlleLesen();
    }

    public void Speichern()
    {
      DBProtokoll.Insert(this);
    }

    public override String ToString()
    {
      return this.Zeitpunkt.ToString() + ": " + this.Ticket.Meldungstext + " -> " + this.Status + " ("+this.Bearbeiter+")"; 
    }
  }
}
