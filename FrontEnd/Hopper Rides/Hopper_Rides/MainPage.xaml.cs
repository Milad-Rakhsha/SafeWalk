using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace Hopper_Rides
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            Title = "Welcome to Hopper Rides!";
		}

        async void onLogin(Object sender, EventArgs e)
         {
            if (!String.IsNullOrEmpty(UsernameEntry.Text) && UsernameEntry.Text.Equals("Driver", StringComparison.OrdinalIgnoreCase))
            {
                await Navigation.PushModalAsync(new DriverMapPage());
            }
            else
            {
                //This is a rider
                await Navigation.PushModalAsync(new MapPage());
            }
        }




		async void sendRequest(Object sender, EventArgs e)
		{
			using (var client = new HttpClient())
			{
				
				client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
				//The following will receive the rider #2
				var response = await client.GetAsync("http://thehopper.azurewebsites.net/api/riders/2");
				//This will be a json file of rider's information
				var responseString = await response.Content.ReadAsStringAsync();
				Debug.WriteLine(responseString);
				//The following will convert the json to an actual rider object
				Hopper_Rides.Models.Rider rider = JsonConvert.DeserializeObject<Hopper_Rides.Models.Rider>(responseString);
				Debug.WriteLine(rider.FirstName);
				Debug.WriteLine(rider.LastName);
				Debug.WriteLine(rider.PhoneNumber);
				Debug.WriteLine(rider.Email);

				//If you want to post some information:
				// Make the object you want
				Hopper_Rides.Models.Rider newRider = new Hopper_Rides.Models.Rider();
				newRider.FirstName = "Kyle2";
				newRider.FirstName = "Steiger";
				newRider.Email = "ksteiger@wisc.edu";
				newRider.PhoneNumber = "608-333-6753";
				newRider.ID = 6;
				//Serialize it
				string ser_obj = JsonConvert.SerializeObject(newRider);
				var content_post = new StringContent(ser_obj, Encoding.UTF8, "text/json");
				//post it to the proper table
				var response_post = await client.PostAsync("http://thehopper.azurewebsites.net/api/riders", content_post);
				var responseString_post = await response_post.Content.ReadAsStringAsync();
				Debug.WriteLine(responseString_post);

			}		

			
		}
	}
}
