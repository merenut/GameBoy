using System.Reflection.Metadata;
using SharpBoy.Memory;

namespace SharpBoy.CPU;

public class GameboyCPU(MemoryController mc)
{
    /// <summary>
    ///     CPU Registers
    /// </summary>

    public ref byte RegisterA => ref _registerA;

    public ref byte RegisterF => ref _registerF; //Used to store results of Math Ops
    public ref byte RegisterB => ref _registerB;
    public ref byte RegisterC => ref _registerC;
    public ref byte RegisterD => ref _registerD;
    public ref byte RegisterE => ref _registerE;
    public ref byte RegisterH => ref _registerH;
    public ref byte RegisterL => ref _registerL;
    
    public UInt16 AF => (UInt16)(RegisterA << 8 | RegisterF);
    public UInt16 BC => (UInt16)(RegisterB << 8 | RegisterC);
    public UInt16 DE => (UInt16)(RegisterD << 8 | RegisterE);
    public UInt16 HL => (UInt16)(RegisterH << 8 | RegisterL);
    //public UInt16 NN => (UInt16)(mc.GetByteReference(++RegisterPC) | mc.GetByteReference(++RegisterPC) << 8);

    public UInt16 RegisterPC { get; set; } //Points to the next instruction (starts at $100)
    public UInt16 RegisterSP { get; set; }

    public bool ZeroFlag { get; set; }
    public bool SubFlag { get; set; }
    public bool HalfCarryFlag { get; set; }
    public bool CarryFlag { get; set; }

    public bool Interupts { get; set; }

    public void Initialize()
    {
        RegisterPC = 0x100;
        RegisterSP = 0xFFFE;
    }

    public void PerformNext()
    {
        byte current = mc.GetByteReference(RegisterPC);

        Console.WriteLine($"Current PC: {RegisterPC} Current OpCode: {current:x8}");
        Lookup(current);
        RegisterPC+=1;
        //Thread.Sleep(200);
    }

    public ushort getNN()
    {
        var lowbit = mc.GetByteReference(RegisterPC + 1);
        var highbit = mc.GetByteReference(RegisterPC + 2);
        RegisterPC += 2;
        return (ushort)(highbit << 8 | lowbit);
    }

    public byte getN()
    {
        var n = mc.GetByteReference(RegisterPC + 1);
        RegisterPC++;
        return n;
    }

