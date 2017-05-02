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


		public DriverMapPage()
		{
			map = new Map(
			MapSpan.FromCenterAndRadius(
					new Position(43.068152, -89.409759), Distance.FromMiles(1)))
			{
				IsShowingUser = true,
				HeightRequest = 100,
				WidthRequest = 960,
				VerticalOptions = LayoutOptions.FillAndExpand
			};


			var name = new Label
			{
				Text = "You are a driver",
				//VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};

            //Hardcoded examples
            //double[] xC = new double[] { 43.0765374, 43.071788, 43.072705, 43.074792, 43.074893, 43.074707, 43.069408 };
            //double[] yC = new double[] { -89.399286, -89.407911, -89.399882, -89.38437, -89.388267, -89.395909, -89.396661 };
            //string[] descrip = new string[] { "Union", "Union South", "Vilas hall", "Capitol", "Overture Center", "Brats", "Kohl Center" };
            //Pin[] pintime = drawPins(7, xC, yC, descrip);
            //for (int i = 0; i < pintime.Length; i++)
            //{
            //	map.Pins.Add(pintime[i]);
            //}

            try
            {
               // DisplayAlert("Debug", requests.Count().ToString(), "OK");

                //getRiderData();
                Task<List<Models.ActiveRequest>> newReqTask = getRequestData();
                Debug.WriteLine("After getRequestDatae()");
                
                //newReqTask.Wait();
                List<Models.ActiveRequest> requests = newReqTask.Result;
                Debug.WriteLine("After result returned");
                this.requests = requests;
                Debug.WriteLine("Result set");


                Task<List<Models.Rider>> newRidTask = getRiderData();

                //newRidTask.Wait();
                List<Models.Rider> riders = newRidTask.Result;
                this.riders = riders;

                //riders.Add(new Models.Rider());

                //getPinData();
                //for every request,
                double[] xC = new double[requests.Count()];
                double[] yC = new double[requests.Count()];
                string[] labels = new string[requests.Count()];

                Debug.WriteLine(requests.Count() + " " + riders.Count());

                for (int i = 0; i < requests.Count(); i++)
                {
                    //find the rider corresponding to that request
                    for (int j = 0; j < riders.Count; j++)
                    {
                        Debug.WriteLine("Does " + riders.ElementAt(j).ID + " == " + requests.ElementAt(i).RiderID + "?");
                        if (riders.ElementAt(j).ID == requests.ElementAt(i).RiderID)
                        {
                            Debug.WriteLine("Match!" + riders.ElementAt(j).ID + " = " + requests.ElementAt(i).RiderID);
                            //IDs are the same, construct a pin and place it on the map
                            //Pin pin = new Pin
                            //{
                            //    Position = new Position(),
                            //    Label = riders.ElementAt(j).FirstName + " " + riders.ElementAt(j).LastName
                            //};
                            try
                            {
                                xC[i] = Double.Parse(requests.ElementAt(i).StartLocation.Split(',')[0]);
                                yC[i] = Double.Parse(requests.ElementAt(i).StartLocation.Split(',')[1]);
                            } catch(FormatException e)
                            {
                                DisplayAlert("Debug", "Could not parse coordinates, defaulting to Comp Sci Building." +
                                    "This shouldn't happen after garbage test data cleared from database.", "OK");
                                xC[i] = 43.0743486;
                                yC[i] = -89.4071083;
                            }
                            //if(riders.ElementAt(j).FirstName == null || riders.ElementAt(j).LastName == null)
                            //{
                            //    labels[i] = riders.ElementAt(j).ID.ToString();
                            //}
                            //else
                            //{
                            //    labels[i] = riders.ElementAt(j).FirstName + " " + riders.ElementAt(j).LastName;
                            //}
                            labels[i] = riders.ElementAt(j).FirstName;
                           
                        }
                    }
                }
                Pin[] pintime = drawPins(requests.Count(), xC, yC, labels);
                for (int i = 0; i < pintime.Length; i++)
                {
                    DisplayAlert("Debug", "A New Pin was Added!", "YAY");
                    map.Pins.Add(pintime[i]);
                    Debug.WriteLine(pintime[i].Label);
                }
            } catch(ArgumentNullException e)
            {
                DisplayAlert("Debug", "Could not get requests (NullArg)", "OK");
            }


            var stack = new StackLayout { Spacing = 0 };
			stack.Children.Add(name);
			stack.Children.Add(map);
			Content = stack;
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
		
		
		Pin[] drawPins(int num, double[] xCords, double[] yCords, string[] labels)
		{
			Pin[] pins = new Pin[num];
			for (int i = 0; i < num; i++)
			{
				Position tempPos = new Position(xCords[i], yCords[i]);
				Pin tempPin = new Pin();
				tempPin.Position = tempPos;
				tempPin.Label = labels[i];
				pins[i] = tempPin;
			}
			return pins;
		}
	}
}
