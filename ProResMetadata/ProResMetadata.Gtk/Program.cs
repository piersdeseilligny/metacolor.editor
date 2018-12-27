using System;
using Eto.Forms;
using Eto.GtkSharp.Forms.Controls;

namespace ProResMetadata.Gtk
{
	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
            new Application(Eto.Platforms.Gtk).Run(new MainForm(args));
		}
	}
}
