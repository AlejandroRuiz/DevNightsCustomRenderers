using System;
using Android.Graphics.Drawables;
using Renderers;
using Renderers.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]
namespace Renderers.Droid
{
	public class ExtendedEntryRenderer : EntryRenderer
	{
		public new ExtendedEntry Element
		{
			get
			{
				return base.Element as ExtendedEntry;
			}
		}

		public ExtendedEntryRenderer()
		{
		}

		#region Renderer Life Cycle

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Control == null || Element == null)
				return;

			if (e.PropertyName == ExtendedEntry.BorderColorProperty.PropertyName)
			{
				ApplyBorderColor();
			}
			else if (e.PropertyName == ExtendedEntry.BorderRadiusProperty.PropertyName)
			{
				ApplyBorderRadius();
			}
			else if (e.PropertyName == ExtendedEntry.BorderWidthProperty.PropertyName)
			{
				ApplyBorderWidth();
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (Control == null || Element == null)
				return;

			if (e.OldElement != null)
			{
				ClearBackground();
			}

			if (e.NewElement != null)
			{
				BuildBackground();
			}
		}

		#endregion

		void ApplyBorderColor()
		{
			BuildBackground();
		}

		void ApplyBorderRadius()
		{
			BuildBackground();
		}

		void ApplyBorderWidth()
		{
			BuildBackground();
		}

		void BuildBackground()
		{
			GradientDrawable gd = new GradientDrawable();
			gd.SetColor(Element.BackgroundColor.ToAndroid().ToArgb());
			gd.SetCornerRadius(Element.BorderRadius);
			gd.SetStroke((int)Element.BorderWidth, Element.BorderColor.ToAndroid());
			Control.Background = gd;
		}

		void ClearBackground()
		{
			Control.Background = new ColorDrawable(Element.BackgroundColor.ToAndroid());
		}
	}
}
