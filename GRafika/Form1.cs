using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GRafika
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int rozmiar = 4;
        Bitmap bmp;

        List<Point> listap = new List<Point>();
        Brush colork = Brushes.Black;
        private static IPEndPoint consumerEndPoint;
        private static UdpClient udpClient = new UdpClient();
        private void Paint4d_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
               
                listap.Add(new Point(e.X, e.Y));

                if (listap.Count() >= 2)
                {
                    
                    Graphics gr = Graphics.FromImage(bmp);

                    gr.DrawLines(new Pen(colork, rozmiar), listap.ToArray());
                    Paint4d.Image = bmp;

                }
            }
            
            this.Text = $"{e.X} X{e.Y}";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            
                Paint4d.CreateGraphics().Clear(Color.White);
                bmp = new Bitmap(Paint4d.Width, Paint4d.Height);
                using (var g = Graphics.FromImage(bmp))
                    g.Clear(Color.White);
            
            
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dr = colorDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                colork = new SolidBrush(colorDialog1.Color);
            }
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            rozmiar = trackBar1.Value;
        }
        private void Paint4d_MouseUp(object sender, MouseEventArgs e)
        {
            listap.Clear();
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            bmp = new Bitmap(Paint4d.Width, Paint4d.Height);
            using (var g = Graphics.FromImage(bmp))
                g.Clear(Color.White);
            Graphics gr = Graphics.FromImage(bmp);
            gr.Clear(Color.White);
            timer1.Interval = 500;
            timer1.Start();


            var port = int.Parse(ConfigurationManager.AppSettings.Get("consumerPort"));
            var client = new UdpClient(port);


            while (true)
            {
                var data = await client.ReceiveAsync();
                using (var ms = new MemoryStream(data.Buffer))
                {
                    StreamReader reader = new StreamReader(ms);
                    char[] str = reader.ReadToEnd().ToCharArray();
                    if (str[0] == 'k')
                    {
                        str[0] = ' ';
                        listBox1.Items.Add("On:" + new string(str));

                    }
                    else
                    {
                        bmp = new Bitmap(ms);
                        Paint4d.Image = bmp;
                    }
                        
                }

            }

        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmpSave = (Bitmap)Paint4d.Image;
                SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    bmpSave.Save(sfd.FileName, ImageFormat.Bmp);
                }
                saveFileDialog1.Filter = "*.jpg|*.jpg|*.bmp|*.bmp";
                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    bmp.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics gr = Graphics.FromImage(bmp);
            gr.Clear(Color.White);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "*.jpg|*.jpg|*.bmp|*.bmp";
                DialogResult dr = openFileDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    Bitmap b1 = new Bitmap(openFileDialog1.FileName);
                    bmp = new Bitmap(b1, new Size(Paint4d.Width, Paint4d.Height));
                    
                    
                    Paint4d.Image = bmp;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {

            tabControl1.Top = this.ClientSize.Height - tabControl1.Height - 10;
            Paint4d.Left = 0;
            Paint4d.Top = 0;
            Paint4d.Width = this.ClientSize.Width;
            Paint4d.Height = this.ClientSize.Height - (this.ClientSize.Height - tabControl1.Top) - 10;
        }
        private void Paint4d_SizeChanged(object sender, EventArgs e)
        {
            Graphics gr = Graphics.FromImage(bmp);
            gr.Clear(Color.White);
        }
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            if (bmp.Width < Paint4d.Width || bmp.Height < Paint4d.Height)
            {
                Bitmap bmp2 = new Bitmap(bmp);
                bmp = new Bitmap(Paint4d.Width, Paint4d.Height);
                Graphics gr = Graphics.FromImage(bmp);
                gr.Clear(Color.White);
                for (int x = 0; x < Math.Min(bmp2.Width, bmp.Width); x++)
                {
                    for (int y = 0; y < Math.Min(bmp2.Height, bmp.Height); y++)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(bmp2.GetPixel(x, y).R, bmp.GetPixel(x, y).G, bmp2.GetPixel(x, y).B));
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    int sredni = (bmp.GetPixel(x, y).R + bmp.GetPixel(x, y).G + bmp.GetPixel(x, y).B) / 3;
                    bmp.SetPixel(x, y, Color.FromArgb(sredni, sredni, sredni));
                }
            }
            Paint4d.Image = bmp;
        }


        private void button7_Click(object sender, EventArgs e)
        {

            for (int x = 1; x < bmp.Width - 1; x++)
            {
                for (int y = 1; y < bmp.Height - 1; y++)
                {
                    int sredni1 = (bmp.GetPixel(x + 1, y).R + bmp.GetPixel(x - 1, y).R + bmp.GetPixel(x, y + 1).R + bmp.GetPixel(x, y - 1).R) / 4;
                    int sredni2 = (bmp.GetPixel(x + 1, y).G + bmp.GetPixel(x - 1, y).G + bmp.GetPixel(x, y + 1).G + bmp.GetPixel(x, y - 1).G) / 4;
                    int sredni3 = (bmp.GetPixel(x + 1, y).B + bmp.GetPixel(x - 1, y).B + bmp.GetPixel(x, y + 1).B + bmp.GetPixel(x, y - 1).B) / 4;
                    bmp.SetPixel(x, y, Color.FromArgb(sredni1, sredni2, sredni3));

                }
            }
            Paint4d.Image = bmp;
        }


        private void button9_Click(object sender, EventArgs e)
        {
           
            bmp = new Bitmap(Paint4d.Image);
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = Convert.ToInt32(bmp.Height * 0.66); y < bmp.Height; y++)
                {
                    bmp.SetPixel(x, y, bmp.GetPixel(x, Convert.ToInt32(bmp.Height * 0.66) - (y - Convert.ToInt32(bmp.Height * 0.66))));
                  
                }
            }
            
            Paint4d.Image = bmp;
            

        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            var consummerIP = ConfigurationManager.AppSettings.Get("consumerIP");
            var consummerPort = int.Parse(ConfigurationManager.AppSettings.Get("consumerPort"));
            consumerEndPoint = new IPEndPoint(IPAddress.Parse(consummerIP), consummerPort);

            var bmp1 = new Bitmap(bmp, Paint4d.Height, Paint4d.Width);
            try
            {
                using (var ms = new MemoryStream())
                {
                    bmp1.Save(ms, ImageFormat.Jpeg);
                    var bytes = ms.ToArray();
                    udpClient.Send(bytes, bytes.Length, consumerEndPoint);
                }

            }
            catch (Exception ex)
            { MessageBox.Show(ex.ToString()); }

        }

            private void button10_Click(object sender, EventArgs e)
        {

        }

        

        private void button10_Click_2(object sender, EventArgs e)
        {
            var consummerIP = ConfigurationManager.AppSettings.Get("consumerIP");
            var consummerPort = int.Parse(ConfigurationManager.AppSettings.Get("consumerPort"));
            consumerEndPoint = new IPEndPoint(IPAddress.Parse(consummerIP), consummerPort);

            var str ="k"+ textBox1.Text;
            try
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(str);
                using (var ms = new MemoryStream(byteArray))
                {


                    udpClient.Send(byteArray, byteArray.Length, consumerEndPoint);
                    listBox1.Items.Add("JA: "+textBox1.Text);
                    textBox1.Text = "";
                }

            }
            catch (Exception ex)
            { MessageBox.Show(ex.ToString()); }
        }
    }  
}
