using SharpBoy.CPU;
using SharpBoy.Memory;

namespace SharpBoyTest.CPU_Test;

public class LDTest
{
    [Fact]
    public void TestLD8()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterA = 0x00;
        cpu.RegisterB = 0xFF;

        //Test
        cpu.LD8(cpu.RegisterA, ref cpu.RegisterB);

        //Assert
        Assert.Equal(0x00, cpu.RegisterB);
    }
}