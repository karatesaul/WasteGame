using System;
using System.Collections.Generic;
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

using BrokenCycleGame.AI.HumanAI;
using BrokenCycleGame.AI;
using BrokenCycleGame.Screens;

using Indiefreaks.AOP.Profiler;
using Indiefreaks.Xna.Profiler;
namespace BrokenCycleGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class WasteGame : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        Texture2D playerSprite;

        //the main player
        public Level currLevel;

        public Vector2 offset;
        public int levelIndex = -1;
        public int numberOfLevels = 3;
        SplashScreen splashscreen;
        public GameScreen gameScreen;
        TitleScreen menuScreen;
        OptionsScreen optionScreen;
        Screen CurrentScreen;
        PauseScreen pauseScreen;
        HubScreen hubScreen;
        CreditsScreen creditsScreen;
        StoryScreen storyScreen;
        DirectionsScreen directionsScreen;

        Random rand = new Random();


        Viewport TopLeft;
        Viewport TopRight;
        Viewport LowLeft;
        Viewport LowRight;
        Viewport DefaultPort;

        public Random random = new Random();

        public World world;
        public Player[] players = new Player[4];

        public WasteGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //var profilerGameComponent = new ProfilerGameComponent(this, "Calibri");
            //ProfilingManager.Run = false;
            //Components.Add(profilerGameComponent);

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //initialize the player
            base.Initialize();

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>

        public Player[] MakePlayers()
        {
            Player[] player = new Player[4];
            player[0] = new Player(world, this, Window, PlayerIndex.One, playerSprite, new Human(PlayerIndex.One), new Vector2(), new Vector2());
            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
                player[1] = new Player(world, this, Window, PlayerIndex.Two, playerSprite, new Human(PlayerIndex.Two), new Vector2(), new Vector2());
            else player[1] = new Player(world, this, Window, PlayerIndex.Two, playerSprite, new BasicAI(), new Vector2(), new Vector2());
            if (GamePad.GetState(PlayerIndex.Three).IsConnected)
                player[2] = new Player(world, this, Window, PlayerIndex.Three, playerSprite, new Human(PlayerIndex.Three), new Vector2(), new Vector2());
            else player[2] = new Player(world, this, Window, PlayerIndex.Three, playerSprite, new BasicAI(), new Vector2(), new Vector2());
            if (GamePad.GetState(PlayerIndex.Four).IsConnected)
                player[3] = new Player(world, this, Window, PlayerIndex.Four, playerSprite, new Human(PlayerIndex.Four), new Vector2(), new Vector2());
            else player[3] = new Player(world, this, Window, PlayerIndex.Four, playerSprite, new BasicAI(), new Vector2(), new Vector2());

            return player;
        }

        public void LoadLevel(int index)
        {
            index = (index) % numberOfLevels;

            if (currLevel != null)
            {
                currLevel.Dispose();
            }

            string LevelPath = string.Format("Content/Levels/{0}.txt", index);
            using (Stream fileStream = TitleContainer.OpenStream(LevelPath))
                currLevel = new Level(Services, Content, fileStream, world, this, new Vector2(), players, index);
        }

        public void LoadNextLevel()
        {
            ++levelIndex;
            LoadLevel(levelIndex);
        }

        public void restartLevel()
        {
            --levelIndex;
            LoadLevel(levelIndex);
        }

        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerSprite = Content.Load<Texture2D>("Images/Sweeper/sweeperdown");
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            graphics.ApplyChanges();
            //create game screens
            world = new World(Vector2.Zero);
            gameScreen = new GameScreen(Content, new EventHandler(GameScreenEvents), this, world);
            menuScreen = new TitleScreen(Content, new EventHandler(TitleEvents), this);
            optionScreen = new OptionsScreen(Content, new EventHandler(OptionEvents), this);
            splashscreen = new SplashScreen(Content, new EventHandler(SplashEvents));
            pauseScreen = new PauseScreen(Content, new EventHandler(PauseEvents), this);
            hubScreen = new HubScreen(Content, new EventHandler(HubEvents), this, world);
            creditsScreen = new CreditsScreen(Content, new EventHandler(CreditsEvents), this);
            storyScreen = new StoryScreen(Content, new EventHandler(StoryEvents));
            directionsScreen = new DirectionsScreen(Content, new EventHandler(directionsEvents), this);
            CurrentScreen = splashscreen;

            //set the viewports' size and locations
            DefaultPort = GraphicsDevice.Viewport;
            TopLeft.Width = DefaultPort.Width / 2;
            TopLeft.Height = DefaultPort.Height / 2;
            TopLeft.X = 0;
            TopLeft.Y = 0;
            TopRight.Width = DefaultPort.Width / 2;
            TopRight.Height = DefaultPort.Height / 2;
            TopRight.X = DefaultPort.Width / 2;
            TopRight.Y = 0;
            LowLeft.Width = DefaultPort.Width / 2;
            LowLeft.Height = DefaultPort.Height / 2;
            LowLeft.X = 0;
            LowLeft.Y = DefaultPort.Height / 2;
            LowRight.Width = DefaultPort.Width / 2;
            LowRight.Height = DefaultPort.Height / 2;
            LowRight.X = DefaultPort.Width / 2;
            LowRight.Y = DefaultPort.Height / 2;

            offset = new Vector2();
            //offset = new Vector2(Window.ClientBounds.Width / 2 - (16 * 15), Window.ClientBounds.Height / 2 - (16 * 15));
        }

        /// <summary>
        /// LoadNewLevel will take in a filestream, open it, and read the data appropriately.
        /// Will not return anything, but initialize a new level.
        /// </summary>

     
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //update the current screen
            CurrentScreen.Update(gameTime);
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        { 
            GraphicsDevice.Clear(Color.White);

            //draw the current screen
            CurrentScreen.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        public void GameScreenEvents(object obj, EventArgs e)
        {
            CurrentScreen = pauseScreen;
        }

        public void HubEvents(object obj, EventArgs e)
        {
            levelIndex = hubScreen.eventCallNum;
            switch (hubScreen.eventCallNum)
            {
                   
                case 0:
                    LoadLevel(levelIndex);
                    gameScreen.makeCameras();
                    
                    break;
                case 1:
                    LoadLevel(levelIndex);
                    gameScreen.makeCameras();
                    break;
                case 2:
                    LoadLevel(levelIndex);
                    gameScreen.makeCameras();
                    break;
                default:
                    levelIndex = 0;
                    LoadLevel(levelIndex);
                    gameScreen.makeCameras();
                    break;

            }
            CurrentScreen = gameScreen;


        }

        public void StoryEvents(object obj, EventArgs e)
        {
            CurrentScreen = menuScreen;
        }

        public void CreditsEvents(object obj, EventArgs e)
        {
            CurrentScreen = menuScreen;
        }
        public void TitleEvents(object obj, EventArgs e)
        {
           
            //Menu switch statement
            switch (menuScreen.eventCallNum)
            {
                //Start Game
                case 0:
                    //GameScreen newGameScreen = new GameScreen(Content, new EventHandler(GameScreenEvents), this, new Level(Window.ClientBounds.Width / 16 + 1, Window.ClientBounds.Height / 16 + 1, this, offset));
                    CurrentScreen = hubScreen;
                    break;
                //Options
                case 1:
                    CurrentScreen = optionScreen;
                    optionScreen.previousScreen = menuScreen;
                    break;
                //Exit Game
                case 2:
                    this.Exit();
                    break;
                case 3:
                    CurrentScreen = creditsScreen;
                    break;
                default:
                    CurrentScreen = directionsScreen;
                    break;
            }
        }
        public void OptionEvents(object obj, EventArgs e)
        {
            // Escape to main menu
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B))
            {
                CurrentScreen = optionScreen.previousScreen;
            }
            switch (optionScreen.eventCallNum)
            {
                case 0:
                    break;
                case 1:

                    break;
                case 2:
                    break;
            }
        }

        public void SplashEvents(object obj, EventArgs e)
        {
            CurrentScreen = storyScreen;
        }

        public void PauseEvents(object obj, EventArgs e)
        {
            switch (pauseScreen.eventCallNum)
            {
                case 0:
                    CurrentScreen = gameScreen;
                    break;
                case 1:
                    CurrentScreen = optionScreen;
                    optionScreen.previousScreen = pauseScreen;
                    break;
                case 2:
                    CurrentScreen = menuScreen;
                    //reset the level
                    gameScreen = new GameScreen(Content, new EventHandler(GameScreenEvents), this, world);
                    break;
                default:
                    break;
            }
        }

        public void directionsEvents(object obj, EventArgs e)
        {
            CurrentScreen = menuScreen;
        }

        /*public void LoadGame(int lg)
        {
            switch (LoadGame.eventCallNum)
            {
                case 0:
                    string line = "Current_Level 1";
                    string[] words = line.Split(" ");
                    a[0] = Current_level;
                case 1:
                    a[1] = 1;
            }

        }*/
    }
}