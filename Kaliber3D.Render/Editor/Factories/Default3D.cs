//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;

namespace Kaliber3D.Render
{
    /// <summary>
    /// 
    /// </summary>
    public class Default3D : IProjectFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static StyleLibrary DefaultStyleLibrary()
        {
            var sgd = StyleLibrary.Create("Default");

            var builder = sgd.Styles.ToBuilder();
            builder.Add(ShapeStyle.Create("Black", 255, 0, 0, 0, 80, 0, 0, 0, 1.0));
            builder.Add(ShapeStyle.Create("Yellow", 255, 255, 255, 0, 80, 255, 255, 0, 1.0));
            builder.Add(ShapeStyle.Create("Red", 255, 255, 0, 0, 80, 255, 0, 0, 1.0));
            builder.Add(ShapeStyle.Create("Green", 255, 0, 255, 0, 80, 0, 255, 0, 1.0));
            builder.Add(ShapeStyle.Create("Blue", 255, 0, 0, 255, 80, 0, 0, 255, 1.0));
            sgd.Styles = builder.ToImmutable();

            sgd.CurrentStyle = sgd.Styles.FirstOrDefault();

            return sgd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static StyleLibrary LinesStyleLibrary()
        {
            var sgdl = StyleLibrary.Create("Lines");

            var solid = ShapeStyle.Create("Solid", 255, 0, 0, 0, 80, 0, 0, 0, 1.0);
            solid.Dashes = default(string);
            solid.DashOffset = 0.0;

            var dash = ShapeStyle.Create("Dash", 255, 0, 0, 0, 80, 0, 0, 0, 1.0);
            dash.Dashes = "2 2";
            dash.DashOffset = 1.0;

            var dot = ShapeStyle.Create("Dot", 255, 0, 0, 0, 80, 0, 0, 0, 1.0);
            dot.Dashes = "0 2";
            dot.DashOffset = 0.0;

            var dashDot = ShapeStyle.Create("DashDot", 255, 0, 0, 0, 80, 0, 0, 0, 1.0);
            dashDot.Dashes = "2 2 0 2";
            dashDot.DashOffset = 1.0;

            var dashDotDot = ShapeStyle.Create("DashDotDot", 255, 0, 0, 0, 80, 0, 0, 0, 1.0);
            dashDotDot.Dashes = "2 2 0 2 0 2";
            dashDotDot.DashOffset = 1.0;

            var builder = sgdl.Styles.ToBuilder();
            builder.Add(solid);
            builder.Add(dash);
            builder.Add(dot);
            builder.Add(dashDot);
            builder.Add(dashDotDot);
            sgdl.Styles = builder.ToImmutable();

            sgdl.CurrentStyle = sgdl.Styles.FirstOrDefault();

            return sgdl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static StyleLibrary TemplateStyleLibrary()
        {
            var sgt = StyleLibrary.Create("Template");
            var gs = ShapeStyle.Create("Grid", 255, 222, 222, 222, 255, 222, 222, 222, 1.0);
            var gb = ShapeStyle.Create("BoundingBox", 255, 51, 153, 255, 255, 51, 153, 255, 1.0);
            var gt = ShapeStyle.Create("Text", 255, 51, 153, 255, 255, 51, 153, 255, 1.0);

            var builder = sgt.Styles.ToBuilder();
            builder.Add(gs);
            builder.Add(gb);
            builder.Add(gt);
            sgt.Styles = builder.ToImmutable();

            sgt.CurrentStyle = sgt.Styles.FirstOrDefault();

            return sgt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="container"></param>
        private void CreateGrid(Project project, Container container)
        {
            var style = project
                .StyleLibraries.FirstOrDefault(g => g.Name == "Template")
                .Styles.FirstOrDefault(s => s.Name == "Grid");
            var layer = container.Layers.FirstOrDefault();
            var builder = layer.Shapes.ToBuilder();
            var grid = XRectangle.Create(
                0, 0,
                container.Width, container.Height,
                style,
                project.Options.PointShape);
            grid.IsStroked = false;
            grid.IsFilled = false;
            grid.IsGrid = true;
            grid.OffsetX = 30.0;
            grid.OffsetY = 30.0;
            grid.CellWidth = 30.0;
            grid.CellHeight = 30.0;
            grid.State.Flags &= ~ShapeStateFlags.Printable;
            builder.Add(grid);
            layer.Shapes = builder.ToImmutable();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Container CreateGridTemplate(Project project, string name)
        {
            var container = GetTemplate(project, name);

            CreateGrid(project, container);

            return container;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="container"></param>
        private void CreateBoundingBox(Project project, Container container)
        {
            var layer = container.Layers.FirstOrDefault();
            var builder = layer.Shapes.ToBuilder();
            var style = project
                .StyleLibraries.FirstOrDefault(g => g.Name == "Template")
                .Styles.FirstOrDefault(s => s.Name == "Grid");
            var grid = XRectangle.Create(
                0, 0,
                container.Width, container.Height,
                style,
                project.Options.PointShape);
            grid.IsStroked = false;
            grid.IsFilled = false;
            grid.IsGrid = true;
            grid.OffsetX = 30.0;
            grid.OffsetY = 30.0;
            grid.CellWidth = 30.0;
            grid.CellHeight = 30.0;
            grid.State.Flags &= ~ShapeStateFlags.Printable;
            //builder.Add(grid);

            var defaultext = XText.Create(
                container.Width / 2, 10,
                container.Width / 2, 20,
                style,
                project.Options.PointShape,
                "3D Template 4.5 Single Scale");
            defaultext.Style.TextStyle.FontSize = 14;
            defaultext.Style.TextStyle.FontStyle.Flags = FontStyleFlags.Italic;
            defaultext.State.Flags &= ~ShapeStateFlags.Printable;
            //defaultext.Style.Stroke = ArgbColor.Create(0xFF, 0x33, 0x99, 0xff);
            defaultext.Style.TextStyle.FontName = "Swis721 BT";
            builder.Add(defaultext);

            style = project
                .StyleLibraries.FirstOrDefault(g => g.Name == "Template")
                .Styles.FirstOrDefault(s => s.Name == "Text");
            var tr = XLine.Create(
                container.Width / 2 + 340.1568F - 45F, container.Height / 2 - 207.8688F,
                container.Width / 2 + 340.1568F + 45F, container.Height / 2 - 207.8688F,
                style,
                project.Options.PointShape);
            builder.Add(tr);
            var tr2 = XLine.Create(
                container.Width / 2 + 340.1568F, container.Height / 2 - 207.8688F - 45F,
                container.Width / 2 + 340.1568F, container.Height / 2 - 207.8688F + 45F,
                style,
                project.Options.PointShape);
            builder.Add(tr2);
            var tr3 = XEllipse.Create(
                container.Width / 2 + 340.1568F - 25F, container.Height / 2 - 207.8688F - 25F,
                container.Width / 2 + 340.1568F + 25F, container.Height / 2 - 207.8688F + 25F,
                style,
                project.Options.PointShape);
            builder.Add(tr3);

            var br = XLine.Create(
                container.Width / 2 - 340.1568F - 45F, container.Height / 2 + 207.8688F,
                container.Width / 2 - 340.1568F + 45F, container.Height / 2 + 207.8688F,
                style,
                project.Options.PointShape);
            builder.Add(br);
            var br2 = XLine.Create(
                container.Width / 2 - 340.1568F, container.Height / 2 + 207.8688F - 45F,
                container.Width / 2 - 340.1568F, container.Height / 2 + 207.8688F + 45F,
                style,
                project.Options.PointShape);
            builder.Add(br2);
            var br3 = XEllipse.Create(
                container.Width / 2 - 340.1568F - 25F, container.Height / 2 + 207.8688F - 25F,
                container.Width / 2 - 340.1568F + 25F, container.Height / 2 + 207.8688F + 25F,
                style,
                project.Options.PointShape);
            builder.Add(br3);

            style = project
                .StyleLibraries.FirstOrDefault(g => g.Name == "Template")
                .Styles.FirstOrDefault(s => s.Name == "Grid");
            var cr = XLine.Create(
                container.Width / 2 - 6.8736F, container.Height / 2,
                container.Width / 2 + 6.8736F, container.Height / 2,
                style,
                project.Options.PointShape);
            cr.State.Flags &= ~ShapeStateFlags.Printable;
            builder.Add(cr);
            var cr2 = XLine.Create(
                container.Width / 2, container.Height / 2 - 6.8736F,
                container.Width / 2, container.Height / 2 + 6.8736F,
                style,
                project.Options.PointShape);
            cr2.State.Flags &= ~ShapeStateFlags.Printable;
            builder.Add(cr2);
            var cr3 = XEllipse.Create(
                container.Width / 2 - 204.096F - 9.4464F, container.Height / 2 - 204.096F - 9.4464F,
                container.Width / 2 + 204.096F + 9.4464F, container.Height / 2 + 204.096F + 9.4464F,
                style,
                project.Options.PointShape);
            cr3.State.Flags &= ~ShapeStateFlags.Printable;
            builder.Add(cr3);

            var cr4 = XEllipse.Create(
            container.Width / 2 - 9.4464F, container.Height / 2 - 9.4464F,
            container.Width / 2 + 9.4464F, container.Height / 2 + 9.4464F,
            style,
            project.Options.PointShape);
            cr4.State.Flags &= ~ShapeStateFlags.Printable;
            builder.Add(cr4);

            style = project
                .StyleLibraries.FirstOrDefault(g => g.Name == "Template")
                .Styles.FirstOrDefault(s => s.Name == "BoundingBox");
            var boundingbox = XRectangle.Create(
                0, 0,
                container.Width, container.Height,
                style,
                project.Options.PointShape);
            boundingbox.IsStroked = true;
            boundingbox.IsFilled = false;
            boundingbox.IsGrid = true;
            boundingbox.OffsetX = 0;
            boundingbox.OffsetY = 0;
            boundingbox.CellWidth = container.Width;
            boundingbox.CellHeight = container.Height;
            boundingbox.State.Flags &= ~ShapeStateFlags.Printable;
            builder.Add(boundingbox);

            layer.Shapes = builder.ToImmutable();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Container CreateBoundingTemplate(Project project, string name)
        {
            var container = GetTemplate(project, name);

            CreateBoundingBox(project, container);

            return container;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Container GetTemplate(Project project, string name)
        {
            // A5	420 x 595	559 x 794	874 x 1240	1748 x 2480
            var container = Container.Create(name, 794, 559);

            container.IsTemplate = true;
            container.Background = ArgbColor.Create(0xFF, 0xFF, 0xFF, 0xFF);

            foreach (var layer in container.Layers)
            {
                layer.Name = string.Concat("Template", layer.Name);
            }

            return container;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Container GetContainer(Project project, string name)
        {
            var container = Container.Create(name);

            if (project.CurrentTemplate == null)
            {
                var template = GetTemplate(project, "Empty");
                var templateBuilder = project.Templates.ToBuilder();
                templateBuilder.Add(template);
                project.Templates = templateBuilder.ToImmutable();
                project.CurrentTemplate = template;
            }

            container.Template = project.CurrentTemplate;
            container.Width = container.Template.Width;
            container.Height = container.Template.Height;
            return container;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Document GetDocument(Project project, string name)
        {
            var document = Document.Create(name);
            return document;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public Project GetProject()
        {
            var project = Project.Create();

            var glBuilder = project.GroupLibraries.ToBuilder();
            glBuilder.Add(GroupLibrary.Create("Default"));
            project.GroupLibraries = glBuilder.ToImmutable();

            var sgBuilder = project.StyleLibraries.ToBuilder();
            sgBuilder.Add(DefaultStyleLibrary());
            sgBuilder.Add(LinesStyleLibrary());
            sgBuilder.Add(TemplateStyleLibrary());
            project.StyleLibraries = sgBuilder.ToImmutable();

            project.CurrentGroupLibrary = project.GroupLibraries.FirstOrDefault();
            project.CurrentStyleLibrary = project.StyleLibraries.FirstOrDefault();

            var templateBuilder = project.Templates.ToBuilder();
            templateBuilder.Add(GetTemplate(project, "Empty"));
            templateBuilder.Add(CreateGridTemplate(project, "Grid"));
            templateBuilder.Add(CreateBoundingTemplate(project, "BoundingBox"));
            project.Templates = templateBuilder.ToImmutable();

            project.CurrentTemplate = project.Templates.FirstOrDefault(t => t.Name == "Grid");
            project.CurrentTemplate = project.Templates.FirstOrDefault(t => t.Name == "BoundingBox");

            var document = GetDocument(project, "Document");
            var container = GetContainer(project, "Container");

            var containerBuilder = document.Containers.ToBuilder();
            containerBuilder.Add(container);
            document.Containers = containerBuilder.ToImmutable();

            var documentBuilder = project.Documents.ToBuilder();
            documentBuilder.Add(document);
            project.Documents = documentBuilder.ToImmutable();

            project.Selected = document.Containers.FirstOrDefault();

            return project;
        }
    }
}
