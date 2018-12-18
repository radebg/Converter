using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace converter
{
	class FFmpegConvertor
	{
		string convertOptions;
		string workingFolder;
		string logsFolder;
		string inputFile;
		string outputFile;
		string fullConvertString;

		public FFmpegConvertor(InputFile inputFile, string convertOptions, string workingFolder, string logsFolder)
		{

			this.convertOptions = convertOptions;
			this.workingFolder = workingFolder;
			this.logsFolder = logsFolder;
			this.inputFile = inputFile.FullFileName;

			//determine if there is already file with same name
			int duplicateOutputFileCount = 0;
			outputFile = inputFile.FullFileName;
			while (File.Exists(outputFile))
			{
				duplicateOutputFileCount++;
				outputFile = inputFile.FilePath + inputFile.FileName + "(" + duplicateOutputFileCount + ")" + inputFile.FileExtension;
			}

		}

		//TODO: edit next ctor that is similar as previous but with passed output file path
		public FFmpegConvertor(string inputFile, string convertOptions, string outputFile, string workingFolder)
		{
			this.convertOptions = convertOptions;
			this.workingFolder = workingFolder;
			this.inputFile = inputFile;
			this.outputFile = outputFile;

			//determine if there is already file with same name
			int duplicateOutputFileCount = 0;
			while (File.Exists(outputFile))
			{
				duplicateOutputFileCount++;

				outputFile = inputFile + "(" + duplicateOutputFileCount + ")";
			}

		}



		public bool Convert()
		{

			string ffmpegExecutablePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ffmpeg";

			fullConvertString = "-i " + "\"" + inputFile + "\" " + convertOptions + " \"" + outputFile + "\"";

			//clear files with ffmpeg outputs:
			File.WriteAllText(logsFolder + "\\stdout.txt", string.Empty);
			File.WriteAllText(logsFolder + "\\sterr.txt", string.Empty);


			Process ffmpeg = new Process()
			{

				StartInfo =
				{
				FileName = ffmpegExecutablePath + "\\ffmpeg.exe",
				Arguments = fullConvertString,

		}
			};

			ffmpeg.StartInfo.UseShellExecute = false;
			ffmpeg.StartInfo.RedirectStandardOutput = true;
			ffmpeg.StartInfo.RedirectStandardError = true;
			ffmpeg.StartInfo.CreateNoWindow = true;
			ffmpeg.EnableRaisingEvents = true;

			//ffmpeg.OutputDataReceived += new DataReceivedEventHandler(FfmpegOutputHandler);
			//ffmpeg.ErrorDataReceived += new DataReceivedEventHandler(ffmpeg_OutputDataReceived);

			ffmpeg.Start();

			while (!ffmpeg.StandardError.EndOfStream)
			{

				//string stdout = ffmpeg.StandardOutput.ReadLine();
				//File.AppendAllText(logsFolder + "\\stdout.txt", stdout + Environment.NewLine);

				string stderr = ffmpeg.StandardError.ReadLine();
				File.AppendAllText(logsFolder + "\\sterr.txt", stderr + Environment.NewLine);


				Application.Current.Dispatcher.Invoke(new System.Action(() =>
				{
					MainWindow.AppWindow.ConverterFeed.AppendText(stderr);
					MainWindow.AppWindow.ConverterFeed.AppendText(Environment.NewLine);
					MainWindow.AppWindow.ConverterFeed.ScrollToEnd();
				}));



				//TODO: Add code that will update textbox on main window in realtime while ffmpeg is converting file

			}
			ffmpeg.WaitForExit();
			return true;

		}

		public void MyProcOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			if (!String.IsNullOrEmpty(outLine.Data))
			{
				MainWindow.AppWindow.ConverterFeed.AppendText(outLine.Data);
				MainWindow.AppWindow.ConverterFeed.AppendText(Environment.NewLine);
			}
		}

		// https://stackoverflow.com/questions/11994610/c-sharp-get-process-output-while-running


		void ffmpeg_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			Debug.WriteLine(e.Data);
		}
		private static void FfmpegOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			if (!String.IsNullOrEmpty(outLine.Data))
			{
				//MainWindow.UpdateStream = outLine;
			}
		}
	}
}
