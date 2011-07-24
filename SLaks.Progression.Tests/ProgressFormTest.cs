using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SLaks.Progression.Display.WinForms;
using System.Windows.Forms;
using System.Reflection;

namespace SLaks.Progression.Tests {
	[TestClass]
	public class ProgressFormTest : ProgressReporterTestBase {
		protected override IProgressReporter CreateReporter() {
			return new ProgressForm();
		}

		public override bool SupportsCancellation { get { return true; } }

		[TestMethod]
		public void CancellationWorks() {
			using (var f = new ProgressForm()) {
				f.Show();	//Necessary to make Visible work

				var cancelButton = (Button)f.Controls.Find("cancelButton", true)[0];
				Assert.IsFalse(cancelButton.Visible);

				f.AllowCancellation = true;
				Assert.IsTrue(cancelButton.Visible);
				Assert.IsFalse(f.WasCanceled);
				cancelButton.PerformClick();
				Assert.IsTrue(f.WasCanceled);
			}
		}

		[TestMethod]
		public void ExecuteReturnsCancellation() {
			Assert.IsTrue(ProgressForm.Execute(pr => { }));
			Assert.IsFalse(ProgressForm.Execute(pr => {
				pr.AllowCancellation = true;

				//I need to wait for the form to be shown by 
				//the UI thread before calling PerformClick()
				System.Threading.Thread.Sleep(300);	

				var f = (ProgressForm)pr;
				var cancelButton = (Button)f.Controls.Find("cancelButton", true)[0];
			 	cancelButton.PerformClick();
			}));
		}
		[TestMethod]
		[ExpectedException(typeof(TargetInvocationException))]
		public void ExecuteForwardsExceptions() {
			ProgressForm.Execute(pr => { throw new Exception(); });
		}
	}
}
