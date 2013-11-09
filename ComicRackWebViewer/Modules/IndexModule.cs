using Nancy;

namespace ComicRackWebViewer
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
            : base("/")
        {
            Get["/"] = x => View["Index.cshtml", API.GetAllLists()];
        }
    }
}
