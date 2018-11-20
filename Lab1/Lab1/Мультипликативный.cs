using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Мультипликативный : Form
    {
        Image image;
        public double[,] FilterMatrix;

        public double Factor /*= 1.0*/;
        public double Bias = 0.0;

        public Мультипликативный()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ///////////////////////Открыть/////////////////////////////
        /// </summary>
        private void ОткрытьToolStripMenuItem_Click_1(object sender, EventArgs e)
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
                image = Image.FromFile(ofd.FileName);
            }
        }

        private void ВыходToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Мультипликативный_Load(object sender, EventArgs e)
        {
            image = Image.FromFile(@"F:\2.jpg");
            pictureBox1.Image = image;
        }

        /// <summary>
        /// ///////////////////////Битмап/////////////////////////////
        /// </summary>
        private Image ConvolutionFilter()
        {
            double k1 = double.Parse(textBox1.Text);
            double k2 = double.Parse(textBox2.Text);
            double k3 = double.Parse(textBox3.Text);
            double k4 = double.Parse(textBox4.Text);
            double k5 = double.Parse(textBox5.Text);
            double k6 = double.Parse(textBox6.Text);
            double k7 = double.Parse(textBox7.Text);
            double k8 = double.Parse(textBox8.Text);
            double k9 = double.Parse(textBox9.Text);

            double kn = k1 + k2 + k3 + k4 + k5 + k6 + k7 + k8 + k9;
            if (kn == 0)
            {
                Factor = 1.0;
            }
            else
            {
                Factor = 1.0 / (k1 + k2 + k3 + k4 + k5 + k6 + k7 + k8 + k9);
            }

            FilterMatrix = new double[,] {{k1, k2, k3},
                                           {k4, k5, k6},
                                           {k7, k8, k9}};

            Bitmap bmp = (Bitmap)image;
            // Задаём формат Пикселя.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Получаем данные картинки.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            //Блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Получаем адрес первой линии.
            IntPtr ptr = bmpData.Scan0;

            // Задаём массив из Byte 
            //source
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[numBytes];

            //result
            int numBytes1 = bmpData.Stride * bmp.Height;
            byte[] rgbValues1 = new byte[numBytes1];


            // Копируем значения в массив.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            bmp.UnlockBits(bmpData);

            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;

            //кол-во строк в матрице
            int filterWidth = FilterMatrix.GetLength(1);
            int filterHeight = FilterMatrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;
            
            //считывание всех пикселей
            for(int offsetY = filterOffset; offsetY < bmp.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < bmp.Width - filterOffset; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;

                    //старый пиксель
                    byteOffset = offsetY * bmpData.Stride + offsetX * 4;

                   
                    for(int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for(int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            //индекс соседнего пикселя
                            calcOffset = byteOffset + (filterX * 4) + (filterY * bmpData.Stride);

                            //поиск оттенков исходя из старого изображения и матрицы фильтра
                            blue += (double)(rgbValues[calcOffset]) * FilterMatrix[filterY + filterOffset, filterX + filterOffset];

                            green += (double)(rgbValues[calcOffset + 1]) * FilterMatrix[filterY + filterOffset, filterX + filterOffset];

                            red += (double)(rgbValues[calcOffset + 2]) * FilterMatrix[filterY + filterOffset, filterX + filterOffset];
                        }
                    }
                    // результирующий оттенок равен фактору(1/сумму всех чисел матрицы) * 
                    //(цвет начального изображения * матрицу фильтра)
                    blue = Factor * blue;
                    green = Factor * green;
                    red = Factor * red;

                    //проверка выходит ли значение оттенка за границы [0,...,255]
                    if (blue > 255)
                    {
                        blue = 255;
                    }

                    else if (blue < 0)
                    {
                        blue = 0;
                    }

                    if (green > 255)
                    {
                        green = 255;
                    }

                    else if (green < 0)
                    {
                        green = 0;
                    }

                    if (red > 255)
                    {
                        red = 255;
                    }
                    else if (red < 0)
                    {
                        red = 0;
                    }

                    rgbValues1[byteOffset] = (byte)(blue);
                    rgbValues1[byteOffset + 1] = (byte)(green);
                    rgbValues1[byteOffset + 2] = (byte)(red);
                    rgbValues1[byteOffset + 3] = 255;
                }
            }

            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            Rectangle rect1 = new Rectangle(0, 0, result.Width, result.Height);

            //Блокируем набор данных изображения в памяти
            BitmapData resultData = result.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Получаем адрес первой линии.
            IntPtr ptr1 = bmpData.Scan0;

            Marshal.Copy(rgbValues1, 0, ptr1, rgbValues1.Length);
            result.UnlockBits(resultData);

            return result;

        }

        //Запустить
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ConvolutionFilter();
        }

        //Очистить
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
        }

        //резкость
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "-1";
            textBox2.Text = "-1";
            textBox3.Text = "-1";
            textBox4.Text = "-1";
            textBox5.Text = "9";
            textBox6.Text = "-1";
            textBox7.Text = "-1";
            textBox8.Text = "-1";
            textBox9.Text = "-1";
        }
        //размытие
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "1";
            textBox2.Text = "2";
            textBox3.Text = "1";
            textBox4.Text = "2";
            textBox5.Text = "4";
            textBox6.Text = "2";
            textBox7.Text = "1";
            textBox8.Text = "2";
            textBox9.Text = "1";
        }

        //яркость
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "0.2";
            textBox2.Text = "0.2";
            textBox3.Text = "0.0";
            textBox4.Text = "0.2";
            textBox5.Text = "0.2";
            textBox6.Text = "0.2";
            textBox7.Text = "0.0";
            textBox8.Text = "0.2";
            textBox9.Text = "0.2";

        }

        //обнаружение кромок
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            textBox2.Text = "-1";
            textBox3.Text = "0";
            textBox4.Text = "-1";
            textBox5.Text = "4";
            textBox6.Text = "-1";
            textBox7.Text = "0";
            textBox8.Text = "-1";
            textBox9.Text = "0";
        }

        //тиснение
        private void radioButton7_CheckedChanged_1(object sender, EventArgs e)
        {
            textBox1.Text = "-2";
            textBox2.Text = "-1";
            textBox3.Text = "0";
            textBox4.Text = "-1";
            textBox5.Text = "1";
            textBox6.Text = "1";
            textBox7.Text = "0";
            textBox8.Text = "1";
            textBox9.Text = "2";
        }

    }
}
