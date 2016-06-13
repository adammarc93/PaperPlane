using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace PaperPlane
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Player> BoxScorses { get; set; }
        public Player User { get; set; }
        public Plane UserPlane { get; set; }
        public Obstacle ObstacleBird { get; set; }
        public Obstacle ObstacleCloud { get; set; }

        private enum distanceSpeedChange { reallySlow = 1, slow = 900, medium = 800, quiteFast = 700, fast = 600, theFastest = 500 }

        private enum Status { mainManu, gamePlay, boxScorses }
        private Status gameStatus { get; set; }

        private System.Windows.Threading.DispatcherTimer planeTimer;
        private System.Windows.Threading.DispatcherTimer birdTimer;
        private System.Windows.Threading.DispatcherTimer cloudTimer;
        private System.Windows.Threading.DispatcherTimer distanceTimer;

        private static MediaPlayer soundtrack;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            BoxScorses = new ObservableCollection<Player>();
            XmlFileToList();

            NickTextBox.Focus();
            SetSoundtrack();

            User = new Player();

            UserPlane = new Plane();
            UserPlane.GetDimensions(this.PlaneImage);
            UserPlane.SetImage(this.PlaneImage);

            ObstacleBird = new Obstacle(this.BirdImage);
            ObstacleBird.SetImage(this.BirdImage);

            ObstacleCloud = new Obstacle(this.CloudImage);
            ObstacleCloud.SetImage(this.CloudImage);

            gameStatus = Status.mainManu;
        }

        //Method stop the timmers couses game elements movement, change grids visibilty, 
        //plane and obstacles position and etc. after game losing.
        private void gameLoss()
        {
            planeTimer.Stop();
            birdTimer.Stop();
            cloudTimer.Stop();
            distanceTimer.Stop();

            GamePlayGrid.Visibility = Visibility.Hidden;
            FinalScoreLabel.Content = UserPlane.Distance.ToString() + " m";
            GameOver.Visibility = Visibility.Visible;

            UserPlane.SetStartPosition();
            ObstacleBird.SetStartPosition();
            ObstacleCloud.SetStartPosition();

            UserPlane.DrawImage();
            ObstacleBird.DrawImage();
            ObstacleCloud.DrawImage();

            User.Nick = this.NickTextBox.Text;
            User.Score = UserPlane.Distance;

            BoxScorses.Add(User);
            ListToXmlFile();

            UserPlane.Distance = 0;
            gameStatus = Status.mainManu;
        }

        //Method create timer responsible for measure of the distance overcome by plane.
        private void DistanceTimer()
        {
            distanceTimer = new System.Windows.Threading.DispatcherTimer();
            distanceTimer.Tick += new EventHandler(DistanceTimer_Tick);
            SetInterval();
            distanceTimer.Start();
        }

        //Every timer's tick change distance value.
        private void DistanceTimer_Tick(object sender, EventArgs e)
        {
            SetInterval();
            UserPlane.Distance++;
            ScoreLabel.Content = UserPlane.Distance.ToString() + " m";
        }

        //With the increase of the distance, the timmer's interval value decrease. 
        private void SetInterval()
        {
            if (UserPlane.Distance < (int)planeDistance.reallyNotFar)
                distanceTimer.Interval = new TimeSpan(0, 0, 0, (int)distanceSpeedChange.reallySlow);
            else if (UserPlane.Distance >= (int)planeDistance.reallyNotFar && UserPlane.Distance < (int)planeDistance.notFar)
                distanceTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)distanceSpeedChange.slow);
            else if (UserPlane.Distance >= (int)planeDistance.notFar && UserPlane.Distance < (int)planeDistance.quiteFar)
                distanceTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)distanceSpeedChange.medium);
            else if (UserPlane.Distance >= (int)planeDistance.quiteFar && UserPlane.Distance < (int)planeDistance.far)
                distanceTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)distanceSpeedChange.quiteFast);
            else if (UserPlane.Distance >= (int)planeDistance.far && UserPlane.Distance < (int)planeDistance.reallyFar)
                distanceTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)distanceSpeedChange.fast);
            else if (UserPlane.Distance >= (int)planeDistance.reallyFar)
                distanceTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)distanceSpeedChange.theFastest);
        }

        //Method causes the float plane by pressing space key.
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && gameStatus == Status.gamePlay)
            {
                if (UserPlane.OnCanvasMin() == true)
                    UserPlane.Flying();
            }
        }

        //Method start timmer, which causes plane image falling down.
        private void PlaneTimer()
        {
            planeTimer = new System.Windows.Threading.DispatcherTimer();
            planeTimer.Tick += new EventHandler(PlaneTimer_Tick);
            planeTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            planeTimer.Start();
        }

        //Method couses plane falling. On every timer tick, plane image
        //falls down.
        private void PlaneTimer_Tick(object sender, EventArgs e)
        {
            if (UserPlane.OnCanvasMax() == true)
                UserPlane.Falling();
        }

        //Method create timer responsible for bird movement.
        private void BirdTimer()
        {
            birdTimer = new System.Windows.Threading.DispatcherTimer();
            birdTimer.Tick += new EventHandler(BirdTimer_Tick);
            birdTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            birdTimer.Start();
        }

        //Every timer's tick change bird's position value.
        private void BirdTimer_Tick(object sender, EventArgs e)
        {
            if (ObstacleBird.OnCanvas() == true)
            {
                ObstacleBird.Flying(UserPlane.Distance);

                if (UserPlane.Collison(ObstacleBird.PositionX, ObstacleBird.PositionY,
                    ObstacleBird.Width, ObstacleBird.Height) == true)
                    gameLoss();
            }
            else
                ObstacleBird.SetStartPosition();
        }

        //Analogously as described above (BirdTimer() and BirdTomer_Tick()).
        private void CloudTimer()
        {
            cloudTimer = new System.Windows.Threading.DispatcherTimer();
            cloudTimer.Tick += new EventHandler(CloudTimer_Tick);
            cloudTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            cloudTimer.Start();
        }

        private void CloudTimer_Tick(object sender, EventArgs e)
        {
            if (ObstacleCloud.OnCanvas() == true)
            {
                ObstacleCloud.Flying(UserPlane.Distance);

                if (UserPlane.Collison(ObstacleCloud.PositionX, ObstacleCloud.PositionY,
                    ObstacleCloud.Width, ObstacleCloud.Height) == true)
                    gameLoss();
            }
            else
                ObstacleCloud.SetStartPosition();
        }

        private static void SetSoundtrack()
        {
            Uri uri = new Uri("../../Sounds/Soundtrack.mp3", UriKind.Relative);
            soundtrack = new MediaPlayer();
            soundtrack.Open(uri);
            soundtrack.Play();
            soundtrack.MediaEnded += Soundtrack_MediaEnded;
        }

        //Method loop the soundtrack.
        private static void Soundtrack_MediaEnded(object sender, EventArgs e)
        {
            soundtrack.Position = TimeSpan.Zero;
            soundtrack.Play();
        }

        //Method couses serialization List to xml file.
        private void ListToXmlFile()
        {
            string filePath = "BoxScorses";

            using (var sw = new StreamWriter(filePath))
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<Player>));
                serializer.Serialize(sw, this.BoxScorses);
            }
        }

        //Method couses eserialization xml file to List.
        private void XmlFileToList()
        {
            string filename = "BoxScorses";

            using (var sr = new StreamReader(filename))
            {
                var deserializer = new XmlSerializer(typeof(ObservableCollection<Player>));
                ObservableCollection<Player> tmpList = (ObservableCollection<Player>)deserializer.Deserialize(sr);

                foreach (var item in tmpList)
                {
                    this.BoxScorses.Add(item);
                }
            }
        }

        #region Button_Clicks
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (NickTextBox.Text != "")
            {
                MainMenuGrid.Visibility = Visibility.Hidden;
                GamePlayGrid.Visibility = Visibility.Visible;

                PlaneTimer();
                BirdTimer();
                CloudTimer();
                DistanceTimer();

                gameStatus = Status.gamePlay;
            }
            else
                MessageBox.Show("First you have to enter the nick!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BoxScorsesButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Hidden;
            BoxScorsesGrid.Visibility = Visibility.Visible;
            gameStatus = Status.boxScorses;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BoxScorsesGrid.Visibility = Visibility.Hidden;
            MainMenuGrid.Visibility = Visibility.Visible;
            gameStatus = Status.mainManu;
        }

        private void BackToMainManuButton_Click(object sender, RoutedEventArgs e)
        {
            GameOver.Visibility = Visibility.Hidden;
            MainMenuGrid.Visibility = Visibility.Visible;
            gameStatus = Status.mainManu;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.BoxScorses.RemoveAt(this.BoxScorsesListView.SelectedIndex);
                ListToXmlFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("First you have to select score you want to delete.", "Delete Score");
            }
        }

        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You sure?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.BoxScorses.Clear();
                ListToXmlFile();
            }
        }
        #endregion

        //Do not work!
        private void SortCollection()
        {
            var sortedList = new ObservableCollection<Player>(BoxScorses.OrderBy(User => User.Score));
            BoxScorses = sortedList;
        }
    }
}
