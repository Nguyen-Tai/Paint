using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOANLTWD
{
    public abstract class ShapeObject 
    {
        public Point p1;
        public Point p2;
        public Pen pen;
        public HatchBrush hatch;
        public bool IsSelected;
        public bool IsResize;
        public abstract void Draw(Graphics myGp);
        public abstract void FillPoint(Graphics myGp);
        public abstract void Move(Point distance);
        public abstract bool IsHit(Point point);  // kiểm tra tọa độ so với object
        public abstract bool IsHitPoint(Point point);
        protected abstract GraphicsPath GraphicsPathObject { get; }
        protected abstract GraphicsPath GraphicsPathPoint { get; }
    }
}
