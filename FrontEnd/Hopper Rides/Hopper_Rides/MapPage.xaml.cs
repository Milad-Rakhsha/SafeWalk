using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using Newtonsoft.Json;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;

namespace Hopper_Rides
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            //Seems to work without this
            //InitializeComponent();

            string googleKey = "AIzaSyA3aaKi6HVMDLcvez0EGcMn6Fsngl5lC5g";

            //Initialize Map with location, zoom, and size of the map
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

            var dest = new Entry
            {
                Placeholder = "Where are you going?",
                //VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,

            };

            map.Pins.Add(pin);

            //Event handler when Enter pressed from search bar
            dest.Completed += async (sender, e) =>
            {
                var text = ((Entry)sender).Text;

                //Send location request to Google and get JSON response
                string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + ((string)text) + "&key=" + googleKey;
                var jsonResponse = await SendRequest(url);

                if(!jsonResponse.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Bad Address?");
                }
                else
                {
                    string content = await jsonResponse.Content.ReadAsStringAsync();
                    
                    System.Diagnostics.Debug.WriteLine(content);

                    //Parse JSON data to extract necessary coordinates
                    JsonResponse parsedResponse = JsonConvert.DeserializeObject<JsonResponse>(content);
                    double latitude = parsedResponse.Results[0].Geo.Loc.Latitude;
                    double longitude = parsedResponse.Results[0].Geo.Loc.Longitude;

                    //Place new pin
                    var newPin = new Pin
                    {
                        Position = new Position(latitude, longitude),
                        Label = "Destination"
                    };

                    map.Pins.Add(newPin);

                    //Find new center of pins and largest radius for new map position
                    Position newCenter = PinFunctions.CenterPosition(map);
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(newCenter, PinFunctions.LargestRadius(map, newCenter)));
                }

                dest.Text = "";
            };

            //Not yet sure what this part does...
            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(dest);
            stack.Children.Add(map);
            Content = stack;

        }

        //Sends HTTP request using given url
        private async Task<HttpResponseMessage> SendRequest(string url)
        {
            HttpClient client = new HttpClient();
            var uri = new Uri(url);

            HttpResponseMessage response = await client.GetAsync(uri);
            return response;
        }
    }
}
