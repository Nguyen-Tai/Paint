using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOANLTWD.Shapes
{
    class Polygon:FillShape
    {
        public List<Point> Points { get; set; } = new List<Point>();

        protected override GraphicsPath GraphicsPathObject
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                if (this.Points.Count < 3)
                {
                    path.AddLine(this.Points[0], this.Points[1]);
                }
                else
                {
                    path.AddPolygon(Points.ToArray());
                }

                return path;
            }
        }


        protected override GraphicsPath GraphicsPathPoint
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
              //  path.AddRectangle(new System.Drawing.Rectangle(p1.X - 2, p1.Y - 2, 5, 5));
                path.AddRectangle(new System.Drawing.Rectangle(p2.X - 2, p2.Y - 2, 5, 5));
                return path;
            }
        }

        public override void Draw(Graphics myGp)
        {
            using (GraphicsPath path = GraphicsPathObject)
            {
                if (!Fill)
                {
                    using (Pen pen = new Pen(this.pen.Color, this.pen.Width) { DashStyle = this.pen.DashStyle })
                    {
                        myGp.DrawPath(pen, path);
                    }
                }
                else
                {
                    if (Points.Count < 3)
                    {
                        using (Pen pen = new Pen(this.hatch.BackgroundColor, this.pen.Width))
                        {
                            myGp.DrawPath(pen, path);
                        }
                    }
                    else
                    {
                            using (HatchBrush hbrush = new HatchBrush(this.hatch.HatchStyle, this.hatch.ForegroundColor, this.hatch.BackgroundColor))
                            {
                                myGp.FillPath(hbrush, path);
                            }
                    }
                }
            }
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

        public override bool IsHit(Point point)
        {
            bool res = false;
            using (GraphicsPath path = GraphicsPathObject)
            {
                if (!Fill)
                {
                    using (Pen pen = new Pen(this.pen.Color, this.pen.Width + 3))
                    {
                        res = path.IsOutlineVisible(point, pen);
                    }
                }
                else
                {
                    res = path.IsVisible(point);
                }
            }
            return res;
        }

        public override bool IsHitPoint(Point point)
        {
            return false;
        }

        public override void Move(Point distance)
        {
            this.p1 = new Point(this.p1.X + distance.X, this.p1.Y + distance.Y);
            this.p2 = new Point(this.p2.X + distance.X, this.p2.Y + distance.Y);
            for (int i = 0; i < this.Points.Count; i++)
            {
                this.Points[i] = new Point(this.Points[i].X + distance.X, this.Points[i].Y + distance.Y);
            }
        }
    }
}
