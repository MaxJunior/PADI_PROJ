using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Threading;

namespace RemotingSample {

	public class Server {

        private static int mainS = 0;
        private static int replica = 0;

        public Server()
        {
        }

        public void executeByPuppet()
        {
            Thread th = new Thread(new ThreadStart(this.method));
            th.Start();
        }

        public static int MainS
        {
            get
            {
                return mainS;
            }
            set
            {
                mainS = value;
            }
        }

        
        public static int Replica
        {
            get
            {
                return replica;
            }
            set
            {
                replica = value;
            }
        }

        public void method()
        {
            if (mainS == 1)
            {
                BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
                provider.TypeFilterLevel = TypeFilterLevel.Full;
                IDictionary props = new Hashtable();
                props["port"] = 8086;
                //props["ip"] = "1.2.3.4";
                TcpChannel channel = new TcpChannel(props, null, provider);
                //TcpChannel channel = new TcpChannel(8086);
                ChannelServices.RegisterChannel(channel, false);
                MyRemoteObject mo = new MyRemoteObject();
                RemotingServices.Marshal(mo,
                "MyRemoteObjectName",
                typeof(MyRemoteObject));
            }
        }

        static void Main(string[] args) {

            if(mainS == 0 && replica == 0)
            {
                Server s1 = new Server(1, 0);
                Thread th = new Thread(new ThreadStart(s1.method));
                th.Start();
            }
			
      
			System.Console.WriteLine("<enter> para sair...");
			System.Console.ReadLine();
		}
	}
}