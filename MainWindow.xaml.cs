using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MySql.Data.MySqlClient;
using System.Threading;
using static Mysqlx.Notice.Frame.Types;
using System.Data;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Threading;
using System.Timers;

namespace Coursework
{
    public partial class MainWindow : Window
    {
        bool IsToggle_Panel = false;
        bool IsToggle_Recordtable = false;
        bool IsToggle_Rool = false;
        bool Author_Rool = false;
        DispatcherTimer timer = new DispatcherTimer();
        int count_of_colors = 5;
        int curr_color = 0;
        readonly System.Windows.Media.Color[] changing_colors;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += New_nickname;
            Title.Foreground = new SolidColorBrush(Colors.Black);
            changing_colors = new System.Windows.Media.Color[count_of_colors];
            changing_colors[0] = Colors.White;
            changing_colors[1] = Colors.Red;
            changing_colors[2] = Colors.Green;
            changing_colors[3] = Colors.Blue;
            changing_colors[4] = Colors.Violet;
            timer.Interval = System.TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        void Timer_Tick(object sender, EventArgs e)
        {
            ColorAnimation change_color = new ColorAnimation();
            if (curr_color >= count_of_colors - 1)
                curr_color = 0;
            change_color.To = changing_colors[curr_color++];
            change_color.Duration = TimeSpan.FromMilliseconds(500);
            SolidColorBrush backbrush = (SolidColorBrush)Title.Foreground;
            backbrush.BeginAnimation(SolidColorBrush.ColorProperty, change_color);
        }
        void New_nickname(object sender, RoutedEventArgs e)
        {
            Paneloftitles.Width = 0;
            Table_of_players.Height = 0;
            Table_rools.Height = 0;
            Table_of_author.Height = 0;
            DataTable dataTable = new DataTable("Person");
            dataTable.Columns.Add("Имя", typeof(string));
            dataTable.Columns.Add("Cчёт", typeof(int));
            string host = "localhost"; 
            string database = "Players";
            string user = "root";
            string password = "toor";
            string Connect = "Database=" + database + ";Datasource=" + host + ";User=" + user + ";Password=" + password;
            MySqlConnection mysql_connection = new MySqlConnection(Connect);
            MySqlCommand cmd = mysql_connection.CreateCommand();
            cmd.CommandText = "SELECT nickname,score FROM players";
            mysql_connection.Open();
            MySqlDataReader mysql_result = cmd.ExecuteReader();
            if (mysql_result.Read())
            {
                dataTable.Rows.Add(mysql_result.GetString(0), mysql_result.GetInt32(1));
                while (mysql_result.Read())
                {
                    dataTable.Rows.Add(mysql_result.GetString(0), mysql_result.GetInt32(1));
                }
                Table_of_players.ItemsSource = dataTable.DefaultView;
            }
            mysql_result.Close();
        }
        void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Name_of_user.Text.Length > 0)
            {
                Play_window new_win = new Play_window(Name_of_user.Text);
                new_win.Show();
                this.Close();
            }
            else
            {
                Name_of_user.BorderBrush = new SolidColorBrush(Colors.Black);
                ThicknessAnimation dothis = new ThicknessAnimation();
                ColorAnimation change_color = new ColorAnimation();
                change_color.To = Colors.Red;
                change_color.FillBehavior = FillBehavior.Stop;
                change_color.Duration = TimeSpan.FromMilliseconds(100);
                dothis.To = new Thickness(10, 10, 10, 10);
                dothis.Duration = TimeSpan.FromMilliseconds(100);
                dothis.FillBehavior = FillBehavior.Stop;
                Name_of_user.BeginAnimation(TextBox.BorderThicknessProperty, dothis);
                SolidColorBrush borderBrush = (SolidColorBrush)Name_of_user.BorderBrush;
                borderBrush.BeginAnimation(SolidColorBrush.ColorProperty, change_color);
            }
        }
        void Options_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation dothis = new DoubleAnimation();
            if (!IsToggle_Panel)
            {
                dothis.To = 185;
                dothis.Duration = TimeSpan.FromMilliseconds(200);
                Paneloftitles.BeginAnimation(Border.WidthProperty, dothis);
                IsToggle_Panel = true;
            }
            else
            {
                dothis.To = 0;
                dothis.Duration = TimeSpan.FromMilliseconds(200);
                Paneloftitles.BeginAnimation(Border.WidthProperty, dothis);
                IsToggle_Panel = false;
            }
        }

        private void Record_Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation dothis = new DoubleAnimation();
            if (!IsToggle_Recordtable)
            {
                dothis.To = 242;
                dothis.Duration = TimeSpan.FromMilliseconds(200);
                Table_of_players.BeginAnimation(DataGrid.HeightProperty, dothis);
                IsToggle_Recordtable = true;
            }
            else
            {
                dothis.To = 0;
                dothis.Duration = TimeSpan.FromMilliseconds(200);
                Table_of_players.BeginAnimation(DataGrid.HeightProperty, dothis);
                IsToggle_Recordtable = false;
            }
        }

        void Rools_button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation dothis = new DoubleAnimation();
            if (!IsToggle_Rool)
            {
                dothis.To = 224;
                dothis.Duration = TimeSpan.FromMilliseconds(200);
                Table_rools.BeginAnimation(TextBlock.HeightProperty, dothis);
                IsToggle_Rool = true;
            }
            else
            {
                dothis.To = 0;
                dothis.Duration = TimeSpan.FromMilliseconds(200);
                Table_rools.BeginAnimation(TextBlock.HeightProperty, dothis);
                IsToggle_Rool = false;
            }
        }
        void Author_button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation dothis = new DoubleAnimation();
            if (!Author_Rool)
            {
                dothis.To = 53;
                dothis.Duration = TimeSpan.FromMilliseconds(200);
                Table_of_author.BeginAnimation(TextBlock.HeightProperty, dothis);
                Author_Rool = true;
            }
            else
            {
                dothis.To = 0;
                dothis.Duration = TimeSpan.FromMilliseconds(200);
                Table_of_author.BeginAnimation(TextBlock.HeightProperty, dothis);
                Author_Rool = false;
            }
        }
    }
}
