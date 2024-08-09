using ForeverMuddled.Models;
using TotkCommon;

namespace ForeverMuddled.Helpers;

static class ActorCollector
{
    public static IEnumerable<Actor> CollectVanillaActors()
    {
    Check:
        if (!File.Exists(Totk.Config.ZsDicPath)) {
            Console.WriteLine("Please enter your TotK game path:");
            Totk.Config.GamePath = Console.ReadLine() ?? string.Empty;
            goto Check;
        }

        string actorsDirectory = Path.Combine(Totk.Config.GamePath, "Pack", "Actor");

        if (!Directory.Exists(actorsDirectory)) {
            throw new Exception(message: $"""
                The vanilla Actors directory '{actorsDirectory}' could not be found.
                """);
        }

        return Directory
            .EnumerateFiles(actorsDirectory,
                searchPattern: "*.pack.zs", SearchOption.TopDirectoryOnly)
            .Select(x => new Actor(x));
    }

    public static IEnumerable<Actor> CollectCustomActors()
    {
        string customActorsDirectory = "custom";

        if (!Directory.Exists(customActorsDirectory)) {
            return [];
        }

        return Directory
            .EnumerateFiles(customActorsDirectory,
                searchPattern: "*.pack*", SearchOption.TopDirectoryOnly)
            .Select(x => new Actor(x));
    }
}
