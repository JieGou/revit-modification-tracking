using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;

namespace TD.Core
{
    public class FileUtils
    {

        //Define cultureInfo
        public static ResourceManager LangResMan;    // declare Resource manager to access to specific cultureinfo
        public static CultureInfo Cult;            // declare culture info

        public static void GetLocalisationValues()
        {
            //Create the culture for english
            FileUtils.Cult = CultureInfo.CreateSpecificCulture("en");
            FileUtils.LangResMan = new System.Resources.ResourceManager("BIM42.Revit2015.Resources.en", System.Reflection.Assembly.GetExecutingAssembly());
        }

        public static void ExtractRessource(string resourceName, string path)
        {
            using (Stream input = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (Stream output = File.Create(path))
            {

                // Insert null checking here for production
                byte[] buffer = new byte[8192];

                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                }

            }
        }

        public static IntPtr GetMainWindowHandle()
        {

            System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
            return p.MainWindowHandle;
        }

        /// <summary>
        /// Validate that the given path is acceptable
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsValidPath(string path)
        {
            try
            {
                var info = new System.IO.FileInfo(path);

                if (info != null) return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public static bool CanWriteToFolder(string folder)
        {
            try
            {
                string file = System.IO.Path.Combine(folder, "Test" + DateTime.Now.Ticks.ToString() + ".log");
                System.IO.File.WriteAllText(file, "This is a test. Should be deleted.");
                System.Threading.Thread.Sleep(100);
                System.IO.File.Delete(file);
                return true;
            }
            catch
            {

            }
            return false;
        }


    }
}
