using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Viewer;
using Nancy;
using Nancy.OData;

namespace ComicRackWebViewer
{
    public static class API
    {
        public static List GetIssuesOfList(string name, NancyContext context)
        {
            var list = Program.Database.ComicLists.FirstOrDefault(x => x.Name == name);
            if (list == null)
            {
                return new List
                {
                    Comics = Enumerable.Empty<Comic>(),
                    Name = name
                };
            }
            return new List
            {
                Comics = context.ApplyODataUriFilter(list.GetBooks().Select(x => x.ToComic())).Cast<Comic>(),
                Name = name
            };
        }

        public static IEnumerable<Series> GetSeries()
        {
            return Plugin.Application.GetLibraryBooks().AsSeries();
        }

        public static List GetSeries(Guid id, NancyContext context)
        {
            var books = Plugin.Application.GetLibraryBooks();
            var book = books.Where(x => x.Id == id).First();
            var series = books.Where(x => x.Series == book.Series)
                .Where(x => x.Volume == book.Volume)
                .Select(x => x.ToComic())
                .OrderBy(x => x.Number).ToList();
            return new List
            {
                Comics = context.ApplyODataUriFilter(series).Cast<Comic>(),
                Name = book.Series
            };
        }

        public static Response GetThumbnailImage(Guid id, int page, IResponseFormatter response)
        {
            var bitmap = Image.FromStream(new MemoryStream(GetPageImageBytes(id, page)), false, false);
            double ratio = 200D / (double)bitmap.Height;
            int width = (int)(bitmap.Width * ratio);
            var callback = new Image.GetThumbnailImageAbort(() => true);
            var thumbnail = bitmap.GetThumbnailImage(width, 200, callback, IntPtr.Zero);
            MemoryStream stream = GetBytesFromImage(thumbnail);
            return response.FromStream(stream, MimeTypes.GetMimeType(".jpg"));
        }

        public static MemoryStream GetBytesFromImage(Image image)
        {
            var bitmap = new Bitmap(image);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);
            stream.Position = 0;
            return stream;
        }

        private static byte[] GetPageImageBytes(Guid id, int page)
        {
            var comic = GetComics().First(x => x.Id == id);
            var index = comic.TranslatePageToImageIndex(page);
            var provider = GetProvider(comic);
            if (provider == null)
            {
                return null;
            }
            return provider.GetByteImage(index);
        }

        public static Response GetPageImage(Guid id, int page, IResponseFormatter response)
        {
            var bytes = GetPageImageBytes(id, page);
            if (bytes == null)
            {
                return response.AsRedirect("/Comics/Images/spacer.png");
            }
            return response.FromStream(new MemoryStream(bytes), MimeTypes.GetMimeType(".jpg"));
        }

        private static ImageProvider GetProvider(ComicBook comic)
        {
            var provider = comic.CreateImageProvider();
            if (provider == null)
            {
                return null;
            }
            if (provider.Status != ImageProviderStatus.Completed)
            {
                provider.Open(false);
            }
            return provider;
        }

        public static Comic GetComic(Guid id)
        {
            var comic = GetComics().First(x => x.Id == id);
            return comic.ToComic();
        }

        public static IQueryable<ComicBook> GetComics()
        {
            return Plugin.Application.GetLibraryBooks().AsQueryable();
        }

        public static Response GetIcon(string key, IResponseFormatter response)
        {
            var image = ComicBook.PublisherIcons.GetImage(key);
            if (image == null)
            {
                return response.AsRedirect("/Comics/Images/spacer.png");
            }
            return response.FromStream(API.GetBytesFromImage(image), MimeTypes.GetMimeType(".jpg"));
        }

        public static IEnumerable<Publisher> GetPublishers()
        {
            return Plugin.Application.GetLibraryBooks().GroupBy(x => x.Publisher).Select(x =>
            {
                return x.GroupBy(y => y.Imprint).Select(y => new Publisher { Name = x.Key, Imprint = y.Key });
            }).SelectMany(x => x).OrderBy(x => x.Imprint).OrderBy(x => x.Name);
        }

        public static IEnumerable<Series> GetSeries(string publisher, string imprint)
        {
            IEnumerable<ComicBook> comics;
            if (string.Compare(publisher, "no publisher", true) == 0)
            {
                comics = Plugin.Application.GetLibraryBooks().Where(x => string.IsNullOrEmpty(x.Publisher));
            }
            else
            {
                comics = Plugin.Application.GetLibraryBooks().Where(x => string.Compare(publisher, x.Publisher, true) == 0);
                if (string.IsNullOrEmpty(imprint))
                {
                    comics = comics.Where(x => string.IsNullOrEmpty(x.Imprint));
                }
                comics = comics.Where(x => string.Compare(imprint, x.Imprint, true) == 0);
            }
            return comics.AsSeries();
        }
    }
}
