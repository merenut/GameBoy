using System.Runtime.Intrinsics;
using System.Text;

namespace SharpBoy.Memory;

public class MemoryController
{
    #region Ranges

    public readonly Range RomBankSpan = new Range(0x0000,0x3FFF);
    public readonly Range SRomBankSpan = new Range(0x4000,0x7FFF);
    public readonly Range VRamSpan = new Range(0x8000,0x9FFF);
    public readonly Range SRamBankSpan = new Range(0xA000,0xBFFF);
    public readonly Range RamBankSpan = new Range(0xC000,0xDFFF);
    public readonly Range RamEchoSpan = new Range(0xE000,0xFDFF);
    public readonly Range OAMSpan = new Range(0xFE00, 0xFE9F);
    public readonly Range IOSoan  = new Range(0xFF00, 0xFF4B);
    public readonly Range RamBank2Span = new Range(0xFF80, 0xFFFE);

    #endregion
    
    #region Banks
    public byte[] RomBank; //0x0000-0x3FFF
    public byte[] SwitchableRomBank; //0x4000-0x7FFF
    public byte[] VRam; //0x8000-0x9FFF
    public byte[] SwitchableRam; //0xA000 - 0xBFFF
    public byte[] RamBank; // 0xC000 - 0xDFFF, echoed at 0xE000
    public byte[] SpriteAttrMem; //0xFE00
    public byte[] IoPorts; //0xFF00
    public byte[] RamBank2;
    public byte Ier; //Interupt Enable Register - 0xFFFF (one byte only)
    
    #endregion

    #region ROM Info
    
    public byte[] Rom;
    
    public bool CartridgeRam { get; set; }
    public bool CartridgeBatt { get; set; }
    public bool CartridgeSRam { get; set; }
    public bool CartridgeTimer { get; set; }
    public bool CartRumble { get; set; }
    public string Title { get; set; }
    public int ColorMode { get; set; }
    public int GameBoyIndicator { get; set; }
    public CartridgeTypes CartridgeType { get; set; }
    public byte ROMSize { get; set; }
    public byte RAMSize { get; set; }
    
    #endregion

    public MemoryController()
    {
        RomBank = new byte[RomBankSpan.End.Value - RomBankSpan.Start.Value];
        SwitchableRomBank = new byte[SRamBankSpan.End.Value - SRomBankSpan.Start.Value];
        VRam = new byte[VRamSpan.End.Value - VRamSpan.Start.Value];
        SwitchableRam = new byte[SRamBankSpan.End.Value - SRamBankSpan.Start.Value];
        RamBank = new byte[RamBankSpan.End.Value - RamBankSpan.Start.Value];
        SpriteAttrMem = new byte[OAMSpan.End.Value - OAMSpan.Start.Value];
        IoPorts = new byte[IOSoan.End.Value - IOSoan.Start.Value];
        RamBank2 = new byte[RamBank2Span.End.Value - RamBank2Span.Start.Value];
        Ier = new byte();
    }

    public struct CartridgeInfo
    {
        public CartridgeTypes Type;
        public bool Ram;
        public bool Batt;
        public bool Sram;
        public bool Timer;
        public bool Rumble;

        public CartridgeInfo(CartridgeTypes type, bool ram = false, bool batt = false, bool sram = false,
            bool timer = false, bool rumble = false)
        {
            Type = type;
            Ram = ram;
            Batt = batt;
            Sram = sram;
            Timer = timer;
            Rumble = rumble;
        }
    }

    public enum CartridgeTypes
    {
        Undetected,
        RomOnly,
        MBC1,
        MBC2,
        MBC3,
        MBC5,
        MMM01
    }
    
    public void LoadRom(string path)
    {
        Rom = File.ReadAllBytes(path);

        if (Rom.Length < 0x0149)
            throw new Exception("ROM size is too small");

        Title = Encoding.ASCII.GetString(Rom, 0x0134, 0x0142 - 0x0134);
        ColorMode = Rom[0x0143];
        GameBoyIndicator = Rom[0x0146];
        CartridgeType = SetMBC(Rom[0x147]);
        ROMSize = Rom[0x148];
        RAMSize = Rom[0x149];
        
        //copy ROM Bank
        Array.Copy(Rom, 0x0000, RomBank, 0x0000, 0x3FFF-1);
        
    }

