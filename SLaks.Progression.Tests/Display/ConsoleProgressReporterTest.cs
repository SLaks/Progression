using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SLaks.Progression.Display;

namespace SLaks.Progression.Tests.Display {
	[TestClass]
	public class ConsoleProgressReporterTest : ProgressReporterTestBase {
		protected override IProgressReporter CreateReporter() {
			return new ConsoleProgressReporter(true);
		}

		public override bool SupportsCancellation { get { return true; } }

		//I'm not sure how to unit-test the actual rendering
	}
}
