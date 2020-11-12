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

namespace nibobo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const int BLOCK_SIZE = 40;
        const int CIRCLE_SIZE = 35;
        const int LINE_SIZE = 10;
        const int EDGE_SIZE = 2;

        public MainWindow()
        {
            InitializeComponent();
            Board board = new Board();
            Block blockA = BlockFactory.GetBlockByName("A");
            Block blockJ = BlockFactory.GetBlockByName("J");
            board.PlaceBlock(blockA, 0, 1);
            board.PlaceBlock(blockJ, 0, 0);
            DrawBoard(board);
        }

        /// <summary>
        /// Draw game board in Canvas
        /// </summary>
        /// <param name="board">the board to draw</param>
        private void DrawBoard(Board board)
        {
            DrawBoardEdge();
            foreach (PlacedBlock pb in board.m_blocks)
            {
                DrawBlock(pb);
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10 - i; j++)
                {
                    if (board.m_index[i, j] == 0)
                    {
                        DrawCircle(i, j, System.Drawing.Color.LightGray);
                    }
                }
            }
        }

        /// <summary>
        /// Draw block on game board.
        /// </summary>
        /// <param name="block"></param>
        private void DrawBlock(PlacedBlock pb)
        {
            Block b = pb.block;
            Position pos = pb.position;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (b.m_index[i, j] == 1)
                    {
                        int x = pos.x + i;
                        int y = pos.y + j;
                        DrawCircle(x, y, b.m_color);
                        if (i + 1 < 4 && b.m_index[i + 1, j] == 1)
                        {
                            DrawLine(x, y, x + 1, y, b.m_color);
                        }
                        if (j + 1 < 4 && b.m_index[i, j + 1] == 1)
                        {
                            DrawLine(x, y, x, y + 1, b.m_color);
                        }
                    }
                }
            }
        }

        private void DrawLine(int x1, int y1, int x2, int y2, System.Drawing.Color m_color)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(m_color.A, m_color.R, m_color.G, m_color.B));
            Line line = new Line()
            {
                X1 = (double)(y1 * BLOCK_SIZE + BLOCK_SIZE / 2 - LINE_SIZE / 2 + LINE_SIZE / 3.0),
                Y1 = (double)(x1 * BLOCK_SIZE + BLOCK_SIZE / 2 - LINE_SIZE / 2 + LINE_SIZE / 3.0),
                X2 = (double)(y2 * BLOCK_SIZE + BLOCK_SIZE / 2 - LINE_SIZE / 2 + LINE_SIZE / 3.0),
                Y2 = (double)(x2 * BLOCK_SIZE + BLOCK_SIZE / 2 - LINE_SIZE / 2 + LINE_SIZE / 3.0),
                Stroke = brush,
                StrokeThickness = LINE_SIZE
            };
            BoardCanvas.Children.Add(line);
        }

        private void DrawCircle(int x, int y, System.Drawing.Color m_color)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(m_color.A, m_color.R, m_color.G, m_color.B));
            Ellipse circle = new Ellipse()
            {
                Width = CIRCLE_SIZE,
                Height = CIRCLE_SIZE,
                Stroke = brush,
                Fill = brush,
                StrokeThickness = 1
            };
            BoardCanvas.Children.Add(circle);
            circle.SetValue(Canvas.TopProperty, (double)x * BLOCK_SIZE);
            circle.SetValue(Canvas.LeftProperty, (double)y * BLOCK_SIZE);
        }

        /// <summary>
        /// Draw game board edge and background.
        /// </summary>
        private void DrawBoardEdge()
        {
            Line line = new Line()
            {
                X1 = (double)(-5),
                Y1 = (double)(-5),
                X2 = (double)(-5),
                Y2 = (double)(BLOCK_SIZE * 11),
                Stroke = Brushes.Black,
                StrokeThickness = EDGE_SIZE
            };
            BoardCanvas.Children.Add(line);
            line = new Line()
            {
                X1 = (double)(-5),
                Y1 = (double)(-5),
                X2 = (double)(BLOCK_SIZE * 11),
                Y2 = (double)(-5),
                Stroke = Brushes.Black,
                StrokeThickness = EDGE_SIZE
            };
            BoardCanvas.Children.Add(line);
            line = new Line()
            {
                X1 = (double)(BLOCK_SIZE * 11),
                Y1 = (double)(-5),
                X2 = (double)(-5),
                Y2 = (double)(BLOCK_SIZE * 11),
                Stroke = Brushes.Black,
                StrokeThickness = EDGE_SIZE
            };
            BoardCanvas.Children.Add(line);
        }
    }
}
