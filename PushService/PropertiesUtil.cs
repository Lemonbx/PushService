using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushService
{
    public class PropertiesUtil
    {
        public static Hashtable Load(string file)
        {
            Hashtable ht = new Hashtable(16);
            string content = null;
            try
            {
                content = File.ReadAllText(file, System.Text.Encoding.UTF8);
            }
            catch (Exception)
            {
                return null;
            }
            string[] rows = content.Split('\n');
            string[] kv = null;
            foreach (string c in rows)
            {
                if (c.Trim().Length == 0)
                    continue;
                kv = c.Split('=');
                if (kv.Length == 1)
                {
                    ht[kv[0].Trim()] = "";
                }
                else if (kv.Length == 2)
                {
                    ht[kv[0].Trim()] = kv[1].Trim();
                }
            }
            return ht;
        }

        public static bool Save(string file, Hashtable ht)
        {
            if (ht == null || ht.Count == 0)
                return false;
            StringBuilder sb = new StringBuilder(ht.Count * 12);
            foreach (string k in ht.Keys)
            {
                sb.Append(k).Append('=').Append(ht[k]).Append(System.Environment.NewLine);
            }
            try
            {
                File.WriteAllText(file, sb.ToString(), System.Text.Encoding.UTF8);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
