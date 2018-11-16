using RemotingSample;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PuppetMaster
{
    class PuppetMaster
    {
        PuppetMaster() { }
            static void Main(string[] args)
        {
            PuppetMaster p = new PuppetMaster();
            Client c = new Client("c1", "", "script.txt");
            Client c2 = new Client("c12", "", "script.txt");
            Server s = new Server();
            s.executeByPuppet();
            Thread th = new Thread(new ThreadStart(c.executeByPuppet));
            th.Start();
            Thread th2 = new Thread(new ThreadStart(c2.executeByPuppet));
            th2.Start();
            Console.ReadLine();
            /*while (true) {
                
                string user_input = Console.ReadLine();
                string[] cmd_params = user_input.Split(' ');
                // get the command
                string cmd = cmd_params[0];

                switch (cmd)
                    {
                        case "Server":
                            
                            break;

                        case "Client":


                            break;

                        case "Status":

                            break;

                        case "Crash":

                            break;

                        case "Freeze":

                            break;

                        case "Unfrezze":

                            break;
                    }

                }*/
            

        }
    }
}

