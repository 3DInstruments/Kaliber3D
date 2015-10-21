﻿//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Kaliber3D.Render
{
    /// <summary>
    /// 
    /// </summary>
    public class XPathGeometry
    {
        private static XFillRule[] _xFillRuleValues = (XFillRule[])Enum.GetValues(typeof(XFillRule));

        /// <summary>
        /// The XFillRule enum values.
        /// </summary>
        public static XFillRule[] XFillRuleValues
        {
            get { return _xFillRuleValues; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<XPathFigure> Figures { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XFillRule FillRule { get; set; }

        private XPathFigure _figure;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="figures"></param>
        /// <param name="fillRule"></param>
        /// <returns></returns>
        public static XPathGeometry Create(
            IList<XPathFigure> figures,
            XFillRule fillRule)
        {
            return new XPathGeometry()
            {
                Figures = figures,
                FillRule = fillRule
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="isFilled"></param>
        /// <param name="isClosed"></param>
        public void BeginFigure(
            XPoint startPoint,
            bool isFilled = true,
            bool isClosed = true)
        {
            _figure = XPathFigure.Create(
                startPoint,
                new List<XPathSegment>(),
                isFilled,
                isClosed);
            Figures.Add(_figure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void LineTo(
            XPoint point,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XLineSegment.Create(
                point,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="size"></param>
        /// <param name="rotationAngle"></param>
        /// <param name="isLargeArc"></param>
        /// <param name="sweepDirection"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void ArcTo(
            XPoint point,
            XPathSize size,
            double rotationAngle,
            bool isLargeArc = false,
            XSweepDirection sweepDirection = XSweepDirection.Clockwise,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XArcSegment.Create(
                point,
                size,
                rotationAngle,
                isLargeArc,
                sweepDirection,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void BezierTo(
            XPoint point1,
            XPoint point2,
            XPoint point3,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XBezierSegment.Create(
                point1,
                point2,
                point3,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void QuadraticBezierTo(
            XPoint point1,
            XPoint point2,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XQuadraticBezierSegment.Create(
                point1,
                point2,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void PolyLineTo(
            IList<XPoint> points,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XPolyLineSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void PolyBezierTo(
            IList<XPoint> points,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XPolyBezierSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        public void PolyQuadraticBezierTo(
            IList<XPoint> points,
            bool isStroked = true,
            bool isSmoothJoin = true)
        {
            var segment = XPolyQuadraticBezierSegment.Create(
                points,
                isStroked,
                isSmoothJoin);
            _figure.Segments.Add(segment);
        }
    }
}
