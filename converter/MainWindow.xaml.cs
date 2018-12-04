using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;


namespace converter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
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
				FfprobeInfo inputInfo = new FfprobeInfo(videoFile.FilePath, videoFile.FileName, videoFile.FileExtension);
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
	}
}
