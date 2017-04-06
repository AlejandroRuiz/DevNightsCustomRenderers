using System;
using System.IO;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using Foundation;
using Renderers;
using Renderers.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MyAwesomeView), typeof(MyAwesomeViewRenderer))]
namespace Renderers.iOS
{
	public class MyAwesomeViewRenderer : ViewRenderer<MyAwesomeView, UIView>
	{
		bool torchOn = false;

		AVCaptureSession captureSession;
		AVCaptureDeviceInput captureDeviceInput;
		AVCaptureStillImageOutput stillImageOutput;
		AVCaptureVideoPreviewLayer videoPreviewLayer;

		UIView liveCameraStream;

		UIButton takePhotoButton;

		public MyAwesomeViewRenderer()
		{
			
		}

		#region Renderer Life Cycle

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

		protected override void OnElementChanged(ElementChangedEventArgs<MyAwesomeView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				liveCameraStream = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - 60));
				takePhotoButton = new UIButton(UIButtonType.Custom);
				takePhotoButton.Frame = new CGRect((liveCameraStream.Frame.Width / 2) - (30 / 2), liveCameraStream.Frame.Height - (20 + 50), 50, 50);
				takePhotoButton.TouchUpInside += TakePhotoButton_TouchUpInside;
				takePhotoButton.Layer.CornerRadius = takePhotoButton.Bounds.Size.Width / 2;
				SwitchButtonColor();
				var mainView = new UIView(liveCameraStream.Frame);
				mainView.Add(liveCameraStream);
				mainView.Add(takePhotoButton);


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
				Element.StartSessionAction = SetupLiveCameraStream;
				SwitchButtonColor();
			}
		}

		#endregion

		async void TakePhotoButton_TouchUpInside(object sender, EventArgs e)
		{
			var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
			var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);

			var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);

			var path = Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempPath() + ".jpg");
			NSError err;
			jpegImageAsNsData.Save(path, false, out err);
			if (err != null)
				path = "";
			Element.SendOnPhotoTaken(path);
		}

		void SwitchButtonColor()
		{
			takePhotoButton.BackgroundColor = Element.ButtonColor.ToUIColor();
		}

		void SwitchCameraType()
		{
			var devicePosition = captureDeviceInput.Device.Position;
			if (devicePosition == AVCaptureDevicePosition.Front)
			{
				devicePosition = AVCaptureDevicePosition.Back;
			}
			else {
				devicePosition = AVCaptureDevicePosition.Front;
			}

			if ((Element.CameraType == CameraType.Front && devicePosition == AVCaptureDevicePosition.Front) || (Element.CameraType == CameraType.Rear && devicePosition == AVCaptureDevicePosition.Back))
				return;

			var device = GetCameraForOrientation(devicePosition);
			ConfigureCameraForDevice(device);

			captureSession.BeginConfiguration();
			captureSession.RemoveInput(captureDeviceInput);
			captureDeviceInput = AVCaptureDeviceInput.FromDevice(device);
			captureSession.AddInput(captureDeviceInput);
			captureSession.CommitConfiguration();
		}

		void SwitchTorchStatus()
		{
			var device = captureDeviceInput.Device;

			if (!device.IsTorchModeSupported(AVCaptureTorchMode.On))
				return;

			var error = new NSError();
			if (device.HasFlash)
			{
				if (device.TorchMode == AVCaptureTorchMode.On)
				{
					if (Element.TorchOn)
						return;
					device.LockForConfiguration(out error);
					device.TorchMode = AVCaptureTorchMode.Off;
					device.UnlockForConfiguration();
				}
				else {
					if (!Element.TorchOn)
						return;
					device.LockForConfiguration(out error);
					device.TorchMode = AVCaptureTorchMode.On;
					device.UnlockForConfiguration();
				}
			}

			torchOn = !torchOn;
		}

		async Task<bool> AuthorizeCameraUse()
		{
			var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

			if (authorizationStatus != AVAuthorizationStatus.Authorized)
			{
				return await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
			}
			return true;
		}

		public void SetupLiveCameraStream()
		{
			captureSession = new AVCaptureSession();

			var viewLayer = liveCameraStream.Layer;
			videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
			{
				Frame = liveCameraStream.Frame
			};
			liveCameraStream.Layer.AddSublayer(videoPreviewLayer);

			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
			ConfigureCameraForDevice(captureDevice);
			captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);
			captureSession.AddInput(captureDeviceInput);

			var dictionary = new NSMutableDictionary();
			dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
			stillImageOutput = new AVCaptureStillImageOutput()
			{
				OutputSettings = new NSDictionary()
			};

			captureSession.AddOutput(stillImageOutput);
			captureSession.StartRunning();
		}

		void ConfigureCameraForDevice(AVCaptureDevice device)
		{
			var error = new NSError();
			if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
			{
				device.LockForConfiguration(out error);
				device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
				device.UnlockForConfiguration();
			}
			else if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
			{
				device.LockForConfiguration(out error);
				device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
				device.UnlockForConfiguration();
			}
			else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
			{
				device.LockForConfiguration(out error);
				device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
				device.UnlockForConfiguration();
			}
		}

		AVCaptureDevice GetCameraForOrientation(AVCaptureDevicePosition orientation)
		{
			var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);

			foreach (var device in devices)
			{
				if (device.Position == orientation)
				{
					return device;
				}
			}

			return null;
		}
	}
}
