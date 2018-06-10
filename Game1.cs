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

                if (output <= 7)
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
        private Tile[] allTiles;
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
        private void GenerateTiles(out Tile[] tiles, int arraySize, int xyArraySize, int pTileSize)
        {
            tiles = new Tile[arraySize];

            for (int x = 0; x < xyArraySize; x++)
            {
                for (int y = 0; y < xyArraySize; y++)
                {
                    tiles[(x * xyArraySize) + y] = new Tile(tileTextures.TextureRandomizer(), pTileSize, pTileSize, x * pTileSize, y * pTileSize);
                }
            }
        }

        //A* Algorithm
        private List<Tile> AStar(Tile startingNode, Tile endNode)
        {

            return null;
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
            GenerateTiles(out allTiles, (gridSize / tileSize) * (gridSize / tileSize), gridSize / tileSize, tileSize);
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
                            Thread.Sleep(500);
                        }
                        else if (gameState == GameStatus.EndTileSelection)
                        {
                            foundCell.TextureStatus = tileTextures.red;
                            gameState = GameStatus.Running;
                            foundCell.IsEnd = true;
                            Thread.Sleep(500);
                            Tile startTile = null;
                            foreach (var tile in allTiles)
                            {
                                if (tile.IsStart)
                                    startTile = tile;
                            }
                            //Now that both beginning and end are set -> the algorthing can do its job;
                            if (startTile != null)
                                AStar(startTile, foundCell);
                        }
                        else
                        {
                            throw new Exception("A none existen state has been passed into the box selection, please check your input");
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
