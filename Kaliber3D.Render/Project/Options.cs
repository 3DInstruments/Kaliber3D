//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Kaliber3D.Render
{
    /// <summary>
    /// 
    /// </summary>
    public class Options : ObservableObject
    {
        private static MoveMode[] _moveModeValues = (MoveMode[])Enum.GetValues(typeof(MoveMode));
        private static XFillRule[] _xFillRuleValues = (XFillRule[])Enum.GetValues(typeof(XFillRule));
        private bool _snapToGrid = true;
        private double _snapX = 15.0;
        private double _snapY = 15.0;
        private double _hitTreshold = 7.0;
        private MoveMode _moveMode = MoveMode.Point;
        private bool _defaultIsStroked = true;
        private bool _defaultIsFilled = false;
        private bool _defaultIsClosed = true;
        private bool _defaultIsSmoothJoin = true;
        private XFillRule _defaultFillRule = XFillRule.EvenOdd;
        private bool _tryToConnect = false;
        private BaseShape _pointShape;
        private ShapeStyle _pointStyle;
        private ShapeStyle _selectionStyle;
        private ShapeStyle _helperStyle;

        /// <summary>
        /// The MoveMode enum values.
        /// </summary>
        public static MoveMode[] MoveModeValues
        {
            get { return _moveModeValues; }
        }

        /// <summary>
        /// The XFillRule enum values.
        /// </summary>
        public static XFillRule[] XFillRuleValues
        {
            get { return _xFillRuleValues; }
        }

        /// <summary>
        /// Gets or sets how grid snapping is handled. 
        /// </summary>
        public bool SnapToGrid
        {
            get { return _snapToGrid; }
            set { Update(ref _snapToGrid, value); }
        }

        /// <summary>
        /// Gets or sets how much grid X axis is snapped.
        /// </summary>
        public double SnapX
        {
            get { return _snapX; }
            set { Update(ref _snapX, value); }
        }

        /// <summary>
        /// Gets or sets how much grid Y axis is snapped.
        /// </summary>
        public double SnapY
        {
            get { return _snapY; }
            set { Update(ref _snapY, value); }
        }

        /// <summary>
        /// Gets or sets hit test treshold radius.
        /// </summary>
        public double HitTreshold
        {
            get { return _hitTreshold; }
            set { Update(ref _hitTreshold, value); }
        }

        /// <summary>
        /// Gets or sets how selected shapes are moved.
        /// </summary>
        public MoveMode MoveMode
        {
            get { return _moveMode; }
            set { Update(ref _moveMode, value); }
        }

        /// <summary>
        /// Gets or sets value indicating whether path/shape is stroked during creation.
        /// </summary>
        public bool DefaultIsStroked
        {
            get { return _defaultIsStroked; }
            set { Update(ref _defaultIsStroked, value); }
        }

        /// <summary>
        /// Gets or sets value indicating whether path/shape is filled during creation.
        /// </summary>
        public bool DefaultIsFilled
        {
            get { return _defaultIsFilled; }
            set { Update(ref _defaultIsFilled, value); }
        }

        /// <summary>
        /// Gets or sets value indicating whether path is closed during creation.
        /// </summary>
        public bool DefaultIsClosed
        {
            get { return _defaultIsClosed; }
            set { Update(ref _defaultIsClosed, value); }
        }

        /// <summary>
        /// Gets or sets value indicating whether path segment is smooth join during creation.
        /// </summary>
        public bool DefaultIsSmoothJoin
        {
            get { return _defaultIsSmoothJoin; }
            set { Update(ref _defaultIsSmoothJoin, value); }
        }

        /// <summary>
        /// Gets or sets value indicating path fill rule during creation.
        /// </summary>
        public XFillRule DefaultFillRule
        {
            get { return _defaultFillRule; }
            set { Update(ref _defaultFillRule, value); }
        }

        /// <summary>
        /// Gets or sets how point connection is handled.
        /// </summary>
        public bool TryToConnect
        {
            get { return _tryToConnect; }
            set { Update(ref _tryToConnect, value); }
        }

        /// <summary>
        /// Gets or sets shape used to draw points.
        /// </summary>
        public BaseShape PointShape
        {
            get { return _pointShape; }
            set { Update(ref _pointShape, value); }
        }

        /// <summary>
        /// Gets or sets point shape style.
        /// </summary>
        public ShapeStyle PointStyle
        {
            get { return _pointStyle; }
            set { Update(ref _pointStyle, value); }
        }

        /// <summary>
        /// Gets or sets selection rectangle style.
        /// </summary>
        public ShapeStyle SelectionStyle
        {
            get { return _selectionStyle; }
            set { Update(ref _selectionStyle, value); }
        }

        /// <summary>
        /// Gets or sets editor helper shapes style.
        /// </summary>
        public ShapeStyle HelperStyle
        {
            get { return _helperStyle; }
            set { Update(ref _helperStyle, value); }
        }

        /// <summary>
        /// Opacity %   255 Step        2 digit HEX prefix
        /// 0%          0.00            00
        /// 5%          12.75           0C
        /// 10%         25.50           19
        /// 15%         38.25           26
        /// 20%         51.00           33
        /// 25%         63.75           3F
        /// 30%         76.50           4C
        /// 35%         89.25           59
        /// 40%         102.00          66
        /// 45%         114.75          72
        /// 50%         127.50          7F
        /// 55%         140.25          8C
        /// 60%         153.00          99
        /// 65%         165.75          A5
        /// 70%         178.50          B2
        /// 75%         191.25          BF
        /// 80%         204.00          CC
        /// 85%         216.75          D8
        /// 90%         229.50          E5
        /// 95%         242.25          F2
        /// 100%        255.00          FF
        /// </summary>
        /// <returns></returns>
        public static Options Create()
        {
            var options = new Options()
            {
                SnapToGrid = true,
                SnapX = 15.0,
                SnapY = 15.0,
                HitTreshold = 7.0,
                MoveMode = MoveMode.Point,
                DefaultIsStroked = true,
                DefaultIsFilled = false,
                DefaultIsClosed = true,
                DefaultIsSmoothJoin = true,
                DefaultFillRule = XFillRule.EvenOdd,
                TryToConnect = true
            };

            options.SelectionStyle =
                ShapeStyle.Create(
                    "Selection",
                    0xBF, 0x33, 0x99, 0xFF,
                    0x19, 0x33, 0x99, 0xFF,
                    0.1);

            options.HelperStyle =
                ShapeStyle.Create(
                    "Helper",
                    0xFF, 0x33, 0x99, 0xFF,
                    0xFF, 0x00, 0x00, 0x00,
                    0.1);

            options.PointStyle =
                ShapeStyle.Create(
                    "Point",
                    0xFF, 0x33, 0x99, 0xFF,
                    0xFF, 0x00, 0x00, 0x00,
                    0.1);

            options.PointShape = EllipsePointShape(options.PointStyle);
            return options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pss"></param>
        /// <returns></returns>
        public static BaseShape EllipsePointShape(ShapeStyle pss)
        {
            return XEllipse.Create(-2, -2, 2, 2, pss, null, true, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pss"></param>
        /// <returns></returns>
        public static BaseShape FilledEllipsePointShape(ShapeStyle pss)
        {
            return XEllipse.Create(-3, -3, 3, 3, pss, null, true, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pss"></param>
        /// <returns></returns>
        public static BaseShape RectanglePointShape(ShapeStyle pss)
        {
            return XRectangle.Create(-4, -4, 4, 4, pss, null, true, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pss"></param>
        /// <returns></returns>
        public static BaseShape FilledRectanglePointShape(ShapeStyle pss)
        {
            return XRectangle.Create(-3, -3, 3, 3, pss, null, true, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pss"></param>
        /// <returns></returns>
        public static BaseShape CrossPointShape(ShapeStyle pss)
        {
            var g = XGroup.Create("PointShape");

            var builder = g.Shapes.ToBuilder();
            builder.Add(XLine.Create(-4, 0, 4, 0, pss, null));
            builder.Add(XLine.Create(0, -4, 0, 4, pss, null));
            g.Shapes = builder.ToImmutable();

            return g;
        }
    }
}
