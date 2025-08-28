using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Minigames;

namespace ShootyMcTooty
{
    internal class ShootingGallery : IMinigame
    {
        public void changeScreenSize()
        {
        }

        public bool doMainGameUpdates()
        {
            return false;
        }

        public void draw(SpriteBatch b)
        {
            throw new NotImplementedException();
        }

        public bool forceQuit()
        {
            throw new NotImplementedException();
        }

        public void leftClickHeld(int x, int y)
        {
            
        }

        public string minigameId()
        {
            return "Shooty McTooty";
        }

        public bool overrideFreeMouseMovement()
        {
            return true;
        }

        public void receiveEventPoke(int data)
        {
        }

        public void receiveKeyPress(Keys k)
        {
            throw new NotImplementedException();
        }

        public void receiveKeyRelease(Keys k)
        {
            throw new NotImplementedException();
        }

        public void receiveLeftClick(int x, int y, bool playSound = true)
        {
            throw new NotImplementedException();
        }

        public void receiveRightClick(int x, int y, bool playSound = true)
        {
            throw new NotImplementedException();
        }

        public void releaseLeftClick(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void releaseRightClick(int x, int y)
        {
            throw new NotImplementedException();
        }

        public bool tick(GameTime time)
        {
            throw new NotImplementedException();
        }

        public void unload()
        {
            throw new NotImplementedException();
        }
    }
}
