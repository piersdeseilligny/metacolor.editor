using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace ProResMetadata
{
    public static class FileManagement
    {
        public class AppSettings
        {
            public bool DetectColrAtom = true;
            public bool DetectFrameHeader = true;
            public bool SkipFrameContent = true;
            public bool CreateColrAtom = false;
            public bool OverrideColrPrint = false;
            public bool OverrideColrUnknown = false;
            public bool OpenFilesInVlc = false;
            public string WriteLocation = "?override";
        }

        public static AppSettings settings = new AppSettings();
        public static void SaveSettings()
        {
            if (!File.Exists("settings.json")) File.Create("settings.json").Dispose();
            try
            {
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
                Logger.Log("Saved settings succesfully");
            }
            catch (Exception ex)
            {
                Eto.Forms.MessageBox.Show("Failed to save the app settings! Is settings.json not being used by another application? Check out the log for more details.", Eto.Forms.MessageBoxType.Error);
                Logger.Log("Failed to save settings: " + ex.Message);
            }
        }
        public static void GetSettings()
        {
            if (File.Exists("settings.json"))
            {
                try
                {
                    settings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("settings.json"));
                    if (settings == null) settings = new AppSettings();
                    Logger.Log("Opened settings succesfully");
                }
                catch
                {
                    settings = new AppSettings();
                    Logger.Log("Failed to open the settings file, resetting AppSettings");
                }
            }
            else
            {
                settings = new AppSettings();
            }
        }
    }
}
