using System;
using System.Diagnostics;
using Eto.Forms;
using Eto.Wpf.Forms.Controls;
using Metacolor.Editor.Classes;
using PresentationTheme.Aero;

namespace Metacolor.Editor.Wpf
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AeroTheme.SetAsCurrentTheme();
            Eto.Style.Add<ButtonHandler>("noborderbutton", handler => {
                handler.Control.BorderThickness = new System.Windows.Thickness(0);
            });
            Eto.Style.Add<PanelHandler>("StatusBar", handler => {
                handler.Control.BorderThickness = new System.Windows.Thickness(0, 1, 0, 0);
                handler.Control.BorderBrush = System.Windows.SystemColors.ControlLightBrush;
                handler.Control.Background = System.Windows.SystemColors.ControlLightLightBrush;
            });

            StartupArgs startup = new StartupArgs() { OpenWithFiles = args };
            try
            {
                //Package identity exists (downloaded from Microsoft Store)
                startup.SettingsDirectory = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            }
            catch(Exception err)
            {
                //No package identity, running in standalone mode
                startup.SettingsDirectory = System.AppContext.BaseDirectory;
            }

            new Application(Eto.Platforms.Wpf).Run(new MainForm(startup));

        }
    }
}
