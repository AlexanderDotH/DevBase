using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using DevBase.Enums;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.RequestData.Types;
using DevBase.Api.Apis.Deezer;
using DevBase.Generic;
using Newtonsoft.Json;

namespace DevBaseLive
{
    class Program
    {
        static void Main(string[] args)
        {

        }

        static long TestOne()
        {
            string[] array = new[] { "A", "B", "C", "D" };
            
            Stopwatch sw = new Stopwatch();
            sw.Start();

            array = CopyToAdd(array, "E");
            
            sw.Stop();
            return sw.ElapsedTicks;
        }
        
        static long TestTwo()
        {
            string[] array = new[] { "A", "B", "C", "D" };
            
            Stopwatch sw = new Stopwatch();
            sw.Start();

            array = ResizeAdd(array, "E");
            
            sw.Stop();
            return sw.ElapsedTicks;
        }

        static string[] CopyToAdd(string[] array, string value)
        {

            string[] newArray = new string[array.Length + 1];
            array.CopyTo(newArray, 0);
            newArray[array.Length] = value;

            return newArray;
        }
        
        static string[] ResizeAdd(string[] array, string value)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = value;
            return array;
        }
    }
}