using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SLaks.Progression.Display {
	static class NativeMethods {
		public static bool HasConsole { get { return GetConsoleWindow() != IntPtr.Zero; } }

		[DllImport("Kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();
	}
}
