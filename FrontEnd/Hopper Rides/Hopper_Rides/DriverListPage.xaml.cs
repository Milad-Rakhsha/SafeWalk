using System;
using System.Collections.Generic;
using System.Linq;
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
            this.Detail = new DriverMapPage(this.requests);
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

        }

        /**
         * Called when the driver swipes right on a rider. Will remove them from the list and try to remove them
         * from the backend.
         */ 
        public void quickReject()
        {

        }

        /**
         * Get the requests from the server. Called when the list is initialized
         * and from a pull-to-refresh event.
         */
        public void getRequests()
        {
            Models.ActiveRequest req = new Models.ActiveRequest();
            req.NumPassangers = 3;
            req.StartLocation = "43.074276,-89.394445";
            req.EndLocation = "43.074997,-89.394852";
            req.StartTime = new DateTime(2017, 4, 16, 13, 30, 50);
            req.RiderID = 4807;

            Models.Rider rider = new Models.Rider();
            rider.ID = 4807;
            rider.FirstName = "Shane";
            rider.LastName = "Jann";

            requests.Add(req);
            riders.Add(rider);

            Models.ActiveRequest req1 = new Models.ActiveRequest();
            req1.NumPassangers = 3;
            req1.StartLocation = "43.071713,-89.407837";
            req1.EndLocation = "43.076144,-89399845";
            req1.StartTime = new DateTime(2017, 4, 16, 12, 51, 47);
            req1.RiderID = 4807;

            Models.Rider rider1 = new Models.Rider();
            rider1.ID = 7084;
            rider1.FirstName = "Saun";
            rider1.LastName = "Jon";

            requests.Add(req1);
            riders.Add(rider1);
        }
    }
}
