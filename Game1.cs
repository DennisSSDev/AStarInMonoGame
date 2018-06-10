using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AStarProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region CUSTOM STRUCT and ENUMS
        private struct TileTextures
        {
            public Texture2D white;
            public Texture2D black;
            public Texture2D green;
            public Texture2D lightBlue;
            public Texture2D yellow;
            public Texture2D red;
            public Texture2D darkBlue;

            private Random randomizer;

            public Texture2D TextureRandomizer()
            {
                if (randomizer == null)
                    randomizer = new Random();

                int output = randomizer.Next(0, 9);

                if (output <= 6)
                {
                    return white;
                }

                return black;
            }
        }
        private enum GameStatus
        {
            StartTileSelection,
            EndTileSelection,
            Running,
            Finished
        }
        #endregion

        #region Private Simple Variables
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private int gridSize;
        private int tileSize;
        private Tile[,] allTiles;
        #endregion

        #region CLASS, STRUCT and ENUM Instances
        private TileTextures tileTextures;
        private GameStatus gameState;
        #endregion

        

        //Some inits are within the game constructor
        public Game1()
        {
            tileTextures = new TileTextures();
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 700;
        }

        //Generate a randomized tile set (Does not check for blocked off areas)
        private void GenerateTiles(out Tile[,] tiles, int arraySize, int xyArraySize, int pTileSize)
        {
            tiles = new Tile[arraySize,arraySize];

            for (int x = 0; x < xyArraySize; x++)
            {
                for (int y = 0; y < xyArraySize; y++)
                {
                    tiles[x,y] = new Tile(tileTextures.TextureRandomizer(), pTileSize, pTileSize, x * pTileSize, y * pTileSize,x,y);
                }
            }
        }

        //A* Algorithm
        private List<Tile> AStar(Tile startingNode, Tile endNode)
        {
            
            //Create an open set with the starting node first
            List<Tile> openSet = new List<Tile>();
            openSet.Add(startingNode);
            //Create a HashSet
            HashSet<Tile> closedSet = new HashSet<Tile>();
            
            while (openSet.Count > 0)
            {
                Thread.Sleep(100);//to Allow the visualization

                Tile currentTile = openSet[0];
                //OPtimize later
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentTile.FCost || openSet[i].FCost == currentTile.FCost && openSet[i].HCost < currentTile.HCost)
                        currentTile = openSet[i];
                }

                openSet.Remove(currentTile);
                closedSet.Add(currentTile);
                foreach (var item in closedSet)
                {
                    item.TextureStatus = tileTextures.green;
                }
                if (currentTile == endNode)
                { 
                    return RetracePath(startingNode, endNode);
                }

                foreach (var neighbour in FindNeighbours(currentTile))
                {
                    if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentTile.GCost + GetDistance(currentTile, neighbour);

                    if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, endNode);
                        neighbour.Parent = currentTile;

                        if (!openSet.Contains(neighbour))
                        {
                            neighbour.TextureStatus = tileTextures.lightBlue;
                            openSet.Add(neighbour);
                            
                        }
                    }
                }
                
            }
            return null;
        }

        private List<Tile> RetracePath(Tile beginTile, Tile endTile)
        {
            List<Tile> pathNodes = new List<Tile>();
            Tile currentTile = endTile;

            while (currentTile != beginTile)
            {
                pathNodes.Add(currentTile);
                currentTile = currentTile.Parent;
            }
            pathNodes.Reverse();
            return pathNodes;
        }

        //Formula for getting the distance between 2 nodes
        private int GetDistance(Tile A_tile, Tile B_tile)
        {
            int dist_x = Math.Abs(A_tile.gridX - B_tile.gridX);
            int dist_y = Math.Abs(A_tile.gridY - B_tile.gridY);

            if (dist_x > dist_y)
            {
                return 14 * dist_y + 10 * (dist_x - dist_y);
            }
            else
            {
                return 14 * dist_x + 10 * (dist_y - dist_x);
            }
        }

        //Find all the possible Neihbours within the range of the tile
        private List<Tile> FindNeighbours(Tile givenNode)
        {
            List<Tile> neighbours = new List<Tile>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = givenNode.gridX + x;
                    int checkY = givenNode.gridY + y;

                    if(checkX>=0 && checkX < (gridSize/tileSize) && checkY >= 0 && checkY < (gridSize / tileSize))
                    {
                        neighbours.Add(allTiles[checkX, checkY]);
                    }     
                }
            }
            return neighbours;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.IsMouseVisible = true;

            gridSize = GraphicsDevice.Viewport.Width;
            tileSize = 20;

            //Initialize game status
            gameState = GameStatus.StartTileSelection;

            //Full Tile Generation
            GenerateTiles(out allTiles, (gridSize / tileSize), gridSize / tileSize, tileSize);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            tileTextures.white = Content.Load<Texture2D>("sprites/whiteTile");
            tileTextures.black = Content.Load<Texture2D>("sprites/blackTile");
            tileTextures.darkBlue = Content.Load<Texture2D>("sprites/startTile");
            tileTextures.green = Content.Load<Texture2D>("sprites/greenTile");
            tileTextures.lightBlue = Content.Load<Texture2D>("sprites/lightBlueTile");
            tileTextures.red = Content.Load<Texture2D>("sprites/endTile");
            tileTextures.yellow = Content.Load<Texture2D>("sprites/yellowTile");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
           
            //this will run as long as you haven't made the 2 selection points
            if (gameState == GameStatus.StartTileSelection || gameState == GameStatus.EndTileSelection)
            {
                MouseState ms2 = Mouse.GetState();
                if (!GraphicsDevice.Viewport.Bounds.Contains(ms2.Position))
                    return;
                else
                {
                    MouseState ms = Mouse.GetState();
                    Tile foundCell = null;
                    Rectangle mouseSelectionPosition = new Rectangle(ms.Position, new Point(1, 1));
                    foreach (var cell in allTiles)
                    {
                        if (cell.Rectangle.Contains(mouseSelectionPosition) && cell.IsWalkable)
                        {
                            foundCell = cell;
                            if(gameState == GameStatus.StartTileSelection && !foundCell.IsEnd && !foundCell.IsStart)
                            {
                                foundCell.TextureStatus = tileTextures.darkBlue;
                            }
                            else if(gameState == GameStatus.EndTileSelection && !foundCell.IsEnd && !foundCell.IsStart)
                            {
                                foundCell.TextureStatus = tileTextures.red;
                            }     
                        }
                        else
                        {
                            if (cell.IsWalkable && !cell.IsEnd && !cell.IsStart)
                                cell.TextureStatus = tileTextures.white;
                            else if(!cell.IsEnd && !cell.IsStart)
                                cell.TextureStatus = tileTextures.black;
                        }
                    }
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        if (gameState == GameStatus.StartTileSelection)
                        {
                            foundCell.TextureStatus = tileTextures.darkBlue;
                            foundCell.IsStart = true;
                            gameState = GameStatus.EndTileSelection;
                            Thread.Sleep(300);
                        }
                        else if (gameState == GameStatus.EndTileSelection)
                        {
                            foundCell.TextureStatus = tileTextures.red;
                            gameState = GameStatus.Running;
                            foundCell.IsEnd = true;
                            Thread.Sleep(300);
                            Tile startTile = null;
                            foreach (var tile in allTiles)
                            {
                                if (tile.IsStart)
                                    startTile = tile;
                            }
                            //Now that both beginning and end are set -> the algorthing can do its job;
                            if (startTile != null)
                            {
                                Thread AStarCalculator = new Thread(new ThreadStart(() => 
                                {
                                    List<Tile> output = AStar(startTile, foundCell);
                                    foreach (var node in output)
                                    {
                                        node.TextureStatus = tileTextures.yellow;
                                    }
                                    gameState = GameStatus.Finished;
                                }));

                                AStarCalculator.Start();
                            }
                        }
                        else
                        {
                            throw new Exception("A none existent state has been passed into the box selection, please check your input");
                        }
                    }
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            foreach (var tile in allTiles)
            {
                spriteBatch.Draw(tile.TextureStatus, tile.Rectangle, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
