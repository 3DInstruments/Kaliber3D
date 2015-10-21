﻿//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Kaliber3D.Render
{
    /// <summary>
    /// 
    /// </summary>
    public class XImage : XText
    {
        private string _path;

        /// <summary>
        /// 
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { Update(ref _path, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void Bind(Record r)
        {
            base.Bind(r ?? this.Data.Record);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="renderer"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var record = r ?? this.Data.Record;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db, record);
                base.Draw(dc, renderer, dx, dy, db, record);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="path"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XImage Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point,
            string path,
            bool isStroked = false,
            bool isFilled = false,
            string text = null,
            string name = "")
        {
            return new XImage()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Data = new Data()
                {
                    Bindings = ImmutableArray.Create<ShapeBinding>(),
                    Properties = ImmutableArray.Create<ShapeProperty>()
                },
                TopLeft = XPoint.Create(x1, y1, point),
                BottomRight = XPoint.Create(x2, y2, point),
                Path = path,
                Text = text
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="path"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XImage Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            string path,
            bool isStroked = false,
            bool isFilled = false,
            string text = null,
            string name = "")
        {
            return Create(x, y, x, y, style, point, path, isStroked, isFilled, text, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="path"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XImage Create(
            XPoint topLeft,
            XPoint bottomRight,
            ShapeStyle style,
            BaseShape point,
            string path,
            bool isStroked = false,
            bool isFilled = false,
            string text = null,
            string name = "")
        {
            return new XImage()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Data = new Data()
                {
                    Bindings = ImmutableArray.Create<ShapeBinding>(),
                    Properties = ImmutableArray.Create<ShapeProperty>()
                },
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Path = path,
                Text = text
            };
        }
    }
}
