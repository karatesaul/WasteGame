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

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

namespace BrokenCycleGame.Particlesystem
{
    class MeleeWeapon : Weapons
    {
        WasteGame game;
        Player player;
        public Texture2D broom;
        SpriteBatch spriteBatch;
        Vector2 testpoint;
        public MeleeWeapon(World gameWorld, WasteGame game, Texture2D broomTex, ContentManager content, PlayerIndex playerNum, SpriteBatch spriteBatch, Player player)
            : base( game, content, playerNum,  spriteBatch)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.player = player;
            this.broom = broomTex;
        }

        public override void Update(Vector2 currposition, Vector2 Direction)
        {
            currposition = player.Position;
            testpoint = new Vector2(currposition.X + Direction.X * 48 / 10, currposition.Y + Direction.Y * 48 / 10);
            List<Tower> towerList = game.currLevel.towerList;
            List<Roomba> roombaList = game.currLevel.roombaList;
            foreach (Tower t in towerList)
            {
                if (t.towerFixture.TestPoint(ref testpoint))
                {
                    Console.WriteLine("damage");
                    t.health -= 10;
                }
            }
            foreach (Roomba r in roombaList)
            {
                if (r.roombaFixture.TestPoint(ref testpoint))
                {
                    Console.WriteLine(r.health);
                    r.health -= 15;
                    if (r.health <= 0 && !r.dead)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (this.player == game.players[i])
                            {
                                r.dead = true;
                                game.players[i].collectedScore += 500;
                            }
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch batch, camera cam)
        {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null,
                cam.get_transformation(game.GraphicsDevice));
            batch.Draw(broom, new Rectangle((int)(player.Position.X - 16), (int)(player.Position.Y - 16), broom.Width, broom.Height), Color.White);
            Texture2D pixel = new Texture2D(batch.GraphicsDevice, 10, 1);
            batch.Draw(pixel, new Rectangle((int)testpoint.X, (int)testpoint.Y, 10, 1),
            new Rectangle(0, 0, 1, 1),
            Color.Red,
            (float)Math.Atan2(testpoint.Y, testpoint.X),
            new Vector2(0, 0),
            SpriteEffects.None,
            1);

   
            batch.End();
        }
    }
}
