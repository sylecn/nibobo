using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace nibobo
{
    internal enum GUIState
    {
        /// <summary>
        /// User can generate puzzle, or solve the default empty puzzle.
        /// </summary>
        FREE,
        /// <summary>
        /// User has clicked auto generate puzzle button, game engine will generate a board with given number of pieces.
        /// </summary>
        AUTO_GENERATE,
        /// <summary>
        /// User has clicked manual generate puzzle button, user can now place block on puzzle board.
        /// </summary>
        MANUAL_GENERATE,
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const int BLOCK_SIZE = 40;
        const int CIRCLE_SIZE = 35;
        const int LINE_SIZE = 10;
        const int EDGE_SIZE = 2;
        const string VERSION = "1.0.1";
        const int NUMBER_OF_ANSWERS_TO_CALCULATE_AT_A_TIME = 10;
        private static readonly Board EMPTY_BOARD = new Board();
        private static readonly System.Drawing.Color EMPTY_CELL_COLOR = System.Drawing.Color.LightGray;
        private static readonly System.Drawing.Color HIGHLIGHT_CELL_COLOR = System.Drawing.Color.DeepSkyBlue;
        /// <summary>
        /// Stores board shown in puzzle board.
        /// </summary>
        private Board m_puzzle;
        private GUIState m_guiState = GUIState.FREE;
        /// <summary>
        /// Stores highlighted cells when manual generate puzzle board.
        /// </summary>
        private List<Position> m_highlightPositions = new List<Position>();

        CancellationTokenSource m_tokenSource = new CancellationTokenSource();
        /// <summary>
        /// For thread support, calculate at most this many answers at a time.
        /// </summary>
        private BlockingCollection<Board> m_answersQueue;
        /// <summary>
        /// keep the answers that solver returns.
        /// </summary>
        private List<Board> m_answers = new List<Board>();
        private int m_currentAnswerIndex = -1;
        private bool m_allSolutionFetched = false;

        public MainWindow()
        {
            InitializeComponent();
            m_puzzle = new Board();
            DrawBoard(PuzzleCanvas, EMPTY_BOARD);
            DrawBoard(AnswerCanvas, EMPTY_BOARD);
            MsgBox.Text = string.Format("nibobo solver {0}\n" +
                "To solve the empty board, click Solve Puzzle button.\n" +
                "To solve a puzzle that is on exercise book, click Manual Generate button and place the blocks on the puzzle board.\n" +
                "You may also auto generate a puzzle and solve it.\n", VERSION);
        }

        private void DrawExampleBoard1()
        {
            DrawBoard(PuzzleCanvas, Board.GetExampleBoard1());
        }

        private void DrawExampleBoard2()
        {
            DrawBoard(AnswerCanvas, Board.GetExampleBoard2());
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
                        DrawCircle(canvas, i, j, EMPTY_CELL_COLOR);
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
            circle.MouseDown += delegate (object sender, MouseButtonEventArgs e)
            {
                cellClick(sender, e, new Position(x, y));
            };
            canvas.Children.Add(circle);
            circle.SetValue(Canvas.TopProperty, (double)x * BLOCK_SIZE);
            circle.SetValue(Canvas.LeftProperty, (double)y * BLOCK_SIZE);
        }

        /// <summary>
        /// Event handler when user click a cell when generating puzzle manully.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="pos"></param>
        private void cellClick(object sender, MouseButtonEventArgs e, Position pos)
        {
            Ellipse circle = (Ellipse)sender;
            // only allow click on puzzle canvas.
            if (circle.Parent != PuzzleCanvas)
            {
                return;
            }
            if (m_guiState != GUIState.MANUAL_GENERATE)
            {
                return;
            }
            Debug.WriteLine("User click on cell {0}", pos);
            // do not allow click a cell if it is already covered by a block.
            if (m_puzzle.m_index[pos.x, pos.y] == 1)
            {
                return;
            }
            // toggle highlight status
            if (m_highlightPositions.Contains(pos))
            {
                DrawCircle(PuzzleCanvas, pos.x, pos.y, EMPTY_CELL_COLOR);
                m_highlightPositions.Remove(pos);
            }
            else
            {
                DrawCircle(PuzzleCanvas, pos.x, pos.y, HIGHLIGHT_CELL_COLOR);
                m_highlightPositions.Add(pos);
            }
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

        private void SolveBoardInGUI(Board b1)
        {
            DateTime startTime = DateTime.Now;
            DrawBoard(PuzzleCanvas, b1);
            MsgBox.Text = startTime.ToString() + " Solving puzzle...\n";
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

        private void SolveBoardInGUINonBlocking(Board b1)
        {
            DateTime startTime = DateTime.Now;
            DrawBoard(PuzzleCanvas, b1);
            MsgBox.Text = startTime.ToString() + " Solving puzzle...\n";
            CancellationToken ct = m_tokenSource.Token;
            m_answersQueue = new BlockingCollection<Board>(NUMBER_OF_ANSWERS_TO_CALCULATE_AT_A_TIME);
            Task solver = Task.Run(() =>
            {
                b1.Solve(m_answersQueue, ct);
            }, m_tokenSource.Token);

            NextSolution_Click(null, null);
        }

        private void AutoGeneratePuzzleButton_Click(object sender, RoutedEventArgs e)
        {
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

            Debug.Assert(m_guiState == GUIState.FREE);
            m_guiState = GUIState.AUTO_GENERATE;
            AutoGeneratePuzzleButton.IsEnabled = false;
            SolvePuzzleButton.IsEnabled = false;
            PreviousSolution.IsEnabled = false;
            NextSolution.IsEnabled = false;

            m_puzzle = GetExamplePuzzle3();
            DrawBoard(PuzzleCanvas, m_puzzle);
            DrawBoard(AnswerCanvas, EMPTY_BOARD);

            m_guiState = GUIState.FREE;
            AutoGeneratePuzzleButton.IsEnabled = true;
            SolvePuzzleButton.IsEnabled = true;
            PreviousSolution.IsEnabled = false;
            NextSolution.IsEnabled = false;
        }

        private Board GetExamplePuzzle3()
        {
            Board b1 = new Board();
            b1.PlaceBlock(BlockFactory.GetBlockByName("F"), 0, 0, 2);
            b1.PlaceBlock(BlockFactory.GetBlockByName("C"), 2, 0, 2);
            return b1;
        }

        private void SolvePuzzleButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_puzzle == null)
            {
                MsgBox.Text = "Please generate puzzle first";
                return;
            }
            SolvePuzzleButton.IsEnabled = false;
            //SolveBoardInGUI(m_puzzle);
            m_answers.Clear();
            m_currentAnswerIndex = -1;
            m_allSolutionFetched = false;
            SolutionNumberLabel.Content = "Solving...";
            SolveBoardInGUINonBlocking(m_puzzle);
        }

        /// <summary>
        /// Start generate a puzzle by placing block on game board manually. To place a block, just click all cells of that block on game board, then click
        /// Next Block button or Finish Placement button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualGeneratePuzzleButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(m_guiState == GUIState.FREE);
            //m_tokenSource.Cancel();
            m_guiState = GUIState.MANUAL_GENERATE;
            m_puzzle = new Board();
            DrawBoard(PuzzleCanvas, m_puzzle);
            DrawBoard(AnswerCanvas, m_puzzle);
            m_highlightPositions.Clear();
            AutoGeneratePuzzleButton.IsEnabled = false;
            SolvePuzzleButton.IsEnabled = false;
            PreviousSolution.IsEnabled = false;
            NextSolution.IsEnabled = false;
            NextBlockButton.IsEnabled = true;
            RemoveLastBlockButton.IsEnabled = true;
            FinishPlacementButton.IsEnabled = true;
            MsgBox.Text = "Click puzzle board cells to highlight them to form a placed block.\nClick Next Block button to confirm.\nClick Finish placement to confirm puzzle.\n";
        }

        private void FinishPlacementButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(m_guiState == GUIState.MANUAL_GENERATE);
            m_guiState = GUIState.FREE;
            AutoGeneratePuzzleButton.IsEnabled = true;
            SolvePuzzleButton.IsEnabled = true;
            PreviousSolution.IsEnabled = false;
            NextSolution.IsEnabled = false;
            NextBlockButton.IsEnabled = false;
            RemoveLastBlockButton.IsEnabled = false;
            FinishPlacementButton.IsEnabled = false;
            MsgBox.Text = "Click Solve Puzzle to solve it.";
            SolvePuzzleButton.IsEnabled = true;
        }

        /// <summary>
        /// If cells in m_highlightPositions forms one block, add block in m_puzzle, and draw the block. Otherwise, do nothing.
        /// </summary>
        /// <returns>PlacedBlock if block is found, null otherwise</returns>
        private PlacedBlock FindAndHighlightBlockMaybe()
        {
            if (m_highlightPositions.Count == 0)
            {
                return null;
            }
            int minX = m_highlightPositions.Select(pos => pos.x).Min();
            int minY = m_highlightPositions.Select(pos => pos.y).Min();
            int maxX = m_highlightPositions.Select(pos => pos.x).Max();
            int maxY = m_highlightPositions.Select(pos => pos.y).Max();
            if (maxX - minX > 3 || maxY - minY > 3)
            {
                return null;
            }
            int[,] hlBlockIndex = new int[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (m_highlightPositions.Contains(new Position(minX + i, minY + j)))
                    {
                        hlBlockIndex[i, j] = 1;
                    }
                }
            }
            foreach (string name in BlockFactory.m_blockNames)
            {
                Block b = BlockFactory.GetBlockByName(name);
                for (int i = 0; i < b.m_varients.Count; i++)
                {
                    if (BlockFactory.ArrayEquals(b.m_varients[i], hlBlockIndex))
                    {
                        PlacedBlock pb = new PlacedBlock(b, i, minX, minY);
                        return pb;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// If highlighted cells can form a block, add PlacedBlock in m_puzzle and clear m_highlightPositions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextBlockButton_Click(object sender, RoutedEventArgs e)
        {
            PlacedBlock pb = FindAndHighlightBlockMaybe();
            if (pb == null)
            {
                MsgBox.Text = "Invalid Block. Please double check highlighted cells.\n";
                return;
            }
            if (m_puzzle.m_blocks.Any(e => e.m_block.m_name == pb.m_block.m_name))
            {
                MsgBox.Text = "Duplicate Block. Please double check highlighted cells.\n";
                return;
            }
            Debug.WriteLine("Found {0}", pb);
            m_puzzle.PlaceBlock(pb.m_block, pb.m_position.x, pb.m_position.y, pb.m_varient);
            m_highlightPositions.Clear();
            DrawBoard(PuzzleCanvas, m_puzzle);
        }

        private void RemoveLastBlockButton_Click(object sender, RoutedEventArgs e)
        {
            m_puzzle.RemoveLastBlock();
            DrawBoard(PuzzleCanvas, m_puzzle);
        }

        private void ShowSavedSolution()
        {
            DrawBoard(AnswerCanvas, m_answers[m_currentAnswerIndex]);
            if (m_currentAnswerIndex > 0)
            {
                PreviousSolution.IsEnabled = true;
            }
            SolutionNumberLabel.Content = string.Format("Solution {0}", m_currentAnswerIndex + 1);
        }

        private void NextSolution_Click(object sender, RoutedEventArgs e)
        {
            DateTime endTime = DateTime.Now;
            Board b;
            if (m_answers.Count > m_currentAnswerIndex + 1)
            {
                m_currentAnswerIndex++;
                ShowSavedSolution();
                return;
            }
            if (m_allSolutionFetched)
            {
                NextSolution.IsEnabled = false;
                return;
            }

            Task<Board>.Factory.StartNew(() =>
            {
                try
                {
                    Board b = m_answersQueue.Take();
                    return b;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }).ContinueWith(t =>
            {
                Board b = t.Result;
                DateTime endTime = DateTime.Now;
                if (b == null)
                {
                    m_allSolutionFetched = true;
                    NextSolution.IsEnabled = false;
                    if (m_currentAnswerIndex == -1)
                    {
                        SolutionNumberLabel.Content = "No solution!";
                        MsgBox.Text += string.Format("{0} No solution found\n", endTime);
                        PreviousSolution.IsEnabled = false;
                    }
                    else
                    {
                        MsgBox.Text += string.Format("{0} All solutions fetched\n", endTime);
                    }
                }
                else
                {
                    m_answers.Add(b);
                    m_currentAnswerIndex++;
                    ShowSavedSolution();
                    NextSolution.IsEnabled = true;
                    if (m_currentAnswerIndex == 0)
                    {
                        // only show timestamp for first solution.
                        MsgBox.Text += string.Format("{0} Solution {1}\n", endTime, m_currentAnswerIndex + 1);
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void PreviousSolution_Click(object sender, RoutedEventArgs e)
        {
            if (m_currentAnswerIndex > 0)
            {
                DateTime endTime = DateTime.Now;
                m_currentAnswerIndex--;
                DrawBoard(AnswerCanvas, m_answers[m_currentAnswerIndex]);
                NextSolution.IsEnabled = true;
                if (m_currentAnswerIndex == 0)
                {
                    PreviousSolution.IsEnabled = false;
                }
                SolutionNumberLabel.Content = string.Format("Solution {0}", m_currentAnswerIndex + 1);
            }
        }
    }
}
