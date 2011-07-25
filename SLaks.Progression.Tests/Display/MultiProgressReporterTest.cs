using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SLaks.Progression.Display;
using SLaks.Progression.Display.WinForms;

namespace SLaks.Progression.Tests {
	///<summary>Tests the compliance of a MultiProgressReporter with a bunch of reporters.</summary>
	[TestClass]
	public class MultiProgressReporterTest : ProgressReporterTestBase {
		protected override IProgressReporter CreateReporter() {
			return new MultiProgressReporter(
				new CancellableDummyReporter(),
				new EmptyProgressReporter(),
				new ProgressBarReporter(new System.Windows.Forms.ProgressBar())
			);
		}
		public override bool SupportsCancellation { get { return true; } }

		[TestMethod]
		public void PropertiesAreForwarded() {
			var pr = (MultiProgressReporter)CreateReporter();

			pr.Maximum = 75;
			foreach (var ch in pr.Reporters) {
				Assert.AreEqual(75, ch.Maximum);
				Assert.AreEqual(0, ch.Progress);
			}
			pr.Progress = 25;
			foreach (var ch in pr.Reporters)
				Assert.AreEqual(25, ch.Progress);

			pr.Caption = "Doing stuff";
			foreach (var ch in pr.Reporters)
				Assert.AreEqual("Doing stuff", ch.Caption);
		}

		[TestMethod]
		public void CancellationWorks() {
			var cdr = new CancellableDummyReporter();
			var pr = new MultiProgressReporter(new EmptyProgressReporter(), cdr);

			pr.AllowCancellation = true;
			Assert.IsTrue(cdr.AllowCancellation);
			cdr.Cancel();
			Assert.IsTrue(pr.WasCanceled);
		}
	}
}
