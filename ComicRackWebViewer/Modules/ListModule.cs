using System.Collections.Generic;
using Nancy;

namespace ComicRackWebViewer
{
    public class ListModule : NancyModule
    {
        public ListModule()
            : base("/Lists")
        {
            Get["/{name}"] = x => View["IssuesList.cshtml", API.GetIssuesOfList(x.name, Context)];
            Get["/{name}/Series"] = x => View["IssuesList.cshtml", API.GetIssuesOfList(x.name, Context).AsSeries()];
        }
    }

    public class List
    {
        public string Name { get; set; }
        public IEnumerable<Comic> Comics { get; set; }
        public bool IsDesc { get; set; }
    }
}
