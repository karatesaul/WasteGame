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

namespace BrokenCycleGame.Screens
{
    public class CreditsScreen : Screen
    {
        Texture2D background;
        bool PressedB = false;
        GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
        public CreditsScreen(ContentManager content, EventHandler events, WasteGame game)
            : base(events)
        {
            background = content.Load<Texture2D>("Images/Screens/credits");
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(background, Vector2.Zero, Color.White);
            base.Draw(batch);
            batch.End();
        }

        public override void Update(GameTime time)
        {
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B))
            {
                PressedB = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                PressedB = true;
            }
            if (PressedB && (GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.B) || Keyboard.GetState().IsKeyUp(Keys.Escape)))
            {
                PressedB = false;
                screenEvent.Invoke(this, new EventArgs());
            }
        }
    }
}
