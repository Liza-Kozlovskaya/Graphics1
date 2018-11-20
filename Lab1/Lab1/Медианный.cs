using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Медианный : Form
    {
        Image image;

        public Медианный()
        {
            InitializeComponent();
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
                image = Image.FromFile(ofd.FileName);
            }
        }

        private Image MedianFilter(Bitmap sourceBitmap, int matrixSize)
        {
            // Задаём формат Пикселя.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Получаем данные картинки.
            Rectangle rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            //Блокируем набор данных изображения в памяти
            BitmapData sourceData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Получаем адрес первой линии.
            IntPtr ptr = sourceData.Scan0;

            // Задаём массив из Byte 
            int numBytes = sourceData.Stride * sourceBitmap.Height;
            int widthBytes = sourceData.Stride;
            byte[] rgbValues = new byte[numBytes];

            // Задаём массив из Byte 
            int numBytes1 = sourceData.Stride * sourceBitmap.Height;
            byte[] rgbValues1 = new byte[numBytes1];

            // Копируем значения в массив.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            sourceBitmap.UnlockBits(sourceData);

            //центр матрицы
            int filterOffset = (matrixSize - 1) / 2;
            int calcOffset = 0;
            int byteOffset = 0;

            //лист соседних пикселей
            List<int> neighbourPixel = new List<int>();
            byte[] rgbValues2;

            //перебираем пиксели
            for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    //индекс текущего пикселя
                    byteOffset = offsetY * sourceData.Stride + offsetX * 4;
                    neighbourPixel.Clear();

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            //индекс соседнего пикселя
                            calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                            //добавляем в список соседний пиксель
                            neighbourPixel.Add(BitConverter.ToInt32(rgbValues, calcOffset));
                        }
                    }
                    //сортируем полученные соседние пиксели
                    neighbourPixel.Sort();

                    rgbValues2 = BitConverter.GetBytes(neighbourPixel[filterOffset]);

                    //Определенного пикселя пикселю в результирующем изображении
                    rgbValues1[byteOffset] = rgbValues2[0];
                    rgbValues1[byteOffset + 1] = rgbValues2[1];
                    rgbValues1[byteOffset + 2] = rgbValues2[2];
                    rgbValues1[byteOffset + 3] = rgbValues2[3];
                }
            }
            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            Rectangle rect1 = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(rect1, ImageLockMode.ReadWrite, pxf);
            IntPtr ptr1 = resultData.Scan0;
            Marshal.Copy(rgbValues1, 0, ptr1, rgbValues1.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }


        private void ВыходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Без фильтра")
            {
                var sourceBitmap = new Bitmap(image);
                pictureBox2.Image = sourceBitmap;

            }
            else if (comboBox1.SelectedItem.ToString() == "3х3")
            {
                var sourceBitmap = new Bitmap(image);
                pictureBox2.Image = MedianFilter(sourceBitmap, 3);
            }
            else if (comboBox1.SelectedItem.ToString() == "5х5")
            {
                var sourceBitmap = new Bitmap(image);
                pictureBox2.Image = MedianFilter(sourceBitmap, 5); ;
            }
            else if (comboBox1.SelectedItem.ToString() == "7х7")
            {
                var sourceBitmap = new Bitmap(image);
                pictureBox2.Image = MedianFilter(sourceBitmap, 7);
            }
            else if (comboBox1.SelectedItem.ToString() == "9х9")
            {
                var sourceBitmap = new Bitmap(image);
                pictureBox2.Image = MedianFilter(sourceBitmap, 9);
            }
            else if (comboBox1.SelectedItem.ToString() == "11х11")
            {
                var sourceBitmap = new Bitmap(image);
                pictureBox2.Image = MedianFilter(sourceBitmap, 11);
            }
            else if (comboBox1.SelectedItem.ToString() == "13х13")
            {
                var sourceBitmap = new Bitmap(image);
                pictureBox2.Image = MedianFilter(sourceBitmap, 13);
            }
        }

        private void Медианный_Load(object sender, EventArgs e)
        {
            image = Image.FromFile(@"F:\5.jpg");
            pictureBox1.Image = image;
        }
    }
}
