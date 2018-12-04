using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace converter
{
	class FFmpegConvertor
	{
		string convertOptions;
		string workingFolder;
		string inputFile;
		string outputFile;
		string fullConvertString;

		public FFmpegConvertor(string inputFile, string convertOptions, string outputFile)
		{
			this.convertOptions = convertOptions;
			workingFolder = "c:\\tmp\\";
			this.inputFile = inputFile;
			this.outputFile = outputFile;
		}
		public FFmpegConvertor(string inputFile, string convertOptions, string outputFile, string workingFolder)
		{
			this.convertOptions = convertOptions;
			this.workingFolder = workingFolder;
			this.inputFile = inputFile;
			this.outputFile = outputFile;
		}

		public void Convert()
		{
			fullConvertString = "-i " + "\"" + inputFile + "\" " + convertOptions + " \"" + outputFile + "\"";
			File.WriteAllText("d:\\stdErrorFfmpeg.txt", string.Empty);
			Process ffmpeg = new Process
			{
				StartInfo =
				{
				FileName = "c:\\ffmpeg.exe",
				Arguments = fullConvertString,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true,
			//WorkingDirectory = "c:\\tmp"
				}
			};

			ffmpeg.OutputDataReceived += new DataReceivedEventHandler(FfmpegOutputHandler);

			ffmpeg.Start();
			while (!ffmpeg.StandardError.EndOfStream)
			{
				string stderr = ffmpeg.StandardError.ReadLine();
				File.AppendAllText("d:\\stdErrorFfmpeg.txt", stderr + Environment.NewLine);
			}
			
			ffmpeg.WaitForExit();

		}

		// https://stackoverflow.com/questions/11994610/c-sharp-get-process-output-while-running
		private static void FfmpegOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			if (!String.IsNullOrEmpty(outLine.Data))
			{
			//	MainWindow.UpdateStream = outLine;
			}
		}
	}
}
