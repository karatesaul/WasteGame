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
    class SplashScreen : Screen
    {
        #region Fields
        Texture2D background;
        bool pressedSpace = false;
        bool pressedStart = false;
        #endregion

        #region Constructor
        public SplashScreen(ContentManager content, EventHandler scrEvent)
            : base(scrEvent)
        {
            background = content.Load<Texture2D>(@"Images\SplashScreen2");
            screenEvent = scrEvent;
        }
        #endregion

        #region Update
        public override void Update(GameTime time)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                pressedSpace = true;
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
            {
                pressedStart = true;
            }
            if (pressedSpace && (Keyboard.GetState().IsKeyUp(Keys.Space) /*|| GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.Start)*/))
            {
                screenEvent.Invoke(this, new EventArgs());
                pressedSpace = false;
            }

            if (pressedStart && GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.Start))
            {
                screenEvent.Invoke(this, new EventArgs());
                pressedStart = false;
            }

            base.Update(time);
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(background, Vector2.Zero, Color.White);
            batch.End();
            base.Draw(batch);
        }
        #endregion
    }
}
