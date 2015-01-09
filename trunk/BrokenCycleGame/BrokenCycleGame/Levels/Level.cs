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
using BrokenCycleGame.Tiles;
using BrokenCycleGame.AI.HumanAI;

namespace BrokenCycleGame
{
    public class Level
    {
        #region Fields
        public Tile[,] tiles;
       // public Texture2D background;
        public SludgeTile[,] backgroundTiles;
        public Texture2D background;
        public int score;
        public WasteGame game;
        ContentManager content;
        public World gameWorld;
        public List<Roomba> roombaList;
        public List<Tower> towerList;
        public Player[] playerList;

        List<Roomba> deadRoombas = new List<Roomba>();
        List<Tower> deadTowers = new List<Tower>();
        public bool winFlag;

        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        public int Height
        {
            get { return tiles.GetLength(1); }
        }
        #endregion

        #region Constructor
        public Level(IServiceProvider serviceProvider, ContentManager Content, Stream file, World gameWorld, WasteGame game, Vector2 offset, Player[] playerList, int levelIndex)
        {
            this.playerList = playerList;
            content = new ContentManager(serviceProvider, "Content");
            background = content.Load<Texture2D>("Images/Screens/final_level");
            score = 0;
            Random random = new Random();
            this.gameWorld = gameWorld;
            this.game = game;
            roombaList = new List<Roomba>();
            towerList = new List<Tower>();
            LoadTiles(file);

        }



        public void Dispose()
        {
            content.Unload();
            if (playerList != null)
            {
                foreach (Player p in playerList)
                {
                    p.DestroyFixture(p.playerFixture);
                }
            }
            if (tiles != null)
            {
                foreach (Tile T in tiles)
                {
                    T.DestroyFixture(T.tileFixture);
                }
            }

            if (roombaList != null)
            {
                foreach (Roomba r in roombaList)
                {
                    r.DestroyFixture(r.roombaFixture);
                }
            }
            if (towerList != null)
            {
                foreach (Tower t in towerList)
                {

                    t.DestroyFixture(t.towerFixture);
                }
            }
        }
        public void LoadTiles(Stream file)
        {
            int width;
            List<string> lines = new List<string>();

            using (StreamReader stream = new StreamReader(file))
            {
                string line = stream.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceding lines.", lines.Count));
                    line = stream.ReadLine();
                }
            }

            tiles = new Tile[width, lines.Count];
            backgroundTiles = new SludgeTile[width, lines.Count];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

        }

