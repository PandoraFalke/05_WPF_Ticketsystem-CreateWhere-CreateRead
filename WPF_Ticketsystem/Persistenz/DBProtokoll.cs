using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Ticketsystem
{
  public class DBProtokoll
  {
    public static ObservableCollection<Protokoll> AlleLesen()
    {
      
      string sql = "SELECT * FROM protokoll";

      ObservableCollection<Protokoll> list = new ObservableCollection<Protokoll>();

            using (DBZugriff dbz = new DBZugriff())
            {

                MySqlDataReader reader = dbz.ExecuteReader(sql);

                while (reader.Read())
                {
                    Protokoll t = new Protokoll();
                    GetDataFromReader(reader, t);
                    list.Add(t);
                }

                reader.Close();
            }

      return list;
    }

    private static void GetDataFromReader(MySqlDataReader reader, Protokoll t)
    {
      t.Id = Convert.ToInt32(reader["id"]);


      int ticketId = Convert.ToInt32(reader["TicketId"]);
      t.Ticket = new Ticket(ticketId);

      t.Bearbeiter = reader.GetString("Bearbeiter");
      t.Zeitpunkt = reader.GetDateTime("Zeitpunkt");
      //t.Zeitpunkt = Convert.ToDateTime(reader["Zeitpunkt"]);

      t.Status = (TicketStatus)(Convert.ToInt32(reader["Status"]));
      /*
      int statusId = Convert.ToInt32(reader["Status"]);

      switch (statusId)
      {
        case 0:
          t.Status = TicketStatus.Neu;
          break;

        case 1:
          t.Status = TicketStatus.Bearbeitung;
          break;

        case 2:
          t.Status = TicketStatus.Gelöst;
          break;

        case 3:
          t.Status = TicketStatus.Gelöscht;
          break;

        default:
          throw new Exception("Unbekannter Status in DB");
      }
      */
    }

    public static void Insert(Protokoll p)
    {

            using (DBZugriff dbz = new DBZugriff())
            {
                string sql = $"INSERT INTO protokoll (TicketId, Bearbeiter, Status, Zeitpunkt) VALUES({p.Ticket.Id},'{p.Bearbeiter}','{(int)p.Status}', '{p.Zeitpunkt.ToString("yyyy-MM-dd HH:mm:ss")}')";
                int anz = dbz.ExecuteNonQuery(sql);

                if (anz == 0)
                {
                    throw new Exception("Änderungen konnten nicht gespeichert werden! \nMöglicherweise gibt es dieses Ticket nicht mehr!");
                }
            }
    }
  }
}
