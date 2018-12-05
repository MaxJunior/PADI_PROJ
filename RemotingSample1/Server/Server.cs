using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;

namespace RemotingSample {

	public class Server {

        private static int mainS = 0;
        private int max_delay;
        private int min_delay;
        private string id;
        private Uri uri;
        bool crash = false;
        bool freeze = false;
        private MyRemoteObject mo;
        private bool receivedAlive = false;
        private int invokeCount;
        private int changing=0;
        private int maxCount = 10;
        private int replica;
        private int numReplicas = 0;
        TcpChannel channel;
        public Server(string id2, Uri uri2, int min, int max, int v)
        {
            id = id2;
            uri = uri2;
            min_delay = min;
            max_delay = max;
            replica = v;
        }

        public Server()
        {
        }

        public void setCrash(bool c)
        {
            crash = c;
            RemotingServices.Disconnect(mo);
            channel.StopListening(mo);
            //Environment.Exit(1);
        }

        public void setReplicas(int n)
        {
            numReplicas = n;
            mo.setReplicas(n);
        }

        public void setFreeze(bool c)
        {
            freeze = c;
            mo.setFreeze(c);
        }

        public void executeByPuppet()
        {
            Console.WriteLine("Server " + id + " is crashed: " + crash + " and is freeze: " + freeze);
            Thread th = new Thread(new ThreadStart(this.method));
            th.Start();
        }

        public void status()
        {
            Console.WriteLine("Server " + id + " is crashed: " + crash + " and is freeze: "+freeze);
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

        public void receiveAlive()
        {      
            var autoEvent = new AutoResetEvent(false);
            var stateTimer = new Timer(CheckStatus, autoEvent, 1000, 500);

            Thread th = new Thread(new ThreadStart(checking));
            th.Start();

            autoEvent.WaitOne();
            stateTimer.Dispose();
            RemotingServices.Marshal(mo,
                 "MyRemoteObjectName",
                 typeof(MyRemoteObject));
            replica = 0;
            sendAlive();
        }

        public void checking()
        {
            var Server = new UdpClient(8888);

            while (true)
            {
                if (changing == 1)
                    break;
                var ClientEp = new IPEndPoint(IPAddress.Any, 8888);
                var ClientRequestData = Server.Receive(ref ClientEp);
                var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);

                Console.WriteLine("Im server {0} and Recived {1} from {2}", id, ClientRequest, ClientEp.Address.ToString());
                if (ClientRequest.Equals("alive"))
                    receivedAlive = true;
            }
        }

        public static string GetIPAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        public void sendAlive()
        {
            var autoEvent = new AutoResetEvent(false);
            var stateTimer = new Timer(sendStatus, autoEvent, 1000, 500);
            Console.WriteLine("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE  " + numReplicas);
            while (true)
            {
                if (crash)
                {
                    stateTimer.Dispose();
                    break;
                }
                autoEvent.WaitOne();
                Console.WriteLine("Alive sent");
            }
            
        }





        // This method is called by the timer delegate.
        public void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            invokeCount++;

            if (invokeCount == maxCount) {

                if (!receivedAlive)
                {
                    // Reset the counter and signal the waiting thread.

                    invokeCount = 0;
                    changing = 1;
                    autoEvent.Set();
                }
                else
                {
                    Console.WriteLine("Checking status {0}.", DateTime.Now.ToString("h:mm:ss.fff"));
                    invokeCount = 0;
                    receivedAlive = false;
                }
            }    

        }

        public void sendStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            
            invokeCount++;

            if (invokeCount == maxCount)
            {
                Console.WriteLine("Im server {0} and im am sending alive broadcast {1}", id, DateTime.Now.ToString("h:mm:ss.fff"));
                var Client = new UdpClient();
                var RequestData = Encoding.ASCII.GetBytes("alive");

                Client.EnableBroadcast = true;
                Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));
                Client.Close();
                // Reset the counter and signal the waiting thread.
                invokeCount = 0;
                autoEvent.Set();
            }
        }
        


            public void method()
        {
           
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            int port = 8086;
            if (uri != null)
            {
                props["port"] = uri.Port;
                port = uri.Port;
            }
            else
                props["port"] = 8086;
            if (id!=null)
                props["name"] = id;
            channel = new TcpChannel(props, null, provider);
            //String reference = "tcp://" + GetIPAddress() + ":" + port + "/MyRemoteObjectName";    
            mo = new MyRemoteObject();
            
            if (replica==0) {
                RemotingServices.Marshal(mo,
                 "MyRemoteObjectName",
                 typeof(MyRemoteObject));
                maxCount = 8;
                sendAlive();
                Console.WriteLine("RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR");
            }
            else
            {
                
                receiveAlive();     
            }
            


        }

        static void Main(string[] args) {

                Server s1 = new Server();
                Thread th = new Thread(new ThreadStart(s1.method));
                th.Start();

			System.Console.WriteLine("<enter> para sair...");
			System.Console.ReadLine();
		}
	}
}