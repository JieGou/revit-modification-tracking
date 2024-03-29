﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.IO;

namespace TrackDirect.Utilities
{
    /// <summary>
    /// This class exists to help with schema updates to the SQLite database used for storing results.
    /// In some cases, it is necessary to upgrade the database structure in order to make things work.
    /// </summary>
    public static class DataUtility
    {
        public static Version CurrentVersion = new Version(1, 0);

        private static Assembly _CurrentAsm = System.Reflection.Assembly.GetExecutingAssembly();
        private static Dictionary<Version, string> _UpgradeScripts = new Dictionary<Version, string>() { { new Version(1, 0), "TrackDirect.DBScript.UpgradeToV1.txt" } };

        public static void UpgradeFrom(SQLiteConnection conn, Version v, Action<string> log)
        {
            var todo = _UpgradeScripts.Where(s => s.Key > v).OrderBy(s => s.Key).ToList();
            if (log != null) log("=> There are " + todo.Count + " upgrades to the database.");



            foreach (var script in todo)
            {
                if (log != null) log("   => Version: " + script.Key + " Update: " + script.Value);

                string[] statements = ReadSQLScript(script.Value);
                if (statements == null)
                {
                    if (log != null) log("ISSUE: Unable to find desired script: " + script.Value);
                    continue;
                }

                foreach (string sql in statements)
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

                // then, after that, let's update the specific headers table explicitly.
                var updatecmd = conn.CreateCommand();
                updatecmd.CommandText = "UPDATE _objects_header SET Value = \"" + script.Key.Major + "." + script.Key.Minor + "\" WHERE Keyword = \"SchemaVersion\"";
                updatecmd.ExecuteNonQuery();

                if (log != null) log("=> Completed update: " + script.Key);
            }
        }

        public static string[] ReadSQLScript(string name)
        {


            Stream s = _CurrentAsm.GetManifestResourceStream(name);

            if (s == null) return null;
            string sql = null;
            using (StreamReader sr = new StreamReader(s))
            {
                sql = sr.ReadToEnd();
            }


            return sql.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        }
    }

}
