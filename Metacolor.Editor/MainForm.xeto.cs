using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using static Metacolor.Editor.ProRes;
using static Metacolor.Editor.Toolkit;
using System.IO;
using Metacolor.Editor.Classes;
using Eto;

namespace Metacolor.Editor
{
    public static class Logger
    {
        public static event EventHandler<string> Logged;
        public static void Log(string content)
        {
            Logged?.Invoke(null, content);
        }
    }

    public class MainForm : Form
    {
        #region Declarations
        TableLayout rootLayout;
        TableLayout fakeToolbar;
        PixelLayout statusBarLayout;
        Panel statusBarLabels;
        ProgressBar statusBarProgress;

        Label statusLabel;
        Label warningLabel;
        TabPage errorTab;
        TextArea logArea;
        Splitter splitter;
        DropDown ColorPrimaryDropDown;
        DropDown TransferFunctionDropDown;
        DropDown ColorMatrixDropDown;
        DropDown PixelFormatDropDown;
        DropDown CreatorIdDropDown;
        GridView errorList;
        public GridView videoGridView;
        TextBox customCreatorField;
        Button processButton;
        Label detailsColorAtomMissing;
        Label detailsFrameMissing;
        StackLayout detailsColorAtomContent;
        StackLayout detailsColorAtomLabels;
        StackLayout detailsFrameContent;
        StackLayout detailsFrameLabels;

        Button duplicateModifications;
        Button resetModifications;

        Label detailsFilePath;
        Label detailsFrameCount;

        Label detailsColorAtomPrimary;
        Label detailsColorAtomMatrix;
        Label detailsColorAtomTransfer;
        Label detailsColorAtomOffset;
        Label detailsColorAtomParameter;

        Label detailsFramePrimary;
        Label detailsFrameTransfer;
        Label detailsFrameMatrix;
        Label detailsFrameCreator;
        Label detailsFrameResolution;
        Label detailsFrameChrominanceFactor;
        Label detailsFrameType;
        Label detailsFramePixelFormat;
        Label detailsFrameAlphaInfo;
        LinkButton detailsFrameLumaQuant;
        LinkButton detailsFrameChromaQuant;
        Scrollable detailsScroller;
        ImageView testimage;

        Bitmap errorBitmapNone = Bitmap.FromResource("Metacolor.Editor.Assets.Ok.png", assembly: null);
        Bitmap errorBitmapCritical = Bitmap.FromResource("Metacolor.Editor.Assets.Error.png", assembly: null);
        Bitmap errorBitmapWarning = Bitmap.FromResource("Metacolor.Editor.Assets.Warning.png", assembly: null);
        Bitmap errorBitmapInterrogation = Bitmap.FromResource("Metacolor.Editor.Assets.Interrogation.png", assembly: null);
        Bitmap errorBitmapInformation = Bitmap.FromResource("Metacolor.Editor.Assets.Information.png", assembly: null);

        Command deleteVideos;
        Command exportjsonVideos;

        /// <summary>
        /// Get an icon corresponding to the specified error gravity
        /// </summary>
        /// <param name="gravity"></param>
        /// <returns></returns>
        public Bitmap GetErrorBitmap(ProRes.DecodeError.ErrorGravity gravity)
        {
            switch (gravity)
            {
                case DecodeError.ErrorGravity.None:
                    return errorBitmapNone;
                case DecodeError.ErrorGravity.Critical:
                    return errorBitmapCritical;
                case DecodeError.ErrorGravity.IncompatibleFile:
                    return errorBitmapInterrogation;
                case DecodeError.ErrorGravity.Informational:
                    return errorBitmapInformation;
                default:
                    return errorBitmapWarning;
            }
        }
        #endregion

