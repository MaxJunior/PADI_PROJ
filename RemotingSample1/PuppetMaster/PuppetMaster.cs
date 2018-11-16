using RemotingSample;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster
{
    class PuppetMaster
    {
        static void Main(string[] args)
        {
            Client c = new Client("c1", "", "script.txt");
            Server s = new Server();
            s.executeByPuppet()
            c.executeByPuppet();
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

