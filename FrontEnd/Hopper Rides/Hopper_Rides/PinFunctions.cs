using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;

namespace Hopper_Rides
{
    class PinFunctions
    {
        //Returns center Position of all pins in map
        public static Position CenterPosition(Map map)
        {
            double totLatitude = 0;
            double totLongitude = 0;
            int count = 0;

            foreach (var pin in map.Pins)
            {
                totLatitude += pin.Position.Latitude;
                totLongitude += pin.Position.Longitude;
                count++;
            }

            double avgLatitude = totLatitude / count;
            double avgLongitude = totLongitude / count;
            return new Position(avgLatitude, avgLongitude);
        }

        //Returns largest Distance needed to show all pins in map centered at center
        public static Distance LargestRadius(Map map, Position center)
        {
            double maxRadius = 0;
            double currRadius;

            foreach (var pin in map.Pins)
            {
                currRadius = CoordDistance(pin.Position, center);

                if (currRadius > maxRadius)
                    maxRadius = currRadius;
            }

            //1.1 to provide buffer around largest radius
            return Distance.FromMiles(maxRadius * 1.1);
        }

        //The following 3 functions have been borrowed from http://geodatasource.com/developers/c-sharp
        //Returns distance (in miles) between two Positions
        private static double CoordDistance(Position pos1, Position pos2)
        {
            double theta = pos1.Longitude - pos2.Longitude;
            double dist = Math.Sin(deg2rad(pos1.Latitude)) * Math.Sin(deg2rad(pos2.Latitude)) + Math.Cos(deg2rad(pos1.Latitude)) * Math.Cos(deg2rad(pos2.Latitude)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            return (dist);
        }

        //converts degrees to radians
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //Converts radians to degrees
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

    }
}
