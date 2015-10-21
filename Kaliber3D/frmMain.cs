using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Docking;
using Kaliber3D.Modules;
using System.Globalization;
using System.Threading.Tasks;
using Kaliber3D.Render;
using System.Diagnostics;
using System.Reflection;

namespace Kaliber3D
{
    public partial class frmMain : RibbonForm, IView
    {
        private Drawable _drawable;
        private string _logFileName = "Kaliber3D.log";

        private List<double> _pressures, _angles;
        private List<double> _majorpressures, _majorangles;
        private List<double> _interpressures, _interangles;
        private List<double> _minorpressures, _minorangles;


        /// <summary>
        /// 
        /// </summary>
        public object DataContext { get; set; }

        public frmMain()
        {
            InitializeComponent();
            InitSkinGallery();
            InitializeContext();
            FormClosing += (s, e) => DeInitializeContext();

            InitializePanel();

            SetContainerInvalidation();


            SetContainerInvalidation();

            //HandlePanelShorcutKeys();
            //HandleMenuShortcutKeys();
            HandleFileDialogs();

            //UpdateToolMenu();
            //UpdateOptionsMenu();

            var usEnglish = new CultureInfo("en-US");
        }

        /// <summary>
        /// Gets the location of the assembly as specified originally.
        /// </summary>
        /// <returns>The location of the assembly as specified originally.</returns>
        private string GetAssemblyPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return System.IO.Path.GetDirectoryName(path);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeContext()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.DoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            var context = new EditorContext()
            {
                View = this,
                Renderers = new IRenderer[] { new EmfRenderer(72.0 / 96.0) },
                ProjectFactory = new Default3D(),
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };

            context.Renderers[0].State.EnableAutofit = true;
            context.InitializeEditor(new TraceLog(), System.IO.Path.Combine(GetAssemblyPath(), _logFileName));
            context.Editor.Renderers[0].State.DrawShapeState.Flags = ShapeStateFlags.Visible;
            context.Editor.GetImageKey = async () => await GetImageKey();

            context.Invalidate = this.InvalidateContainer;

            DataContext = context;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeInitializeContext()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            context.Dispose();
        }

