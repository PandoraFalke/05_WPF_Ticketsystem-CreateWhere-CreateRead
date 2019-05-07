using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Collections.ObjectModel;

namespace WPF_Ticketsystem
{
    // Umlaute sind hier der Einfachheit halber erlaubt
    public enum TicketStatus { Neu, Bearbeitung, Gelöst, Gelöscht };

    public class Ticket
    {
        private int _id;
        private TicketStatus _status;
        private string _meldungstext, _benutzer;
        private DateTime _datum;

        public Ticket Sik { get; set; }

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public TicketStatus Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
            }
        }

        public string Meldungstext
        {
            get
            {
                return _meldungstext;
            }

            set
            {
                _meldungstext = value;
            }
        }

        public String Kurztext
        {
            get
            {
                int l = this.Meldungstext.Length;

                if (l > 50)
                    l = 50;

                return _meldungstext.Substring(0, l) + " ...";
            }
        }

        public string Benutzer
        {
            get
            {
                return _benutzer;
            }

            set
            {
                _benutzer = value;
            }
        }

        public DateTime Datum
        {
            get
            {
                return _datum;
            }

            set
            {
                _datum = value;
            }
        }



        public Ticket(int id, string meldungstext, string benutzer, TicketStatus status, DateTime datum)
        {
            Id = id;
            Meldungstext = meldungstext;
            Status = status;
            Benutzer = benutzer;
            Datum = datum;
        }

        public Ticket(string benutzer, string meldungstext, DateTime datum)
        {
            // die ID wird erst beim Speichern (in die Datei) gesetzt
            Meldungstext = meldungstext;
            Status = TicketStatus.Neu;
            Benutzer = benutzer;
            Datum = datum;
        }

        public Ticket(int id)
        {
            this.Id = id;
            DBTicket.Lesen(this);
        }

        public Ticket()
        {
            // default Werte für neue Tickets
            this.Datum = DateTime.Now;
            this.Status = TicketStatus.Neu;
        }


        public static ObservableCollection<Ticket> AlleLesen()
        {
            return DBTicket.AlleLesen();
        }



        public void Loeschen()
        {
            DBTicket.Loeschen(this);
        }

        public Ticket Clone()
        {
            // erzeugt eine flache Kopie des Objekts
            return this.MemberwiseClone() as Ticket;
        }

        public override string ToString()
        {
            return Id.ToString().PadLeft(3) + " " + Datum.ToShortDateString() + " " + Status.ToString().PadRight(11) + " " + Benutzer.PadRight(10) + " " + Kurztext;
        }

        public override bool Equals(object obj)
        {
            if (obj is Ticket)
                return this.Id == ((Ticket)obj).Id;
            else
                return false;
        }


        public void Insert()
        {
            DBTicket.Insert(this);
        }

        public void Update()
        {
            DBTicket.Update(this);
        }
    }
}



