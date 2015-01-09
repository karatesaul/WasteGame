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

namespace BrokenCycleGame
{
    public abstract class Weapons
    {
        ParticleEngine particleEngine;
        SpriteBatch spriteBatch;
        Vector2 location;
        protected WasteGame game;

        public Weapons(WasteGame game, ContentManager content, SpriteBatch spriteBatch)
        {
        }
        public Weapons(WasteGame game, ContentManager content, PlayerIndex playernum, SpriteBatch spritebatch)
        {
        }

        public abstract void Update(Vector2 currposition, Vector2 Direction);
        public abstract void Draw(SpriteBatch batch, camera cam);
    }
}
