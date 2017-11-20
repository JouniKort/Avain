using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AccessHandler = AccessSQL.AccessHandler;

namespace Avain
{
    static class DataBase
    {

        public static DataTable Komennot;
        public static DataTable DT;
        public static List<string> sarakkeet = new List<string>();

        public static void Connect()
        {
            sarakkeet.Add("Hotkey");
            sarakkeet.Add("Komento");
            try
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Avain\\Komennot.mdb"))
                    AccessHandler.Yhdista(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Avain\\Komennot.mdb");
                else
                    AccessHandler.Yhdista("G:\\VS\\VisualStudio Projektit\\Projektit\\Avain\\Komennot.mdb");

                AccessHandler.ViestienNaytto(true, true);
                Komennot = AccessHandler.HaeTaulut(AccessHandler.EtsiTaulut())[0];
                KeyListenercs.DT = Komennot;
            }catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }

        public static void Update()
        {
            AccessHandler.SQLkomentoTaulu("DELETE * FROM komennot");
            AccessHandler.Viesti("Rivejä on " + DT.Rows.Count);
            List<string> Hotkeyt = DT.AsEnumerable().Select(x => x[0].ToString()).ToList();
            AccessHandler.Viesti(String.Join("\n",Hotkeyt));
            foreach (DataRow DR in DT.Rows)
            {
                string[] rivi = new string[] { DR[0].ToString(), DR[1].ToString() };
                AccessHandler.SQLkomentoINSERT(sarakkeet: sarakkeet, nimi: "Komennot", rivi: rivi);
            }
            KeyListenercs.DT = DT;
        }
    }
}
