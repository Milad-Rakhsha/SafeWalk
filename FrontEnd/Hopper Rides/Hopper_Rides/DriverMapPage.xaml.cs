using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hopper_Rides
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DriverMapPage : ContentPage
	{
        List<Models.ActiveRequest> requests;

        public DriverMapPage(List<Models.ActiveRequest> requests)
		{
            

			var map = new Map(
			MapSpan.FromCenterAndRadius(
                    //Distance.FromMiles() represents Map zoom
					new Position(43.068152, -89.409759), Distance.FromMiles(0.20)))
			{
				IsShowingUser = true,
				HeightRequest = 100,
				WidthRequest = 960,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

            //IsShowingUser already takes care of this
			//var pin = new Pin()
			//{
			//	Position = new Position(43.068152, -89.409759),
			//	Label = "You are here!"
			//};

			var name = new Label
			{
				Text = "You are a driver",
				//VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};

			
			var stack = new StackLayout { Spacing = 0 };
			stack.Children.Add(name);
			stack.Children.Add(map);
			Content = stack;

            this.requests = requests;
            for(int i = 0; i < this.requests.Count(); i++)
            {
                try
                {
                    var riderLocation = requests.ElementAt(i).StartLocation.Split(',');
                    var pin = new Pin()
                    {
                        Position = new Position(Double.Parse(riderLocation[0]), Double.Parse(riderLocation[1])),
                        Label = requests.ElementAt(i).RiderID.ToString()
                    };

                    map.Pins.Add(pin);
                }
                catch (FormatException e)
                {
                    System.Diagnostics.Debug.WriteLine(requests.ElementAt(i).StartLocation);
                    //DisplayAlert("Input Error","Your Input Format Doesn't work!","Sorry");
                }
            }

            
        }

        public void updateRequests(List<Models.ActiveRequest> requests)
        {
            this.requests = requests;
        }

        
	}
}
