using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SLaks.Progression.Display {
	///<summary>An <see cref="IProgressReporter"/> implementation that displays progress on multiple other <see cref="IProgressReporter"/>s.</summary>
	public class MultiProgressReporter : IProgressReporter {
		///<summary>Creates a MultiProgressReporter that forwards to the specified set of child reporters.</summary>
		///<param name="reporters">The <see cref="IProgressReporter"/>s that will display progress passed to the new instance.</param>
		public MultiProgressReporter(params IProgressReporter[] reporters) {
			if (reporters == null) throw new ArgumentNullException("reporters");
			if (reporters.Length == 0) throw new ArgumentException("MultiProgressReporter requires at least one child reporter");
			Reporters = new ReadOnlyCollection<IProgressReporter>(reporters);
		}
		///<summary>Creates a MultiProgressReporter that forwards to the specified set of child reporters.</summary>
		///<param name="reporters">The <see cref="IProgressReporter"/>s that will display progress passed to the new instance.</param>
		public MultiProgressReporter(IEnumerable<IProgressReporter> reporters) {
			if (reporters == null) throw new ArgumentNullException("reporters");
			if (!reporters.Any()) throw new ArgumentException("MultiProgressReporter requires at least one child reporter");
			Reporters = new ReadOnlyCollection<IProgressReporter>(reporters.ToList());
		}

		///<summary>Gets the reporters that this instance displays progress on.</summary>
		public ReadOnlyCollection<IProgressReporter> Reporters { get; private set; }

		///<summary>Gets or sets a string describing the current operation to display above the progress bar.</summary>
		public string Caption {
			get { return Reporters[0].Caption; }
			set { foreach (var r in Reporters)r.Caption = value; }
		}

		///<summary>Gets or sets the progress value at which the operation will be completed.</summary>
		public long Maximum {
			get { return Reporters[0].Maximum; }
			set { foreach (var r in Reporters)r.Maximum = value; }
		}
		///<summary>Gets or sets the current progress, between 0 and Maximum, or null to display a marquee.</summary>
		public long? Progress {
			get { return Reporters[0].Progress; }
			set { foreach (var r in Reporters)r.Progress = value; }
		}

		///<summary>Gets or sets whether the operation can be cancelled.  The default is false.  
		///If none of the child reporters allow users to cancel operations, this property will always return false.</summary>
		public bool AllowCancellation {
			get { return Reporters.Any(r => r.AllowCancellation); }	//Even if the first child doesn't support cancellation
			set { foreach (var r in Reporters)r.AllowCancellation = value; }
		}
		///<summary>Indicates whether the user has cancelled the operation in any of the child reporters..</summary>
		public bool WasCanceled { get { return Reporters.Any(r => r.WasCanceled); } }
	}
}
