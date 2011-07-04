using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SLaks.Progression.Tests {
	///<summary>Tests an IProgressReporter implementation to ensure that it fully conforms to the interface spec.</summary>
	[TestClass]
	public abstract class ProgressReporterTestBase {
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		///<summary>Creates the IProgressReporter implementation that is tested.</summary>
		protected abstract IProgressReporter CreateReporter();

		///<summary>Indicates whether the reporter under test allows the user to cancel the operation.</summary>
		public abstract bool SupportsCancellation { get; }

		[TestMethod]
		public void DefaultSettings() {
			var pr = CreateReporter();
			Assert.AreEqual(false, pr.AllowCancellation);
			Assert.AreEqual(100, pr.Maximum);
			Assert.AreEqual(0, pr.Progress);
			Assert.AreEqual(false, pr.WasCanceled);
		}

		[TestMethod]
		public void CancellationBehavior() {
			var pr = CreateReporter();

			pr.AllowCancellation = true;
			Assert.AreEqual(SupportsCancellation, pr.AllowCancellation);
			Assert.AreEqual(false, pr.WasCanceled);
		}

		[TestMethod]
		public void MaximumResetsProgress() {
			var pr = CreateReporter();

			pr.Progress = 50;
			pr.Maximum = 78;
			Assert.AreEqual(0, pr.Progress, "Setting Maximum should reset progress to 0");
		}

		[TestMethod]
		public void IndeterminateWorks() {
			var pr = CreateReporter();
			pr.Progress = null;
			Assert.IsNull(pr.Progress);
		}

		#region Exceptions
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void MaximumCantBe0() {
			CreateReporter().Maximum = 0;
		}
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void MaximumCantBeNegative() {
			CreateReporter().Maximum = -1;
		}
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ProgressCantBeNegative() {
			CreateReporter().Progress = -1;
		}
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ProgressCantBeTooHigh() {
			CreateReporter().Progress = 101;
		}
		#endregion
	}
}
