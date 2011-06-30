﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLaks.ProgressReporting.Display {
	///<summary>A base class for a progress reporter that scales progress values to a fixed maximum.</summary>
	public abstract class ScaledProgressReporter {
		long progress = 0, maximum = 100;

		///<summary>Gets the maximum that the progress bar's value will be scaled to.</summary>
		protected abstract int ScaledMax { get; }
		///<summary>Gets the value of the progress bar, scaled to ScaledMax.</summary>
		protected int ScaledValue { get; private set; }

		private void Update() {
			int oldValue = ScaledValue;
			int newValue = Maximum < 0 ? -1 : (int)((double)Progress / Maximum * ScaledMax);
			if (newValue == oldValue) return;

			ScaledValue = newValue;
			UpdateBar(oldValue, newValue);
		}
		///<summary>Draws an updated progress bar.</summary>
		protected abstract void UpdateBar(int oldValue, int newValue);

		///<summary>Gets or sets the progress value at which the operation will be completed, or -1 to display a marquee.</summary>
		public virtual long Maximum {
			get { return maximum; }
			set {
				if (value < 0)
					value = -1;
				else if (value == 0)
					throw new ArgumentOutOfRangeException("value");

				maximum = value;
				value = 0;
				Update();
			}
		}

		///<summary>Gets or sets the current progress, between 0 and Maximum.</summary>
		public virtual long Progress {
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
