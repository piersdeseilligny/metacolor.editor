using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using static Metacolor.Editor.ProRes;

namespace Metacolor.Editor.Classes
{
    class UICommands
    {
		public class OpenSettings : Command
		{
			MainForm mainform;
			Settings.SettingsTab tab;
			public OpenSettings(MainForm _mainform, Settings.SettingsTab _tab)
			{
				MenuText = "Settings";
				mainform = _mainform;
				tab = _tab;
			}
			protected override void OnExecuted(EventArgs e)
			{
				base.OnExecuted(e);
				var page = new Settings(tab);
				page.Closed += mainform.UpdateStatusBarWarning;
				page.ShowModal(mainform);
			}
		}
		public class OpenAbout : Command
		{
			Form mainform;
			public OpenAbout(MainForm form)
			{
				MenuText = "About";
				mainform = form;
			}

			protected override async void OnExecuted(EventArgs e)
			{
				base.OnExecuted(e);
				await ((new About()).ShowModalAsync(mainform));
			}
		}
		public class ImportFile : Command
		{
			MainForm mainform;
			public ImportFile(MainForm form)
			{
				MenuText = "Import files";
				mainform = form;
			}

			protected override async void OnExecuted(EventArgs e)
			{
				base.OnExecuted(e);
				mainform.AddFiles(mainform, e);
			}
		}
		public class ImportFolder : Command
		{
			MainForm mainform;
			public ImportFolder(MainForm form)
			{
				MenuText = "Import folders";
				mainform = form;
			}

			protected override async void OnExecuted(EventArgs e)
			{
				base.OnExecuted(e);
				mainform.AddFolder(mainform, e);
			}
		}

		public class DeleteSelected : Command
		{
			MainForm mainform;
			public DeleteSelected(MainForm form)
			{
				MenuText = "Remove selected files";
				Shortcut = Keys.Delete;
				mainform = form;
				Enabled = false;
			}

			protected override async void OnExecuted(EventArgs e)
			{
				base.OnExecuted(e);
				mainform.RemoveVideos(mainform.videoGridView.SelectedItems);
			}
		}

		public class ExportJSONSelected : Command
        {
			MainForm mainform;
			public ExportJSONSelected(MainForm form)
            {
				MenuText = "Export JSON for selected files";
				Shortcut = Keys.Control | Keys.E;
				mainform = form;
				Enabled = false;
            }

			protected override async void OnExecuted(EventArgs e)
			{
				base.OnExecuted(e);
				mainform.ExportAsJson_Click(null, null);
			}
		}

		public class ResetModifications : Command
		{
			MainForm mainform;
			public ResetModifications(MainForm form)
			{
				MenuText = "Reset modifications";
				Shortcut = Keys.Control | Keys.R;
				mainform = form;
			}

			protected override async void OnExecuted(EventArgs e)
			{
				base.OnExecuted(e);
				mainform.ResetModifications(null, null);
			}
		}
	}
}
