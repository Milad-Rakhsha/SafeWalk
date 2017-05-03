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
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                string providername = "FaceBook";

                if (OAuthConfig.User == null && !String.IsNullOrEmpty(UserPhoneNumber.Text))
                {
                    Debug.WriteLine("You have chosen  " + providername + " to log in");
                    Navigation.PushModalAsync(new ProviderPage(providername, UserPhoneNumber.Text));
                }

                if (String.IsNullOrEmpty(UserPhoneNumber.Text))
                {
                    DisplayAlert("Alert", "Please enter your phone number for the driver to contact you", "OK");

                }
            };
            facebook.GestureRecognizers.Add(tap);
		}
	}
}