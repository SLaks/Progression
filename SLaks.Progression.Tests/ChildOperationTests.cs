﻿using System;
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

			child.Maximum = 3;
			Assert.AreEqual(150, parent.Progress, "Resetting child should reset Parent.Progress to base");
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

			child.Maximum = 3;
			Assert.AreEqual(150, parent.Progress, "Resetting child should reset Parent.Progress to base");
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