using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pankow_ColorConverter
{
    public partial class Form1 : Form
    {


        Stack undoStack_valColor = new Stack();//Стек,отвечающий за значение измененного цвета
        Stack undoStack_valOfParameterATypeChange = new Stack();//Стек,отвечающий за вид измения цвета
        Stack undoStack_valOfParameter_1 = new Stack();//Стек,отвечающий за деление изображения

        ColorConvert cc = new ColorConvert();

        int valueOfParameter_1 = 0;

        public Form1()
        {
            InitializeComponent();
        }


        public void ColorChange(int val)//Функция изменения цвета
        {
            int width = cc.rgbImage.Width;
            int height = cc.rgbImage.Height;
            int tmpA = 0;
            int tmpB = 0;
            int tmpWidth = width;
            int tmpHeight = height;
            int[] halfArr = { 0, 0, 0, 0 };
            halfArr = HalfChange(valueOfParameter_1, width, height, tmpA, tmpB, tmpWidth, tmpHeight);
            tmpA = halfArr[0];
            tmpB = halfArr[1];
            tmpWidth = halfArr[2];
            tmpHeight = halfArr[3];
            cc.CovertRGBimagetoHSV();
            for (int i = tmpA; i < tmpWidth; ++i)
            {
                for (int j = tmpB; j < tmpHeight; ++j)
                {
                    int tmpH = cc.hsvImage[i, j].h + val;
                    if (tmpH < 0)
                    {
                        cc.hsvImage[i, j].h = (ushort)(tmpH + 360);
                    }
                    else if (tmpH >= 360)
                    {
                        cc.hsvImage[i, j].h = (ushort)(tmpH - 360);
                    }
                    else
                    {
                        cc.hsvImage[i, j].h = (ushort)tmpH;
                    }
                }
            }
            cc.CovertHSVimagetoRGB();
            pictureBox1.Image = cc.rgbImage;
        }

        public void BrightnessChange(int val)//Функция изменения яркости
        {
            int width = cc.rgbImage.Width;
            int height = cc.rgbImage.Height;
            int tmpA = 0;
            int tmpB = 0;
            int tmpWidth = width;
            int tmpHeight = height;
            cc.CovertRGBimagetoHSV();
            int[] halfArr = { 0, 0, 0, 0 };
            halfArr = HalfChange(valueOfParameter_1, width, height, tmpA, tmpB, tmpWidth, tmpHeight);
            tmpA = halfArr[0];
            tmpB = halfArr[1];
            tmpWidth = halfArr[2];
            tmpHeight = halfArr[3];
            for (int i = tmpA; i < tmpWidth; ++i)
            {
                for (int j = tmpB; j < tmpHeight; ++j)
                {
                    int tmpV = cc.hsvImage[i, j].v + val;
                    if (tmpV < 0)
                    {
                        cc.hsvImage[i, j].v = 0;
                    }
                    else if (tmpV >= 100)
                    {
                        cc.hsvImage[i, j].v = 100;
                    }
                    else
                    {
                        cc.hsvImage[i, j].v = (byte)tmpV;
                    }
                }
            }
            cc.CovertHSVimagetoRGB();
            pictureBox1.Image = cc.rgbImage;
        }

        public void ContrastValChange(int val)//Функция изменения контраста
        {
            int width = cc.rgbImage.Width;
            int height = cc.rgbImage.Height;
            int tmpA = 0;
            int tmpB = 0;
            int tmpWidth = width;
            int tmpHeight = height;
            int[] halfArr = { 0, 0, 0, 0 };
            halfArr = HalfChange(valueOfParameter_1, width, height, tmpA, tmpB, tmpWidth, tmpHeight);
            tmpA = halfArr[0];
            tmpB = halfArr[1];
            tmpWidth = halfArr[2];
            tmpHeight = halfArr[3];
            cc.CovertRGBimagetoHSV();
            for (int i = tmpA; i < tmpWidth; ++i)
            {
                for (int j = tmpB; j < tmpHeight; ++j)
                {
                    int tmpS = cc.hsvImage[i, j].v + val;
                    if (tmpS < 0)
                    {
                        cc.hsvImage[i, j].s = 0;
                    }
                    else if (tmpS >= 100)
                    {
                        cc.hsvImage[i, j].s = 100;
                    }
                    else
                    {
                        cc.hsvImage[i, j].s = (byte)tmpS;
                    }
                }
            }

            cc.CovertHSVimagetoRGB();
            pictureBox1.Image = cc.rgbImage;
            
            
        }

        private int[] HalfChange(int valHalf, int width, int height, int tmpA, int tmpB, int tmpWidth, int tmpHeight)
        {
            int[] arr = { 0, 0, 0, 0 };

            if (valHalf == 0)//деление изображния на 2 части
            {
                arr[0] = 0; arr[1] = 0; arr[2] = width; arr[3] = height;
                undoStack_valOfParameter_1.Push(0);
            }

            else if (valHalf == 1)
            {
                arr[0] = 0; arr[1] = 0; arr[2] = width / 2; arr[3] = height;
                undoStack_valOfParameter_1.Push(1);
            }

            else if (valHalf == 2)
            {
                arr[0] = width / 2; arr[1] = 0; arr[2] = width; arr[3] = height;
                undoStack_valOfParameter_1.Push(2);
            }
            return arr;

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string imagePath;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "image files|*.jpg;*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                imagePath = dialog.FileName;
                cc.rgbImage = new Bitmap(imagePath);
                this.pictureBox1.Image = cc.rgbImage;
            }
        }


        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "image files|*.jpg;*.png";
            saveDialog.FileName = "Image";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }


        private void button1_Click(object sender, EventArgs e)//Цвет
        {

            this.Cursor = Cursors.WaitCursor;
            ColorChange(trackBar1.Value);
            undoStack_valColor.Push(trackBar1.Value);
            undoStack_valOfParameterATypeChange.Push(0);
            this.Cursor = Cursors.Default;
            trackBar1.Value = 0;
            if (undoStack_valColor.Peek() != null)
            {
                this.возвратToolStripMenuItem.Visible = true;
            }
            else
            {
                this.возвратToolStripMenuItem.Visible = false;
            }
        }


        private void button2_Click_1(object sender, EventArgs e)//Яркость
        {
            this.Cursor = Cursors.WaitCursor;
            BrightnessChange(trackBar2.Value);
            undoStack_valColor.Push(trackBar2.Value);
            undoStack_valOfParameterATypeChange.Push(1);
            this.Cursor = Cursors.Default;
            trackBar2.Value = 0;
            if (undoStack_valColor.Peek() != null)
            {
                this.возвратToolStripMenuItem.Visible = true;
            }
        }


        private void button3_Click(object sender, EventArgs e)//Контрастность
        {
            this.Cursor = Cursors.WaitCursor;
            ContrastValChange(trackBar3.Value);
            undoStack_valColor.Push(trackBar3.Value);
            undoStack_valOfParameterATypeChange.Push(2);
            this.Cursor = Cursors.Default;
            trackBar3.Value = 0;
            if (undoStack_valColor.Peek() != null)
            {
                this.возвратToolStripMenuItem.Visible = true;
            }
        }


        private void всёИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            valueOfParameter_1 = 0;
        }


        private void леваяПоловинаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            valueOfParameter_1 = 1;
        }


        private void праваяПоловинаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            valueOfParameter_1 = 2;
        }


        private void button2_Click(object sender, EventArgs e)
        {

            /*if (tmpStack == true)
            {
                object stackPopElemColor = undoStack_valColor.Pop();
                int stackElemColor = Convert.ToInt32(stackPopElemColor);

                object stackPopElemvalOfParameter_1 = undoStack_valOfParameter_1.Pop();
                int stackElemvalueOfParameter_1 = Convert.ToInt32(stackPopElemvalOfParameter_1);

                object stackPopElemvalOfParameter_2 = undoStack_valOfParameter_2.Pop();
                int stackElemvalOfParameter_2 = Convert.ToInt32(stackPopElemvalOfParameter_2);
                trackBar2.Value = (-1) * stackElemColor;
            }*/
        }

        private void возвратToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            trackBar1.Value = 0;
            trackBar2.Value = 0; 
            trackBar3.Value = 0;
            try
            {
                if (undoStack_valColor.Peek() != null)
                {
                    this.возвратToolStripMenuItem.Visible = true;
                }

                object stackPopElemColor = undoStack_valColor.Pop();
                int stackElemColor = Convert.ToInt32(stackPopElemColor);

                object stackPopElemvalOfParameter_1 = undoStack_valOfParameter_1.Pop();
                int stackElemvalueOfParameter_1 = Convert.ToInt32(stackPopElemvalOfParameter_1);

                object stackPopElemOfParameterATypeChange = undoStack_valOfParameterATypeChange.Pop();
                int stackElemvalOfParameterATypeChange = Convert.ToInt32(stackPopElemOfParameterATypeChange);

                int tmpTypeChange = valueOfParameter_1;

                if (stackElemvalueOfParameter_1 == 0)
                {
                    valueOfParameter_1 = 0;
                    if (stackElemvalOfParameterATypeChange == 0)
                    {
                        ColorChange((-1) * stackElemColor);
                    }
                    if (stackElemvalOfParameterATypeChange == 1)
                    {
                        BrightnessChange((-1) * stackElemColor);
                    }
                    if (stackElemvalOfParameterATypeChange == 2)
                    {
                        ContrastValChange((-1) * stackElemColor);
                    }
                }

                if (stackElemvalueOfParameter_1 == 1)
                {
                    valueOfParameter_1 = 1;
                    if (stackElemvalOfParameterATypeChange == 0)
                    {
                        ColorChange((-1) * stackElemColor);
                    }
                    if (stackElemvalOfParameterATypeChange == 1)
                    {
                        BrightnessChange((-1) * stackElemColor);
                    }
                    if (stackElemvalOfParameterATypeChange == 2)
                    {
                       ContrastValChange((-1) * stackElemColor);
                    }
                }

                if (stackElemvalueOfParameter_1 == 2)
                {
                    valueOfParameter_1 = 2;
                    if (stackElemvalOfParameterATypeChange == 0)
                    {
                        ColorChange((-1) * stackElemColor);
                    }
                    if (stackElemvalOfParameterATypeChange == 1)
                    {
                        BrightnessChange((-1) * stackElemColor);
                    }
                    if (stackElemvalOfParameterATypeChange == 2)
                    {
                        ContrastValChange((-1) * stackElemColor);
                    }
                }

               /* object tmpEmpty = undoStack_valColor.Peek();
                int intTmpEmty = Convert.ToInt32(tmpEmpty);

                if (intTmpEmty == 0)
                {
                    this.возвратToolStripMenuItem.Visible = false;
                }*/

            }
            catch
            {
                this.возвратToolStripMenuItem.Visible = false;
            }
        }



    }
}
