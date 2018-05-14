using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CitystarCms.Models
{
    public class TextHelper
    {
        private static string translate(string sourceText, string originalLetters, string latinLetters)
        {
            string textout = "";
            int pos;
            string word = "";
            string lword = "";

            if (!string.IsNullOrEmpty(sourceText) && !string.IsNullOrEmpty(originalLetters) && !string.IsNullOrEmpty(latinLetters))
            {
                textout = "";

                string[] wLatin = latinLetters.Split(",".ToCharArray());
                for (int i = 0; i < sourceText.Length; i++)
                {
                    pos = originalLetters.IndexOf(sourceText.ToLower().Substring(i, 1));
                    try
                    {
                        word = wLatin[pos];
                    }
                    catch { word = ""; }

                    if ((lword + word) == "--") { }
                    else if ((sourceText.Length - 1) == i & word == "-") { }
                    else
                    {
                        if (!string.IsNullOrEmpty(word))
                        {
                            textout = textout + word;
                        }
                    }
                    lword = word;
                }
            }
            return textout;
        }
        public static string Translit(string ptext)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(ptext))
                return result;
            //Россия
            string ruLetters = "абвгдеёжзийклмнопрстуфхцчшщыэюя ,-_0123456789abcdefghijklmnopqrstuvwxyz";
            string ruLatinLetters = "a,b,v,g,d,e,jo,zh,z,i,j,k,l,m,n,o,p,r,s,t,u,f,h,c,ch,sh,i,y,je,ju,ja,-,-,-,_,0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,','";
            result = translate(ptext, ruLetters, ruLatinLetters);

            return result;

        }
    }
}