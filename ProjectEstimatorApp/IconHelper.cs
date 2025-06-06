using FontAwesome.Sharp;
using System.Drawing;

public static class IconHelper
{
    public static Bitmap ToBitmap(this IconChar icon, int size, Color color)
    {
        using (var font = new Font("Font Awesome 6 Free Solid", size)) 
        {
            return icon.ToBitmap(font, color);
        }
    }

    public static Bitmap ToBitmap(this IconChar icon, Font font, Color color)
    {
        var bitmap = new Bitmap(font.Height, font.Height);
        using (var graphics = Graphics.FromImage(bitmap))
        using (var brush = new SolidBrush(color))
        {
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.DrawString(char.ConvertFromUtf32((int)icon), font, brush, 0, 0); 
        }
        return bitmap;
    }
}