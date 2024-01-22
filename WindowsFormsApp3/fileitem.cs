using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class fileitem : UserControl
    {
        rm remove;

        public string filename = "";
        public fileitem(rm remove)
        {
            InitializeComponent();
            this.remove = remove;
        }

        private void fileitem_Load(object sender, EventArgs e)
        {
            string fileName = Path.GetFileName(filename);
            label1.Text = fileName;
            string fileExtension = Path.GetExtension(filename).ToLower();
            if(fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".jpg" || fileExtension == ".jpeg")
                guna2ImageButton1.Image = Properties.Resources.icons8_image_50;
            else if (fileExtension == ".txt")
                guna2ImageButton1.Image = Properties.Resources.fileinter;
            else if (fileExtension == ".xlsx")
                guna2ImageButton1.Image = Properties.Resources.icons8_excel_48;
            else if (fileExtension == ".pdf")
                guna2ImageButton1.Image = Properties.Resources.icons8_pdf_60;

        }


        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {
            remove.removeitem(filename);
        }
    }
}
