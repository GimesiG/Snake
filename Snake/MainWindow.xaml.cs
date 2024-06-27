using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            {GridValue.Empty, Images.empty },
            {GridValue.Snake, Images.body },
            {GridValue.Food, Images.food },
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.up, 0},
            {Direction.down, 180},
            {Direction.left, 270},
            {Direction.right, 90}
        };

        private readonly int rows = 25, cols = 25;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = setupGrid();
            gameState = new GameState(rows, cols);
        }

        private async Task runGame()
        {
            draw();
            await showCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await gameLoop();
            await showGameOver();
            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await runGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.gameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.changeDirection(Direction.left);
                    break;
                case Key.Right:
                    gameState.changeDirection(Direction.right);
                    break;
                case Key.Up:
                    gameState.changeDirection(Direction.up);
                    break;
                case Key.Down:
                    gameState.changeDirection(Direction.down);
                    break;
            }
        }

        private async Task gameLoop()
        {
            while (!gameState.gameOver)
            {
                await Task.Delay(100);
                gameState.move();
                draw();
            }
        }

        private Image[,] setupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;   
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private void draw()
        {
            drawGrid();
            drawSnakehead();
            ScoreText.Text = $"SCORE {gameState.score}";
        }

        private void drawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void drawSnakehead()
        {
            Position headPos = gameState.headPosition();
            Image image = gridImages[headPos.row, headPos.column];
            image.Source = Images.head;

            int rotation = dirToRotation[gameState.dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task drawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.snakePosition());

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.deadHead : Images.deadBody;
                gridImages[pos.row, pos.column].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task showCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task showGameOver()
        {
            await drawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press any key to start";
        }
    }
}

