using System;
using Eto.Forms;
using Metacolor.Editor.Classes;

namespace Metacolor.Editor.Mac
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            StartupArgs startup = new StartupArgs() { OpenWithFiles = args };
            new Application(Eto.Platforms.Mac64).Run(new MainForm(startup));
        }
    }
}
