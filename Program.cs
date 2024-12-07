using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace LabWork
{
    public class GraphForm : Form
    {
        private double tMin;
        private double tMax;
        private double step;

        public GraphForm(double tMin, double tMax, double step)
        {
            this.tMin = tMin;
            this.tMax = tMax;
            this.step = step;

            // Налаштування форми
            this.Text = "Графік функції";
            this.Size = new Size(800, 600);
            this.Resize += (s, e) => this.Invalidate(); // Перемальовуємо при зміні розміру

            AddControls(); // Додаємо елементи керування
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Параметри графіку
            double tMin = this.tMin;
            double tMax = this.tMax;
            double step = this.step;

            // Отримання графічного об'єкта
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Розміри вікна
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            // Межі для побудови графіку
            float margin = 50;
            RectangleF graphAreaBounds = new RectangleF(
                margin,
                margin,
                width - 2 * margin,
                height - 2 * margin
            );

            // Малюємо осі
            DrawAxes(g, graphAreaBounds);

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
                float x = (float)((tValues[i] - tMin) / (tMax - tMin) * graphAreaBounds.Width + graphAreaBounds.Left);
                float y = (float)((1 - (yValues[i] - yMin) / (yMax - yMin)) * graphAreaBounds.Height + graphAreaBounds.Top);
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
            g.DrawString("X (t)", font, brush, graphAreaBounds.Right - 30, graphAreaBounds.Bottom + 10);
            g.DrawString("Y", font, brush, graphAreaBounds.Left - 20, graphAreaBounds.Top - 10);
        }

        private void DrawAxes(Graphics g, RectangleF graphAreaBounds)
        {
            Pen axisPen = new Pen(Color.Black, 2);
            g.DrawLine(axisPen, graphAreaBounds.Left, graphAreaBounds.Bottom, graphAreaBounds.Right, graphAreaBounds.Bottom); // X-вісь
            g.DrawLine(axisPen, graphAreaBounds.Left, graphAreaBounds.Bottom, graphAreaBounds.Left, graphAreaBounds.Top); // Y-вісь
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
                try
                {
                    yValues[i] = (t - Math.Log(2 * t)) / (3 * t + 1);
                }
                catch (Exception ex)
                {
                    yValues[i] = double.NaN; // або інше значення за замовчуванням
                }
            }
            return yValues;
        }

        private void AddControls()
        {
            TextBox tMinBox = new TextBox() { Location = new Point(10, 10), Width = 100, Text = tMin.ToString() };
            TextBox tMaxBox = new TextBox() { Location = new Point(120, 10), Width = 100, Text = tMax.ToString() };
            TextBox stepBox = new TextBox() { Location = new Point(230, 10), Width = 100, Text = step.ToString() };
            Button applyButton = new Button() { Text = "Apply", Location = new Point(340, 10) };

            applyButton.Click += (s, e) =>
            {
                if (double.TryParse(tMinBox.Text, out double tMin) &&
                    double.TryParse(tMaxBox.Text, out double tMax) &&
                    double.TryParse(stepBox.Text, out double step))
                {
                    this.tMin = tMin;
                    this.tMax = tMax;
                    this.step = step;
                    this.Invalidate(); // Перемалювати графік
                }
            };

            this.Controls.Add(tMinBox);
            this.Controls.Add(tMaxBox);
            this.Controls.Add(stepBox);
            this.Controls.Add(applyButton);
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GraphForm(2.1, 8.5, 0.7));
        }
    }
}
