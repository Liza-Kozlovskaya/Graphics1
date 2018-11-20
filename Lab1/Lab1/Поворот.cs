using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Поворот : Form
    {
        Image img;
        int width;
        int height;
        float degrees;

        public Поворот()
        {
            InitializeComponent();
        }


        private void Form3_Load(object sender, EventArgs e)
        {
            img = Image.FromFile(@"F:\3.jpg");
            width = img.Width;
            height = img.Height;
            pictureBox1.Image = img;
        }

        private void Form3_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private Image RotateImage(double degrees, int width, int height)
        {
            Bitmap bmp = (Bitmap)img;

            //градус поворота
            double angle = degrees * Math.PI / 180.0;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            double d = Math.Sqrt(Math.Pow(bmp.Width, 2) + Math.Pow(bmp.Height, 2));

            // середина картинки
            int xOffset = (int)(bmp.Width / 2.0);
            int yOffset = (int)(bmp.Height / 2.0);

            ////размеры результирующего изображения
            //int Width = (int)Math.Round(2 * ((Math.Sqrt(Math.Pow(bmp.Width, 2) + Math.Pow(bmp.Height, 2))) / 2) * Math.Sin(Math.Atan((double)(bmp.Height / bmp.Width) + 35)));
            //int Height = (int)Math.Round(2 * ((Math.Sqrt(Math.Pow(bmp.Width, 2) + Math.Pow(bmp.Height, 2))) / 2) * Math.Sin(Math.Atan((double)(bmp.Width / bmp.Height) + 60)));

            int Width = (int)d;
            int Height = (int)d;

            int dx = (Width - bmp.Width) / 2;
            int dy = (Height - bmp.Height) / 2;

            Bitmap result = new Bitmap(Width, Height);

            //задаём формат пикселя
            PixelFormat pxf = PixelFormat.Format24bppRgb;

            //получаем данные картинки
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Rectangle rect1 = new Rectangle(0, 0, result.Width, result.Height);

            //блокируем набор данных изображения в памяти для обеих картинок
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            BitmapData resultData = result.LockBits(rect1, ImageLockMode.ReadWrite, pxf);


            //получаем адрес первой линии в обеих картинках
            IntPtr ptr = bmpData.Scan0;
            IntPtr ptr1 = resultData.Scan0;


            //задаём массив из Byte и помещаем в него набор данных
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];

            //для пустой картинки
            int numBytes1 = resultData.Stride * result.Height;
            byte[] rgbValues1 = new byte[numBytes1];

            
            //копируем значения в массив
            Marshal.Copy(ptr, rgbValues, 0, rgbValues.Length);

            for (int y = 0; y < bmp.Height; ++y)
            {
                for (int x = 0; x < bmp.Width; ++x)
                {
                    var index = y * bmpData.Stride + x * 3;

                    var X = (int)Math.Round((x - xOffset) * cos + (y - yOffset) * sin + xOffset + dx);
                    var Y = (int)Math.Round(-(x - xOffset) * sin + (y - yOffset) * cos + yOffset + dy);

                    var newIndex = Y * resultData.Stride + X * 3;

                    if (Y >= 0 && Y < Height && X >= 0 && X < Width)
                    {
                        if (newIndex + 5 < rgbValues1.Length)
                        {
                            rgbValues1[newIndex + 3] = rgbValues1[newIndex] = rgbValues[index];
                            rgbValues1[newIndex + 4] = rgbValues1[newIndex + 1] = rgbValues[index + 1];
                            rgbValues1[newIndex + 5] = rgbValues1[newIndex + 2] = rgbValues[index + 2];
                        }
                        else
                        {
                            rgbValues1[newIndex] = rgbValues[index];
                            rgbValues1[newIndex + 1] = rgbValues[index + 1];
                            rgbValues1[newIndex + 2] = rgbValues[index + 2];
                        }
                    }
                }
            }

            Marshal.Copy(rgbValues1, 0, ptr1, rgbValues1.Length);
            bmp.UnlockBits(bmpData);
            result.UnlockBits(resultData);
            return result;
        }

        private void ОткрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP; *.JPG; *.PNG)|*.BMP; *.JPG; *.PNG|All files (*.*)|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    img = new Bitmap(ofd.FileName);
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                img = Image.FromFile(ofd.FileName);
            }
        }

        private void ВыходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            degrees = (float)numericUpDown1.Value;
            pictureBox1.Image = RotateImage(degrees, width, height);
            Invalidate();
        }

        private void ОсновнаяФормаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 fr1 = new Form1();
            fr1.Show();
            Hide();
        }

        private void МасштабированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Масштабирование fr2 = new Масштабирование();
            fr2.Show();
            Hide();
        }

        private void ФильтрыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 fr5 = new Form5();
            fr5.Show();
            Hide();
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            degrees = (float)trackBar1.Value;
            pictureBox1.Image = RotateImage(degrees, width, height);
            Invalidate();
        }
    }
}

