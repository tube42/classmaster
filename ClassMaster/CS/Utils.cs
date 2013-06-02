using System;
using System.Text;
using System.Reflection;


namespace ClassMaster
{
    public static class Utils
    {
        private static readonly String HEX_STRING = "0123456789ABCDEF";

        public static String GetCompleteName(Type t)
        {
            String[] tmp = t.FullName.Split(' ', '.');
            String name = tmp[tmp.Length - 1];

            // name = name.Replace('+', '.'); // nested classes?            
            return name;
        }

        public static String UrlEncode(String text)
        {
            StringBuilder sb = new StringBuilder();
            int len = text.Length;
            for (int i = 0; i < len; i++) {
                char c = text[i];
                if( Char.IsLetterOrDigit(c)) 
                    sb.Append(c);
                else {
                    sb.Append('%');
                    sb.Append(HEX_STRING[(c >> 4) & 0xF]);
                    sb.Append(HEX_STRING[(c >> 0) & 0xF]);
                }                    
            }
            return sb.ToString();
        }

    }
}
