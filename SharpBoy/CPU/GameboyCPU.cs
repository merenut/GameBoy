using SharpBoy.Memory;

namespace SharpBoy.CPU;

public class GameboyCPU(MemoryController mc)
{
    /// <summary>
    ///     Special Registers
    /// </summary>
    public byte FF00 { get; set; } //P1

    public byte FF01 { get; set; } //SB
    public byte FF02 { get; set; } //SC 
    public byte FF04 { get; set; } //DIV
    public byte FF05 { get; set; } //TIMA
    public byte FF06 { get; set; } //TMA
    public byte FF07 { get; set; } //TAC - Timer Control
    public byte FF0F { get; set; } //IF - Interupt Flag
    public byte FF10 { get; set; } //NR10 - Sound Mode
    public byte FF11 { get; set; } //NR11 - Sount length
    public byte FF12 { get; set; } //NR12 - Envelope

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

    public short RegisterPC { get; set; } //Points to the next instruction (starts at $100)

    public bool ZeroFlag { get; set; }
    public bool SubFlag { get; set; }
    public bool HalfCarryFlag { get; set; }
    public bool CarryFlag { get; set; }

    public bool Interupts { get; set; }

    public void PerformNext()
    {
    }

    /// <summary>
    ///     Op Lookup (Had to return empty Action due to .Net 8 switch limitations :))
    /// </summary>
    /// <param name="current"></param>
    public void Lookup(byte current)
    {
        (current switch
        {
            0x00 => (Action)(() => { })
        })();
    }

    #region Restarts

    public void Restart(byte n)
    {
        RegisterPC = n;
    }

    #endregion

    private void Cycle(short inc)
    {
        RegisterPC += inc;
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
    public Action LD8(byte v1, ref byte v2)
    {
        v2 = v1;
        return () => { };
    }

    public Action LD16(byte v1, byte v2, ref byte n1, ref byte n2)
    {
        n1 = v1;
        n2 = v2;
        return () => { };
    }

    #endregion

    #region Stack

    #endregion

    #region Arithmetic

    /// <summary>
    ///     Add 8 bit - add nn to n (result stored in n)
    /// </summary>
    public Action Add8(byte nn, ref byte n)
    {
        n += nn;
        return () => { };
    }

    public void Sub8(byte nn, ref byte n)
    {
        n -= nn;
    }

    public void Or8(byte nn, ref byte n)
    {
        n |= nn;
    }

    public void And8(byte nn, ref byte n)
    {
        n &= nn;
    }

    public void Xor8(byte nn, ref byte n)
    {
        n ^= nn;
    }

    public void CMP8(byte nn, ref byte n)
    {
    }

    public void Inc8(ref byte n)
    {
        Add8(1, ref n);
    }

    public void Dec8(ref byte n)
    {
        Sub8(1, ref n);
    }

    public void Add16((byte, byte) nn, ref byte n1, ref byte n2)
    {
        var nncom = (nn.Item1 << 8) | nn.Item2;
        var ncom = (n1 << 8) | n2;

        var result = nncom + ncom;

        n2 = (byte)(result & 0xFF);
        n1 = (byte)(result >> 8);
    }

    public void Inc16(ref byte n1, ref byte n2)
    {
        var ncom = (n1 << 8) | n2; //registers combined into int

        ncom++;

        n2 = (byte)(ncom & 0xFF);
        n1 = (byte)(ncom >> 8);
    }

    public void Dec16(ref byte n1, ref byte n2)
    {
        var ncom = (n1 << 8) | n2; //registers combined into int

        ncom--;

        n2 = (byte)(ncom & 0xFF);
        n1 = (byte)(ncom >> 8);
    }

    #endregion

    #region Misc

    public void Swap8(ref byte n)
    {
        var upper = n >> 4;
        var lower = n & 0x0F;

        n = (byte)((lower << 4) | upper);
    }

    public void CPL8(ref byte n)
    {
        n = (byte)~n;
    }

    public void CCF()
    {
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = !CarryFlag;
    }

    public void SCF()
    {
        SubFlag = false;
        HalfCarryFlag = false;
        CarryFlag = true;
    }

    public void DI()
    {
        Interupts = false;
    }

    public void EI()
    {
        Interupts = true;
    }

    #endregion

    #region Rotates

    #endregion

    #region Bitwise

    #endregion

    #region Jumps

    public void JP() //uses 2 byte immediate value
    {
        //int address = rom.GetRom()[RegisterPC + 4] << 8 | rom.GetRom()[RegisterPC + 8];
        //RegisterPC = (short)address;
    }

    public void JPCC(bool flag) //Jump if conditions are true
    {
        if (flag)
            JP();
    }

    public void JP_HL()
    {
        RegisterPC = (short)((RegisterH << 8) | RegisterL);
    }

    public void JR()
    {
        //RegisterPC += rom.GetRom()[RegisterPC+4];
    }

    public void JRCC(bool flag)
    {
        if (flag)
            JR();
    }

    #endregion

    #region Calls

    public void Call16()
    {
        //Push address of next instruction (PC + 12)

        //Jump
        JP();
    }

    public void CallCC16(bool flag)
    {
        if (flag)
            Call16();
    }

    #endregion

    #region Returns

    public void Ret()
    {
        //Pop 2 bytes from stack then JP
    }

    public void RetCC(bool flag)
    {
        if (flag)
            Ret();
    }

    public void RetI()
    {
        Ret();
        //TODO enable interupts
    }

    #endregion
}