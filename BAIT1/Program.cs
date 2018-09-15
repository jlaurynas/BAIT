using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace BAIT1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> darz_id = new List<int>();
            List<string> school_name = new List<string>();
            List<int> type_id = new List<int>();
            List<string> type_label = new List<string>();
            List<int> lan_id = new List<int>();
            List<string> lan_label = new List<string>();
            List<int> childs_count = new List<int>();
            List<int> free_space = new List<int>();

            string[] c = readfile(@"C:\Users\lauri\Desktop\csv.csv");
            for(int i=1; i<c.Length; i++)
            {
                darz_id.Add(int.Parse(c[i].Split(',')[0]));                
                school_name.Add(c[i].Split(',')[1]);         
                type_id.Add(int.Parse(c[i].Split(',')[2]));
                if (c[i].Split(',')[3].Length > 7 || c[i].Split(',')[3].Contains('.')) 
                {
                    type_label.Add(c[i].Split(',')[3]);
                    lan_id.Add(int.Parse(c[i].Split(',')[4]));
                    lan_label.Add(c[i].Split(',')[5]);
                    childs_count.Add(int.Parse(c[i].Split(',')[6]));
                    free_space.Add(int.Parse(c[i].Split(',')[7]));
                }
                else
                {
                    type_label.Add(c[i].Split(',')[3] + "," +c[i].Split(',')[4]);
                    lan_id.Add(int.Parse(c[i].Split(',')[5]));
                    lan_label.Add(c[i].Split(',')[6]);
                    childs_count.Add(int.Parse(c[i].Split(',')[7]));
                    free_space.Add(int.Parse(c[i].Split(',')[8]));
                }              
            }
            int[] max = find_max(childs_count);
            int[] min = find_min(childs_count);
            //Didziausi
            Console.WriteLine("-----------------------Didziausi-------------------");
            for(int i=0; i< max.Length; i++)
            {
                type_label[max[i]] = type_label[max[i]].Replace("iki", " - ");
                  Console.WriteLine(school_name[max[i]].Substring(0, 3) + "_" + Regex.Replace(type_label[max[i]], "[^0-9.,-]", "") + "_"+ lan_label[max[i]].Substring(0,4));

                using (StreamWriter writetext = new StreamWriter("eilutes.txt"))
                {
                    writetext.WriteLine(school_name[max[i]].Substring(0, 3) + "_" + Regex.Replace(type_label[max[i]], "[^0-9.,-]", "") + "_" + lan_label[max[i]].Substring(0, 4));
                }

            }
            //Maziausi
            Console.WriteLine("------------------------Maziausi-------------");
            for (int i = 0; i < min.Length; i++)
            {
                type_label[min[i]] = type_label[min[i]].Replace("iki", " - ");
                using (FileStream fs = new FileStream("eilutes.txt", FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(school_name[min[i]].Substring(0, 3) + "_" + Regex.Replace(type_label[min[i]], "[^0-9.,-]", "") + "_" + lan_label[min[i]].Substring(0, 4));
                }
                Console.WriteLine(school_name[min[i]].Substring(0, 3) + "_" + Regex.Replace(type_label[min[i]], "[^0-9.,-]", "") + "_" + lan_label[min[i]].Substring(0, 4));
            }

            vietos(lan_label, childs_count, free_space);
            paskutine(free_space, school_name);
            Console.Read();
        }

        static string[] readfile(string filePath)
        {
            string[] text = System.IO.File.ReadAllLines(filePath);
            return text;
        }

        static int[] find_max(List<int> child_count)
        {
            int x = child_count.Max();
            List<int> skaiciai = new List<int>();
            for(int i = 0; i<child_count.Count; i++)
            {
                if(child_count[i] == x)
                {
                    skaiciai.Add(i);
                }
            }
            return skaiciai.ToArray();
        }

        static int[] find_min(List<int> child_count)
        {
            int x = child_count.Min();
            List<int> skaiciai = new List<int>();
            for (int i = 0; i < child_count.Count; i++)
            {
                if (child_count[i] == x)
                {
                    skaiciai.Add(i);
                }
            }
            return skaiciai.ToArray();
        }

        static void vietos(List<string> lan, List<int> child_count, List<int> free_space)
        {
            List<string> temp_lan = new List<string>();
            List<int> temp_space = new List<int>();
            List<int> temp_count = new List<int>();

            for(int i=1; i<child_count.Count; i++)
            {
                if(temp_lan.Count == 0)
                {
                    temp_lan.Add(lan[i]);
                    temp_count.Add(0);
                    temp_space.Add(0);
                }
                else
                {
                    bool check = false;
                    for(int j=0; j<temp_lan.Count; j++)
                    {
                        if(temp_lan[j] == lan[i])
                        {
                            check = true;
                        }
                    }
                    if(!check)
                    {
                        temp_lan.Add(lan[i]);
                        temp_count.Add(0);
                        temp_space.Add(0);
                    }
                }

            }

            for(int i=0; i<lan.Count; i++)
            {
                for(int j=0; j<temp_lan.Count; j++)
                {
                    if(lan[i] == temp_lan[j])
                    {
                        temp_count[j] += child_count[i];
                        temp_space[j] += free_space[i];
                    }

                }
            }
            double max = 0;
            int max_i = 0;
            for(int i=0;i<temp_space.Count; i++)
            {
               double temp = (double)((double)temp_space[i] / (double)(temp_count[i] + temp_space[i])) * 100.0f;
                if(temp > max)
                {
                    max = temp;
                    max_i = i;
                }
            }

            using (StreamWriter sw = new StreamWriter("vietos.txt"))
            {
                sw.WriteLine(temp_lan[max_i] + " " + Math.Round(max,2));
            }
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine((temp_lan[max_i] + " " + Math.Round(max, 2)));
        }

        static void paskutine(List<int> free_space, List<string> school_name)
        {
            List<string> temp_name = new List<string>();
            for(int i=0; i<free_space.Count; i++)
            {
                if(free_space[i] >= 2 && free_space[i] <= 4)
                {
  
                    temp_name.Add(school_name[i]);
                }
            }

            temp_name.Sort();
            Console.WriteLine("--------------------------------------------");
            for(int i=temp_name.Count-1; i>=0; i--)
            {       
                if (i == temp_name.Count-1)
                {
                    File.Delete("pavadinimai.txt");
                    using (var tw = new StreamWriter("pavadinimai.txt"))
                    {
                        tw.WriteLine(temp_name[i]);
                    }
                }
                else
                {
                    using (var tw = new StreamWriter("pavadinimai.txt", true))
                    {

                        tw.WriteLine(temp_name[i]);
                    }
                }

                Console.WriteLine(temp_name[i]);
            }
        }

    }
}