        public MainForm(StartupArgs args)
        {
            FileManagement.appdir = args.SettingsDirectory;
            if (!args.SettingsDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                FileManagement.appdir += Path.DirectorySeparatorChar.ToString();
            
            

            //Custom cross-platform styles:
            #region Styles
            Eto.Style.Add<Label>("header3", label =>
            {
              //  float fontsize = 12.0f;
              //  if (EtoEnvironment.Platform.IsMac) fontsize = 16.0f;
                label.Font = new Eto.Drawing.Font(label.Font.Family, label.Font.Size*1.35f, label.Font.FontStyle);
            });
            Eto.Style.Add<Label>("hintlabel", label =>
            {
                label.TextColor = new Color(0, 0, 0, 0.6f);
            });
            Eto.Style.Add<Label>("iconscredit", label =>
            {
                label.TextColor = new Color(0, 0, 0, 0.6f);
                label.Font = new Eto.Drawing.Font(label.Font.Family, label.Font.Size*0.6f, Eto.Drawing.FontStyle.None);
            });
            Eto.Style.Add<LinkButton>("iconscredit", label =>
            {
                label.TextColor = new Color(0, 0, 0, 0.6f);
                label.Font = new Eto.Drawing.Font(label.Font.Family, label.Font.Size * 0.6f, Eto.Drawing.FontStyle.None);
            });
            Eto.Style.Add<Label>("DetailsLabel", label =>
            {
                label.TextColor = new Color(0, 0, 0, 0.6f);
            });
            Eto.Style.Add<Label>("DetailsContent", label =>
            {
                label.TextColor = new Color(0, 0, 0, 1);
            });
            Eto.Style.Add<Label>("MatrixContent", area =>
            {
                area.Font = new Eto.Drawing.Font("Courier New", 10.0f);
            });

            Style = "mainform";

            #endregion

            //Load the UI, hook up the basic events:
            XamlReader.Load(this);
            ProRes.AnalysisProgress += ProRes_AnalysisProgress;
            Logger.Logged += Logger_Logged;

            //Set up the menubarand toolbar:
            #region MenuBar+Toolbar

            deleteVideos = new UICommands.DeleteSelected(this);
            exportjsonVideos = new UICommands.ExportJSONSelected(this);
            Menu = new MenuBar
            {
                Items =
                {
                    new ButtonMenuItem
                    {
                        Text = "&File",
                        Items =
                        { 
							// you can add commands or menu items
							new UICommands.ImportFile(this),
                            new UICommands.ImportFolder(this),
                            deleteVideos,
                            exportjsonVideos
                        }
                    },
                    new ButtonMenuItem
                    {
                        Text = "&Edit",
                        Items =
                        { 
							// you can add commands or menu items
                            new UICommands.ResetModifications(this)
                        }
                    }
                },
                // quit item (goes in Application menu on OS X, File menu for others)
                QuitItem = new Command((sender, e) => Application.Instance.Quit())
                {
                    MenuText = "Quit",
                    Shortcut = Application.Instance.CommonModifier | Keys.Q
                },
                // about command (goes in Application menu on OS X, Help menu for others)
                AboutItem = new UICommands.OpenAbout(this)
            };
            Menu.ApplicationMenu.Items.Add(new UICommands.OpenSettings(this, Settings.SettingsTab.General));

            if (!EtoEnvironment.Platform.IsWindows)
            {
                fakeToolbar.Visible = false;
                var importFile = new ButtonToolItem() { Text = "Add Files", Command = new UICommands.ImportFile(this), Image = Bitmap.FromResource("Metacolor.Editor.Assets.file.png").TemplateImage() };
                var importFolder = new ButtonToolItem() { Text = "Add Folders", Command = new UICommands.ImportFolder(this), Image = Bitmap.FromResource("Metacolor.Editor.Assets.folder.png").TemplateImage() };
                var openSettings = new ButtonToolItem() { Text = "Settings", Command = new UICommands.OpenSettings(this, Settings.SettingsTab.General), Image = Bitmap.FromResource("Metacolor.Editor.Assets.settings.png").TemplateImage() };
                var openAbout = new ButtonToolItem() { Text = "About", Command = new UICommands.OpenAbout(this), Image = Bitmap.FromResource("Metacolor.Editor.Assets.about.png").TemplateImage() };


                ToolBar = new ToolBar()
                {
                    Items =
                    {
                        importFile,
                        importFolder,
                        new SeparatorToolItem() { Type= SeparatorToolItemType.FlexibleSpace },
                        openSettings,
                        openAbout
                        
                    },
                    Style = "macostoolbar"
                };
            }

            AllowDrop = true;
            DragDrop += MainForm_DragDrop;

            #endregion

            #region Status Bar
            if(EtoEnvironment.Platform.IsMac) statusBarLayout.Add(statusBarLabels, 12, 5);
            else statusBarLayout.Add(statusBarLabels, 12, 4);
            statusBarLayout.Add(statusBarProgress, 0, 20);
            UpdateStatusBarLayout();
            #endregion

            //Load the app settings:
            #region Load Settings
            FileManagement.GetSettings();
            if (FileManagement.settings.WriteLocation == "?override")
                warningLabel.Text = "File override enabled!";
            else
                warningLabel.Text = "";
            #endregion

            //Finish setting up the UI:
            #region Basic UI setting up
            //Fill the modification dropdowns with relevant data:
            ColorPrimaryDropDown.Items.Add(new ListItem() { Key = "-1", Text = "Don't modify" });
            TransferFunctionDropDown.Items.Add(new ListItem() { Key = "-1", Text = "Don't modify" });
            ColorMatrixDropDown.Items.Add(new ListItem() { Key = "-1", Text = "Don't modify" });
            PixelFormatDropDown.Items.Add(new ListItem() { Key = "-1", Text = "Don't modify" });
            CreatorIdDropDown.Items.Add(new ListItem() { Key = "-1", Text = "Don't modify" });
            foreach (var colorprimary in ProRes.ColorPrimaries)
                ColorPrimaryDropDown.Items.Add(new ListItem() { Key = colorprimary.Key.ToString(), Text = colorprimary.Value });
            foreach (var transferfunction in ProRes.TransferFunctions)
                TransferFunctionDropDown.Items.Add(new ListItem() { Key = transferfunction.Key.ToString(), Text = transferfunction.Value });
            foreach (var colormatrix in ProRes.ColorMatrixes)
                ColorMatrixDropDown.Items.Add(new ListItem() { Key = colormatrix.Key.ToString(), Text = colormatrix.Value });
            foreach (var pixelformat in ProRes.SourcePixelFormats)
                PixelFormatDropDown.Items.Add(new ListItem() { Key = pixelformat.Key.ToString(), Text = pixelformat.Value });
            foreach (var fourcc in ProRes.CreatorIDs)
                CreatorIdDropDown.Items.Add(new ListItem() { Key = fourcc.Key.ToString(), Text = fourcc.Value });
            ColorPrimaryDropDown.SelectedIndex = 0;
            TransferFunctionDropDown.SelectedIndex = 0;
            ColorMatrixDropDown.SelectedIndex = 0;
            PixelFormatDropDown.SelectedIndex = 0;
            CreatorIdDropDown.SelectedIndex = 0;



            //Set up the error list gridview:
            var errorImage = Bitmap.FromResource("Metacolor.Editor.Assets.Error.png", assembly: null);
            var warningImage = Bitmap.FromResource("Metacolor.Editor.Assets.Error.png", assembly: null);
            errorList.Columns.Add(new GridColumn
            {
                DataCell = new ImageViewCell { Binding = Binding.Property<ProRes.DecodeError, Image>(r => GetErrorBitmap(r.Gravity)) },
                Width = 12,
                Sortable = false,
                Resizable = false
            });
            errorList.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<ProRes.DecodeError, string>(r => r.FileName) },
                HeaderText = "File",
            });
            errorList.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<ProRes.DecodeError, string>(r => r.Title) },
                HeaderText = "Error"
            });
            errorList.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<ProRes.DecodeError, string>(r => r.Details) },
                HeaderText = "Description"
            });
            errorList.DataStore = errorCollection;

            //Set up the main file list gridview:
            videoGridView.DeleteItemHandler = new Func<object, bool>((object obj) =>
            {
                Debug.WriteLine("Deleted");
                return videoCollection.Remove((VideoElement)obj);
            });
            videoGridView.Columns.Add(new GridColumn
            {
                DataCell = new ImageViewCell { Binding = Binding.Property<VideoElement, Image>(r => GetErrorBitmap(r.WorstError)) },
                Width = 12,
                Sortable = false,
                Resizable = false,
            });
            videoGridView.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<VideoElement, string>(r => r.FileName) },
                HeaderText = "File name",
            });
            videoGridView.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<VideoElement, string>(r => r.FrameCount.ToString()) },
                HeaderText = "Frames",
            });
            videoGridView.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<VideoElement, string>(r => r.DisplayColorPrimary.ToString()) },
                HeaderText = "Color primary",
            });
            videoGridView.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<VideoElement, string>(r => r.DisplayTransferFunction.ToString()) },
                HeaderText = "Transfer function"
            });
            videoGridView.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<VideoElement, string>(r => r.DisplayColorMatrix.ToString()) },
                HeaderText = "Color matrix"
            });
            videoGridView.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<VideoElement, string>(r => r.DisplayCreatorId.ToString()) },
                HeaderText = "Creator ID"
            });
            videoGridView.DataStore = videoCollection;
            videoGridView.CellClick += VideoGridView_CellClick;
            videoGridView.SelectedItemsChanged += VideoGridView_SelectedItemsChanged;


            //resetModifications.Image = Bitmap.FromResource("Metacolor.Editor.Assets.reset.png").TemplateImage();
            // duplicateModifications.Image = Bitmap.FromResource("Metacolor.Editor.Assets.duplicate.png");
            #endregion

            //Import the files included in the args
            ProcessFiles(args.OpenWithFiles.ToList());
            SizeChanged += MainForm_SizeChanged;
            LoadComplete += MainForm_LoadComplete;

        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            // throw new NotImplementedException();
            Console.Write("DRAG DROP");
        }

        private void VideoGridView_SelectedItemsChanged(object sender, EventArgs e)
        {
            if (videoGridView.SelectedItems.Count() > 0)
            {
                deleteVideos.Enabled = true;
                exportjsonVideos.Enabled = true;
            }
            else
            {
                deleteVideos.Enabled = false;
                exportjsonVideos.Enabled = false;
            }
        }

        private void ProRes_AnalysisProgress(object sender, float e)
        {
            Application.Instance.Invoke(() =>
            {
                UpdateStatusBarProgress(Convert.ToInt32((finishedfiles * 100) + (e * 100)));
            });
        }

        #region Right-click options
        private void VideoGridView_CellClick(object sender, GridCellMouseEventArgs e)
        {
            if (e.Row != -1 && e.Buttons == MouseButtons.Alternate) //Check if a row is selected, and if it was right-clicked on
            {
                if (!videoGridView.SelectedRows.Contains(e.Row))
                {
                    //If the selected rows don't contain the one that was clicked on, de-select them
                    videoGridView.SelectedRow = e.Row;
                }

                ContextMenu menu = new ContextMenu();
                if (videoGridView.SelectedRows.Count() == 1)
                {
                    //Add the "Open file" option if a single item was selected
                    ButtonMenuItem openFile = new ButtonMenuItem() { Text = "Open file" };
                    openFile.Click += OpenFile_Click;
                    menu.Items.Add(openFile);
                }

                //Add the "Show In Explorer" button:
                ButtonMenuItem showInExplorer = new ButtonMenuItem();
                if (Eto.Platform.Instance.IsMac)
                    showInExplorer.Text = "Show in Finder";
                else
                    showInExplorer.Text = "Show in File Explorer";
                showInExplorer.Click += ShowInExplorer_Click;
                menu.Items.Add(showInExplorer);

                //Add the "Export as JSON" button:
                ButtonMenuItem exportAsJson = new ButtonMenuItem()
                {
                    Text = "Export as JSON"
                };
                exportAsJson.Click += ExportAsJson_Click;
                menu.Items.Add(exportAsJson);

                //Add the re-analysis options:
                ButtonMenuItem fullAnalysis = new ButtonMenuItem()
                {
                    Text = "Slow analysis",
                    ToolTip = "Perform an analysis with all optimizations disabled"
                };
                fullAnalysis.Click += FullAnalysis_Click;
                ButtonMenuItem defaultAnalysis = new ButtonMenuItem()
                {
                    Text = "Default analysis",
                    ToolTip = "Perform an analysis with the default settings"
                };
                defaultAnalysis.Click += DefaultAnalysis_Click;
                ButtonMenuItem reAnalyse = new ButtonMenuItem()
                {
                    Text = "Re-Analyse",
                    Items =
                    {
                       defaultAnalysis,
                       fullAnalysis
                    }
                };
                menu.Items.Add(reAnalyse);

                menu.Show(videoGridView);
            }
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            if (videoGridView.SelectedItem != null)
            {
                if (FileManagement.settings.OpenFilesInVlc)
                    Application.Instance.Open("vlc://" + ((VideoElement)videoGridView.SelectedItem).FilePath);
                else
                    Application.Instance.Open(((VideoElement)videoGridView.SelectedItem).FilePath);
            }
        }

        /// <summary>
        /// Open the folder containing each selected item with the OS's file explorer
        /// </summary>
        private void ShowInExplorer_Click(object sender, EventArgs e)
        {
            HashSet<string> Directories = new HashSet<string>();
            foreach (var row in videoGridView.SelectedRows)
            {
                var dir = new FileInfo((videoCollection[row]).FilePath).DirectoryName;
                if (!Directories.Contains(dir))
                    Directories.Add(dir);
            }
            foreach (var dir in Directories)
                Application.Instance.Open(dir);
        }

        /// <summary>
        /// Open a dialog to save the selected video elements as JSON files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExportAsJson_Click(object sender, EventArgs e)
        {
            if (videoGridView.SelectedRows.Count() > 1)
            {
                //Get the directory in which the majority of the files are, and set it as the default one for the save dialog
                List<string> Directories = new List<string>();
                foreach (var row in videoGridView.SelectedRows)
                {
                    var dir = new FileInfo((videoCollection[row]).FilePath).DirectoryName;
                    Directories.Add(dir);
                }
                string MostCommonDir = Directories.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
                SelectFolderDialog selectFolder = new SelectFolderDialog();
                selectFolder.Directory = MostCommonDir;
                if (selectFolder.ShowDialog(this) == DialogResult.Ok)
                {
                    foreach (var row in videoGridView.SelectedRows)
                    {
                        //Create json for each selected video:
                        var dir = new FileInfo((videoCollection[row]).FilePath);
                        var stream = new FileInfo(selectFolder.Directory + Path.DirectorySeparatorChar + videoCollection[row].FileName + ".json").CreateText();
                        stream.Write(Newtonsoft.Json.JsonConvert.SerializeObject(videoCollection[row]));
                        stream.Dispose();
                    }
                }
            }
            else
            {
                //A single file is selected, open the save file dialog
                var currentfile = new FileInfo(videoCollection[videoGridView.SelectedRows.First()].FilePath);
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = currentfile.Name + ".json";
                if (savefile.ShowDialog(this) == DialogResult.Ok)
                {
                    var stream = new FileInfo(savefile.FileName).CreateText();
                    stream.Write(Newtonsoft.Json.JsonConvert.SerializeObject(videoCollection[videoGridView.SelectedRows.First()]));
                    stream.Dispose();
                }
            }

        }

        /// <summary>
        /// Re-analyse the selected files with default optimizations enabled
        /// </summary>
        private void DefaultAnalysis_Click(object sender, EventArgs e)
        {
            List<VideoElement> videos = new List<VideoElement>();
            foreach (VideoElement video in videoGridView.SelectedItems)
                videos.Add(video);
            RemoveVideos(videos);
            ProcessFiles(videos.Select(x => x.FilePath).ToList());
        }

        /// <summary>
        /// Re-analyse the selected files without any optimizations
        /// </summary>
        private void FullAnalysis_Click(object sender, EventArgs e)
        {
            List<VideoElement> videos = new List<VideoElement>();
            foreach (VideoElement video in videoGridView.SelectedItems)
                videos.Add(video);
            RemoveVideos(videos);
            ProcessFiles(videos.Select(x => x.FilePath).ToList(), false);
        }

        #endregion

        #region File details pane
        public void SelectedRowsChanged(object sender, EventArgs e)
        {
            int withAtom = 0; //how many of the selected items have a QuickTime atom
            int withFrame = 0; //how many of the selected items have at least one prores frame

            string globalFilepath = null;
            string globalFramecount = null;

            string atomPrimary = null;
            string atomTransfer = null;
            string atomMatrix = null;
            string atomOffset = null;
            string atomParameter = null;

            string framePrimary = null;
            string frameTransfer = null;
            string frameMatrix = null;
            string frameCreator = null;
            string framePixelFormat = null;
            string frameAlphaInfo = null;
            string frameChrominance = null;
            string frameType = null;
            string frameResolution = null;

            string frameLumaQuantsHash = null;
            string frameChromaQuantsHash = null;
            bool frameLumaQuantIsNull = false;
            bool frameChromaQuantIsNull = false;

            bool generalFirstIteration = true; //Is this the first time we're looping over the general properties?
            bool frameFirstIteration = true; //Is this the first time we're looping over the frame properties?
            bool atomFirstIteration = true; //Is this the first time we're looping over the quicktime atom properties?

            if (videoGridView.SelectedRow != -1) //Check that at least one item is selected
            {
                processButton.Enabled = true;
                detailsScroller.Visible = true;
                int selectedrowcount = videoGridView.SelectedRows.Count();
                foreach (VideoElement vid in videoGridView.SelectedItems)
                {
                    detailsFilePath.UpdateTextForMultiple(ref globalFilepath, vid.FilePath, generalFirstIteration);
                    detailsFrameCount.UpdateTextForMultiple(ref globalFramecount, vid.FrameCount.ToString(), generalFirstIteration);

                    if (vid.ColorAtom != null)
                    {
                        withAtom++;

                        detailsColorAtomPrimary.UpdateTextForMultiple(ref atomPrimary, ColorPrimaries.GetValue(vid.ColorAtom.PrimariesIndex), atomFirstIteration);
                        detailsColorAtomTransfer.UpdateTextForMultiple(ref atomTransfer, TransferFunctions.GetValue(vid.ColorAtom.TransferFunction), atomFirstIteration);
                        detailsColorAtomMatrix.UpdateTextForMultiple(ref atomMatrix, ColorMatrixes.GetValue(vid.ColorAtom.ColorMatrix), atomFirstIteration);

                        detailsColorAtomOffset.UpdateTextForMultiple(ref atomOffset, vid.ColorAtom.Offset.ToString(), atomFirstIteration);
                        detailsColorAtomParameter.UpdateTextForMultiple(ref atomParameter, vid.ColorAtom.Type.ToString() + " [" + vid.ColorAtom.RawColorParam + "]", atomFirstIteration);
                        atomFirstIteration = false;
                    }
                    if (vid.Frames.Count > 0)
                    {
                        withFrame++;

                        detailsFramePrimary.UpdateTextForMultiple(ref framePrimary, vid.DisplayColorPrimary, frameFirstIteration);
                        detailsFrameTransfer.UpdateTextForMultiple(ref frameTransfer, vid.DisplayTransferFunction, frameFirstIteration);
                        detailsFrameMatrix.UpdateTextForMultiple(ref frameMatrix, vid.DisplayColorMatrix, frameFirstIteration);

                        detailsFrameCreator.UpdateTextForMultiple(ref frameCreator, vid.DisplayCreatorId, frameFirstIteration);
                        detailsFramePixelFormat.UpdateTextForMultiple(ref framePixelFormat, vid.DisplayPixelFormat, frameFirstIteration);
                        detailsFrameAlphaInfo.UpdateTextForMultiple(ref frameAlphaInfo, vid.DisplayAlphaInfo, frameFirstIteration);
                        detailsFrameChrominanceFactor.UpdateTextForMultiple(ref frameChrominance, vid.DisplayChrominanceFactor, frameFirstIteration);
                        detailsFrameType.UpdateTextForMultiple(ref frameType, vid.DisplayPictureType, frameFirstIteration);
                        detailsFrameResolution.UpdateTextForMultiple(ref frameResolution, vid.DisplayResolution, frameFirstIteration);

                        //Compare the hash of the custom quantization matrixes, to compare them with the others in the selection
                        //TODO This is buggy, must be fixed
                        CheckMultiple(ref frameLumaQuantsHash, vid.LumaQuants.Keys.GetHashCode().ToString(), frameFirstIteration);
                        CheckMultiple(ref frameChromaQuantsHash, vid.ChromaQuants.Keys.GetHashCode().ToString(), frameFirstIteration);
                        if (frameFirstIteration)
                        {
                            var firstLumaQuant = vid.LumaQuants.Keys.First();
                            if (firstLumaQuant.Length == 1 && firstLumaQuant[0] == 0x00)
                                frameLumaQuantIsNull = true;
                            var firstChromaQuant = vid.ChromaQuants.Keys.First();
                            if (firstChromaQuant.Length == 1 && firstChromaQuant[0] == 0x00)
                                frameChromaQuantIsNull = true;
                        }
                        frameFirstIteration = false;
                    }
                    generalFirstIteration = false;
                }

                if (frameLumaQuantsHash == MultipleSelectionString)
                {
                    //There are multiple luma quantization matrixes, disable clicking
                    detailsFrameLumaQuant.Enabled = false;
                    detailsFrameLumaQuant.Text = MultipleSelectionString;
                }
                else
                {
                    if (frameLumaQuantIsNull)
                    {
                        detailsFrameLumaQuant.Enabled = false;
                        detailsFrameLumaQuant.Text = "Not specified";
                    }
                    else
                    {
                        detailsFrameLumaQuant.Enabled = true;
                        detailsFrameLumaQuant.Text = "Show";
                    }
                }

                if (frameChromaQuantsHash == MultipleSelectionString)
                {
                    //There are multiple chroma quantization matrixes, disable clicking
                    detailsFrameChromaQuant.Enabled = false;
                    detailsFrameChromaQuant.Text = MultipleSelectionString;
                }
                else
                {
                    if (frameChromaQuantIsNull)
                    {
                        detailsFrameChromaQuant.Enabled = false;
                        detailsFrameChromaQuant.Text = "Not specified";
                    }
                    else
                    {
                        detailsFrameChromaQuant.Enabled = true;
                        detailsFrameChromaQuant.Text = "Show";
                    }
                }
                if (withAtom == 0)
                {
                    //Nothing in the selection has a qt color atom
                    detailsColorAtomLabels.Visible = false;
                    detailsColorAtomContent.Visible = false;
                    detailsColorAtomMissing.Visible = true;
                    detailsColorAtomMissing.Text = "Missing QuickTime Color atom";
                }
                else if (withAtom != selectedrowcount)
                {
                    //Some files have a qt color atom, but not all those in the selection
                    detailsColorAtomLabels.Visible = true;
                    detailsColorAtomContent.Visible = true;
                    detailsColorAtomMissing.Visible = true;
                    detailsColorAtomMissing.Text = "Some of the files selected are missing a QuickTime Color atom";
                }
                else
                {
                    //All selected files have a qt color atom
                    detailsColorAtomLabels.Visible = true;
                    detailsColorAtomContent.Visible = true;
                    detailsColorAtomMissing.Visible = false;
                }
                if (withFrame == 0)
                {
                    detailsFrameLabels.Visible = false;
                    detailsFrameContent.Visible = false;
                    detailsFrameMissing.Visible = true;
                    detailsFrameMissing.Text = "No Prores frames";
                }
                else if (withFrame != selectedrowcount)
                {
                    detailsFrameLabels.Visible = true;
                    detailsFrameContent.Visible = true;
                    detailsFrameMissing.Visible = true;
                    detailsFrameMissing.Text = "Some of the files selected don't have any frames";
                }
                else
                {
                    detailsFrameLabels.Visible = true;
                    detailsFrameContent.Visible = true;
                    detailsFrameMissing.Visible = false;
                }
            }
            else
            {
                processButton.Enabled = false;
                //No files are selected, hide the content of the details pane
                detailsScroller.Visible = false;
            }
        }
        #endregion

        #region Basic UI interaction

        private async void ClickSettings(object sender, EventArgs e)
        {
            new UICommands.OpenSettings(this, Settings.SettingsTab.General).Execute();
        }
        private async void ClickAbout(object sender, EventArgs e)
        {
            new UICommands.OpenAbout(this).Execute();
        }
        private async void ClickSponsor(object sender, EventArgs e)
        {
            Application.Instance.Open("https://github.com/sponsors/piersdeseilligny");
        }

        private async void OpenProcessingOptions(object sender, EventArgs e)
        {
            new UICommands.OpenSettings(this, Settings.SettingsTab.Output).Execute();
        }
        public async void UpdateStatusBarWarning(object sender, EventArgs e)
        {
            if (FileManagement.settings.WriteLocation == "?override")
                warningLabel.Text = "File override enabled!";
            else
                warningLabel.Text = "";
        }
        protected void HandleQuit(object sender, EventArgs e)
        {
            Application.Instance.Quit();
        }

        public void AddFiles(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.MultiSelect = true;
            if (openfile.ShowDialog(this) == DialogResult.Ok)
            {
                ProcessFiles(openfile.Filenames.ToList());
            }
        }
        public void AddFolder(object sender, EventArgs e)
        {
            SelectFolderDialog openfolder = new SelectFolderDialog();
            if (openfolder.ShowDialog(this) == DialogResult.Ok)
            {
                ProcessFolders(new List<string>() { openfolder.Directory });
            }
        }

        public void CreatorSelectionChanged(object sender, EventArgs e)
        {
            if (((DropDown)sender).SelectedKey == "custom")
            {
                customCreatorField.Visible = true;
            }
            else
            {
                customCreatorField.Visible = false;
            }
        }
        #endregion


        #region Logging
        private void Logger_Logged(object sender, string e)
        {
            Application.Instance.Invoke(() =>
            {
                Log(e);
            });
        }
        public void Log(string text)
        {
            logArea.Text += "[" + DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff") + "]: " + text + "\n";
        }
        #endregion

        #region GridView events
        public void PressedKeyOnGrid(object sender, KeyEventArgs args)
        {
            /*if (args.Key == Keys.Delete || args.Key == Keys.Backspace && videoGridView.SelectedItems.Count() > 0)
            {
                RemoveVideos(videoGridView.SelectedItems);
            }*/
        }

        public void DragEnterFile(object sender, DragEventArgs e)
        {
            e.Effects = DragEffects.Link;
        }

        public void DropFile(object sender, DragEventArgs e)
        {
            e.Effects = DragEffects.Link;
            List<string> filepaths = new List<string>();
            foreach (var uri in e.Data.Uris)
            {
                if (new FileInfo(uri.OriginalString).Attributes.HasFlag(FileAttributes.Directory))
                {
                    var dir = new DirectoryInfo(uri.OriginalString);
                    dir.GetFiles();
                    foreach (var file in dir.GetFiles())
                        filepaths.Add(file.FullName);
                }
                else
                {
                    filepaths.Add(uri.OriginalString);
                }
            }
            ProcessFiles(filepaths);
        }

        #endregion

        /// <summary>
        /// Remove all specified videos from the gridview list, as well as their corresponding errors in the Error list
        /// </summary>
        /// <param name="videos">A list of VideoElements</param>
        public void RemoveVideos(IEnumerable<object> videos)
        {
            List<DecodeError> toRemoveErrors = new List<DecodeError>();
            List<VideoElement> toRemoveVideos = new List<VideoElement>();
            foreach (VideoElement video in videos)
            {
                toRemoveVideos.Add(video);
                foreach (var error in errorCollection)
                    if (error.FilePath == video.FilePath)
                        toRemoveErrors.Add(error);
            }
            foreach (var error in toRemoveErrors)
                errorCollection.Remove(error);
            foreach (var video in toRemoveVideos)
                videoCollection.Remove(video);
            errorTab.Text = "Errors (" + errorCollection.Count + ")";
        }


        //Observable collections binded to the GridViews:
        ObservableCollection<VideoElement> videoCollection = new ObservableCollection<VideoElement>();
        ObservableCollection<DecodeError> errorCollection = new ObservableCollection<DecodeError>();

        //Used for updating the ProgressBar from the ProRes_AnalysisUpdate event:
        int filecount = 0;
        int finishedfiles = 0;

        /// <summary>
        /// Analyse the specified files and add them to the file list
        /// </summary>
        /// <param name="filepaths">The path of each file to analyse</param>
        /// <param name="optimizations">Should the default optimizations be used?</param>
        public async void ProcessFiles(List<string> filepaths, bool optimizations = true)
        {
            string oldStatus = statusLabel.Text;
            filecount = filepaths.Count;
            if (filepaths.Count > 0)
            {
                statusBarProgress.MaxValue = filepaths.Count * 100;
                for (var i = 0; i < filepaths.Count; i++)
                {
                    bool broke = false;
                    int rowPos = 0;
                    foreach (VideoElement existing in videoCollection)
                    {
                        if (existing.FilePath == filepaths[i])
                        {
                            videoGridView.SelectedRow = rowPos;
                            finishedfiles++;
                            broke = true;
                            break;
                        }
                        rowPos++;
                    }
                    if (broke) continue; //The file is already in the file list, ignore it

                    string filename = filepaths[i].Substring(filepaths[i].LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    string logtext = statusLabel.Text = "Analysing file " + (i + 1) + "/" + filepaths.Count + " (" + filename + ")";
                    VideoElement video = null;
                    await Task.Run(() =>
                    {
                        video = ProRes.Decode(filepaths[i], optimizations);
                        video.FileName = filename;
                        video.FilePath = filepaths[i];
                    });
                    foreach (var error in video.Errors)
                    {
                        error.FileName = filename;
                        error.FilePath = filepaths[i];
                        errorCollection.Add(error);
                        errorTab.Text = "Errors (" + errorCollection.Count + ")";
                    }
                    videoCollection.Add(video);
                    finishedfiles++;
                }
            }
            UpdateStatusBarProgress(0);
            finishedfiles = 0;
            statusLabel.Text = oldStatus;
        }

        /// <summary>
        /// Analyse the specified folders
        /// </summary>
        /// <param name="folders">A list with the path of each folder</param>
        public void ProcessFolders(List<string> folders)
        {
            List<string> files = new List<string>();
            foreach (var folder in folders)
            {
                var dir = new DirectoryInfo(folder);
                dir.GetFiles();
                foreach (var file in dir.GetFiles())
                    files.Add(file.FullName);
            }
            ProcessFiles(files);
        }

        /// <summary>
        /// Modify the metadata of the selected files with the specified parameters
        /// </summary>
        public async void Encode(object sender, EventArgs e)
        {
            string oldStatus = statusLabel.Text;
            int i = 0;

            statusBarProgress.MaxValue = videoGridView.SelectedItems.Count();
            UpdateStatusBarProgress(0);
            List<object> selecteditems = videoGridView.SelectedItems.ToList();
            foreach (VideoElement selecteditem in selecteditems)
            {
                statusLabel.Text = "Processing file " + (i + 1) + "/" + videoGridView.SelectedItems.Count().ToString() + " (" + selecteditem.FileName + ")";
                Logger.Log(statusLabel.Text);
                string creatorid = CreatorIdDropDown.SelectedKey;
                if (creatorid == "-1") creatorid = null;
                else if (creatorid == "custom") creatorid = customCreatorField.Text;
                List<DecodeError> errors = new List<DecodeError>();
                errors = await ProRes.Encode(selecteditem, Convert.ToInt32(ColorPrimaryDropDown.SelectedKey), Convert.ToInt32(TransferFunctionDropDown.SelectedKey), Convert.ToInt32(ColorMatrixDropDown.SelectedKey), creatorid);
                foreach (var error in errors)
                {
                    error.FileName = selecteditem.FileName;
                    error.FilePath = selecteditem.FilePath;
                    errorCollection.Add(error);
                    errorTab.Text = "Errors (" + errorCollection.Count + ")";
                    Logger.Log("Error processing file: " + error);
                }
                i++;
                UpdateStatusBarProgress(i);
                RemoveVideos(new VideoElement[] { selecteditem });
                Logger.Log("Finished processing " + selecteditem.FileName);
            }
            UpdateStatusBarProgress(0);
            statusLabel.Text = oldStatus;
        }

        public void ResetModifications(object sender, EventArgs e)
        {
            TransferFunctionDropDown.SelectedIndex = 0;
            ColorPrimaryDropDown.SelectedIndex = 0;
            ColorMatrixDropDown.SelectedIndex = 0;
            CreatorIdDropDown.SelectedIndex = 0;
        }

        public void UpdateStatusBarLayout()
        {
            if (rootLayout.Width > 0)
            {
                statusBarLabels.Width = rootLayout.Width -24;
                statusBarProgress.Width = rootLayout.Width;
                statusBarLabels.Height = 24;
                statusBarProgress.Height = 4;
            }
        }

        public void UpdateStatusBarProgress(int progress)
        {
            if (progress > 0)
            {
                statusBarProgress.Visible = true;
                statusBarProgress.Value = progress;
                float taskbarvalue = progress / Convert.ToSingle(statusBarProgress.MaxValue);
                Debug.WriteLine(taskbarvalue);
                Taskbar.SetProgress(TaskbarProgressState.Progress, taskbarvalue);
            }
            else
            {
                statusBarProgress.Visible = false;
                Taskbar.SetProgress(TaskbarProgressState.None, 0);
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            UpdateStatusBarLayout();
        }
        private void MainForm_LoadComplete(object sender, EventArgs e)
        {
            UpdateStatusBarLayout();
        }
    }
}