    public CartridgeTypes SetMBC(byte cartridgeByte)
    {
        var cartridgeMap = new Dictionary<byte, CartridgeInfo>
        {
            { 0x00, new CartridgeInfo(CartridgeTypes.RomOnly) },
            { 0x01, new CartridgeInfo(CartridgeTypes.MBC1) },
            { 0x02, new CartridgeInfo(CartridgeTypes.MBC1, ram: true) },
            { 0x03, new CartridgeInfo(CartridgeTypes.MBC1, ram: true, batt: true) },
            { 0x05, new CartridgeInfo(CartridgeTypes.MBC2) },
            { 0x06, new CartridgeInfo(CartridgeTypes.MBC2, batt: true) },
            { 0x08, new CartridgeInfo(CartridgeTypes.RomOnly, ram: true) },
            { 0x09, new CartridgeInfo(CartridgeTypes.RomOnly, ram: true, batt: true) },
            { 0x0B, new CartridgeInfo(CartridgeTypes.MMM01) },
            { 0x0C, new CartridgeInfo(CartridgeTypes.MMM01, sram: true) },
            { 0x0D, new CartridgeInfo(CartridgeTypes.MMM01, sram: true, batt: true) },
            { 0x0F, new CartridgeInfo(CartridgeTypes.MBC3, timer: true, batt: true) },
            { 0x10, new CartridgeInfo(CartridgeTypes.MBC3, ram: true, timer: true, batt: true) },
            { 0x11, new CartridgeInfo(CartridgeTypes.MBC3) },
            { 0x12, new CartridgeInfo(CartridgeTypes.MBC3, ram: true) },
            { 0x13, new CartridgeInfo(CartridgeTypes.MBC3, ram: true, batt: true) },
            { 0x19, new CartridgeInfo(CartridgeTypes.MBC5) },
            { 0x1A, new CartridgeInfo(CartridgeTypes.MBC5, ram: true) },
            { 0x1B, new CartridgeInfo(CartridgeTypes.MBC5, ram: true, batt: true) },
            { 0x1C, new CartridgeInfo(CartridgeTypes.MBC5, rumble: true) },
            { 0x1D, new CartridgeInfo(CartridgeTypes.MBC5, sram: true, rumble: true) },
            { 0x1E, new CartridgeInfo(CartridgeTypes.MBC5, sram: true, batt: true, rumble: true) }
        };

        if (cartridgeMap.TryGetValue(cartridgeByte, out CartridgeInfo cartridge))
        {
            CartridgeRam = cartridge.Ram;
            CartridgeBatt = cartridge.Batt;
            CartridgeSRam = cartridge.Sram;
            CartridgeTimer = cartridge.Timer;
            CartRumble = cartridge.Rumble;
            return cartridge.Type;
        }

        return CartridgeTypes.Undetected;
    }

    public ref byte GetByteReference(int address)
    {
        if (address > 0xFFFF)
            throw new Exception("Address out of bounds");

        if (address >= SRomBankSpan.Start.Value && address <= SRomBankSpan.End.Value)
        {
            return ref SwitchableRomBank[address - SRomBankSpan.Start.Value];
        }
        if (address >= VRamSpan.Start.Value && address <= VRamSpan.End.Value)
        {
            return ref VRam[address - VRamSpan.Start.Value];
        }
        if (address >= SRamBankSpan.Start.Value && address <= SRamBankSpan.End.Value)
        {
            return ref SwitchableRam[address - SRamBankSpan.Start.Value];
        }
        if (address >= RamBankSpan.Start.Value && address <= RamBankSpan.End.Value)
        {
            return ref RamBank[address - RamBankSpan.Start.Value];
        }
        if (address >= RamEchoSpan.Start.Value && address <= RamEchoSpan.End.Value)
        {
            return ref RamBank[address - RamEchoSpan.Start.Value];
        }
        if (address >= OAMSpan.Start.Value && address <= OAMSpan.End.Value)
        {
            return ref SpriteAttrMem[address - OAMSpan.Start.Value];
        }
        if (address >= IOSoan.Start.Value && address <= IOSoan.End.Value)
        {
            return ref IoPorts[address - IOSoan.Start.Value];
        }

        if (address == 0xFFFF)
            return ref Ier;
        
        return ref RomBank[address];
    }



    public void SwapRam()
    {
        if (CartridgeType == CartridgeTypes.MBC1)
        {
            
        }

    }

    public void SwapRom()
    {
        
    }

}