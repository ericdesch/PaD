﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PaD.Infrastructure
{
    //public class HttpPostedFileHelper
    //{
    //    public static HttpPostedFile ConstructHttpPostedFile(byte[] data, string filename, string contentType)
    //    {
    //        // Get the System.Web assembly reference
    //        Assembly systemWebAssembly = typeof(HttpPostedFileBase).Assembly;
    //        // Get the types of the two internal types we need
    //        Type typeHttpRawUploadedContent = systemWebAssembly.GetType("System.Web.HttpRawUploadedContent");
    //        Type typeHttpInputStream = systemWebAssembly.GetType("System.Web.HttpInputStream");

    //        // Prepare the signatures of the constructors we want.
    //        Type[] uploadedParams = { typeof(int), typeof(int) };
    //        Type[] streamParams = { typeHttpRawUploadedContent, typeof(int), typeof(int) };
    //        Type[] parameters = { typeof(string), typeof(string), typeHttpInputStream };

    //        // Create an HttpRawUploadedContent instance
    //        object uploadedContent = typeHttpRawUploadedContent
    //          .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, uploadedParams, null)
    //          .Invoke(new object[] { data.Length, data.Length });

    //        // Call the AddBytes method
    //        typeHttpRawUploadedContent
    //          .GetMethod("AddBytes", BindingFlags.NonPublic | BindingFlags.Instance)
    //          .Invoke(uploadedContent, new object[] { data, 0, data.Length });

    //        // This is necessary if you will be using the returned content (ie to Save)
    //        typeHttpRawUploadedContent
    //          .GetMethod("DoneAddingBytes", BindingFlags.NonPublic | BindingFlags.Instance)
    //          .Invoke(uploadedContent, null);

    //        // Create an HttpInputStream instance
    //        object stream = (Stream)typeHttpInputStream
    //          .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, streamParams, null)
    //          .Invoke(new object[] { uploadedContent, 0, data.Length });

    //        // Create an HttpPostedFile instance
    //        HttpPostedFile postedFile = (HttpPostedFile)typeof(HttpPostedFile)
    //          .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null)
    //          .Invoke(new object[] { filename, contentType, stream });

    //        return postedFile;
    //    }
    //}
}