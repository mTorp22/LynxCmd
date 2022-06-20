using System;
using System.Net;
using System.Net.Sockets;


namespace LynxCmd
{
    class Program
    {

        static void Main(string[] args)   //1: IP, 2:command
        {
            bool quietmode = false;
            LynxMsg lm = new LynxMsg();
            IPAddress IA = null;
            bool ok = false;
            
            switch (args.Length)
            {
                case 0:
                    Console.WriteLine("Try parse -h for help or -v for version" + Environment.NewLine);
                    break;
                case 1:
                    switch (args[0].ToString().ToLower())
                    {
                        case "-h":
                            
                            Console.WriteLine(Properties.Resources.HelpFile);

                            //Console.Write("Give me 2 parameters, 1: Ip Address of TLX/FLX \n 2: Command \n Commands are:\n EnergyNow \n ProductionToday \n 6:Move file to backup after succesful sql upload, True/false " + Environment.NewLine);
                            break;
                        case "-v":
                            Console.WriteLine(System.Reflection.Assembly.GetEntryAssembly().GetName().Version);

                            break;
                        default:
                            Console.WriteLine("Unknown argument" + Environment.NewLine);
                            break;



                    }
                    break;
                case 2:     //Cast arg[0] to IP of ok check if args[1] is on the command list... ENUM?
                    try
                    {
                        IA = IPAddress.Parse(args[0]);
                        ok = true;
                        quietmode = false;
                    }
                    catch (Exception)
                    {
                        ok = false;   //stating the obivious
                        
                    }
                    
                    break;
                case 3:
                    try
                    {
                        IA = IPAddress.Parse(args[0]);
                        if (args[2].ToLower() == "-q")
                        {
                            ok = true;
                            quietmode = true;
                        }
                        else {
                            ok = false;
                            quietmode = true;
                        }
                        
                    }
                    catch (Exception)
                    {
                        ok = false;   //stating the obivious

                    }
                    break;
                default:
                    Console.WriteLine("You supplied " + args.Length.ToString() + " arguments I want 2/3, try parse -h for help");

                    foreach (string a in args)
                    {
                        Console.WriteLine(a);

                    }
                    break;
                    
            }


            if (ok) {
                try
                {
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
                   // IPEndPoint endPoint = new IPEndPoint(IA, 48004);
                    IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] send_buffer = lm.LynxEncode(args[1]);
                    byte[] recv_buffer;
                    string[] ss;
                    if (send_buffer != null)
                    {
                        UdpClient client = new UdpClient();
                        client.Send(send_buffer, send_buffer.Length, args[0], 48004);  //Etherlynx port is 48004
                        recv_buffer = client.Receive(ref remotePoint);
                        string s = lm.LynxDecode(recv_buffer);
                        if (quietmode)
                        {
                            ss = s.Split(' ');
                            string d = ss[ss.Length - 2].ToString() + " " + ss[ss.Length - 1].ToString();
                            Console.WriteLine(d);

                        }
                        else {
                            Console.WriteLine(s);
                        }
                      
                        Environment.Exit(0);    //Alles palletti!!
                    }
                    else {
                        Console.WriteLine("Wrong argument parsed, nothing to send");
                        Environment.Exit(-1);
                    }
                    
                }
                catch (Exception)
                {

                    throw;
                }


            }
            
        }

    }    
}
