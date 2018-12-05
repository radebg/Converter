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
				InputFile videoFile = new InputFile(openFileDialog.FileName);
				FullFileName.Text = videoFile.FullFileName;
				FilePath.Text = videoFile.FilePath;
				FileName.Text = videoFile.FileName;
				FileExtension.Text = videoFile.FileExtension;

				//instanciate ffprobeInfo file for analysing file
				FfprobeInfo inputInfo = new FfprobeInfo(videoFile.FilePath, videoFile.FileName, videoFile.FileExtension, workingFolder, logsFolder);
				inputInfo.Analyse();

				NumberOfStreams.Content = "Number of streams:  " + inputInfo.NumberOfStreams;
				TextBoxInfo.Clear();
				for (int i = 0; i < inputInfo.NumberOfStreams; i++)
				{
					TextBoxInfo.AppendText("Stream " + i + ": " + inputInfo.StreamType[i]);
					if (inputInfo.StreamType[i] == "video")
					{
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Width: " + inputInfo.StreamWidth[i].ToString());
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Height: " + inputInfo.StreamHeight[i].ToString());
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Codec: " + inputInfo.StreamCodecName[i]);
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Duration: " + inputInfo.StreamDuration[i]);
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Frame rate: " + inputInfo.StreamFrameRate[i]);
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Aspect Ratio: " + inputInfo.StreamAspectRatio[i]);
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Bit Rate: " + inputInfo.StreamBitRate[i]);
						TextBoxInfo.AppendText(Environment.NewLine);

					}
					TextBoxInfo.AppendText(Environment.NewLine);
					if (inputInfo.StreamType[i] == "audio")
					{
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Codec: " + inputInfo.StreamCodecName[i]);
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Duration: " + inputInfo.StreamDuration[i]);
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Sample Rate: " + inputInfo.StreamSampleRate[i]);
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Number of Channels: " + inputInfo.StreamNumberOfChannels[i]);
						TextBoxInfo.AppendText(Environment.NewLine);
						TextBoxInfo.AppendText("	Bit Rate: " + inputInfo.StreamBitRate[i]);
						TextBoxInfo.AppendText(Environment.NewLine);

					}
				}
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

	}
}
