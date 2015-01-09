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
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;

namespace BrokenCycleGame
{
    public class Level
    {
        #region Fields
        public Tile[,] tiles;
        public int score;
        public World gameWorld;
        public List<Roomba> roombaList;
        public List<Tower> towerList;
        public Player[] playerList;
        public int height
        {
            get
            {
                return tiles.GetLength(0);
            }
        }

        public int width
        {
            get
            {
                return tiles.GetLength(1);
            }
        }
        #endregion

        #region Constructor
        public Level(ContentManager Content, World gameWorld, int width, int height, WasteGame game, Vector2 offset, Player[] playerList)
        {
            score = 0;
            Random random = new Random();
            this.gameWorld = gameWorld;
            this.playerList = playerList;
            tiles = new Tile[width, height];
            roombaList = new List<Roomba>();
            towerList = new List<Tower>();
            for (int i = 0; i < 1; i++ )
            {
                roombaList.Add(new Roomba(gameWorld, game, tiles, game.Window, game.Content.Load<Texture2D>(@"Images/Enemies/Roomba")));
            }

            for (int j = 0; j < 2; j++)
            {
                towerList.Add(new Tower(gameWorld, game, game.Window, game.Content.Load<Texture2D>(@"Images/Enemies/Tower"), game.offset));
            }
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    //Tile constructor now takes in Level, 
                    //so it can access this array and grab its neighbors
                    tiles[i, j] = new NormalTile(game.world, new Vector2(i, j), game, offset, this);
                }

            }
        }
        #endregion

        #region Draw
        public void draw(SpriteBatch spriteBatch)
        {
            foreach (Tile t in tiles)
            {
                t.draw(spriteBatch);
            }
            foreach (Roomba r in roombaList)
            {
                r.Draw(spriteBatch);
            }

            foreach (Tower tower in towerList)
            {
                tower.Draw(spriteBatch);
            }
        }
        #endregion

        #region Update
        public void update(GameTime gameTime)
        {
            foreach (Tile t in tiles)
            {
                if(t != null)
                t.Update();
            }
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
            List<Roomba> deadRoombas = new List<Roomba>();
            List<Tower> deadTowers = new List<Tower>();
            foreach (Roomba r in roombaList)
            {
                r.Update(gameTime);
                if (r.dead)
                    deadRoombas.Add(r);
            }

            foreach (Tower tower in towerList)
            {
                tower.Update(gameTime);
                if (tower.dead)
                    deadTowers.Add(tower);
            }
            while (!(deadRoombas.Count() == 0))
            { //
                Roomba r = deadRoombas.First();
                deadRoombas.Remove(r);
                roombaList.Remove(r);
            }
            while (!(deadTowers.Count() == 0))
            {
                Tower t = deadTowers.First();
                deadTowers.Remove(t);
                towerList.Remove(t);
            }

            gameWorld.Step(1);
        }
        #endregion
    }
}
