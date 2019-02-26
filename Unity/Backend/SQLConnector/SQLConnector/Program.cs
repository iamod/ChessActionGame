using System;
using System.Configuration;
using System.Data;
using ConsoleApp1;
using MySql.Data;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Net;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Sha1Hasher hashObj = new Sha1Hasher();
            using (var client = new SshClient("shell1.sigmatic.fi", "shakkili", "2600_Chess_GM"))
            {
                string connectionStr = "server=mysql2.sigmatic.fi;user=shakkili_master;database=shakkili_avainsiirto;port=22;password=abckukko";
                client.Connect();
                if (client.IsConnected)
                {

                    Console.WriteLine("Connected to SSH Tunnel");
                    var portForwarded = new ForwardedPortLocal("127.0.0.1", 3306, "127.0.0.1", 22);
                    client.AddForwardedPort(portForwarded);
                    portForwarded.Start();
                    MySqlConnection conn = new MySqlConnection(connectionStr);

                    try
                    {
                        Console.WriteLine("Connecting to mySQL DB");
                        conn.Open();
                        Console.WriteLine("Managed to connect to DB");
                    }
                    catch (Exception exc)
                    {

                        Console.WriteLine(exc.ToString());
                    }

                    ConsoleKeyInfo a = Console.ReadKey();
                    conn.Close();
                }
                Console.WriteLine("Connection to SSH Tunnel failed");
            }

        }
    }

}