    /// <summary>
    ///     Op Lookup (Had to return empty Action due to .Net 8 switch limitations :))
    /// </summary>
    /// <param name="current"></param>
    public void Lookup(byte current)
    {
        (current switch
        {
            0x06 => LD8(mc.GetByteReference(++RegisterPC), ref RegisterB, 8),
            0x0E => LD8(mc.GetByteReference(++RegisterPC), ref RegisterC, 8),
            0x16 => LD8(mc.GetByteReference(++RegisterPC), ref RegisterD, 8),
            0x1E => LD8(mc.GetByteReference(++RegisterPC), ref RegisterE, 8),
            0x26 => LD8(mc.GetByteReference(++RegisterPC), ref RegisterH, 8),
            0x2E => LD8(mc.GetByteReference(++RegisterPC), ref RegisterL, 8),
            0x7F => LD8(RegisterA, ref RegisterA, 4),
            0x78 => LD8(RegisterB, ref RegisterA, 4),
            0x79 => LD8(RegisterC, ref RegisterA, 4),
            0x7A => LD8(RegisterD, ref RegisterA, 4),
            0x7B => LD8(RegisterE, ref RegisterA, 4),
            0x7C => LD8(RegisterH, ref RegisterA, 4),
            0x7D => LD8(RegisterL, ref RegisterA, 4),
            0x7E => LD8(mc.GetByteReference(HL), ref RegisterA, 8), //LD item at address contained in HL
            0x40 => LD8(RegisterB, ref RegisterB, 4),
            0x41 => LD8(RegisterC, ref RegisterB, 4),
            0x42 => LD8(RegisterD, ref RegisterB, 4),
            0x43 => LD8(RegisterE, ref RegisterB, 4),
            0x44 => LD8(RegisterH, ref RegisterB, 4),
            0x45 => LD8(RegisterL, ref RegisterB, 4),
            0x46 => LD8(mc.GetByteReference(HL), ref RegisterB, 8), //LD item at address contained in HL
            0x48 => LD8(RegisterB, ref RegisterC, 4),
            0x49 => LD8(RegisterC, ref RegisterC, 4),
            0x4A => LD8(RegisterD, ref RegisterC, 4),
            0x4B => LD8(RegisterE, ref RegisterC, 4),
            0x4C => LD8(RegisterH, ref RegisterC, 4),
            0x4D => LD8(RegisterL, ref RegisterC, 4),
            0x4E => LD8(mc.GetByteReference(HL), ref RegisterC, 8), //LD item at address contained in HL
            0x50 => LD8(RegisterB, ref RegisterD, 4),
            0x51 => LD8(RegisterC, ref RegisterD, 4),
            0x52 => LD8(RegisterD, ref RegisterD, 4),
            0x53 => LD8(RegisterE, ref RegisterD, 4),
            0x54 => LD8(RegisterH, ref RegisterD, 4),
            0x55 => LD8(RegisterL, ref RegisterD, 4),
            0x56 => LD8(mc.GetByteReference(HL), ref RegisterD, 8), //LD item at address contained in HL
            0x58 => LD8(RegisterB, ref RegisterE, 4),
            0x59 => LD8(RegisterC, ref RegisterE, 4),
            0x5A => LD8(RegisterD, ref RegisterE, 4),
            0x5B => LD8(RegisterE, ref RegisterE, 4),
            0x5C => LD8(RegisterH, ref RegisterE, 4),
            0x5D => LD8(RegisterL, ref RegisterE, 4),
            0x5E => LD8(mc.GetByteReference(HL), ref RegisterE, 8), //LD item at address contained in HL
            0x60 => LD8(RegisterB, ref RegisterH, 4),
            0x61 => LD8(RegisterC, ref RegisterH, 4),
            0x62 => LD8(RegisterD, ref RegisterH, 4),
            0x63 => LD8(RegisterE, ref RegisterH, 4),
            0x64 => LD8(RegisterH, ref RegisterH, 4),
            0x65 => LD8(RegisterL, ref RegisterH, 4),
            0x66 => LD8(mc.GetByteReference(HL), ref RegisterH, 8), //LD item at address contained in HL
            0x68 => LD8(RegisterB, ref RegisterL, 4),
            0x69 => LD8(RegisterC, ref RegisterL, 4),
            0x6A => LD8(RegisterD, ref RegisterL, 4),
            0x6B => LD8(RegisterE, ref RegisterL, 4),
            0x6C => LD8(RegisterH, ref RegisterL, 4),
            0x6D => LD8(RegisterL, ref RegisterL, 4),
            0x6E => LD8(mc.GetByteReference(HL), ref RegisterL, 8), //LD item at address contained in HL
            0x70 => LD8(RegisterB, ref mc.GetByteReference(HL), 8),
            0x71 => LD8(RegisterC, ref mc.GetByteReference(HL), 8),
            0x72 => LD8(RegisterD, ref mc.GetByteReference(HL), 8),
            0x73 => LD8(RegisterE, ref mc.GetByteReference(HL), 8),
            0x74 => LD8(RegisterH, ref mc.GetByteReference(HL), 8),
            0x75 => LD8(RegisterL, ref mc.GetByteReference(HL), 8),
            0x36 => LD8(mc.GetByteReference(++RegisterPC), ref mc.GetByteReference(HL), 8),
            0x0A => LD8(mc.GetByteReference(BC), ref RegisterA, 8),
            0x1A => LD8(mc.GetByteReference(DE), ref RegisterA, 8),
            0xFA => LD8(mc.GetByteReference(getN()), ref RegisterA, 16),
            0x3E => LD8(mc.GetByteReference(++RegisterPC), ref RegisterA, 8),
            0x47 => LD8(RegisterA, ref RegisterB, 4),
            0x4F => LD8(RegisterA, ref RegisterC, 4),
            0x57 => LD8(RegisterA, ref RegisterD, 4),
            0x5F => LD8(RegisterA, ref RegisterE, 4),
            0x67 => LD8(RegisterA, ref RegisterH, 4),
            0x6F => LD8(RegisterA, ref RegisterL, 4),
            0x02 => LD8(RegisterA, ref mc.GetByteReference(BC), 8),
            0x12 => LD8(RegisterA, ref mc.GetByteReference(DE), 8),
            0x77 => LD8(RegisterA, ref mc.GetByteReference(HL), 8),
            0xEA => LD8(RegisterA, ref mc.GetByteReference(getNN()), 16),
            0xF2 => LD8(mc.GetByteReference(0xFF00 + RegisterC), ref RegisterA, 8),
            0xE2 => LD8(RegisterA, ref mc.GetByteReference(0xFF00 + RegisterC), 8),
            0x3A => LDAHLD(),
            0x32 => LDHLAD(),
            0x2A => LDAHLI(),
            0x22 => LDHLAI(),
            0xE0 => LD8(RegisterA, ref mc.GetByteReference(0xFF00 + mc.GetByteReference(++RegisterPC)), 12),
            0xF0 => LD8(mc.GetByteReference(0xFF00 + mc.GetByteReference(++RegisterPC)), ref RegisterA, 12),
            //16 bit loads
            0x01 => LD16(getNN(), ref RegisterB, ref RegisterC),
            0x11 => LD16(getNN(), ref RegisterD, ref RegisterE),
            0x21 => LD16(getNN(), ref RegisterH, ref RegisterL),
            0x31 => LD16SP(getNN()),
            0xF9 => LD16SP(HL),
            0xF8 => LDHL_SP_N(),
            0x08 => LD16_To_Memory(getNN(), RegisterSP),
            //Push & Pop
            0xF5 => Push(RegisterA, RegisterF),
            0xC5 => Push(RegisterB, RegisterC),
            0xD5 => Push(RegisterD, RegisterE),
            0xE5 => Push(RegisterH, RegisterH),
            0xF1 => Pop(ref RegisterA, ref RegisterF),
            0xC1 => Pop(ref RegisterB, ref RegisterC),
            0xD1 => Pop(ref RegisterD, ref RegisterE),
            0xE1 => Pop(ref RegisterH, ref RegisterL),
            //Arithmetic
            0x87 => Add8(RegisterA, ref RegisterA),
            0x80 => Add8(RegisterB, ref RegisterA),
            0x81 => Add8(RegisterC, ref RegisterA),
            0x82 => Add8(RegisterD, ref RegisterA),
            0x83 => Add8(RegisterE, ref RegisterA),
            0x84 => Add8(RegisterH, ref RegisterA),
            0x85 => Add8(RegisterL, ref RegisterA),
            0x86 => Add8(mc.GetByteReference(HL), ref RegisterA),
            0xC6 => Add8(mc.GetByteReference(++RegisterPC), ref RegisterA),
            0x8F => Add8((byte)(RegisterA + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x88 => Add8((byte)(RegisterB + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x89 => Add8((byte)(RegisterC + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x8A => Add8((byte)(RegisterD + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x8B => Add8((byte)(RegisterE + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x8C => Add8((byte)(RegisterH + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x8D => Add8((byte)(RegisterL + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x8E => Add8((byte)(mc.GetByteReference(HL) + (CarryFlag ? 1 : 0)), ref RegisterA),
            0xCE => Add8((byte)(mc.GetByteReference(++RegisterPC) + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x97 => Sub8(RegisterA, ref RegisterA),
            0x90 => Sub8(RegisterB, ref RegisterA),
            0x91 => Sub8(RegisterC, ref RegisterA),
            0x92 => Sub8(RegisterD, ref RegisterA),
            0x93 => Sub8(RegisterE, ref RegisterA),
            0x94 => Sub8(RegisterH, ref RegisterA),
            0x95 => Sub8(RegisterL, ref RegisterA),
            0x96 => Sub8(mc.GetByteReference(HL), ref RegisterA),
            0xD6 => Sub8(mc.GetByteReference(++RegisterPC), ref RegisterA),
            0x9F => Sub8((byte)(RegisterA + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x98 => Sub8((byte)(RegisterB + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x99 => Sub8((byte)(RegisterC + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x9A => Sub8((byte)(RegisterD + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x9B => Sub8((byte)(RegisterE + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x9C => Sub8((byte)(RegisterH + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x9D => Sub8((byte)(RegisterL + (CarryFlag ? 1 : 0)), ref RegisterA),
            0x9E => Sub8((byte)(mc.GetByteReference(HL) + (CarryFlag ? 1 : 0)), ref RegisterA),
            0xDE => Sub8((byte)(mc.GetByteReference(++RegisterPC) + (CarryFlag ? 1 : 0)), ref RegisterA),
            0xA7 => And8(RegisterA, ref RegisterA),
            0xA0 => And8(RegisterB, ref RegisterA),
            0xA1 => And8(RegisterC, ref RegisterA),
            0xA2 => And8(RegisterD, ref RegisterA),
            0xA3 => And8(RegisterE, ref RegisterA),
            0xA4 => And8(RegisterH, ref RegisterA),
            0xA5 => And8(RegisterL, ref RegisterA),
            0xA6 => And8(mc.GetByteReference(HL), ref RegisterA),
            0xE6 => And8(mc.GetByteReference(++RegisterPC), ref RegisterA),
            0xB7 => Or8(RegisterA, ref RegisterA),
            0xB0 => Or8(RegisterB, ref RegisterA),
            0xB1 => Or8(RegisterC, ref RegisterA),
            0xB2 => Or8(RegisterD, ref RegisterA),
            0xB3 => Or8(RegisterE, ref RegisterA),
            0xB4 => Or8(RegisterH, ref RegisterA),
            0xB5 => Or8(RegisterL, ref RegisterA),
            0xB6 => Or8(mc.GetByteReference(HL), ref RegisterA),
            0xF6 => Or8(mc.GetByteReference(++RegisterPC), ref RegisterA),
            0xAF => Xor8(RegisterA, ref RegisterA),
            0xA8 => Xor8(RegisterB, ref RegisterA),
            0xA9 => Xor8(RegisterC, ref RegisterA),
            0xAA => Xor8(RegisterD, ref RegisterA),
            0xAB => Xor8(RegisterE, ref RegisterA),
            0xAC => Xor8(RegisterH, ref RegisterA),
            0xAD => Xor8(RegisterL, ref RegisterA),
            0xAE => Xor8(mc.GetByteReference(HL), ref RegisterA),
            0xEE => Xor8(mc.GetByteReference(++RegisterPC), ref RegisterA),
            0xBF => CMP8(RegisterA, RegisterA),
            0xB8 => CMP8(RegisterA, RegisterB),
            0xB9 => CMP8(RegisterA, RegisterC),
            0xBA => CMP8(RegisterA, RegisterD),
            0xBB => CMP8(RegisterA, RegisterE),
            0xBC => CMP8(RegisterA, RegisterH),
            0xBD => CMP8(RegisterA, RegisterL),
            0xBE => CMP8(RegisterA, mc.GetByteReference(HL)),
            0xFE => CMP8(RegisterA, mc.GetByteReference(++RegisterPC)),
            0x3C => Inc8(ref RegisterA),
            0x04 => Inc8(ref RegisterB),
            0x0C => Inc8(ref RegisterC),
            0x14 => Inc8(ref RegisterD),
            0x1C => Inc8(ref RegisterE),
            0x24 => Inc8(ref RegisterH),
            0x2C => Inc8(ref RegisterL),
            0x34 => Inc8(ref mc.GetByteReference(HL)),
            0x3D => Dec8(ref RegisterA),
            0x05 => Dec8(ref RegisterB),
            0x0D => Dec8(ref RegisterC),
            0x15 => Dec8(ref RegisterD),
            0x1D => Dec8(ref RegisterE),
            0x25 => Dec8(ref RegisterH),
            0x2D => Dec8(ref RegisterL),
            0x35 => Dec8(ref mc.GetByteReference(HL)),
            0x09 => Add16(BC, ref RegisterH, ref RegisterL),
            0x19 => Add16(DE, ref RegisterH, ref RegisterL),
            0x29 => Add16(HL, ref RegisterH, ref RegisterL),
            0x39 => Add16(RegisterSP, ref RegisterH, ref RegisterL),
            0xE8 => AddSP(mc.GetByteReference(++RegisterPC)),
            0x03 => Inc16(ref RegisterB, ref RegisterC),
            0x13 => Inc16(ref RegisterD, ref RegisterE),
            0x23 => Inc16(ref RegisterH, ref RegisterL),
            0x33 => () =>
            {
                RegisterSP++;
            },
            0x0B => Dec16(ref RegisterB, ref RegisterC),
            0x1B => Dec16(ref RegisterD, ref RegisterE),
            0x2B => Dec16(ref RegisterH, ref RegisterL),
            0x3B => () =>
            {
                RegisterSP--;
            },
            //0xCB => ProcessCB(),
            0x27 => () => { }, //Find out how DAA works
            0x2F => CPL8(ref RegisterA),
            0x3F => CCF(),
            0x37 => SCF(),
            0x00 => () => { Console.WriteLine("No Op"); },
            //Todo add CB
            //Halt & Stop
            //Todo add stop
            0x76 => () => { }, //Cant do until IO Interupts are programmed
            //Interupts
            0xF3 => DI(),
            0xFB => EI(),
            //Rotates
            0x07 => RLCA(),
            0x17 => RLCA(),
            0x0F => RRCA(),
            0x1F => RRCA(),
            //Todo figure out bit opcodes
            0xC3 => JP(),
            0xC2 => JPCC(!ZeroFlag),
            0xCA => JPCC(ZeroFlag),
            0xD2 => JPCC(!CarryFlag),
            0xDA => JPCC(CarryFlag),
            0xE9 => JP_HL(),
            0x18 => JR(),
            0x20 => JRCC(!ZeroFlag),
            0x28 => JRCC(ZeroFlag),
            0x30 => JRCC(!CarryFlag),
            0x38 => JRCC(CarryFlag),
            0xCD => Call16(),
            0xC4 => CallCC16(!ZeroFlag),
            0xCC => CallCC16(ZeroFlag),
            0xD4 => CallCC16(!CarryFlag),
            0xDC => CallCC16(CarryFlag),
            0xC7 => Restart(0x00),
            0xCF => Restart(0x08),
            0xD7 => Restart(0x10),
            0xDF => Restart(0x18),
            0xE7 => Restart(0x20),
            0xEF => Restart(0x28),
            0xF7 => Restart(0x30),
            0xFF => Restart(0x38),
            0xC9 => Ret(),
            0xC0 => RetCC(!ZeroFlag),
            0xC8 => RetCC(ZeroFlag),
            0xD0 => RetCC(!CarryFlag),
            0xD8 => RetCC(CarryFlag),
            0xD9 => RetI(),

            _ => () => { Console.WriteLine($"Current OP Code: {current:x8} not recognized."); }
        }) ();
    }

    private Action ProcessCB()
    {
        //Console.WriteLine("Performing CB");
        return () => { };
    }

    #region Restarts

    public Action Restart(UInt16 n)
    {
        Console.WriteLine($"Performing Restart at {n:x8}");
        RegisterPC = n;
        return () => { };
    }

    #endregion

    private void Cycle(short inc)
    {
        //Cycle determines how long to wait in between executes
    }

    #region Fields

    private byte _registerA;
    private byte _registerB;
    private byte _registerC;
    private byte _registerD;
    private byte _registerE;
    private byte _registerF;
    private byte _registerH;
    private byte _registerL;

    #endregion

    #region LD

    /// <summary>
    ///     LD 8 bit - Load v1 into v2
    /// </summary>
    public Action LD8(byte v1, ref byte v2, short cycle)
    {
         Console.WriteLine($"Performing Load (8), Loading value: {v1:x8}");
        v2 = v1;
        return () => { Cycle(cycle); };
    }

    public Action LDD8(ref byte h, ref byte l, ref byte v2, short cycle)
    {
        var hl = h << 8 | l;
        v2 = mc.GetByteReference(hl);
        hl--;
        h = (byte)(hl >> 8);
        l = (byte)(hl & 0xFF);

        Console.WriteLine($"Performing Load (8), Loading value: {hl:x8} int {v2:x8}");
        return () => { Cycle(cycle); };
    }

    public Action LDHLAI()
    {
        Console.WriteLine($"Performing LDHLAI");
        var n1 = mc.GetByteReference(HL);
        n1 = RegisterA;
        Inc16(ref RegisterH, ref RegisterL);
        return () => { };
    }

    public Action LDAHLI()
    {
        Console.WriteLine($"Performing LDAHLI");
        var n1 = mc.GetByteReference(HL);
        RegisterA = n1;
        Inc16(ref RegisterH, ref RegisterL);
        return () => { };
    }
    public Action LDHLAD()
    {
        Console.WriteLine($"Performing LDHLAD");
        var n1 = mc.GetByteReference(HL);
        n1 = RegisterA;
        Dec16(ref RegisterH, ref RegisterL);
        return () => { };
    }

    public Action LDAHLD()
    {
        Console.WriteLine($"Performing LDAHLD");
        var n1 = mc.GetByteReference(HL);
        RegisterA = n1;
        Dec16(ref RegisterH, ref RegisterL);
        return () => { };
    }

    public Action LD16(byte v1, byte v2, ref byte n1, ref byte n2)
    {
        Console.WriteLine($"Performing Load (16), Loading value: {v1:x8} and {v2:x8}");
        n1 = v1;
        n2 = v2;
        return () => { };
    }

    public Action LD16(ushort v1, ref byte n1, ref byte n2)
    {
        Console.WriteLine($"Performing Load (16), Loading value: {v1:x16}");
        var high = v1 & 0x00FF;
        var low = v1 >> 8;
        n1 = (byte)low;
        n2 = (byte)high;
        return () => { };
    }

    public Action LD16SP(ushort v1)
    {
        Console.WriteLine($"Performing Load (16), Loading value: {v1:x8} into SP");
        RegisterSP = v1;
        return () => { };
    }

    public Action LDHL_SP_N()
    {
        Console.WriteLine($"Performing LDHL_SP_N");
        var n = getN();
        var result = RegisterSP + n;
        RegisterH = (byte)(result >> 8);
        RegisterL = (byte)(result & 0xFF);

        //Set Flags
        ZeroFlag = false;
        SubFlag = false;
        HalfCarryFlag = ((RegisterSP & 0xF) + (n & 0xF)) > 0xF;
        CarryFlag = ((RegisterSP & 0xFF) + (n & 0xFF)) > 0xFF;
        return () => { };
    }

    public Action LD16_To_Memory(ushort address, ushort value)
    {
        Console.WriteLine($"Performing Load (16), Loading value: {value:x8} to address {address:x8}");
        byte n1 = mc.GetByteReference(address);
        byte n2 = mc.GetByteReference(address + 1);

        n1 = (byte)(value & 0xFF);
        n2 = (byte)(value >> 8);

        return () => { };
    }

    #endregion

    #region Stack

    public Action Pop(ref byte v, ref byte v2)
    {
        v2 = (byte)(mc.GetByteReference(RegisterSP));
        RegisterSP++;
        v = mc.GetByteReference(RegisterSP);
        Console.WriteLine($"Performing Pop, Loading value from: {v:x8} and {v2:x8}");
        RegisterSP++;
        return () => { };
    }

    public Action Push(byte v, byte v2)
    {
        Console.WriteLine($"Performing Push, Loading value from: {v:x8} and {v2:x8} into address {RegisterSP:x8}");
        //Dec SP
        RegisterSP--;
        mc.GetByteReference(RegisterSP) = v;
        RegisterSP--;
        mc.GetByteReference(RegisterSP) = v2;
        return () => { };
    }
    
    #endregion

    #region Arithmetic

    /// <summary>
    ///     Add 8 bit - add nn to n (result stored in n)
    /// </summary>
    public Action Add8(byte nn, ref byte n)
    {
        Console.WriteLine($"Performing Add8");
        var original = n;
        int result = (int)n + (int)nn;
        n += nn;

        //Set Flags
        ZeroFlag = n == 0;
        SubFlag = false;
        HalfCarryFlag = (((original & 0xF) + (nn & 0xF)) & 0x10) != 0;
        CarryFlag = result > 0xFF;

        return () => { };
    }

    public Action Sub8(byte nn, ref byte n)
    {
        Console.WriteLine($"Performing Sub8");
        var original = n;
        var result = n - nn;
        n -= nn;

        ZeroFlag = n == 0;
        SubFlag = true;
        HalfCarryFlag = (original & 0xF) < (nn & 0xF);
        CarryFlag = result > 0;

        return () => { };
    }

    public Action Or8(byte nn, ref byte n)
    {
        Console.WriteLine($"Performing Or8");
        n |= nn;

        //Set Flags
        ZeroFlag = n == 0;
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = false;

        return () => { };
    }

    public Action And8(byte nn, ref byte n)
    {
        Console.WriteLine($"Performing And8");
        n &= nn;

        //Set Flags
        ZeroFlag = n == 0;
        SubFlag = false;
        HalfCarryFlag = true;
        CarryFlag = false;

        return () => { };
    }

    public Action Xor8(byte nn, ref byte n)
    {
        Console.WriteLine($"Performing Xor8");
        n ^= nn;

        //Set Flags
        ZeroFlag = n == 0;
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = false;

        return () => { };
    }

    public Action CMP8(byte nn, byte n)
    {
        Console.WriteLine($"Performing Cmp8");

        //Set Flags
        ZeroFlag = nn - n == 0;
        SubFlag = true;
        HalfCarryFlag = (nn & 0xF) < (n & 0xF);
        CarryFlag = nn < n;

        return () => { };
    }

    public Action Inc8(ref byte n)
    {
        Console.WriteLine($"Performing Inc8");
        var original = n;
        Add8(1, ref n);

        ZeroFlag = n == 0;
        SubFlag = false;
        HalfCarryFlag = (((original & 0xF) + (1 & 0xF)) & 0x10) != 0;
        CarryFlag = false;

        return () => { };
    }

    public Action Dec8(ref byte n)
    {
        Console.WriteLine($"Performing Dec8");
        var original = n;
        Sub8(1, ref n);

        ZeroFlag = n == 0;
        SubFlag = true;
        HalfCarryFlag = (original & 0xF) < (1 & 0xF);
        CarryFlag = false;


        return () => { };
    }

    public Action Add16(UInt16 nn, ref byte n1, ref byte n2)
    {
        Console.WriteLine($"Performing Add16");
        var ncom = (n1 << 8) | n2;

        var result = nn + ncom;

        n2 = (byte)(result & 0xFF);
        n1 = (byte)(result >> 8);
        return () => { };
    }

    public Action AddSP(byte n)
    {
        Console.WriteLine($"Performing AddSP");
        RegisterSP += n;
        return () => { };
    }

    public Action Inc16(ref byte n1, ref byte n2)
    {
        Console.WriteLine($"Performing Inc16");
        //No Flags Affected
        var ncom = (n1 << 8) | n2; //registers combined into int

        ncom++;

        n2 = (byte)(ncom & 0xFF);
        n1 = (byte)(ncom >> 8);
        return () => { };
    }

    public Action Dec16(ref byte n1, ref byte n2)
    {
        Console.WriteLine($"Performing Dec16");
        //No flags affected
        var ncom = (n1 << 8) | n2; //registers combined into int

        ncom--;

        n2 = (byte)(ncom & 0xFF);
        n1 = (byte)(ncom >> 8);
        return () => { };
    }

    #endregion

    #region Misc

    public Action Swap8(ref byte n)
    {
        Console.WriteLine($"Performing Swap 8");
        var upper = n >> 4;
        var lower = n & 0x0F;

        n = (byte)((lower << 4) | upper);

        ZeroFlag = n == 0;
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = false;

        return () => { };
    }

    public Action CPL8(ref byte n)
    {
        Console.WriteLine($"Performing Cpl8");
        n = (byte)~n;

        //Set Flags
        SubFlag = true;
        HalfCarryFlag = true;

        return () => { };
    }

    public Action CCF()
    {
        Console.WriteLine($"Performing CCF");
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = !CarryFlag;
        return () => { };
    }

    public Action SCF()
    {
        Console.WriteLine($"Performing SCF");
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = true;
        return () => { };
    }

    public Action DI()
    {
        Console.WriteLine($"Performing DI");
        Interupts = false;
        return () => { };
    }

    public Action EI()
    {
        Console.WriteLine($"Performing EI");
        Interupts = true;
        return () => { };
    }

    #endregion

    #region Rotates

    public Action RLCA()
    {
        Console.WriteLine($"Performing RLCA");
        var bit7 = (RegisterA & 0b10000000) >> 7;
        RegisterA = (byte)(RegisterA << 1);

        ZeroFlag = RegisterA == 0;
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = bit7 == 1;

        return () => { };
    }

    public Action RRCA()
    {
        Console.WriteLine($"Performing RRCA");
        var bit0 = (RegisterA & 0b00000001);
        RegisterA = (byte)(RegisterA >> 1);

        ZeroFlag = RegisterA == 0;
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = bit0 == 1;

        return () => { };
    }

    public Action RLC(ref byte n)
    {
        Console.WriteLine($"Performing RLC");
        var bit7 = (n & 0b10000000) >> 7;
        n = (byte)(n << 1);

        ZeroFlag = n == 0;
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = bit7 == 1;

        return () => { };
    }

    public Action RRC(ref byte n)
    {
        Console.WriteLine($"Performing RRC");
        var bit0 = (n & 0b00000001);
        n = (byte)(n >> 1);

        ZeroFlag = n == 0;
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = bit0 == 1;

        return () => { };
    }

    #endregion

    #region Bitwise

    #endregion

    #region Jumps

    public Action JP() //uses 2 byte immediate value
    {
        var nn = getNN();
        Console.WriteLine($"Performing JP PC: {RegisterPC} NN: {nn}");
        RegisterPC = nn;
        RegisterPC--; ;
        return () => { };
    }

    public Action JPCC(bool flag) //Jump if conditions are true
    {
        Console.WriteLine($"Performing Jump = {flag}");
        if (flag)
            JP();
        else RegisterPC += 2;
        return () => { };
    }

    public Action JP_HL()
    {
        Console.WriteLine($"Peforming jump to {((RegisterH << 8) | RegisterL)}");
        RegisterPC = (UInt16)((RegisterH << 8) | RegisterL);
        RegisterPC--;
        return () => { };
    }

    public Action JR()
    {
        //Console.WriteLine($"Performing JR"); 
        var n = (sbyte)getN();
        Console.WriteLine($"Performing JR PC: {RegisterPC} N: {n}");
        RegisterPC += (UInt16)(n);
        return () => { };
    }
    public Action JRCC(bool flag)
    {
        Console.WriteLine($"Performing JRCC = {flag}");
        if (flag)
            JR();
        else RegisterPC += 1;
        return () => { };
    }

    #endregion

    #region Calls

    public Action Call16()
    {
        Console.WriteLine($"Performing Call16");
        Console.WriteLine($"Next address is {RegisterPC + 3}");
        //Push address of next instruction (PC + 3)
        var low = (byte)((RegisterPC + 3) & 0xFF);
        var high = (byte)(((RegisterPC + 3) >> 8) & 0xFF);
        Push(high, low);

        //Jump
        JP();
        return () => { };
    }

    public Action CallCC16(bool flag)
    {
        Console.WriteLine($"Performing CallCC16");
        if (flag)
            Call16();
        return () => { };
    }

    #endregion

    #region Returns

    public Action Ret()
    {
       Console.WriteLine($"Performing Ret");
        
        byte low = new byte();
        byte high = new byte();
        Pop(ref high, ref low);

        RegisterPC = (UInt16)(high << 8 | low);
        Console.WriteLine($"Returning to: {RegisterPC + 3}");
        //Pop 2 bytes from stack then JP
        return () => { };
    }

    public Action RetCC(bool flag)
    {
        Console.WriteLine($"Performing RetCC");
        if (flag)
            Ret();
        return () => { };
    }

    public Action RetI()
    {
        Console.WriteLine($"Performing RETI");
        Ret();
        EI();
        return () => { };
    }

    #endregion
}