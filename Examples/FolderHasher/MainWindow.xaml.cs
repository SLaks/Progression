using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;

namespace FolderHasher {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, System.Windows.Forms.IWin32Window {
		class HashOption {
			public HashOption(string name, Func<HashAlgorithm> creator) {
				this.name = name;
				this.creator = creator;
			}

			readonly Func<HashAlgorithm> creator;
			readonly string name;

			public HashAlgorithm CreateHasher() { return creator(); }
			public override string ToString() { return name; }
		}

		readonly ObservableCollection<FileHashModel> files = new ObservableCollection<FileHashModel>();

		public MainWindow() {
			InitializeComponent();
			hashAlgorithm.ItemsSource = new[] {
				new HashOption("MD5",		() => new MD5CryptoServiceProvider()),
				new HashOption("SHA1",		() => new SHA1Managed()),
				new HashOption("SHA256",	() => new SHA256Managed()),
				new HashOption("SHA384",	() => new SHA384Managed()),
				new HashOption("SHA512",	() => new SHA512Managed()),
			};
			hashAlgorithm.SelectedIndex = 0;
			list.ItemsSource = files;
		}

		private void Browse_Click(object sender, RoutedEventArgs e) {
			using (var dialog = new FolderBrowserDialog {
				SelectedPath = path.Text,
				Description = "Select a folder containing files to hash",
				ShowNewFolderButton = false
			}) {
				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
					return;
				path.Text = dialog.SelectedPath;
			}
		}


		private void path_TextChanged(object sender, TextChangedEventArgs e) {
			go.IsEnabled = !String.IsNullOrEmpty(path.Text) && Directory.Exists(path.Text);
		}

		private void Go_Click(object sender, RoutedEventArgs e) {
			var rootPath = path.Text;
			AddFiles(Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories), rootPath);
		}

		void AddFiles(IEnumerable<string> paths, string rootPath) {
			var hash = (HashOption)hashAlgorithm.SelectedItem;

			ThreadPool.QueueUserWorkItem(delegate {
				//This loop can take a long time, so I run it in
				//the background and add files to the UI as they
				//come in.
				foreach (var filePath in paths) {
					var file = new FileHashModel(filePath, rootPath);
					ThreadPool.QueueUserWorkItem(delegate { file.ComputeHash(hash.CreateHasher()); });

					Dispatcher.BeginInvoke(new Action(() => files.Add(file)));
				}
			});
		}


		IntPtr System.Windows.Forms.IWin32Window.Handle { get { return new WindowInteropHelper(this).Handle; } }
	}
}
