using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

namespace BrokenCycleGame.Tiles
{
    public class SludgeTile : Tile
    {
        public SludgeTile(World gameWorld, Vector2 location, WasteGame game, Vector2 offset, Level curLevel)
            : base(gameWorld, location, game, offset, curLevel)
        {
            health = 150;
            this.scorevalue = -100;
            prevhealth = health;
            maxhealth = health;

            this.game = game;
            contentName = "Images/Tiles/dirty water";
            tileTex = game.Content.Load<Texture2D>(contentName);
            location.X *= tileTex.Width;
            location.Y *= tileTex.Height;
            location += offset;
            Position = location;
            
        }

        public override void draw(SpriteBatch spriteBatch, camera cam)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null,
                cam.get_transformation(game.GraphicsDevice));
            spriteBatch.Draw(tileTex, new Rectangle((int)Position.X, (int)Position.Y, tileTex.Width, tileTex.Height), Color.White);
            spriteBatch.End();
            base.draw(spriteBatch, cam);
        }
    }
}
