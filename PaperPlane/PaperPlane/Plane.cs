namespace PaperPlane
{
    public enum planeDistance { reallyNotFar = 30, notFar = 60, quiteFar = 90, far = 120, reallyFar = 150 }

public class Plane : GameElement
    {
        public int Distance { get; set; }

        private enum startPosition { lenght = 10, height = 187 }
        private enum planeFlyParameters { flying = 5, falling = 3 }

        //Constructor set plane on start position.
        public Plane()
        {
            SetStartPosition();
        }

        public override void SetStartPosition()
        {
            PositionX = (int)startPosition.lenght;
            PositionY = (int)startPosition.height;
        }

        //Method change plane's position and move the image up.
        public void Flying()
        {
            PositionY -= (int)planeFlyParameters.flying;
            DrawImage();
        }

        //Mathod change plane's position and move the image down.
        public void Falling()
        {
            PositionY += (int)planeFlyParameters.falling;
            DrawImage();
        }

        //Method returns "true" if position of plane's image is smaller then max height of the canvas,
        //else return "false". 
        public bool OnCanvasMax()
        {
            if (PositionY + (int)planeFlyParameters.falling < (int)canvasDimensions.maxHeight - Height)
                return true;
            else
                return false;
        }

        //Method returns "true" if the position of plane's image is greater then min height of the canvas,
        //else return "false". 
        public bool OnCanvasMin()
        {
            if (PositionY - (int)planeFlyParameters.falling > (int)canvasDimensions.minHeight + Height)
                return true;
            else
                return false;
        }
        
        //Method returns "true" if position of plane's image is in another element with 15 px fail margin, 
        //else return "false".
        public bool Collison(int positionX, int positionY, int sizeX, int sizeY)
        {
            bool result1, result2, result3, result4;
            int failMargin = 15;

            if (PositionX + Width - failMargin >= positionX)
                result1 = true;
            else
                result1 = false;

            if (PositionY + Height - failMargin >= positionY)
                result2 = true;
            else
                result2 = false;

            if (positionX + sizeX - failMargin >= positionX)
                result3 = true;
            else
                result3 = false;

            if (positionY + sizeY - failMargin >= PositionY)
                result4 = true;
            else
                result4 = false;

            if (result1 == true && result2 == true && result3 == true && result4 == true)
                return true;
            else
                return false;

        }
    }
}
