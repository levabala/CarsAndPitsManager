using CarsAndPitsWPF2.Classes.DataTypes;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CarsAndPitsWPF2.Classes.Visualizers
{
    public class CPNetVisualizerGMap : CPNetVisualizer
    {
        private GMapControl mapView;

        public CPNetVisualizerGMap() : base()
        {
            mapView = new GMapControl();
            bottomLayer.Children.Add(mapView);

            transformFunc = transformPoint;
            topLayer = new DrawElement(transformFunc);

            mapView.OnMapDrag += delegate
            {
                updateViewArea();
                Invalidate();
            };
            mapView.OnMapZoomChanged += delegate
            {
                updateViewArea();
                Invalidate();
            };
        }

        private void updateViewArea()
        {
            PointLatLng leftTop = mapView.ViewArea.LocationTopLeft;
            PointLatLng rightBottom = mapView.ViewArea.LocationRightBottom;
            viewArea = new RectGeo(leftTop.Lat, leftTop.Lng, rightBottom.Lat, rightBottom.Lng);
            topLayer.viewArea = viewArea;
        }

        private Point transformPoint(PointGeo point)
        {
            GPoint p = mapView.FromLatLngToLocal(new PointLatLng(point.Lat, point.Lng));
            return new Point(p.X, p.Y);
        }

        private void setUpMap(GMapControl mapView)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            mapView.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            mapView.MinZoom = 2;
            mapView.MaxZoom = 25;
            mapView.Zoom = 2;
            mapView.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            mapView.ShowCenter = false;
            mapView.CanDragMap = true;
            mapView.DragButton = MouseButton.Left;            
        }
    }
}
