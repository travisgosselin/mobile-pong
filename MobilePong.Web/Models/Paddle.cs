using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobilePong.Web.Models
{
    public class Paddle
    {
        public int X { get; set; } // position at center of paddle
        public int Width { get; set; }
        public bool IsInPaddleExpand { get; set; }
        public GameSettings Settings { get; set; }

        public Paddle(GameSettings settings)
        {
            Settings = settings;
            ResetPosition();
        }

        public void PaddleExpand()
        {
            if (!IsInPaddleExpand && Width == Settings.PaddleWidth)
            {
                IsInPaddleExpand = true;
            }
        }

        public void ResetPosition()
        {
            Width = Settings.PaddleWidth;
            X = Settings.GameWidth/2;
            IsInPaddleExpand = false;
        }

        public void PaddleMove(int xPosition)
        {
            X = xPosition;
        }

        public void Move()
        {
            if (IsInPaddleExpand && Width < Settings.PaddleExpandMaxWidth)
            {
                Width += Settings.PaddleExpandIncrement;
            }
            else if (Width > Settings.PaddleWidth)
            {
                Width -= Settings.PaddleExpandIncrement;
                IsInPaddleExpand = false;
            }
        }
    }
}