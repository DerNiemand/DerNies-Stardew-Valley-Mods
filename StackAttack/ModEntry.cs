using System;
using Microsoft.Xna.Framework;
using StackAttack;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


namespace StackAttack
{
    internal sealed class ModEntry : Mod
    {

        public static IModHelper modHelper;
        public override void Entry(IModHelper helper)
        {
            modHelper = helper;
            modHelper.Events.Input.ButtonPressed += this.OnButtonPressed;

            modHelper.ConsoleCommands.Add("stacker", "start the Stacker", new Action<string, string[]>(this.StartStacker));

            modHelper.Events.Display.WindowResized += OnScreenSizeChanged;
        }

        private void OnScreenSizeChanged(object? sender, WindowResizedEventArgs e)
        {
            Monitor.Log("ScreensizeChanged");
        }

        private void StartStacker(string cmd, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            bool success = Stacker.Start();

            if (!success)
            {
                Monitor.Log($"{Game1.player.Name} tried to start the Stacker but it failed, oh no.", LogLevel.Error);
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (!SButtonExtensions.IsActionButton(e.Button))
            {
                return;
            }

            if(Game1.currentLocation.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", "Buildings") == "StackAttack")
            {
                StartStacker(new string(""), new string[0]);
            }
        }
    }
}
