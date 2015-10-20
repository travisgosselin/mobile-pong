using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobilePong.Web.Models
{
    public class Ball
    {
        public GameSettings Settings { get; set; }

        public bool YDirectionUp { get; set; }
        public bool XDirectionLeft { get; set; }

        public int Y { get; set; }
        public int X { get; set; }

        public bool IsInSpeedBurst { get; set; }
        public int SpeedBurstCounter { get; set; }

        public Ball(GameSettings settings)
        {
            Settings = settings;
            ResetPosition();
        }


        public void SpeedBurst()
        {
            if (!IsInSpeedBurst)
            {
                IsInSpeedBurst = true;
                SpeedBurstCounter = Settings.SpeedBurstTimeLength;
            }
        }

        public void Move()
        {
            // calculate ball speed based on speed burst
            var speed = Settings.BallSpeed;
            if (IsInSpeedBurst)
            {
                if (SpeedBurstCounter <= 0)
                {
                    IsInSpeedBurst = false;
                }

                speed = Settings.SpeedBurstIncrement;
                --SpeedBurstCounter;
            }

             // move the ball
            X = XDirectionLeft ? X - speed : X + speed;
            Y = YDirectionUp ? Y - speed : Y + speed;

            // check for edges
            if (X <= Settings.BallRadius || X >= Settings.GameWidth - Settings.BallRadius)
            {
                XDirectionLeft = !XDirectionLeft;
            }

            if (Y <= Settings.BallRadius || Y >= Settings.GameHeight - Settings.BallRadius + 50)
            {
                YDirectionUp = !YDirectionUp;
            }
        }

        public void ResetPosition()
        {
            X = Settings.GameWidth / 3;
            Y = Settings.GameHeight / 2;
            XDirectionLeft = false;
            YDirectionUp = true;
        }

        public bool CheckBeyondGoal()
        {
            if (Y > Settings.GameHeight)
            {
                ResetPosition();
                return true;
            }

            return false;
        }
    }
}