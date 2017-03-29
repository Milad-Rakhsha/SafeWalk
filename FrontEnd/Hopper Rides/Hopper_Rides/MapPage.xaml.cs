using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;

namespace Hopper_Rides
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            //Seems to work without this
            //InitializeComponent();

            //Initialize Map with location, zoom, and size of the map
            var map = new Map(
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

            var dest = new Entry
            {
                Placeholder = "Where are you going?",
                //VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,

            };

            map.Pins.Add(pin);

            //Event handler when Enter pressed from search bar
            dest.Completed += async (sender, e) =>
            {
                var text = ((Entry)sender).Text;
                Geocoder geo = new Geocoder();
                IEnumerable<Position> positions = await geo.GetPositionsForAddressAsync(text);
                if (positions.Count() == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Bad Address");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Position: " + positions.First().Latitude + ", " + positions.First().Longitude);

                    //Place new pin
                    var newPin = new Pin
                    {
                        Position = positions.First(),
                        Label = "Destination"
                    };

                    map.Pins.Add(newPin);

                    //Find new center of pins and largest radius for new map position
                    Position newCenter = PinFunctions.CenterPosition(map);
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(newCenter, PinFunctions.LargestRadius(map, newCenter)));
                }

                dest.Text = "";
            };

            //Not yet sure what this part does...
            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(dest);
            stack.Children.Add(map);
            Content = stack;

        }
    }
}
