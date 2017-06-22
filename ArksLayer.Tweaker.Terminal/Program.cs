using ArksLayer.Tweaker.Abstractions;
using ArksLayer.Tweaker.UpdateEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Terminal
{
    public class Program
    {
        public static async Task MainAsync()
        {
            // Use IOC Container in the UI project to deal with dependencies.
            var output = new ConsoleTrigger();
            var settings = new JsonTweakerSettings(JsonTweakerSettings.DefaultTweakerJsonFilePath);
            var updater = new UpdateManager(settings, output);

            Console.WriteLine("pso_bin is located at : " + settings.GameDirectory);

            var version = (settings.GameVersion ?? "version.ver not found!");
            Console.WriteLine("Game version: " + version);
            Console.WriteLine("Game is up to date? " + await updater.IsGameUpToDate());
            await updater.Update(false, true);
        }

        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
    }
}
