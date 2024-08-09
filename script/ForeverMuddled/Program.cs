using ForeverMuddled.Helpers;
using ForeverMuddled.Models;
using System.Diagnostics;

Console.WriteLine("Muddling enemies...");

string outputDirectory = "output";

float muddleFactor = float.PositiveInfinity;
MuddleMode mode = MuddleMode.Set;

if (args.Length > 0) {
    muddleFactor = InputHelper.GetMuddleFactor(args[0], out mode);
}

Console.WriteLine($"Muddling with a factor of {mode switch {
    MuddleMode.Add => "+",
    MuddleMode.Multiply => "*",
    _ => ""
}}{muddleFactor}");

IEnumerable<Actor> actors = ActorCollector.CollectVanillaActors();
actors = actors.Concat(ActorCollector.CollectCustomActors());

Stopwatch stopwatch = Stopwatch.StartNew();
int count = 0;

foreach (Actor actor in actors) {
    if (actor.Muddle(muddleFactor, mode)) {
        actor.Save(outputDirectory);
        count++;
    }
}

stopwatch.Stop();
Console.WriteLine($"Muddled {count} actors in {stopwatch.ElapsedMilliseconds / 100.0} seconds!");