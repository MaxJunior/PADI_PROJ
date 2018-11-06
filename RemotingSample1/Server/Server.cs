using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace RemotingSample {

	class Server {

        private static int mainS = 0;
        private static int replica = 0;

        public Server(int main, int replicai)
        {
            MainS = main;
            replica = replicai;
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
                TcpChannel channel = new TcpChannel(8086);
                ChannelServices.RegisterChannel(channel, true);

                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(MyRemoteObject),
                    "MyRemoteObjectName",
                    WellKnownObjectMode.Singleton);
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