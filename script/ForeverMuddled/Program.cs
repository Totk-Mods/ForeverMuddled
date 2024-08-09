using ForeverMuddled.Helpers;
using ForeverMuddled.Models;

string outputDirectory = "output";

IEnumerable<Actor> actors = ActorCollector.CollectVanillaActors();
actors = actors.Concat(ActorCollector.CollectCustomActors());

foreach (Actor actor in actors) {
    if (actor.Muddle(1.0f)) {
        actor.Save(outputDirectory);
    }
}
