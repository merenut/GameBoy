using SharpBoy.CPU;
using SharpBoy.Memory;

namespace SharpBoyTest.CPU_Test;

public class LDTest
{
    [Fact]
    public void TestLD8()
    {
        //Init
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterA = 0x00;
        cpu.RegisterB = 0xFF;

        //Test
        cpu.LD8(cpu.RegisterA, ref cpu.RegisterB, 4);

        //Assert
        Assert.Equal(0x00, cpu.RegisterB);
    }

    [Fact]
    public void TestPUSHPOP()
    {
        MemoryController memC = new();
        GameboyCPU cpu = new(memC);
        cpu.Initialize();

        byte b1 = 0x12;
        byte b2 = 0x34;
        byte r1 = 0x00;
        byte r2 = 0x00;

        cpu.Push(b1, b2);
        cpu.Pop(ref r1, ref r2);

        Assert.Equal(b1, r1);
        Assert.Equal(b2, r2);

        cpu.RegisterPC = 0x100;
        cpu.Call16();

        cpu.Ret();
        Assert.Equal(cpu.RegisterPC, 0x103);
    }
}