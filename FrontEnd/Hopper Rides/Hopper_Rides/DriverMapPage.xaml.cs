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
        List<Models.ActiveRequest> requests;
        List<Models.Rider> riders;
        Map map;
        Label refresh;
		double avgLat = 43.068152;
		double avgLong = -89.409759;
        string googleKey = "AIzaSyA3aaKi6HVMDLcvez0EGcMn6Fsngl5lC5g";

        public DriverMapPage()
		{
            var stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            stack.Children.Add(new ActivityIndicator
            {
                IsRunning = true,
                VerticalOptions = LayoutOptions.Center,
                Color = Color.Tomato
            });
            Content = stack;

            map = new Map(
			MapSpan.FromCenterAndRadius(
					new Position(avgLat,avgLong), Distance.FromMiles(1.5)))
			{
				IsShowingUser = true,
				HeightRequest = 100,
				WidthRequest = 960,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

            refresh = new Label
            {
                Text = "Tap to Refresh",
                //VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
			};
            var tap = new TapGestureRecognizer();
            tap.Tapped += refreshData;
            refresh.GestureRecognizers.Add(tap);

			map.SelectedPinChanged += onPinClick;

            refreshData(this, null);
		}

        async void refreshData(Object sender, EventArgs e)
        {
            try
            {
                List<Models.ActiveRequest> requests = await getRequestData();
                Debug.WriteLine("After result returned");
                this.requests = requests;
                Debug.WriteLine("Result set");
                List<Models.Rider> riders = await getRiderData();
                this.riders = riders;

                //for every request,
                double[] xC = new double[requests.Count()];
                double[] yC = new double[requests.Count()];
                string[] labels = new string[requests.Count()];

                Debug.WriteLine(requests.Count() + " " + riders.Count());

                for (int i = 0; i < requests.Count(); i++)
                {
                    labels[i] = "" + requests[i].ID;
                    try
                    {
                        xC[i] = Double.Parse(requests.ElementAt(i).StartLocation.Split(',')[0]);
                        yC[i] = Double.Parse(requests.ElementAt(i).StartLocation.Split(',')[1]);
                    } catch (FormatException ex)
                    {
                        await DisplayAlert("Debug", "Could not parse coordinates, defaulting to Comp Sci Building." +
                            "This shouldn't happen after garbage test data cleared from database.", "OK");
                        xC[i] = 43.0743486;
                        yC[i] = -89.4071083;
                    }
                }

                Pin[] pintime = drawPins(requests.Count(), xC, yC, labels);
                map.Pins.Clear();
                for (int i = 0; i < pintime.Length; i++)
                {
                    //DisplayAlert("Debug", "A New Pin was Added!", "YAY");
                    if (requests[i].EndTime == null)
                        map.Pins.Add(pintime[i]);
                    else
                        Debug.WriteLine("Found completed ride!");

                    Debug.WriteLine(pintime[i].Label);
                }
                recenterMap();
                var stack = new StackLayout { Spacing = 0 };
                stack.Children.Add(refresh);
                stack.Children.Add(map);
                Content = stack;
            }
            catch (ArgumentNullException ex)
            {
                await DisplayAlert("Debug", "Could not get requests (NullArg)", "OK");
            }
        }

        async void onPinClick(Object sender, SelectedPinChangedEventArgs e)
        {
            if(e.SelectedPin != null)
            {
                int selectedID = Int32.Parse(e.SelectedPin.Label);
                Models.ActiveRequest request = null;
                foreach (var req in requests)
                {
                    if(req.ID == selectedID)
                    {
                        request = req;
                    }
                }

                Debug.WriteLine("Ride ID = " + request.ID);
                string startAddr = await getAddress(request.StartLocation);
                string destAddr = await getAddress(request.EndLocation);

                //Display info about request and option to accept ride
                var accept = await DisplayAlert("Accept this ride?", "Location: " + startAddr + "\n\nDestination: " + destAddr + "\n\nRequest time: " + request.StartTime.ToString() + "\n\nNumber of Passengers: " + request.NumPassangers, "Yes", "No");
                if (accept)
                {
                    acceptRide(request, destAddr);
                }
            }
        }

        async void acceptRide(Models.ActiveRequest request, string destAddr)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                request.EndTime = System.DateTime.Now;

                //Delete old request
                var response_post = await client.DeleteAsync("http://thehopper.azurewebsites.net/api/ActiveRequests/" + request.ID);
                var responseString_post = await response_post.Content.ReadAsStringAsync();
                Debug.WriteLine("Old request deleted");
                Debug.WriteLine(responseString_post);

                //Create new request with valid EndTime
                string ser_obj = JsonConvert.SerializeObject(request);
                var content_post = new StringContent(ser_obj, Encoding.UTF8, "text/json");
                //post it to the proper table
                response_post = await client.PostAsync("http://thehopper.azurewebsites.net/api/ActiveRequests/", content_post);
                responseString_post = await response_post.Content.ReadAsStringAsync();
                Debug.WriteLine("New request created with EndTime");
                Debug.WriteLine(responseString_post);
            }

            await DisplayAlert("Ride Accepted", "Destination: " + destAddr, "Back");
            refreshData(this, null);
        }

        async Task<string> getAddress(string LatLng)
        {
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + LatLng + "&key=" + googleKey;
            var jsonResponse = await SendRequest(url);

            if (!jsonResponse.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("Bad Coordinates?");
                return null;
            }
            else
            {
                string content = await jsonResponse.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine(content);

                //Parse JSON data to extract address
                GeocodeResponse parsedResponse = JsonConvert.DeserializeObject<GeocodeResponse>(content);
                return parsedResponse.Results[0].Formatted_address;
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

        async Task<List<Models.ActiveRequest>> getRequestData()
        {
           
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                List<Models.ActiveRequest> req = new List<Models.ActiveRequest>();
                Debug.WriteLine("Before the request to server");
                var reqResponse = await client.GetStringAsync("http://thehopper.azurewebsites.net/api/ActiveRequests/").ConfigureAwait(false);
                //This will be an IQueryable object
                Debug.WriteLine("Before the response from server");
                string reqResponseStr =  reqResponse;
                Debug.WriteLine("After the response from server");

                //The following will convert the json to an actual rider object
                req = JsonConvert.DeserializeObject<List<Models.ActiveRequest>>(reqResponseStr);

                //DEBUG Stuff
                for (int i = 0; i < req.Count; i++)
                {
                    Debug.WriteLine("Request " + i + ": " + req.ElementAt(i).ID);
                }
                return req;
            }
        }

        async Task<List<Models.Rider>> getRiderData()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");

                List<Models.Rider> rid = new List<Models.Rider>();

                //Same process here
                var riderResponse = await client.GetStringAsync("http://thehopper.azurewebsites.net/api/riders/").ConfigureAwait(false);
                var riderResponseStr = riderResponse;
                rid = JsonConvert.DeserializeObject<List<Models.Rider>>(riderResponseStr);

                //DEBUG Stuff
                for (int i = 0; i < rid.Count; i++)
                {
                    Debug.WriteLine("Rider " + i + ": " + rid.ElementAt(i).ID);
                }
                return rid;
            }
            
        }

		/*
        async void getPinData()
		{
			using (var client = new HttpClient())
			{

                   client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                    requests = new List<Models.ActiveRequest>();
                    riders = new List<Models.Rider>();

                    var reqResponse = await client.GetAsync("http://thehopper.azurewebsites.net/api/ActiveRequests/");
                    //This will be an IQueryable object
                    var reqResponseStr = await reqResponse.Content.ReadAsStringAsync();
                    //The following will convert the json to an actual rider object
                    requests = JsonConvert.DeserializeObject<List<Models.ActiveRequest>>(reqResponseStr);
                    updateRequests(requests);

                    //Same process here
                    var riderResponse = await client.GetAsync("http://thehopper.azurewebsites.net/api/riders/");
                    var riderResponseStr = await riderResponse.Content.ReadAsStringAsync();
                    riders = JsonConvert.DeserializeObject<List<Models.Rider>>(riderResponseStr);
                    updateRiders(riders); 
                } catch(ArgumentNullException e)
                {
                    await DisplayAlert("GET Request Failed" , "Could not get requests (NullArg)", "OK");
                }
                //DEBUG Stuff
                for(int i = 0; i < requests.Count; i++)
                {
                    Debug.WriteLine("Request " + i + ": " + requests.ElementAt(i).ID);
                }
                for (int i = 0; i < riders.Count; i++)
                {
                    Debug.WriteLine("Rider " + i + ": " + riders.ElementAt(i).ID);
                }
                //end
            }
        }*/
		void recenterMap()
		{	
			//No guarantee to have all pins on the map.  Just recenters the map around the average lat and long of all pins - 1.5 mile radius
			var pos = new Xamarin.Forms.GoogleMaps.Position(avgLat, avgLong);			
			map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMiles(1.5)));
		}
		
		Pin[] drawPins(int num, double[] xCords, double[] yCords, string[] labels)
		{
			Pin[] pins = new Pin[num];
			double totalLat = 0;
			double totalLong = 0;
			for (int i = 0; i < num; i++)
			{
				Position tempPos = new Position(xCords[i], yCords[i]);
				totalLat += xCords[i];
				totalLong += yCords[i];
				Pin tempPin = new Pin();
				tempPin.Position = tempPos;
				tempPin.Label = labels[i];
				pins[i] = tempPin;
			}
			avgLat = totalLat / num;
			avgLong = totalLong / num;
			return pins;
		}
	}
}
