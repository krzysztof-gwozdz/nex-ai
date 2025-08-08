using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI").Color(Color.Aquamarine1));
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
}
Console.WriteLine("Press any key to exit...");
Console.ReadKey();