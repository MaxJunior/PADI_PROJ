using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RemotingSample
{
    public class MyRemoteObject : MarshalByRefObject
    {

        private IDictionary<string, List<List<Field>>> tupleSpace = new Dictionary<string, List<List<Field>>>();

        public string MetodoOla()
        {
            return "ola!";
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void add(List<string> l)
        {
            string first = l[0];
            string className, key;
            string[] splited;
            List<Field> aux = new List<Field>();
            if (!first.Contains("("))
                key = first;
            else
            {
                splited = first.Split(new Char[] { ',', '(', ')' });
                key = splited[0];
            }

            
            foreach (string s in l)
            {
                if (!s.Contains("("))
                {
                    aux.Add(new Field(s));
                }
                else
                {
                    splited = s.Split(new Char[] { ',', '(', ')' });
                    className = splited[0];
                    if (className.Equals("DADTestA"))
                    {
                        int i;
                        int.TryParse(splited[1], out i);
                        DADTestA t = new DADTestA(i, splited[2]);
                        aux.Add(new Field(t,1));
                                
                    }
                    else if (className.Equals("DADTestB"))
                    {
                        int i,j;
                        int.TryParse(splited[1], out i);
                        int.TryParse(splited[3], out j);
                        DADTestB t = new DADTestB(i, splited[2], j);
                        aux.Add(new Field(t,2));
                    }
                    else if (className.Equals("DADTestC"))
                    {
                        int i;
                        int.TryParse(splited[1], out i);
                        DADTestC t = new DADTestC(i, splited[2],splited[3]);
                        aux.Add(new Field(t,3));
                    }
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

            foreach (List<Field> ls in tupleSpace[key])
                foreach (Field f in ls)
                {
                    if (f.getTN() == 0)
                        Console.WriteLine(f.getString());
                    else
                        Console.WriteLine(f.getTest());

                }
           
        }


        public List<List<Field>> readTuple(List<string> l)
        {
            //FALTA COMECAr Por * ou NULL ou type
            string first = l[0];
            string className, key;
            string[] splited;
            List<Field> aux = new List<Field>();
            List<List<Field>> aux2 = new List<List<Field>>();
            int founded = 0;


            
            if (first.Equals("*"))
                key = first;
            else if (first.Equals("null"))
                key = first;
            if (!first.Contains("("))
                key = first;
            else
            {
                splited = first.Split(new Char[] { ',', '(', ')' });
                key = splited[0];
            }

            foreach (string s in l)
            {
                if (s.StartsWith("DADTest") && !s.Contains("("))
                {
                    aux.Add(new Field(s,3));
                }
                else if (!s.Contains("(") && !s.Equals("null"))
                {
                    aux.Add(new Field(s));
                }
                else if (s.Equals("null"))
                {
                    aux.Add(new Field(s, 2));
                }
                else
                {
                    splited = s.Split(new Char[] { ',', '(', ')' });
                    className = splited[0];
                    if (className.Equals("DADTestA"))
                    {
                        int i;
                        int.TryParse(splited[1], out i);
                        DADTestA t = new DADTestA(i, splited[2]);
                        aux.Add(new Field(t, 1));

                    }
                    else if (className.Equals("DADTestB"))
                    {
                        int i, j;
                        int.TryParse(splited[1], out i);
                        int.TryParse(splited[3], out j);
                        DADTestB t = new DADTestB(i, splited[2], j);
                        aux.Add(new Field(t, 2));
                    }
                    else if (className.Equals("DADTestC"))
                    {
                        int i;
                        int.TryParse(splited[1], out i);
                        DADTestC t = new DADTestC(i, splited[2], splited[3]);
                        aux.Add(new Field(t, 3));
                    }
                }
            }
            List<string> keys = new List<string>();
            if (key.Equals("null") || key.Equals("*"))
            {
                keys = tupleSpace.Keys.ToList();
            }
            else if (key.Contains("*"))
            {
                String[] subKey = key.Split('*');
                if (key.StartsWith("*"))
                {
                    foreach(string k in tupleSpace.Keys)
                    {
                        if (k.EndsWith(subKey[0]))
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
                        if (aux[i].getType() == 3 && ls[i].getType() == 0)
                        {
                            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBB     " + ls[i].getTest().GetType().Name);
                            if (aux[i].getString().Equals(ls[i].getTest().GetType().Name))
                                continue;
                            else {
                                eq = false;
                                break;
                            }
                        }
                        if (aux[i].getTN() != ls[i].getTN() && aux[i].getType() != 2)
                        {
                            eq = false;
                            break;
                        }

                        if (aux[i].getType() == 2 && ls[i].getType() == 0)
                            continue;
                        if (aux[i].getType() == 1 && ls[i].getType() == 1)
                        {
                            if (aux[i].getString().Equals("*"))
                            {
                                continue;
                            }
                            else if (aux[i].getString().Contains("*"))
                            {
                                String[] sTest = aux[i].getString().Split('*');
                                if (aux[i].getString().StartsWith("*"))
                                {
                                    if (ls[i].getString().EndsWith(sTest[0]))
                                        continue;
                                }
                                else if (aux[i].getString().EndsWith("*"))
                                {
                                    if (ls[i].getString().StartsWith(sTest[0]))
                                        continue;
                                }
                                else
                                {
                                    eq = false;
                                    break;
                                }
                            }
                        }
                        else if (aux[i].getTN() != 0)
                        {
                            if (aux[i].getType() == 2 && ls[i].getTN() != 0)
                                continue;
                            else if (ls[i].GetType().Equals(aux[i]))
                                continue;
                        }

                        else if (!ls[i].equals(aux[i]))
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
            return aux2;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<List<Field>> take(List<string> l) {

            string first = l[0];
            string className, key;
            string[] splited;
            List<Field> aux = new List<Field>();
            List<List<Field>> aux2 = new List<List<Field>>();
            List<int> takes = new List<int>();


            if (first.Equals("*"))
                key = first;
            else if (first.Equals("null"))
                key = first;
            if (!first.Contains("("))
                key = first;
            else
            {
                splited = first.Split(new Char[] { ',', '(', ')' });
                key = splited[0];
            }

            foreach (string s in l)
            {
                if (!s.Contains("(") && !s.Equals("null"))
                {
                    aux.Add(new Field(s));
                }
                else if (s.Equals("null"))
                {
                    aux.Add(new Field(s, 2));
                }
                else
                {
                    splited = s.Split(new Char[] { ',', '(', ')' });
                    className = splited[0];
                    if (className.Equals("DADTestA"))
                    {
                        int i;
                        int.TryParse(splited[1], out i);
                        DADTestA t = new DADTestA(i, splited[2]);
                        aux.Add(new Field(t, 1));

                    }
                    else if (className.Equals("DADTestB"))
                    {
                        int i, j;
                        int.TryParse(splited[1], out i);
                        int.TryParse(splited[3], out j);
                        DADTestB t = new DADTestB(i, splited[2], j);
                        aux.Add(new Field(t, 2));
                    }
                    else if (className.Equals("DADTestC"))
                    {
                        int i;
                        int.TryParse(splited[1], out i);
                        DADTestC t = new DADTestC(i, splited[2], splited[3]);
                        aux.Add(new Field(t, 3));
                    }
                }
            }

            List<string> keys = new List<string>();
            if (key.Equals("null") || key.Equals("*"))
            {
                keys = tupleSpace.Keys.ToList();
            }
            else if (key.Contains("*"))
            {
                String[] subKey = key.Split('*');
                if (key.StartsWith("*"))
                {
                    foreach (string k in tupleSpace.Keys)
                    {
                        if (k.EndsWith(subKey[0]))
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
            foreach (string k in keys)
            {
                foreach (List<Field> ls in tupleSpace[k])
                {
                    if (!(ls.Count == l.Count))
                        continue;
                    bool eq = true;
                    int j = 0;
                    for (int i = 0; i < ls.Count; i++)
                    {
                        if (aux[i].getType() == 3 && ls[i].getType() == 0)
                        {
                            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBB     " + ls[i].getTest().GetType().Name);
                            if (aux[i].getString().Equals(ls[i].getTest().GetType().Name))
                                continue;
                            else
                            {
                                eq = false;
                                break;
                            }
                        }
                        if (aux[i].getTN() != ls[i].getTN() && aux[i].getType() != 2)
                        {
                            eq = false;
                            break;
                        }

                        if (aux[i].getType() == 2 && ls[i].getType() == 0)
                            continue;
                        if (aux[i].getType() == 1 && ls[i].getType() == 1)
                        {
                            if (aux[i].getString().Equals("*"))
                            {
                                continue;
                            }
                            else if (aux[i].getString().Contains("*"))
                            {
                                String[] sTest = aux[i].getString().Split('*');
                                if (aux[i].getString().StartsWith("*"))
                                {
                                    if (ls[i].getString().EndsWith(sTest[0]))
                                        continue;
                                }
                                else if (aux[i].getString().EndsWith("*"))
                                {
                                    if (ls[i].getString().StartsWith(sTest[0]))
                                        continue;
                                }
                                else
                                {
                                    eq = false;
                                    break;
                                }
                            }
                        }
                        else if (aux[i].getTN() != 0)
                        {
                            if (aux[i].getType() == 2 && ls[i].getTN() != 0)
                                continue;
                            else if (ls[i].GetType().Equals(aux[i]))
                                continue;
                        }

                        else if (!ls[i].equals(aux[i]))
                        {
                            eq = false;
                            break;
                        }

                    }
                    if (eq)
                    {
                        takes.Add(j);
                        aux2.Add(ls);
                    }
                    j++;
                }
                foreach (int w in takes)
                    tupleSpace[k].RemoveAt(w);
            }
            return aux2;

        }

    }

    public class Test : MarshalByRefObject { }

    public class DADTestA : Test
        {
            public int i1;
            public string s1;

            public DADTestA(int pi1, string ps1)
            {
                i1 = pi1;
                s1 = ps1;
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

        public class DADTestB : Test
    {
            public int i1;
            public string s1;
            public int i2;

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

        public class DADTestC : Test
    {
            public int i1;
            public string s1;
            public string s2;

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
        private int type = 0, tN = 0;
        private string s;
        private Test test;

        public Field(string s2)
        {
            type = 1;
            s = s2;
        }
        public Field(string s2,int i)
        {
            type = i;
            s = s2;
        }
        public Field(Test t, int t2)
        {
            test = t;
            tN = t2;
        }

        public Test getTest()
        {
            return test;
        }

        public string getString()
        {
            return s;
        }

        public int getTN()
        {
            return tN;
        }
        public int getType()
        {
            return type;
        }

        public bool equals(Field f)
        {
            if (type == 1)
            {
                return s.Equals(f.getString());
            }
            else
            {
                if (tN == f.getTN())
                    return test.Equals(f.getTest());
                else
                    return false;
            }
        }
    }

}