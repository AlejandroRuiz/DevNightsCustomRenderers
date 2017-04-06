using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Renderers
{
	public enum CameraType
	{
		Front,
		Rear
	}

	public class MyAwesomeView : View
	{

		public static readonly BindableProperty TorchOnProperty = BindableProperty.Create("TorchOn", typeof(bool), typeof(MyAwesomeView), false);

		public static readonly BindableProperty CameraTypeProperty = BindableProperty.Create("CameraType", typeof(CameraType), typeof(MyAwesomeView), CameraType.Rear);

		public static readonly BindableProperty ButtonColorProperty = BindableProperty.Create("ButtonColor", typeof(Color), typeof(MyAwesomeView), Color.White);

		public bool TorchOn
		{
			get
			{
				return (bool)base.GetValue(MyAwesomeView.TorchOnProperty);
			}
			set
			{
				base.SetValue(MyAwesomeView.TorchOnProperty, value);
			}
		}

		public CameraType CameraType
		{
			get
			{
				return (CameraType)base.GetValue(MyAwesomeView.CameraTypeProperty);
			}
			set
			{
				base.SetValue(MyAwesomeView.CameraTypeProperty, value);
			}
		}

		public Color ButtonColor
		{
			get
			{
				return (Color)base.GetValue(MyAwesomeView.ButtonColorProperty);
			}
			set
			{
				base.SetValue(MyAwesomeView.ButtonColorProperty, value);
			}
		}

		public event EventHandler<string> OnPhotoTaken;

		public void SendOnPhotoTaken(string path)
		{
			OnPhotoTaken?.Invoke(this, path);
		}

		public MyAwesomeView()
		{
		}

		public void StartSession()
		{
			StartSessionAction?.Invoke();
		}

		#region Native Calls

		public Task<bool> RequestCameraPermissionTask;
		public Action StartSessionAction;

		#endregion
	}
}
