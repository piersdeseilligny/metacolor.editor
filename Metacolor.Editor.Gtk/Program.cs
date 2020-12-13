using System;
using Eto.Forms;
using Metacolor.Editor.Classes;
using System.Collections.Generic;

namespace Metacolor.Editor.Gtk
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            StartupArgs startup = new StartupArgs() { OpenWithFiles = args };
            new Application(Eto.Platforms.Gtk).Run(new MainForm(startup));
        }
    }
}
