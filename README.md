TED
======
Tag EDitor for MP3 files. 

TED is a ID3 tag editor for bulk editing MP3 files. 
NOTE: TED is early in development and very much a work in progress. 

Built on:
---------
* [.Net Core](https://docs.microsoft.com/en-us/dotnet/core/)
* [Audio Tools Library (ATL) for .NET](https://github.com/Zeugma440/atldotnet)
* [Blazor Server](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Fluent Validation](https://docs.fluentvalidation.net/en/latest/aspnet.html)
* [MudBlazor](https://mudblazor.com/)

Core Features:
---------
* Batch delete of selected Releases
* Batch select of Releases to move into Inbound (or Library) Directory
* Checks Release Track count against SFV and M3U files
* Converts FLAC files to MP3 files
* Creates JSON metadata file for each Release (ted.data.json)
* Filtering including; all Releases with less than 3 tracks, all Releases less than 10 minutes
* Find all Releases (or Albums) recursively in a directory
* Remove unwanted text from Track Titles 
* Removes featuring artist from Track Title and puts as Track Artist
* Renumber Tracks
* Splits CUE managed FLAC files into MP3 Tracks
* Designed to work with [Roadie API server](https://raw.githubusercontent.com/sphildreth/roadie)

Installation
------------
* Clone the Repo, modify the appsettings.json to point to your folder where MP3 files are grouped by Directory and where you want processed Releases to be moved to, run TED (future plans include making a Windows desktop app).

License
-------
MIT