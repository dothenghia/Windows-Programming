using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

/*
MAIN FLOW :
- New Game -> RenderGrid -> Cell_Click -> CheckWin 
- Resize Window -> RenderGrid
- Restart Game -> RenderGrid

 
 
 
*/


namespace Ass02_21127367
{
    public partial class MainWindow : Window
    {
        public Game game;

        public MainWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            game = new Game(Main_Canvas);
            game.RenderGrid(12, 12); // Default when the program starts
            SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            game.RenderGrid(game.gridCells.GetLength(0), game.gridCells.GetLength(1));
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                game.MoveSelectedCell("Left");
            }
            else if (e.Key == Key.Right)
            {
                game.MoveSelectedCell("Right");
            }
            else if (e.Key == Key.Up)
            {
                game.MoveSelectedCell("Up");
            }
            else if (e.Key == Key.Down)
            {
                game.MoveSelectedCell("Down");
            }
            //else if (e.Key == Key.Enter)
            //{
            //    game.PlaceMark();
            //}
        }

    }
}
