using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Hopper_Rides
{
	public partial class App : Application
	{
		public static int riderID = 0;
		public static int activeReqID = 0;
		public App()
		{
			InitializeComponent();
			//MainPage MAIN_PAGE = new Hopper_Rides.MainPage()

			MainPage = new NavigationPage(new Hopper_Rides.MainPage());
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
