using System.Linq;
using cYo.Projects.ComicRack.Viewer;
using Nancy;

namespace ComicRackWebViewer
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
            : base("/")
        {
            Get["/"] = x => View["Index.cshtml", Program.Database.ComicLists.Select(c => c.Name).OrderBy(c => c)];
        }
    }
}
