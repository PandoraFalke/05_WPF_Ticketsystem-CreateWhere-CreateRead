using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WPF_Ticketsystem
{
  public static class DBTicket
  {

    public static ObservableCollection<Ticket> AlleLesen()
    {

            //using (DBZugriff dbz = new DBZugriff())
            //{


            //    try
            //    {
            //        string sql = "SELECT * FROM Ticket";
            //        ObservableCollection<Ticket> list = new ObservableCollection<Ticket>();
            //        MySqlDataReader reader = dbz.ExecuteReader(sql);

            //        while (reader.Read())
            //        {
            //            Ticket t = new Ticket();
            //            GetDataFromReader(reader, t);
            //            list.Add(t);
            //        }



            //        return list;
            //    }
            //    catch (Exception ex)
            //    {
            //        return new ObservableCollection<Ticket>();
            //    }

            string sql = "SELECT * FROM Ticket";
            List<Ticket> lt = null;
            using (DBZugriff dbz = new DBZugriff())
            {
                lt = dbz.Lesen<Ticket>(sql);
            }

            // Sik objekt erzeugen (wird nicht von der generischen Lesen() erledigt !)
            foreach(Ticket t in lt)
               t.Sik = new Ticket(t.Id, t.Meldungstext, t.Benutzer, t.Status, t.Datum);

            return new ObservableCollection<Ticket>(lt);
        
         }

    private static void GetDataFromReader(MySqlDataReader reader, Ticket t)
    {
      t.Id = Convert.ToInt32(reader["id"]);
      t.Benutzer = Convert.ToString(reader["benutzer"]);
      t.Status = (TicketStatus)(reader["statusid"]); 
      t.Meldungstext = Convert.ToString(reader["meldungstext"]);
      t.Datum = Convert.ToDateTime(reader["datum"]);
      t.Sik = new Ticket(t.Id, t.Meldungstext, t.Benutzer, t.Status, t.Datum);
    }

        public static void Update(Ticket t)
        {

            using (DBZugriff dbz = new DBZugriff())
            {
                // Trasnaktion starten
                // Eine Änderung (UPDATE) des Tickets MUSS einen
                // Eintrag (INSERT) in die Protokolltabelle durchführen

                // Transaktion starten
                dbz.BeginTransaction();
                try
                {
                    string sql = $"UPDATE Ticket SET Status='{(int)t.Status}', benutzer='{t.Benutzer}', meldungstext='{t.Meldungstext}',Datum='{t.Datum.ToString("yyyy-MM-dd")}' WHERE Id={t.Sik.Id} AND benutzer='{t.Sik.Benutzer}' AND meldungstext='{t.Sik.Meldungstext}' AND status='{(int)t.Sik.Status}'";
                    int anz = dbz.ExecuteNonQuery(sql);

                    // Es muss genau 1 Ticket geändert werden, ansonsten Fehler
                    if (anz != 1)
                    {
                        dbz.RollBack();
                        throw new MultiAccessException("Änderungen konnten nicht gespeichert werden! \nMöglicherweise gibt es dieses Ticket nicht mehr oder die Daten des Tickets wurden in der Zwischenzeit geändert und stimmen nicht mehr mit Ihrem letzten \nKentnissstand überein!");

                    }
                    else
                    {
                        // wurde der Status geändert?
                        if (t.Status != t.Sik.Status)
                        {

                            Protokoll p = new Protokoll(t);
                            p.Speichern();

                        }

                        // Wenn bisher keine Exception aufgetreten ist
                        // --> alles ok 
                        dbz.Commit();

                        t.Sik = new Ticket(t.Id, t.Meldungstext, t.Benutzer, t.Status, t.Datum);
                    }

                }
                catch
                {
                    // Falls bei einem Zustandsübergang ein Problem (Exception)
                    // aufgetreten ist --> Zurück
                    dbz.RollBack();
                    throw new Exception("FEHLER: INSERT Protokoll. Rollback durchgeführt !");
                }
            }
        }
        

    public static void Insert(Ticket t)
    {

            // ToDo Transaktion einfügen !!!!

            using (DBZugriff dbz = new DBZugriff())
            {

                string sql = $"INSERT INTO Ticket(Status, Meldungstext, Benutzer, Datum) VALUES({(int)t.Status},'{t.Meldungstext}','{t.Benutzer}', CURRENT_DATE())";
                int anz = dbz.ExecuteNonQuery(sql, true); // Con soll offen bleiben!

                if (anz == 0)
                {
                    throw new Exception("Änderungen konnten nicht gespeichert werden! \nMöglicherweise gibt es dieses Ticket nicht mehr!");
                }

                t.Id = dbz.GetLastInsertId();


                Protokoll p = new Protokoll(t);
                p.Speichern();
            }
    }

    public static void Loeschen(Ticket t)
    {

            // Variante mit "logischem" Löschen:
            t.Status = TicketStatus.Gelöscht;
            DBTicket.Update(t);

            // Variante mit "echtem" Löschen:

      //      string sql = $"DELETE FROM Ticket WHERE Id={t.Id}";
      //int anz = new DBZugriff().ExecuteNonQuery(sql);

      //if (anz == 0)
      //{
      //  throw new Exception("Ticket konnte nicht gelöscht werden! \nMöglicherweise gibt es dieses Ticket nicht mehr!");
      //}
      
    }
    public static void Lesen(Ticket tick)
    {

            using (DBZugriff dbz = new DBZugriff())
            {
                string sql = $"SELECT * FROM Ticket WHERE Id = '{tick.Id}'";

                ////      dbz.OpenDB();
                //MySqlDataReader rd = dbz.ExecuteReader(sql);

                //try
                //{
                //    while (rd.Read())
                //    {
                //        GetDataFromReader(rd, tick);
                //    }
                //}
                //catch (Exception)
                //{
                //    throw new Exception("Kein Ticket gefunden! \n\nMöglicherweise wurde das Ticket auch in der Zwischenzeit geändert und die Daten stimmen nicht mehr mit ihrem letzen Kentnissstand überein!");
                //}
                //finally
                //{
                //    rd.Close();


                //}

                dbz.EinenLesen<Ticket>(sql, tick);

               
                tick.Sik = new Ticket(tick.Id, tick.Meldungstext, tick.Benutzer, tick.Status, tick.Datum);


            }
    }
    
  }
}
