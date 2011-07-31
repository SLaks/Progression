using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SLaks.Progression.Tests {
	///<summary>Tests INotifyPropertyChanged implementations.</summary>
	public static class PropertyChangedVerifier {
		///<summary>Tests that PropertyChanged events are raised correctly.</summary>
		///<param name="instance">The instance to verify with.</param>
		///<param name="propertySetters">A set of lambda expressions that each change a single property.</param>
		public static void TestPropertyChanged<T>(this T instance, params Action<T>[] propertySetters) where T : INotifyPropertyChanged {
			if (instance == null) throw new ArgumentNullException("instance");
			if (propertySetters == null) throw new ArgumentNullException("propertySetters");

			var changedProperties = new HashSet<string>();
			instance.PropertyChanged += (s, e) => {
				Assert.AreEqual(instance, s, "PropertyChanged has wrong sender");
				changedProperties.Add(e.PropertyName);
			};
			var currentVals = PropertyGetters<T>.GetValues(instance);

			Assert.IsTrue(currentVals.Count > 0, typeof(T) + " has no public properties");

			foreach (var prop in propertySetters) {
				changedProperties.Clear();
				var priorVals = currentVals;
				prop(instance);
				currentVals = PropertyGetters<T>.GetValues(instance);

				var changes = from p in priorVals
							  join c in currentVals on p.Key equals c.Key
							  where !changedProperties.Contains(p.Key)
							  select new { Name = p.Key, OldValue = p.Value, NewValue = c.Value };

				foreach (var c in changes) {
					Assert.AreEqual(c.OldValue, c.NewValue, typeof(T) + ".PropertyChanged didn't fire for " + c.Name);
				}
			}
		}

		static class PropertyGetters<T> {
			public static Dictionary<string, object> GetValues(T instance) {
				return getters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value(instance));
			}

			static readonly Dictionary<string, Func<T, object>> getters = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty).ToDictionary(p => p.Name, GetGetter);

			static Func<T, object> GetGetter(PropertyInfo prop) {
				if (prop.PropertyType.IsValueType)
					return o => prop.GetValue(o, null);

				return (Func<T, object>)Delegate.CreateDelegate(typeof(Func<T, object>), prop.GetGetMethod());
			}
		}
	}
}
