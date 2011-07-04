using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLaks.Progression.Display;

namespace SLaks.Progression {
	///<summary>Contains extension methods for progress reporters.</summary>
	public static class ProgressReporterExtensions {
		///<summary>Returns an IProgressReporter that adds progress to an existing reporter without affecting the maximum.</summary>
		///<remarks>The new reporter will add its progress directly to the existing reporter without scaling for the maximum; 
		///the maximum of the original reporter is expected to equal the sum of the maximums of the child operations.</remarks>
		public static IProgressReporter ChildOperation(this IProgressReporter reporter) {
			if (reporter == null) return new EmptyProgressReporter();
			if (reporter.Progress == null) throw new ArgumentException("Child operations cannot be started on an indeterminate progress reporter.", "reporter");
			return new ChildReporter(reporter, null);
		}
		///<summary>Returns an IProgressReporter that adds progress to an existing reporter, scaled to a given range within the parent reporter.</summary>
		///<remarks>The maximum of the original reporter is expected to equal the sum of the ranges of the child operations.</remarks>
		public static IProgressReporter ScaledChildOperation(this IProgressReporter reporter, long range) {
			if (reporter == null) return new EmptyProgressReporter();
			if (reporter.Progress == null) throw new ArgumentException("Child operations cannot be started on an indeterminate progress reporter.", "reporter");
			return new ChildReporter(reporter, range);
		}

		//For a detailed description of the behavior of child operations
		//see https://github.com/SLaks/Progression/wiki/Child-Reporters.
		class ChildReporter : IProgressReporter {
			public ChildReporter(IProgressReporter parent, long? range) {
				this.parent = parent;
				parentRange = range;
				parentStart = parent.Progress.Value;
			}

			readonly IProgressReporter parent;
			///<summary>The range of the parent reporter's progress that is covered by this child, or null for an unscaled child.</summary>
			readonly long? parentRange;
			readonly long parentStart;

			public string Caption {
				get { return parent.Caption; }
				set { parent.Caption = value; }
			}

			//The child cannot affect the cancelability of the parent.
			bool childAllowCancel;
			public bool AllowCancellation {
				get { return childAllowCancel && parent.AllowCancellation; }
				set { childAllowCancel = value; }
			}

			public bool WasCanceled { get { return AllowCancellation && parent.WasCanceled; } }

			long? progress = 0;
			long maximum = 100;
			public long Maximum {
				get { return maximum; }
				set {
					if (value <= 0)
						throw new ArgumentOutOfRangeException("value");

					maximum = value;
					Progress = 0;
				}
			}

			public long? Progress {
				get { return progress; }
				set {
					if (value < 0 || value > Maximum)
						throw new ArgumentOutOfRangeException("value", "Progress must be between 0 and " + Maximum);
					progress = value;

					if (value != null) {
						if (parentRange == null)
							parent.Progress = parentStart + value;
						else
							parent.Progress = parentStart + (int)((double)value / Maximum * parentRange);
					}
				}
			}
		}
	}
}
