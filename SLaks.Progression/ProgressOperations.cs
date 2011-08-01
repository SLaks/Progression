using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace SLaks.Progression {
	///<summary>Contains methods that perform useful operations and report progress.</summary>
	public static class ProgressOperations {
		///<summary>Copies one stream to another while reporting progress.</summary>
		///<param name="from">The stream to copy from.  This stream must be readable.</param>
		///<param name="to">The stream to copy to.  This stream must be writable.</param>
		///<param name="progress">An IProgressReporter implementation to report the progress of the upload.</param>
		///<returns>The number of bytes copied.</returns>
		public static long CopyTo(this Stream from, Stream to, IProgressReporter progress) { return from.CopyTo(to, null, progress); }
		///<summary>Copies one stream to another while reporting progress.</summary>
		///<param name="from">The stream to copy from.  This stream must be readable.</param>
		///<param name="to">The stream to copy to.  This stream must be writable.</param>
		///<param name="length">The length of the source stream.  This parameter is only used to report progress.</param>
		///<param name="progress">An IProgressReporter implementation to report the progress of the upload.</param>
		///<returns>The number of bytes copied.</returns>
		public static long CopyTo(this Stream from, Stream to, long? length, IProgressReporter progress) {
			if (from == null) throw new ArgumentNullException("from");
			if (to == null) throw new ArgumentNullException("to");

			if (!from.CanRead) throw new ArgumentException("Source stream must be readable", "from");
			if (!to.CanWrite) throw new ArgumentException("Destination stream must be writable", "to");

			if (progress != null) {
				if (length == null) {
					try {
						length = from.Length;
					} catch (NotSupportedException) { progress.Progress = null; }
				}

				if (length == null)
					progress.Progress = null;
				else
					progress.Maximum = length.Value;
			}
			progress = progress ?? new EmptyProgressReporter();

			long totalCopied = 0;
			var buffer = new byte[4096];
			while (true) {
				var bytesRead = from.Read(buffer, 0, buffer.Length);

				if (length != null) progress.Progress = totalCopied;
				if (progress.WasCanceled) return -1;

				totalCopied += bytesRead;
				if (bytesRead == 0) return totalCopied;
				to.Write(buffer, 0, bytesRead);
			}
		}

		///<summary>Calculates a cryptographic hashcode of a file while reporting progress.</summary>
		///<returns>The hash of the data, or null if the user clicked cancel.</returns>
		public static byte[] ComputeHash(this HashAlgorithm hasher, string fileName, IProgressReporter progress) {
			using (var stream = File.OpenRead(fileName))
				return ComputeHash(hasher, stream, progress);
		}
		///<summary>Calculates a cryptographic hashcode of a stream while reporting progress.</summary>
		///<returns>The hash of the data, or null if the user clicked cancel.</returns>
		public static byte[] ComputeHash(this HashAlgorithm hasher, Stream stream, IProgressReporter progress) {
			if (hasher == null) throw new ArgumentNullException("hasher");
			if (stream == null) throw new ArgumentNullException("stream");


			if (progress != null) {
				try {
					progress.Maximum = stream.Length;
				} catch (NotSupportedException) { progress.Progress = null; }
			}
			progress = progress ?? new EmptyProgressReporter();

			long totalCopied = 0;
			var buffer = new byte[4096];
			while (true) {
				var bytesRead = stream.Read(buffer, 0, buffer.Length);

				if (progress.Progress != null) progress.Progress = totalCopied;
				if (progress.WasCanceled) return null;

				totalCopied += bytesRead;
				if (bytesRead == 0) break;
				hasher.TransformBlock(buffer, 0, bytesRead, null, 0);
			}
			hasher.TransformFinalBlock(new byte[0], 0, 0);
			var retVal = (byte[])hasher.Hash.Clone();
			hasher.Initialize();
			return retVal;
		}
	}
}