        public Tile LoadTile(char tileType, int x, int y)
        {
            switch(tileType)
            {
                //Normal tile types
                case '.':
                    return new NormalTile(gameWorld, new Vector2(x, y), game, new Vector2(), this);
                //Wall tile types
                case 'w':
                    return new Wall(gameWorld, new Vector2(x , y), game, new Vector2(), this);
                case 'r':
                    return LoadRoomba(roombaList, x, y);
                case 't':
                    return LoadTower(towerList, x, y);
                
                case 's':
                    return LoadTarTile(x, y);

                case 'z':
                    return LoadChargeTile(x, y);
                //Load player 1's position
                case 'u':
                    return LoadPlayerOne(x, y);

                //Load player 2's position    
                case 'i':
                    return LoadPlayerTwo(x, y);

                //Load player 3's position
                case 'o':
                    return LoadPlayerThree(x, y);

                //Load player 4's position
                case 'p':
                     return LoadPlayerFour(x, y);
                    

                default: throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        private Tile LoadPlayerOne(int x, int y)
        {
            playerList[0] = (new Player(gameWorld, game, game.Window, PlayerIndex.One, game.Content.Load<Texture2D>("Images/Sweeper/sweeperdown"), new Human(PlayerIndex.One), new Vector2(), new Vector2(x, y)));
            return new NormalTile(gameWorld, new Vector2(x,y), game, new Vector2(), this);
        }
        private Tile LoadPlayerTwo(int x, int y)
        {
            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
                playerList[1] = new Player(gameWorld, game, game.Window, PlayerIndex.Two, game.Content.Load<Texture2D>("Images/Sweeper/sweeperdown"), new Human(PlayerIndex.Two), new Vector2(), new Vector2(x, y));
            else playerList[1] = new Player(gameWorld, game, game.Window, PlayerIndex.Two, game.Content.Load<Texture2D>("Images/Sweeper/sweeperdown"), new BasicAI(), new Vector2(), new Vector2(x, y));
            return new NormalTile(gameWorld, new Vector2(x, y), game, new Vector2(), this);
        }

        private Tile LoadPlayerThree(int x, int y)
        {
            if (GamePad.GetState(PlayerIndex.Three).IsConnected)
                playerList[2] = new Player(gameWorld, game, game.Window, PlayerIndex.Three, game.Content.Load<Texture2D>("Images/Sweeper/sweeperdown"), new Human(PlayerIndex.Three), new Vector2(), new Vector2(x, y));
            else playerList[2] = new Player(gameWorld, game, game.Window, PlayerIndex.Three, game.Content.Load<Texture2D>("Images/Sweeper/sweeperdown"), new BasicAI(), new Vector2(), new Vector2(x, y));
            return new NormalTile(gameWorld, new Vector2(x, y), game, new Vector2(), this);
        }

        private Tile LoadPlayerFour(int x, int y)
        {
            if (GamePad.GetState(PlayerIndex.Four).IsConnected)
                playerList[3] = new Player(gameWorld, game, game.Window, PlayerIndex.Four, game.Content.Load<Texture2D>("Images/Sweeper/sweeperdown"), new Human(PlayerIndex.Four), new Vector2(), new Vector2(x, y));
            else playerList[3] = new Player(gameWorld, game, game.Window, PlayerIndex.Four, game.Content.Load<Texture2D>("Images/Sweeper/sweeperdown"), new BasicAI(), new Vector2(), new Vector2(x, y));
            return new NormalTile(gameWorld, new Vector2(x, y), game, new Vector2(), this);
        }
        private Tile LoadRoomba(List<Roomba> roombaList, int x, int y)
        { 
            roombaList.Add(new Roomba(gameWorld, game, tiles, game.Window, game.Content.Load<Texture2D>("Images/Enemies/Roomba"), new Vector2(x, y)));
            return new NormalTile(gameWorld, new Vector2(x,y), game, new Vector2(), this);
        }

        private Tile LoadTower(List<Tower> towerList, int x, int y)
        {
            towerList.Add(new Tower(gameWorld, game, game.Window, game.Content.Load<Texture2D>("Images/Enemies/Tower"), new Vector2(), new Vector2(x, y)));
            return new NormalTile(gameWorld, new Vector2(x, y), game, new Vector2(), this);
        }

        private Tile LoadTarTile(int x, int y)
        {
            return new TarTile(gameWorld, new Vector2(x, y), game, new Vector2(), this);
        }

        private Tile LoadChargeTile(int x, int y)
        {
            return new ChargeTile(gameWorld, new Vector2(x, y), game, new Vector2(), this);
        }

        public Boolean levelOver()
        {

            return playersAlive();
        
        }
        public Boolean playersAlive()
        {
            Boolean playersAreAlive = true;
            for (int i = 0; i < 3; i++)
            {
                //if any players are alive, return true
                if (!playerList[i].isDead()) return false;
                else
                {
                    continue;
                }
            }
            return playersAreAlive;
        }


        #endregion


        #region Draw
        public void draw(SpriteBatch spriteBatch, camera cam)
        {

            if (tiles != null)
            {
                foreach (Tile t in tiles)
                {
                    if (t != null)
                        t.draw(spriteBatch, cam);
                }
            }

            if (roombaList != null)
            {
                foreach (Roomba r in roombaList)
                {
                    if (r != null)
                        r.Draw(spriteBatch, cam);
                }
            }

            if (towerList != null)
            {
                foreach (Tower tower in towerList)
                {
                    if (tower != null)
                        tower.Draw(spriteBatch, cam);
                }
            }
        }
        #endregion

        #region Update
        public void update(GameTime gameTime)
        {
            if (!levelOver())
            {
                if (tiles != null)
                {
                    foreach (Tile t in tiles)
                    {
                        if (t != null)
                            t.Update();
                    }
                }

                if (playerList != null)
                {
                    foreach (Player p in playerList)
                    {
                        if (p != null)
                        {
                            if (p.isRemoved())
                            {
                                score += p.collectedScore;
                                p.collectedScore = 0;
                            }
                        }
                    }
                }
                if (roombaList != null)
                {
                    foreach (Roomba r in roombaList)
                    {
                        if (r != null)
                            r.Update(gameTime);
                        if (r.dead)
                            deadRoombas.Add(r);
                    }
                }

                if (towerList != null)
                {
                    foreach (Tower tower in towerList)
                    {
                        if (tower != null)
                            tower.Update(gameTime);
                        if (tower.dead)
                            deadTowers.Add(tower);
                    }
                }
                while (!(deadRoombas.Count() == 0))
                { //
                    Roomba r = deadRoombas.First();
                    deadRoombas.Remove(r);
                    roombaList.Remove(r);
                    r.DestroyFixture(r.roombaFixture);
                }
                while (!(deadTowers.Count() == 0))
                {
                    Tower t = deadTowers.First();
                    deadTowers.Remove(t);
                    towerList.Remove(t);
                    t.DestroyFixture(t.towerFixture);
                }

                gameWorld.Step(1);
            }
            if (levelOver() && score > 10000)
            {
                game.LoadNextLevel();
            }
            else if (levelOver() && score < 10000)
                game.restartLevel();
            else if (!levelOver()) return;
        }


        #endregion
    }
}
