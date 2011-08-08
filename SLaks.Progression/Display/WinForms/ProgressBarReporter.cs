using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLaks.Progression.Display.WinForms {
	///<summary>An IProgressReporter implementation that displays progress on a WinForms progress bar control.  Does not support captions or cancellation.</summary>
	public class ProgressBarReporter : ScaledProgressReporter, IProgressReporter {
		readonly ProgressBarStyle defaultStyle;

		///<summary>Creates a new ProgressBarReporter that displays progress on the specified ProgressBar.</summary>
		public ProgressBarReporter(ProgressBar bar) {
			if (bar == null) throw new ArgumentNullException("bar");
			Bar = bar;
			if (Bar.Style == ProgressBarStyle.Marquee)
				defaultStyle = ProgressBarStyle.Blocks;
			else
				defaultStyle = Bar.Style;
		}

		///<summary>Gets the progress bar that this instance controls.</summary>
		public ProgressBar Bar { get; private set; }
		///<summary>Gets the maximum that the progress bar's value will be scaled to.</summary>
		protected override int ScaledMax { get { return Bar.Maximum; } }

		///<summary>Gets or sets the progress value at which the operation will be completed.</summary>
		public override long Maximum {
			get { return base.Maximum; }
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException("value");

				Bar.Maximum = (int)Math.Min(131072, value);	//According to the source, this can be set on other threads.
				base.Maximum = value;
			}
		}

		void LazyInvoke(Action a) {
			if (Bar.InvokeRequired)
				Bar.BeginInvoke(a);
			else
				a();
		}
		///<summary>Updates the progress bar control to reflect the current progress.</summary>
		protected override void UpdateBar(int? oldValue, int? newValue) {
			LazyInvoke(delegate {
				Bar.Style = newValue == null ? ProgressBarStyle.Marquee : defaultStyle;
				if (newValue != null)
					Bar.Value = newValue.Value;
			});
		}

		///<summary>Gets or sets a caption for the current operation.  This property is ignored.</summary>
		public string Caption { get; set; }

		///<summary>Always returns false.</summary>
		public bool AllowCancellation { get { return false; } set { } }
		///<summary>Always returns false.</summary>
		public bool WasCanceled { get { return false; } }
	}
}
