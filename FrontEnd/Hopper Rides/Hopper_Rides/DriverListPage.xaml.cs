using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace Hopper_Rides
{
    //This is the Master Page
    public partial class DriverListPage : MasterDetailPage
    {
        
        public List<Models.ActiveRequest> requests = new List<Models.ActiveRequest>();
        public List<Models.Rider> riders = new List<Models.Rider>();
        

        public DriverListPage()
        {
            InitializeComponent();
            getRequests();
            this.Detail = new DriverMapPage();
            this.FindByName<ListView>("list").ItemsSource = requests;
            
            //TODO Dynamically add in buttons for each riders
        }

        void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            DisplayAlert("Item Selected", e.SelectedItem.ToString(), "Ok");
            //((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
        }

        void OnTapped(object sender, SelectedItemChangedEventArgs e)
        {
            //TODO
        }

        /**
         * Called when the driver swipes right on a rider. Will remove them from the list and try to remove them
         * from the backend.
         */ 
        public void quickReject()
        {
            //TODO
        }

        /**
         * Get the requests from the server. Called when the list is initialized
         * and from a pull-to-refresh event.
         */
        async void getRequests()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                    requests = new List<Models.ActiveRequest>();
                    riders = new List<Models.Rider>();

                    var reqResponse = await client.GetAsync("http://thehopper.azurewebsites.net/api/ActiveRequests/");
                    //This will be an IQueryable object
                    var reqResponseStr = await reqResponse.Content.ReadAsStringAsync();
                    //The following will convert the json to an actual rider object
                    requests = JsonConvert.DeserializeObject<List<Models.ActiveRequest>>(reqResponseStr);

                    //Same process here
                    var riderResponse = await client.GetAsync("http://thehopper.azurewebsites.net/api/riders/");
                    var riderResponseStr = await riderResponse.Content.ReadAsStringAsync();
                    riders = JsonConvert.DeserializeObject<List<Models.Rider>>(riderResponseStr);
                }
                catch (ArgumentNullException e)
                {
                    await DisplayAlert("GET Request Failed", "Could not get requests (NullArg)", "OK");
                }
            }
        }
    }
}
