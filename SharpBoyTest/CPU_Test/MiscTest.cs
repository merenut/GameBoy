using SharpBoy.CPU;
using SharpBoy.Memory;

namespace SharpBoyTest.CPU_Test;

public class MiscTest
{
    [Fact]
    public void TestSwap()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterA = 0x67;

        //Test
        cpu.Swap8(ref cpu.RegisterA);

        //Assert
        Assert.Equal(0x76, cpu.RegisterA);
    }
}