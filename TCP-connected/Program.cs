using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Transactions;
using TCP_connected;

TcpListener server = new TcpListener(IPAddress.Any, 8888);
StreamReader reader;
Users users = new Users(readFile());
int numberOfClient = 0;
server.Start();
Console.WriteLine("Сервер запущено. Чекає з'єднань...");
try
{
    while (true)
    {
        TcpClient tcpClient = await server.AcceptTcpClientAsync();
        numberOfClient++;
        Task.Run(async () => await ProcessClientAsync(tcpClient));
    }
}
catch (SocketException)
{
    Console.WriteLine(" ");
}
catch (Exception ex)
{
    Console.WriteLine(ex.StackTrace);
}
finally
{
    server.Stop();
}

async Task ProcessClientAsync(TcpClient tcpClient)
{
    Console.WriteLine($"Адреса під'єднаного користувача: {tcpClient.Client.RemoteEndPoint}");
    Stream stream = tcpClient.GetStream();
    List<byte> buffer = new List<byte>();
    int bytesRead = 0;
    while (true)
    {

        while ((bytesRead = stream.ReadByte()) != '\n')
        {
            buffer.Add((byte)bytesRead);
        }
        string request = Encoding.UTF8.GetString(buffer.ToArray());
        if (request == "Close")
        {
            break;
        }

        string[] dataForCheck = request.Split(' ');

        await stream.WriteAsync(Encoding.UTF8.GetBytes(users.SearchUser(dataForCheck[0], dataForCheck[1])));
        buffer.Clear();
    }
    Console.WriteLine($"Адреса від'єднаного користувача: {tcpClient.Client.RemoteEndPoint}");
    numberOfClient--;
    tcpClient.Close();
    if (CheckByUsers())
    {
        server.Stop();
    }
}

bool CheckByUsers()
{
    if (numberOfClient == 0)
    {
        while (true)
        {
            Console.WriteLine("Немає з'єднань. Продовжити роботу сервера ?(Y/n):");
            string ans = Console.ReadLine();
            if (ans == "Y")
            {
                break;
            }
            
            if (ans == "n")
            {
                return true;
            }
        }
        return false;
    }
    return false;
}

User[] readFile()
{
    while (true)
    {
        try
        {
            Console.WriteLine("Введіть шлях до файлу з парлями та логінами:");
            string? filePath = Console.ReadLine();
            reader = new StreamReader(@filePath);
            string[] list = reader.ReadToEnd().Split(new char[] { '\n', ' ' });
            User[] users = new User[list.Length / 2];
            for (int i = 0; i < list.Length; i += 2)
            {
                users[i / 2] = new User(list[i], list[i + 1].TrimEnd('\r'));
            }
            return users;
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
} 
