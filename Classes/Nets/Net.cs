using CarsAndPitsWPF2.Classes.DataTypes;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CarsAndPitsWPF2.Classes.Nets
{
    public class Net
    {
        public readonly int maxDepth;
        public readonly double accuracy; //in meters 

        public NetSquare zeroSquare;
        public int totalSquaresCount;
        public int totalValuesContains;        

        private NetSquare lastUpperSquare;
        private NetSquare[] squaresCache;

        public Net(int maxDepth = 25)
        {
            this.maxDepth = maxDepth;

            double latDist = 180d / Math.Pow(2, maxDepth - 1);
            double lngDist = 360d / Math.Pow(2, maxDepth - 1);

            zeroSquare = new NetSquare(-90, -180, 0, 0);
            accuracy = new GeoCoordinate(0, 0).GetDistanceTo(new GeoCoordinate(latDist, lngDist));
            totalSquaresCount = 0;
            totalValuesContains = 0;
        }

        public void putVector(double lat, double lng,
            CPVectorAbs vectorAbs, string deviceId, SensorType sensor)
        {
            putToSquareTree(lat, lng, vectorAbs, deviceId, sensor);            
            totalValuesContains++;
        }

        private void putToSquareTree(double lat, double lng, 
            CPVectorAbs vectorAbs, string deviceId, SensorType sensor)
        {
            putAndGetBottomSquare(lat, lng, vectorAbs.length).putData(vectorAbs, deviceId, sensor);
        }

        private NetSquareCPData putAndGetBottomSquare(double lat, double lng, double intensityAdd)
        {
            NetSquare netSquare = zeroSquare;
            zeroSquare.intensity += intensityAdd;

            int[] path = getPathToSquare(lat, lng);
            int index;
            for (int i = 0; i < path.Length - 1; i++)
            {
                index = path[i];
                if (netSquare.children[index] == null)
                {
                    netSquare.children[index] = new NetSquare(netSquare, index, 0);
                    totalSquaresCount++;
                }

                netSquare = netSquare.children[index];
                netSquare.intensity += intensityAdd;
            }

            index = path.Last();
            if (netSquare.children[index] == null)
            {
                NetSquareCPData square = new NetSquareCPData(netSquare, index, 0);
                netSquare.children[index] = square;
                totalSquaresCount++;
                return square;
            }
            else
            {
                NetSquareCPData square = (NetSquareCPData)netSquare.children[index];
                square.intensity += intensityAdd;
                return square;
            }
        }

        //GetSquare functions
        public NetSquare calcChild(NetSquare s, int index)
        {
            return new NetSquare(s, index, s.intensity);
        }

        public NetSquareCPData getSquare(double lat, double lng)
        {
            NetSquare netSquare = zeroSquare;

            int[] path = getPathToSquare(lat, lng);
            foreach (int i in path)
                if (netSquare.children[i] == null) return (NetSquareCPData)netSquare;
                else netSquare = netSquare.children[i];

            return (NetSquareCPData)netSquare;
        }

        public NetSquare getSquare(int[] path)
        {
            NetSquare NetSquare = zeroSquare;
            foreach (int i in path)
            {
                if (NetSquare.children == null)
                {
                    NetSquare.children = new NetSquare[4];
                    NetSquare.children[i] = new NetSquare(NetSquare, i, 0);
                    totalSquaresCount++;
                }
                else if (NetSquare.children[i].children == null)
                {
                    NetSquare.children[i] = new NetSquare(NetSquare, i, 0);
                    totalSquaresCount++;
                }

                NetSquare = NetSquare.children[i];
            }

            return NetSquare;
        }

        public NetSquare getUpperParent(NetSquare baseSquare, int level)
        {
            return getSquare(baseSquare.path.Take(level).ToArray());
        }

        public NetSquare getMostSquare(NetSquare baseSquare, int[] preferMap, int maxLevel = -1)
        {
            bool childAvailable = baseSquare.children[0] != null || baseSquare.children[1] != null || baseSquare.children[2] != null || baseSquare.children[3] != null;
            NetSquare NetSquare = baseSquare;
            while (childAvailable && NetSquare.level >= maxLevel)
            {
                foreach (int i in preferMap)
                    if (NetSquare.children[i] != null)
                    {
                        NetSquare = NetSquare.children[i];
                        childAvailable = true;
                        break;
                    }
            }

            return NetSquare;
        }

        //Multi-NetSquare getters
        public List<NetSquare> getChildSquares(NetSquare NetSquare, int deepness = -1)
        {
            if (deepness == 0)
                return new List<NetSquare>();
            deepness--;

            List<NetSquare>[] rectOfrects = new List<NetSquare>[] {
                    new List<NetSquare>(),new List<NetSquare>(),new List<NetSquare>(),new List<NetSquare>()
                };
            for (int i = 0; i < 4; i++)
                if (NetSquare.children[i] != null)
                {
                    rectOfrects[i].Add(NetSquare.children[i]);
                    rectOfrects[i].AddRange(getChildSquares(NetSquare.children[i], deepness));
                }

            List<NetSquare> rects = new List<NetSquare>();
            bool nextAvailable = (rectOfrects[0].Count + rectOfrects[1].Count + rectOfrects[2].Count + rectOfrects[3].Count) > 0;
            int index = 0;
            while (nextAvailable)
            {
                nextAvailable = false;
                if (index < rectOfrects[0].Count)
                {
                    rects.Add(rectOfrects[0][index]);
                    nextAvailable = true;
                }
                if (index < rectOfrects[1].Count)
                {
                    rects.Add(rectOfrects[1][index]);
                    nextAvailable = true;
                }
                if (index < rectOfrects[2].Count)
                {
                    rects.Add(rectOfrects[2][index]);
                    nextAvailable = true;
                }
                if (index < rectOfrects[3].Count)
                {
                    rects.Add(rectOfrects[3][index]);
                    nextAvailable = true;
                }
                index++;
            }

            return rects;
        }

        public List<NetSquare> getChildSquaresLimited(NetSquare NetSquare, int maxSquares)
        {
            List<NetSquare> rects = new List<NetSquare>();
            List<NetSquare> lowermostChildren = getNotNullChildren(NetSquare);

            while (rects.Count < maxSquares && lowermostChildren.Count > 0)
            {
                List<NetSquare> buffer = new List<NetSquare>();
                foreach (NetSquare s in lowermostChildren)
                {
                    rects.Add(s);
                    buffer.AddRange(getNotNullChildren(s));
                }
                lowermostChildren = buffer;
            }

            return rects;
        }

        public List<NetSquare> getNotNullChildren(NetSquare NetSquare)
        {
            List<NetSquare> output = new List<NetSquare>();
            foreach (NetSquare s in NetSquare.children)
                if (s != null) output.Add(s);
            return output;
        }

        public List<NetSquare> getChildSquares(NetSquare baseSquare, int[] indexes)
        {
            List<NetSquare> children = new List<NetSquare>();
            foreach (int index in indexes)
                if (baseSquare.children[index] != null) children.Add(baseSquare.children[index]);

            return children;
        }

        public List<NetSquare> getLayer(NetSquare startSquare, int level)
        {
            List<NetSquare> layer = new List<NetSquare>();
            for (int i = 0; i < startSquare.children.Length; i++)
                if (startSquare.children[i] == null) continue;
                else if (startSquare.children[i].level == level)
                    layer.Add(startSquare.children[i]);
                else layer.AddRange(getLayer(startSquare.children[i], level));

            return layer;
        }

        public NetSquare[] getSquaresArray(int[] path, bool emulateNullSquares = false)
        {
            List<NetSquare> output = new List<NetSquare>();
            NetSquare s = zeroSquare;
            foreach (int i in path)
                if (s.children[i] != null)
                {
                    output.Add(s);
                    s = s.children[i];
                }
                else if (emulateNullSquares)
                {
                    for (int ii = i; ii < path.Length; ii++)
                    {
                        output.Add(s);
                        s = calcChild(s, ii);
                    }
                    break;
                }
                else break;

            return output.ToArray();
        }

        //View functions
        public bool findSquaresInViewRect(NetSquare baseSquare, Rect viewRect, int maxSquaresCount = 500, int deepness = -1)
        {
            if (deepness == -1) deepness = maxDepth;

            List<NetSquare> visibleSquares = new List<NetSquare>();
            NetSquare NetSquare = baseSquare;
            Point[] points = new Point[]
            {
                new Point(viewRect.X, viewRect.Y),
                new Point(viewRect.X + viewRect.Width, viewRect.Y),
                new Point(viewRect.X, viewRect.Y + viewRect.Height),
                new Point(viewRect.X + viewRect.Width, viewRect.Y + viewRect.Height)
            };

            for (int ii = 0; ii < 4; ii++)
            {
                if (points[ii].X > 180) points[ii].X = 180;
                else if (points[ii].X < -180) points[ii].X = -180;
                if (points[ii].Y > 90) points[ii].Y = 90;
                else if (points[ii].Y < -90) points[ii].Y = -90;
            }

            int[][] pathes = new int[][]
            {
                getPathToSquare(points[0].Y, points[0].X, baseSquare.level),
                getPathToSquare(points[1].Y, points[1].X, baseSquare.level),
                getPathToSquare(points[2].Y, points[2].X, baseSquare.level),
                getPathToSquare(points[3].Y, points[3].X, baseSquare.level)
            };

            Point centerPoint = new Point(viewRect.X + viewRect.Width / 2, viewRect.Y + viewRect.Height / 2);
            int[] centerPath = getPathToSquare(centerPoint.Y, centerPoint.X, baseSquare.level);

            NetSquare[] centerSquares = getSquaresArray(centerPath, true);
            NetSquare lowermostCommonSquare = baseSquare;
            for (int i = centerSquares.Length - 1; i > -1; i--)
            {
                NetSquare s = centerSquares[i];
                Rect rect = new Rect(s.lng, s.lat, getSWidth(s), getSHeight(s));
                bool p0In = rect.Contains(points[0]);
                bool p1In = rect.Contains(points[1]);
                bool p2In = rect.Contains(points[2]);
                bool p3In = rect.Contains(points[3]);
                if (p0In && p1In && p2In && p3In)
                {
                    if (i == centerSquares.Length - 1) i--;
                    lowermostCommonSquare = centerSquares[i];
                    break;
                }
            }

            if (lowermostCommonSquare == lastUpperSquare)
                return false;

            lastUpperSquare = lowermostCommonSquare;

            visibleSquares.Add(lowermostCommonSquare);

            List<NetSquare> squares = getChildSquaresLimited(lowermostCommonSquare, maxSquaresCount);
            foreach (NetSquare s in squares)
            {
                Rect rect = new Rect(s.lng, s.lat, getSWidth(s), getSHeight(s));
                bool p0In = rect.Contains(points[0]);
                bool p1In = rect.Contains(points[1]);
                bool p2In = rect.Contains(points[2]);
                bool p3In = rect.Contains(points[3]);

                if (p0In && p1In && p2In && p3In)
                    visibleSquares.Add(s);
            }


            squaresCache = visibleSquares.ToArray();

            return true;
        }

        public NetSquare[] getCachedSquares()
        {
            return squaresCache;
        }

        //Additional        
        private int getChildIndex(ref double squareLat, ref double squareLng, double lat, double lng, int level)
        {
            double sHeight = getSHeight(level);
            double sWidth = getSWidth(level); //squareWidth
            double centerLat = squareLat + sHeight / 2;
            double centerLng = squareLng + sWidth / 2;

            //can I replace it??
            int index = 0;
            if (lat >= centerLat)
            {
                if (lng >= centerLng)
                {
                    index = 3;
                    squareLng = centerLng;
                }
                else index = 2;
                squareLat = centerLat;
            }
            else if (lng >= centerLng)
            {
                index = 1;
                squareLng = centerLng;
            }

            return index;
        }

        private int getChildIndex(double squareLat, double squareLng, double lat, double lng, int level)
        {
            double sHeight = getSHeight(level);
            double sWidth = getSWidth(level); //squareWidth
            double centerLat = squareLat + sHeight / 2;
            double centerLng = squareLng + sWidth / 2;

            //can I replace it??
            int index = 0;
            if (lat > centerLat)
            {
                if (lng > centerLng)
                {
                    index = 3;
                    squareLng = centerLng;
                }
                else index = 2;
                squareLat = centerLat;
            }
            else if (lng > centerLng)
            {
                index = 1;
                squareLng = centerLng;
            }

            return index;
        }

        public int[] getPathToSquare(double lat, double lng, int startLevel = 0)
        {
            int[] path = new int[maxDepth - 1];
            double squareLat = -getSHeight(startLevel) / 2;
            double squareLng = -getSWidth(startLevel) / 2;
            for (int level = startLevel; level < maxDepth - 1; level++)
                path[level] = getChildIndex(ref squareLat, ref squareLng, lat, lng, level);

            return path;
        }

        private double getSWidth(NetSquare s)
        {
            return 360 / Math.Pow(2, s.level);
        }

        private double getSWidth(int level)
        {
            return 360 / Math.Pow(2, level);
        }

        private double getSHeight(NetSquare s)
        {
            return 180 / Math.Pow(2, s.level);
        }

        private double getSHeight(int level)
        {
            return 180 / Math.Pow(2, level);
        }

        //Utils
        public bool pointInRect(Rect rect, Point p)
        {
            return p.X > rect.X && p.X < rect.X + rect.Width && p.Y > rect.Y && p.Y < rect.Y + rect.Height;
        }
    }
}
