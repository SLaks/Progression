using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using SLaks.Progression.Display.WinForms;

namespace SLaks.Progression.Tests {
	[TestClass]
	public class ProgressBarReporterTest : ProgressReporterTestBase {
		protected override IProgressReporter CreateReporter() {
			return new ProgressBarReporter(new ProgressBar());
		}

		public override bool SupportsCancellation { get { return false; } }

		[TestMethod]
		public void ProgressBarIsUpdated() {
			var pr = new ProgressBarReporter(new ProgressBar());
			pr.Progress = 50;
			Assert.AreEqual(50, pr.Bar.Value);
			Assert.AreEqual(100, pr.Bar.Maximum);
		}
		[TestMethod]
		public void SupportsInt64s() {
			var pr = new ProgressBarReporter(new ProgressBar());
			pr.Maximum = Int64.MaxValue / 3;
			pr.Progress = Int64.MaxValue / 6;
			Assert.AreEqual(pr.Bar.Maximum / 2, pr.Bar.Value);
		}
		[TestMethod]
		public void MarqueeWorks() {
			var pr = new ProgressBarReporter(new ProgressBar());
			pr.Progress = null;
			Assert.AreEqual(ProgressBarStyle.Marquee, pr.Bar.Style);
		}
		[TestMethod]
		public void StyleIsPreserved() {
			var pr = new ProgressBarReporter(new ProgressBar { Style = ProgressBarStyle.Continuous });
			pr.Progress = null;
			pr.Progress = 50;
			Assert.AreEqual(ProgressBarStyle.Continuous, pr.Bar.Style);
		}
	}
}
