using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SLaks.Progression.Tests {
	[TestClass]
	public class UnscaledChildOperationTest : ProgressReporterTestBase {
		protected override IProgressReporter CreateReporter() {
			return new CancellableDummyReporter { AllowCancellation = true }.ChildOperation();
		}

		[TestMethod]
		public void SupportsNullParent() {
			Assert.IsInstanceOfType(ProgressReporterExtensions.ChildOperation(null), typeof(EmptyProgressReporter));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CantStartWithIndeterminate() {
			new EmptyProgressReporter { Progress = null }.ChildOperation();
		}

		public override bool SupportsCancellation { get { return true; } }
		[TestMethod]
		public void ParentCaptionIsUpdated() {
			var parent = new EmptyProgressReporter { Caption = "Parent in progress" };
			var child = parent.ChildOperation();
			child.Caption = "Child in progress";
			Assert.AreEqual(child.Caption, parent.Caption);
		}
		[TestMethod]
		public void ParentProgressIsUpdated() {
			var parent = new EmptyProgressReporter { Maximum = 400, Progress = 150 };
			var child = parent.ChildOperation();

			child.Maximum = 700;
			child.Progress = 150;

			Assert.AreEqual(300, parent.Progress, "Parent.Progress == baseProgress + Child.Progress");

			child.Maximum = 50;
			Assert.AreEqual(200, parent.Progress, "Clipping the child's progress through its maximum should update the parent's progress");
		}
		[TestMethod]
		public void MarqueeIsIgnored() {
			var parent = new EmptyProgressReporter { Maximum = 400, Progress = 150 };
			var child = parent.ChildOperation();

			child.Progress = 50;
			child.Progress = null;

			Assert.AreEqual(200, parent.Progress);
		}
	}
	[TestClass]
	public class ScaledChildOperationTest : ProgressReporterTestBase {
		protected override IProgressReporter CreateReporter() {
			return new CancellableDummyReporter { AllowCancellation = true }.ScaledChildOperation(50);
		}

		[TestMethod]
		public void SupportsNullParent() {
			Assert.IsInstanceOfType(ProgressReporterExtensions.ScaledChildOperation(null, 2), typeof(EmptyProgressReporter));
		}
		
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CantStartWithIndeterminate() {
			new EmptyProgressReporter { Progress = null }.ScaledChildOperation(2);
		}
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RangeMustBePositive() {
			new EmptyProgressReporter().ScaledChildOperation(0);
		}

		public override bool SupportsCancellation { get { return true; } }
		[TestMethod]
		public void ParentCaptionIsUpdated() {
			var parent = new EmptyProgressReporter { Caption = "Parent in progress" };
			var child = parent.ChildOperation();
			child.Caption = "Child in progress";
			Assert.AreEqual(child.Caption, parent.Caption);
		}
		[TestMethod]
		public void ParentProgressIsUpdated() {
			var parent = new EmptyProgressReporter { Maximum = 400, Progress = 150 };
			var child = parent.ScaledChildOperation(200);

			child.Maximum = 300;
			child.Progress = 150;

			Assert.AreEqual(250, parent.Progress, "Parent.Progress == baseProgress + Child.Progress");

			child.Maximum = 50;
			Assert.AreEqual(350, parent.Progress, "Clipping the child's progress through its maximum should fill the progress range");
		}
		[TestMethod]
		public void MarqueeIsIgnored() {
			var parent = new EmptyProgressReporter { Maximum = 400, Progress = 150 };
			var child = parent.ScaledChildOperation(200);

			child.Progress = 50;
			child.Progress = null;

			Assert.AreEqual(250, parent.Progress);
		}
	}
}
