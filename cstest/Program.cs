for (;;) 
{
    string? input = Console.ReadLine();
    Console.WriteLine(input is null);
    if (input is null) 
    {
        break;
    }
}
Console.WriteLine("exit");