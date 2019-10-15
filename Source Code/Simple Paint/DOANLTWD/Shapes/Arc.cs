using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOANLTWD.Shapes
{
    class Arc : ShapeObject
    {
        public float start;
        public float sweep;

        protected override GraphicsPath GraphicsPathObject
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                path.AddArc(new System.Drawing.Rectangle(Math.Min(p1.X,p2.X),Math.Min(p1.Y ,p2.Y), Math.Abs(p2.X - p1.X),Math.Abs( p2.Y - p1.Y)), start, sweep);
                return path;
            }
        }

        protected override GraphicsPath GraphicsPathPoint
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                // path.AddEllipse(new System.Drawing.Rectangle(p1.X-4, p1.Y-4, 16, 16));
                path.AddRectangle(new System.Drawing.Rectangle(p2.X - 2, p2.Y - 2, 5, 5));
                return path;
            }
        }

        public override void Draw(Graphics myGp)
        {
            using (GraphicsPath path = GraphicsPathObject)
            {
                using (Pen pen = new Pen(this.pen.Color, this.pen.Width) { DashStyle = this.pen.DashStyle })
                {
                    myGp.DrawPath(pen, path);
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
                    using (Pen pen = new Pen(this.pen.Color, this.pen.Width + 3))
                    {
                        res = path.IsOutlineVisible(point, pen);
                    }
            }
            return res;
        }

        public override bool IsHitPoint(Point point)
        {
            bool res = false;
            using (GraphicsPath path = GraphicsPathPoint)
            {
                res = path.IsVisible(point);
            }
            return res;
        }

        public override void Move(Point distance)
        {
            this.p1 = new Point(this.p1.X + distance.X, this.p1.Y + distance.Y);
            this.p2 = new Point(this.p2.X + distance.X, this.p2.Y + distance.Y);
        }
    }
}
