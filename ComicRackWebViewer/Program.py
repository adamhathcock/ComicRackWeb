import clr

clr.AddReferenceByPartialName("IronPython")
clr.AddReferenceByPartialName("Microsoft.Scripting")
clr.AddReferenceByPartialName("ComicRackWebViewer")

from ComicRackWebViewer import Plugin

#@Name	ComicRack Web
#@Key	ComicRackWebViewer
#@Hook	Books, Editor
#@Image nancy.jpg
#@Description ComicRack Web
def ComicRackWebViewer(books):
    Plugin.Run(ComicRack.App)

#@Name ComicRack Web (Startup)
#@Hook Startup
#@Enabled false
#@Image nancy.jpg
#@Description ComicRack Web (Startup)
def ComicRackWebViewerStartup():
    Plugin.RunAtStartup(ComicRack.App)