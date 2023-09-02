using System.Reflection;

var commands = Environment.GetCommandLineArgs();
var text = string.Join("; ", commands.Skip(1));
Console.WriteLine($"Command Line: {text}");
var assembly = Assembly.GetExecutingAssembly();
var name = assembly.GetName().Name;
var info = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
Console.WriteLine($"Program: {name}/{info.InformationalVersion}");
