using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace SLaks.Progression.Display {
	///<summary>A WPF ViewModel that reports progress.</summary>
	public class ProgressModel : IProgressReporter, INotifyPropertyChanged {
		static readonly CancellationCommand generalCancelCommand = new CancellationCommand(null);
		///<summary>Gets a WPF ICommand instance that cancels the ProgressModel passed as its parameter.</summary>
		public static ICommand GeneralCancelCommand { get { return generalCancelCommand; } }

		readonly CancellationCommand cancelCommand;
		///<summary>Gets a WPF ICommand that cancels this ProgressModel instance.</summary>
		public ICommand CancelCommand { get { return cancelCommand; } }

		///<summary>Creates a new ProgressModel instance.</summary>
		public ProgressModel() {
			cancelCommand = new CancellationCommand(this);
		}

		class CancellationCommand : ICommand {
			readonly ProgressModel fixedTarget;
			public CancellationCommand(ProgressModel target) { fixedTarget = target; }

			public bool CanExecute(object parameter) {
				var target = parameter as ProgressModel ?? fixedTarget;
				return target != null && target.AllowCancellation;
			}

			public event EventHandler CanExecuteChanged;
			public void RaiseChanged() {
				if (CanExecuteChanged != null)
					CanExecuteChanged(this, EventArgs.Empty);
			}

			public void Execute(object parameter) {
				var target = parameter as ProgressModel ?? fixedTarget;
				if (target != null)
					target.WasCanceled = true;
			}
		}

		string caption;
		///<summary>Gets or sets a string describing the current operation.</summary>
		public string Caption {
			get { return caption; }
			set { caption = value; OnPropertyChanged("Caption"); }
		}

		long maximum = 100;
		long? progress = 0;
		///<summary>Gets or sets the progress value at which the operation will be completed.</summary>
		public virtual long Maximum {
			get { return maximum; }
			set {
				if (value <= 0)
					throw new ArgumentOutOfRangeException("value");

				maximum = value;
				Progress = 0;
				OnPropertyChanged("Maximum");
				OnPropertyChanged("ScaledMaximum");
				OnPropertyChanged("ScaledProgress");
			}
		}

		///<summary>Gets or sets the current progress, between 0 and Maximum.</summary>
		public virtual long? Progress {
			get { return progress; }
			set {
				if (progress == value) return;
				if (value < 0 || value > Maximum)
					throw new ArgumentOutOfRangeException("value", "Progress must be between 0 and " + Maximum);

				bool wasIndeterminate = IsIndeterminate;
				double oldScaledProgress = ScaledProgress;

				progress = value;
				OnPropertyChanged("Progress");
				if (wasIndeterminate != IsIndeterminate)
					OnPropertyChanged("IsIndeterminate");
				if (oldScaledProgress != ScaledProgress)
					OnPropertyChanged("ScaledProgress");
			}
		}

		///<summary>Indicates whether the operation has a definite progress.</summary>
		public bool IsIndeterminate { get { return !Progress.HasValue; } }

		double scaledLimit;
		///<summary>Gets or sets a maximum value to scale the progress to.</summary>
		public double ScaledMaximum {
			get { return Math.Min(Maximum, scaledLimit); }
			set {
				scaledLimit = value;
				OnPropertyChanged("ScaledMaximum");
				OnPropertyChanged("ScaledProgress");
			}
		}
		///<summary>Gets the current progress of the operation, scaled to ScaledMaximum.</summary>
		public double ScaledProgress {
			get {
				if (Progress == null) return 0;
				if (Maximum <= scaledLimit) return Progress.Value;
				return ((double)Progress / Maximum) * ScaledMaximum;
			}
		}
		
		bool allowCancellation;
		bool wasCanceled;
		///<summary>Gets or sets whether this operation can be canceled by the user.</summary>
		public bool AllowCancellation {
			get { return allowCancellation; }
			set {
				if (AllowCancellation == value) return;
				wasCanceled = false;
				allowCancellation = value;
				OnPropertyChanged("AllowCancellation");
				OnPropertyChanged("WasCanceled");

				cancelCommand.RaiseChanged();
				generalCancelCommand.RaiseChanged();
			}
		}

		///<summary>Indicates whether the user has cancelled the operation.</summary>
		public bool WasCanceled {
			get { return wasCanceled; }
			set {
				if (!AllowCancellation) throw new InvalidOperationException();
				wasCanceled = value;
				OnPropertyChanged("WasCanceled");
			}
		}

		///<summary>Occurs when a property value is changed.</summary>
		public event PropertyChangedEventHandler PropertyChanged;
		///<summary>Raises the PropertyChanged event.</summary>
		///<param name="name">The name of the property that changed.</param>
		protected virtual void OnPropertyChanged(string name) { OnPropertyChanged(new PropertyChangedEventArgs(name)); }
		///<summary>Raises the PropertyChanged event.</summary>
		///<param name="e">An EventArgs object that provides the event data.</param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}
