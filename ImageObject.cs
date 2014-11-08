using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INFOIBV
{
    class ImageObject
    {
        public int Area;
        public float Variance;
        public int ID;

        public ImageObject(int area, List<Tuple<int,int>> perimeter, int ID)
        {
            this.Area = area;
            this.ID = ID;
            CalculateVariance(perimeter);
        }

        private void CalculateVariance(List<Tuple<int, int>> perimeter)
        {
            float xSum = 0;
            float ySum = 0;

            for(int i = 0; i < perimeter.Count; i++)
            {
                xSum += perimeter[i].Item1;
                ySum += perimeter[i].Item2;
            }

            float xCenter = xSum / (float)perimeter.Count;
            float yCenter = ySum / (float)perimeter.Count;

            float distanceSum = 0;

            List<int> distances = new List<int>();

            for(int i = 0; i < perimeter.Count; i++)
            {
                int xDistance = (int)Math.Abs(perimeter[i].Item1 - xCenter);
                int yDistance = (int)Math.Abs(perimeter[i].Item2 - yCenter);
                int distance = (int)Math.Sqrt(Math.Pow(xDistance,2) + Math.Pow(yDistance,2));
                distances.Add(distance);
                distanceSum += distance;
            }

            float averageDistance = distanceSum / distances.Count;

            float differenceSum = 0;

            for(int i = 0; i < distances.Count;i++)
            {
                differenceSum += (float)Math.Pow(Math.Abs(averageDistance - distances[i]),2);
            }

            Variance = differenceSum / distances.Count;
        }

    }
}
