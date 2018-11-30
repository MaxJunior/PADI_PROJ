using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using System.Runtime.Serialization.Formatters;

namespace RemotingSample {

    public class Client
    {

        private string name;
        private Uri url;
        private string fileName,urL;
        private TcpChannel channel;
        bool crash = false;

        public Client(string n, string url2, string file)
        {
            name = n;
            fileName = file;
            urL = url2;
        }

        public void setCrash(bool c)
        {
            crash = c;
        }

        public void executeByPuppet()
        {
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = 0;
            props["name"] = name;
            //props["ip"] = "1.2.3.4";
            TcpChannel channel = new TcpChannel(props, null, provider);
            ChannelServices.RegisterChannel(channel, false);
            MyRemoteInterface obj = (MyRemoteInterface)Activator.GetObject(
            typeof(MyRemoteInterface),
            "tcp://localhost:8086/MyRemoteObjectName");
            if (obj == null)
                System.Console.WriteLine("Could not locate server");
            executeMain(obj, 1, fileName);
            
        }

        public void status()
        {
            if (crash)
                Console.WriteLine("Client " + name + " crashed status");
            else
                Console.WriteLine("Client " + name + " available");
        }

        public static void repeatCmd(MyRemoteInterface obj, List<List<string>> field_list2, List<string> cmds)
        {
            int delay = 0;
            List<List<Field>> lField;
            for (int i=0;i<cmds.Count;i++)
            {
                
                switch (cmds[i])
                {
                    case "add":
                        obj.add(field_list2[i]);
                        break;
                    case "read":
                        lField = obj.readTuple(field_list2[i]);
                        while (lField == null)
                            lField = obj.readTuple(field_list2[i]);
                        foreach (List<Field> ls in lField)
                            foreach (Field f in ls)
                            {
                                if (f.getType() == 0 || f.getType() == 2)
                                    Console.WriteLine(f.getClassName());
                                else
                                    Console.WriteLine(f.getString());

                            }
                        break;
                    case "take":
                        lField = obj.take(field_list2[i]);
                        while (lField == null)
                            lField = obj.take(field_list2[i]);
                        foreach (List<Field> ls in lField)
                            foreach (Field f in ls)
                            {
                                if (f.getType() == 0 || f.getType() == 2)
                                    Console.WriteLine(f.getClassName());
                                else
                                    Console.WriteLine(f.getString());

                            }
                        break;

                    case "wait":
                        Int32.TryParse(field_list2[i][0], out delay);
                        System.Threading.Thread.Sleep(delay);
                        Console.WriteLine(delay);
                        break;
                }
            }
        }

