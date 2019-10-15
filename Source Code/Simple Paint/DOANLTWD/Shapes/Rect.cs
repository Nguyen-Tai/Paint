using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOANLTWD.Shapes
{
    class Rect:FillShape
    {
        protected override GraphicsPath GraphicsPathObject
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                if (this.p2.X < this.p1.X && this.p2.Y < this.p1.Y) //trên trái
                {
                    path.AddRectangle(new Rectangle(this.p2, new Size(this.p1.X - this.p2.X, this.p1.Y - this.p2.Y)));
                }
                else if (this.p1.X > this.p2.X && this.p1.Y < this.p2.Y) 
                {
                    path.AddRectangle(new System.Drawing.Rectangle(new Point(this.p2.X, this.p1.Y), new Size(this.p1.X - this.p2.X, this.p2.Y - this.p1.Y)));
                }
                else if (this.p1.X < this.p2.X && this.p1.Y > this.p2.Y)
                {
                    path.AddRectangle(new System.Drawing.Rectangle(new Point(this.p1.X, this.p2.Y), new Size(this.p2.X - this.p1.X, this.p1.Y - this.p2.Y)));
                }
                else  // xuống phải
                {
                    path.AddRectangle(new System.Drawing.Rectangle(this.p1, new Size(this.p2.X - this.p1.X, this.p2.Y - this.p1.Y)));
                }
                return path;
            }
        }


        protected override GraphicsPath GraphicsPathPoint
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                if (this.p2.X < this.p1.X && this.p2.Y < this.p1.Y) //trên trái
                {
                    path.AddRectangle(new Rectangle(p2.X-6,p2.Y-6, 6,6));
                }
                else if (this.p1.X > this.p2.X && this.p1.Y < this.p2.Y)  //trên phải
                {
                    path.AddRectangle(new Rectangle(p2.X-6, p2.Y, 6, 6));
                }
                else if (this.p1.X < this.p2.X && this.p1.Y > this.p2.Y) //xuống trái 
                {
                    path.AddRectangle(new Rectangle(p2.X, p2.Y-6, 6, 6));
                }
                else  // xuống phải
                {
                    path.AddRectangle(new Rectangle(p2.X, p2.Y, 6, 6));
                }
                //path.AddRectangle(new System.Drawing.Rectangle(p2.X , p2.Y, 6, 6));
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
                        using (HatchBrush hbrush = new HatchBrush(this.hatch.HatchStyle, this.hatch.ForegroundColor, this.hatch.BackgroundColor))
                        {
                            myGp.FillPath(hbrush, path);
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
