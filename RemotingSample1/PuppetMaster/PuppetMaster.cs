using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster
{
    class PuppetMaster
    {
        static void Main(string[] args)
        {

            while (true) {
                
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

                }
        }
    }
}
