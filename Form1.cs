using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KGLaba4
{
    public partial class Form1 : Form
    {
        private Bitmap bitmap;
        private Size origSize;
        private int zoom = 1;
        private Point point1;
        private Point point2;

        private Color lineColor = Color.Black;

        public Form1()
        {
            InitializeComponent();

            origSize = new Size(panel1.Width, panel1.Height);
            bitmap = new Bitmap(origSize.Width, origSize.Height);
            listBox1.SetSelected(0, true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // var data = bitmap.LockBits(new Rectangle(0, 0, origSize.Width, origSize.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            //
            // unsafe
            // {
            //     byte* bDate = (byte*) data.Scan0;
            //     int x = 0;
            //     int x2 = bitmap.Height;
            //
            //     for (int y = 0; y < bitmap.Height; y++)
            //     {
            //         x = x < bitmap.Height ? x + 1 : x;
            //         x2 = x2 > 0 ? x2 - 1 : x2;
            //
            //         bDate[x * 4 + y * data.Stride] = 0;
            //         bDate[x * 4 + y * data.Stride + 1] = 0;
            //         bDate[x * 4 + y * data.Stride + 2] = 0;
            //         bDate[x * 4 + y * data.Stride + 3] = 255;
            //
            //         bDate[x2 * 4 + y * data.Stride] = 0;
            //         bDate[x2 * 4 + y * data.Stride + 1] = 0;
            //         bDate[x2 * 4 + y * data.Stride + 2] = 0;
            //         bDate[x2 * 4 + y * data.Stride + 3] = 255;
            //     }
            // }
            //
            // bitmap.UnlockBits(data);
            pictureBox1.Image = Zoom(bitmap, zoom);
            point1 = Point.Empty;
            point2 = Point.Empty;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private static Bitmap Zoom(Bitmap oldBitmap, int zoom, int sizeMuiltipler = 0)
        {
            sizeMuiltipler = sizeMuiltipler == 0 ? zoom : sizeMuiltipler;
            var newSize = new Size(oldBitmap.Width * sizeMuiltipler, oldBitmap.Height * sizeMuiltipler);
            var bitmap = new Bitmap(newSize.Width, newSize.Height);
            var data = bitmap.LockBits(new Rectangle(0, 0, newSize.Width, newSize.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var oldDate = oldBitmap.LockBits(new Rectangle(0, 0, oldBitmap.Width, oldBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* bDate = (byte*)data.Scan0;
                byte* oldBDate = (byte*)oldDate.Scan0;

                for (int y = 0; y < newSize.Height; y += zoom)
                {
                    for (int x = 0; x < newSize.Width; x += zoom)
                    {
                        int oldCord = (x / zoom) * 4 + (y / zoom) * oldDate.Stride;

                        for (int i = 0; i < zoom; i++)
                        {
                            if (y + i > newSize.Width) break;

                            for (int j = 0; j < zoom; j++)
                            {
                                if (x + j > newSize.Height) break;

                                int cord = (x + j) * 4 + (y + i) * data.Stride;
                                bDate[cord] = oldBDate[oldCord];
                                bDate[cord + 1] = oldBDate[oldCord + 1];
                                bDate[cord + 2] = oldBDate[oldCord + 2];
                                bDate[cord + 3] = oldBDate[oldCord + 3];
                            }
                        }
                    }
                }
            }

            oldBitmap.UnlockBits(oldDate);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem.ToString().Contains("Line "))
            {
                if (point1 == Point.Empty)
                {
                    point1.X = (int)Math.Round((float)(e.X / zoom));
                    point1.Y = (int)Math.Round((float)(e.Y / zoom));
                }
                else
                {
                    point2.X = (int)Math.Round((float)(e.X / zoom));
                    point2.Y = (int)Math.Round((float)(e.Y / zoom));
                    if (listBox1.SelectedItem.ToString().Equals("Line step by step"))
                    {
                        LineStepbyStep(lineColor);
                    }

                    if (listBox1.SelectedItem.ToString().Equals("Line DDA"))
                    {
                        LineDDA(lineColor);
                    }

                    if (listBox1.SelectedItem.ToString().Equals("Line Bresenham"))
                    {
                        LineBresenham(lineColor);
                    }

                    if (listBox1.SelectedItem.ToString().Equals("Line Castle-Pitway"))
                    {
                        LineCastlePitway(lineColor);
                    }

                    panel1.Invalidate();
                }
            }
            else
            {
                point1.X = (int)Math.Round((float)(e.X / zoom));
                point1.Y = (int)Math.Round((float)(e.Y / zoom));

                if (listBox1.SelectedItem.Equals("Circle(Bresenham)"))
                {
                    CircleBresengam(lineColor, (int)numericUpDown1.Value);
                }

                if (listBox1.SelectedItem.Equals("Circle(Midpoint)"))
                {
                    CircleMidpoint(lineColor, (int)numericUpDown1.Value);
                }

                panel1.Invalidate();
            }
        }

        private unsafe void LineStepbyStep(Color color)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, origSize.Width, origSize.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* bDate = (byte*)data.Scan0;
            if (point1.X == point2.X)
            {
                int step = point2.Y > point1.Y ? 1 : -1;
                for (int currentY = point1.Y; currentY != point2.Y; currentY += step)
                {
                    bDate[point2.X * 4 + currentY * data.Stride] = color.B;
                    bDate[point2.X * 4 + currentY * data.Stride + 1] = color.G;
                    bDate[point2.X * 4 + currentY * data.Stride + 2] = color.R;
                    bDate[point2.X * 4 + currentY * data.Stride + 3] = color.A;
                }

                bDate[point2.X * 4 + point2.Y * data.Stride] = color.B;
                bDate[point2.X * 4 + point2.Y * data.Stride + 1] = color.G;
                bDate[point2.X * 4 + point2.Y * data.Stride + 2] = color.R;
                bDate[point2.X * 4 + point2.Y * data.Stride + 3] = color.A; ;
                return;
            }

            int length = Math.Max(Math.Abs(point2.X - point1.X), Math.Abs(point2.Y - point1.Y));
            float k = (float)(point2.Y - point1.Y) / (point2.X - point1.X);
            float b = point1.Y;

            for (int index = 0; index <= length; ++index)
            {
                float currentX = point1.X + (point2.X - point1.X) * ((float)index / length);
                float currentY = k * (currentX - point1.X) + b;
                bDate[(int)Math.Round(currentX) * 4 + (int)Math.Round(currentY) * data.Stride] = color.B;
                bDate[(int)Math.Round(currentX) * 4 + (int)Math.Round(currentY) * data.Stride + 1] = color.G;
                bDate[(int)Math.Round(currentX) * 4 + (int)Math.Round(currentY) * data.Stride + 2] = color.R;
                bDate[(int)Math.Round(currentX) * 4 + (int)Math.Round(currentY) * data.Stride + 3] = color.A; ;

            }
            bitmap.UnlockBits(data);
        }

        private unsafe void LineDDA(Color color)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, origSize.Width, origSize.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* bDate = (byte*)data.Scan0;

            int dx = point2.X - point1.X;
            int dy = point2.Y - point1.Y;
            int length = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float xStep = (float)dx / length;
            float yStep = (float)dy / length;

            for (int index = 0; index <= length; ++index)
            {
                bDate[(int)Math.Round(point1.X + index * xStep) * 4 + (int)Math.Round(point1.Y + index * yStep) * data.Stride] = color.B;
                bDate[(int)Math.Round(point1.X + index * xStep) * 4 + (int)Math.Round(point1.Y + index * yStep) * data.Stride + 1] = color.G;
                bDate[(int)Math.Round(point1.X + index * xStep) * 4 + (int)Math.Round(point1.Y + index * yStep) * data.Stride + 2] = color.R;
                bDate[(int)Math.Round(point1.X + index * xStep) * 4 + (int)Math.Round(point1.Y + index * yStep) * data.Stride + 3] = color.A;
            }
            bitmap.UnlockBits(data);
        }

        private unsafe void LineBresenham(Color color)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, origSize.Width, origSize.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* bDate = (byte*)data.Scan0;

            int dx = Math.Abs(point2.X - point1.X);
            int sx = point1.X < point2.X ? 1 : -1;

            int dy = Math.Abs(point2.Y - point1.Y);
            int sy = point1.Y < point2.Y ? 1 : -1;

            int err = (dx > dy ? dx : -dy) / 2;
            int e2 = 0;

            int currentX = point1.X;
            int currentY = point1.Y;

            while (true)
            {
                bDate[currentX * 4 + currentY * data.Stride] = color.B;
                bDate[currentX * 4 + currentY * data.Stride + 1] = color.G;
                bDate[currentX * 4 + currentY * data.Stride + 2] = color.R;
                bDate[currentX * 4 + currentY * data.Stride + 3] = color.A;
                if (currentX == point2.X && currentY == point2.Y)
                {
                    break;
                }

                e2 = err;
                if (e2 > -dx)
                {
                    err -= dy; currentX += sx;
                }

                if (e2 < dy)
                {
                    err += dx; currentY += sy;
                }
            }
            bitmap.UnlockBits(data);
        }

        private unsafe void LineCastlePitway(Color color)
        {
            if (point1.X > point2.X)
            {
                Point v = point1;
                point1 = point2;
                point2 = v;
            }

            var data = bitmap.LockBits(new Rectangle(0, 0, origSize.Width, origSize.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* bDate = (byte*)data.Scan0;
            int slope;

            int dx = point2.X - point1.X;
            int dy = point2.Y - point1.Y;
            int d = dx - 2 * dy;
            int currentY = point1.Y;

            if (dy < 0)
            {
                slope = -1;
                dy = -dy;
            }
            else
            {
                slope = 1;
            }

            for (int currentX = point1.X; currentX <= point2.X; currentX++)
            {
                bDate[currentX * 4 + currentY * data.Stride] = color.B;
                bDate[currentX * 4 + currentY * data.Stride + 1] = color.G;
                bDate[currentX * 4 + currentY * data.Stride + 2] = color.R;
                bDate[currentX * 4 + currentY * data.Stride + 3] = color.A;
                if (d <= 0)
                {
                    d += 2 * dx - 2 * dy;
                    currentY += slope;
                }
                else
                {
                    d += -2 * dy;
                }
            }

            bitmap.UnlockBits(data);
        }
        private unsafe void CircleBresengam(Color color, int radius)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, origSize.Width, origSize.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* bDate = (byte*)data.Scan0;

            var width = origSize.Width;
            var height = origSize.Height;

            int x = 0, y = radius;
            int decesionParameter = 3 - 2 * radius;

            if (point1.X + x < origSize.Width && point1.Y + y < origSize.Height)
            {
                bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride] = color.B;
                bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride + 1] = color.G;
                bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride + 2] = color.R;
                bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride + 3] = color.A;
            }
            //////////////////////////////////////////////////////////////////////
            if (point1.X - x >0 && point1.Y + y < origSize.Height)
            {
                bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride] = color.B;
                bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride + 1] = color.G;
                bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride + 2] = color.R;
                bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride + 3] = color.A;
            }
            ///////////////////////////////////////////////////////////////////////
            if (point1.X + x < origSize.Width && point1.Y - y > 0)
            {
                bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride] = color.B;
                bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride + 1] = color.G;
                bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride + 2] = color.R;
                bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride + 3] = color.A;
            }
            ///////////////////////////////////////////////////////////////////////
            if (point1.X - x > 0 && point1.Y - y >0)
            {
                bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride] = color.B;
                bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride + 1] = color.G;
                bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride + 2] = color.R;
                bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride + 3] = color.A;
            }
            ///////////////////////////////////////////////////////////////////////
            if (point1.X + y < origSize.Width && point1.Y + x < origSize.Height)
            {
                bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride] = color.B;
                bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride + 1] = color.G;
                bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride + 2] = color.R;
                bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride + 3] = color.A;
            }
            //////////////////////////////////////////////////////////////////////
            if (point1.X - y > 0 && point1.Y + x < origSize.Height)
            {
                bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride] = color.B;
                bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride + 1] = color.G;
                bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride + 2] = color.R;
                bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride + 3] = color.A;
            }
            ///////////////////////////////////////////////////////////////////////
            if (point1.X + y < origSize.Width && point1.Y - x >0)
            {
                bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride] = color.B;
                bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride + 1] = color.G;
                bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride + 2] = color.R;
                bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride + 3] = color.A;
            }
            ///////////////////////////////////////////////////////////////////////
            if (point1.X - y >0 && point1.Y - x >0)
            {
                bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride] = color.B;
                bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride + 1] = color.G;
                bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride + 2] = color.R;
                bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride + 3] = color.A;
            }

            while (y >= x)
            {
                x++;
                if (decesionParameter > 0)
                {
                    y--;
                    decesionParameter = decesionParameter + 4 * (x - y) + 10;
                }
                else
                {
                    decesionParameter = decesionParameter + 4 * x + 6;
                }

                if (point1.X + x < origSize.Width && point1.Y + y < origSize.Height)
                {
                    bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride] = color.B;
                    bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride + 1] = color.G;
                    bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride + 2] = color.R;
                    bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride + 3] = color.A;
                }
                //////////////////////////////////////////////////////////////////////
                if (point1.X - x > 0 && point1.Y + y < origSize.Height)
                {
                    bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride] = color.B;
                    bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride + 1] = color.G;
                    bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride + 2] = color.R;
                    bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride + 3] = color.A;
                }
                ///////////////////////////////////////////////////////////////////////
                if (point1.X + x < origSize.Width && point1.Y - y > 0)
                {
                    bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride] = color.B;
                    bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride + 1] = color.G;
                    bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride + 2] = color.R;
                    bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride + 3] = color.A;
                }
                ///////////////////////////////////////////////////////////////////////
                if (point1.X - x > 0 && point1.Y - y > 0)
                {
                    bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride] = color.B;
                    bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride + 1] = color.G;
                    bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride + 2] = color.R;
                    bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride + 3] = color.A;
                }
                ///////////////////////////////////////////////////////////////////////
                if (point1.X + y < origSize.Width && point1.Y + x < origSize.Height)
                {
                    bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride] = color.B;
                    bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride + 1] = color.G;
                    bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride + 2] = color.R;
                    bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride + 3] = color.A;
                }
                //////////////////////////////////////////////////////////////////////
                if (point1.X - y > 0 && point1.Y + x < origSize.Height)
                {
                    bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride] = color.B;
                    bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride + 1] = color.G;
                    bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride + 2] = color.R;
                    bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride + 3] = color.A;
                }
                ///////////////////////////////////////////////////////////////////////
                if (point1.X + y < origSize.Width && point1.Y - x > 0)
                {
                    bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride] = color.B;
                    bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride + 1] = color.G;
                    bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride + 2] = color.R;
                    bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride + 3] = color.A;
                }
                ///////////////////////////////////////////////////////////////////////
                if (point1.X - y > 0 && point1.Y - x > 0)
                {
                    bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride] = color.B;
                    bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride + 1] = color.G;
                    bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride + 2] = color.R;
                    bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride + 3] = color.A;
                }
            }

            bitmap.UnlockBits(data);
        }

        private unsafe void CircleMidpoint(Color color, int radius)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, origSize.Width, origSize.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* bDate = (byte*)data.Scan0;
            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            do
            {

                if (point1.X + x < origSize.Width && point1.Y + y < origSize.Height)
                {
                    bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride] = color.B;
                    bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride + 1] = color.G;
                    bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride + 2] = color.R;
                    bDate[(point1.X + x) * 4 + (point1.Y + y) * data.Stride + 3] = color.A;
                }
                //////////////////////////////////////////////////////////////////////
                if (point1.X - x > 0 && point1.Y + y < origSize.Height)
                {
                    bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride] = color.B;
                    bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride + 1] = color.G;
                    bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride + 2] = color.R;
                    bDate[(point1.X - x) * 4 + (point1.Y + y) * data.Stride + 3] = color.A;
                }
                ///////////////////////////////////////////////////////////////////////
                if (point1.X + x < origSize.Width && point1.Y - y > 0)
                {
                    bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride] = color.B;
                    bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride + 1] = color.G;
                    bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride + 2] = color.R;
                    bDate[(point1.X + x) * 4 + (point1.Y - y) * data.Stride + 3] = color.A;
                }
                ///////////////////////////////////////////////////////////////////////
                if (point1.X - x > 0 && point1.Y - y > 0)
                {
                    bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride] = color.B;
                    bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride + 1] = color.G;
                    bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride + 2] = color.R;
                    bDate[(point1.X - x) * 4 + (point1.Y - y) * data.Stride + 3] = color.A;
                }
                ///////////////////////////////////////////////////////////////////////
                if (point1.X + y < origSize.Width && point1.Y + x < origSize.Height)
                {
                    bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride] = color.B;
                    bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride + 1] = color.G;
                    bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride + 2] = color.R;
                    bDate[(point1.X + y) * 4 + (point1.Y + x) * data.Stride + 3] = color.A;
                }
                //////////////////////////////////////////////////////////////////////
                if (point1.X - y > 0 && point1.Y + x < origSize.Height)
                {
                    bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride] = color.B;
                    bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride + 1] = color.G;
                    bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride + 2] = color.R;
                    bDate[(point1.X - y) * 4 + (point1.Y + x) * data.Stride + 3] = color.A;
                }
                ///////////////////////////////////////////////////////////////////////
                if (point1.X + y < origSize.Width && point1.Y - x > 0)
                {
                    bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride] = color.B;
                    bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride + 1] = color.G;
                    bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride + 2] = color.R;
                    bDate[(point1.X + y) * 4 + (point1.Y - x) * data.Stride + 3] = color.A;
                }
                ///////////////////////////////////////////////////////////////////////
                if (point1.X - y > 0 && point1.Y - x > 0)
                {
                    bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride] = color.B;
                    bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride + 1] = color.G;
                    bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride + 2] = color.R;
                    bDate[(point1.X - y) * 4 + (point1.Y - x) * data.Stride + 3] = color.A;
                }


                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }

                x++;
            } while (x <= y);
            bitmap.UnlockBits(data);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            zoom = trackBar1.Value;
            pictureBox1.Image = Zoom(bitmap, zoom);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // установка цвета формы
            lineColor = colorDialog1.Color;
        }
    }
}