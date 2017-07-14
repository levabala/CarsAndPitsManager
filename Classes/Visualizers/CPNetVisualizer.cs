using CarsAndPitsWPF2.Classes.DataTypes;
using CarsAndPitsWPF2.Classes.Nets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CarsAndPitsWPF2.Classes.Visualizers
{
    public abstract class CPNetVisualizer : Canvas
    {
        protected Net net;
        protected DrawElement topLayer;
        protected Canvas bottomLayer;
        protected RectGeo viewArea;
        protected Func<PointGeo, Point> transformFunc;

        public CPNetVisualizer()
        {            
            bottomLayer = new Canvas();            
        }
        
        public void Invalidate()
        {
            topLayer.InvalidateVisual();
        }

        //public Properties
        public Net Net
        {
            get { return net; }
            set
            {
                net = value;
                topLayer.net = value;
            }
        }

        protected internal class DrawElement : FrameworkElement
        {
            public Net net;
            public RectGeo viewArea;
            public Func<PointGeo, Point> transformFunc;
            public int visibleSquaresCount;
            public SquareRect[] sRectsCache = new SquareRect[0];

            public DrawElement(Func<PointGeo, Point> transformFunc)
            {
                viewArea = new RectGeo(new PointGeo(-90, -180), 180, 360);
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                PointGeo[] edges = new PointGeo[]
                {
                    new PointGeo(viewArea.Lat, viewArea.Lng),
                    new PointGeo(viewArea.Lat - viewArea.HeightLat, viewArea.Lng + viewArea.WidthLng)
                };
                bool changed = net.findSquaresInViewRect(
                    net.zeroSquare,
                    new Rect(new Point(edges[0].Lng, edges[0].Lat), new Point(edges[1].Lng, edges[1].Lat)),
                    2000);

                NetSquare[] squaresToRender = net.getCachedSquares();
                if (squaresToRender == null)
                {
                    drawRect(new SquareRect(net.zeroSquare, net.maxDepth), drawingContext);
                    foreach (NetSquare square in net.getChildSquares(net.zeroSquare, 2))
                        drawRect(new SquareRect(square, net.maxDepth), drawingContext);
                    return;
                }

                if (changed)
                    sRectsCache = generateSRects(squaresToRender);

                visibleSquaresCount = squaresToRender.Length;
                foreach (SquareRect sRect in sRectsCache)
                    drawRect(sRect, drawingContext);
            }

            private SquareRect[] generateSRects(NetSquare[] squares)
            {
                SquareRect[] sRects = new SquareRect[squares.Length];

                for (int i = 0; i < squares.Length; i++)
                    sRects[i] = new SquareRect(squares[i], net.maxDepth);

                return sRects;
            }

            private void drawRect(SquareRect sRect, DrawingContext context)
            {
                Point p1 = transformFunc(new PointGeo(sRect.rect.Y + sRect.rect.Height, sRect.rect.X));
                Point p2 = transformFunc(new PointGeo(sRect.rect.Y, sRect.rect.X + sRect.rect.Width));

                Rect rect = new Rect(p1.X, p1.Y, p2.X - p2.X, p2.Y - p2.Y);
                context.DrawRectangle(sRect.brush, sRect.pen, rect);
            }

            public struct SquareRect
            {
                public Rect rect;
                public Brush brush;
                public Pen pen;
                public int[] path;

                public SquareRect(NetSquare square, int maxDepth)
                {
                    double width = 360 / Math.Pow(2, square.level);
                    double height = 180 / Math.Pow(2, square.level);
                    double left = square.lng;// + 180;
                    double top = square.lat;// + 90;
                    rect = new Rect(left, top, width, height);

                    double intesity = 0.5;

                    Color color;
                    color = Color.FromRgb(255, (byte)(255 * intesity), 255);

                    brush = new SolidColorBrush(color);
                    brush.Opacity = 0.1;

                    pen = new Pen(Brushes.Black, width * 0.003 * Math.Pow(2, square.level));

                    path = square.path;
                }
            }
        }
    }
}
