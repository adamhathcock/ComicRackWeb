using Nancy;

namespace ComicRackWebViewer
{
    public class PublishersModule : NancyModule
    {
        public PublishersModule()
            : base("/Publishers")
        {
            Get["/"] = x => View["PublishersList.cshtml", API.GetPublishers()];
            Get["/{publisher}"] = x => View["SeriesList.cshtml", API.GetSeries(x.publisher, string.Empty)];
            Get["/{publisher}/icon"] = x => API.GetIcon(x.publisher, Response);
            Get["/{publisher}/{imprint}"] = x => View["SeriesList.cshtml", API.GetSeries(x.publisher, x.imprint)];
        }
    }
}
