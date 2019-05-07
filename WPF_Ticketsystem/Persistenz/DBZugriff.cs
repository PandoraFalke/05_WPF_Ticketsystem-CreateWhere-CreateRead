using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPF_Ticketsystem
{
    public class DBZugriff : IDisposable
    {
        const string CONSTRING  = "server=192.168.70.155;user id=stephan; pwd=geheim1234?; database=ticket;allowuservariables=True; SslMode=none";

        MySqlConnection _con;

        // Wenn nicht null, dann mit Transaktion

        // Referenz auf ein Transaktionobjekt
        // Nur eine Transaktion ist gleichzeitig erlaubt, daher statisch
        static MySqlTransaction _trans;


        // Transaktion beginnen
        // Jede Transaktion hat eine feste Connection, die NICHT geschlossen
        // werden darf, bevor die Transaktion fertig ist

        public void BeginTransaction()
        {
            // Connection erzeugen und Transaktionsobjekt merken
            // Über das Transaktionobjekt kann wieder auf die
            // Connection zugegriffen werden
            // Die lokale Referenz con wird daher nicht mehr benötigt
            MySqlConnection con = new MySqlConnection(CONSTRING);
            con.Open();
            _trans = con.BeginTransaction();
        }

        // Ist die Transaktion fehlerfrei verlaufen, dann Commit()

        public void Commit()
        {
       
            _trans.Commit();
            // Connection der Transaktion schließen
            _trans.Connection.Close();
            // Transaktionsobjekt löschen
            _trans = null;
          
        }

        public void RollBack()
        {
            _trans.Rollback();
            _trans.Connection.Close();
            _trans = null;
         
        }

        void OpenDB()
        {
            
                _con = new MySqlConnection(CONSTRING);
                _con.Open();
              
           
        }

        void CloseDB()
        {
            _con.Close();
           
        }

    public int ExecuteNonQuery(string sql, bool doNotCloseCon = false)
    {

            int anz = 0;

            if (_trans == null)
            {

                // Datenbankverbindung öffnen
                OpenDB();
                // SQL Befehl über die Connection an DB schicken
                MySqlCommand cmd = new MySqlCommand(sql, _con);
                // NonQuery SQL Kommando, d.h. es wird nur das Kommando an die DB geschickt und dort ausgeführt
                anz = cmd.ExecuteNonQuery(); // anz = Anzahl der vom Kommando betroffenen Datensätze
                CloseDB();
               
            }
            // unter Transaktionskontrolle
            else
            {
                // Transaktionsobjekt ist vorhanden (BeginTransaction())
                // Referenz auf das Transaktionsobjekt wird an das Commendobjekt übergeben
                MySqlCommand cmd = new MySqlCommand(sql, _trans.Connection, _trans);
                // NonQuery SQL Kommando, d.h. es wird nur das Kommando an die DB geschickt und dort ausgeführt
                anz = cmd.ExecuteNonQuery(); // anz = Anzahl der vom Kommando betroffenen Datensätze
                // Connection MUSS offen bleiben
            }

      return anz;
    }

    public MySqlDataReader ExecuteReader(string sql)
    {
      OpenDB();
      MySqlCommand cmd = new MySqlCommand(sql, _con);

      // DataReader(Art virtuelle Datei oder Tabelle) an Aufrufer schicken. Connection muss offen bleiben, da nicht alle 
      // Sätze auf einmal geschickt werden!
      return cmd.ExecuteReader();
    }

    public int GetLastInsertId()
    {
      MySqlCommand cmd = new MySqlCommand("SELECT LAST_INSERT_ID()", _con);
      return Convert.ToInt32(cmd.ExecuteScalar());
    }

        public void Dispose()
        {
           // Nullable
           // d.h. Methode wird nicht aufgeführt wenn Referenz nicht null  
                _con?.Close();

         
        }

        public List<T> Lesen<T>(string sql)
        {
            // Keine Arrays, Collections, Referenzen

            List<T> pal = new List<T>();

            // Reader holen
            MySqlDataReader rdr = ExecuteReader(sql);

            // und abarbeiten
            while (rdr.Read())
            {
                // Objekt erzeugen (Default Konstruktor nötig)
                object t = Activator.CreateInstance(typeof(T), null);

                // Jedes Feld in Reader
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    string name = rdr.GetName(i);

                    // Namensgleiche Property holen
                    PropertyInfo pi = typeof(T).GetProperty(name);

                    // Info in Variable übernehmen
                    pi.SetValue(t, rdr[i]);
                }

                // In Liste ausnehmen
                pal.Add((T)t);
            }


            rdr.Close();

            return pal;


        }

        public void EinenLesen<T>(string sql, T ofill)
        {

            MySqlDataReader rdr = ExecuteReader(sql);

            // Ersten Satz Lesen
            rdr.Read();

            for (int i = 0; i < rdr.FieldCount; i++)
            {
                string name = rdr.GetName(i);
                PropertyInfo pi = typeof(T).GetProperty(name);
                pi.SetValue(ofill, rdr[i]);
            }

            rdr.Close();
        }

        public static string CreateWhere(object o)
        {
            Type t = o.GetType();


            //       BindingFlags f = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            PropertyInfo[] fia = t.GetProperties();

            string pstring = " Where ";



            foreach (PropertyInfo fi in fia)
            {

                // Properties vom gleichen Typ werden NICHT reflektiert.
                if (fi.PropertyType != t)
                {
                    // Sonderbehandung für DateTime (Format)
                    if (fi.PropertyType == typeof(DateTime))
                    {
                        pstring += "" + fi.Name + " = '" + Convert.ToDateTime(fi.GetValue(o)).ToString("yyyy-MM-dd") + "' AND ";
                    }
                    // Enum in int wandeln 
                    else if (fi.PropertyType.BaseType == typeof(Enum))
                    {
                        pstring += "" + fi.Name + " = '" + (int)Convert.ChangeType(fi.GetValue(o), fi.PropertyType) + "' AND ";
                    }
                    else
                    {
                        // Rest konvertieren 
                        // pstring += "" + fi.Name + " = '" + Convert.ChangeType(fi.GetValue(o), fi.PropertyType).ToString() + "' AND ";
                        pstring += "" + fi.Name + " = '" + fi.GetValue(o).ToString() + "' AND ";
                    }
                }
            }

            // where += pstring;
            pstring = pstring.Remove(pstring.Length - 4, 4);

            return pstring;
        }

        public List<T> Lesen4<T>(string sql, List<MySqlParameter> pa)
        {
            // Keine Arrays, Collections, Referenzen

            List<T> pal = new List<T>();

            MySqlDataReader rdr = ExecuteReaderP(sql, pa);

            while (rdr.Read())
            {
                object t = Activator.CreateInstance(typeof(T), null);
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    PropertyInfo pi = typeof(T).GetProperty(rdr.GetName(i));
                    pi.SetValue(t, rdr[i]);
                }

                pal.Add((T)t);
            }



            return pal;


        }

    }
}
