using System;
using Xamarin.Forms;

namespace Renderers
{
	public class ExtendedEntry : Entry
	{
		
		public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create("BorderWidth", typeof(double), typeof(ExtendedEntry), -1d);

		public static readonly BindableProperty BorderColorProperty = BindableProperty.Create("BorderColor", typeof(Color), typeof(ExtendedEntry), Color.Default);

		public static readonly BindableProperty BorderRadiusProperty = BindableProperty.Create("BorderRadius", typeof(int), typeof(ExtendedEntry), 5);

		public Color BorderColor
		{
			get
			{
				return (Color)base.GetValue(ExtendedEntry.BorderColorProperty);
			}
			set
			{
				base.SetValue(ExtendedEntry.BorderColorProperty, value);
			}
		}

		public int BorderRadius
		{
			get
			{
				return (int)base.GetValue(ExtendedEntry.BorderRadiusProperty);
			}
			set
			{
				base.SetValue(ExtendedEntry.BorderRadiusProperty, value);
			}
		}

		public double BorderWidth
		{
			get
			{
				return (double)base.GetValue(ExtendedEntry.BorderWidthProperty);
			}
			set
			{
				base.SetValue(ExtendedEntry.BorderWidthProperty, value);
			}
		}

		public ExtendedEntry()
		{
			
		}

	}
}
