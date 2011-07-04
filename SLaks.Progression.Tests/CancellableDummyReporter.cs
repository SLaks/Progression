using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SLaks.Progression.Tests {
	class CancellableDummyReporter : EmptyProgressReporter, IProgressReporter {
		bool allowCancellation, wasCanceled;

		public bool AllowCancellation {
			get { return allowCancellation; }
			set { allowCancellation = value; wasCanceled = false; }
		}

		public bool WasCanceled { get { return wasCanceled; } }

		public void Cancel() {
			if (!AllowCancellation) throw new InvalidOperationException();
			wasCanceled = true;
		}
	}
	[TestClass]
	public class CancellableDummyReporterTest : ProgressReporterTestBase {
		protected override IProgressReporter CreateReporter() { return new CancellableDummyReporter(); }

		public override bool SupportsCancellation { get { return true; } }
	}
}
