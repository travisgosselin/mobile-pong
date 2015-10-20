using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobilePong.Web.Models
{
    public class GameSettings
    {
        public int GameSpeed = 30;
        public int BallSpeed = 2;
        public int BallRadius = 5;
        
        public int GameHeight = 500;
        public int GameWidth = 500;

        public int PaddleHeight = 5;
        public int PaddleSpeed = 10;
        public int PaddleWidth
        {
            get { return Convert.ToInt32(GameWidth * 0.2); }
        }
        
        public int SpeedBurstIncrement = 5;
        public int SpeedBurstTimeLength = 50;

        public int PaddleExpandIncrement = 15;
        public int PaddleExpandMaxWidth
        {
            get { return PaddleWidth*3; }
        }
    }
}