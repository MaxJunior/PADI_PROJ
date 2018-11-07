using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;

namespace RemotingSample {

    class Client
    {

        static void Main()
        {


            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            List<List<Field>> lField;
            MyRemoteObject obj = (MyRemoteObject)Activator.GetObject(
                typeof(MyRemoteObject),
                "tcp://localhost:8086/MyRemoteObjectName");

            try
            {
                while (true)
                {
                    Console.WriteLine("Write a command add/take/read <field1,field2,...,fieldn>  (exit:to leave) :");
                    string user_input = Console.ReadLine();
                    string[] cmd_params = user_input.Split(' ');
                    // get the command
                    string cmd = cmd_params[0];
                    // given string : <field1,...,fieldN> get  just field1,...,fieldN
                    string str_fields = cmd_params[1].Substring(1, cmd_params[1].Length - 2);
                    // wilcardValue
                    int wildValue = 0;
                    // get the list of fields
                    Console.WriteLine(cmd_params[1]);
                    List<string> field_list = argumentParser(str_fields, ref wildValue);
                    bool endProcessCommand = false;

                    switch (cmd)
                    {
                        case "add":
                            foreach (string l in field_list)
                                Console.WriteLine(l);
                            obj.add(field_list);
                            break;
                        case "read":

                            lField = obj.readTuple(field_list);
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
                            lField = obj.take(field_list);
                            foreach (List<Field> ls in lField)
                                foreach (Field f in ls)
                                {
                                    if (f.getTN() == 0)
                                        Console.WriteLine(f.getString());
                                    else
                                        Console.WriteLine(f.getTest());

                                }
                            break;
                        case "exit":
                            endProcessCommand = true; break;
                        default: Console.WriteLine("Invalid Command : {0}. Please,use the correct command syntax.", cmd); break;
                    }
                    if (endProcessCommand)
                        break;
                }
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate server");
            }
        }
        static List<string> argumentParser(string fields, ref int wilcardFounded)
        {
            List<string> argsList = new List<string>();
            Console.WriteLine(fields);
            int bracket_counter = 0;
            Console.WriteLine(fields);
            string cur_field = "";
            for (int i = 0; i < fields.Length; i++)
            {
                
                //Console.WriteLine("MEW : {0} kkkkkkk  Value {1}", i, fields[i]);
                if (fields[i] == ',' && bracket_counter == 0)
                {
                    //Console.WriteLine(cur_field);
                    argsList.Add(cur_field);

                    cur_field = "";
                }
                else if (fields[i] == '(')
                {
                    cur_field += fields[i];
                    bracket_counter++;
                }
                else if (fields[i] == ',' && bracket_counter > 0)
                {
                    cur_field += fields[i];

                }
                else if (fields[i] == ')')
                {
                    cur_field += fields[i];
                    argsList.Add(cur_field);
                    //Console.WriteLine("Addd : {0}", cur_field);
                    cur_field = "";
                    bracket_counter = 0;
                }
                else
                {
                    cur_field += fields[i];
                    if (i == fields.Length - 1)
                        argsList.Add(cur_field);

                    if (fields[i] == '*')
                    {
                        Console.WriteLine("WilDCARD FOUND");
                        wilcardFounded = (cur_field == "\""  || cur_field == @"'") ? 1 : 2;
                        //Console.WriteLine("W : {0}", wilcardFounded);
                    }
                    //Console.WriteLine("Else  {0} ...",cur_field);
                }
            }
            return argsList;
        }

    }
}