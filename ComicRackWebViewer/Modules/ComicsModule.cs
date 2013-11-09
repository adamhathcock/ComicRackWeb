using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using Nancy;

namespace ComicRackWebViewer
{
    public class ComicsModule : NancyModule
    {
        public ComicsModule()
            : base("/Comics")
        {
            Get["/{id*}"] = p =>
                {
                    string id = p.id;

                    while (true)
                    {
                        if (id.StartsWith("css/", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return Response.AsCss(id.Substring(4));
                        }
                        if (id.StartsWith("js/", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return Response.AsJs(id.Substring(3));
                        }
                        if (id.StartsWith("images/", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return Response.AsImage(id.Substring(7));
                        }
                        if (id.IndexOf('/') > -1)
                        {
                            id = id.Substring(id.IndexOf('/') + 1);
                        }
                        else
                        {
                            return View["ComicBook.cshtml", API.GetComic(new Guid(p.id))];
                        }
                    }
                };
            Get["/Metadata/{id}"] = x => View["ComicInfoPanel.cshtml", API.GetComic(new Guid(x.id))];
            Get["/Thumbnail/{id}/{page}"] = parameters => API.GetThumbnailImage(new Guid(parameters.id), int.Parse(parameters.page), Response);
            Get["/Image/{id}/{page}"] = parameters => API.GetPageImage(new Guid(parameters.id), int.Parse(parameters.page), Response);
        }

       
    }
}
