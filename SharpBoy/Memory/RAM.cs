using System.Drawing;

namespace SharpBoy.Memory;

public class RAM
{
    private Pixel[] _screenBuffer = new Pixel[256 * 256];
    private Rectangle bounds = new(0, 0, 160, 144);

    public void ScrollX(int x)
    {
    }

    public void ScrollY(int y)
    {
    }

    public struct Pixel
    {
        public byte R, G, B, A;
    }
}