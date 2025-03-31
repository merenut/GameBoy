using System.Drawing;
using System.Windows.Forms;
using SharpBoy.Memory;

namespace SharpBoy.Graphics;

public class Window : Form
{
    private MemoryController Mc { get; set; }
    private byte[] screenBuffer = new byte[256*256];
    private int mult = 3;
    private int X { get; set; }
    private int Y { get; set; }
    private int screenHeight = 144;
    private int screenWidth = 160;
    public Window(string title, MemoryController mc)
    {
        Mc = mc;
        base.Text = title;
        Size = new Size(screenWidth * mult, screenHeight * mult);
        Show();
    }

    private void DecodeVRam()
    {
        
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Render(e);
    }

    public void Render(PaintEventArgs e)
    {
        Pen pen = new Pen(new SolidBrush(Color.Aqua));
        e.Graphics.DrawRectangle(pen, X, Y, screenWidth, screenHeight);
        e.Graphics.FillRectangle(new SolidBrush(Color.AliceBlue), X, Y, screenWidth, screenHeight);
    }
}