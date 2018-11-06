using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;

namespace RemotingSample {

	class Client {

        public static void exit()
        {
            Console.WriteLine("Exiting");
        }

        static void Main() {
            string command = "";
            string s;

            TcpChannel channel = new TcpChannel();
			ChannelServices.RegisterChannel(channel,true);
            List<string> splited;
            List<List<Field>> lField;


            MyRemoteObject obj = (MyRemoteObject) Activator.GetObject(
				typeof(MyRemoteObject),
				"tcp://localhost:8086/MyRemoteObjectName");

	 		try
	 		{
	 			Console.WriteLine(obj.MetodoOla());

                Console.WriteLine("Enter command <add / read / take> : ");
                command = Console.ReadLine();
                while (command != "exit")
                {

                    switch (command)
                    {
                        case "add":
                            Console.WriteLine("Insert tuple schema, ex: ola;DADTestA(1,'a')");
                            s = Console.ReadLine();
                            splited = s.Split(';').ToList();
                            obj.add(splited);
                            break;
                        case "read":
                            Console.WriteLine("Insert tuple schema, ex: ola;DADTestA(1,a)");
                            s = Console.ReadLine();
                            splited = s.Split(';').ToList();
                            lField = obj.readTuple(splited);
                            foreach (List<Field> ls in lField)
                                foreach (Field f in ls)
                                {
                                    if (f.getTN() == 0)
                                        Console.WriteLine(f.getString());
                                    else
                                        Console.WriteLine(f.getTest());

                                }
                            break;
                        case "take":
                            Console.WriteLine("Case 2");
                            break;
                    }
                    Console.WriteLine("Enter command:");
                    command = Console.ReadLine();
                }
                exit();

             }
	 		catch(SocketException)
	 		{
	 			System.Console.WriteLine("Could not locate server");
	 		}

		}

        
    }
}