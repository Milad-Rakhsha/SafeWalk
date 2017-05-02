using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.GoogleMaps;
using Plugin.Geolocator;

namespace Hopper_Rides
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        Map map;
        string googleKey = "AIzaSyA3aaKi6HVMDLcvez0EGcMn6Fsngl5lC5g";
		double currentLong;
		double currentLat;
		Position currentPosition;
        SearchBar start;
        SearchBar dest;
		string myAddress = "Starting Position...";

		public MapPage()
        {
            //Seems to work without this
            //InitializeComponent();

            var stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            stack.Children.Add(new ActivityIndicator
            {
                IsRunning = true,
                VerticalOptions = LayoutOptions.Center
            });
            Content = stack;

            //Initialize Map with location, zoom, and size of the map
            getUserLocation();
			

			Debug.WriteLine("Position Latitude: {0}", currentLat);
			Debug.WriteLine("Position Longitude: {0}", currentLong);	

       

        }

        async void startFocus(Object sender, EventArgs e)
        {
            var searchPage = new SearchPage(false, start.Text);

            //Set the Selected event handler in the SearchPage to OnSelection
            searchPage.Selected += OnStartSelection;
            searchPage.ButtonClicked += OnStartLocation;
            await Navigation.PushModalAsync(searchPage);
        }

        async void destFocus(Object sender, EventArgs e)
        {
            var searchPage = new SearchPage(true, dest.Text);

            //Set the Selected event handler in the SearchPage to OnSelection
            searchPage.Selected += OnDestSelection;
            await Navigation.PushModalAsync(searchPage);
        }

        async void OnStartSelection(Object sender, EventArgs e)
        {
            //Extract description of selected autocomplete prediction
            SelectedItemChangedEventArgs se = (SelectedItemChangedEventArgs)e;
            string text = ((Prediction)(se.SelectedItem)).description;

            //Update search bar with address selected
            start.Text = text;

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

                AddOrChangePin(latitude, longitude, false);
            }
        }

        async void OnDestSelection(Object sender, EventArgs e)
        {
            //Extract description of selected autocomplete prediction
            SelectedItemChangedEventArgs se = (SelectedItemChangedEventArgs)e;
            string text = ((Prediction)(se.SelectedItem)).description;

            //Update search bar with address selected
            dest.Text = text;

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

                AddOrChangePin(latitude, longitude, true);
            }
        }

        async void OnStartLocation(Object sender, EventArgs e)
        {
            try
            {
                var locator = CrossGeolocator.Current;

                //Try to get current location with 10 second timeout
                var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                if (position == null)
                    throw new Exception();

                await Navigation.PopModalAsync();

                //Add address of location to search bar
                string text = position.Latitude + "," + position.Longitude;
                string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + text + "&key=" + googleKey;
                var jsonResponse = await SendRequest(url);

                if (!jsonResponse.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Bad Coordinates?");
                }
                else
                {
                    string content = await jsonResponse.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine(content);

                    //Parse JSON data to extract address
                    GeocodeResponse parsedResponse = JsonConvert.DeserializeObject<GeocodeResponse>(content);
                    start.Text = parsedResponse.Results[0].Formatted_address;

                    AddOrChangePin(position.Latitude, position.Longitude, false);
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't get current location.");
            }
        }

        //Test if either existing pin is a destination/starting point
        //if so, move that pin, otherwise create a new destination/starting point
        private void AddOrChangePin(double latitude, double longitude, bool isDestination)
        {
            string pinLabel = "Starting Point";
            if (isDestination)
                pinLabel = "Destination";

            if(map.Pins.Count == 0 || (map.Pins.Count == 1 && !map.Pins[0].Label.Equals(pinLabel)))
            {
                Color color = Color.Red;
                if (isDestination)
                    color = Color.Blue;

                map.Pins.Add(new Pin
                {
                    Position = new Position(latitude, longitude),
                    Label = pinLabel,
					IsDraggable = true,
				Icon = BitmapDescriptorFactory.DefaultMarker(color)
                });
            }
            else if (map.Pins[0].Label.Equals(pinLabel))
            {
                map.Pins[0].Position = new Position(latitude, longitude);
            }
            else if (map.Pins.Count == 2 && map.Pins[1].Label.Equals(pinLabel))
            {
                map.Pins[1].Position = new Position(latitude, longitude);
            }

            //Find new center of pins and largest radius for new map position
            Position newCenter = PinFunctions.CenterPosition(map);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(newCenter, PinFunctions.LargestRadius(map, newCenter)));
        }
		/*
        void onDropClicked(object sender, EventArgs e)
		{
			Pin destination = new Pin();		

			destination.Position = new Position(43.068152, -89.409759);
			destination.Label = "Hold to Drag";
			
			map.Pins.Add(destination);

                
		}
		*/
        async void onSubmitClicked(Object sender, EventArgs e)
        {
            //Add code to submit ride request to database
            if (App.riderID == 0)
            {
                await DisplayAlert("Bad Login", "Rider ID is invalid, please log in via Facebook", "Back To Main Menu");
                await Navigation.PushModalAsync(new MyPage());
            }
            else if(map.Pins.Count() != 2)
            {
                await DisplayAlert("Missing Starting Point or Destination", "Please make sure you have a valid starting point and destination", "OK");
            }
            else
            {
                List<Models.ActiveRequest> reqList = new List<Models.ActiveRequest>();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                    Debug.WriteLine("Before the request to server");
                    var reqResponse = await client.GetAsync("http://thehopper.azurewebsites.net/api/ActiveRequests/");
                    //This will be an IQueryable object
                    Debug.WriteLine("Before the response from server");
                    string reqResponseStr = await reqResponse.Content.ReadAsStringAsync();
                    Debug.WriteLine("After the response from server");

                    //The following will convert the json to an actual rider object
                    reqList = JsonConvert.DeserializeObject<List<Models.ActiveRequest>>(reqResponseStr);
                }
                App.activeReqID = reqList[reqList.Count - 1].ID + 1;

                Models.ActiveRequest req = new Models.ActiveRequest();
                req.ID = App.activeReqID;
                req.RiderID = App.riderID;
                if(map.Pins[0].Label == "Starting Point")
                {
                    req.StartLocation = map.Pins[0].Position.Latitude + "," + map.Pins[0].Position.Longitude;
                    req.EndLocation = map.Pins[1].Position.Latitude + "," + map.Pins[1].Position.Longitude; ;
                }
                else
                {
                    req.StartLocation = map.Pins[1].Position.Latitude + "," + map.Pins[1].Position.Longitude;
                    req.EndLocation = map.Pins[0].Position.Latitude + "," + map.Pins[0].Position.Longitude;
                }

                var subPage = new SubmissionPage(req, start.Text, dest.Text);
                subPage.ButtonClicked += OnFinalSubmission;
                await Navigation.PushModalAsync(subPage);
            }
        }

		async void getUserLocation()
		{
			try
			{

				var locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 50;

				var position = await locator.GetPositionAsync();
				if (position == null)
					return;
				currentLat = position.Latitude;
				currentLong = position.Longitude;
				currentPosition = new Position(currentLat,currentLong);


				/*
				Debug.WriteLine("Position Latitude: {0}", position.Latitude);
				Debug.WriteLine("Position Longitude: {0}", position.Longitude);
				Debug.WriteLine("Position Latitude: {0}", currentLat);
				Debug.WriteLine("Position Longitude: {0}", currentLong);
				*/
				map = new Map(
					MapSpan.FromCenterAndRadius(currentPosition, Distance.FromMiles(1)))
				{
					IsShowingUser = true,
					VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand
				};
				
				//building starting position field with user's current lat/long
				string text = currentLat + "," + currentLong;
				
				//sends request to google
				String url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + text + "&key=" + googleKey;
				var jsonResponse = await SendRequest(url);

				if (!jsonResponse.IsSuccessStatusCode)
				{
					//bad response, debu will output something and Starting position will be set to default which is "Starting Position..."
					System.Diagnostics.Debug.WriteLine("Bad Coordinates?");
				}
				else
				{
					string content = await jsonResponse.Content.ReadAsStringAsync();

					System.Diagnostics.Debug.WriteLine(content);

					//Parse JSON data to extract address
					GeocodeResponse parsedResponse = JsonConvert.DeserializeObject<GeocodeResponse>(content);
					myAddress = parsedResponse.Results[0].Formatted_address;
					
					
				}
				
				
				start = new SearchBar
				{					
					Placeholder = myAddress,
					HorizontalTextAlignment = TextAlignment.Start
                };

                start.Focused += startFocus;

                dest = new SearchBar
				{
					Placeholder = "Where are you going?",
					HorizontalTextAlignment = TextAlignment.Start
				};
			
                Button submit = new Button
                {
                    Text = "Submit Request",
                    BackgroundColor = Color.Red
                };
                submit.Clicked += onSubmitClicked;


				//When search bar is clicked
				dest.Focused += destFocus;

                var layout = new StackLayout
                {
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                layout.Children.Add(start);
                layout.Children.Add(dest);
                layout.Children.Add(map);
                layout.Children.Add(submit);
                Content = layout;

            }
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
			}
		}

        async void OnFinalSubmission(Object sender, EventArgs e)
        {
            start.Text = "";
            dest.Text = "";
            map.Pins.Clear();
            await Navigation.PopModalAsync();
            await DisplayAlert("Ride Request Successful!", "Your ride request has been submitted to Hopper Rides.", "Back");
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
