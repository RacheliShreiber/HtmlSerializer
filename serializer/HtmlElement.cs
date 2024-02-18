using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serializer
{
    public class HtmlElement
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }=new List<string>();
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

        public IEnumerable<HtmlElement> Descendants(HtmlElement root)
        {
            HtmlElement current;
            Queue<HtmlElement> elements = new Queue<HtmlElement>();
            elements.Enqueue(root);
            while(elements.Count > 0)
            {
                current=elements.Dequeue();
                yield return current;
                foreach(HtmlElement child in current.Children)
                    elements.Enqueue(child);
            }
        }
        public IEnumerable<HtmlElement> Ancestors(HtmlElement root)
        {
            HtmlElement current = root;
            while (current.Parent != null)
            {
                yield return current.Parent;
                current = current.Parent;
            }
        }
        public IEnumerable<HtmlElement> Match(Selector selector)
        {
            List<HtmlElement> result = new List<HtmlElement>();
            SearchElements(this, selector, result);
            var resultHash = new HashSet<HtmlElement>(result);
            return resultHash;
        }
        private void SearchElements(HtmlElement element,Selector selector,List<HtmlElement> list)
        {

            if (selector == null)
                return;
            IEnumerable<HtmlElement> descendants = Descendants(element);
            foreach (HtmlElement descendant in descendants)
            {
                if (IsSame(descendant, selector))
                {
                    if (selector.Child == null)
                    {
                        list.Add(descendant);
                    }
                    else
                        SearchElements(descendant, selector.Child, list);
                }
            }
        }
        private bool IsSame(HtmlElement element,Selector selector1)
        {
            if (element.Id != selector1.Id || element.Name != selector1.TagName)
                return false;
            if (selector1.Classes != null)
            {
                foreach (string item in selector1.Classes)
                {
                    if (element.Classes == null)
                        return false;
                    if (!element.Classes.Contains(item))
                        return false;
                }
            }
            return true;
        }
        public override string ToString()
        {
            string s=$"{this.Name} id={this.Id} class= ";
            if (this.Classes != null)
            {
                foreach (var item in this.Classes)
                {
                    s += item;
                }
            }
            return s;
        }
    }
}
