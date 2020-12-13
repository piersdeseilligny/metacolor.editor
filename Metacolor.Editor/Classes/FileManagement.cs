using Newtonsoft.Json;
using System;
using System.IO;


namespace Metacolor.Editor
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

        public static string appdir = "";
        public static string SettingsLocation()
        {
            return appdir + "settings.json";
        }
        public static void SaveSettings()
        {
            try
            {
                if (!File.Exists(SettingsLocation())) File.Create(SettingsLocation()).Dispose();
                try
                {
                    File.WriteAllText(SettingsLocation(), JsonConvert.SerializeObject(settings));
                    Logger.Log("Saved settings succesfully");
                }
                catch (Exception ex)
                {
                    Eto.Forms.MessageBox.Show("Failed to save the app settings! Is settings.json not being used by another application? Check out the log for more details.", Eto.Forms.MessageBoxType.Error);
                    Logger.Log("Failed to save settings.json: " + ex.Message);
                }
            }
            catch(Exception ex)
            {
                Eto.Forms.MessageBox.Show("Failed to save the app settings! Are you executing from a read-only directory? Check out the log for more details.", Eto.Forms.MessageBoxType.Error);
                Logger.Log("Failed to create settings.json: " + ex.Message);
            }
        }
        public static void GetSettings()
        {
            if (File.Exists(SettingsLocation()))
            {
                try
                {
                    settings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(SettingsLocation()));
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
