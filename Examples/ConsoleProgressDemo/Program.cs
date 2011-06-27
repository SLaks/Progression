using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLaks.ProgressReporting;
using SLaks.ProgressReporting.Display;
using System.Threading;

namespace ConsoleProgressDemo {
	static class Program {
		static void RunAll(IProgressReporter progress, params Action[] actions) {
			progress.Maximum = actions.Length;
			foreach (var a in actions) {
				a();
				progress.Progress++;
			}
		}

		static void CancellableWait(TimeSpan duration, IProgressReporter progress) {
			progress.AllowCancellation = true;
			progress.Maximum = 100;
			for (int i = 0; i < 100; i++) {
				Thread.Sleep((int)(duration.TotalMilliseconds / 100));
				progress.Progress++;

				if (progress.WasCanceled)
					return;
			}
		}

		static readonly string[] stepNames = {
												 "Reticulating splines", 
												 "Charging flux capacitor", 
												 "Ordering pizza",
												 "Panicing",
												 "Waiting for Godot",
												 "Step not found",
												 "Being useless",
												 "(null)",
												 "Re-calibrating the internet",
												 "Contacting elves",
												 "Contacting elfs"
											 };
		static void BigFakeOperation(IProgressReporter progress) {
			Shuffle(stepNames);
			stepNames[2] = "Performing long complicated operation which has a caption so long that it won't fit in the console";

			var rand = new Random();
			progress.Maximum = rand.Next(5, stepNames.Length - 2);
			progress.AllowCancellation = true;
			for (int i = 0; i < progress.Maximum; i++) {
				progress.Caption = i + ": " + stepNames[i];
				progress.Progress++;

				Thread.Sleep(rand.Next(500, 3500));

				if (progress.WasCanceled) break;
			}
		}

		static void Main(string[] args) {
			RunAll(new ConsoleProgressReporter(showCaption: false),
				() => Console.WriteLine("\r\nThis program demonstrates the console-based progress bar."),
				() => Thread.Sleep(TimeSpan.FromSeconds(1)),
				() => Console.WriteLine("The upper progress bar tells how far you are in the demo"),
				() => {
					Console.Write("I will now wait 30 seconds. Press C to stop waiting. ");
					CancellableWait(TimeSpan.FromSeconds(30), new ConsoleProgressReporter(showCaption: false));
				},
				() => {
					Console.Write("Progress bars can also have captions!   ");
					BigFakeOperation(new ConsoleProgressReporter(showCaption: true));
				}
			);
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey(true);
		}

		//http://stackoverflow.com/questions/375351/most-efficient-way-to-randomly-sort-shuffle-a-list-of-integers-in-c/375446#375446
		public static void Shuffle<T>(IList<T> array) {
			Random random = new Random();
			for (int i = 0; i < array.Count; i++) {
				int swapIndex = random.Next(i, array.Count);
				if (swapIndex != i) {
					T temp = array[i];
					array[i] = array[swapIndex];
					array[swapIndex] = temp;
				}
			}
		}
	}
}
