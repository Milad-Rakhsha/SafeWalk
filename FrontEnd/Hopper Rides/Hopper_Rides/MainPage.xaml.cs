using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hopper_Rides
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            
               
		}

        void onLogin(Object sender, EventArgs e)
        {
            //OK need to replace this with a call to the new map screen.
            //Content = new Label{
            //    Text = "It's Clicked"           
            //};
            App.Current.MainPage = new MapPage();
        }
	}
}
