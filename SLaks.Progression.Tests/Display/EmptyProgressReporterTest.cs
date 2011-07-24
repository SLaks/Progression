using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SLaks.Progression.Tests {
	[TestClass]
	public class EmptyProgressReporterTest : ProgressReporterTestBase {
		protected override IProgressReporter CreateReporter() {
			return new EmptyProgressReporter();
		}

		public override bool SupportsCancellation { get { return false; } }
	}
}
