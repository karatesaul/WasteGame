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
    class TowerWeapon : Weapons
    {
        Tower tower;
        ParticleEngine particleEngine;
        SpriteBatch spriteBatch;

        public TowerWeapon(WasteGame game, ContentManager content, SpriteBatch spritebatch, Tower tower)
            : base(game, content, spritebatch)
        {
            this.tower = tower;
            spriteBatch = spritebatch;
            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(content.Load<Texture2D>("images/Particle"));
            particleEngine = new ParticleEngine(game, textures, new Vector2(400, 240), tower);
            this.game = game;
        }

        public override void Update(Vector2 currposition, Vector2 Direction)
        {
            particleEngine.EmitterLocation = currposition;
            particleEngine.Update();

            Vector2 testpoint = new Vector2(currposition.X + Direction.X * 48 / 5, currposition.Y + Direction.Y * 48 / 5);
            Player[] playerList = game.currLevel.playerList;
            foreach (Player p in playerList)
            {
                if (p.playerFixture.TestPoint(ref testpoint))
                {
                    p.health -= 1;
                }
            }
            
        }
        public override void Draw(SpriteBatch batch, camera cam)
        {
            particleEngine.Draw(batch, cam);
        }
    }
}

