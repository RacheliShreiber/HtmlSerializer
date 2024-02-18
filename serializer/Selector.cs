using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace serializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }


        public static Selector CreateSelector(string str)
        {
            Selector root = new Selector();
            Selector current=new Selector();
            List<string> selectors = new List<string>();
            selectors = str.Split(' ').ToList();
            //string name, id,temp1;
            //List<string> classes = new List<string>();
            bool isFirst=false;
            foreach(string s in selectors)
            {
                Selector temp=new Selector();
                
                var attributes = new Regex("^(.*?)?(#.*?)?(\\..*?)?$").Matches(s);
                if (HtmlHelper.Instance.AllTags.Contains(attributes[0].Groups[1].Value))
                    temp.TagName = attributes[0].Groups[1].Value;
                if (attributes[0].Groups[2].Value.StartsWith("#"))
                    temp.Id = attributes[0].Groups[2].Value.Substring(1);
                if (attributes[0].Groups[3].Value.StartsWith("."))
                {
                    temp.Classes = new List<string>();
                    temp.Classes = attributes[0].Groups[3].Value.Split('.').ToList();
                    temp.Classes.Remove("");
                }
                if (!isFirst)
                {
                    isFirst = true;
                    root = temp;
                    current = temp;
                }
                else
                {
                    temp.Parent = current;
                    current.Child = temp;
                    current=temp;
                }
            }
            return root;
        }
    }
}