using System;
using Eto.Forms;
using Eto.Mac.Forms.Controls;

namespace ProResMetadata.Mac
{
	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
            new Application(Eto.Platforms.Mac64).Run(new MainForm(args));
		}
	}
}
