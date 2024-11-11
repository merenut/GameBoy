using System.Drawing;
using System.Windows.Forms;

namespace SharpBoy.Graphics;

public class Window : Form
{
    public Window(string title)
    {
        Text = title;
        Size = new Size(100, 100);
        ShowDialog();
    }
}