using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace RemotingSample
{
    public class MyRemoteObject : MarshalByRefObject, MyRemoteInterface
    {

        private IDictionary<string, List<List<Field>>> tupleSpace = new Dictionary<string, List<List<Field>>>();

        public string MetodoOla()
        {
            return "ola!";
        }

        public void serialize(string k_str, File_Serializer fle)
        {
            string dir_name = k_str;

            //check in the directory  exists
            if (!Directory.Exists(dir_name))
            {
                Directory.CreateDirectory(dir_name);
            }

            File_Serializer serila = new File_Serializer();
            serila.s = fle.s;
            serila.test = fle.test;
            serila.type = fle.type;


            Random rnd = new Random();
            int id = rnd.Next(0, 2000);
            // add a txt file with object serialized in the directory
            string file_path_name = @"" + dir_name + "\\" + id + ".txt";
            TextWriter tx_writer = new StreamWriter(file_path_name); ;
            XmlSerializer x = new XmlSerializer(serila.GetType()); ;



            x.Serialize(tx_writer, serila);
            //Console.WriteLine("object was written in the file");
            //Console.WriteLine("obj : {0} ", t.ToString());
            tx_writer.Close();

        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void add(List<string> l)
        {
            List<Field> aux = new List<Field>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string key;
            if (l == null)
                return;
            string first = l[0];
            if (!first.Contains("("))
                key = first.Replace("\"", "");
            else
            {
                string[] key2 = first.Split(new char[] { '(', ')', ',' });
                key = key2[0];

            }
            if (first.Contains("*") || first.Equals("null"))
                return;


            foreach (string s in l)
            {     
                if (!s.Contains("("))
                {
                    if (s.Contains("\"") || s.Equals("null"))
                        aux.Add(new Field(s.Replace("\"","")));
                    else
                    {
                        Type type = assembly.GetType("RemotingSample." + s);
                        aux.Add(new Field(Activator.CreateInstance(type), 2));
                    }
                }
                else
                {
                    string[] splited = s.Split(new char[] { '(', ')', ',' });
                    string func = splited[0];
                    object[] o = new object[splited.Length - 2];
                    for (int i = 1; i < splited.Length - 1; i++)
                        o[i - 1] = funcArgs(splited[i]);
                    Type type = assembly.GetType("RemotingSample." + func);
                    aux.Add(new Field(Activator.CreateInstance(type, o), 0));


                }
                
            }

            if (tupleSpace.ContainsKey(key))
                tupleSpace[key].Add(aux);
            
            else
            {           
                List<List<Field>> aux2 = new List<List<Field>>();
                aux2.Add(aux);
                tupleSpace.Add(key, aux2);
                
            }
           
        }


        public List<List<Field>> readTuple(List<string> l)
        {
            List<Field> aux = new List<Field>();
            List<List<Field>> aux2 = new List<List<Field>>();
            Assembly assembly = Assembly.GetExecutingAssembly();

            if (l == null)
            {
                return null;
            }
            string first = l[0];
            int founded = 0;
            string key;

            if (!first.Contains("("))
                key = first.Replace("\"", "");
            else
            {
                string[] key2 = first.Split(new char[] { '(', ')', ',' });
                key = key2[0];
            }


            foreach (string s in l)
            {
                if (!s.Contains("("))
                {
                    if (s.Contains("\"") || s.Equals("null"))
                        aux.Add(new Field(s.Replace("\"","")));
                    else
                    {
                        Type type = assembly.GetType("RemotingSample." + s);
                        aux.Add(new Field(Activator.CreateInstance(type), 2));
                    }
                }
                else
                {
                    string[] splited = s.Split(new char[] { '(', ')', ',' });
                    string func = splited[0];
                    object[] o = new object[splited.Length - 2];
                    for (int i = 1; i < splited.Length - 1; i++)
                        o[i - 1] = funcArgs(splited[i]);
                    Type type = assembly.GetType("RemotingSample." + func);
                    aux.Add(new Field(Activator.CreateInstance(type, o), 0));


                }

            }


            List<string> keys = new List<string>();
            if (key.Equals("null") || key.Equals("*"))
            {
                keys = tupleSpace.Keys.ToList();
            }
            else if (key.Contains("*"))
            {
                string[] subKey = key.Split('*');
                if (key.StartsWith("*"))
                {
                    foreach(string k in tupleSpace.Keys)
                    {
                        if (k.EndsWith(subKey[1]))
                            key = k;
                    }
                    keys.Add(key);
                }
                else if (key.EndsWith("*"))
                {
                    foreach (string k in tupleSpace.Keys)
                    {
                        if (k.StartsWith(subKey[0]))
                            key = k;
                    }
                    keys.Add(key);
                }
            }
            else keys.Add(key);

            foreach (string k in keys) {
                foreach (List<Field> ls in tupleSpace[k])
                {
                    if (!(ls.Count == l.Count))
                        continue;
                    bool eq = true;
                    for (int i = 0; i < ls.Count; i++)
                    {
                        if (aux[i].equals(ls[i]))
                        {
                            continue;
                        }
                        else
                        {
                            eq = false;
                            break;
                        }

                    }
                    if (eq)
                    {
                        aux2.Add(ls);
                        founded = 1;
                    }
                }
            }
            if (founded == 1)
                return aux2;
            else
                return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<List<Field>> take(List<string> l)
        {
            foreach (string k in tupleSpace.Keys)
                Console.WriteLine(k + "RRRRRRRRRRRRRRR");
            List<Field> aux = new List<Field>();
            List<List<Field>> aux2 = new List<List<Field>>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<int> takes = new List<int>();

            if (l == null)
                return null;
            string first = l[0];
            int founded = 0;
            string key;
            
            if (!first.Contains("("))
                key = first.Replace("\"", "");
            else
            {
                string[] key2 = first.Split(new char[] { '(', ')', ',' });
                key = key2[0];
                
            }


            foreach (string s in l)
            {
                if (!s.Contains("("))
                {
                    if (s.Contains("\"") || s.Equals("null"))
                        aux.Add(new Field(s.Replace("\"", "")));
                    else
                    {
                        Type type = assembly.GetType("RemotingSample." + s);              
                        aux.Add(new Field(Activator.CreateInstance(type), 2));
                    }
                }
                else
                {
                    string[] splited = s.Split(new char[] { '(', ')', ',' });
                    string func = splited[0];
                    object[] o = new object[splited.Length - 2];
                    for (int i = 1; i < splited.Length-1; i++)
                        o[i - 1] = funcArgs(splited[i]);
                    Type type = assembly.GetType("RemotingSample." + func);
                    aux.Add(new Field(Activator.CreateInstance(type, o), 0));


                }

            }

            List<string> keys = new List<string>();
            if (key.Equals("null") || key.Equals("*"))
            {
                keys = tupleSpace.Keys.ToList();
            }
            else if (key.Contains("*"))
            {
                string[] subKey = key.Split('*');
                if (key.StartsWith("*"))
                {
                    foreach (string k in tupleSpace.Keys)
                    {
                        if (k.EndsWith(subKey[1]))
                            key = k;
                    }
                    keys.Add(key);
                }
                else if (key.EndsWith("*"))
                {
                    foreach (string k in tupleSpace.Keys)
                    {
                        if (k.StartsWith(subKey[1]))
                            key = k;
                    }
                    keys.Add(key);
                }
            }
            else keys.Add(key);
            
            foreach (string k in keys)
            {
                
               
                int j = 0;
                if (!tupleSpace.Keys.Contains(k))
                    continue;
                foreach (List<Field> ls in tupleSpace[k])
                {
                    if (!(ls.Count == l.Count))
                        continue;
                    bool eq = true;
                    for (int i = 0; i < ls.Count; i++)
                    {
                        if (aux[i].equals(ls[i]))
                            continue;
                        else
                        {
                            eq = false;
                            break;

                        }

                    }
                    if (eq)
                    {
                        aux2.Add(ls);
                        takes.Add(j);
                        founded = 1;
                        continue;
                    }
                    j++;
                }
                foreach (int w in takes)
                    tupleSpace[k].RemoveAt(w);
                if (tupleSpace[k].Count == 0)
                    tupleSpace.Remove(k);
            }
                if (founded == 1)
                return aux2;
            else
                return null;
        }

        public static object funcArgs(string splited)
        {
            object args;
            int j;
            if (splited.StartsWith("\"") && splited.EndsWith("\""))
            {       
                args = splited;
            }
            else
            {
                try
                {
                    Int32.TryParse(splited, out j);
                    args = j;
                }
                catch (FormatException) { return null; }
            }
            return args;
        }
    }


    public class DADTestA
        {
            public int i1;
            public string s1;

            public DADTestA() { }
            public DADTestA(int pi1, string ps1)
            {
                i1 = pi1;
                s1 = ps1;
            }
        public override string ToString()
        {
           return this.i1 + "    " + this.s1;
        }
            public bool Equals(DADTestA o)
            {
                if (o == null)
                {
                return false;
                }
                else
                {
                    return ((this.i1 == o.i1) && (this.s1.Equals(o.s1)));
                }
            }
        }

        public class DADTestB
    {
            public int i1;
            public string s1;
            public int i2;
            public DADTestB() { }
            public DADTestB(int pi1, string ps1, int pi2)
            {
                i1 = pi1;
                s1 = ps1;
                i2 = pi2;
            }

            public bool Equals(DADTestB o)
            {
                if (o == null)
                {
                    return false;
                }
                else
                {
                    return ((this.i1 == o.i1) && (this.s1.Equals(o.s1)) && (this.i2 == o.i2));
                }
            }
        }

        public class DADTestC
    {
            public int i1;
            public string s1;
            public string s2;

            public DADTestC() { }
            public DADTestC(int pi1, string ps1, string ps2)
            {
                i1 = pi1;
                s1 = ps1;
                s2 = ps2;
            }

            public bool Equals(DADTestC o)
            {
                if (o == null)
                {
                    return false;
                }
                else
                {
                    return ((this.i1 == o.i1) && (this.s1.Equals(o.s1)) && (this.s2.Equals(o.s2)));
                }
            }
        }

    public class Field : MarshalByRefObject
    {
        private int type = 0;
        private string s;
        private object test;

        public Field(string s2)
        {
            type = 1;
            s = s2;
        }

        public Field(object v, int i)
        {
            test = v;
            type = i;
        }

        public object getTest()
        {
            return test;
        }

        public string getString()
        {
            return s;
        }

        public int getType()
        {
            return type;
        }
        
        public string getClassName()
        {
            return test.GetType().Name;
        }

        public bool equals(Field f)
        {
            if (type == 1)
            {
                if (s.Equals("*") && f.getType() == 1)
                    return true;
                if(s.Equals("null") && f.getType() == 0)
                    return true;
                if (s.Contains("*") && f.getType() == 1)
                {
                    String[] sTest = s.Split('*');
                    if (s.StartsWith("*"))
                    {
                        if (f.getString().EndsWith(sTest[0]))
                            return true;
                    }
                    else if (s.EndsWith("*"))
                    {
                        if (f.getString().StartsWith(sTest[0]))
                            return true;
                    }
                    else
                    {
                        return false;
                    }
                } 
                return s.Equals(f.getString());
            }
            else
            {
                if (test.GetType().Name.Equals(f.getClassName()))
                {
                    if (type == 2)
                    {
                        return true;
                    }
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    Type typ = test.GetType();
                    MethodInfo mt = typ.GetMethod("Equals", new Type[] { typ });
                    object[] o = new object[1];
                    o[0] = f.getTest();
                    object result = mt.Invoke(test, o);    
                    return (bool)result;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    public class File_Serializer
    {
        public string type = "0";
        public string s;
        public string test;

        public File_Serializer() { }
        public File_Serializer(string str)
        {
            int i = 1;
            s = str;
            type = i.ToString();
            test = "null";
        }
        public File_Serializer(string s2, int i)
        {
            type = i.ToString();
            s = s2;

        }

    }

    public interface MyRemoteInterface
    {
        string MetodoOla();
        void add(List<string> l);
        List<List<Field>> readTuple(List<string> l);
        List<List<Field>> take(List<string> l);


    }
}