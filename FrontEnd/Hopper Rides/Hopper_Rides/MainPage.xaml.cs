using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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

        async void sendRequest(Object sender, EventArgs e)
        {
            using (var client = new HttpClient())
            {
                string json = "{ \"Something\": \"Hello\" }";
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://thehopper.azurewebsites.net/", content);
                var responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);

            }
        }
	}
}
