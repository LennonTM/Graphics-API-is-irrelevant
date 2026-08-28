using System.Numerics;

namespace Black_Hole
{
    public partial class Form1 : Form
    {
        Boolean isBlackHole = false;
        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;

            timer1.Interval = 16;
            timer1.Start();
        }

        private float _time = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            var old = pictureBox1.Image;
            pictureBox1.Image = Program.render_image(_time);
            _time += 1.0f / 60.0f;
            old?.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.ToggleSlide();
        }
    }
}
