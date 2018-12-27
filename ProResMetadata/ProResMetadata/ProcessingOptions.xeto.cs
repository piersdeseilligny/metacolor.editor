using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using static ProResMetadata.FileManagement;
using System.ComponentModel;

namespace ProResMetadata
{	
	public class ProcessingOptions : Dialog
	{
        private RadioButtonList writeLocation;
        private FilePicker selectFolder;
        private CheckBox createColrAtom;
        private CheckBox overrideColrPrint;
        private CheckBox overrideColrUnknown;
        public ProcessingOptions()
		{
			XamlReader.Load(this);
            Setup();
            writeLocation.SelectedIndexChanged += WriteLocation_SelectedIndexChanged;
		}

        private void WriteLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            //If the save location is set to custom, enable the folder picker
            if (writeLocation.SelectedIndex == 2)
                selectFolder.Enabled = true;
            else
                selectFolder.Enabled = false;
        }

        public void Setup()
        {
            GetSettings();
            switch (settings.WriteLocation)
            {
                case "?override":
                    writeLocation.SelectedIndex = 0;
                    break;
                case "?default":
                    writeLocation.SelectedIndex = 1;
                    break;
                default:
                    writeLocation.SelectedIndex = 2;
                    selectFolder.FilePath = settings.WriteLocation;
                    selectFolder.Enabled = true;
                    break;
            }
            //TODO:
            //createColrAtom.Checked = settings.CreateColrAtom;
            //overrideColrPrint.Checked = settings.OverrideColrPrint;
            //overrideColrUnknown.Checked = settings.OverrideColrUnknown;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            switch (writeLocation.SelectedIndex)
            {
                case 0:
                    settings.WriteLocation = "?override"; break;
                case 1:
                    settings.WriteLocation = "?default"; break;
                case 2:
                    settings.WriteLocation = selectFolder.FilePath; break;
            }
            //TODO:
            //settings.CreateColrAtom = (bool)createColrAtom.Checked;
            //settings.OverrideColrPrint = (bool)overrideColrPrint.Checked;
            //settings.OverrideColrUnknown = (bool)overrideColrUnknown.Checked;
            SaveSettings();
            base.OnClosing(e);
        }
    }
}
