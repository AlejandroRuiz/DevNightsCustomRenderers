using System;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Hardware;
using Android.Views;
using Android.Widget;
using Renderers;
using Renderers.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MyAwesomeView), typeof(MyAwesomeViewRenderer))]
namespace Renderers.Droid
{
	public class MyAwesomeViewRenderer : ViewRenderer<MyAwesomeView, Android.Views.View>, TextureView.ISurfaceTextureListener, Android.Hardware.Camera.IPictureCallback
	{

		Android.Hardware.Camera camera;
		TextureView liveCameraStream;
		Android.Widget.Button takePhotoButton;

		public MyAwesomeViewRenderer()
		{
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Control == null || Element == null)
				return;

			if (e.PropertyName == MyAwesomeView.TorchOnProperty.PropertyName)
			{
				SwitchTorchStatus();
			}
			else if (e.PropertyName == MyAwesomeView.CameraTypeProperty.PropertyName)
			{
				SwitchCameraType();
			}
			else if (e.PropertyName == MyAwesomeView.ButtonColorProperty.PropertyName)
			{
				SwitchButtonColor();
			}
		}

		#region Renderer Life Cycle

		protected override void OnElementChanged(ElementChangedEventArgs<MyAwesomeView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				takePhotoButton = new Android.Widget.Button(this.Context);
				var layoutparams = new Android.Widget.RelativeLayout.LayoutParams(150, 150);
				layoutparams.AddRule(LayoutRules.AlignParentBottom);
				layoutparams.AddRule(LayoutRules.CenterHorizontal);
				layoutparams.BottomMargin = 50;
				takePhotoButton.LayoutParameters = layoutparams;
				takePhotoButton.Click += TakePhotoButton_Click;
				SwitchButtonColor();
				liveCameraStream = new TextureView(this.Context);
				liveCameraStream.SurfaceTextureListener = this;

				var mainView = new Android.Widget.RelativeLayout(this.Context);
				mainView.AddView(liveCameraStream);
				mainView.AddView(takePhotoButton);

				SetNativeControl(mainView);
			}

			if (Control == null || Element == null)
				return;

			if (e.OldElement != null)
			{
				Element.RequestCameraPermissionTask = null;
				Element.StartSessionAction = null;
			}

			if (e.NewElement != null)
			{
				Element.RequestCameraPermissionTask = AuthorizeCameraUse();
				Element.StartSessionAction = SetupLiveCameraRear;
			}
		}

		#endregion

		void SwitchTorchStatus()
		{
			if (!Xamarin.Forms.Forms.Context.PackageManager.HasSystemFeature(PackageManager.FeatureCameraFlash))
				return;

			var param = camera.GetParameters();
			if (Element.TorchOn)
			{
				if (param.FlashMode == Android.Hardware.Camera.Parameters.FlashModeTorch)
					return;
				param.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
			}
			else
			{
				if (param.FlashMode == Android.Hardware.Camera.Parameters.FlashModeOff)
					return;
				param.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
			}
			camera.SetParameters(param);
		}

		void SwitchCameraType()
		{
			if (Android.Hardware.Camera.NumberOfCameras != 2)
				return;
			camera.StopPreview();
			camera.Release();

			if (Element.CameraType == CameraType.Rear)
				SetupLiveCameraStream(0);
			else
				SetupLiveCameraStream(1);
		}

		void SwitchButtonColor()
		{
			ShapeDrawable drawable = new ShapeDrawable(new OvalShape());
			drawable.Paint.Color = Element.ButtonColor.ToAndroid();
			takePhotoButton.Background = drawable;
		}

		async Task<bool> AuthorizeCameraUse()
		{
			return true;
		}

		void TakePhotoButton_Click(object sender, EventArgs e)
		{
			camera.TakePicture(null, null, this);
		}

		public void SetupLiveCameraRear()
		{
			IsInitRequested = true;
			SetupLiveCameraStream();
		}

		public void SetupLiveCameraStream(int id = 0)
		{
			lock(lockObj)
			{
				if (surface == null)
					return;
				try
				{
					camera = Android.Hardware.Camera.Open(id);
					camera.SetPreviewTexture(surface);
					camera.StartPreview();
				}
				catch (Java.IO.IOException ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		private bool IsInitRequested { get; set; }

		static object lockObj = new object();

		#region TextureView.ISurfaceTextureListener

		public SurfaceTexture surface;

		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
		{
			this.surface = surface;
			liveCameraStream.LayoutParameters = new Android.Widget.RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
			if (IsInitRequested)
			{
				IsInitRequested = false;
				SetupLiveCameraStream();
			}
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			camera.StopPreview();
			camera.Release();

			return true;
		}

		public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
		{
			//throw new NotImplementedException();
		}

		public void OnSurfaceTextureUpdated(SurfaceTexture surface)
		{
			//throw new NotImplementedException();
		}

		#endregion

		#region Android.Hardware.Camera.IPictureCallback

		public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
		{
			var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempPath() + ".jpg");
			System.IO.File.WriteAllBytes(path, data);
			Element.SendOnPhotoTaken(path);
			camera.StartPreview();
		}

		#endregion
	}
}
