// See https://aka.ms/new-console-template for more information

using SharpBoy.CPU;
using SharpBoy.Graphics;
using SharpBoy.Memory;

var testpath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
testpath += "\\TestRoms\\PG.gb";

Console.WriteLine(new string('-', 80));
Console.WriteLine("Starting up....");
Console.WriteLine(new string('-', 80));

//Init Memory Controller
var mc = new MemoryController();
mc.LoadRom(testpath);

//Init CPU
var cpu = new GameboyCPU(mc);

//Init GUI
var gameWindow = new Window(mc.Title);

while (true)
{
}