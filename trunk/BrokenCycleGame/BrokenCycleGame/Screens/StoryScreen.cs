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
    class StoryScreen : Screen
    {
        #region Fields
        Texture2D background;
        SpriteFont font;
        bool pressedSpace = false;
        bool pressedStart = false;
        #endregion

        #region Constructor
        public StoryScreen(ContentManager content, EventHandler scrEvent)
            : base(scrEvent)
        {
            font = content.Load<SpriteFont>("Calibri");
            background = content.Load<Texture2D>(@"Images\Screens\final_level");
            screenEvent = scrEvent;
        }
        #endregion

        #region Update
        public override void Update(GameTime time)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                pressedSpace = true;
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
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
            batch.DrawString(font, "It is the year 2235", new Vector2(30, 30), Color.Blue);
            batch.DrawString(font, "The worlds resources have gradually diminshed,", new Vector2(30, 45), Color.CornflowerBlue);
            batch.DrawString(font, "and the global economy has never been worse.", new Vector2(30, 60), Color.CornflowerBlue);
            batch.DrawString(font, "Furthermore, the environment has degraded to a nigh irreperable state", new Vector2(30, 75), Color.CornflowerBlue);
            batch.DrawString(font, "A glaring example of this negligence is the ever growing island of trash", new Vector2(30, 90), Color.CornflowerBlue);
            batch.DrawString(font, "in the middle of the Pacific Ocean.", new Vector2(30, 105), Color.CornflowerBlue);
            batch.DrawString(font, "In an attempt to solve two problems simultaneously, the governments of the world", new Vector2(30, 120), Color.CornflowerBlue);
            batch.DrawString(font, "have decided to send in teams of 'Sweepers,' small teams that will mine", new Vector2(30, 135), Color.CornflowerBlue);
            batch.DrawString(font, "this massive garbage pile for resources, and in the process help to clean up", new Vector2(30, 150), Color.CornflowerBlue);
            batch.DrawString(font, "the local environment. You are a sweeper and it is your job to scrape this", new Vector2(30, 165), Color.CornflowerBlue);
            batch.DrawString(font, "oceanic blight clean of waste, for profit and more importantly, to aid the environment!", new Vector2(30, 180), Color.CornflowerBlue);
            batch.DrawString(font, "Press A to Begin!!!", new Vector2(30, 195), Color.CornflowerBlue);
            batch.End();
            base.Draw(batch);
        }
        #endregion
    }
}
