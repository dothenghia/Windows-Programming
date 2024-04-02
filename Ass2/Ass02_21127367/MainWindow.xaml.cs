using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ass02_21127367
{
    public partial class MainWindow : Window
    {
        public Game game;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Game game = new Game(Main_Canvas);
            game.RenderGrid(12, 12); // Default when the program starts
            SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            game.RenderGrid(game.gridCells.GetLength(0), game.gridCells.GetLength(1));
        }



    }
}