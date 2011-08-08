using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLaks.Progression {
	///<summary>Displays the progress of an operation.</summary>
	///<remarks>
	///	<para>
	///	A method that performs a lengthy operation can take an IProgressReporter 
	///	to allow callers to display the operation's progress in the UI, without coupling 
	///	the method to any UI framework.
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
	public interface IProgressReporter {
		///<summary>Gets or sets a string describing the current operation to display above the progress bar.</summary>
		string Caption { get; set; }

		///<summary>Gets or sets the progress value at which the operation will be completed.</summary>
		long Maximum { get; set; }
		///<summary>Gets or sets the current progress, between 0 and Maximum, or null to display marquee.</summary>
		long? Progress { get; set; }

		///<summary>Gets or sets whether the operation can be cancelled.  The default is false.  
		///If this progress reporter does not allow users to cancel operations, this property will always return false.</summary>
		///<remarks>Setting this property will reset <see cref="WasCanceled"/>.</remarks>
		bool AllowCancellation { get; set; }
		///<summary>Indicates whether the user has cancelled the operation.</summary>
		bool WasCanceled { get; }
	}
}
