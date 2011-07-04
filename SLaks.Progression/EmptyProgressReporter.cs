using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLaks.Progression {
	///<summary>A placeholder ProgressReporter implementation that doesn't do anything.</summary>
	///<remarks>
	///	<para>
	///	 This class exists to allow progress-reporting methods to be called without progress.
	///	</para>
	///	<para>
	///	 Methods should take an <see cref="IProgressReporter"/> as an optional parameter and default to <see cref="EmptyProgressReporter"/>.
	///	 <code>
	///		public static void Example(IProgressReporter progress) {	
	///	 	progress = progress ?? new EmptyProgressReporter();
	///	 	progress.Caption = "Demonstrating an example";
	///	 }</code>
	///	</para>
	///</remarks>
	public class EmptyProgressReporter : IProgressReporter {
		///<summary>Gets or sets a string describing the current operation.  This property has no effect.</summary>
		public string Caption { get; set; }

		///<summary>Gets or sets the progress value at which the operation will be completed.  This property has no effect.</summary>
		public long Maximum { get; set; }
		///<summary>Gets or sets the current progress, between 0 and Maximum.  This property has no effect.</summary>
		public long? Progress { get; set; }

		///<summary>Always returns false.</summary>
		public bool AllowCancellation {
			get { return false; }
			set { }
		}

		///<summary>Always returns false.</summary>
		public bool WasCanceled { get { return false; } }
	}
}
