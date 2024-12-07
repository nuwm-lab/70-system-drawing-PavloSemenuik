using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace LabWork
{
    public class GraphForm : Form
    {
        public GraphForm()
        {
            // Налаштування форми
            this.Text = "Графік функції";
            this.Size = new Size(800, 600);
            this.Resize += (s, e) => this.Invalidate(); // Перемальовуємо при зміні розміру
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Параметри графіку
            double tMin = 2.1;
            double tMax = 8.5;
            double step = 0.7;

            // Отримання графічного об'єкта
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Розміри вікна
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            // Межі для побудови графіку
            float margin = 50;
            RectangleF graphBounds = new RectangleF(
                margin,
                margin,
                width - 2 * margin,
                height - 2 * margin
            );

            // Малюємо осі
            Pen axisPen = new Pen(Color.Black, 2);
            g.DrawLine(axisPen, graphBounds.Left, graphBounds.Bottom, graphBounds.Right, graphBounds.Bottom); // X-вісь
            g.DrawLine(axisPen, graphBounds.Left, graphBounds.Bottom, graphBounds.Left, graphBounds.Top); // Y-вісь

            // Вираховуємо значення функції
            double[] tValues = GetTValues(tMin, tMax, step);
            double[] yValues = GetYValues(tValues);

            // Знаходимо мінімум та максимум для масштабування
            double yMin = double.MaxValue, yMax = double.MinValue;
            foreach (double y in yValues)
            {
                if (y < yMin) yMin = y;
                if (y > yMax) yMax = y;
            }

            // Масштабування графіку
            PointF[] points = new PointF[tValues.Length];
            for (int i = 0; i < tValues.Length; i++)
            {
                float x = (float)((tValues[i] - tMin) / (tMax - tMin) * graphBounds.Width + graphBounds.Left);
                float y = (float)((1 - (yValues[i] - yMin) / (yMax - yMin)) * graphBounds.Height + graphBounds.Top);
                points[i] = new PointF(x, y);
            }

            // Малюємо графік
            Pen graphPen = new Pen(Color.Blue, 2);
            if (points.Length > 1)
            {
                g.DrawLines(graphPen, points);
            }

            // Підписи осей
            Font font = new Font("Arial", 10);
            Brush brush = Brushes.Black;
            g.DrawString("X (t)", font, brush, graphBounds.Right - 30, graphBounds.Bottom + 10);
            g.DrawString("Y", font, brush, graphBounds.Left - 20, graphBounds.Top - 10);
        }

        private double[] GetTValues(double tMin, double tMax, double step)
        {
            int count = (int)Math.Ceiling((tMax - tMin) / step) + 1;
            double[] tValues = new double[count];
            for (int i = 0; i < count; i++)
            {
                tValues[i] = tMin + i * step;
            }
            return tValues;
        }

        private double[] GetYValues(double[] tValues)
        {
            double[] yValues = new double[tValues.Length];
            for (int i = 0; i < tValues.Length; i++)
            {
                double t = tValues[i];
                yValues[i] = (t - Math.Log(2 * t)) / (3 * t + 1);
            }
            return yValues;
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GraphForm());
        }
    }
}
