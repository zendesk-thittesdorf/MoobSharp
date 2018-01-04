using System;
using MoobSharp.LOM;
using System.Collections.Generic;

namespace MoobSharp
{
    public static class Rac
    {
        public static ILom GetDracForHost(string Username, string Password, string Hostname, bool NoSSL)
        {
            // Try iDrac 7-8
            Console.WriteLine("Trying iDrac 7-8...");
            ILom iDrac7_8 = new Idrac7_8(Username, Password, Hostname, !NoSSL);
            if (iDrac7_8.Detect()) return iDrac7_8;

            // Try iDrac 6
            Console.WriteLine("Trying iDrac 6...");
            ILom iDrac6 = new Idrac6(Username, Password, Hostname, !NoSSL);
            if (iDrac6.Detect()) return iDrac6;
            // else
            return null;
        }
    }
}