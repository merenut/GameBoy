namespace SharpBoy.Memory;

public class Rom(string romPath)
{
    private int _cursor;
    private byte[] _rom = new byte[0];

    public byte GetNextByte => _rom[_cursor++];

    public Rom LoadRom()
    {
        _cursor = 0;
        _rom = File.ReadAllBytes(romPath);
        return this;
    }

    public ref byte[] GetRom()
    {
        return ref _rom;
    }
}