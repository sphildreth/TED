TED
======
Tag EDitor for MP3 files. 

TED is a ID3 tag editor for bulk editing MP3 files. 
NOTE: TED is early in development and very much a work in progress. 

Core Features:
---------
* Batch select of Releases to move into Inbound (or Library) Directory
* Batch delete of selected Releases
* Checks Release Track count against SFV and M3U files
* Converts FLAC files to MP3 files
* Created JSON metadata file for each Release
* Find all Releases (or Albums) recursively in a directory
* Remove unwanted text from Track Titles 
* Removes featuring artist from Track Title and puts as Track Artist
* Renumber Tracks
* Filtering including; all Releases with less than 3 tracks, all Releases less than 10 minutes
* Designed to work with [Roadie API server](https://raw.githubusercontent.com/sphildreth/roadie)

Installation
------------
* Clone the Repo, modify the appsettings.json to point to your folder where MP3 files are grouped by Directory and where you want processed Releases to be moved to, run TED (future plans include making a Windows desktop app).

License
-------
MIT