using System;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;


namespace TrackDirect.Utilities
{
    public class ChangeSettings : AppSettings<ChangeSettings>
    {
        public enum LogLevel { Basic, Verbose };
        private static XmlDocument _doc;
        public bool IsAutoEnable = true;


        #region Settings by metamorphosis
        public static Autodesk.Revit.DB.Color GetColor(object typeName)
        {
            readData();

            string name = typeName.ToString();

            if (_doc == null)
            {
                System.Diagnostics.Debug.WriteLine("Did not find settings document!");
                return new Autodesk.Revit.DB.Color(255, 0, 0); // default
            }

            XmlNode node = _doc.SelectSingleNode("/Settings/ColorChoices/ChangeType[@name='" + name + "']");

            if (node == null)
            {
                System.Diagnostics.Debug.WriteLine("Did not find setting: " + name);
                return new Autodesk.Revit.DB.Color(255, 0, 0);
            }

            string val = node.Attributes["color"].Value;


            System.Drawing.Color c = System.Drawing.Color.FromName(val);
            if (c != null)
            {
                return new Autodesk.Revit.DB.Color(c.R, c.G, c.B);
            }
            else
            {
                // see if it's RGB
                string[] vals = val.Split(',');
                if (vals.Length == 3)
                {
                    int r, g, b;
                    if ((Int32.TryParse(vals[0], out r)) && (Int32.TryParse(vals[1], out g)) && (Int32.TryParse(vals[2], out b)))
                    {
                        return new Autodesk.Revit.DB.Color((byte)r, (byte)g, (byte)b);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("Unable to understand color: " + val);

            return new Autodesk.Revit.DB.Color(255, 0, 0);
        }

        public static string GetDefaultCategories()
        {
            readData();


            if (_doc == null)
            {
                System.Diagnostics.Debug.WriteLine("Did not find settings document!");
                return null;
            }

            XmlElement elem = _doc.SelectSingleNode("/Settings/DefaultSelection") as XmlElement;

            if (elem == null) return null;

            if (String.IsNullOrEmpty(elem.InnerText)) return null;

            return elem.InnerText;

        }

        public static LogLevel GetLogLevel()
        {
            readData();


            if (_doc == null)
            {
                System.Diagnostics.Debug.WriteLine("Did not find settings document!");
                return LogLevel.Basic;
            }

            XmlElement elem = _doc.SelectSingleNode("/Settings/LogLevel") as XmlElement;

            if (elem == null) return LogLevel.Basic;

            if (String.IsNullOrEmpty(elem.InnerText)) return LogLevel.Basic;

            LogLevel level = LogLevel.Basic;
            if (Enum.TryParse<LogLevel>(elem.InnerText, out level))
            {
                return level;
            }

            return LogLevel.Basic;
        }

        public static void ReadTolerance(out double moveTol, out float angleTol)
        {
            readData();

            //defaults...
            moveTol = 0.0006;
            angleTol = 0.0349f;

            if (_doc == null)
            {
                System.Diagnostics.Debug.WriteLine("Did not find settings document!");

                return;
            }

            XmlElement elem = _doc.SelectSingleNode("/Settings/Tolerance") as XmlElement;
            if (elem != null)
            {

                if (Double.TryParse(elem.Attributes["length"].Value, out moveTol) == false)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid Length Tolerance Setting!" + elem.Attributes["length"].Value);
                }
                if (float.TryParse(elem.Attributes["angle"].Value, out angleTol) == false)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid Angle Tolerance Setting!" + elem.Attributes["angle"].Value);
                }
            }
        }

        private static void readData()
        {
            if (_doc != null) return;

            _doc = new XmlDocument();


            string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Settings.xml");

            _doc.Load(filename);


        }
        #endregion //Setings by metamorphosis

    }
    #region Settings json application file
    // grabbed from http://stackoverflow.com/questions/453161/best-practice-to-save-application-settings-in-a-windows-forms-application
    public class AppSettings<T> where T : new()
    {
        private const string _default_filename = "settings.json";

        public void Save(
          string fileName = _default_filename)
        {
            File.WriteAllText(Path.Combine(AppCommand.Path, fileName), (new JavaScriptSerializer()).Serialize(this));
        }

        public static void Save( T pSettings, string fileName = _default_filename)
        {
            string path = Path.Combine(AppCommand.Path, fileName);
            File.WriteAllText(path,(new JavaScriptSerializer()).Serialize(pSettings));
        }

        public static T Load( string fileName = _default_filename)
        {
            string path = Path.Combine(AppCommand.Path, fileName);
            T t = new T();
            if (File.Exists(path))
            {
                t = (new JavaScriptSerializer()).Deserialize<T>( File.ReadAllText(path));
            }
            return t;
        }
    }
    #endregion
}
