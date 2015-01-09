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

namespace BrokenCycleGame.Particlesystem
{
    class RangedWeapon : Weapons
    {
        Player player;
        public ParticleEngine particleEngine;
        SpriteBatch spriteBatch;

         public RangedWeapon(WasteGame game, ContentManager content, PlayerIndex playernum, SpriteBatch spritebatch, Player player) : base(game, content, playernum, spritebatch)
        {
            this.player = player;
            spriteBatch = spritebatch;
            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(content.Load<Texture2D>("images/Particle"));
            particleEngine = new ParticleEngine(game, textures, new Vector2(400, 240), player);
            this.game = game;
        }

        public override void Update(Vector2 currposition, Vector2 Direction)
        {
            particleEngine.EmitterLocation = currposition;
            particleEngine.Update();

            //Console.WriteLine(Direction.ToString());
            Vector2 testpoint4 = new Vector2(currposition.X + Direction.X * 16 * 4 / 5, currposition.Y + Direction.Y * 16 * 4 / 5);
            Vector2 testpoint3 = new Vector2(currposition.X + Direction.X * 16 * 3 / 5, currposition.Y + Direction.Y * 16 * 3 / 5);
            Vector2 testpoint2 = new Vector2(currposition.X + Direction.X * 16 * 2 / 5, currposition.Y + Direction.Y * 16 * 2 / 5);
            Vector2 testpoint1 = new Vector2(currposition.X + Direction.X * 16 * 1 / 5, currposition.Y + Direction.Y * 16 * 1 / 5);
            List<Tower> towerList = game.currLevel.towerList;
            List<Roomba> roombaList = game.currLevel.roombaList;
            foreach (Tower t in towerList)
            {
                if (t.towerFixture.TestPoint(ref testpoint1) ||
                    t.towerFixture.TestPoint(ref testpoint2) ||
                    t.towerFixture.TestPoint(ref testpoint3) ||
                    t.towerFixture.TestPoint(ref testpoint4))
                {
                    Console.WriteLine("damage");
                    t.health -= 1;
                }
            }
            foreach (Roomba r in roombaList)
            {
                if (r.roombaFixture.TestPoint(ref testpoint1) ||
                    r.roombaFixture.TestPoint(ref testpoint2) ||
                    r.roombaFixture.TestPoint(ref testpoint3) ||
                    r.roombaFixture.TestPoint(ref testpoint4))
                {
                    Console.WriteLine(r.health);
                    r.health -= 1;
                    if (r.health <= 0 && !r.dead)
                    {
                        for (int i = 0; i < 4; i++)
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
            particleEngine.Draw(batch, cam);
        }
    }
}
