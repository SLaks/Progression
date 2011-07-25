using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using SLaks.Progression.Display;

namespace SLaks.Progression.Tests {
	[TestClass]
	public class BackgroundWorkerReporterTest : ProgressReporterTestBase {
		protected override IProgressReporter CreateReporter() {
			return new BackgroundWorkerReporter(new BackgroundWorker());
		}

		public override bool SupportsCancellation { get { return true; } }

		[TestMethod]
		public void ProgressWorks() {
			var bwr = new BackgroundWorkerReporter(new BackgroundWorker());

			int workerProgress = -1;
			bwr.Worker.ProgressChanged += (s, e) => { workerProgress = e.ProgressPercentage; };

			bwr.Maximum = 50;
			bwr.Progress = 10;

			Assert.AreEqual(20, workerProgress);
		}

		[TestMethod]
		public void CancellationWorks() {
			var bwr = new BackgroundWorkerReporter(new BackgroundWorker());
			bwr.AllowCancellation = true;
			bwr.Worker.CancelAsync();
			Assert.IsTrue(bwr.WasCanceled);
		}
	}
}
