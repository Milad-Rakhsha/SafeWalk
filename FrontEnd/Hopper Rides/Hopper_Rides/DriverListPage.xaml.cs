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
        public DriverListPage()
        {
            InitializeComponent();
            this.Detail = new DriverMapPage();
            //TODO Dynamically add in buttons for each riders
        }

        
    }
}
