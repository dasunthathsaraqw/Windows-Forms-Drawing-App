using System.Drawing;

namespace DrawingApp
{
    public class RectangleShape : Shape
    {
        public override void Draw(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(FillColor))
            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                g.FillRectangle(brush, Bounds);
                g.DrawRectangle(pen, Bounds);
            }
        }
    }
}