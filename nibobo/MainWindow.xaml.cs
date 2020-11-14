using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private Board m_puzzle;

        public MainWindow()
        {
            InitializeComponent();
            Board emptyBoard = new Board();
            DrawBoard(PuzzleCanvas, emptyBoard);
            DrawBoard(AnswerCanvas, emptyBoard);
            MsgBox.Text = "nibobo solver is ready.\nYou may generate a puzzle and ask me to solve it.";
        }

        private void DrawExampleBoard1()
        {
            Board b1 = new Board();
            b1.PlaceBlock(BlockFactory.GetBlockByName("G"), 0, 0, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("C"), 0, 2, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("J"), 0, 4, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("I"), 0, 5, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("F"), 0, 8, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("K"), 1, 1, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("L"), 1, 5, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("E"), 3, 0, 1);
            b1.PlaceBlock(BlockFactory.GetBlockByName("H"), 3, 3, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("A"), 4, 0, 1);
            b1.PlaceBlock(BlockFactory.GetBlockByName("D"), 6, 0, 4);
            b1.PlaceBlock(BlockFactory.GetBlockByName("B"), 6, 1, 5);
            DrawBoard(PuzzleCanvas, b1);
        }

        private void DrawExampleBoard2()
        {
            Board b1 = new Board();
            b1.PlaceBlock(BlockFactory.GetBlockByName("A"), 0, 0, 4);
            b1.PlaceBlock(BlockFactory.GetBlockByName("E"), 0, 1, 6);
            b1.PlaceBlock(BlockFactory.GetBlockByName("J"), 0, 2, 1);
            b1.PlaceBlock(BlockFactory.GetBlockByName("F"), 0, 6, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("H"), 0, 7, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("G"), 1, 3, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("D"), 2, 4, 4);
            b1.PlaceBlock(BlockFactory.GetBlockByName("K"), 2, 5, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("C"), 3, 0, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("I"), 4, 1, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("L"), 5, 1, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("B"), 7, 0, 2);
            DrawBoard(AnswerCanvas, b1);
        }

        /// <summary>
        /// Draw game board in Canvas
        /// </summary>
        /// <param name="board">the board to draw</param>
        private void DrawBoard(Canvas canvas, Board board)
        {
            DrawBoardEdge(canvas);
            foreach (PlacedBlock pb in board.m_blocks)
            {
                DrawBlock(canvas, pb);
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10 - i; j++)
                {
                    if (board.m_index[i, j] == 0)
                    {
                        DrawCircle(canvas, i, j, System.Drawing.Color.LightGray);
                    }
                }
            }
        }

        /// <summary>
        /// Draw block on game board.
        /// </summary>
        /// <param name="block"></param>
        private void DrawBlock(Canvas canvas, PlacedBlock pb)
        {
            Block b = pb.m_block;
            Position pos = pb.m_position;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (pb[i, j] == 1)
                    {
                        int x = pos.x + i;
                        int y = pos.y + j;
                        DrawCircle(canvas, x, y, b.m_color);
                        if (i + 1 < 4 && pb[i + 1, j] == 1)
                        {
                            DrawLine(canvas, x, y, x + 1, y, b.m_color);
                        }
                        if (j + 1 < 4 && pb[i, j + 1] == 1)
                        {
                            DrawLine(canvas, x, y, x, y + 1, b.m_color);
                        }
                    }
                }
            }
        }

        private void DrawLine(Canvas canvas, int x1, int y1, int x2, int y2, System.Drawing.Color m_color)
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
            canvas.Children.Add(line);
        }

        private void DrawCircle(Canvas canvas, int x, int y, System.Drawing.Color m_color)
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
            canvas.Children.Add(circle);
            circle.SetValue(Canvas.TopProperty, (double)x * BLOCK_SIZE);
            circle.SetValue(Canvas.LeftProperty, (double)y * BLOCK_SIZE);
        }

        /// <summary>
        /// Draw game board edge and background.
        /// </summary>
        private void DrawBoardEdge(Canvas canvas)
        {
            Polygon polygon = new Polygon()
            {
                Stroke = Brushes.DarkGray,
                Fill = Brushes.DarkGray,
                StrokeThickness = EDGE_SIZE,

            };
            polygon.Points = new PointCollection() { new Point(-5, -5), new Point(-5, BLOCK_SIZE * 11), new Point(BLOCK_SIZE * 11, -5) };
            canvas.Children.Add(polygon);
        }

        private static Board GetExamplePuzzle1()
        {
            Board b1 = new Board();
            b1.PlaceBlock(BlockFactory.GetBlockByName("G"), 0, 0, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("C"), 0, 2, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("J"), 0, 4, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("I"), 0, 5, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("F"), 0, 8, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("K"), 1, 1, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("L"), 1, 5, 0);
            b1.PlaceBlock(BlockFactory.GetBlockByName("E"), 3, 0, 1);
            b1.PlaceBlock(BlockFactory.GetBlockByName("H"), 3, 3, 0);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("A"), 4, 0, 1);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("D"), 6, 0, 4);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("B"), 6, 1, 5);
            return b1;
        }

        private void SolveBoardInGUI(Board b1)
        {
            DateTime startTime = DateTime.Now;
            DrawBoard(PuzzleCanvas, b1);
            MsgBox.Text = startTime.ToString() + " solving puzzle...\n";
            Board b = b1.Solve();
            DateTime endTime = DateTime.Now;
            if (b != null)
            {
                DrawBoard(AnswerCanvas, b);
                MsgBox.Text += endTime.ToString() + " solved\n";
            }
            else
            {
                MsgBox.Text += endTime.ToString() + " No solution found\n";
            }
        }

        private static Board GetExamplePuzzle2()
        {
            Board b1 = new Board();
            b1.PlaceBlock(BlockFactory.GetBlockByName("G"), 0, 0, 2);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("C"), 0, 2, 0);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("J"), 0, 4, 0);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("I"), 0, 5, 2);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("F"), 0, 8, 2);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("K"), 1, 1, 0);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("L"), 1, 5, 0);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("E"), 3, 0, 1);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("H"), 3, 3, 0);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("A"), 4, 0, 1);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("D"), 6, 0, 4);
            //b1.PlaceBlock(BlockFactory.GetBlockByName("B"), 6, 1, 5);
            return b1;
        }

        private void GeneratePuzzleButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO handle exception when input is not valid.
            int numberOfBlocksOnBoard;
            try
            {
                numberOfBlocksOnBoard = Int32.Parse(BlocksOnBoardTextBox.Text);
                if (numberOfBlocksOnBoard < 0 || numberOfBlocksOnBoard > 10)
                {
                    MsgBox.Text = "number of blocks on board should be integer 0-10";
                    return;
                }
            }
            catch (FormatException)
            {
                MsgBox.Text = "number of blocks on board should be integer 0-10";
                return;
            }
            m_puzzle = GetExamplePuzzle2();
            DrawBoard(PuzzleCanvas, m_puzzle);
        }

        private void SolvePuzzleButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_puzzle == null)
            {
                MsgBox.Text = "Please generate puzzle first";
                return;
            }
            SolveBoardInGUI(m_puzzle);
        }
    }
}
