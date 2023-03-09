﻿using System.Text.RegularExpressions;
using TED.Enums;

namespace TED.Utility
{
    public static class ImageHelper
    {
        public static string[] ImageExtensions()
        { 
            return new string[7] { "*.bmp", "*.jpeg", "*.jpe", "*.jpg", "*.png", "*.gif", "*.webp" }; 
        }

        public static string[] GetFiles(string path,
                                        string[] patterns = null,
                                        SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(path))
            {
                return new string[0];
            }
            if (patterns == null || patterns.Length == 0)
            {
                return Directory.GetFiles(path, "*", options);
            }
            if (patterns.Length == 1)
            {
                return Directory.GetFiles(path, patterns[0], options);
            }
            return patterns.SelectMany(pattern => Directory.GetFiles(path, pattern, options)).Distinct().ToArray();
        }

        public static string[] ImageFilesInDirectory(string directory, SearchOption searchOption)
        { 
            return GetFiles(directory, ImageExtensions(), searchOption); 
        }

        public static bool IsArtistImage(FileInfo fileinfo)
        {
            if (fileinfo == null)
            {
                return false;
            }

            return Regex.IsMatch(fileinfo.Name,
                                @"(band|artist|group|photo)(.*[0-9]*.*)*\.(jpg|jpeg|png|bmp|gif)",
                                RegexOptions.IgnoreCase);
        }

        public static bool IsArtistSecondaryImage(FileInfo fileinfo)
        {
            if (fileinfo == null)
            {
                return false;
            }

            return Regex.IsMatch(fileinfo.Name,
                                @"(artist_logo|logo|photo[-_\s]*[0-9]+|(artist[\s_-]+[0-9]+)|(band[\s_-]+[0-9]+))\.(jpg|jpeg|png|bmp|gif)",
                                RegexOptions.IgnoreCase);
        }


        public static bool IsReleaseImage(FileInfo fileinfo, string releaseName = null)
        {
            if (fileinfo == null)
            {
                return false;
            }

            return Regex.IsMatch(fileinfo.Name,
                                @"((f[-_\s]*[0-9]*)|00|art|big[art]*|cover|cvr|folder|release|front[-_\s]*)\.(jpg|jpeg|png|bmp|gif)",
                                RegexOptions.IgnoreCase);
        }

        public static bool IsReleaseSecondaryImage(FileInfo fileinfo)
        {
            if (fileinfo == null)
            {
                return false;
            }

            return Regex.IsMatch(fileinfo.Name,
                                @"((img[\s-_]*[0-9]*[\s-_]*[0-9]*)|(book[let]*[#-_\s(]*[0-9]*-*[0-9]*(\))*)|(encartes[-_\s]*[(]*[0-9]*[)]*)|sc[an]*(.)?[0-9]*|matrix(.)?[0-9]*|(cover[\s_-]*[0-9]+)|back|dvd|traycard|jewel case|disc|(.*)[in]*side(.*)|in([side|lay|let|site])*[0-9]*|digipack.?\[?\(?([0-9]*)\]?\)?|cd.?\[?\(?([0-9]*)\]?\)?|(release[\s_-]+[0-9]+))\.(jpg|jpeg|png|bmp|gif)",
                                RegexOptions.IgnoreCase);
        }


        public static IEnumerable<FileInfo> FindImageTypeInDirectory(DirectoryInfo directory,
                                                                     ImageType type,
                                                                     SearchOption directorySearchOptions = SearchOption.AllDirectories)
        {
            var result = new List<FileInfo>();
            if (directory?.Exists != true)
            {
                return result;
            }
            var imageFilesInDirectory = ImageFilesInDirectory(directory.FullName, directorySearchOptions);
            if (imageFilesInDirectory?.Any() != true)
            {
                return result;
            }
            foreach (var imageFile in imageFilesInDirectory)
            {
                var image = new FileInfo(imageFile);
                switch (type)
                {
                    case ImageType.Artist:
                        if (IsArtistImage(image))
                        {
                            result.Add(image);
                        }
                        break;

                    case ImageType.ArtistSecondary:
                        if (IsArtistSecondaryImage(image))
                        {
                            result.Add(image);
                        }
                        break;

                    case ImageType.Release:
                        if (IsReleaseImage(image))
                        {
                            result.Add(image);
                        }
                        break;

                    case ImageType.ReleaseSecondary:
                        if (IsReleaseSecondaryImage(image))
                        {
                            result.Add(image);
                        }
                        break; 
                }
            }
            return result.OrderBy(x => x.Name);
        }

    }
}
