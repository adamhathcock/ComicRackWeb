using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;

namespace ComicRackWebViewer
{
    public class SeriesModule : NancyModule
    {
        public SeriesModule()
            : base("/Series")
        {
            Get["/"] = parameters => View["SeriesList.cshtml", API.GetSeries()];
            Get["/{id}"] = parameters =>
            {
                var id = new Guid(parameters.id);
                return View["IssuesList.cshtml", API.GetSeries(id, Context)];
            };
        }
    }
}
