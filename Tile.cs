using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AStarProject
{
    class Tile
    {
        #region Private Variables
        private int width;
        private int height;
        private Texture2D textureStatus;
        private bool isWalkable;
        private bool isStart;
        private bool isEnd;
        #endregion

        #region Public Variables
        public int xPos;
        public int yPos;
        #endregion

        #region Properties
        public Texture2D TextureStatus { get { return textureStatus; } set { if (value is Texture2D) { textureStatus = value; } } }
        public Vector2 Position { get { return new Vector2(xPos, yPos); } }
        public Rectangle Rectangle { get { return new Rectangle(xPos, yPos, width, height); } }
        public bool IsWalkable { get { return isWalkable; } }
        public bool IsStart { get { return isStart; } set { isStart = value; } }
        public bool IsEnd { get { return isEnd; } set { isEnd = value; } }
        #endregion

        #region Constructor

        public Tile(Texture2D textureStatus, int width, int height, int x, int y)
        {
            this.textureStatus = textureStatus;

            if (textureStatus.Name == "sprites/blackTile")
                isWalkable = false;
            else
                isWalkable = true;

            
            this.width = width;
            this.height = height;
            this.xPos = x;
            this.yPos = y;

            isStart = false;
            isEnd = false;
        }

        #endregion
    }
}
