using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using System.Windows.Threading;
using MySql.Data.MySqlClient;
using static Mysqlx.Notice.Frame.Types;
using Image = System.Windows.Controls.Image;

namespace Coursework
{
    public enum LevelsOfPlay: int
    {
        LEVEL1 = 1,
        LEVEL2 = 2
    }
    public class CurrentScore : INotifyPropertyChanged
    {
        uint score;
        string text_score;
        public string Text_score { get { return text_score; } set { text_score = value; OnPropertyChanged(nameof(Text_score)); } }
        public CurrentScore()
        {
            Score = 0;
        }
        public uint Score { get { return score; } set { score = value; Text_score = $"Счёт: {Score}"; } }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class Play_window : Window
    {
        string Nick_name;
        double player_speed = 7;
        bool player_right = false;
        bool player_left = false;
        LevelsOfPlay current_level = LevelsOfPlay.LEVEL1;
        bool paused = false;
        Fallen_Object[] fallen_elements = null;
        uint countofelem = 0;
        readonly uint max_count;
        Random rnd = new Random();
        DispatcherTimer timer = new DispatcherTimer();
        System.Windows.Shapes.Rectangle pause_table = null;
        CurrentScore MyPhone;
        bool pause_before = false;
        public Play_window()
        { }
        public Play_window(string nick_name)
        {
            InitializeComponent();
            this.Loaded += Play_window_Loaded;
            MyPhone = new CurrentScore();
            this.DataContext = MyPhone;
            Nick_name = nick_name;
            max_count = 20;
            fallen_elements = new Fallen_Object[max_count];
            Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            timer.Interval = System.TimeSpan.FromMilliseconds(5);
            timer.Tick += GameLoop;
            timer.Start();
            mycanvas.Focus();
        }
        private void Play_window_Loaded(object sender, RoutedEventArgs e)
        {
            fallen_elements = Make_objects(max_count);
        }
        Fallen_Object[] Make_objects(uint count_of_elem)
        {
            Fallen_Object[] new_objects = new Fallen_Object[(int)count_of_elem];    
            for (int i = 0; i < fallen_elements.Length; i++)
            {
                if (rnd.Next() % 2 == 0)
                    new_objects[i] = new Fallen_Object(rnd, current_level);
                else
                    new_objects[i] = new Fallen_Object(rnd, current_level);
            }
            return new_objects;
        }

        void GameLoop(object sender, EventArgs e)
        {
            if (pause_table == null)
            {
                pause_table = new System.Windows.Shapes.Rectangle
                {
                    Width = 650,
                    Height = 370
                };
                Canvas.SetLeft(pause_table, My_window.Width / 2 - pause_table.Width / 2- 50);
                Canvas.SetTop(pause_table, My_window.Height / 2 - pause_table.Height / 2 - 50);
                ImageSource mybrush = new BitmapImage(new Uri("C:\\C#\\Coursework\\bin\\Debug\\Media\\Pause.png"));
                ImageBrush My_brush = new ImageBrush(mybrush);
                pause_table.Fill = My_brush;
            }

            if (paused)
            {
                if (!mycanvas.Children.Contains(pause_table))
                {
                    mycanvas.Children.Add(pause_table);
                }
                pause_before = true;
                return;
            }
            else
            {
                if (pause_before && mycanvas.Children.Contains(pause_table))
                {
                    mycanvas.Children.Remove(pause_table);
                    pause_before = false;
                }
            }

            if (player_left && Canvas.GetLeft(Player) > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - player_speed);
            }
            else if (player_right && Canvas.GetLeft(Player) + player_speed + Player.Width < My_window.Width)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + player_speed);
            }
            if (countofelem < max_count)
            {
                if (rnd.Next() % 111 == 0)
                {
                    mycanvas.Children.Add(fallen_elements[countofelem++].Get_obj());
                    fallen_elements[countofelem - 1].Visible = true;
                }
            }
            else
            {
                for (int i = 0; i < fallen_elements.Length; i++)
                {
                    if (fallen_elements[i] != null)
                        mycanvas.Children.Remove(fallen_elements[i].Get_obj());
                }
                fallen_elements = Make_objects(max_count);
                countofelem = 0;
            }
            for (int i = 0; i < fallen_elements.Length; i++)
            {
                if (fallen_elements[i] != null && fallen_elements[i].Visible)
                {
                    if (Canvas.GetTop(fallen_elements[i].Get_obj()) > My_window.Height)
                    {
                        if (fallen_elements[i].Is_good)
                            Game_over();
                        mycanvas.Children.Remove(fallen_elements[i].Get_obj());
                        fallen_elements[i] = null;
                        continue;
                    }
                    fallen_elements[i].Update();
                }
            }
            for (int i = 0; i < fallen_elements.Length; i++)
            {
                if (fallen_elements[i] != null && fallen_elements[i].Visible)
                {
                    if (fallen_elements[i].Collision_with_player(Player))
                    {
                        if (fallen_elements[i].Is_good)
                        {
                            MyPhone.Score++;
                            if (MyPhone.Score > (int)current_level * max_count)
                                current_level++;
                        }
                        else
                            Game_over();
                        mycanvas.Children.Remove(fallen_elements[i].Get_obj());
                        fallen_elements[i] = null;
                    }
                }
            }
        }
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            string host = "localhost"; // Имя хоста
            string database = "Players"; // Имя базы данных
            string user = "root"; // Имя пользователя
            string password = "toor"; // Пароль пользователя
            string Connect = "Database=" + database + ";Datasource=" + host + ";User=" + user + ";Password=" + password;

            MySqlConnection mysql_connection = new MySqlConnection(Connect);
            MySqlCommand cmd = mysql_connection.CreateCommand();
            cmd.CommandText = "SELECT nickname,score FROM players WHERE nickname=" + "'" + Nick_name + "'";
            mysql_connection.Open();
            MySqlDataReader mysql_result = cmd.ExecuteReader();
            if (mysql_result.Read())
            {
                if (mysql_result.GetInt32(1) < MyPhone.Score)
                {
                    mysql_result.Close();
                    cmd.CommandText = $"UPDATE players SET score={(int)MyPhone.Score} WHERE nickname='{Nick_name}'";
                    //Console.WriteLine("Name = {0} Score={1}", Nick_name, MyPhone.Score);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                mysql_result.Close();
                cmd.CommandText = "INSERT INTO players (nickname, score) VALUES(" + "'" + Nick_name + "'" + ", " + "'" + MyPhone.Score.ToString() + "'" + ")";
                cmd.ExecuteNonQuery();
            }
            mysql_connection.Close();
        }

        void Key_is_pressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                player_right = false;
                player_left = true;
            }
            if (e.Key == Key.Right)
            {
                player_left = false;
                player_right = true;
            }
            if (e.Key == Key.Space)
            {
                if (paused)
                    paused = false;
                else
                    paused = true;
            }
        }

        void Key_is_up(object sender, KeyEventArgs e)
        {
            player_right = false;
            player_left = false;
        }
        async void Game_over()
        {
            mycanvas.Children.Remove(Player);
            for (int i = 0; i < fallen_elements.Length; i++)
            {
                if (fallen_elements[i] != null)
                {
                    mycanvas.Children.Remove(fallen_elements[i].Get_obj());
                }
            }
            mycanvas.Background = System.Windows.Media.Brushes.Black;
            System.Windows.Shapes.Rectangle lose_table = new System.Windows.Shapes.Rectangle();
            lose_table.Width = 750;
            lose_table.Height = 300;
            Canvas.SetLeft(lose_table, My_window.Width / 2 - lose_table.Width / 2 - 50);
            Canvas.SetTop(lose_table, My_window.Height / 2 - lose_table.Height / 2 - 50);
            ImageSource mybrush = new BitmapImage(new Uri("C:\\C#\\Coursework\\bin\\Debug\\Media\\Game_over.png"));
            ImageBrush My_brush = new ImageBrush(mybrush);
            lose_table.Fill = My_brush;
            mycanvas.Children.Add(lose_table);
            await Task.Delay(3000);
            this.Close();
        }
    }
}
