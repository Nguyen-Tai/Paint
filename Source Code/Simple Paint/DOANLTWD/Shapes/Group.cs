using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOANLTWD.Shapes
{
    public class Group : ShapeObject, IEnumerable
    {
        public List<ShapeObject> Shapes = new List<ShapeObject>();

        private GraphicsPath[] GraphicsPaths
        {
            get
            {
                GraphicsPath[] paths = new GraphicsPath[Shapes.Count];

                for (int i = 0; i < Shapes.Count; i++)
                {
                    GraphicsPath path = new GraphicsPath();
                    if (Shapes[i] is Line line)
                    {
                        path.AddLine(line.p1, line.p2);
                    }
                    else if (Shapes[i] is Rect rect)
                    {
                        if (rect.p2.X < rect.p1.X && rect.p2.Y < rect.p1.Y) //trên trái
                        {
                            path.AddRectangle(new Rectangle(rect.p2, new Size(rect.p1.X - rect.p2.X, rect.p1.Y - rect.p2.Y)));
                        }
                        else if (rect.p1.X > rect.p2.X && rect.p1.Y < rect.p2.Y)
                        {
                            path.AddRectangle(new System.Drawing.Rectangle(new Point(rect.p2.X, rect.p1.Y), new Size(rect.p1.X - rect.p2.X, rect.p2.Y - rect.p1.Y)));
                        }
                        else if (rect.p1.X < rect.p2.X && rect.p1.Y > rect.p2.Y)
                        {
                            path.AddRectangle(new System.Drawing.Rectangle(new Point(rect.p1.X, rect.p2.Y), new Size(rect.p2.X - rect.p1.X, rect.p1.Y - rect.p2.Y)));
                        }
                        else  // xuống phải
                        {
                            path.AddRectangle(new System.Drawing.Rectangle(rect.p1, new Size(rect.p2.X - rect.p1.X, rect.p2.Y - rect.p1.Y)));
                        }

                    }
                    else if (Shapes[i] is Ellipse ellipse)
                    {
                        if (ellipse is Circle circle)
                        {
                            if (circle.p2.X < circle.p1.X && circle.p2.Y < circle.p1.Y) // lên trái
                                path.AddEllipse(circle.p1.X, circle.p1.Y, -Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)),
                                    -Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)));
                            if (circle.p2.X > circle.p1.X && circle.p2.Y > circle.p1.Y) // xuống phải
                                path.AddEllipse(circle.p1.X, circle.p1.Y, Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)),
                                        Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)));
                            if (circle.p2.X > circle.p1.X && circle.p2.Y < circle.p1.Y) // lên phải
                                path.AddEllipse(circle.p1.X, circle.p1.Y, Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)),
                                    -Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)));
                            if (circle.p2.X < circle.p1.X && circle.p2.Y > circle.p1.Y) // xuống trái
                                path.AddEllipse(circle.p1.X, circle.p1.Y, -Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)),
                                    Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)));
                        }
                        else
                        {
                            if (ellipse.p2.X < ellipse.p1.X && ellipse.p2.Y < ellipse.p1.Y) //trên trái
                            {
                                path.AddEllipse(new Rectangle(ellipse.p2, new Size(ellipse.p1.X - ellipse.p2.X, ellipse.p1.Y - ellipse.p2.Y)));
                            }
                            else if (ellipse.p1.X > ellipse.p2.X && ellipse.p1.Y < ellipse.p2.Y)
                            {
                                path.AddEllipse(new System.Drawing.Rectangle(new Point(ellipse.p2.X, ellipse.p1.Y), new Size(ellipse.p1.X - ellipse.p2.X, ellipse.p2.Y - ellipse.p1.Y)));
                            }
                            else if (ellipse.p1.X < ellipse.p2.X && ellipse.p1.Y > ellipse.p2.Y)
                            {
                                path.AddEllipse(new System.Drawing.Rectangle(new Point(ellipse.p1.X, ellipse.p2.Y), new Size(ellipse.p2.X - ellipse.p1.X, ellipse.p1.Y - ellipse.p2.Y)));
                            }
                            else  // xuống phải
                            {
                                path.AddEllipse(new System.Drawing.Rectangle(ellipse.p1, new Size(ellipse.p2.X - ellipse.p1.X, ellipse.p2.Y - ellipse.p1.Y)));
                            }
                        }
                    }
                    else if (Shapes[i] is Polygon polygon)
                    {
                        path.AddPolygon(polygon.Points.ToArray());
                    }
                    else if(Shapes[i] is Arc arc)
                    {
                        path.AddArc(new System.Drawing.Rectangle(Math.Min(arc.p1.X, arc.p2.X), Math.Min(arc.p1.Y, arc.p2.Y), Math.Abs(arc.p2.X - arc.p1.X), Math.Abs(arc.p2.Y - arc.p1.Y)), arc.start, arc.sweep);

                    }
                    else if (Shapes[i] is Group group)
                    {
                        for (int j = 0; j < group.GraphicsPaths.Length; j++)
                        {
                            path.AddPath(group.GraphicsPaths[j], false);
                        }
                    }
                    paths[i] = path;
                }

                return paths;
            }
        }

        public override void Draw(Graphics graphics)
        {
            GraphicsPath[] paths = GraphicsPaths;
            for (int i = 0; i < paths.Length; i++)
            {
                using (GraphicsPath path = paths[i])
                {
                    if (Shapes[i] is FillShape shape)
                    {
                        if (shape.Fill)
                        {
                            using (HatchBrush hbrush = new HatchBrush(shape.hatch.HatchStyle, shape.hatch.ForegroundColor, shape.hatch.BackgroundColor))
                            {
                                graphics.FillPath(hbrush, path);
                            }
                        }
                        else
                        {
                            using (Pen pen = new Pen(shape.pen.Color, shape.pen.Width) { DashStyle = shape.pen.DashStyle })
                            {
                                graphics.DrawPath(pen, path);
                            }
                        }
                    }
                    else if (Shapes[i] is Group group)
                    {
                        group.Draw(graphics);
                    }
                    else
                    {
                        using (Pen pen = new Pen(Shapes[i].pen.Color, Shapes[i].pen.Width) { DashStyle = Shapes[i].pen.DashStyle })
                        {
                            graphics.DrawPath(pen, path);
                        }
                    }
                }
            }
        }

        public override bool IsHit(Point point)
        {
            GraphicsPath[] paths = GraphicsPaths;
            for (int i = 0; i < paths.Length; i++)
            {
                using (GraphicsPath path = paths[i])
                {                   
                    if (Shapes[i] is Arc arc)
                    {
                        using (Pen pen = new Pen(arc.pen.Color, arc.pen.Width + 3))
                        {
                            if (path.IsOutlineVisible(point, pen))
                            {
                                return true;
                            }
                        }
                    }
                    if(Shapes[i] is Line line)
                    {
                        using (Pen pen = new Pen(line.pen.Color, line.pen.Width + 3))
                        {
                            if (path.IsOutlineVisible(point, pen))
                            {
                                
                                return true;
                            }
                        }
                    }
                    if (Shapes[i] is FillShape fillShape)
                    {
                        if (fillShape.Fill)
                        {
                            using (Brush brush = new SolidBrush(Color.White))
                            {
                                if (path.IsVisible(point))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            using (Pen pen = new Pen(fillShape.pen.Color, fillShape.pen.Width + 3))
                            {
                                if (path.IsOutlineVisible(point, pen))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    if (Shapes[i] is Group group)
                    {
                        return group.IsHit(point);
                    }
                }
            }
            return false;
        }

        public override void Move(Point distance)
        {
            for (int i = 0; i < Shapes.Count; i++)
            {
                if (Shapes[i] is Polygon polygon)
                {
                    polygon.p1 = new Point(polygon.p1.X + distance.X, polygon.p1.Y + distance.Y);
                    polygon.p2 = new Point(polygon.p2.X + distance.X, polygon.p2.Y + distance.Y);

                    for (int j = 0; j < polygon.Points.Count; j++)
                    {
                        polygon.Points[j] = new Point(polygon.Points[j].X + distance.X, polygon.Points[j].Y + distance.Y);
                    }
                }
                else if (Shapes[i] is Group group)
                {
                    group.Move(distance);
                }
                else
                {
                    Shapes[i].p1 = new Point(Shapes[i].p1.X + distance.X, Shapes[i].p1.Y + distance.Y);
                    Shapes[i].p2 = new Point(Shapes[i].p2.X + distance.X, Shapes[i].p2.Y + distance.Y);
                }
            }
            p1 = new Point(p1.X + distance.X, p1.Y + distance.Y);
            p2 = new Point(p2.X + distance.X, p2.Y + distance.Y);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Shapes.GetEnumerator();
        }
        public override void FillPoint(Graphics myGp)
        {
            using (GraphicsPath path = GraphicsPathPoint)
            {
                using (Brush brush = new SolidBrush(Color.White))
                {
                    myGp.FillPath(brush, path);
                }
                using (Pen pen = new Pen(Color.Black) { DashStyle = DashStyle.Dash })
                {
                    myGp.DrawPath(pen, path);
                }
            }
        }

        public override bool IsHitPoint(Point point)
        {
            return false;
        }

        public int Count => Shapes.Count;

        protected override GraphicsPath GraphicsPathObject => throw new NotImplementedException();

        protected override GraphicsPath GraphicsPathPoint
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                path.AddRectangle(new System.Drawing.Rectangle(p1.X - 2, p1.Y - 2, 5, 5));
                path.AddRectangle(new System.Drawing.Rectangle(p2.X - 2, p2.Y - 2, 5, 5));
                return path;
            }
        }
    }
}
