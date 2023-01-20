using System;
using Frends.HIT.RoboSharp;
using Newtonsoft.Json;

class Tests {
    public static void Main() {
        TargetSettings sett = new TargetSettings();
        sett.Source = new PathSettings();
        sett.Destination = new PathSettings();
        sett.Source.Path = Environment.GetEnvironmentVariable("TestSourcePath");
        sett.Source.Username = Environment.GetEnvironmentVariable("TestSourceUsername");
        sett.Source.Password = Environment.GetEnvironmentVariable("TestSourcePassword");
        sett.Destination.Path = Environment.GetEnvironmentVariable("TestDestinationPath");
        sett.Destination.Username = Environment.GetEnvironmentVariable("TestDestinationUsername");
        sett.Destination.Password = Environment.GetEnvironmentVariable("TestDestinationPassword");

        SyncParameters param = SyncParameters.GetDefaultInstance();
        Console.WriteLine("Syncing folders...");
        Console.WriteLine(JsonConvert.SerializeObject(sett));
        Console.WriteLine(JsonConvert.SerializeObject(param));
        var exInfo = Frends.HIT.RoboSharp.Main.SyncFolders(sett, param);
        Console.WriteLine(exInfo.ToString());
    }
}