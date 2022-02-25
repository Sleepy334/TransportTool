using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PublicTransportInfo
{
    public class Utils
    {
        public static string GetSortCharacter(bool bDesc)
        {
            string sUnicode;
            if (bDesc)
            {
                sUnicode = "\u25BC";
            }
            else
            {
                sUnicode = "\u25B2";
            }
            return GetUnicodeCharacter(sUnicode);
        }

        public static string GetUnicodeCharacter(string sUnicode)
        {
            return Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(sUnicode)));
        }

        public static string GetFileVersion()
        {
            try
            {
                //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                //System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                //return fvi.FileVersion;

                string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return assemblyVersion;
                //string assemblyVersion = Assembly.LoadFile("your assembly file").GetName().Version.ToString();
                //string fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                //string productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            } catch (Exception ex)
            {
                return "TEST";
            }
            
        }
    }
}
