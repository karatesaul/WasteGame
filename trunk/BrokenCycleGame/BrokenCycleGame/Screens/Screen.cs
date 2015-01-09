using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//gamestate tutorial at  

namespace BrokenCycleGame
{
    public class Screen
    {
        
        protected EventHandler screenEvent;  

        public Screen(EventHandler scrEvent)
        {
            screenEvent = scrEvent;  
        }

        public virtual void Update(GameTime time)
        {
        }

        public virtual void Draw(SpriteBatch batch)
        {
        }
    }
}
