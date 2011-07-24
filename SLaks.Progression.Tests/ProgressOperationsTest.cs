using SLaks.Progression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace SLaks.Progression.Tests {
	[TestClass]
	public class ProgressOperationsTest {

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		static readonly Random rand = new Random();
		[TestMethod]
		public void CopyToCopiesData() {
			var bytes = new byte[rand.Next(65536, 1048576)];
			rand.NextBytes(bytes);

			var source = new MemoryStream(bytes);
			var target = new MemoryStream();

			ProgressOperations.CopyTo(source, target, null);
			CollectionAssert.AreEqual(bytes, target.ToArray());
		}

		[TestMethod]
		public void ProgressBarIsFilled() {
			var source = new MemoryStream();
			source.SetLength(rand.Next(65536, 1048576));

			var pr = new EmptyProgressReporter();
			ProgressOperations.CopyTo(source, Stream.Null, pr);
			Assert.AreEqual(source.Length, pr.Maximum);
			Assert.AreEqual(pr.Maximum, pr.Progress);
		}
	}
}
