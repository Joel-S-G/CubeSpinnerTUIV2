//communications to rust

using System;

while(true)
{
    string? command = Console.ReadLine();

    if(command== null)
    {
        break;
    }
    else if(command == "Ping")
    {
        Console.WriteLine("Pong");
    }
    else
    {
        Console.WriteLine("unknown command");
    }

    Console.WriteLine($"Recieved {command}");
    Console.Out.Flush();  

}