using Game.Module;
using Game.Protocol;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public partial class WinForm : Form, IMessageHandle
    {
        private Bitmap bitmap;
        private Graphics g;

        private Client client;

        public WinForm()
        {
            InitializeComponent();

            bitmap = new Bitmap(1000, 1000);
            g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            g.Clear(Color.White);

            for (int i = 0; i < 10; i++)
            {
                g.DrawLine(Pens.Black, i * 100, 0, i * 100, 1000);
                g.DrawLine(Pens.Black, 0, i * 100, 1000, i * 100);
            }

            lock (client.Around)
            {
                foreach (var c in client.Around)
                {
                    Bundle.Point p = c.Value.Point;
                    g.FillRectangle(Brushes.DodgerBlue, p.X - 2, p.Y - 2, 4, 4);
                }
            }

            g.FillRectangle(Brushes.Red, client.Point.X - 2, client.Point.Y - 2, 4, 4);

            pictureBox.Image = bitmap;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            client = new Client(ProtocolType.Kcp, this);
            client.Connect(textBox.Text);
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            float x = e.X - 250;
            float y = e.Y - 250;
            float degree = (float)(Math.Atan(y / x) / Math.PI * 180);

            degree += 180;

            if (x > 0)
                degree += 180;
            degree %= 360;
            client.Move(degree);
        }

        public void OnConnected()
        {
            Invoke(new EventHandler(delegate
            {
                pictureBox.Enabled = true;
                timer.Enabled = true;
            }));
        }

        public void OnClosed()
        {
            Invoke(new EventHandler(delegate
            {
                pictureBox.Enabled = false;
                timer.Enabled = false;
                Console.WriteLine("closed");
            }));
        }

        private void WinForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            client?.Dispose();
        }
    }
}
