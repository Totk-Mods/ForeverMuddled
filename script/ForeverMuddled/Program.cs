using ForeverMuddled.Helpers;
using ForeverMuddled.Models;

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

foreach (Actor actor in actors) {
    if (actor.Muddle(muddleFactor, mode)) {
        actor.Save(outputDirectory);
    }
}
