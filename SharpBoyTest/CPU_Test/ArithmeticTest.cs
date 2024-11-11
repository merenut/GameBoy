using SharpBoy.CPU;
using SharpBoy.Memory;

namespace SharpBoyTest.CPU_Test;

public class ArithmeticTest
{
    [Fact]
    public void TestAdd8()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterA = 0x00;
        cpu.RegisterB = 0xFF;

        //Test
        cpu.Add8(cpu.RegisterA, ref cpu.RegisterB);

        //Assert
        Assert.Equal(0xFF, cpu.RegisterB);
    }

    [Fact]
    public void TestSub8()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterA = 0x10;
        cpu.RegisterB = 0xFF;

        //Test
        cpu.Sub8(cpu.RegisterA, ref cpu.RegisterB);

        //Assert
        Assert.Equal(0xEF, cpu.RegisterB);
    }

    [Fact]
    public void TestOr8()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterA = 0x06;
        cpu.RegisterB = 0xF0;

        //Test
        cpu.Or8(cpu.RegisterA, ref cpu.RegisterB);

        //Assert
        Assert.Equal(0xF6, cpu.RegisterB);
    }

    [Fact]
    public void TestAnd8()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterA = 0x06;
        cpu.RegisterB = 0xF0;

        //Test
        cpu.And8(cpu.RegisterA, ref cpu.RegisterB);

        //Assert
        Assert.Equal(0x00, cpu.RegisterB);
    }

    [Fact]
    public void TestXor8()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterA = 0x66;
        cpu.RegisterB = 0xF0;

        //Test
        cpu.And8(cpu.RegisterA, ref cpu.RegisterB);

        //Assert
        Assert.Equal(0x60, cpu.RegisterB);
    }

    [Fact]
    public void TestInc8()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterA = 0x66;

        //Test
        cpu.Inc8(ref cpu.RegisterA);

        //Assert
        Assert.Equal(0x67, cpu.RegisterA);
    }

    [Fact]
    public void TestDec8()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterA = 0x66;

        //Test
        cpu.Dec8(ref cpu.RegisterA);

        //Assert
        Assert.Equal(0x65, cpu.RegisterA);
    }

    [Fact]
    public void Add16()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterH = 0x00;
        cpu.RegisterL = 0x00;

        byte a = 0x06;
        byte b = 0xF0;

        //Test
        cpu.Add16((a, b), ref cpu.RegisterH, ref cpu.RegisterL);

        //Assert
        Assert.Equal(a, cpu.RegisterH);
        Assert.Equal(b, cpu.RegisterL);
    }

    [Fact]
    public void Inc16()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterH = 0x06;
        cpu.RegisterL = 0xF0;

        byte a = 0x06;
        byte b = 0xF0;

        //Test
        cpu.Inc16(ref cpu.RegisterH, ref cpu.RegisterL);

        //Assert
        Assert.Equal(a, cpu.RegisterH);
        Assert.Equal(b + 1, cpu.RegisterL);
    }

    [Fact]
    public void Dec16()
    {
        //Init
        var rom = new Rom("");
        var cpu = new GameboyCPU(new MemoryController());

        cpu.RegisterH = 0x06;
        cpu.RegisterL = 0xF0;

        byte a = 0x06;
        byte b = 0xF0;

        //Test
        cpu.Dec16(ref cpu.RegisterH, ref cpu.RegisterL);

        //Assert
        Assert.Equal(a, cpu.RegisterH);
        Assert.Equal(b - 1, cpu.RegisterL);
    }
}