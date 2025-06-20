using System.Drawing;

namespace DrawingApp
{
    public class CircleShape : Shape
    {
        public override void Draw(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(FillColor))
            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                g.FillEllipse(brush, Bounds);
                g.DrawEllipse(pen, Bounds);
            }
        }
    }
}