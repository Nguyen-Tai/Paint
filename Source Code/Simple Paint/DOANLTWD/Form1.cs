using DOANLTWD.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DOANLTWD
{
    public partial class Form1 : Form
    {
        public Color myColor;
        private Pen framePen = new Pen(Color.Blue, 1)
        {
            DashPattern = new float[] { 3, 3, 3, 3 },
            DashStyle = DashStyle.Custom
        };
        private Brush brush = new SolidBrush(Color.Blue);
        private bool isMouseSelect;
        private System.Drawing.Rectangle selectedRegion;
        private CurrentShape currentShape = CurrentShape.NoDrawing;
        private ShapeMode mode = ShapeMode.NoFill;
        private bool isMovingShape;
        private bool isResizeShape;
        private bool isMouseDown;
        private bool isDrawPolygon;
        private bool isControlKeyPress;
        private Point previousPoint;
        //Danh sách đối tượng đồ họa  
        List<ShapeObject> lstObject = new List<ShapeObject>();
        private ShapeObject selectedShape;  //hình đang được select
        private ShapeObject resizeShape;
        public Form1()
        {
            InitializeComponent();
            myColor = Color.Red;
            useColor.BackColor = myColor;
            cmbShapeMode.SelectedIndex = 0;
            cblineWidth.SelectedIndex = 2;
            cbDashMode.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
        }
        private void UncheckAll()
        {
            btnLine.BackColor = Color.Transparent;
            btnPolygon.BackColor = Color.Transparent;
            btnElipse.BackColor = Color.Transparent;
            btnCircle.BackColor = Color.Transparent;
            btnArc.BackColor = Color.Transparent;
            btnRect.BackColor = Color.Transparent;
            btnSelect.BackColor = Color.Transparent;
        }
        private void FindPolygonRegion(Polygon polygon)
        {
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
            polygon.Points.ForEach(p =>
            {
                if (minX > p.X)
                {
                    minX = p.X;
                }
                if (minY > p.Y)
                {
                    minY = p.Y;
                }
                if (maxX < p.X)
                {
                    maxX = p.X;
                }
                if (maxY < p.Y)
                {
                    maxY = p.Y;
                }
            });
            polygon.p1 = new Point(minX, minY);
            polygon.p2 = new Point(maxX, maxY);
        }
        private void FindGroupRegion(Group group)
        {
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

            for (int i = 0; i < group.Shapes.Count; i++)
            {
                ShapeObject shape = group.Shapes[i];
                if (shape is Polygon polygon)
                {
                    FindPolygonRegion(polygon);
                }


                if (shape.p1.X < minX)
                {
                    minX = shape.p1.X;
                }
                if (shape.p2.X < minX)
                {
                    minX = shape.p2.X;
                }

                if (shape.p1.Y < minY)
                {
                    minY = shape.p1.Y;
                }
                if (shape.p2.Y < minY)
                {
                    minY = shape.p2.Y;
                }

                if (shape.p1.X > maxX)
                {
                    maxX = shape.p1.X;
                }
                if (shape.p2.X > maxX)
                {
                    maxX = shape.p2.X;
                }

                if (shape.p1.Y > maxY)
                {
                    maxY = shape.p1.Y;
                }
                if (shape.p2.Y > maxY)
                {
                    maxY = shape.p2.Y;
                }
            }
            group.p1 = new Point(minX, minY);
            group.p2 = new Point(maxX, maxY);
        }
        private void btnSelect_Click(object sender, EventArgs e)
        {
            lstObject.ForEach(shape => shape.IsSelected = false);
            PanelPaint.Invalidate();
            currentShape = CurrentShape.NoDrawing;
            UncheckAll();
            btnSelect.BackColor = Color.Silver;
            PanelPaint.Cursor = Cursors.Default;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstObject.Count; i++)
            {
                if (lstObject[i].IsSelected)
                {
                    lstObject.RemoveAt(i);
                    i--;
                }
            }
            PanelPaint.Invalidate();
        }

        private void btnUnGroup_Click(object sender, EventArgs e)
        {
            if (lstObject.Find(shape => shape.IsSelected) is Group selectedGroup)
            {
                foreach (ShapeObject shape in selectedGroup)
                {
                    lstObject.Add(shape);
                }
                lstObject.Remove(selectedGroup);
            }
            PanelPaint.Invalidate();
        }

        private void btnGroup_Click(object sender, EventArgs e)
        {
            if (lstObject.Count(shape => shape.IsSelected) > 1)
            {
                Group group = new Group();

                for (int i = 0; i < lstObject.Count; i++)
                {
                    if (lstObject[i].IsSelected)
                    {
                        if (lstObject[i] is Group gr)
                        {
                            for (int j = 0; j < gr.Count; j++)
                            {
                                group.Shapes.Add(gr.Shapes[j]);

                            }
                            lstObject.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            group.Shapes.Add(lstObject[i]);
                            lstObject.RemoveAt(i);
                            i--;
                        }
                    }
                }
                FindGroupRegion(group);
                lstObject.Add(group);
                group.IsSelected = true;
                PanelPaint.Invalidate();
            }
        }
        private void btnColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                useColor.BackColor = colorDialog1.Color;
                myColor = colorDialog1.Color;
                lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
                {
                    if (shape is Group group)
                    {
                        foreach (ShapeObject s in group)
                        {
                            s.pen.Color = myColor;
                        }
                    }
                    else
                        shape.pen.Color = myColor;
                });
                PanelPaint.Invalidate();
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
                myColor = button1.BackColor;
                useColor.BackColor = myColor;
                lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
                {
                    if (shape is Group group)
                    {
                        foreach (ShapeObject s in group)
                        {
                            s.pen.Color = myColor;
                        }
                    }
                    else
                        shape.pen.Color = myColor;
                });
                PanelPaint.Invalidate();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            myColor = button2.BackColor;
            useColor.BackColor = myColor;
            lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
            {
                if (shape is Group group)
                {
                    foreach (ShapeObject s in group)
                    {
                        s.pen.Color = myColor;
                    }
                }
                else
                    shape.pen.Color = myColor;
            });
            PanelPaint.Invalidate();

        }
        private void button3_Click(object sender, EventArgs e)
        {
            myColor = button3.BackColor;
            useColor.BackColor = myColor;
            lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
            {
                if (shape is Group group)
                {
                    foreach (ShapeObject s in group)
                    {
                        s.pen.Color = myColor;
                    }
                }
                else
                    shape.pen.Color = myColor;
            });
            PanelPaint.Invalidate();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            myColor = button4.BackColor;
            useColor.BackColor = myColor;
            lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
            {
                if (shape is Group group)
                {
                    foreach (ShapeObject s in group)
                    {
                        s.pen.Color = myColor;
                    }
                }
                else
                    shape.pen.Color = myColor;
            });
            PanelPaint.Invalidate();

        }
        private void button5_Click(object sender, EventArgs e)
        {
            myColor = button5.BackColor;
            useColor.BackColor = myColor;
            lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
            {
                if (shape is Group group)
                {
                    foreach (ShapeObject s in group)
                    {
                        s.pen.Color = myColor;
                    }
                }
                else
                    shape.pen.Color = myColor;
            });
            PanelPaint.Invalidate();

        }
        private void button6_Click(object sender, EventArgs e)
        {
            myColor = button6.BackColor;
            useColor.BackColor = myColor;
            lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
            {
                if (shape is Group group)
                {
                    foreach (ShapeObject s in group)
                    {
                        s.pen.Color = myColor;
                    }
                }
                else
                    shape.pen.Color = myColor;
            });
            PanelPaint.Invalidate();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            myColor = button7.BackColor;
            useColor.BackColor = myColor;
            lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
            {
                if (shape is Group group)
                {
                    foreach (ShapeObject s in group)
                    {
                        s.pen.Color = myColor;
                    }
                }
                else
                    shape.pen.Color = myColor;
            });
            PanelPaint.Invalidate();

        }
        private void button8_Click(object sender, EventArgs e)
        {
            myColor = button8.BackColor;
            useColor.BackColor = myColor;
            lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
            {
                if (shape is Group group)
                {
                    foreach (ShapeObject s in group)
                    {
                        s.pen.Color = myColor;
                    }
                }
                else
                    shape.pen.Color = myColor;
            });
            PanelPaint.Invalidate();
        }
        private void PanelPaint_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (currentShape == CurrentShape.NoDrawing) // select
                {
                    if (isControlKeyPress)  // nhấn phím controls
                    {
                        for (int i = 0; i < lstObject.Count; i++)
                        {
                            if (lstObject[i].IsHit(e.Location))
                            {
                                lstObject[i].IsSelected = !lstObject[i].IsSelected;
                                PanelPaint.Invalidate();
                                break;
                            }
                        }
                    }
                    else
                    {
                        lstObject.ForEach(shape => shape.IsSelected = false);
                        lstObject.ForEach(shape => shape.IsResize = false);
                        PanelPaint.Invalidate();
                        for (int i = 0; i < lstObject.Count; i++)
                        {
                            if (lstObject[i].IsHit(e.Location) && PanelPaint.Cursor == Cursors.Hand)
                            {
                                selectedShape = lstObject[i];
                                lstObject[i].IsSelected = true;
                                lstObject[i].IsResize = false;
                                PanelPaint.Invalidate();
                                break;
                            }
                        }
                        for (int j = 0; j < lstObject.Count; j++)
                        {
                            if (lstObject[j].IsHitPoint(e.Location) && PanelPaint.Cursor == Cursors.SizeAll)
                            {
                                resizeShape = lstObject[j];
                                lstObject[j].IsResize = true;
                                lstObject[j].IsSelected = true;
                                PanelPaint.Invalidate();
                                break;
                            }
                        }
                        if (selectedShape != null || resizeShape != null)
                        {
                            if (selectedShape != null)
                            {
                                isMovingShape = true;
                                previousPoint = e.Location;
                            }
                            if (resizeShape != null)
                            {
                                isResizeShape = true;
                            }
                        }
                        else
                        {
                            isMouseSelect = true;
                            selectedRegion = new System.Drawing.Rectangle(e.Location, new Size(0, 0));
                        }
                        
                    }
                }
                else
                {
                    isMouseDown = true;
                    lstObject.ForEach(shape => shape.IsSelected = false);
                    if (currentShape == CurrentShape.Line)
                    {
                        Line myObj = new Line();
                        myObj.p1 = e.Location;
                        myObj.pen = new Pen(myColor, Convert.ToInt32(cblineWidth.SelectedItem));
                        myObj.pen.DashStyle = (DashStyle)cbDashMode.SelectedIndex;
                        lstObject.Add(myObj);
                    }
                    else if (currentShape == CurrentShape.Circle)
                    {
                        Circle myObj = new Circle();
                        myObj.p1 = e.Location;
                        myObj.pen = new Pen(myColor, Convert.ToInt32(cblineWidth.SelectedItem));
                        myObj.pen.DashStyle = (DashStyle)cbDashMode.SelectedIndex;
                        if (mode == ShapeMode.Fill)
                        {
                            myObj.Fill = true;
                                HatchStyle hs = (HatchStyle)Enum.Parse(typeof(HatchStyle),
                             comboBox1.SelectedItem.ToString(), true);
                                myObj.hatch = new HatchBrush(hs, forecolor.BackColor, bc.BackColor);
                        }
                        lstObject.Add(myObj);
                    }
                    else if (currentShape == CurrentShape.Ellipse)
                    {
                        Ellipse myObj = new Ellipse();
                        myObj.p1 = e.Location;
                        myObj.pen = new Pen(myColor, Convert.ToInt32(cblineWidth.SelectedItem));
                        myObj.pen.DashStyle = (DashStyle)cbDashMode.SelectedIndex;
                        myObj.hatch = new HatchBrush(HatchStyle.BackwardDiagonal, myColor);
                        if (mode == ShapeMode.Fill)
                        {
                            myObj.Fill = true;
                   
                                HatchStyle hs = (HatchStyle)Enum.Parse(typeof(HatchStyle),
                             comboBox1.SelectedItem.ToString(), true);
                                myObj.hatch = new HatchBrush(hs, forecolor.BackColor, bc.BackColor);
                            
                        }
                        lstObject.Add(myObj);
                    }
                    else if (currentShape == CurrentShape.Rectangle)
                    {
                        Rect myObj = new Rect();
                        myObj.p1 = e.Location;
                        myObj.pen = new Pen(myColor, Convert.ToInt32(cblineWidth.SelectedItem));
                        myObj.pen.DashStyle = (DashStyle)cbDashMode.SelectedIndex;
                        if (mode == ShapeMode.Fill)
                        {
                            myObj.Fill = true;
                      
                            {
                                HatchStyle hs = (HatchStyle)Enum.Parse(typeof(HatchStyle),
                             comboBox1.SelectedItem.ToString(), true);
                                myObj.hatch = new HatchBrush(hs, forecolor.BackColor, bc.BackColor);
                            }
                        }
                        lstObject.Add(myObj);
                    }
                    else if (currentShape == CurrentShape.Arc)
                    {
                        Arc myObj = new Arc();
                        myObj.p1 = e.Location;
                        myObj.pen = new Pen(myColor, Convert.ToInt32(cblineWidth.SelectedItem));
                        myObj.pen.DashStyle = (DashStyle)cbDashMode.SelectedIndex;
                        myObj.start = float.Parse(start.Text.Trim());
                        myObj.sweep = float.Parse(sweep.Text.Trim());
                        lstObject.Add(myObj);
                    }
                    else if (currentShape == CurrentShape.Polygon)
                    {
                        if (!isDrawPolygon)  //  vẽ mới polygon
                        {
                            Polygon myObj = new Polygon();
                            myObj.pen = new Pen(myColor, Convert.ToInt32(cblineWidth.SelectedItem));
                            myObj.pen.DashStyle = (DashStyle)cbDashMode.SelectedIndex;
                            if (mode == ShapeMode.Fill)
                            {
                                myObj.Fill = true;
                               
                                    HatchStyle hs = (HatchStyle)Enum.Parse(typeof(HatchStyle),
                                 comboBox1.SelectedItem.ToString(), true);
                                    myObj.hatch = new HatchBrush(hs, forecolor.BackColor, bc.BackColor);
                                
                            }
                            myObj.Points.Add(e.Location); // add 2 tọa độ để vẽ 
                            myObj.Points.Add(e.Location);
                            lstObject.Add(myObj);
                            isDrawPolygon = true;
                        }
                        else
                        {
                            Polygon polygon = lstObject[lstObject.Count - 1] as Polygon;
                            polygon.Points[polygon.Points.Count - 1] = e.Location;
                            polygon.Points.Add(e.Location);
                        }
                        isMouseDown = false;
                    }
                }
            }
            catch
            {

            }
        }
        private void PanelPaint_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (isMouseDown)
                {
                    lstObject[lstObject.Count - 1].p2 = e.Location;
                    PanelPaint.Invalidate();
                }
                if (isMovingShape)
                {
                    Point d = new Point(e.X - previousPoint.X, e.Y - previousPoint.Y);
                    selectedShape.Move(d);
                    previousPoint = e.Location;
                    PanelPaint.Invalidate();
                }
                if (isResizeShape)
                {
                    if (resizeShape is Line)
                    {
                        resizeShape.p2 = e.Location;
                        PanelPaint.Invalidate();
                    }
                    if (resizeShape is Circle )
                    {
                        if (resizeShape.p2.X > resizeShape.p1.X + 4 && resizeShape.p2.Y > resizeShape.p1.Y + 4) //xuống phải
                        {
                            if (e.Location.X > resizeShape.p1.X + 4 && e.Location.Y > resizeShape.p1.Y + 4)
                                resizeShape.p2 = new Point(resizeShape.p1.X + Math.Min(Math.Abs(e.Location.X - resizeShape.p1.X), Math.Abs(e.Location.Y - resizeShape.p1.Y)), resizeShape.p1.Y
                            + Math.Min(Math.Abs(e.Location.X - resizeShape.p1.X), Math.Abs(e.Location.Y - resizeShape.p1.Y)));
                        }
                        else if (resizeShape.p2.X < resizeShape.p1.X && resizeShape.p2.Y < resizeShape.p1.Y) // lên trái
                        {
                            if (e.Location.X + 4 < resizeShape.p1.X && e.Location.Y + 4 < resizeShape.p1.Y)
                                resizeShape.p2 = new Point(resizeShape.p1.X - Math.Min(Math.Abs(e.Location.X - resizeShape.p1.X), Math.Abs(e.Location.Y - resizeShape.p1.Y)), resizeShape.p1.Y
                             - Math.Min(Math.Abs(e.Location.X - resizeShape.p1.X), Math.Abs(e.Location.Y - resizeShape.p1.Y)));
                        }
                        else if (resizeShape.p2.X > resizeShape.p1.X && resizeShape.p2.Y < resizeShape.p1.Y)  // lên phải
                        {
                            if (e.Location.X > resizeShape.p1.X + 4 && e.Location.Y + 4 < resizeShape.p1.Y)
                                resizeShape.p2 = new Point(resizeShape.p1.X + Math.Min(Math.Abs(e.Location.X - resizeShape.p1.X), Math.Abs(e.Location.Y - resizeShape.p1.Y)), resizeShape.p1.Y
                            - Math.Min(Math.Abs(e.Location.X - resizeShape.p1.X), Math.Abs(e.Location.Y - resizeShape.p1.Y)));
                        }
                        else if (resizeShape.p2.X < resizeShape.p1.X && resizeShape.p2.Y > resizeShape.p1.Y)// xuống trái
                        {
                            if (e.Location.X + 4 < resizeShape.p1.X && e.Location.Y > resizeShape.p1.Y + 4)
                                resizeShape.p2 = new Point(resizeShape.p1.X - Math.Min(Math.Abs(e.Location.X - resizeShape.p1.X), Math.Abs(e.Location.Y - resizeShape.p1.Y)), resizeShape.p1.Y
                              + Math.Min(Math.Abs(e.Location.X - resizeShape.p1.X), Math.Abs(e.Location.Y - resizeShape.p1.Y)));
                        }
                    }

                 
                    if (!(resizeShape is Line) && !(resizeShape is Circle))
                    {
                        if (resizeShape.p2.X > resizeShape.p1.X + 4 && resizeShape.p2.Y > resizeShape.p1.Y + 4)
                        {
                            if (e.Location.X > resizeShape.p1.X + 4 && e.Location.Y > resizeShape.p1.Y + 4)
                                resizeShape.p2 = e.Location;
                        }
                        else if (resizeShape.p2.X < resizeShape.p1.X && resizeShape.p2.Y < resizeShape.p1.Y)
                        {
                            if (e.Location.X + 4 < resizeShape.p1.X && e.Location.Y + 4 < resizeShape.p1.Y)
                                resizeShape.p2 = e.Location;
                        }
                        else if (resizeShape.p2.X > resizeShape.p1.X && resizeShape.p2.Y < resizeShape.p1.Y)
                        {
                            if (e.Location.X > resizeShape.p1.X + 4 && e.Location.Y + 4 < resizeShape.p1.Y)
                                resizeShape.p2 = e.Location;
                        }
                        else if (resizeShape.p2.X < resizeShape.p1.X && resizeShape.p2.Y > resizeShape.p1.Y)
                        {
                            if (e.Location.X + 4 < resizeShape.p1.X && e.Location.Y > resizeShape.p1.Y + 4)
                                resizeShape.p2 = e.Location;
                        }
                    }
                     PanelPaint.Invalidate();
                }
                else if (currentShape == CurrentShape.NoDrawing)
                {
                    if (isMouseSelect)
                    {
                        selectedRegion.Width = e.Location.X - selectedRegion.X;
                        selectedRegion.Height = e.Location.Y - selectedRegion.Y;

                        PanelPaint.Invalidate();
                    }
                    else
                    {
                        if (lstObject.Exists(shape => shape.IsHit(e.Location)))
                        {
                            PanelPaint.Cursor = Cursors.Hand;
                        }
                        else if (lstObject.Exists(shape => shape.IsHitPoint(e.Location)))
                        {
                            PanelPaint.Cursor = Cursors.SizeAll;
                        }
                        else
                        {
                            PanelPaint.Cursor = Cursors.Default;
                        }
                    }

                }

                if (isDrawPolygon)
                {
                    Polygon polygon = lstObject[lstObject.Count - 1] as Polygon;
                    polygon.Points[polygon.Points.Count - 1] = e.Location;

                    PanelPaint.Invalidate();
                }
            }
            catch { }

        }
        private void PanelPaint_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                isMouseDown = false;
                if (isMovingShape)
                {
                    isMovingShape = false;
                    selectedShape = null;
                }
                else if (isResizeShape)
                {
                    isResizeShape = false;
                    resizeShape = null;
                }
                else if (isMouseSelect)
                {
                    isMouseSelect = false;  // select 
                    for (int i = 0; i < lstObject.Count; i++)
                    {
                        lstObject[i].IsSelected = false;

                        if (lstObject[i].p1.X >= selectedRegion.X && lstObject[i].p2.X <= selectedRegion.X + selectedRegion.Width
                            && lstObject[i].p1.Y >= selectedRegion.Y && lstObject[i].p2.Y <= selectedRegion.Y + selectedRegion.Height)
                        {
                            lstObject[i].IsSelected = true;
                        }
                    }
                    PanelPaint.Invalidate();
                }
                if (currentShape != CurrentShape.Polygon && currentShape != CurrentShape.NoDrawing)
                {
                    lstObject[lstObject.Count - 1].p2 = e.Location;
                    PanelPaint.Invalidate();
                }
                ShapeObject shape = lstObject[lstObject.Count - 1];

                if (shape is Circle circle)
                {
                    if (circle.p2.X < circle.p1.X && circle.p2.Y < circle.p1.Y) // lên trái
                        circle.p2 = new Point(circle.p1.X - Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)), circle.p1.Y
                         - Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)));
                    if (circle.p2.X > circle.p1.X && circle.p2.Y > circle.p1.Y) // xuống phải
                        circle.p2 = new Point(circle.p1.X + Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)), circle.p1.Y
                        + Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)));
                    if (circle.p2.X > circle.p1.X && circle.p2.Y < circle.p1.Y) // lên phải
                        circle.p2 = new Point(circle.p1.X + Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)), circle.p1.Y
                         - Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)));
                    if (circle.p2.X < circle.p1.X && circle.p2.Y > circle.p1.Y) // xuống trái
                        circle.p2 = new Point(circle.p1.X - Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)), circle.p1.Y
                        + Math.Min(Math.Abs(circle.p2.X - circle.p1.X), Math.Abs(circle.p2.Y - circle.p1.Y)));
                }
            }
            catch
            {
            }
        }
        private void PanelPaint_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (isDrawPolygon)
            {
                isDrawPolygon = false;
                Polygon polygon = lstObject[lstObject.Count - 1] as Polygon;
                polygon.Points.RemoveAt(polygon.Points.Count - 1);
                // polygon.Points.RemoveAt(polygon.Points.Count - 1);
                FindPolygonRegion(polygon);
            }
        }

        private void PanelPaint_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            try
            {
                lstObject.ForEach(shape =>
                {
                    if (shape.IsSelected)
                    {
                        shape.Draw(e.Graphics);
                        if (shape is Ellipse || shape is Group || shape is Arc)
                        {
                            // e.Graphics.DrawRectangle(framePen, new System.Drawing.Rectangle(Math.Min(shape.p1.X, shape.p2.X),Math.Min(shape.p2.Y,shape.p1.Y), shape.p2.X - shape.p1.X, shape.p2.Y - shape.p1.Y));
                            if (shape.p2.X < shape.p1.X && shape.p2.Y < shape.p1.Y)
                            {
                                e.Graphics.DrawRectangle(framePen, new System.Drawing.Rectangle(shape.p2, new Size(shape.p1.X - shape.p2.X, shape.p1.Y - shape.p2.Y)));
                            }
                            else if (shape.p1.X > shape.p2.X && shape.p1.Y < shape.p2.Y)
                            {
                                e.Graphics.DrawRectangle(framePen, new System.Drawing.Rectangle(new Point(shape.p2.X, shape.p1.Y), new Size(shape.p1.X - shape.p2.X, shape.p2.Y - shape.p1.Y)));
                            }
                            else if (shape.p1.X < shape.p2.X && shape.p1.Y > shape.p2.Y)
                            {
                                e.Graphics.DrawRectangle(framePen,new System.Drawing.Rectangle(new Point(shape.p1.X, shape.p2.Y), new Size(shape.p2.X - shape.p1.X, shape.p1.Y - shape.p2.Y)));
                            }
                            else
                            {
                                e.Graphics.DrawRectangle(framePen, new System.Drawing.Rectangle(shape.p1, new Size(shape.p2.X - shape.p1.X, shape.p2.Y - shape.p1.Y)));
                            }
                            shape.FillPoint(e.Graphics);
                        }
                        else if (shape is Polygon polygon)
                        {
                            e.Graphics.DrawRectangle(framePen, new System.Drawing.Rectangle(shape.p1.X, shape.p1.Y, shape.p2.X - shape.p1.X, shape.p2.Y - shape.p1.Y));
                            shape.FillPoint(e.Graphics);
                        }
                        else
                        {
                            shape.FillPoint(e.Graphics);
                        }
                    }
                    else
                    {
                        shape.Draw(e.Graphics);
                    }
                });

                if (isMouseSelect)
                {
                    e.Graphics.DrawRectangle(framePen, selectedRegion);
                }
            }
            catch { }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            isControlKeyPress = e.Control;
            PanelPaint.Focus();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            isControlKeyPress = e.Control;
            PanelPaint.Focus();
            if (e.KeyCode == Keys.Delete)
            {
                btnDelete.PerformClick();
            }
        }

        private void btnPolygon_Click(object sender, EventArgs e)
        {
            lstObject.ForEach(shape => shape.IsSelected = false);
            PanelPaint.Invalidate();

            if (btnPolygon.BackColor == Color.Silver)
            {
                UncheckAll();

                currentShape = CurrentShape.NoDrawing;
                PanelPaint.Cursor = Cursors.Default;
                btnSelect.BackColor = Color.Silver;
            }
            else
            {
                UncheckAll();

                PanelPaint.Cursor = Cursors.Cross;
                currentShape = CurrentShape.Polygon;
                btnPolygon.BackColor = Color.Silver;
            }
        }

        private void btnArc_Click(object sender, EventArgs e)
        {
            lstObject.ForEach(shape => shape.IsSelected = false);
            PanelPaint.Invalidate();

            if (btnArc.BackColor == Color.Silver)
            {
                UncheckAll();
                currentShape = CurrentShape.NoDrawing;
                PanelPaint.Cursor = Cursors.Default;
                btnSelect.BackColor = Color.Silver;
            }
            else
            {
                UncheckAll();
                PanelPaint.Cursor = Cursors.Cross;
                currentShape = CurrentShape.Arc;
                btnArc.BackColor = Color.Silver;
            }
        }

        private void btnCircle_Click(object sender, EventArgs e)
        {
            lstObject.ForEach(shape => shape.IsSelected = false);
            PanelPaint.Invalidate();

            if (btnCircle.BackColor == Color.Silver)
            {
                UncheckAll();

                currentShape = CurrentShape.NoDrawing;
                PanelPaint.Cursor = Cursors.Default;
                btnSelect.BackColor = Color.Silver;
            }
            else
            {
                UncheckAll();

                PanelPaint.Cursor = Cursors.Cross;
                currentShape = CurrentShape.Circle;
                btnCircle.BackColor = Color.Silver;
            }
        }

        private void cmbShapeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.PerformClick();
            if (cmbShapeMode.SelectedIndex == 0)
            {
                mode = ShapeMode.NoFill;
                btnArc.Enabled = true;
                btnLine.Enabled = true;
                cblineWidth.Enabled = true;
                cbDashMode.Enabled = true;
            }
            else if (cmbShapeMode.SelectedIndex == 1)
            {
                mode = ShapeMode.Fill;
                btnLine.Enabled = false;
                btnArc.Enabled = false;
                cbDashMode.Enabled = false;
            }
        }

        private void btnElipse_Click(object sender, EventArgs e)
        {
            lstObject.ForEach(shape => shape.IsSelected = false);
            PanelPaint.Invalidate();

            if (btnElipse.BackColor == Color.Silver)
            {
                UncheckAll();
                currentShape = CurrentShape.NoDrawing;
                PanelPaint.Cursor = Cursors.Default;
                btnSelect.BackColor = Color.Silver;
            }
            else
            {
                UncheckAll();
                PanelPaint.Cursor = Cursors.Cross;
                currentShape = CurrentShape.Ellipse;
                btnElipse.BackColor = Color.Silver;
            }
        }

        private void btnRect_Click(object sender, EventArgs e)
        {
            lstObject.ForEach(shape => shape.IsSelected = false);
            PanelPaint.Invalidate();
            if (btnRect.BackColor == Color.Silver)
            {
                UncheckAll();
                currentShape = CurrentShape.NoDrawing;
                PanelPaint.Cursor = Cursors.Default;
                btnSelect.BackColor = Color.Silver;
            }
            else
            {
                UncheckAll();
                PanelPaint.Cursor = Cursors.Cross;
                currentShape = CurrentShape.Rectangle;
                btnRect.BackColor = Color.Silver;
            }
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            lstObject.ForEach(shape => shape.IsSelected = false);
            PanelPaint.Invalidate();
            if (btnLine.BackColor == Color.Silver)
            {
                UncheckAll();
                currentShape = CurrentShape.NoDrawing;
                PanelPaint.Cursor = Cursors.Default;
                btnSelect.BackColor = Color.Silver;
            }
            else
            {
                UncheckAll();
                PanelPaint.Cursor = Cursors.Cross;
                currentShape = CurrentShape.Line;
                btnLine.BackColor = Color.Silver;
            }
        }
        private void cbDashMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
            {
                if (shape is Group group)
                {
                    foreach (ShapeObject s in group)
                    {
                        s.pen.DashStyle = (DashStyle)cbDashMode.SelectedIndex;
                    }
                }
                else
                    shape.pen.DashStyle = (DashStyle)cbDashMode.SelectedIndex;
            });
            PanelPaint.Invalidate();
        }

        private void cblineWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
            {
                if (shape is Group group)
                {
                    foreach (ShapeObject s in group)
                    {
                        s.pen.Width = cblineWidth.SelectedIndex;
                    }
                }
                else
                    shape.pen.Width = cblineWidth.SelectedIndex;
            });
            PanelPaint.Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
            {
              
                    if (shape is Group group)
                    {
                        foreach (ShapeObject s in group)
                        {
                            HatchStyle hs = (HatchStyle)Enum.Parse(typeof(HatchStyle),
                             comboBox1.SelectedItem.ToString(), true);
                            s.hatch = new HatchBrush(hs, forecolor.BackColor, bc.BackColor);

                        }
                    }
                    else
                    {
                        HatchStyle hs = (HatchStyle)Enum.Parse(typeof(HatchStyle),
                             comboBox1.SelectedItem.ToString(), true);
                        shape.hatch = new HatchBrush(hs, forecolor.BackColor, bc.BackColor);
                    }
                
            });
            PanelPaint.Invalidate();
        }

        private void forecolor_Click(object sender, EventArgs e)
        {
            try
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    forecolor.BackColor = colorDialog1.Color;
                    lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
                    {
                        if (shape is Group group)
                        {
                            foreach (ShapeObject s in group)
                            {
                                if (s is FillShape fs)
                                    if (fs.Fill)
                                        s.hatch = new HatchBrush(s.hatch.HatchStyle, forecolor.BackColor, s.hatch.BackgroundColor);
                            }
                        }
                        else if (shape is FillShape)
                            shape.hatch = new HatchBrush(shape.hatch.HatchStyle, forecolor.BackColor, shape.hatch.BackgroundColor);
                    });
                    PanelPaint.Invalidate();
                }
            }
            catch { }
        }

        private void bc_Click(object sender, EventArgs e)
        {
            try
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    bc.BackColor = colorDialog1.Color;
                    lstObject.FindAll(shape => shape.IsSelected).ForEach(shape =>
                    {
                        if (shape is Group group)
                        {
                            foreach (ShapeObject s in group)
                            {
                                if (s is FillShape fs)
                                    if (fs.Fill)
                                        s.hatch = new HatchBrush(s.hatch.HatchStyle, s.hatch.ForegroundColor, bc.BackColor);
                            }
                        }
                        else if (shape is FillShape)
                            shape.hatch = new HatchBrush(shape.hatch.HatchStyle, shape.hatch.ForegroundColor, bc.BackColor);
                    });
                    PanelPaint.Invalidate();
                }
            }
            catch { }
        }
    }
}
