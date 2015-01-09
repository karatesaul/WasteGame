using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using System;

namespace BrokenCycleGame
{

    public class camera
    {
        public Vector2 pos;
        public Matrix transform;
        //A player to center the camera on
        public Player subject;
        public WasteGame game;

        public camera(Player sbj, WasteGame game)
        {
            subject = sbj;
            pos = subject.Position;
            this.game = game;
            //if the camera tries to draw out of bounds
            if (pos.X - game.GraphicsDevice.Viewport.Width / 4 < 0)
                pos.X = game.GraphicsDevice.Viewport.Width / 4;
            if (pos.Y - game.GraphicsDevice.Viewport.Height / 4 < 0)
                pos.Y = game.GraphicsDevice.Viewport.Height / 4;
            if (pos.X + game.GraphicsDevice.Viewport.Width / 4 > game.currLevel.Width * 32 - subject.playerSprite.Width - 64/*game.GraphicsDevice.Viewport.Width - 32*/)
                pos.X = MathHelper.Clamp(pos.X, 0, game.currLevel.Width * 32 - subject.playerSprite.Width - game.GraphicsDevice.Viewport.Width / 4);
            if (pos.Y + game.GraphicsDevice.Viewport.Height / 4 > game.currLevel.Height * 32 - 24 /*game.Window.ClientBounds.Height - 24*/)
                pos.Y = MathHelper.Clamp(pos.Y, 0, game.currLevel.Height * 32 - game.GraphicsDevice.Viewport.Height / 4);

        }

        public void update()
        {
            pos = subject.Position;
            //if the camera tries to draw out of bounds
            if (pos.X - game.GraphicsDevice.Viewport.Width / 4 < 0)
                pos.X = game.GraphicsDevice.Viewport.Width / 4;
            if (pos.Y - game.GraphicsDevice.Viewport.Height / 4 < 0)
                pos.Y = game.GraphicsDevice.Viewport.Height / 4;
            if (pos.X + game.GraphicsDevice.Viewport.Width / 4 > game.currLevel.Width * 32 - subject.playerSprite.Width - 64/*game.GraphicsDevice.Viewport.Width - 32*/)
                pos.X = MathHelper.Clamp(pos.X, 0, game.currLevel.Width * 32 - subject.playerSprite.Width - game.GraphicsDevice.Viewport.Width / 4);
            if (pos.Y + game.GraphicsDevice.Viewport.Height / 4 > game.currLevel.Height * 32 - 24 /*game.Window.ClientBounds.Height - 24*/)
                pos.Y = MathHelper.Clamp(pos.Y, 0, game.currLevel.Height * 32 - game.GraphicsDevice.Viewport.Height / 4);

        }

        public void Move(Vector2 amount)
        {
            pos += amount;
        }

        public Matrix get_transformation(GraphicsDevice device)
        {
            transform = Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                        Matrix.CreateTranslation(new Vector3(device.Viewport.Width * 0.5f, device.Viewport.Height * 0.5f, 0));
            return transform;
        }
    }
}
