using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLaks.ProgressReporting.Display {
	///<summary>Prints a progress bar on the console.</summary>
	///<remarks>
	///The progress bar is one line tall, or two if configured to show a caption.
	///</remarks>
	public class ConsoleProgressReporter : ScaledProgressReporter, IProgressReporter {
		#region Settings
		readonly int originX, originY;

		///<summary>Creates a ConsoleProgressReporter instance.</summary>
		///<param name="showCaption">Whether to show the caption in the above the progress bar in the console.</param>
		///<param name="width">The width of the bar in characters.  Defaults to fill the console window.</param>
		public ConsoleProgressReporter(bool showCaption = false, int width = -1) {
			if (width > Console.BufferWidth - Console.CursorLeft)
				throw new ArgumentOutOfRangeException("width", "Width must fit within the console window.");

			ShowCaption = showCaption;
			if (width < 0)
				BarWidth = Console.WindowWidth - Console.CursorLeft;
			else
				BarWidth = width;

			originX = Console.CursorLeft;
			originY = Console.CursorTop;

			if (ShowCaption) {
				DrawCaption(TrimCaption(null));
				Console.CursorTop++;
			}
			DrawChars(blankChar, BarWidth);	//Draw the initial blank bar, and don't move the cursor back
		}

		///<summary>Indicates whether this instance has been configured to show a caption in the console.</summary>
		public bool ShowCaption { get; private set; }
		///<summary>Gets the width of the entire progress bar in characters.</summary>
		public int BarWidth { get; private set; }
		#endregion


		static IDisposable CursorPosition(int x, int y) {
			var retVal = CursorPosition();
			Console.SetCursorPosition(x, y);
			return retVal;
		}
		static IDisposable CursorPosition() {
			int x = Console.CursorLeft, y = Console.CursorTop;
			bool v = Console.CursorVisible;
			Console.CursorVisible = false;
			return new Disposable(() => {
				Console.SetCursorPosition(x, y);
				Console.CursorVisible = v;
			});
		}

		#region Caption
		string caption;

		///<summary>Gets or sets a string describing the current operation to display above the progress bar.  The caption is only drawn if <see cref="ShowCaption"/> is true.</summary>
		public string Caption {
			get { return caption; }
			set {
				caption = value;
				if (ShowCaption)
					DrawCaption(TrimCaption(value));
			}
		}

		string TrimCaption(string value) {
			string retVal;
			if (String.IsNullOrWhiteSpace(value))
				retVal = "Please wait";
			else
				retVal = value.Trim();

			if (retVal.Length > BarWidth)
				retVal = retVal.Remove(BarWidth - 1) + "…";
			return retVal;
		}
		void DrawCaption(string text) {
			double edgeWidth = (BarWidth - text.Length) / 2.0;
			using (CursorPosition(originX, originY)) {
				Console.Write(new string(' ', (int)Math.Ceiling(edgeWidth)));
				Console.Write(text);
				Console.Write(new string(' ', (int)Math.Floor(edgeWidth)));
			}
		}
		#endregion

		#region Bar
		const char barChar = '█';
		const char blankChar = '_';

		///<summary>Gets the maximum that the progress bar's value will be scaled to.</summary>
		protected override int ScaledMax { get { return BarWidth; } }

		///<summary>Draws the progress bar to the console.</summary>
		protected override void UpdateBar(int oldValue, int newValue) {
			int barY = originY;
			if (ShowCaption) barY++;

			if (newValue < 0) {
				//Marquee bar; there isn't much scope for a marquee bar on the console
				//TODO: Optional animation; bounce a spinner back and forth.
				using (CursorPosition(originX, barY))
					DrawChars('▒', BarWidth);
				return;
			}

			if (oldValue < 0) {
				//Draw entire bar
				using (CursorPosition(originX, barY)) {
					DrawChars(barChar, newValue);
					DrawChars(blankChar, BarWidth - newValue);
				}
			} else {
				//Only draw the portion that changed
				if (newValue > oldValue) {
					using (CursorPosition(originX + oldValue, barY))
						DrawChars(barChar, newValue - oldValue);
				} else {
					using (CursorPosition(originX + newValue, barY))
						DrawChars(blankChar, oldValue - newValue);
				}
			}
		}

		static void DrawChars(char ch, int count) {
			if (count == 0) return;
			Console.Write(new string(ch, count));
		}
		#endregion

		///<summary>Gets or sets whether the operation can be cancelled.  The default is false.</summary>
		public bool AllowCancellation { get; set; }

		bool wasAlreadyCanceled;
		///<summary>Indicates whether the user has cancelled the operation.</summary>
		public bool WasCanceled {
			get { return AllowCancellation && wasAlreadyCanceled || (wasAlreadyCanceled = Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.C); }
		}
	}
}
