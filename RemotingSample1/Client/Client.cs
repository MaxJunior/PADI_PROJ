using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace RemotingSample {

    public class Client
    {

        private string name;
        private Uri url;
        private string fileName,urL;
        private TcpChannel channel;

        public Client(string n, string url2, string file)
        {
            name = n;
            //url = new Uri(url2);
            fileName = file;
            urL = url2;
        }


        public void executeByPuppet()
        {
            TcpChannel channel2 = new TcpChannel();
            ChannelServices.RegisterChannel(channel2, true);
            MyRemoteObject obj = (MyRemoteObject)Activator.GetObject(
                typeof(MyRemoteObject),
                "tcp://localhost:8086/MyRemoteObjectName");
            executeMain(obj, 1, fileName);
            
        }

        public static void executeMain(MyRemoteObject obj, int args,string arg)
        {
            List<List<Field>> lField;
            
            if (args == 1)
            {
                string[] lines = File.ReadAllLines(arg);
                int delay = 0;
                int repeat = 1;
                StreamWriter file = new StreamWriter("outC.txt");
                string str_fields="";     
                foreach (string l in lines)
                {
                    if (l.Length == 0)
                        continue;
                    Console.WriteLine("AAAAAAAAAAAAAAAAAAAAA");
                    string[] cmd_params = l.Split(' ');
                    string cmd = cmd_params[0]; 
                    if (cmd.Equals("begin-repeat") || cmd.Equals("wait"))
                        str_fields = cmd_params[1];
                    else if (cmd.Equals("end-repeat"))
                        Console.WriteLine("end repeat");
                    else
                    {
                        if (!cmd_params[1].StartsWith("<") && !cmd_params[1].EndsWith(">"))
                        {

                            file.Write("bad input");
                            break;

                        }
                        str_fields = cmd_params[1].Substring(1, cmd_params[1].Length - 2);
                    }
                     
                    int wildValue = 0;
                    // get the list of fields
                    List<string> field_list = argumentParser(str_fields, ref wildValue);

                    Console.WriteLine(str_fields+ "       CCCCCCCCCCCCCCCCCCCCCCCC");
                    for (int i = 0; i <= repeat-1; i++)
                    {

                        switch (cmd)
                        {
                            case "add":
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

                            case "wait":
                                Int32.TryParse(field_list[0], out delay);
                                System.Threading.Thread.Sleep(delay);
                                Console.WriteLine(delay);
                                break;
                            case "begin-repeat":
                                Int32.TryParse(field_list[0], out repeat);
                                break;
                            case "end-repeat":
                                repeat = 1;
                                break;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    while (true)
                    {

                        
                        Console.WriteLine("Write a command add|take|read <field1,field2,...,fieldn>  (exit:to leave) :");
                        string user_input = Console.ReadLine();
                        string[] cmd_params = user_input.Split(' ');
                        // get the command
                        string cmd = cmd_params[0];
                        // given string : <field1,...,fieldN> get  just field1,...,fieldN
                        if (cmd.Equals("exit"))
                            break; 

                        if (!cmd_params[1].StartsWith("<") && !cmd_params[1].EndsWith(">"))
                        {
                            Console.WriteLine("Invalid Command sintax: Please,use the correct command syntax.");
                            executeMain(obj, args,arg);
                            return;
                        }
                        string str_fields = cmd_params[1].Substring(1, cmd_params[1].Length - 2);
                        // wilcardValue
                        int wildValue = 0;
                        // get the list of fields     
                        List<string> field_list = argumentParser(str_fields, ref wildValue);
                       

                        switch (cmd)
                        {
                            case "add":
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
                            
                            default: Console.WriteLine("Invalid Command : {0}. Please,use the correct command syntax.", cmd); break;
                        }
                        
                    }
                }
                catch (SocketException)
                {
                    System.Console.WriteLine("Could not locate server");
                }
                catch (ArgumentOutOfRangeException)
                {
                    System.Console.WriteLine("bad input format1\nadd|take|read <field1,field2,...,fieldn>\nEx: add <\"ola\">");
                    executeMain(obj, args, arg);
                }
                catch (IndexOutOfRangeException)
                {
                    System.Console.WriteLine("bad input format2\nadd|take|read <field1,field2,...,fieldn>\nEx: add <\"ola\">");
                    executeMain(obj, args, arg);
                }
            }
            
        }


        static void Main(string[] args)
        {


            TcpChannel channel2 = new TcpChannel();
            ChannelServices.RegisterChannel(channel2, true);  
            MyRemoteObject obj = (MyRemoteObject)Activator.GetObject(
                typeof(MyRemoteObject),
                "tcp://localhost:8086/MyRemoteObjectName");

            if (args.Length == 0)
                executeMain(obj, 0, "");
            else if (args.Length == 0)
            {
                executeMain(obj, 1, args[0]);
            }
            else
                return;
        }
        static List<string> argumentParser(string fields, ref int wilcardFounded)
        {
            List<string> argsList = new List<string>();
            int bracket_counter = 0;
            
            string cur_field = "";
            fields = fields.Replace("\"", "");
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