using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hopper_Rides
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubmissionPage : ContentPage
    {
        public event EventHandler ButtonClicked;
        Models.ActiveRequest request;

        public SubmissionPage(Models.ActiveRequest req, string startAddr, string destAddr)
        {
            InitializeComponent();
            passengerCount.Items.Add("1");
            passengerCount.Items.Add("2");
            passengerCount.Items.Add("3");
            passengerCount.Items.Add("4");
            passengerCount.Items.Add("5");
            passengerCount.Items.Add("6");
            start.Text += startAddr;
            dest.Text += destAddr;
            request = req;
        }

        async void submitClicked(Object sender, EventArgs e)
        {
            bool goodCount = true;
            if (passengerCount.SelectedItem == null)
            {
                await DisplayAlert("Select Number of Passengers", "", "OK");
                goodCount = false;
            }
            else
            {
                switch (passengerCount.SelectedItem.ToString())
                {
                    case "1":
                        request.NumPassangers = 1;
                        break;
                    case "2":
                        request.NumPassangers = 2;
                        break;
                    case "3":
                        request.NumPassangers = 3;
                        break;
                    case "4":
                        request.NumPassangers = 4;
                        break;
                    case "5":
                        request.NumPassangers = 5;
                        break;
                    case "6":
                        request.NumPassangers = 6;
                        break;
                    default:
                        await DisplayAlert("Select Number of Passengers", "", "OK");
                        goodCount = false;
                        break;
                }
            }

            if (goodCount)
            {
                request.Comment = comment.Text;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                    request.StartTime = System.DateTime.Now;
                    string ser_obj = JsonConvert.SerializeObject(request);
                    System.Diagnostics.Debug.WriteLine(ser_obj);
                    var content_post = new StringContent(ser_obj, Encoding.UTF8, "text/json");
                    //post it to the proper table
                    var response_post = await client.PostAsync("http://thehopper.azurewebsites.net/api/ActiveRequests/", content_post);
                    var responseString_post = await response_post.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine("Here's the POST response");
                    System.Diagnostics.Debug.WriteLine(responseString_post);
                }

                ButtonClicked(this, e);
            }
        }
    }
}
