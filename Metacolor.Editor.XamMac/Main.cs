using System;
using System.IO;
using AppKit;
using Eto.Forms;
using Metacolor.Editor;
using Metacolor.Editor.Classes;
using static System.Environment;

namespace Metacolor.Editor.XamMac
{
	static class MainClass
	{
		static void Main (string[] args)
		{
            StartupArgs startup = new StartupArgs() { OpenWithFiles = args };
            // Use DoNotVerify in case LocalApplicationData doesn’t exist.
            string appData = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "metacolor.editor");
            // Ensure the directory and all its parents exist.
            Directory.CreateDirectory(appData);
            startup.SettingsDirectory = appData;
            new Application(Eto.Platforms.XamMac2).Run(new MainForm(startup));
        }
	}
}
