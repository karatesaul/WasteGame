using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrokenCycleGame
{
    public class ParticleEngine
    {
        WasteGame game;
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        public List<Particle> particles;
        private List<Texture2D> textures;
        Player player;

        public ParticleEngine(WasteGame game, List<Texture2D> textures, Vector2 location, Tower tower)
        {
            this.game = game;
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();

        }
        public ParticleEngine(WasteGame game, List<Texture2D> textures, Vector2 location, Player player)
        {
            this.game = game;
            this.player = player;
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
        }

        public void KillParticles(List<Particle> particles)
        {
            foreach (Particle p in particles)
            {
                p.TTL = 0;
            }
        }
        public void Update()
        {
            int total = 10;

            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle());
            }

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        private Particle GenerateNewParticle()
        
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = player.Direction;
            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            Color color = new Color(
                        0,
                        0,
                        (float)random.NextDouble());
            float size = 2 * (float)random.NextDouble();
            int ttl = random.Next(20);
            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        public void Draw(SpriteBatch spriteBatch, camera cam)
        {
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch, cam, game);
            }
        }
    }
}
