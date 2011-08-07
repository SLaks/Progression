using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FolderHasher {
	static class NativeMethods {
		[DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
		static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

		public static string FormatSizeString(long size) {
			var buffer = new StringBuilder(32);
			StrFormatByteSize(size, buffer, buffer.Capacity);
			return buffer.ToString();
		}
	}
}
