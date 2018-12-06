using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;


namespace converter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 
	public partial class MainWindow : Window
	{

		public static string workingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Convertor";
		public static string logsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Convertor\\logs";
		InputFile mediaFile;
		
		//check if it is first start of the application
		//It is done by checking if there is "Converter" folder in main roaming\Appdata folder
		public bool firstStart = !File.Exists(workingFolder);


		public MainWindow()
		{
			initialiseStartupEnvironment();
			InitializeComponent();
		}

		public void initialiseStartupEnvironment()
		{
			if (firstStart)
			{
				Directory.CreateDirectory(workingFolder);
				Directory.CreateDirectory(logsFolder);
			}
		}

		private void File_Open_Button_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog(); // instanciate OpenFileDialog

			// set parameters for openFileDialog
			openFileDialog.Title = "Select video file";
			openFileDialog.Filter = "All Video Files|*.mp4;*.mpg;*.avi;*.m4a|Mp4Files(*.mp4)|*.mp4|Mpg Files(*.mpg)|*.mpg|Avi Files(*.avi)|*.avi|All files|*.*";
			if (openFileDialog.ShowDialog() == true)
			{
				//Instanciate input file

				mediaFile = new InputFile(openFileDialog.FileName, workingFolder, logsFolder);
				//instanciate ffprobeInfo file for analysing file
				//mediaInfo = new FfprobeInfo(mediaFile.FilePath, mediaFile.FileName, mediaFile.FileExtension, workingFolder, logsFolder);
				//fillup box with basic media-file data
				fillMainMediaInfo(mediaFile);
				//instanciate ffprobeInfo file for analysing file
				//fillup box with extended media-file data
				fillExtendedMediaInfo(mediaFile);
			}
		}

		private void Convert_Click(object sender, RoutedEventArgs e)
		{
			FFmpegConvertor convertor = new FFmpegConvertor(FullFileName.Text, "", SavePath.Text);
			convertor.Convert();
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Title = "Save File Location";
			if (saveFileDialog.ShowDialog() == true)
			{
				SavePath.Text = saveFileDialog.FileName;
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

	}
}
