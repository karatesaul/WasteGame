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


namespace BrokenCycleGame
{
    public class Wall: Tile
    {
        public Wall(World gameWorld, Vector2 location, WasteGame game, Vector2 offset, Level curlevel)
            : base(gameWorld, location, game, offset, curlevel)
        {
            health = 100;
            prevhealth = health;
            maxhealth = health;
            scorevalue = -10000;
            //A* set
            level = curlevel;
            Neighbors = new Tile[8];

            this.game = game;
            contentName = game.random.NextDouble() > 0.5 ? "Images/Tiles/red_container" : "Images/Tiles/blue_container";
            tileTex = game.Content.Load<Texture2D>(contentName);
            location.X *= tileTex.Width;
            location.Y *= tileTex.Height;
            //location += offset;
            Position = location;
            tileFixture = FixtureFactory.AttachRectangle(tileTex.Width, tileTex.Height, 1, new Vector2(), this);
            tileFixture.Body.BodyType = BodyType.Static;
            tileFixture.CollisionCategories = Category.Cat5;
            tileFixture.CollidesWith = Category.Cat1 | Category.Cat3;
            tileFixture.OnCollision += _OnCollision;
        }

        public override bool _OnCollision(Fixture fix1, Fixture fix2, Contact con)
        {
            switch (fix2.CollisionCategories)
            { 
                case Category.Cat1:
                    return true;
                case Category.Cat3:
                    return true;
                default: 
                    return false;

            }
            
        }
        public override void draw(SpriteBatch spriteBatch, camera cam)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null,
                cam.get_transformation(game.GraphicsDevice));
            if (health > 0)
                spriteBatch.Draw(tileTex, Position - new Vector2(tileTex.Width / 32, tileTex.Height / 32), Color.White);
            spriteBatch.End();
            base.draw(spriteBatch, cam);
        }
    }
}
