using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BD_visual_
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.imageList1 = new System.Windows.Forms.ImageList();
            this.imageList1.Images.Add("available", Image.FromFile("gray.png"));    // боломжтой
            this.imageList1.Images.Add("booked", Image.FromFile("red.png"));         // захиалагдсан
            this.imageList1.Images.Add("selected", Image.FromFile("green.png"));     // сонгосон
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is PictureBox)
                {
                    PictureBox suudal = (PictureBox)ctrl;
                    suudal.Image = imageList1.Images["available"]; // эхлээд саарал
                    suudal.SizeMode = PictureBoxSizeMode.StretchImage;
                    suudal.Click += suudal_Click;
                }
            }
        }

        private void suudal_Click(object sender, EventArgs e)
        {
            PictureBox clickedSeat = (PictureBox)sender;

            // ☝️ эхлээд шалгана
            if ((string)clickedSeat.Tag == "reserved")
                return;

            // ✔ боломжтой бол ногоон болгоно
            if (clickedSeat.Image == imageList1.Images["available"])
            {
                clickedSeat.Image = imageList1.Images["selected"];
            }
            else if (clickedSeat.Image == imageList1.Images["selected"])
            {
                clickedSeat.Image = imageList1.Images["available"];
            }
        }

        private void suudal3_Click(object sender, EventArgs e)
        {

        }

        private void suudal2_Click(object sender, EventArgs e)
        {

        }
    }
}
