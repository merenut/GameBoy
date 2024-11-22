using System.Drawing;
using System.Windows.Forms;
using SharpBoy.Memory;

namespace SharpBoy.Graphics;

public class Window : Form
{
    private MemoryController Mc { get; set; }
    private byte[] screenBuffer = new byte[256*256];
    private int X { get; set; }
    private int Y { get; set; }
    private int screenHeight = 160;
    private int screenWidth = 144;
    public Window(string title, MemoryController mc)
    {
        Mc = mc;
        base.Text = title;
        Size = new Size(screenWidth, screenHeight);
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
        e.Graphics.FillRectangle(new SolidBrush(Color.Black), X, Y, screenWidth, screenHeight);
    }
}