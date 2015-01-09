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
    public class TarTile : Tile
    {
        public TarTile(World gameWorld, Vector2 location, WasteGame game, Vector2 offset, Level curLevel)
            : base(gameWorld, location, game, offset, curLevel)
        {
            health = 150;
            maxhealth = health;
            prevhealth = health;

            level = curLevel;
            Neighbors = new Tile[8];

            this.game = game;
            contentName = "Images/Tiles/tar1";
            tileTex = game.Content.Load<Texture2D>(contentName);
            location.X *= tileTex.Width;
            location.Y *= tileTex.Height;

            Position = location;
            tileFixture = FixtureFactory.AttachRectangle(tileTex.Width, tileTex.Height, 1, new Vector2(), this);
            tileFixture.Body.BodyType = BodyType.Static;
            tileFixture.CollisionCategories = Category.Cat6;
            tileFixture.CollidesWith = Category.Cat1 | Category.Cat3;
            tileFixture.OnCollision += _OnCollision;

        }
        public Boolean _OnCollision(Fixture fix1, Fixture fix2, Contact con)
        {
            switch (fix2.CollisionCategories)
            {
                case Category.Cat1:
                    return false;
                case Category.Cat3:
                    return false;

                default: return false;
            }
        }

        public override void checkNeighbor()
        {
            int i = 0;
            int outx = level.tiles.GetLength(0);
            int outy = level.tiles.GetLength(1);
            int x, y;
            int posX = (int)Position.X / tileTex.Width;
            int posY = (int)Position.Y / tileTex.Height;

            //Use x and y as loop control, going from -1 to 1 (to be added to position)
            for (y = -1; y <= 1; y++)
            {
                for (x = -1; x <= 1; x++)
                {
                    //if neither posX+x or posY+y are out of bounds, continue assignment
                    if (((posX + x >= 0) && (posX + x < outx))
                         && ((posY + y >= 0) && (posY + y < outy)))
                    {
                        //Make sure x and y are not the current tile, then set
                        if (!((x == 0) && (y == 0)))
                        {
                            Neighbors[i] = level.tiles[posX + x, posY + y];
                        }
                        else
                        {
                            i--;
                        }
                    }
                    i++;
                }
            }
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
