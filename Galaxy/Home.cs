using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Galaxy
{
    public partial class Home : Form
    {
        List<Star> galaxy;
        Graphics g;
        bool scrolling = false;
        bool isPause = false;

        int mouse_prev_x = 0, mouse_prev_y = 0, viewX = 0, viewY = 0;
        float scale = 1f;
        int focusX = 800, focusY = 450;
        Timer timer;
        public Home()
        {
            InitializeComponent();

            this.MouseWheel += Home_MouseWheel;

            galaxy = new List<Star>();
            Random r = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            int countOfStars = r.Next(500, 700);

            while (galaxy.Count <= countOfStars)
            {
                Star star = new Star();
                star.x = r.Next(0, 16000) - 8000;
                star.y = r.Next(0, 8000) - 4000;
                star.size = r.Next(20, 40);
                bool iter = false;
                foreach (Star st in galaxy)
                {
                    double distance1 = Math.Sqrt(Math.Pow(star.x - st.x, 2) + Math.Pow(star.y - st.y, 2));
                    double distance2 = Math.Sqrt(Math.Pow(st.x - star.x, 2) + Math.Pow(st.y - star.y, 2));
                    double radius;
                    if (star.size >= st.size) radius = star.size / 2;
                    else radius = st.size / 2;

                    if ((distance1 < radius * 5) || (distance2 < radius * 5))
                    {
                        iter = true;
                        break;
                    }
                }

                if (iter) continue;

                star.color = Color.FromArgb(100 + r.Next(0, 155), 100 + r.Next(0, 155), 100 + r.Next(0, 155));

                if (star.size >= 30)
                {
                    int countOfPlanets = r.Next(1, 5);
                    List<Planet> planets = new List<Planet>();

                    while (planets.Count <= countOfPlanets)
                    {
                        Planet planet = new Planet();
                        planet.size = r.Next(5, 10);
                        planet.color = Color.FromArgb(100 + r.Next(0, 155), 100 + r.Next(0, 155), 100 + r.Next(0, 155));

                        if (planet.size > 6)
                        {
                            int countOfSatellits = r.Next(1, 3);

                            List<Satellit> satellits = new List<Satellit>();

                            while (satellits.Count <= countOfSatellits)
                            {
                                Satellit satellit = new Satellit();
                                satellit.size = r.Next(2, 5);
                                satellit.color = Color.FromArgb(100 + r.Next(0, 155), 100 + r.Next(0, 155), 100 + r.Next(0, 155));
                                satellit.dangle = Math.Pow(r.NextDouble() - 1, 2);
                                satellit.angle = r.NextDouble();
                                satellits.Add(satellit);
                            }
                            planet.satellits = satellits;
                        }
                        planet.dangle = Math.Pow(r.NextDouble() - 1, 2);
                        planet.angle = r.NextDouble();
                        planets.Add(planet);
                    }
                    star.planets = planets;
                }
                galaxy.Add(star);
            }

            Draw();

            timer = new Timer();
            timer.Interval = 70;
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void Home_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                scale += 0.05f;
            }
            else
            {
                if (scale <= 0.1f) return;
                scale -= 0.05f;
            }
            focusX = e.X;
            focusY = e.Y;
            Draw();
        }

        private void Draw()
        {
            Bitmap bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            g = Graphics.FromImage(bitmap);
            g.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);
            this.BackgroundImage = bitmap;
            foreach (Star star in galaxy)
            {
                g.FillEllipse(new SolidBrush(star.color), (star.x - star.size / 2) / scale + viewX + focusX/scale, (star.y - star.size / 2) / scale + viewY + focusY / scale, star.size / scale, star.size / scale);

                if (star.planets != null)
                {
                    foreach (Planet planet in star.planets)
                    {
                        g.FillEllipse(new SolidBrush(planet.color), (planet.x - planet.size / 2) / scale + viewX + focusX/scale, (planet.y - planet.size / 2) / scale + viewY + focusY/ scale, planet.size / scale, planet.size / scale);

                        if (planet.satellits != null)
                        {
                            foreach (Satellit satellit in planet.satellits)
                            {
                                g.FillEllipse(new SolidBrush(satellit.color), (satellit.x - satellit.size / 2) / scale + viewX + focusX / scale, (satellit.y - satellit.size / 2) / scale + viewY + focusY / scale, satellit.size / scale, satellit.size / scale);
                            }
                        }
                    }
                }
            }
        }

        private void Process()
        {
            foreach (Star star in galaxy)
            {
                if (star.planets != null)
                {
                    foreach (Planet planet in star.planets)
                    {
                        planet.angle += planet.dangle;
                        planet.x = (int)((star.size / 2 * (3 + star.planets.IndexOf(planet))) * Math.Cos(planet.angle)) + star.x;
                        planet.y = (int)((star.size / 2 * (3 + star.planets.IndexOf(planet))) * Math.Sin(planet.angle)) + star.y;

                        if (planet.satellits != null)
                        {
                            foreach (Satellit satellit in planet.satellits)
                            {
                                satellit.angle += satellit.dangle;
                                satellit.x = (int)((planet.size / 2 * (3 + planet.satellits.IndexOf(satellit))) * Math.Cos(satellit.angle)) + planet.x;
                                satellit.y = (int)((planet.size / 2 * (3 + planet.satellits.IndexOf(satellit))) * Math.Sin(satellit.angle)) + planet.y;
                            }
                        }
                    }
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Process();
            Draw();
        }

        public void home_MouseDown(object sender, MouseEventArgs e)
        {
            scrolling = true;
            mouse_prev_x = e.X;
            mouse_prev_y = e.Y;
        }
        
        public void home_MouseMove(object sender, MouseEventArgs e)
        {
            if(scrolling)
            {
                viewX += (e.X - mouse_prev_x);
                viewY += (e.Y - mouse_prev_y);
	            Draw();
	            this.Refresh();
                mouse_prev_x = e.X;
                mouse_prev_y = e.Y;
            }
        }

        public void home_MouseUp(object sender, MouseEventArgs e)
        {
            scrolling = false;
        }

        private void Home_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        timer.Stop();
                        this.Close();
                    } break;

                case Keys.P:
                    {
                        isPause = !isPause;
                        if (isPause) timer.Stop();
                        else timer.Start();
                    } break;
            }
        }
    }
}
