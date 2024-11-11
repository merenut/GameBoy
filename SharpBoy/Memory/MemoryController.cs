using System.Text;

namespace SharpBoy.Memory;

public class MemoryController
{
    public byte[] Memory;


    public MemoryController()
    {
        Memory = new byte[0xFFFF];
    }

    public string Title { get; set; }
    public int ColorMode { get; set; }
    public int GameBoyIndicator { get; set; }
    public byte CartridgeType { get; set; }
    public byte ROMSize { get; set; }
    public byte RAMSize { get; set; }

    public void LoadRom(string path)
    {
        var rom = File.ReadAllBytes(path);

        if (rom.Length < 0x0149)
            throw new Exception("ROM size is too small");

        Title = Encoding.ASCII.GetString(rom, 0x0134, 0x0142 - 0x0134);
        ColorMode = rom[0x0143];
        GameBoyIndicator = rom[0x0146];
        CartridgeType = rom[0x147];
        ROMSize = rom[0x148];
        RAMSize = rom[0x149];
    }
}