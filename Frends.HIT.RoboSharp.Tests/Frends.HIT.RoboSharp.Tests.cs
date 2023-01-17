using Frends.HIT.RoboSharp;
using System;

namespace Frends.HIT.RoboSharp.Tests {
    public class Tests {
        public static void Main() {
            var source = new PathSettings() {
                Path = @"\\server\something",
                Username = "username",
                Password = "password"
            };

            Frends.HIT.RoboSharp.Main.SyncFolders(source, source, new SyncParameters() {
                Options = new RoboOption[] { RoboOption.MIR },
                Shell = RunWithShell.cmd
            });

            Console.WriteLine("Hello");
        }
    }
}