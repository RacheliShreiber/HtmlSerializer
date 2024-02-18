// See https://aka.ms/new-console-template for more information


using serializer;
using System.Text.RegularExpressions;
var html = await Load("https://learn.malkabruk.co.il/practicode/projects/pract-2/#_3");
var cleanHtml=new Regex("\\s+(?=<[^<>]+>)",RegexOptions.Singleline).Replace(html, "");
var finalCleanHtml = new Regex("\\s+", RegexOptions.Singleline).Replace(cleanHtml, " ");
var htmlLines=new Regex("<(.*?)>").Split(finalCleanHtml).Where(s=>s.Length>0);
Console.WriteLine();
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
HtmlElement CreateTree()
{   
    htmlLines = htmlLines.Skip(1);
    HtmlElement root=null;
    HtmlElement currentElement=root;
    foreach (var line in htmlLines)
    {
        string firstWord=line.Split(' ')[0];
        if (firstWord == "/html")
            break;
        if (root!=null&&firstWord!=""&&firstWord[0] == '/')
        {
            if (HtmlHelper.Instance.AllTags.Contains(firstWord.Substring(1)))
                currentElement = currentElement.Parent;
        }
        else if (HtmlHelper.Instance.AllTags.Contains(firstWord))
        {
            HtmlElement myElement = new HtmlElement();
            myElement.Name = firstWord;
            myElement.Parent = currentElement;
            var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
            foreach (Match attribute in attributes)
            {
                if (attribute.Groups[1].Value == "id")
                    myElement.Id = attribute.Groups[2].Value;
                else if (attribute.Groups[1].Value == "class")
                    myElement.Classes = new List<string>(attribute.Groups[2].Value.Split(' '));
                else
                    myElement.Attributes.Add(attribute.ToString());
            }
            if (root == null)
            {
                root = myElement;
                currentElement = root;
            }
            else
                currentElement.Children.Add(myElement);
            if (!HtmlHelper.Instance.SelfClosingTags.Contains(firstWord))
                currentElement = myElement;
        }
        else
        {
            currentElement.InnerHtml = currentElement.InnerHtml+ line;
        }      
    }
    return root;
}

void PrintTree(HtmlElement element, int depth)
{
    Console.WriteLine(new string(' ', depth * 2) + element.Name);

    foreach (var child in element.Children)
    {
        PrintTree(child, depth + 1);
    }
}
void PrintSelector(Selector s, int depth)
{
    Console.Write(new string(' ', depth * 2) + s.TagName);
    Console.Write(" "+s.Id);
    if (s.Classes != null)
    {
        foreach (var item in s.Classes)
        {
            Console.Write(" " + item);
        }
    }
    Console.WriteLine();
    if(s.Child!=null)
        PrintSelector(s.Child, depth + 1);
}
HtmlElement root = CreateTree();
PrintTree(root,0);
Console.WriteLine();
Console.WriteLine("----------selector----------");
Selector s = new Selector();
s = Selector.CreateSelector("li.md-tabs__item a");
PrintSelector(s, 0);
var result=root.Match(s);
result.ToList().ForEach(z => Console.WriteLine(z));
Console.WriteLine();