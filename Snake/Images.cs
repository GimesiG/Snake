using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snake
{
    public static class Images
    {
        public readonly static ImageSource empty = loadImage("Empty.png");
        public readonly static ImageSource head = loadImage("Head.png");
        public readonly static ImageSource deadBody = loadImage("DeadBody.png");
        public readonly static ImageSource deadHead = loadImage("DeadHead.png");
        public readonly static ImageSource food = loadImage("Food.png");
        public readonly static ImageSource body = loadImage("Body.png");

        private static ImageSource loadImage(string fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative));
        }
    }
}
