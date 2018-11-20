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
    public partial class Масштабирование : Form
    {
        Image targer_image;
        int width;
        int height;

        public Масштабирование()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            targer_image = Image.FromFile(@"F:\2.jpg");
            width = targer_image.Width;
            height = targer_image.Height;

            pictureBox1.Image = targer_image;
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            width = (targer_image.Width * trackBar1.Value) / 100;
            pictureBox1.Image = ResizeNow(width, height);
            label2.Text = "Width:" + width.ToString() + "ph " + "Height" + height.ToString() + "ph";
        }
 
        private void TrackBar2_Scroll_1(object sender, EventArgs e)
        {
            height = (targer_image.Height * trackBar2.Value) / 100;
            pictureBox1.Image = ResizeNow(width, height);
            label2.Text = "Width:" + width.ToString() + "ph " + "Height" + height.ToString() + "ph";
        }

        private Image ResizeNow(int target_width, int target_height)
        {
            //оригинальная картинка
            Bitmap bmp = (Bitmap)targer_image;
            //создаём пустую картинку 
            Bitmap bmp1 = new Bitmap(width, height);
            //задаём формат пикселя
            PixelFormat pxf = PixelFormat.Format24bppRgb;

            //получаем данные картинки
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            //получаем данные пустой картинки
            Rectangle rect1 = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            //блокируем набор данных изображения в памяти для обеих картинок
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            BitmapData bmpData1 = bmp1.LockBits(rect1, ImageLockMode.ReadWrite, pxf);


            //получаем адрес первой линии в обеих картинках
            IntPtr ptr = bmpData.Scan0;
            IntPtr ptr1 = bmpData1.Scan0;

            //задаём массив из Byte и помещаем в него набор данных
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];

            //для пустой картинки
            int numBytes1 = bmpData1.Stride * bmp1.Height;
            int widthBytes1 = bmpData1.Stride;
            byte[] rgbValues1 = new byte[numBytes1];


            //копируем значения в массив
            Marshal.Copy(ptr, rgbValues, 0, numBytes);
            Marshal.Copy(ptr1, rgbValues1, 0, numBytes1);
            var scaleX = 1f * targer_image.Width / width;
            var scaleY = 1f * targer_image.Height / height;

            //перебираем исходное изображение и перестраиваем его в пустое
            for (int y = 0; y < bmp1.Height; y += 1)
            {
                for (int x = 0; x < bmp1.Width; x += 1)
                {
                    //позиция точки в исходном изображении 
                    var sourceY = (int)(y * scaleY);
                    var sourceX = (int)(x * scaleX);
                    int position = sourceY * widthBytes + sourceX * 3;

                    //позиция точки в записываемом изображении 
                    int position1 = y * widthBytes1 + x * 3;

                    //копируем 3 байта цвета точки
                    rgbValues1[position1] = rgbValues[position];
                    rgbValues1[position1 + 1] = rgbValues[position + 1];
                    rgbValues1[position1 + 2] = rgbValues[position + 2];
                }
            }

            //копируем набор данных обратно в изображения
            Marshal.Copy(rgbValues, 0, ptr, numBytes);
            Marshal.Copy(rgbValues1, 0, ptr1, numBytes1);

            //разблок. набор данных изображения в памяти
            bmp.UnlockBits(bmpData);
            bmp1.UnlockBits(bmpData1);

            return bmp1;

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
                    pictureBox1.Image = new Bitmap(ofd.FileName);
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                targer_image = Image.FromFile(ofd.FileName);

            }
        }

        private void ВыходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            width = (targer_image.Width * (int)(numericUpDown1.Value)) / targer_image.Width;
            pictureBox1.Image = ResizeNow(width, height);
            label2.Text = "Width:" + width.ToString() + "ph " + "Height" + height.ToString() + "ph";
            Invalidate();
        }

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            height = (targer_image.Height * (int)(numericUpDown2.Value)) / targer_image.Height;
            pictureBox1.Image = ResizeNow(width, height);
            label2.Text = "Width:" + width.ToString() + "ph " + "Height" + height.ToString() + "ph";
            Invalidate();
        }

        private void ОсновнаяФормаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 fr1 = new Form1();
            fr1.Show();
            Hide();
        }

        private void ПоворотToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Поворот fr3 = new Поворот();
            fr3.Show();
            Hide();
        }

        private void ФильтрыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 fr5 = new Form5();
            fr5.Show();
            Hide();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            width = (int)(targer_image.Width * (numericUpDown3.Value));
            pictureBox1.Image = ResizeNow(width, height);
            label2.Text = "Width:" + width.ToString() + "ph " + "Height" + height.ToString() + "ph";
            Invalidate();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            height = (int)(targer_image.Height * (numericUpDown4.Value));
            pictureBox1.Image = ResizeNow(width, height);
            label2.Text = "Width:" + width.ToString() + "ph " + "Height" + height.ToString() + "ph";
            Invalidate();
        }
    }
}
