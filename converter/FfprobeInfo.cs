using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace converter
{

	public class FfprobeInfo
	{
		InputFileDetails typedObject;
		int numberOfStreams;
		string[] typeOfStream;
		string[] streamCodecName;
		string[] streamFrameRate;
		string[] streamDuration;
		string[] streamAspectRatio;
		string[] streamBitRate;
		string[] streamSampleRate;
		int?[] streamNumberOfChannels;
		int?[] streamWidth, streamHeight;
		string examinedFile;
		string jsonString;
		string workingFolder;
		string logsFolder;



		public FfprobeInfo(string path, string name, string extension, string workingFolder, string logsFolder)
		{
			examinedFile = path + name + extension;     //reconstructed path name
			this.workingFolder = workingFolder;         //working folder
			this.logsFolder = logsFolder;               //logs folder
		}

		// analyse input file with ffprobe
		public void Analyse()
		{
			//clear contents of the output files
			File.WriteAllText(logsFolder + "\\stderr.txt", string.Empty);
			File.WriteAllText(logsFolder + "\\stdout.txt", string.Empty);

			//todo - integrate working path


			string commandString = "-show_format -show_streams -print_format json -loglevel quiet \"" + examinedFile + "\"";
			//string commandString = "-select_streams v:0 -show_entries stream=width,height,bit_rate,duration -of default=noprint_wrappers=1 -print_format json \"" + examinedFile + "\"";
			//location of the ffmpeg files after installation
			string ffmpegExecutablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\ffmpeg"; 
			Process ffprobe = new Process
			{
				StartInfo =
				{
				FileName =ffmpegExecutablePath+"\\ffprobe.exe",
				Arguments = commandString,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true,
				//WorkingDirectory = "c:\\tmp"
				}
			};

			// stream output to file
			ffprobe.Start();
			while (!ffprobe.StandardOutput.EndOfStream)
			{
				string stdout = ffprobe.StandardOutput.ReadLine();
				string stderr = ffprobe.StandardError.ReadLine();
				File.AppendAllText(logsFolder + "\\stderr.txt", stderr + Environment.NewLine);
				File.AppendAllText(logsFolder + "\\stdout.txt", stdout + Environment.NewLine);
			}
			ffprobe.WaitForExit();

			//pickup json output as a string
			jsonString = File.ReadAllText(logsFolder + "\\stdout.txt");

			JObject parent = JObject.Parse(jsonString);

			// credits for next line of code goes to Tim Miller: https://gist.github.com/drasticactions/b68a68e9d84ad4abd26addc33b8429a7

			typedObject = JsonConvert.DeserializeObject<InputFileDetails>(jsonString);

			// You could think it is funny... but I was trying to solve it for a week now :( (that is funny)

			numberOfStreams = typedObject.streams.Count();  // get number of streams in opened file

			typeOfStream = new string[numberOfStreams];     //get type of each stream
			streamWidth = new int?[numberOfStreams];
			streamHeight = new int?[numberOfStreams];
			streamAspectRatio = new string[numberOfStreams];
			streamCodecName = new string[numberOfStreams];
			streamDuration = new string[numberOfStreams];
			streamFrameRate = new string[numberOfStreams];
			streamBitRate = new string[numberOfStreams];
			streamNumberOfChannels = new int?[numberOfStreams];
			streamSampleRate = new string[numberOfStreams];


			foreach (Stream stream in typedObject.streams)
			{
				int index = stream.index;
				typeOfStream[index] = stream.codec_type;
				streamWidth[index] = stream.width;
				streamHeight[index] = stream.height;
				streamAspectRatio[index] = stream.display_aspect_ratio;
				streamCodecName[index] = stream.codec_name;
				streamDuration[index] = stream.duration;
				streamFrameRate[index] = stream.r_frame_rate;
				streamBitRate[index] = stream.bit_rate;
				streamNumberOfChannels[index] = stream.channels;
				streamSampleRate[index] = stream.sample_rate;
			}
		}


		public int NumberOfStreams
		{
			get { return numberOfStreams; }
		}
		public string[] StreamType
		{
			get { return typeOfStream; }
		}
		public int?[] StreamWidth
		{
			get { return streamWidth; }
		}
		public int?[] StreamHeight
		{
			get { return streamHeight; }
		}
		public string[] StreamFrameRate
		{
			get { return streamFrameRate; }
		}
		public string[] StreamAspectRatio
		{
			get { return streamAspectRatio; }
		}
		public string[] StreamCodecName
		{
			get { return streamCodecName; }
		}
		public string[] StreamDuration
		{
			get { return streamDuration; }
		}
		public string[] StreamBitRate
		{
			get { return streamBitRate; }
		}
		public int?[] StreamNumberOfChannels
		{
			get { return streamNumberOfChannels; }
		}
		public string[] StreamSampleRate
		{
			get { return streamSampleRate; }
		}

	}
}