        public static void executeMain(MyRemoteInterface obj, int args,string arg)
        {
            string str_fields = "";
            List<string> cmds = new List<string>();
            List<string> field_list = new List<string>();
            List<List<string>> field_list2 = new List<List<string>>();
            List<List<Field>> lField;
            string[] cmd_params;
            string cmd = "";
            int delay = 0;
            int repeat = 0;
            bool startRepeat = false;

            if (args == 1)
            {
                string[] lines = File.ReadAllLines(arg);
                foreach (string l in lines)
                {
                    if (l.Length == 0)
                        continue;
                    if (l.Contains("end"))
                        cmd = l;
                    else
                    {
                        cmd_params = l.Split(' ');
                        cmd = cmd_params[0];
                        str_fields = cmd_params[1];

                        if (cmd.Contains("repeat") || cmd.Equals("wait"))
                            Console.WriteLine("status command " + cmd + " " + cmd_params[1]);
                        else
                        {
                            if (cmd_params.Length > 2)
                                return;
                            field_list = argumentParser(str_fields);
                        }
                    }

                    Console.WriteLine(l);
                    

                    switch (cmd)
                    {
                        case "add":
                            obj.add(field_list);
                            if (repeat > 0)
                            {
                                field_list2.Add(field_list);
                                cmds.Add(cmd);
                            }
                            break;
                        case "read":
                            lField = obj.readTuple(field_list);
                            if (repeat > 0)
                            {
                                field_list2.Add(field_list);
                                cmds.Add(cmd);
                            }
                            while (lField==null)
                                lField = obj.readTuple(field_list);
                            foreach (List<Field> ls in lField)
                                foreach (Field f in ls)
                                {
                                    if (f.getType() == 0 || f.getType() == 2)
                                        Console.WriteLine(f.getClassName());
                                    else
                                        Console.WriteLine(f.getString());

                                }
                            break;
                        case "take":
                            if (repeat > 0)
                            {
                                field_list2.Add(field_list);
                                cmds.Add(cmd);
                            }
                            lField = obj.take(field_list);
                            while (lField == null)
                                lField = obj.take(field_list);
                            foreach (List<Field> ls in lField)
                                foreach (Field f in ls)
                                {
                                    if (f.getType() == 0 || f.getType() == 2)
                                        Console.WriteLine(f.getClassName());
                                    else
                                        Console.WriteLine(f.getString());

                                }
                            break;

                        case "wait":
                            Int32.TryParse(str_fields, out delay);
                            System.Threading.Thread.Sleep(delay);
                            Console.WriteLine(delay);
                            if (repeat > 0)
                            {
                                List<string> x = new List<string>();
                                x.Add(str_fields);
                                field_list2.Add(x);
                                cmds.Add(cmd);
                            }
                            break;
                        case "begin-repeat":
                            Int32.TryParse(str_fields, out repeat);
                            break;
                        case "end-repeat":
                            int v = 0;
                            foreach (List<string> ls in field_list2)
                            {
                                Console.WriteLine("TTT   "+cmds[v]);
                                foreach (string s in ls)
                                    Console.WriteLine("HHHH   " + s);
                                Console.WriteLine("TT222T");
                                v++;
                            }
                            for (int i = 0; i < (repeat - 1); i++)
                            {
                                Console.WriteLine(i + "    GFggg");
                                repeatCmd(obj, field_list2, cmds);
                            }
                            repeat = 0;
                            break;
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
                        string l = Console.ReadLine();
                        if (l.Length == 0)
                            continue;
                        cmd_params = l.Split(' ');
                        cmd = cmd_params[0];
                        str_fields = cmd_params[1].Replace("\n", "");
                        field_list = argumentParser(str_fields);
                        if (cmd.Equals("exit"))
                        {
                            Console.WriteLine("Exiting");
                            break;
                        }

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
                                        if (f.getType() == 0)
                                            Console.WriteLine(f.getClassName());
                                        else
                                            Console.WriteLine(f.getString());

                                    }
                                break;
                            case "take":
                                lField = obj.take(field_list);
                                foreach (List<Field> ls in lField)
                                    foreach (Field f in ls)
                                    {
                                        if (f.getType() == 0)
                                            Console.WriteLine(f.getClassName());
                                        else
                                            Console.WriteLine(f.getString());

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


            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            MyRemoteInterface obj = (MyRemoteInterface)Activator.GetObject(
            typeof(MyRemoteInterface),
            "tcp://localhost:8086/MyRemoteObjectName");
            if (obj == null)
                System.Console.WriteLine("Could not locate server");

            if (args.Length == 0)
                executeMain(obj, 0, "");
            else if (args.Length == 0)
            {
                executeMain(obj, 1, args[0]);
            }
            else
                return;
        }
        static List<string> argumentParser(string fields)
        {
           
            List<string> argsList = new List<string>();    
            int bracket_counter = 0;
            int quote = 0;
            string cur_field = "";
            int i = 0;
            if (!fields.StartsWith(" < ") && !fields.EndsWith(">"))
            { 
                return null;
            }
            fields = fields.Substring(1, fields.Length - 2);           
            while(i < fields.Length)
            {
                

                if (fields[i] == '"' && quote == 0)
                {
                    cur_field += fields[i];
                    i++;
                    while (fields[i] != '"')
                    {      
                        cur_field += fields[i];
                        i++;
                        quote = 1;
                    }

                    if (quote == 0)
                        return null;
                    cur_field += fields[i];
                    quote = 0;
                    argsList.Add(cur_field);
                    cur_field = "";
                    i++;
                    bracket_counter = 0;

                }

                else if(fields[i] >= 'a' && fields[i] <= 'z' || fields[i] >= 'A' && fields[i] <= 'Z')
                {
                    cur_field += fields[i];
                    i++;
                    while (fields[i] != '(' && fields[i] != ',')
                    {
                        cur_field += fields[i];
                        if (i == (fields.Length - 1))
                        {
                            break;
                        }
                        else
                            i++;
                    }
                    if (fields[i] == ',' || i == (fields.Length - 1))
                    {
                        argsList.Add(cur_field);
                        bracket_counter = 0;
                        cur_field = "";
                        i++;
                    }                   
                    else if (fields[i] == '(')
                    {
                        cur_field += fields[i];
                        i++;
                        string args = "";
                        while (fields[i] != ')')
                        {
                            args += fields[i];
                            i++;
                        }
                        
                        object[] splited = funcArgs(args.Split(','));
                        if (splited == null)
                            return null;
                        cur_field += args + fields[i];
                        argsList.Add(cur_field);
                        cur_field = "";
                        bracket_counter = 0;
                        i++;

                    }
                }
                else if(fields[i] == ',')
                {
                    bracket_counter = 1;
                    i++;
                    continue;
                }
            }     
            if (bracket_counter==0)
                return argsList;
            else
                return null;
        }

        public static object[] funcArgs(string[] splited)
        {
            object[] args = new object[splited.Length];
            int j;
            int i = 0;
            foreach (string field in splited)
            {
                if (field.StartsWith("\"") && field.EndsWith("\""))
                {

                    args[i] = field;
                }
                else
                {
                    try
                    {
                        Int32.TryParse(field, out j);
                        args[i] = j;
                    }
                    catch (FormatException) { return null; }
                }
                i++;
            }
            return args;
        }

    }
}