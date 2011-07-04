using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SLaks.Progression.Display.WinForms {
	///<summary>An IProgressReporter implementation that displays progress in a small popup form.  Supports captions and cancellation.</summary>
	public partial class ProgressForm : Form, IProgressReporter {
		#region Helpers
		///<summary>Executes an operation and displays its progress.</summary>
		///<param name="method">The method to execute on the background thread.</param>
		///<returns>False if operation was cancelled.</returns>
		public static bool Execute(Action<IProgressReporter> method) { return Execute(null,method); }
		///<summary>Executes an operation and displays its progress.</summary>
		///<param name="parent">The form that will own the progress display.  This parameter can be null.</param>
		///<param name="method">The method to execute on the background thread.</param>
		///<returns>False if operation was cancelled.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Cross-thread exception marshalling")]
		public static bool Execute(IWin32Window parent, Action<IProgressReporter> method) {
			if (method == null) throw new ArgumentNullException("method");

			Exception exception = null;

			bool canceled = false, finished = true;
			using (var dialog = new ProgressForm()) {
				dialog.Show(parent);

				ThreadPool.QueueUserWorkItem(delegate {
					try {
						method(dialog);
					} catch (Exception ex) {
						exception = ex;
					} finally {
						finished = true;
						dialog.Hide();
					}
					canceled = dialog.WasCanceled;
				});
				if (!dialog.IsDisposed && !finished)
					dialog.ShowDialog(parent);	//Only show modally if the operation hasn't finished yet; this avoids a brief flicker for very quick operations
			}
			if (exception != null)
				throw new TargetInvocationException(exception);
			return !canceled;
		}
		#endregion

		readonly int cancelWidthDelta;

		///<summary>Creates a new ProgressForm instance.</summary>
		public ProgressForm() {
			InitializeComponent();
			barReporter = new ProgressBarReporter(progressBar);
			cancelWidthDelta = cancelButton.Right - progressBar.Right;
			AllowCancellation = false;
		}

		readonly ProgressBarReporter barReporter;
		///<summary>Gets or sets the progress value at which the operation will be completed.</summary>
		public long Maximum {
			get { return barReporter.Maximum; }
			set { barReporter.Maximum = value; }
		}
		///<summary>Gets or sets the current progress, between 0 and Maximum, or null to display a marquee.</summary>
		public long? Progress {
			get { return barReporter.Progress; }
			set { barReporter.Progress = value; }
		}

		void LazyInvoke(Action a) {
			if (InvokeRequired)
				BeginInvoke(a);
			else
				a();
		}

		///<summary>Gets or sets the text of the label above the progress bar.</summary>
		public string Caption {
			get { return label.Text; }	//Text can be read from non-UI threads.
			set { LazyInvoke(() => label.Text = value); }
		}

		///<summary>Gets or sets whether the operation can be cancelled.  The default is false.</summary>
		///<remarks>Setting this property will reset <see cref="WasCanceled"/>.</remarks>
		public bool AllowCancellation {
			get { return cancelButton.Visible; }	//Visible can be read from non-UI threads.
			set {
				if (AllowCancellation == value) return;
				Invoke(new Action(delegate {	//This must be synchronous or the property value won't be reflected right away
					cancelButton.Visible = value;
					if (value)
						progressBar.Width -= cancelWidthDelta;
					else
						progressBar.Width += cancelWidthDelta;

					WasCanceled = false;
					cancelButton.Enabled = true;
				}));
			}
		}
		///<summary>Indicates whether the user has clicked Cancel.</summary>
		public bool WasCanceled { get; private set; }

		private void cancelButton_Click(object sender, EventArgs e) {
			WasCanceled = true;
			cancelButton.Enabled = true;
		}
	}
}
