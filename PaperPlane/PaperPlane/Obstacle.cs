using System;
using System.Windows.Controls;

namespace PaperPlane
{
    public class Obstacle : GameElement
    {
        private enum flyParameters { reallySlow = 5, slow = 7, quiteFast = 9, fast = 11, reallyFast = 13, theFastest = 15 }

        //Constructor set obstacle on start random position.
        public Obstacle(Image img)
        {
            GetDimensions(img);
            SetStartPosition();
        }

        //Method set obstacle on new position.
        public override void SetStartPosition()
        {
            PositionX = (int)canvasDimensions.maxWidth;

            Random rnd1 = new Random();
            PositionY = rnd1.Next((int)canvasDimensions.minHeight,
                (int)canvasDimensions.maxHeight - Height);
        }

        //Method returns "true" if position of obstacle's image is smaller then min width of the canvas,
        //else return "false".
        public bool OnCanvas()
        {
            if (PositionX > (int)canvasDimensions.minWidth - Width)
                return true;
            else
                return false;
        }

        //Method causes move the obstacle's image to the left.
        public void Flying(int distance)
        {
            SetFlyingSpeed(distance);
            DrawImage();
        }

        private void SetFlyingSpeed(int distance)
        {
            if (distance <= (int)planeDistance.reallyNotFar)
                PositionX -= (int)flyParameters.reallySlow;
            else if (distance > (int)planeDistance.reallyNotFar && distance <= (int)planeDistance.notFar)
                PositionX -= (int)flyParameters.slow;
            else if (distance > (int)planeDistance.notFar && distance <= (int)planeDistance.quiteFar)
                PositionX -= (int)flyParameters.quiteFast;
            else if (distance > (int)planeDistance.quiteFar && distance <= (int)planeDistance.far)
                PositionX -= (int)flyParameters.fast;
            else if (distance > (int)planeDistance.far && distance <= (int)planeDistance.reallyFar)
                PositionX -= (int)flyParameters.reallyFast;
            else if (distance > (int)planeDistance.reallyFar)
                PositionX -= (int)flyParameters.theFastest;
        }
    }
}