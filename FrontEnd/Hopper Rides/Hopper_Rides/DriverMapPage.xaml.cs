using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Hopper_Rides
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DriverMapPage : ContentPage
	{
		public DriverMapPage()
		{
			var map = new Map(
			MapSpan.FromCenterAndRadius(
					new Position(43.068152, -89.409759), Distance.FromMiles(1)))
			{
				IsShowingUser = true,
				HeightRequest = 100,
				WidthRequest = 960,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var pin = new Pin()
			{
				Position = new Position(43.068152, -89.409759),
				Label = "You are here!"
			};

			var name = new Label
			{
				Text = "You are a driver",
				//VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};

			double[] xC = new double[] { 43.0765374, 43.071788, 43.072705, 43.074792, 43.074893, 43.074707, 43.069408 };
			double[] yC = new double[] { -89.399286, -89.407911, -89.399882, -89.38437, -89.388267, -89.395909, -89.396661 };
			string[] descrip = new string[] { "Union", "Union South", "Vilas hall", "Capitol", "Overture Center", "Brats", "Kohl Center" };
			Pin[] pintime = drawPins(7, xC, yC, descrip);
			for (int i = 0; i < pintime.Length; i++)
			{
				map.Pins.Add(pintime[i]);
			}

			//getPinData();

			map.Pins.Add(pin);
			
			var stack = new StackLayout { Spacing = 0 };
			stack.Children.Add(name);
			stack.Children.Add(map);
			Content = stack;
		}
		
		async void getPinData()
		{
			using (var client = new HttpClient())
			{

				client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
				
				var response = await client.GetAsync("http://thehopper.azurewebsites.net/api/ActiveRequests");
				//This will be an IQueryable object
				//var responseString = await response.Content.ReadAsStringAsync();
				//Debug.WriteLine(responseString);
				//The following will convert the json to an actual rider object
				//Hopper_Rides.Models.Rider rider = JsonConvert.DeserializeObject<Hopper_Rides.Models.Rider>(responseString);
				/*
				Debug.WriteLine(rider.FirstName);
				Debug.WriteLine(rider.LastName);
				Debug.WriteLine(rider.PhoneNumber);
				Debug.WriteLine(rider.Email);
				*/
				

			}
		}
		
		
		Pin[] drawPins(int num, double[] xCords, double[] yCords, string[] desc)
		{
			Pin[] pins = new Pin[num];
			for (int i = 0; i < num; i++)
			{
				Position tempPos = new Position(xCords[i], yCords[i]);
				Pin tempPin = new Pin();
				tempPin.Position = tempPos;
				tempPin.Label = desc[i];
				pins[i] = tempPin;
			}
			return pins;
		}
	}
}
