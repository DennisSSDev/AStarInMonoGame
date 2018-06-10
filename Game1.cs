using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AStarProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int gridSize;
        int tileSize;
        Tile[] allTiles;

        struct TileTextures
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
                
                int output = randomizer.Next(0,9);

                if(output <= 7)
                {
                    return white;
                }

                return black;
            }
        }
        //Make an instance to store all tile textures
        TileTextures tileTextures;
        
        public Game1()
        {
            tileTextures = new TileTextures();
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            //Make sure to set the viewport to be equal in size
            SetViewPort();
            gridSize = GraphicsDevice.Viewport.Width;
            tileSize = 20;

            GenerateTiles(out allTiles, (gridSize / tileSize) * (gridSize / tileSize), gridSize / tileSize, tileSize);

        }

        private void SetViewPort()
        {
            var viewport = GraphicsDevice.Viewport;
            viewport.Height = viewport.Width;
            GraphicsDevice.Viewport = viewport;
        }
        private void GenerateTiles(out Tile[] tiles, int arraySize, int xyArraySize,int pTileSize)
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
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // TODO: Add your update logic here

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
                spriteBatch.Draw(tile.TextureStatus,tile.Rectangle,Color.White);
            }
            spriteBatch.End();

           
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
