//communications to rust

using System;

while(true)
{
    string? command = Console.ReadLine();

    if(command== null)
    {
        break;
    }

    Console.WriteLine($"Recieved {command}");
    Console.Out.Flush();  

}