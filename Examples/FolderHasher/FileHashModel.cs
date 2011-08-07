using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLaks.Progression.Display;
using System.IO;
using SLaks.Progression;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

namespace FolderHasher {
	class FileHashModel : ProgressModel {
		static readonly string sampleDir = Environment.ExpandEnvironmentVariables(@"%SYSTEMROOT%\Web\Wallpaper");
		public static readonly ObservableCollection<FileHashModel> Sample = new ObservableCollection<FileHashModel>(
			Directory.EnumerateFiles(sampleDir, "*", SearchOption.AllDirectories).Select(p => new FileHashModel(p, sampleDir))
		);

		public FileHashModel(string path, string rootPath) {
			FilePath = Path.GetFullPath(path);
			RelativePath = FilePath.Substring(Path.GetFullPath(rootPath).Length).TrimStart('\\');
			SizeString = NativeMethods.FormatSizeString(new FileInfo(path).Length);
		}

		string result;
		///<summary>Gets or sets the textual representation of the file's hash.</summary>
		public string Result {
			get { return result; }
			set { result = value; OnPropertyChanged("Result"); }
		}

		///<summary>Gets the full path of this file.</summary>
		public string FilePath { get; private set; }
		///<summary>Gets the path to this file, relative to the directory being hashed.</summary>
		public string RelativePath { get; private set; }

		///<summary>Gets the size of the file in the appropriate units.</summary>
		public string SizeString { get; private set; }

		public void ComputeHash(HashAlgorithm hasher) {
			var hashBytes = hasher.ComputeHash(FilePath, progress: this);
			if (hashBytes == null)
				Result = "Canceled";
			else
				Result = String.Concat(hashBytes.Select(b => b.ToString("X2")));
		}
	}
}
