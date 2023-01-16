using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RoboSharp;

namespace Frends.HIT.RoboSharp
{
    public class General
    {
        public static string TestSync(string A, string B, string C)
        {
            RoboCommand backup = new RoboCommand();
            backup.CopyOptions.Source = @"\\HMGMT04.intern.hoglandet.se\d$\LarSch\src";
            backup.CopyOptions.Destination = @"\\HMGMT04.intern.hoglandet.se\d$\LarSch\dst";
            backup.CopyOptions.CopySubdirectories = true;
            backup.CopyOptions.CopySubdirectoriesIncludingEmpty = true;
            backup.CopyOptions.UseUnbufferedIo = true;

            backup.Start();

            // var robosharp = new RoboSharp.RoboSharp(Server, User, Pass);
            // var result = robosharp.Sync();
            // return result;
            return "Yes";
        }
        
    }
}

