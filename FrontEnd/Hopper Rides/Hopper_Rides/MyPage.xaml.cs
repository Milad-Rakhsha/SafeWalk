using System;
using Xamarin.Forms;
using System.Diagnostics;


namespace Hopper_Rides
{
	public partial class MyPage : ContentPage
	{
		public MyPage()
		{
			InitializeComponent();
		}
		void LoginClick(object sender, EventArgs args)
		{
			Button btncontrol = (Button)sender;
			string providername = btncontrol.Text;

			if (OAuthConfig.User == null && !String.IsNullOrEmpty(UserPhoneNumber.Text))
			{
				Debug.WriteLine("You have chosen  " + providername + " to log in");
				Navigation.PushModalAsync(new ProviderPage(providername,UserPhoneNumber.Text));
			}

			if (String.IsNullOrEmpty(UserPhoneNumber.Text)){
                DisplayAlert("Alert", "Please enter your phone number for the driver to contact you", "OK");

			}

		}

	}
}