using System.Collections.Generic;
using Xamarin.Forms;

namespace Renderers
{
	public partial class RenderersPage : ContentPage
	{
		void Handle_OnPhotoTaken(object sender, string e)
		{
			//_photoView.Source = e;
		}

		void Camera_Handle_Clicked(object sender, System.EventArgs e)
		{
			//_cameraPreview.CameraType = (_cameraPreview.CameraType == CameraType.Front) ? CameraType.Rear : CameraType.Front;
		}

		void Torch_Handle_Clicked(object sender, System.EventArgs e)
		{
			//_cameraPreview.TorchOn = !_cameraPreview.TorchOn;
		}

		public RenderersPage()
		{
			InitializeComponent();
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			//if (await _cameraPreview.RequestCameraPermissionTask)
			//{
			//	_cameraPreview.StartSession();
			//}
			//else
			//{
			//	await DisplayAlert("Error", "Camera Permissions", "Ok");
			//}
		}
	}
}
