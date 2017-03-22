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

			map.Pins.Add(pin);
            //Not yet sure what this part does...
            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(map);
            Content = stack;

		}
	}
}
