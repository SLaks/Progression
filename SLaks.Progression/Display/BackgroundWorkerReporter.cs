using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SLaks.Progression.Display {
	///<summary>Reports progress through an existing BackgroundWorker.  Does not support captions.</summary>
	///<remarks>
	/// You are responsible for handling the BackgroundWorker's ProgressChagned events and updating UI
	///<para>
	///	This class is primarily intended to be used with existing code that
	///	already uses a BackgroundWorker.  New code can usually use other 
	///	progress reporters to update the UI directly.
	///</para>
	///</remarks>
	public class BackgroundWorkerReporter : ScaledProgressReporter, IProgressReporter {
		///<summary>Creates a BackgroundWorkerReporter instance that reports progress to the specified BackgroundWorker.</summary>
		public BackgroundWorkerReporter(BackgroundWorker worker) {
			if (worker == null) throw new ArgumentNullException("worker");
			Worker = worker;
			worker.WorkerReportsProgress = true;
		}

		///<summary>Gets the BackgroundWorker instance that this reporter updates.</summary>
		public BackgroundWorker Worker { get; private set; }

		///<summary>Gets the maximum that the progress bar's value will be scaled to.</summary>
		protected override int ScaledMax { get { return 100; } }

		///<summary>Updates the BackgroundWorker's progress.</summary>
		protected override void UpdateBar(int? oldValue, int? newValue) {
			if (newValue != null)
				Worker.ReportProgress(newValue.Value);
		}

		///<summary>Gets or sets a caption for the current operation.  This property is ignored.</summary>
		public string Caption { get; set; }

		///<summary>Gets or sets whether the operation can be cancelled.  The default is false.</summary>
		///<remarks>Setting this property will reset <see cref="WasCanceled"/>.</remarks>
		public bool AllowCancellation {
			get { return Worker.WorkerSupportsCancellation; }
			set { Worker.WorkerSupportsCancellation = value; }
		}

		///<summary>Indicates whether the user has cancelled the operation.</summary>
		public bool WasCanceled { get { return Worker.CancellationPending; } }
	}
}
