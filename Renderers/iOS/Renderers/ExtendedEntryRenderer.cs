using System;
using Renderers;
using Renderers.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]
namespace Renderers.iOS
{
	public class ExtendedEntryRenderer : EntryRenderer
	{
		public new ExtendedEntry Element {
			get{
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
				Control.BorderStyle = UIKit.UITextBorderStyle.RoundedRect;
				RemoveBorderColor();
				RemoveBorderRadius();
				RemoveBorderWidth();
			}

			if (e.NewElement != null)
			{
				Control.BorderStyle = UIKit.UITextBorderStyle.None;
				ApplyBorderColor();
				ApplyBorderRadius();
				ApplyBorderWidth();
			}
		}

		#endregion

		void ApplyBorderColor()
		{
			Control.Layer.BorderColor = Element.BorderColor.ToCGColor();
		}

		void ApplyBorderRadius()
		{
			Control.Layer.CornerRadius = Element.BorderRadius;
		}

		void ApplyBorderWidth()
		{
			Control.Layer.BorderWidth = (nfloat)Element.BorderWidth;
		}

		void RemoveBorderColor()
		{
			Control.Layer.BorderColor = Color.Default.ToCGColor();
		}

		void RemoveBorderRadius()
		{
			Control.Layer.CornerRadius = 0;
		}

		void RemoveBorderWidth()
		{
			Control.Layer.BorderWidth = 0;
		}
	}
}
