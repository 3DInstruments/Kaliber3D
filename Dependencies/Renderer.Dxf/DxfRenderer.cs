﻿//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using netDxf;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Header;
using netDxf.IO;
using netDxf.Objects;
using netDxf.Tables;
using netDxf.Units;

namespace netDxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfRenderer : Kaliber3D.Render.ObservableObject, Kaliber3D.Render.IRenderer
    {
        private bool _enableImageCache = true;
        private IDictionary<string, ImageDef> _biCache;
        private double _pageWidth;
        private double _pageHeight;
        private string _outputPath;
        private Layer _currentLayer;
        private Kaliber3D.Render.RendererState _state = new Kaliber3D.Render.RendererState();

        /// <summary>
        /// 
        /// </summary>
        public Kaliber3D.Render.RendererState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DxfRenderer()
        {
            ClearCache(isZooming: false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Kaliber3D.Render.IRenderer Create()
        {
            return new DxfRenderer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="container"></param>
        public void Save(string path, Kaliber3D.Render.Container container)
        {
            _outputPath = System.IO.Path.GetDirectoryName(path);
            var doc = new DxfDocument(DxfVersion.AutoCad2010);
            Add(doc, container);
            doc.Save(path);
            ClearCache(isZooming: false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="container"></param>
        private void Add(DxfDocument doc, Kaliber3D.Render.Container container)
        {
            _pageWidth = container.Width;
            _pageHeight = container.Height;

            if (container.Template != null)
            {
                Draw(doc, container.Template, container.Properties, null);
            }

            Draw(doc, container, container.Properties, null);
        }

        private static double LineweightFactor = 96.0 / 2540.0;
        private static short[] Lineweights = { -3, -2, -1, 0, 5, 9, 13, 15, 18, 20, 25, 30, 35, 40, 50, 53, 60, 70, 80, 90, 100, 106, 120, 140, 158, 200, 211 };

        private static short ThicknessToLineweight(double thickness)
        {
            short lineweight = (short)(thickness / LineweightFactor);
            return Lineweights.OrderBy(x => Math.Abs((long)x - lineweight)).First();
        }

        private static AciColor GetColor(Kaliber3D.Render.ArgbColor color)
        {
            return new AciColor(color.R, color.G, color.B);
        }

        private static short GetTransparency(Kaliber3D.Render.ArgbColor color)
        {
            return (short)(90.0 - color.A * 90.0 / 255.0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double ToDxfX(double x)
        {
            return x;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        private double ToDxfY(double y)
        {
            return _pageHeight - y;
        }

        private Line CreateLine(double x1, double y1, double x2, double y2)
        {
            double _x1 = ToDxfX(x1);
            double _y1 = ToDxfY(y1);
            double _x2 = ToDxfX(x2);
            double _y2 = ToDxfY(y2);
            return new Line(new Vector3(_x1, _y1, 0), new Vector3(_x2, _y2, 0));
        }

        private Ellipse CreateEllipse(double x, double y, double width, double height)
        {
            double _cx = ToDxfX(x + width / 2.0);
            double _cy = ToDxfY(y + height / 2.0);
            double minor = Math.Min(height, width);
            double major = Math.Max(height, width);

            return new Ellipse()
            {
                Center = new Vector3(_cx, _cy, 0),
                MajorAxis = major,
                MinorAxis = minor,
                StartAngle = 0.0,
                EndAngle = 360.0,
                Rotation = height > width ? 90.0 : 0.0
            };
        }

        private Ellipse CreateEllipticalArc(Kaliber3D.Render.XArc arc, double dx, double dy)
        {
            var a = Kaliber3D.Render.GdiArc.FromXArc(arc, dx, dy);

            double _cx = ToDxfX(a.X + a.Width / 2.0);
            double _cy = ToDxfY(a.Y + a.Height / 2.0);
            double minor = Math.Min(a.Height, a.Width);
            double major = Math.Max(a.Height, a.Width);
            double startAngle = -a.EndAngle;
            double endAngle = -a.StartAngle;
            double rotation = 0;

            if (a.Height > a.Width)
            {
                startAngle += 90;
                endAngle += 90;
                rotation = -90;
            }

            return new Ellipse()
            {
                Center = new Vector3(_cx, _cy, 0),
                MajorAxis = major,
                MinorAxis = minor,
                StartAngle = startAngle,
                EndAngle = endAngle,
                Rotation = rotation
            };
        }

        private Spline CreateQuadraticSpline(double p1x, double p1y, double p2x, double p2y, double p3x, double p3y)
        {
            double _p1x = ToDxfX(p1x);
            double _p1y = ToDxfY(p1y);
            double _p2x = ToDxfX(p2x);
            double _p2y = ToDxfY(p2y);
            double _p3x = ToDxfX(p3x);
            double _p3y = ToDxfY(p3y);

            return new Spline(
                new List<SplineVertex>
                {
                    new SplineVertex(_p1x, _p1y, 0.0),
                    new SplineVertex(_p2x, _p2y, 0.0),
                    new SplineVertex(_p3x, _p3y, 0.0)
                }, 2);
        }

        private Spline CreateCubicSpline(double p1x, double p1y, double p2x, double p2y, double p3x, double p3y, double p4x, double p4y)
        {
            double _p1x = ToDxfX(p1x);
            double _p1y = ToDxfY(p1y);
            double _p2x = ToDxfX(p2x);
            double _p2y = ToDxfY(p2y);
            double _p3x = ToDxfX(p3x);
            double _p3y = ToDxfY(p3y);
            double _p4x = ToDxfX(p4x);
            double _p4y = ToDxfY(p4y);

            return new Spline(
                new List<SplineVertex>
                {
                    new SplineVertex(_p1x, _p1y, 0.0),
                    new SplineVertex(_p2x, _p2y, 0.0),
                    new SplineVertex(_p3x, _p3y, 0.0),
                    new SplineVertex(_p4x, _p4y, 0.0)
                }, 3);
        }

        private void DrawRectangleInternal(DxfDocument doc, Layer layer, bool isFilled, bool isStroked, Kaliber3D.Render.BaseStyle style, ref Kaliber3D.Render.Rect2 rect)
        {
            double x = rect.X;
            double y = rect.Y;
            double w = rect.Width;
            double h = rect.Height;

            var dxfLine1 = CreateLine(x, y, x + w, y);
            var dxfLine2 = CreateLine(x + w, y, x + w, y + h);
            var dxfLine3 = CreateLine(x + w, y + h, x, y + h);
            var dxfLine4 = CreateLine(x, y + h, x, y);

            if (isFilled)
            {
                var fill = GetColor(style.Fill);
                var fillTransparency = GetTransparency(style.Fill);

                var bounds =
                    new List<HatchBoundaryPath>
                    {
                        new HatchBoundaryPath(
                            new List<EntityObject>
                            {
                                (Line)dxfLine1.Clone(),
                                (Line)dxfLine2.Clone(),
                                (Line)dxfLine3.Clone(),
                                (Line)dxfLine4.Clone()
                            })
                    };

                var hatch = new Hatch(HatchPattern.Solid, bounds, false);
                hatch.Layer = layer;
                hatch.Color = fill;
                hatch.Transparency.Value = fillTransparency;

                doc.AddEntity(hatch);
            }

            if (isStroked)
            {
                var stroke = GetColor(style.Stroke);
                var strokeTansparency = GetTransparency(style.Stroke);
                var lineweight = ThicknessToLineweight(style.Thickness);

                dxfLine1.Layer = layer;
                dxfLine1.Color = stroke;
                dxfLine1.Transparency.Value = strokeTansparency;
                dxfLine1.Lineweight.Value = lineweight;

                dxfLine2.Layer = layer;
                dxfLine2.Color = stroke;
                dxfLine2.Transparency.Value = strokeTansparency;
                dxfLine2.Lineweight.Value = lineweight;

                dxfLine3.Layer = layer;
                dxfLine3.Color = stroke;
                dxfLine3.Transparency.Value = strokeTansparency;
                dxfLine3.Lineweight.Value = lineweight;

                dxfLine4.Layer = layer;
                dxfLine4.Color = stroke;
                dxfLine4.Transparency.Value = strokeTansparency;
                dxfLine4.Lineweight.Value = lineweight;

                doc.AddEntity(dxfLine1);
                doc.AddEntity(dxfLine2);
                doc.AddEntity(dxfLine3);
                doc.AddEntity(dxfLine4);
            }
        }

        private void DrawEllipseInternal(DxfDocument doc, Layer layer, bool isFilled, bool isStroked, Kaliber3D.Render.BaseStyle style, ref Kaliber3D.Render.Rect2 rect)
        {
            var dxfEllipse = CreateEllipse(rect.X, rect.Y, rect.Width, rect.Height);

            if (isFilled)
            {
                var fill = GetColor(style.Fill);
                var fillTransparency = GetTransparency(style.Fill);

                // TODO: The netDxf does not create hatch for Ellipse with end angle equal to 360.
                var bounds =
                    new List<HatchBoundaryPath>
                    {
                        new HatchBoundaryPath(
                            new List<EntityObject>
                            {
                                (Ellipse)dxfEllipse.Clone()
                            })
                    };

                var hatch = new Hatch(HatchPattern.Solid, bounds, false);
                hatch.Layer = layer;
                hatch.Color = fill;
                hatch.Transparency.Value = fillTransparency;

                doc.AddEntity(hatch);
            }

            if (isStroked)
            {
                var stroke = GetColor(style.Stroke);
                var strokeTansparency = GetTransparency(style.Stroke);
                var lineweight = ThicknessToLineweight(style.Thickness);

                dxfEllipse.Layer = layer;
                dxfEllipse.Color = stroke;
                dxfEllipse.Transparency.Value = strokeTansparency;
                dxfEllipse.Lineweight.Value = lineweight;

                doc.AddEntity(dxfEllipse);
            }
        }

        private void DrawGridInternal(DxfDocument doc, Layer layer, Kaliber3D.Render.ShapeStyle style, double offsetX, double offsetY, double cellWidth, double cellHeight, ref Kaliber3D.Render.Rect2 rect)
        {
            var stroke = GetColor(style.Stroke);
            var strokeTansparency = GetTransparency(style.Stroke);
            var lineweight = ThicknessToLineweight(style.Thickness);

            double ox = rect.X;
            double oy = rect.Y;
            double sx = ox + offsetX;
            double sy = oy + offsetY;
            double ex = ox + rect.Width;
            double ey = oy + rect.Height;

            for (double gx = sx; gx < ex; gx += cellWidth)
            {
                var dxfLine = CreateLine(gx, oy, gx, ey);
                dxfLine.Layer = layer;
                dxfLine.Color = stroke;
                dxfLine.Transparency.Value = strokeTansparency;
                dxfLine.Lineweight.Value = lineweight;
                doc.AddEntity(dxfLine);
            }

            for (double gy = sy; gy < ey; gy += cellHeight)
            {
                var dxfLine = CreateLine(ox, gy, ex, gy);
                dxfLine.Layer = layer;
                dxfLine.Color = stroke;
                dxfLine.Transparency.Value = strokeTansparency;
                dxfLine.Lineweight.Value = lineweight;
                doc.AddEntity(dxfLine);
            }
        }

        private void CreateHatchBoundsAndEntitiess(Kaliber3D.Render.XPathGeometry pg, double dx, double dy, out ICollection<HatchBoundaryPath> bounds, out ICollection<EntityObject> entities)
        {
            bounds = new List<HatchBoundaryPath>();
            entities = new List<EntityObject>();

            // TODO: FillMode = pg.FillRule == Kaliber3D.Render.XFillRule.EvenOdd ? FillMode.Alternate : FillMode.Winding;

            foreach (var pf in pg.Figures)
            {
                var edges = new List<EntityObject>();
                var startPoint = pf.StartPoint;

                foreach (var segment in pf.Segments)
                {
                    if (segment is Kaliber3D.Render.XArcSegment)
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                        //var arcSegment = segment as Kaliber3D.Render.XArcSegment;
                        // TODO: Convert WPF/SVG elliptical arc segment format to DXF ellipse arc.
                        //startPoint = arcSegment.Point;
                    }
                    else if (segment is Kaliber3D.Render.XBezierSegment)
                    {
                        var bezierSegment = segment as Kaliber3D.Render.XBezierSegment;
                        var dxfSpline = CreateCubicSpline(
                            startPoint.X + dx,
                            startPoint.Y + dy,
                            bezierSegment.Point1.X + dx,
                            bezierSegment.Point1.Y + dy,
                            bezierSegment.Point2.X + dx,
                            bezierSegment.Point2.Y + dy,
                            bezierSegment.Point3.X + dx,
                            bezierSegment.Point3.Y + dy);
                        edges.Add(dxfSpline);
                        entities.Add((Spline)dxfSpline.Clone());
                        startPoint = bezierSegment.Point3;
                    }
                    else if (segment is Kaliber3D.Render.XLineSegment)
                    {
                        var lineSegment = segment as Kaliber3D.Render.XLineSegment;
                        var dxfLine = CreateLine(
                            startPoint.X + dx,
                            startPoint.Y + dy,
                            lineSegment.Point.X + dx,
                            lineSegment.Point.Y + dy);
                        edges.Add(dxfLine);
                        entities.Add((Line)dxfLine.Clone());
                        startPoint = lineSegment.Point;
                    }
                    else if (segment is Kaliber3D.Render.XPolyBezierSegment)
                    {
                        var polyBezierSegment = segment as Kaliber3D.Render.XPolyBezierSegment;
                        if (polyBezierSegment.Points.Count >= 3)
                        {
                            var dxfSpline = CreateCubicSpline(
                                startPoint.X + dx,
                                startPoint.Y + dy,
                                polyBezierSegment.Points[0].X + dx,
                                polyBezierSegment.Points[0].Y + dy,
                                polyBezierSegment.Points[1].X + dx,
                                polyBezierSegment.Points[1].Y + dy,
                                polyBezierSegment.Points[2].X + dx,
                                polyBezierSegment.Points[2].Y + dy);
                            edges.Add(dxfSpline);
                            entities.Add((Spline)dxfSpline.Clone());
                        }

                        if (polyBezierSegment.Points.Count > 3
                            && polyBezierSegment.Points.Count % 3 == 0)
                        {
                            for (int i = 3; i < polyBezierSegment.Points.Count; i += 3)
                            {
                                var dxfSpline = CreateCubicSpline(
                                    polyBezierSegment.Points[i - 1].X + dx,
                                    polyBezierSegment.Points[i - 1].Y + dy,
                                    polyBezierSegment.Points[i].X + dx,
                                    polyBezierSegment.Points[i].Y + dy,
                                    polyBezierSegment.Points[i + 1].X + dx,
                                    polyBezierSegment.Points[i + 1].Y + dy,
                                    polyBezierSegment.Points[i + 2].X + dx,
                                    polyBezierSegment.Points[i + 2].Y + dy);
                                edges.Add(dxfSpline);
                                entities.Add((Spline)dxfSpline.Clone());
                            }
                        }

                        startPoint = polyBezierSegment.Points.Last();
                    }
                    else if (segment is Kaliber3D.Render.XPolyLineSegment)
                    {
                        var polyLineSegment = segment as Kaliber3D.Render.XPolyLineSegment;
                        if (polyLineSegment.Points.Count >= 1)
                        {
                            var dxfLine = CreateLine(
                                startPoint.X + dx,
                                startPoint.Y + dy,
                                polyLineSegment.Points[0].X + dx,
                                polyLineSegment.Points[0].Y + dy);
                            edges.Add(dxfLine);
                            entities.Add((Line)dxfLine.Clone());
                        }

                        if (polyLineSegment.Points.Count > 1)
                        {
                            for (int i = 1; i < polyLineSegment.Points.Count; i++)
                            {
                                var dxfLine = CreateLine(
                                    polyLineSegment.Points[i - 1].X + dx,
                                    polyLineSegment.Points[i - 1].Y + dy,
                                    polyLineSegment.Points[i].X + dx,
                                    polyLineSegment.Points[i].Y + dy);
                                edges.Add(dxfLine);
                                entities.Add((Line)dxfLine.Clone());
                            }
                        }

                        startPoint = polyLineSegment.Points.Last();
                    }
                    else if (segment is Kaliber3D.Render.XPolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as Kaliber3D.Render.XPolyQuadraticBezierSegment;
                        if (polyQuadraticSegment.Points.Count >= 2)
                        {
                            var dxfSpline = CreateQuadraticSpline(
                                startPoint.X + dx,
                                startPoint.Y + dy,
                                polyQuadraticSegment.Points[0].X + dx,
                                polyQuadraticSegment.Points[0].Y + dy,
                                polyQuadraticSegment.Points[1].X + dx,
                                polyQuadraticSegment.Points[1].Y + dy);
                            edges.Add(dxfSpline);
                            entities.Add((Spline)dxfSpline.Clone());
                        }

                        if (polyQuadraticSegment.Points.Count > 2
                            && polyQuadraticSegment.Points.Count % 2 == 0)
                        {
                            for (int i = 3; i < polyQuadraticSegment.Points.Count; i += 3)
                            {
                                var dxfSpline = CreateQuadraticSpline(
                                    polyQuadraticSegment.Points[i - 1].X + dx,
                                    polyQuadraticSegment.Points[i - 1].Y + dy,
                                    polyQuadraticSegment.Points[i].X + dx,
                                    polyQuadraticSegment.Points[i].Y + dy,
                                    polyQuadraticSegment.Points[i + 1].X + dx,
                                    polyQuadraticSegment.Points[i + 1].Y + dy);
                                edges.Add(dxfSpline);
                                entities.Add((Spline)dxfSpline.Clone());
                            }
                        }

                        startPoint = polyQuadraticSegment.Points.Last();
                    }
                    else if (segment is Kaliber3D.Render.XQuadraticBezierSegment)
                    {
                        var qbezierSegment = segment as Kaliber3D.Render.XQuadraticBezierSegment;
                        var dxfSpline = CreateQuadraticSpline(
                            startPoint.X + dx,
                            startPoint.Y + dy,
                            qbezierSegment.Point1.X + dx,
                            qbezierSegment.Point1.Y + dy,
                            qbezierSegment.Point2.X + dx,
                            qbezierSegment.Point2.Y + dy);
                        edges.Add(dxfSpline);
                        entities.Add((Spline)dxfSpline.Clone());
                        startPoint = qbezierSegment.Point2;
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }

                // TODO: Add support for pf.IsClosed

                var path = new HatchBoundaryPath(edges);
                bounds.Add(path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isZooming"></param>
        public void ClearCache(bool isZooming)
        {
            if (!isZooming)
            {
                if (_biCache != null)
                {
                    _biCache.Clear();
                }
                _biCache = new Dictionary<string, ImageDef>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="container"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.Container container, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            var _doc = doc as DxfDocument;

            foreach (var layer in container.Layers)
            {
                var dxfLayer = new Layer(layer.Name)
                {
                    IsVisible = layer.IsVisible
                };

                _doc.Layers.Add(dxfLayer);

                _currentLayer = dxfLayer;

                Draw(doc, layer, db, r);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="layer"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.Layer layer, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            var _doc = doc as DxfDocument;

            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(_state.DrawShapeState.Flags))
                {
                    shape.Draw(_doc, this, 0, 0, db, r);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="line"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.XLine line, double dx, double dy, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            if (!line.IsStroked)
                return;

            var _doc = doc as DxfDocument;

            var style = line.Style;
            var stroke = GetColor(style.Stroke);
            var strokeTansparency = GetTransparency(style.Stroke);
            var lineweight = ThicknessToLineweight(style.Thickness);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

            Kaliber3D.Render.XLine.SetMaxLength(line, ref _x1, ref _y1, ref _x2, ref _y2);

            var dxfLine = CreateLine(_x1, _y1, _x2, _y2);

            // TODO: Draw line start arrow.

            // TODO: Draw line end arrow.

            dxfLine.Layer = _currentLayer;
            dxfLine.Color = stroke;
            dxfLine.Transparency.Value = strokeTansparency;
            dxfLine.Lineweight.Value = lineweight;

            _doc.AddEntity(dxfLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="rectangle"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.XRectangle rectangle, double dx, double dy, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            if (!rectangle.IsStroked && !rectangle.IsFilled && !rectangle.IsGrid)
                return;

            var _doc = doc as DxfDocument;
            var style = rectangle.Style;
            var rect = Kaliber3D.Render.Rect2.Create(rectangle.TopLeft, rectangle.BottomRight, dx, dy);

            DrawRectangleInternal(_doc, _currentLayer, rectangle.IsFilled, rectangle.IsStroked, style, ref rect);

            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    _doc,
                    _currentLayer,
                    style,
                    rectangle.OffsetX, rectangle.OffsetY,
                    rectangle.CellWidth, rectangle.CellHeight,
                    ref rect);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="ellipse"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.XEllipse ellipse, double dx, double dy, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            if (!ellipse.IsStroked && !ellipse.IsFilled)
                return;

            var _doc = doc as DxfDocument;
            var style = ellipse.Style;
            var rect = Kaliber3D.Render.Rect2.Create(ellipse.TopLeft, ellipse.BottomRight, dx, dy);

            DrawEllipseInternal(_doc, _currentLayer, ellipse.IsFilled, ellipse.IsStroked, style, ref rect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.XArc arc, double dx, double dy, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            var _doc = doc as DxfDocument;
            var style = arc.Style;

            var dxfEllipse = CreateEllipticalArc(arc, dx, dy);

            if (arc.IsFilled)
            {
                var fill = GetColor(style.Fill);
                var fillTransparency = GetTransparency(style.Fill);

                // TODO: The netDxf does not create hatch for Ellipse with end angle equal to 360.
                var bounds =
                    new List<HatchBoundaryPath>
                    {
                        new HatchBoundaryPath(
                            new List<EntityObject>
                            {
                                (Ellipse)dxfEllipse.Clone()
                            })
                    };

                var hatch = new Hatch(HatchPattern.Solid, bounds, false);
                hatch.Layer = _currentLayer;
                hatch.Color = fill;
                hatch.Transparency.Value = fillTransparency;

                _doc.AddEntity(hatch);
            }

            if (arc.IsStroked)
            {
                var stroke = GetColor(style.Stroke);
                var strokeTansparency = GetTransparency(style.Stroke);
                var lineweight = ThicknessToLineweight(style.Thickness);

                dxfEllipse.Layer = _currentLayer;
                dxfEllipse.Color = stroke;
                dxfEllipse.Transparency.Value = strokeTansparency;
                dxfEllipse.Lineweight.Value = lineweight;

                _doc.AddEntity(dxfEllipse);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.XBezier bezier, double dx, double dy, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            if (!bezier.IsStroked && !bezier.IsFilled)
                return;

            var _doc = doc as DxfDocument;
            var style = bezier.Style;

            var dxfSpline = CreateCubicSpline(
                bezier.Point1.X + dx,
                bezier.Point1.Y + dy,
                bezier.Point2.X + dx,
                bezier.Point2.Y + dy,
                bezier.Point3.X + dx,
                bezier.Point3.Y + dy,
                bezier.Point4.X + dx,
                bezier.Point4.Y + dy);

            if (bezier.IsFilled)
            {
                var fill = GetColor(style.Fill);
                var fillTransparency = GetTransparency(style.Fill);

                var bounds =
                    new List<HatchBoundaryPath>
                    {
                        new HatchBoundaryPath(
                            new List<EntityObject>
                            {
                                (Spline)dxfSpline.Clone()
                            })
                    };

                var hatch = new Hatch(HatchPattern.Solid, bounds, false);
                hatch.Layer = _currentLayer;
                hatch.Color = fill;
                hatch.Transparency.Value = fillTransparency;

                _doc.AddEntity(hatch);
            }

            if (bezier.IsStroked)
            {
                var stroke = GetColor(style.Stroke);
                var strokeTansparency = GetTransparency(style.Stroke);
                var lineweight = ThicknessToLineweight(style.Thickness);

                dxfSpline.Layer = _currentLayer;
                dxfSpline.Color = stroke;
                dxfSpline.Transparency.Value = strokeTansparency;
                dxfSpline.Lineweight.Value = lineweight;

                _doc.AddEntity(dxfSpline);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="qbezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.XQBezier qbezier, double dx, double dy, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            if (!qbezier.IsStroked && !qbezier.IsFilled)
                return;

            var _doc = doc as DxfDocument;
            var style = qbezier.Style;

            var dxfSpline = CreateQuadraticSpline(
                qbezier.Point1.X + dx,
                qbezier.Point1.Y + dy,
                qbezier.Point2.X + dx,
                qbezier.Point2.Y + dy,
                qbezier.Point3.X + dx,
                qbezier.Point3.Y + dy);

            if (qbezier.IsFilled)
            {
                var fill = GetColor(style.Fill);
                var fillTransparency = GetTransparency(style.Fill);

                var bounds =
                    new List<HatchBoundaryPath>
                    {
                        new HatchBoundaryPath(
                            new List<EntityObject>
                            {
                                (Spline)dxfSpline.Clone()
                            })
                    };

                var hatch = new Hatch(HatchPattern.Solid, bounds, false);
                hatch.Layer = _currentLayer;
                hatch.Color = fill;
                hatch.Transparency.Value = fillTransparency;

                _doc.AddEntity(hatch);
            }

            if (qbezier.IsStroked)
            {
                var stroke = GetColor(style.Stroke);
                var strokeTansparency = GetTransparency(style.Stroke);
                var lineweight = ThicknessToLineweight(style.Thickness);

                dxfSpline.Layer = _currentLayer;
                dxfSpline.Color = stroke;
                dxfSpline.Transparency.Value = strokeTansparency;
                dxfSpline.Lineweight.Value = lineweight;

                _doc.AddEntity(dxfSpline);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="text"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.XText text, double dx, double dy, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            var _doc = doc as DxfDocument;

            var tbind = text.BindToTextProperty(db, r);
            if (string.IsNullOrEmpty(tbind))
                return;

            var style = text.Style;
            var stroke = GetColor(style.Stroke);
            var strokeTansparency = GetTransparency(style.Stroke);

            var attachmentPoint = default(MTextAttachmentPoint);
            double x, y;
            var rect = Kaliber3D.Render.Rect2.Create(text.TopLeft, text.BottomRight, dx, dy);

            switch (text.Style.TextStyle.TextHAlignment)
            {
                default:
                case Kaliber3D.Render.TextHAlignment.Left:
                    x = rect.X;
                    break;
                case Kaliber3D.Render.TextHAlignment.Center:
                    x = rect.X + rect.Width / 2.0;
                    break;
                case Kaliber3D.Render.TextHAlignment.Right:
                    x = rect.X + rect.Width;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                default:
                case Kaliber3D.Render.TextVAlignment.Top:
                    y = rect.Y;
                    break;
                case Kaliber3D.Render.TextVAlignment.Center:
                    y = rect.Y + rect.Height / 2.0;
                    break;
                case Kaliber3D.Render.TextVAlignment.Bottom:
                    y = rect.Y + rect.Height;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                default:
                case Kaliber3D.Render.TextVAlignment.Top:
                    switch (text.Style.TextStyle.TextHAlignment)
                    {
                        default:
                        case Kaliber3D.Render.TextHAlignment.Left:
                            attachmentPoint = MTextAttachmentPoint.TopLeft;
                            break;
                        case Kaliber3D.Render.TextHAlignment.Center:
                            attachmentPoint = MTextAttachmentPoint.TopCenter;
                            break;
                        case Kaliber3D.Render.TextHAlignment.Right:
                            attachmentPoint = MTextAttachmentPoint.TopRight;
                            break;
                    }
                    break;
                case Kaliber3D.Render.TextVAlignment.Center:
                    switch (text.Style.TextStyle.TextHAlignment)
                    {
                        default:
                        case Kaliber3D.Render.TextHAlignment.Left:
                            attachmentPoint = MTextAttachmentPoint.MiddleLeft;
                            break;
                        case Kaliber3D.Render.TextHAlignment.Center:
                            attachmentPoint = MTextAttachmentPoint.MiddleCenter;
                            break;
                        case Kaliber3D.Render.TextHAlignment.Right:
                            attachmentPoint = MTextAttachmentPoint.MiddleRight;
                            break;
                    }
                    break;
                case Kaliber3D.Render.TextVAlignment.Bottom:
                    switch (text.Style.TextStyle.TextHAlignment)
                    {
                        default:
                        case Kaliber3D.Render.TextHAlignment.Left:
                            attachmentPoint = MTextAttachmentPoint.BottomLeft;
                            break;
                        case Kaliber3D.Render.TextHAlignment.Center:
                            attachmentPoint = MTextAttachmentPoint.BottomCenter;
                            break;
                        case Kaliber3D.Render.TextHAlignment.Right:
                            attachmentPoint = MTextAttachmentPoint.BottomRight;
                            break;
                    }
                    break;
            }

            var ts = new TextStyle(style.TextStyle.FontName, style.TextStyle.FontFile);
            var dxfMText = new MText(
                new Vector3(ToDxfX(x), ToDxfY(y), 0),
                text.Style.TextStyle.FontSize * 72.0 / 96.0,
                rect.Width,
                ts);
            dxfMText.AttachmentPoint = attachmentPoint;

            var fs = text.Style.TextStyle.FontStyle;
            var options = new MTextFormattingOptions(dxfMText.Style);
            options.Bold = fs.Flags.HasFlag(Kaliber3D.Render.FontStyleFlags.Bold);
            options.Italic = fs.Flags.HasFlag(Kaliber3D.Render.FontStyleFlags.Italic);
            options.Underline = fs.Flags.HasFlag(Kaliber3D.Render.FontStyleFlags.Underline);
            options.StrikeThrough = fs.Flags.HasFlag(Kaliber3D.Render.FontStyleFlags.Strikeout);

            options.Aligment = MTextFormattingOptions.TextAligment.Default;
            options.Color = null;
            dxfMText.Write(tbind, options);

            dxfMText.Layer = _currentLayer;
            dxfMText.Transparency.Value = strokeTansparency;
            dxfMText.Color = stroke;

            _doc.AddEntity(dxfMText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="image"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.XImage image, double dx, double dy, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            var _doc = doc as DxfDocument;

            var bytes = _state.ImageCache.GetImage(image.Path);
            if (bytes != null)
            {
                var rect = Kaliber3D.Render.Rect2.Create(image.TopLeft, image.BottomRight, dx, dy);

                if (_enableImageCache
                    && _biCache.ContainsKey(image.Path))
                {
                    var dxfImageDefinition = _biCache[image.Path];
                    var dxfImage = new Image(
                        dxfImageDefinition,
                        new Vector3(ToDxfX(rect.X), ToDxfY(rect.Y + rect.Height), 0),
                        rect.Width,
                        rect.Height);
                    _doc.AddEntity(dxfImage);
                }
                else
                {
                    if (_state.ImageCache == null || string.IsNullOrEmpty(image.Path))
                        return;

                    var path = System.IO.Path.Combine(_outputPath, System.IO.Path.GetFileName(image.Path));
                    System.IO.File.WriteAllBytes(path, bytes);
                    var dxfImageDefinition = new ImageDef(path);

                    if (_enableImageCache)
                        _biCache[image.Path] = dxfImageDefinition;

                    var dxfImage = new Image(
                        dxfImageDefinition,
                        new Vector3(ToDxfX(rect.X), ToDxfY(rect.Y + rect.Height), 0),
                        rect.Width,
                        rect.Height);
                    _doc.AddEntity(dxfImage);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="path"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object doc, Kaliber3D.Render.XPath path, double dx, double dy, ImmutableArray<Kaliber3D.Render.ShapeProperty> db, Kaliber3D.Render.Record r)
        {
            if (!path.IsStroked && !path.IsFilled)
                return;

            var _doc = doc as DxfDocument;
            var style = path.Style;

            ICollection<HatchBoundaryPath> bounds;
            ICollection<EntityObject> entities;
            CreateHatchBoundsAndEntitiess(path.Geometry, dx, dy, out bounds, out entities);
            if (entities == null || bounds == null)
                return;

            if (path.IsFilled)
            {
                var fill = GetColor(style.Fill);
                var fillTransparency = GetTransparency(style.Fill);

                var hatch = new Hatch(HatchPattern.Solid, bounds, false);
                hatch.Layer = _currentLayer;
                hatch.Color = fill;
                hatch.Transparency.Value = fillTransparency;

                _doc.AddEntity(hatch);
            }

            if (path.IsStroked)
            {
                // TODO: Add support for Closed paths.

                var stroke = GetColor(style.Stroke);
                var strokeTansparency = GetTransparency(style.Stroke);
                var lineweight = ThicknessToLineweight(style.Thickness);

                foreach (var entity in entities)
                {
                    entity.Layer = _currentLayer;
                    entity.Color = stroke;
                    entity.Transparency.Value = strokeTansparency;
                    entity.Lineweight.Value = lineweight;
                    _doc.AddEntity(entity);
                }
            }
        }
    }
}
