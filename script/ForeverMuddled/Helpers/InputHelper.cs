using ForeverMuddled.Models;
using System.Globalization;

namespace ForeverMuddled.Helpers;

static class InputHelper
{
    public static float GetMuddleFactor(string arg, out MuddleMode mode)
    {
        if (string.IsNullOrEmpty(arg)) {
            mode = MuddleMode.Set;
            return float.PositiveInfinity;
        }

        if (arg is ".inf" or "∞") {
            arg = float.PositiveInfinity.ToString();
        }

        mode = arg[0] switch {
            '+' => MuddleMode.Add,
            '*' or 'x' => MuddleMode.Multiply,
            _ => MuddleMode.Set,
        };

        return float.Parse(mode switch {
            MuddleMode.Multiply or MuddleMode.Add => arg[1..],
            _ => arg
        }, CultureInfo.InvariantCulture);
    }
}
