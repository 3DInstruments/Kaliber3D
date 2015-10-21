﻿//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Kaliber3D.Render;

namespace Kaliber3D
{
    /// <summary>
    /// 
    /// </summary>
    internal class Drawable : Panel
    {
        private ZoomState _state;

        /// <summary>
        /// 
        /// </summary>
        public EditorContext Context { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            this.SetStyle(
                ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            this.BackColor = Color.Transparent;

            _state = new ZoomState(Context);

            this.MouseDown +=
                (sender, e) =>
                {
                    var p = e.Location;

                    if (e.Button == MouseButtons.Left)
                    {
                        this.Focus();
                        _state.LeftDown(p.X, p.Y);
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        this.Focus();
                        this.Cursor = Cursors.Hand;
                        _state.RightDown(p.X, p.Y);
                    }
                };

            this.MouseUp +=
                (sender, e) =>
                {
                    var p = e.Location;

                    if (e.Button == MouseButtons.Left)
                    {
                        this.Focus();
                        _state.LeftUp(p.X, p.Y);
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        this.Focus();
                        this.Cursor = Cursors.Default;
                        _state.RightUp(p.X, p.Y);
                    }
                };

            this.MouseMove +=
                (sender, e) =>
                {
                    var p = e.Location;
                    _state.Move(p.X, p.Y);
                };

            this.MouseWheel +=
                (sender, e) =>
                {
                    var p = e.Location;
                    _state.Wheel(p.X, p.Y, e.Delta);
                };
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetZoom()
        {
            if (Context != null && Context.Editor.Project != null)
            {
                var container = Context.Editor.Project.CurrentContainer;
                _state.ResetZoom(
                    this.Width,
                    this.Height,
                    container.Width,
                    container.Height);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void AutoFit()
        {
            if (Context != null && Context.Editor.Project != null)
            {
                var container = Context.Editor.Project.CurrentContainer;
                _state.AutoFit(
                    this.Width,
                    this.Height,
                    container.Width,
                    container.Height);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (Context != null && Context.Editor.Project != null)
            {
                if (Context.Renderers[0].State.EnableAutofit)
                {
                    AutoFit();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Draw(e.Graphics);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="c"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void DrawBackground(Graphics g, ArgbColor c, double width, double height)
        {
            var brush = new SolidBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
            var rect = Rect2.Create(0, 0, width, height);
            g.FillRectangle(
                brush,
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height);
            brush.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        private void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PageUnit = GraphicsUnit.Display;

            g.Clear(Color.FromArgb(255, 211, 211, 211));

            if (Context == null || Context.Editor.Project == null)
                return;

            var container = Context.Editor.Project.CurrentContainer;
            var renderer = Context.Editor.Renderers[0];

            var gs = g.Save();

            g.TranslateTransform((float)_state.PanX, (float)_state.PanY);
            g.ScaleTransform((float)_state.Zoom, (float)_state.Zoom);

            if (container.Template != null)
            {
                DrawBackground(g, container.Template.Background, container.Template.Width, container.Template.Height);
                renderer.Draw(g, container.Template, container.Properties, null);
            }

            DrawBackground(g, container.Background, container.Width, container.Height);
            renderer.Draw(g, container, container.Properties, null);

            if (container.WorkingLayer != null)
            {
                renderer.Draw(g, container.WorkingLayer, container.Properties, null);
            }

            if (container.HelperLayer != null)
            {
                renderer.Draw(g, container.HelperLayer, container.Properties, null);
            }

            g.Restore(gs);
        }
    }
}
