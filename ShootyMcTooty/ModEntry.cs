using System;
using StardewModdingAPI;

namespace ShootyMcTooty
{
    internal sealed class ModEntry: Mod
    {
        public static IModHelper modHelper;
        public override void Entry(IModHelper helper)
        {
            modHelper = helper;
        }
    }
}
