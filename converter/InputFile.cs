using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft;

namespace converter
{
	public class InputFile
	{
		string filePath;
		string fileName;
		string fileExtension;
		string fullFileName;

		public InputFile(string fullFileName)
		{
			filePath = Path.GetDirectoryName(fullFileName);
			filePath = filePath.TrimEnd(Path.DirectorySeparatorChar) + "\\";	//If there is directory path searator character at the end
																				// trim it and then add directory path separator character
																				//TODO - Make some more elegant solution

			fileName = Path.GetFileNameWithoutExtension(fullFileName);
			fileExtension = Path.GetExtension(fullFileName);
			this.fullFileName = fullFileName;

		}
		public string FileName
		{
			get
			{
				return fileName;
			}
		}

		public string FilePath
		{
			get
			{
				return filePath;
			}
		}

		public string FileExtension
		{
			get
			{
				return fileExtension;
			}
		}
		public string FullFileName
		{
			get
			{
				return fullFileName;
			}
				
		}
	}
}
