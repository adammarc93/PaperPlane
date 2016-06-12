using System.Windows.Controls;

namespace PaperPlane
{
    public abstract class GameElement
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Image elementImage;

        public enum canvasDimensions { minWidth = 0, minHeight = 0, maxWidth = 692, maxHeight = 417 }
        
        public void SetImage(Image img)
        {
            this.elementImage = img;
            DrawImage();
        }

        public void GetDimensions(Image img)
        {
            this.Width = (int)img.Width;
            this.Height = (int)img.Height;
        }

        public void DrawImage()
        {
            Canvas.SetLeft(this.elementImage, this.PositionX);
            Canvas.SetTop(this.elementImage, this.PositionY);
        }

        public abstract void SetStartPosition();
    }
}
