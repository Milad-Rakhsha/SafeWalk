using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using Newtonsoft.Json;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.GoogleMaps;

namespace Hopper_Rides
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        Map map;
        string googleKey = "AIzaSyA3aaKi6HVMDLcvez0EGcMn6Fsngl5lC5g";

        public MapPage()
        {
            //Seems to work without this
            //InitializeComponent();
            

            //Initialize Map with location, zoom, and size of the map
            map = new Map(
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

            var dest = new SearchBar
            {
                Placeholder = "Where are you going?",
                //VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,

            };

            map.Pins.Add(pin);

            //When search bar is clicked
            dest.Focused += searchFocus;

            //Not yet sure what this part does...
            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(dest);
            stack.Children.Add(map);
            Content = stack;

        }

        async void searchFocus(Object sender, EventArgs e)
        {
            var searchPage = new SearchPage();

            //Set the Selected event handler in the SearchPage to OnSelection
            searchPage.Selected += OnSelection;
            await Navigation.PushModalAsync(searchPage);
        }

        async void OnSelection(Object sender, EventArgs e)
        {
            //Extract description of selected autocomplete prediction
            SelectedItemChangedEventArgs se = (SelectedItemChangedEventArgs)e;
            string text = ((Prediction)(se.SelectedItem)).description;

            //Pop the SearchPage off of the stack, returning to the map
            await Navigation.PopModalAsync();

            //Send location request to Google and get JSON response
            string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + text + "&key=" + googleKey;
            var jsonResponse = await SendRequest(url);

            if (!jsonResponse.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("Bad Address?");
            }
            else
            {
                string content = await jsonResponse.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine(content);

                //Parse JSON data to extract necessary coordinates
                GeocodeResponse parsedResponse = JsonConvert.DeserializeObject<GeocodeResponse>(content);
                double latitude = parsedResponse.Results[0].Geo.Loc.Latitude;
                double longitude = parsedResponse.Results[0].Geo.Loc.Longitude;

                //Change location of existing pin (since riders should only have one destination)
                map.Pins[0].Position = new Position(latitude, longitude);

                //Find new center of pins and largest radius for new map position
                Position newCenter = PinFunctions.CenterPosition(map);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(newCenter, PinFunctions.LargestRadius(map, newCenter)));
            }
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
