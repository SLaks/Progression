using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLaks.Progression.Display {
	///<summary>A base class for a progress reporter that scales progress values to a fixed maximum.</summary>
	public abstract class ScaledProgressReporter {
		long? progress = 0;
		long maximum = 100;

		///<summary>Gets the maximum that the progress bar's value will be scaled to.</summary>
		protected abstract int ScaledMax { get; }
		///<summary>Gets the value of the progress bar, scaled to ScaledMax.</summary>
		protected int ScaledValue { get; private set; }

		private void Update() {
			int? oldValue = ScaledValue;
			int? newValue;
			if (Maximum == 0)
				newValue = 0;
			else
				newValue = (int?)((double?)Progress / Maximum * ScaledMax);

			if (newValue == oldValue) return;

			ScaledValue = newValue ?? -1;
			UpdateBar(oldValue, newValue);
		}
		///<summary>Draws an updated progress bar.</summary>
		protected abstract void UpdateBar(int? oldValue, int? newValue);

		///<summary>Gets or sets the progress value at which the operation will be completed.</summary>
		///<remarks>Setting this property will reset Progress to 0.</remarks>
		public virtual long Maximum {
			get { return maximum; }
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException("value");

				maximum = value;
				if (Progress > value) progress = value;
				Update();
			}
		}

		///<summary>Gets or sets the current progress, between 0 and Maximum, or null to display a marquee.</summary>
		public virtual long? Progress {
			get { return progress; }
			set {
				if (value < 0 || value > Maximum)
					throw new ArgumentOutOfRangeException("value", "Progress must be between 0 and " + Maximum);
				progress = value;
				Update();
			}
		}
	}
}
