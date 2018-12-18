using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace converter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 
	public partial class MainWindow : Window
	{
		//TODO: NEXT LINE i DONT UNDERSTAND
		public static MainWindow AppWindow;

		// initialise working folders on program startup (%AppData% folder)
		public static string ffprobeWorkingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Convertor\\ffprobe";
		public static string ffprobeLogsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Convertor\\ffprobe\\logs";


		public static string ffmpegWorkingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Convertor\\ffmpeg";
		public static string ffmpegLogsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Convertor\\ffmpeg\\logs";

		public static string ConvertOptions = "";

		// declare object mediaFile as InputFile object
		InputFile mediaFile;

		//check if it is first start of the application
		//It is done by checking if there is "Converter" folder in main roaming\Appdata folder
		//TODO: make better way to check if program is running for the first time
		public bool firstStart = !File.Exists(ffprobeWorkingFolder);


		public MainWindow()
		{
			//TODO: dont anderstand next line
			AppWindow = this;
			initialiseStartupEnvironment();
			InitializeComponent();
		}

		public void initialiseStartupEnvironment()
		{
			if (firstStart)
			{
				Directory.CreateDirectory(ffmpegWorkingFolder);
				Directory.CreateDirectory(ffmpegLogsFolder);
				Directory.CreateDirectory(ffprobeWorkingFolder);
				Directory.CreateDirectory(ffprobeLogsFolder);
			}
		}



		//methods

		public void fillMainMediaInfo(InputFile mediaFile)
		{
			FullFileName.Text = mediaFile.FullFileName;
			FilePath.Text = mediaFile.FilePath;
			FileName.Text = mediaFile.FileName;
			FileExtension.Text = mediaFile.FileExtension;
		}

		public void fillExtendedMediaInfo(InputFile mediaFile)
		{
			NumberOfStreams.Content = "Number of streams:  " + mediaFile.NumberOfStreams;
			TextBoxInfo.Clear();
			for (int i = 0; i < mediaFile.NumberOfStreams; i++)
			{
				TextBoxInfo.AppendText("Stream " + i + ": " + mediaFile.TypeOfStream[i]);
				if (mediaFile.TypeOfStream[i] == "video")
				{
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Width: " + mediaFile.StreamWidth[i].ToString());
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Height: " + mediaFile.StreamHeight[i].ToString());
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Codec: " + mediaFile.StreamCodecName[i]);
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Duration: " + mediaFile.StreamDuration[i]);
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Frame rate: " + mediaFile.StreamFrameRate[i]);
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Aspect Ratio: " + mediaFile.StreamAspectRatio[i]);
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Bit Rate: " + mediaFile.StreamBitRate[i]);
					TextBoxInfo.AppendText(Environment.NewLine);

				}
				TextBoxInfo.AppendText(Environment.NewLine);
				if (mediaFile.TypeOfStream[i] == "audio")
				{
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Codec: " + mediaFile.StreamCodecName[i]);
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Duration: " + mediaFile.StreamDuration[i]);
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Sample Rate: " + mediaFile.StreamSampleRate[i]);
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Number of Channels: " + mediaFile.StreamNumberOfChannels[i]);
					TextBoxInfo.AppendText(Environment.NewLine);
					TextBoxInfo.AppendText("	Bit Rate: " + mediaFile.StreamBitRate[i]);
					TextBoxInfo.AppendText(Environment.NewLine);

				}
			}
		}


		//***************************************************************
		//Events
		//***************************************************************

		private void File_Open_Button_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog(); // instanciate OpenFileDialog

			// set parameters for openFileDialog
			openFileDialog.Title = "Select video file";
			openFileDialog.Filter = "All Video Files|*.mp4;*.mpg;*.avi;*.m4a|Mp4Files(*.mp4)|*.mp4|Mpg Files(*.mpg)|*.mpg|Avi Files(*.avi)|*.avi|All files|*.*";
			if (openFileDialog.ShowDialog() == true)
			{
				//Instanciate input file
				mediaFile = new InputFile(openFileDialog.FileName, ffprobeWorkingFolder, ffprobeLogsFolder);

				//fillup box with basic media-file data
				fillMainMediaInfo(mediaFile);

				//fillup box with extended media-file data
				fillExtendedMediaInfo(mediaFile);

				//enable save and convert buttons
				ConvertButton.IsEnabled = true;
				SaveFileButton.IsEnabled = true;
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();

			//default values when save file dialog shows
			saveFileDialog.Title = "Save File Location";
			saveFileDialog.FileName = mediaFile.FilePath + mediaFile.FileName;
			saveFileDialog.DefaultExt = "mp4";


			//put save path dialog in the save textbox in window
			if (saveFileDialog.ShowDialog() == true)
			{
				SavePath.Text = saveFileDialog.FileName;
			}
		}

		private void Convert_Click(object sender, RoutedEventArgs e)
		{
			if (SavePath.Text == "Same Path as Input File")     //initialise 
			{
				//define async function?????????
				Action action = () =>
				  {
					  FFmpegConvertor worker = new FFmpegConvertor(mediaFile, ConvertOptions, ffmpegWorkingFolder, ffmpegLogsFolder);
					  worker.Convert();
				  };
				//start async function?????????
				Task t2 = Task.Factory.StartNew(action);
			}
			else
			{
				Action action = () =>
				{
					FFmpegConvertor worker = new FFmpegConvertor(FullFileName.Text, ConvertOptions, SavePath.Text, ffmpegWorkingFolder);
				   worker.Convert();
				};
			}
		}



		public string UpdateStream
		{
			get { return ConverterFeed.Text; }
			set { ConverterFeed.AppendText(value); }
		}

		private void mnuWokingDirectory_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog workingFileDialog = new SaveFileDialog();
			workingFileDialog.Title = "Working File Location";
			if (workingFileDialog.ShowDialog() == true)
			{
				SavePath.Text = workingFileDialog.FileName;
			}
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			//disable buttons for save and covert on first initialisation of the window (startup)
			//It will be enabled when user choose input file
			SaveFileButton.IsEnabled = false;
			ConvertButton.IsEnabled = false;
		}

	}
}
