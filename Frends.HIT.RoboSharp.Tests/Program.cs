using System;
using Frends.HIT.RoboSharp;

class Tests {
    public static void Main() {
        var sett = new TargetSettings(){
            Source = new PathSettings(){
                Path = "a",
                Username = "b",
                Password = "c"
            },
            Destination = new PathSettings(){
                Path = "d",
                Username = "e",
                Password = "f"
            }
        };
        var param = new SyncParameters();

        var exInfo = Frends.HIT.RoboSharp.Main.SyncFolders(sett, param);
        Console.WriteLine(exInfo.ToString());
    }
}