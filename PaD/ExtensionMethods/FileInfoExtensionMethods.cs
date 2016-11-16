using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Win32;

// Use the same namespace as FileInfo so it will be included when that namespace is included.
namespace System.IO
{
    public static class FileInfoExtensionMethods
    {

        /// <summary>
        /// Returns the content type for the passed FileInfo by looking up the content type
        /// for the file's extension in the registry.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetContentTypeFromExtension(this FileInfo fileInfo)
        {
            // Default to "application/unknown"
            string mimeType = "application/unknown";
            string ext = fileInfo.Extension;

            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
            {
                mimeType = regKey.GetValue("Content Type").ToString();
            }

            return mimeType;
        }
    }
}