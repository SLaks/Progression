﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLaks.Progression.Display {
	///<summary>Prints a progress bar on the console.</summary>
	///<remarks>
	///The progress bar is one line tall, or two if configured to show a caption.
	///</remarks>
	public class ConsoleProgressReporter : ScaledProgressReporter, IProgressReporter {
		///<summary>Indicates whether the bar is actually drawn on the console.  If the process had no console when this instance was created, this will be false.</summary>
		public bool IsVisible { get; private set; }

		#region Settings
		readonly int originX, originY;

		///<summary>Creates a ConsoleProgressReporter instance that fills the remainder of the current line in the console.</summary>
		///<param name="showCaption">Whether to show the caption in the above the progress bar in the console.</param>
		public ConsoleProgressReporter(bool showCaption) : this(showCaption, -1) { }

		///<summary>Creates a ConsoleProgressReporter instance.</summary>
		///<param name="showCaption">Whether to show the caption in the above the progress bar in the console.</param>
		///<param name="width">The width of the bar in characters.</param>
		public ConsoleProgressReporter(bool showCaption, int width) {
			ShowCaption = showCaption;
			IsVisible = NativeMethods.HasConsole;
			if (!IsVisible) return;

			if (width > Console.BufferWidth)
				throw new ArgumentOutOfRangeException("width", "Width must fit within the console window.");

			if (width < 0) {
				BarWidth = Console.WindowWidth - Console.CursorLeft;

				//If there isn't enough room for an auto-sized bar,
				//wrap to the next line.  Otherwise, we would crash
				//on long lines, which wouldn't be good.
				if (BarWidth < 3) {
					Console.WriteLine();
					BarWidth = Console.WindowWidth - Console.CursorLeft;
				}
			} else if (BarWidth < 3)
				throw new ArgumentOutOfRangeException("width", "Progress bar must be at least three characters wide");
			else {
				if (width > Console.BufferWidth - Console.CursorLeft)
					Console.WriteLine();	//If the bar won't fit within this line, wrap to the next line
				BarWidth = width;
			}

			BarWidth -= 2;	//Subtract two characters for the frame

			originX = Console.CursorLeft + 1;	//Add one character for the left frame
			originY = Console.CursorTop;

			if (ShowCaption) {
				//Draw the caption and its frame
				Console.Write('│');
				DrawCaption(TrimCaption(null));
				Console.CursorLeft = originX + BarWidth;
				Console.Write('│');
				//Move back into position for the bar frame
				Console.CursorLeft = originX - 1;
				Console.CursorTop = originY + 1;
			}
			//Draw the initial blank bar & frame, and keep the cursor after the bar
			Console.Write('│');
			DrawChars(blankChar, BarWidth);
			Console.Write('│');
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
			if (String.IsNullOrEmpty(value))
				retVal = "Please wait";
			else
				retVal = value.Trim();

			if (retVal.Length > BarWidth)
				retVal = retVal.Remove(BarWidth - 1) + "…";
			return retVal;
		}
		void DrawCaption(string text) {
			if (!IsVisible) return;

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
		protected override void UpdateBar(int? oldValue, int? newValue) {
			if (!IsVisible) return;
			int barY = originY;
			if (ShowCaption) barY++;

			if (newValue == null) {
				//Marquee bar; there isn't much scope for a marquee bar on the console
				//TODO: Optional animation; bounce a spinner back and forth.
				using (CursorPosition(originX, barY))
					DrawChars('▒', BarWidth);
				return;
			}

			if (oldValue == null) {
				//Draw entire bar
				using (CursorPosition(originX, barY)) {
					DrawChars(barChar, newValue.Value);
					DrawChars(blankChar, BarWidth - newValue.Value);
				}
			} else {
				//Only draw the portion that changed
				if (newValue > oldValue) {
					using (CursorPosition(originX + oldValue.Value, barY))
						DrawChars(barChar, newValue.Value - oldValue.Value);
				} else {
					using (CursorPosition(originX + newValue.Value, barY))
						DrawChars(blankChar, oldValue.Value - newValue.Value);
				}
			}
		}

		static void DrawChars(char ch, int count) {
			if (count == 0) return;
			Console.Write(new string(ch, count));
		}
		#endregion

		bool allowCancellation;
		///<summary>Gets or sets whether the operation can be cancelled.  The default is false.</summary>
		///<remarks>Setting this property will reset <see cref="WasCanceled"/>.</remarks>
		public bool AllowCancellation {
			get { return allowCancellation; }
			set {
				allowCancellation = value;
				wasAlreadyCanceled = false;
			}
		}
		bool wasAlreadyCanceled;
		///<summary>Indicates whether the user has cancelled the operation.</summary>
		public bool WasCanceled {
			get { return AllowCancellation && IsVisible 
					 && (wasAlreadyCanceled || (wasAlreadyCanceled = Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.C)); }
		}
	}
}