        void InitSkinGallery()
        {
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void InitializePanel()
        {
            _pressures = new InitializedList<double>();
            _angles = new InitializedList<double>();
            _majorpressures = new InitializedList<double>();
            _majorangles = new InitializedList<double>();
            _interpressures = new InitializedList<double>();
            _interangles = new InitializedList<double>();
            _minorpressures = new InitializedList<double>();
            _minorangles = new InitializedList<double>();

            var context = DataContext as EditorContext;


            if (context == null)
                return;

            _drawable = new Drawable();

            _drawable.Context = context;
            _drawable.Initialize();

            _drawable.Dock = DockStyle.Fill;
            _drawable.Name = "containerPanel";
            _drawable.Margin = new System.Windows.Forms.Padding(0);
            _drawable.TabIndex = 0;

            this.SuspendLayout();
            this.dockPanel2.Controls.Add(_drawable);
            this.ResumeLayout(false);

            _drawable.Select();
        }
        /// <summary>
        /// 
        /// </summary>
        private void InvalidateContainer()
        {
            SetContainerInvalidation();

            var context = DataContext as EditorContext;
            if (context == null || context.Editor.Project == null)
            {
                _drawable.Invalidate();
            }
            else
            {
                var container = context.Editor.Project.CurrentContainer;
                if (container != null)
                {
                    container.Invalidate();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ResetZoom()
        {
            _drawable.ResetZoom();
            InvalidateContainer();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AutoFit()
        {
            _drawable.AutoFit();
            InvalidateContainer();
        }
        /// <summary>
        /// 
        /// </summary>
        private void HandleFileDialogs()
        {
            this.openFileDialog1.FileOk += (sender, e) =>
            {
                var context = DataContext as EditorContext;
                if (context == null)
                    return;

                string path = openFileDialog1.FileName;
                int filterIndex = openFileDialog1.FilterIndex;
                context.Open(path);
                InvalidateContainer();
            };

            this.saveFileDialog1.FileOk += (sender, e) =>
            {
                var context = DataContext as EditorContext;
                if (context == null || context.Editor.Project == null)
                    return;

                string path = saveFileDialog1.FileName;
                int filterIndex = saveFileDialog1.FilterIndex;
                context.Save(path);
            };

            this.saveFileDialog2.FileOk += (sender, e) =>
            {
                var context = DataContext as EditorContext;
                if (context == null || context.Editor.Project == null)
                    return;

                string path = saveFileDialog2.FileName;
                int filterIndex = saveFileDialog2.FilterIndex;
                switch (filterIndex)
                {
                    case 1:
                        context.ExportAsPdf(path, context.Editor.Project);
                        Process.Start(path);
                        break;
                    case 2:
                        context.ExportAsDxf(path);
                        Process.Start(path);
                        break;
                    default:
                        break;
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetContainerInvalidation()
        {
            var context = DataContext as EditorContext;
            if (context == null || context.Editor.Project == null)
                return;

            var container = context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            foreach (var layer in container.Layers)
            {
                layer.InvalidateLayer += (s, e) => _drawable.Invalidate();
            }

            if (container.WorkingLayer != null)
            {
                container.WorkingLayer.InvalidateLayer += (s, e) => _drawable.Invalidate();
            }

            if (container.HelperLayer != null)
            {
                container.HelperLayer.InvalidateLayer += (s, e) => _drawable.Invalidate();
            }
        }
        /// <summary>
        /// No longer use
        /// </summary>
        //private void SetPanelSize()
        //{
        //    var context = DataContext as EditorContext;
        //    if (context == null)
        //        return;

        //    var container = context.Editor.Project.CurrentContainer;
        //    if (container == null)
        //        return;

        //    int width = (int)container.Width;
        //    int height = (int)container.Height;

        //    int x = (this.Width - width) / 2;
        //    int y = (this.Height - height) / 2;

        //    _drawable.Location = new System.Drawing.Point(x, y);
        //    _drawable.Size = new System.Drawing.Size(width, height);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetImageKey()
        {
            var context = DataContext as EditorContext;
            if (context == null || context.Editor.Project == null)
                return null;

            openFileDialog2.Filter = "All (*.*)|*.*";
            openFileDialog2.FilterIndex = 0;
            var result = openFileDialog2.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                var path = openFileDialog2.FileName;
                var bytes = System.IO.File.ReadAllBytes(path);
                var key = context.Editor.Project.AddImageFromFile(path, bytes);
                return await Task.Run(() => key);
            }
            return null;
        }
        private void ribbonControl_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            configPane1.digitalGauge1.Text = DateTime.Now.ToString("hmm.ss tt");
            configPane1.digitalGauge3.Text = DateTime.Now.ToString("hmm.ss tt");
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            chartPane.Visibility = DockVisibility.Visible;
        }
        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            test1dLinearRegression();
        }

        private void PressurePanel_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            if (e.Button == PressurePanel.CustomHeaderButtons[0])
            {
                KaliberDataset.Tables["TableReading"].Clear();
            }
        }

        /// <summary>Convert the previous pressure to the new unit.</summary>
        /// <summary>PressureUnitConverter(Pressure.ParseUnit(string unit), Convert.ToDouble(value), Pressure.ParseUnit(string unit));</summary>
        public double PressureUnitConverter(PressureUnit prevUnit, double prevValue, PressureUnit newUnit)
        {
            // Construct from dynamic unit and value
            var prevPressure = Pressure.From(prevValue, prevUnit);

            // Convert to the new unit
            double newPressureValue = prevPressure.As(newUnit);
            return newPressureValue;
        }


        #region [Testing]
        public void test1dLinearRegression()
        {
             List<double> _temppressures = new List<double>();
             List<double> _tempangles = new List<double>();
             _pressures.Clear();
             _angles.Clear();
            //Pressure
            _pressures.Add(5.31E-04F);
            _pressures.Add(25.581153F);
            _pressures.Add(52.471346F);
            _pressures.Add(79.532937F);
            _pressures.Add(106.8682F);
            _pressures.Add(146.13019F);
            _pressures.Add(173.5059F);
            _pressures.Add(206.2034F);
            _pressures.Add(236.15716F);
            _pressures.Add(266.12751F);
            _pressures.Add(296.10766F);

            //Angles
            _angles.Add(0F);
            _angles.Add(23.073F);
            _angles.Add(47.316F);
            _angles.Add(70.958F);
            _angles.Add(95.561F);
            _angles.Add(129.898F);
            _angles.Add(154.082F);
            _angles.Add(183.04F);
            _angles.Add(209.667F);
            _angles.Add(236.482F);
            _angles.Add(262.677F);
            _temppressures.Clear();
            DataTable table = KaliberDataset.Tables["TableReading"];
            DataRow row = table.NewRow();
            LinearInterpolation _poldata = new LinearInterpolation(_pressures, _pressures.Count, _angles);
            _poldata.extrapolate = true;
            table.Clear();

            for (int count = 0; count < _pressures.Count; count++)
            {
                Double convertedunit = PressureUnitConverter(Pressure.ParseUnit("psi"), _pressures[count], Pressure.ParseUnit(configPane1.CBOuterValue));
                _temppressures.Add(MathHelper.RoundToNearest(convertedunit, 0.002));
                _tempangles.Add(_angles[count]);
            }

            //CONVERT THE PRESSURE FIRST BECAUSE IT IS NOT AT THE RIGHT POSITION 300 PSI is like 291092 other unit << need it to be whole number
            //Major Interpolation
            for (int count = 0; count <= 300; count += 50)
            {
                _majorpressures.Add(count);
                _majorangles.Add(Convert.ToSingle(MathHelper.RoundToNearest(_poldata.value(count), 0.002)));

                Console.Write(" x" + count + "    y:" + MathHelper.RoundToNearest(MathHelper.DegreesToRadians(_poldata.value(count)), 0.002) + Environment.NewLine);
                Double convertedcount = PressureUnitConverter(Pressure.ParseUnit("psi"), count, Pressure.ParseUnit(configPane1.CBOuterValue));
                table.Rows.Add(MathHelper.RoundToNearest(convertedcount, 0.002), MathHelper.RoundToNearest(_poldata.value(count), 0.002), configPane1.CBOuterValue, configPane1.CBInnerValue);
            }

            //Inter Interpolation
            for (int count = 0; count <= 300; count += 25)
            {
                _interpressures.Add(count);
                _interangles.Add(Convert.ToSingle(MathHelper.RoundToNearest(_poldata.value(count), 0.002)));

                Console.Write(" x" + count + "    y:" + MathHelper.RoundToNearest(MathHelper.DegreesToRadians(_poldata.value(count)), 0.002) + Environment.NewLine);
                Double convertedcount = PressureUnitConverter(Pressure.ParseUnit("psi"), count, Pressure.ParseUnit(configPane1.CBOuterValue));
                //table.Rows.Add(MathHelper.RoundToNearest(convertedcount, 0.002), MathHelper.RoundToNearest(_poldata.value(count), 0.002), configPane1.CBOuterValue, configPane1.CBInnerValue);
            }

            //Minor Interpolation
            for (int count = 0; count <= 300; count += 5)
            {
                _minorpressures.Add(count);
                _minorangles.Add(Convert.ToSingle(MathHelper.RoundToNearest(_poldata.value(count), 0.002)));

                Console.Write(" x" + count + "    y:" + MathHelper.RoundToNearest(MathHelper.DegreesToRadians(_poldata.value(count)), 0.002) + Environment.NewLine);
                Double convertedcount = PressureUnitConverter(Pressure.ParseUnit("psi"), count, Pressure.ParseUnit(configPane1.CBOuterValue));
                //table.Rows.Add(MathHelper.RoundToNearest(convertedcount, 0.002), MathHelper.RoundToNearest(_poldata.value(count), 0.002), configPane1.CBOuterValue, configPane1.CBInnerValue);
            }


            var rangeTotal = _majorangles[_majorangles.Count - 1] - _majorangles[0];
            var startDeg = (rangeTotal / 2) + 90F;

            var context = DataContext as EditorContext;
            var container = context.Editor.Project.CurrentContainer;
            //Find the radius of the control by dividing the width by 2 
            double radius = 189.12;
            Draw3D();
            //Find the origin of the circle by dividing the width and height of the control 
            XPoint origin = XPoint.Create(container.Width / 2, container.Height / 2);

            if (context == null)
                Debug.Write("---Failed--- Context is null");
            var factory = new Factory(context);
            var majorstyle = ShapeStyle.Create("LogoStyle", 255, 0, 0, 00, 255, 0, 0, 0, 2.0, TextStyle.Create("Wika", "Swis721 BlkCn BT", "SWZ721BE.TTF", 32.0, Kaliber3D.Render.FontStyle.Create(Kaliber3D.Render.FontStyleFlags.Bold)), null, null, null, Kaliber3D.Render.LineCap.Square);
            var majorstyle2 = ShapeStyle.Create("LogoStyle2", 255, 0, 0, 00, 255, 0, 0, 0, 10.0, TextStyle.Create("Wika2", "Swis721 Ex BT", "SWZ721BE.TTF", 14.0, Kaliber3D.Render.FontStyle.Create(Kaliber3D.Render.FontStyleFlags.Bold | Kaliber3D.Render.FontStyleFlags.Italic)), null, null, null, Kaliber3D.Render.LineCap.Square);
            var majorstyle3 = ShapeStyle.Create("LogoStyle3", 255, 0, 0, 00, 255, 0, 0, 0, 4.0, TextStyle.Create("Wika3", "Swis721 Ex BT", "SWZ721BE.TTF", 14.0, Kaliber3D.Render.FontStyle.Create(Kaliber3D.Render.FontStyleFlags.Bold | Kaliber3D.Render.FontStyleFlags.Italic)), null, null, null, Kaliber3D.Render.LineCap.Square);
            var interstyle = ShapeStyle.Create("LogoStyle4", 255, 0, 0, 00, 255, 0, 0, 0, 2.0, TextStyle.Create("Wika4", "Swis721 BlkCn BT", "SWZ721BE.TTF", 20.0, Kaliber3D.Render.FontStyle.Create(Kaliber3D.Render.FontStyleFlags.Bold | Kaliber3D.Render.FontStyleFlags.Bold)), null, null, null, Kaliber3D.Render.LineCap.Square);
            var minorstyle = ShapeStyle.Create("LogoStyle4", 255, 0, 0, 00, 255, 0, 0, 0, 1.0, TextStyle.Create("Wika4", "Swis721 Ex BT", "SWZ721BE.TTF", 14.0, Kaliber3D.Render.FontStyle.Create(Kaliber3D.Render.FontStyleFlags.Bold | Kaliber3D.Render.FontStyleFlags.Italic)), null, null, null, Kaliber3D.Render.LineCap.Square);

            XPoint ArcTopLeft = XPoint.Create(container.Width / 2 - radius, container.Height / 2 - radius);
            XPoint ArcBottomRight = XPoint.Create(container.Width / 2 + radius, container.Height / 2 + radius);
            var singlescalearc = factory.Arc(ArcTopLeft, ArcBottomRight, PointOnCircle(radius, _majorangles[0] - 1.0, origin, startDeg), PointOnCircle(radius, rangeTotal + 1.0, origin, startDeg), true, false);
            singlescalearc.Style = majorstyle3;

            //Draw the Major segments for the clock 
            for (int i = 0; i < _majorpressures.Count; i++)
            {
                var majortick = factory.Line(PointOnCircle(radius, _majorangles[i], origin, startDeg), PointOnCircle(radius - 34.944, _majorangles[i], origin, startDeg));
                majortick.Style = majorstyle;
                var majortick2 = factory.Line(PointOnCircle(radius - 4.005, _majorangles[i], origin, startDeg), PointOnCircle(radius - 6.32, _majorangles[i], origin, startDeg));
                majortick2.Style = majorstyle2;
                //var majortick3 = factory.Line(PointOnCircle(radius - 4.005, MajorAngles[i], origin, startDeg), PointOnCircle(radius - 20, MajorAngles[i], origin, startDeg));
                //majortick3.Style = majorstyle3;
                var majortext = factory.Text(PointOnCircle(radius - 20, _majorangles[i], origin, startDeg), PointOnCircle(radius - 110, _majorangles[i], origin, startDeg), _majorpressures[i].ToString(), false);
                majortext.Style = majorstyle;
            }

            //Draw the Inter segments for the control 
            for (int i = 0; i < _interpressures.Count; i++)
            {
                bool _exist = _majorpressures.Exists(element => element == _interpressures[i]);
                if (_exist == false)
                {
                    var intertick = factory.Line(PointOnCircle(radius - 1, _interangles[i], origin, startDeg), PointOnCircle(radius - 25, _interangles[i], origin, startDeg));
                    intertick.Style = interstyle;
                    var intertext = factory.Text(PointOnCircle(radius - 1, _interangles[i], origin, startDeg), PointOnCircle(radius - 90, _interangles[i], origin, startDeg), _interpressures[i].ToString(), false);
                    intertext.Style = interstyle;
                }
            }

            //Draw the minor segments for the control 
            for (int i = 0; i < _minorpressures.Count; i++)
            {
                bool _exist = _interpressures.Exists(element => element == _minorpressures[i]);
                if (_exist == false)
                {
                    var minortick = factory.Line(PointOnCircle(radius - 1, _minorangles[i], origin, startDeg), PointOnCircle(radius - 19.584, _minorangles[i], origin, startDeg));
                    minortick.Style = minorstyle;
                }
            }
            
            ////  USEFUL STUFF
            ////    if (!data.Ranges.Any()) return null;
            ////var min = data.Ranges.Min(t => t.MinValue);
            ////var max = data.Ranges.Max(t => t.MaxValue);
            ////var rangeTotal = max - min;
            ////var value = data.Value;
            ////var valueAngle = (((value - min) / rangeTotal) * 270) + 135;
            ////foreach (var item in data.Ranges.OrderBy(o => o.MinValue))
            ////{
            ////    g.FillPie(new SolidBrush(ColorTranslator.FromHtml(item.Color)), rec, startDeg,
            ////        (float)(((item.MaxValue - item.MinValue) / rangeTotal) * 270));
            ////    startDeg = (float)(startDeg + (((item.MaxValue - item.MinValue) / rangeTotal) * 270));
            ////}
        }

        private void DrawWika()
        {
            var context = DataContext as EditorContext;
            var container = context.Editor.Project.CurrentContainer;

            XPoint RegisterRight = XPoint.Create(container.Width / 2 + 40F, container.Height / 2 + 152F);
            var logo = ShapeStyle.Create("LogoStyle", 255, 0, 0, 255, 255, 0, 0, 255, 1.0, TextStyle.Create("Wika", "Swis721 Ex BT", "SWZ721BE.TTF", 14.0, Kaliber3D.Render.FontStyle.Create(Kaliber3D.Render.FontStyleFlags.Bold | Kaliber3D.Render.FontStyleFlags.Italic)));
            var factory = new Factory(context);

            var w = factory.Geometry();
            w.BeginFigure(XPoint.Create(372.58405577016975, 430.25669799372895));
            w.LineTo(XPoint.Create(375.29994262060666, 430.25669799372895));
            w.LineTo(XPoint.Create(377.23768211501948, 436.86332162428607));
            w.LineTo(XPoint.Create(379.14490602684316, 430.25669799372895));
            w.LineTo(XPoint.Create(381.64718379915581, 430.25669799372895));
            w.LineTo(XPoint.Create(383.52389212839029, 436.89383720687528));
            w.LineTo(XPoint.Create(385.46163162280311, 430.25669799372895));
            w.LineTo(XPoint.Create(388.19277626453459, 430.25669799372895));
            w.LineTo(XPoint.Create(385.17173358820594, 440.5404493262821));
            w.LineTo(XPoint.Create(382.272753242234, 440.50993374369295));
            w.LineTo(XPoint.Create(380.38078712170488, 433.87279453054657));
            w.LineTo(XPoint.Create(378.519336583765, 440.5404493262821));
            w.LineTo(XPoint.Create(375.58984065520389, 440.50993374369295));
            var wstyle = factory.Path(w, false, true);
            wstyle.Style = logo;

            var i = factory.Geometry();
            i.BeginFigure(XPoint.Create(389.94742226341236, 430.25669799372895));
            i.LineTo(XPoint.Create(392.58702015737634, 430.25669799372895));
            i.LineTo(XPoint.Create(392.57176236608177, 440.50993374369295));
            i.LineTo(XPoint.Create(389.962680054707, 440.50993374369295));
            var istyle = factory.Path(i, false, true);
            istyle.Style = logo;

            var k = factory.Geometry();
            k.BeginFigure(XPoint.Create(395.57754725111585, 430.25669799372895));
            k.LineTo(XPoint.Create(398.26291851896354, 430.27195578502347));
            k.LineTo(XPoint.Create(398.247660727669, 434.11691919125997));
            k.LineTo(XPoint.Create(402.18417088167297, 430.27195578502347));
            k.LineTo(XPoint.Create(405.6171739229556, 430.25669799372889));
            k.LineTo(XPoint.Create(401.28396119529219, 434.49836397362469));
            k.LineTo(XPoint.Create(405.75449404460687, 440.5099337436929));
            k.LineTo(XPoint.Create(402.58087345533232, 440.5099337436929));
            k.LineTo(XPoint.Create(399.37673728346857, 436.32929892897539));
            k.LineTo(XPoint.Create(398.26291851896354, 437.44311769348042));
            k.LineTo(XPoint.Create(398.26291851896354, 440.52519153498747));
            k.LineTo(XPoint.Create(395.54703166852664, 440.55570711757667));
            var kstyle = factory.Path(k, false, true);
            kstyle.Style = logo;

            var a = factory.Geometry();
            a.BeginFigure(XPoint.Create(406.60893035710387, 440.49467595239832));
            a.LineTo(XPoint.Create(410.0419333983865, 430.25669799372889));
            a.LineTo(XPoint.Create(413.84112343073923, 430.27195578502347));
            a.LineTo(XPoint.Create(417.27412647202181, 440.54044932628204));
            a.LineTo(XPoint.Create(414.57433755389826, 440.53240036012534));
            a.LineTo(XPoint.Create(411.94390306448275, 432.6979446008632));
            a.LineTo(XPoint.Create(410.77430738052675, 436.14620543344034));
            a.LineTo(XPoint.Create(413.07823386600973, 436.13094764214577));
            a.LineTo(XPoint.Create(413.81060784815, 438.31281179727205));
            a.LineTo(XPoint.Create(413.81060784815, 438.31281179727205));
            a.LineTo(XPoint.Create(410.0419333983865, 438.32806958856662));
            a.LineTo(XPoint.Create(409.3095594162462, 440.52519153498747));
            var astyle = factory.Path(a, false, true);
            astyle.Style = logo;

            var box = factory.Geometry();
            box.BeginFigure(XPoint.Create(422.28365468527687, 440.50201842969892));
            box.LineTo(XPoint.Create(422.28365468527687, 425.53060722355633));
            box.LineTo(XPoint.Create(367.98835777911279, 425.53060722355633));
            box.LineTo(XPoint.Create(367.98835777911279, 445.17340504644926));
            box.LineTo(XPoint.Create(427.03162141049859, 445.17340504644926));
            box.LineTo(XPoint.Create(427.03162141049859, 425.53060722355633));
            box.LineTo(XPoint.Create(424.4278977224738, 425.53060722355633));
            box.LineTo(XPoint.Create(424.4278977224738, 442.56968135842448));
            box.LineTo(XPoint.Create(370.70695162984458, 442.56968135842448));
            box.LineTo(XPoint.Create(370.70695162984458, 428.09604085734549));
            box.LineTo(XPoint.Create(419.56506083454514, 428.09604085734549));
            box.LineTo(XPoint.Create(419.56506083454514, 440.46372837546323));
            var boxstyle = factory.Path(box, false, true);
            boxstyle.Style = logo;

            //Register Right
            var line4 = factory.Text(RegisterRight, RegisterRight, "\u00AE", false);
            line4.Style = ShapeStyle.Create("TextWika", 255, 0, 0, 255, 255, 0, 0, 255, 1.0, TextStyle.Create("Wika", "Swis721 BT", "", 14.0, Kaliber3D.Render.FontStyle.Create(FontStyleFlags.Bold)));
        }

        private void Draw3D()
        {
            var context = DataContext as EditorContext;
            var container = context.Editor.Project.CurrentContainer;
            var logo = ShapeStyle.Create("LogoStyle", 255, 0, 0, 0, 255, 0, 0, 0, 1.0, TextStyle.Create("2D", "Swis721 Ex BT", "SWZ721BE.TTF", 12.0, Kaliber3D.Render.FontStyle.Create(Kaliber3D.Render.FontStyleFlags.Bold | Kaliber3D.Render.FontStyleFlags.Italic)));

            //Number 3
            var factory = new Factory(context);
            var g3 = factory.Geometry();
            g3.BeginFigure(XPoint.Create(394.99696547095516, 371.46794717111896));
            g3.BezierTo(XPoint.Create(402.01410451853485, 371.93063904395785), XPoint.Create(405.34829347796841, 361.18649518106793), XPoint.Create(403.70604304939127, 359.29922506964658));
            g3.BezierTo(XPoint.Create(403.70604304939127, 359.29922506964658), XPoint.Create(409.79460694841924, 348.07583833788379), XPoint.Create(403.21480873729519, 348.36264488659788));
            g3.LineTo(XPoint.Create(383.38224205707934, 348.369751477921));
            g3.LineTo(XPoint.Create(381.538179948309, 353.8972211057237));
            g3.LineTo(XPoint.Create(399.49593134876829, 353.89722110572376));
            g3.LineTo(XPoint.Create(398.3170573217073, 357.36875200571262));
            g3.LineTo(XPoint.Create(383.21181003956696, 357.36875200571257));
            g3.LineTo(XPoint.Create(381.36655298438927, 362.83113045232119));
            g3.LineTo(XPoint.Create(396.41262073025109, 362.77786256095851));
            g3.LineTo(XPoint.Create(395.37223060330894, 365.89431624327659));
            g3.LineTo(XPoint.Create(377.47365873912793, 365.8540665881527));
            g3.LineTo(XPoint.Create(375.54318567519414, 371.50109023491973));
            var g3style = factory.Path(g3, true, true);
            g3style.Style = logo;

            //Letter D
            var gd = factory.Geometry();
            gd.BeginFigure(XPoint.Create(421.35067290935058, 371.430874439893));
            gd.BezierTo(XPoint.Create(425.76218453983472, 371.53157924859954), XPoint.Create(429.1853920229849, 370.47097822257388), XPoint.Create(430.86734109829263, 365.09788188633871));
            gd.LineTo(XPoint.Create(433.99984650706926, 355.957977418667));
            gd.BezierTo(XPoint.Create(435.38633240666456, 352.55842160684261), XPoint.Create(436.35073726966669, 347.98294012763114), XPoint.Create(429.85645896572549, 348.386803149629));
            gd.LineTo(XPoint.Create(411.2021485310305, 348.39902058625665));
            gd.LineTo(XPoint.Create(403.36846419304356, 371.45501793423779));
            gd.LineTo(XPoint.Create(409.8926204204775, 371.4318405746194));
            gd.LineTo(XPoint.Create(415.72950455521561, 354.16939003893492));
            gd.LineTo(XPoint.Create(428.07274155718449, 354.16275852588279));
            gd.LineTo(XPoint.Create(424.13705043851121, 365.72234638091919));
            gd.LineTo(XPoint.Create(411.82343237263825, 365.73031391047266));
            gd.LineTo(XPoint.Create(409.8830258060018, 371.43587760564731));
            var gdstyle = factory.Path(gd, true, true);
            gdstyle.Style = logo;

            //Circle 6 segments
            var c1 = factory.Geometry();
            c1.BeginFigure(XPoint.Create(385.97246591291059, 332.56313517458807));
            c1.BezierTo(XPoint.Create(393.23618441163507, 333.30070163709763), XPoint.Create(398.56573687268479, 336.32048835114546), XPoint.Create(402.52937116299069, 340.1547292848868));
            c1.LineTo(XPoint.Create(398.91432634856272, 343.14798639123313));
            c1.BezierTo(XPoint.Create(393.23618441163507, 337.63865809404496), XPoint.Create(385.69772250701408, 337.00241020670563), XPoint.Create(385.69772250701408, 337.00241020670563));
            var c1style = factory.Path(c1, true, true);
            c1style.Style = logo;

            var c2 = factory.Geometry();
            c2.BeginFigure(XPoint.Create(366.133772387345, 338.00286675626057));
            c2.BezierTo(XPoint.Create(373.47600514775945, 332.33303145794048), XPoint.Create(381.59325136621771, 332.30583800327236), XPoint.Create(383.55118010232826, 332.4418052766133));
            c2.LineTo(XPoint.Create(383.29284228298036, 336.8743383875302));
            c2.BezierTo(XPoint.Create(375.01243533651285, 336.90153184219844), XPoint.Create(370.37595131558447, 340.6270351317421), XPoint.Create(369.27461640152234, 341.37485513511763));
            var c2style = factory.Path(c2, true, true);
            c2style.Style = logo;

            var c3 = factory.Geometry();
            c3.BeginFigure(XPoint.Create(356.14940507109003, 355.83580791834288));
            c3.BezierTo(XPoint.Create(356.91352130384837, 345.673062022657), XPoint.Create(364.28724294996636, 339.50282344313342), XPoint.Create(364.28724294996636, 339.50282344313342));
            c3.LineTo(XPoint.Create(367.4774282217324, 342.86493486727011));
            c3.BezierTo(XPoint.Create(361.05885186656235, 348.86324729442305), XPoint.Create(360.90602862001072, 356.12235150562725), XPoint.Create(360.90602862001072, 356.12235150562725));
            var c3style = factory.Path(c3, true, true);
            c3style.Style = logo;

            var c4 = factory.Geometry();
            c4.BeginFigure(XPoint.Create(362.01399715751029, 373.67792195325006));
            c4.BezierTo(XPoint.Create(355.55721499070233, 366.22778868385626), XPoint.Create(356.0538905419952, 358.03264208752313), XPoint.Create(356.0538905419952, 358.03264208752313));
            c4.LineTo(XPoint.Create(360.79141118509693, 358.35739148644541));
            c4.BezierTo(XPoint.Create(361.05885186656241, 365.31084920454629), XPoint.Create(365.41431439328488, 370.86979479786316), XPoint.Create(365.41431439328488, 370.86979479786316));
            var c4style = factory.Path(c4, true, true);
            c4style.Style = logo;

            var c5 = factory.Geometry();
            c5.BeginFigure(XPoint.Create(380.198706247227, 383.02343934876529));
            c5.BezierTo(XPoint.Create(369.57419020035769, 382.05757425359536), XPoint.Create(363.64577685759048, 375.39643566621646), XPoint.Create(363.64577685759048, 375.39643566621646));
            c5.LineTo(XPoint.Create(366.82647053306391, 372.73198023126491));
            c5.BezierTo(XPoint.Create(373.15455219107383, 379.959315598571), XPoint.Create(380.28197047956928, 381.17497339076766), XPoint.Create(380.28197047956928, 381.17497339076766));
            var c5style = factory.Path(c5, true, true);
            c5style.Style = logo;

            var c6 = factory.Geometry();
            c6.BeginFigure(XPoint.Create(397.41774949560141, 379.24324320042774));
            c6.BezierTo(XPoint.Create(392.32197847625656, 382.90686942348611), XPoint.Create(384.49514063608638, 383.2066206599182), XPoint.Create(382.56341044574651, 383.2066206599182));
            c6.LineTo(XPoint.Create(382.61336898515185, 381.64125309188415));
            c6.BezierTo(XPoint.Create(391.83904592867162, 382.84025803761233), XPoint.Create(397.31783241679074, 379.20993750749085), XPoint.Create(397.31783241679074, 379.20993750749085));
            var c6style = factory.Path(c6, true, true);
            c6style.Style = logo;

            //Accu Label
            XPoint SubHeading = XPoint.Create(container.Width / 2, container.Height / 2 + 113F);
            var line3 = factory.Text(SubHeading, SubHeading, "Accu-Drive", false);
            line3.Style = logo;

            //Register Right
            XPoint RegisterRight = XPoint.Create(container.Width / 2 + 47F, container.Height / 2 + 108F);
            var line4 = factory.Text(RegisterRight, RegisterRight, "\u00AE", false);
            line4.Style = ShapeStyle.Create("RegisterRight", 255, 0, 0, 0, 255, 0, 0, 0, 1.0, TextStyle.Create("3D", "Swis721 BT", "", 12.0, Kaliber3D.Render.FontStyle.Create(Kaliber3D.Render.FontStyleFlags.Bold)));

            //Trademark Sign
            XPoint Trademark = XPoint.Create(container.Width / 2 + 45F, container.Height / 2 + 75F);
            var line5 = factory.Text(Trademark, Trademark, "\u2122", false);
            line5.Style = ShapeStyle.Create("Trademark", 255, 0, 0, 0, 255, 0, 0, 0, 1.0, TextStyle.Create("3D", "Swis721 BT", "", 16.0, Kaliber3D.Render.FontStyle.Create(Kaliber3D.Render.FontStyleFlags.Bold)));
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            Draw3D();
        }

        #endregion
 
        /// <summary>
        /// 
        /// </summary>
        private void OnExport()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            saveFileDialog2.Filter = "Pdf (*.pdf)|*.pdf|Dxf (*.dxf)|*.dxf|All (*.*)|*.*";
            saveFileDialog2.FilterIndex = 0;
            saveFileDialog2.FileName = context.Editor.Project.Name;
            saveFileDialog2.ShowDialog(this);
        }
        #region Point on circle

        /// <summary> 
        /// Find the point on the circumference of a circle 
        /// </summary> 
        /// <param name="radius">The radius of the circle</param> 
        /// <param name="angleInDegrees">The angle of the point to origin</param> 
        /// <param name="origin">The origin of the circle</param> 
        /// <returns>Return the point</returns> 
        private XPoint PointOnCircle(double radius, double angleInDegrees, XPoint origin, double startDeg)
        {
            //Find the x and y using the parametric equation for a circle 
            double x = (radius * Math.Cos((angleInDegrees - startDeg) * Math.PI / 180F)) + origin.X;
            double y = (radius * Math.Sin((angleInDegrees - startDeg) * Math.PI / 180F)) + origin.Y;

            return XPoint.Create(x, y);
        }

        #endregion

        // Assigning a required content for each auto generated Document
        void tabbedView1_QueryControl(object sender, DevExpress.XtraBars.Docking2010.Views.QueryControlEventArgs e)
        {
            if (e.Control == null)
                e.Control = new System.Windows.Forms.Control();
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnExport();
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            AutoFit();
        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            DrawWika();
        }

        private void barButtonItem10_ItemClick(object sender, ItemClickEventArgs e)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            context.Editor.Project.CurrentContainer.Template = context.Editor.Project.Templates.FirstOrDefault(s => s.Name == "Empty");
            InvalidateContainer();
        }

        private void barButtonItem11_ItemClick(object sender, ItemClickEventArgs e)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            context.Editor.Project.CurrentContainer.Template = context.Editor.Project.Templates.FirstOrDefault(s => s.Name == "BoundingBox");
            InvalidateContainer();
        }

        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Add Layer
            Addlayer("layer5");
            DrawWika();
        }

        private void barButtonItem13_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Remove
            Removelayer("layer5");
        }

        private void Addlayer(string name)
        {
            //Add Layer
            var context = DataContext as EditorContext;
            if (context == null)
                return;
            context.OnAddLayer(name);
        }
        private void Removelayer(string name)
        {
            //Remove Layer
            var context = DataContext as EditorContext;
            if (context == null)
                return;
            context.OnRemoveLayer(name);
            InvalidateContainer();
        }
    }
}