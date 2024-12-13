using Mysqlx.Expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
namespace Coursework
{
    class Fallen_Object
    {
        public uint Height { get; set; }
        public uint Width { get; set; }
        public System.Windows.Shapes.Rectangle Thing { get; set; }
        public double Speed { get; set; }
        public bool Visible { get; set; }

        string[] Uri_names = null;
        public bool Is_good { get; set; }
        public Fallen_Object(Random Rnd, LevelsOfPlay levelsOfPlay)
        {
            KeyValuePair<string, System.Drawing.Color>[] Uri_names;
            if (Rnd.Next() % 2 == 0)
            {
                Is_good = true;
                Uri_names = new KeyValuePair<string, System.Drawing.Color>[] {
                new KeyValuePair<string, System.Drawing.Color>("C:\\C#\\Coursework\\bin\\Debug\\Media\\Apple.png", System.Drawing.Color.FromArgb(255, 255, 255)),
                new KeyValuePair<string, System.Drawing.Color>("C:\\C#\\Coursework\\bin\\Debug\\Media\\Bread.png", System.Drawing.Color.FromArgb(247, 247, 247)),
                new KeyValuePair<string, System.Drawing.Color>("C:\\C#\\Coursework\\bin\\Debug\\Media\\Cake.png", System.Drawing.Color.FromArgb(246, 250, 249)),
                new KeyValuePair<string, System.Drawing.Color>("C:\\C#\\Coursework\\bin\\Debug\\Media\\Cheese.png", System.Drawing.Color.FromArgb(246, 246, 246)),
                new KeyValuePair<string, System.Drawing.Color>("C:\\C#\\Coursework\\bin\\Debug\\Media\\Sausage.png", System.Drawing.Color.FromArgb(255, 255, 255))
                };
                Height = (uint)Rnd.Next(55, 300 / (int)levelsOfPlay);
                Width = (uint)Rnd.Next(50, 400 / (int)levelsOfPlay);
                Speed = Rnd.Next(1, (int)levelsOfPlay * 3);
            }
            else
            {
                Is_good = false;
                Uri_names = new KeyValuePair<string, System.Drawing.Color>[] {
                new KeyValuePair<string, System.Drawing.Color>("C:\\C#\\Coursework\\bin\\Debug\\Media\\Boot.png", System.Drawing.Color.FromArgb(255, 255, 255)),
                new KeyValuePair<string, System.Drawing.Color>("C:\\C#\\Coursework\\bin\\Debug\\Media\\Disk.png", System.Drawing.Color.FromArgb(247, 247, 247)),
                new KeyValuePair<string, System.Drawing.Color>("C:\\C#\\Coursework\\bin\\Debug\\Media\\Nail.png", System.Drawing.Color.FromArgb(246, 250, 249)),
                new KeyValuePair<string, System.Drawing.Color>("C:\\C#\\Coursework\\bin\\Debug\\Media\\Spinning.png", System.Drawing.Color.FromArgb(246, 246, 246)),
                new KeyValuePair<string, System.Drawing.Color>("C:\\C#\\Coursework\\bin\\Debug\\Media\\Chair.png", System.Drawing.Color.FromArgb(255, 255, 255))
                };
                Height = (uint)Rnd.Next(55, 75 * (int)levelsOfPlay);
                Width = (uint)Rnd.Next(50, 100 * (int)levelsOfPlay);
                Speed = Rnd.Next(1, (int)levelsOfPlay * 3);
            }
            Visible = false;
            Thing = new System.Windows.Shapes.Rectangle();
            Thing.Height = Height;
            Thing.Width = Width;
            ImageBrush imgBrush = new ImageBrush();
            var Unnoen = Uri_names[Rnd.Next(0, 5)];
            imgBrush.ImageSource = new BitmapImage(new Uri(Uri_names[Rnd.Next(0, 5)].Key)); //MakeImageSourceFromPicture(Unnoen.Key, Unnoen.Value.R, Unnoen.Value.G, Unnoen.Value.B);
            Thing.Fill = imgBrush;//imgBrush
            Canvas.SetTop(Thing, Height * -1);
            Canvas.SetLeft(Thing, Rnd.Next(0, 800 - (int)Width));
        }
        public bool Collision_with_player(System.Windows.Shapes.Rectangle player)
        {
            if (Canvas.GetLeft(player) <= Canvas.GetLeft(Thing) + Thing.Width && Canvas.GetLeft(player) + player.Width >= Canvas.GetLeft(Thing) &&
                Canvas.GetTop(Thing) + Thing.Height <= Canvas.GetTop(player) + Speed && Canvas.GetTop(Thing) + Thing.Height >= Canvas.GetTop(player) )
                return true;
            return false;
        }
        public System.Windows.Shapes.Rectangle Get_obj()
        {
            return Thing;
        }

        public void Update()
        {
            Canvas.SetTop(Thing, Canvas.GetTop(Thing) + Speed);
        }
    }
}