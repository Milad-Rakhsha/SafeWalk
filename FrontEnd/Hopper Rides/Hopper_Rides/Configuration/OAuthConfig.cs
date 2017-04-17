using System;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Hopper_Rides
{
	public class OAuthConfig
	{

		public static MainPage my_MainPage = new MainPage();
		static NavigationPage _NavigationPage = new NavigationPage();
		public static Models.Rider User;

		public static void PostRider()
		{
			using (var client = new HttpClient())
			{
				//Serialize it
				string ser_obj = JsonConvert.SerializeObject(User);
				var content_post = new StringContent(ser_obj, Encoding.UTF8, "text/json");
				//post it to the proper table
				var response_post = client.PostAsync("http://thehopper.azurewebsites.net/api/riders", content_post);
				response_post.Wait();
				Debug.WriteLine(ser_obj);
				Debug.WriteLine("Posting a new User to the database");
			}
		}


		public static Action SuccessfulLoginAction
		{
			get
			{
				return new Action(() =>
				{

					//_NavigationPage.Navigation.PushModalAsync(new SearchPage(true));
					//Navigation.PushModalAsync(new SearchPage(true))
					//Xamarin.Forms.NavigationPage.PushModalAsync(new DriverListPage())
					//_NavigationPage.Navigation.PushModalAsync(my_MainPage);
					_NavigationPage.Navigation.PopAsync();
				});
			}
		}

	}
}
