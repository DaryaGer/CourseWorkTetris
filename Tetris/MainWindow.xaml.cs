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
using System.Windows.Threading;

namespace Tetris
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	/// 
	public class Board
	{
		private int Rows;
		private int Cols;
		private int Score;
		private int LinesFilled;
		private Tetriss CurrentTetris;
		private Label[,] BlockControls;

		static private Brush NoBrush = Brushes.Transparent;
		static private Brush SilverBrush = Brushes.Gray;

		public Board(Grid TetrisGrid)
		{
			Rows = TetrisGrid.RowDefinitions.Count;
			Cols = TetrisGrid.ColumnDefinitions.Count;
			Score = 0;
			LinesFilled = 0;

			BlockControls = new Label[Cols,Rows];
			for (int i = 0; i < Cols; i++)
				for (int j = 0; j < Rows; j++)
				{
					BlockControls[i, j] = new Label();
					BlockControls[i, j].Background = NoBrush;
					BlockControls[i, j].BorderBrush = SilverBrush;
					Grid.SetRow(BlockControls[i, j], j);
					Grid.SetColumn(BlockControls[i, j], i);
					TetrisGrid.Children.Add(BlockControls[i, j]);
				}
			CurrentTetris = new Tetriss();
		}

		public int getScore()
		{
			return Score;
		}

		public int getLines()
		{
			return LinesFilled;
		}
		private void CurrentTetrisDraw()
		{
			Point Position = CurrentTetris.getCurrentPosition();
			Point[] Shape = CurrentTetris.getCurrentShape();
			Brush Color = CurrentTetris.getCurrentColor();
			foreach (Point S in Shape)
			{
				BlockControls[(int)(S.X + Position.X) + ((Cols / 2) - 1),
					(int)(S.Y + Position.Y) + 2].Background = Color;
			}
		}

		private void CurrentTetrisErase()
		{
			Point Position = CurrentTetris.getCurrentPosition();
			Point[] Shape = CurrentTetris.getCurrentShape();
			foreach (Point S in Shape)
			{
				BlockControls[(int)(S.X + Position.X) + ((Cols / 2) - 1),
					(int)(S.Y + Position.Y) + 2].Background = NoBrush;
			}
		}
		
		private void CheckRows()
		{
			bool full;
			for (int i = Rows - 1; i < 0; i--)
			{
				full = true;
				for (int j = 0; j < Cols; j++)
				{
					if (BlockControls[i, j].Background == NoBrush)
						full = false;
				}
			if (full)
				{
					RemoveRow(i);
					Score += 100;
					LinesFilled++;
				}
			}
		}

		private void RemoveRow(int Row)
		{
			for (int i = Row; i > 2; i--)
				for (int j=0;j<Cols;j++)
			{
					BlockControls[i, j].Background = BlockControls[j, i - 1].Background;
			}
		}

		public void CurrentTetrisMoveLeft()
		{
			Point Position = CurrentTetris.getCurrentPosition();
			Point[] Shape = CurrentTetris.getCurrentShape();
			bool Move = true;
			CurrentTetrisErase();
			foreach (Point S in Shape)
			{
				if ((int)(S.X + Position.X) + ((Cols / 2) - 1) < 0)
					Move = false;
				else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1) - 1), (int)(S.Y + Position.Y) + 2].Background != NoBrush)
					Move = false;
			}
			if (Move)
			{
				CurrentTetris.MoveLeft();
				CurrentTetrisDraw();
			}
			else
				CurrentTetrisDraw();
		}

		public void CurrentTetrisMoveRight()
		{
			Point Position = CurrentTetris.getCurrentPosition();
			Point[] Shape = CurrentTetris.getCurrentShape();
			bool Move = true;
			foreach (Point S in Shape)
			{
				if ((int)(S.X + Position.X) + ((Cols / 2) + 1) >= Cols)
					Move = false;
				else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1) + 1), (int)(S.Y + Position.Y) + 2].Background != NoBrush)
					Move = false;
			}
			if (Move)
			{
				CurrentTetris.MoveRight();
				CurrentTetrisDraw();
			}
			else
				CurrentTetrisDraw();
		}

		public void CurrentTetrisMoveDown()
		{
			Point Position = CurrentTetris.getCurrentPosition();
			Point[] Shape = CurrentTetris.getCurrentShape();
			bool Move = true;
			CurrentTetrisErase();
			foreach (Point S in Shape)
			{
				if (((int)(S.Y + Position.Y) + 2+1) >=Rows)
					Move = false;
				else if (BlockControls[(int)(S.X + Position.X) + ((Cols / 2) - 1) , (int)(S.Y + Position.Y) + 2 +1].Background != NoBrush)
					Move = false;
			}
			if (Move)
			{
				CurrentTetris.MoveDown();
				CurrentTetrisDraw();
			}
			else
			{
				CurrentTetrisDraw();
				CheckRows();
				CurrentTetris = new Tetriss();
			}
		}

		public void CurrentTetrisMoveRotate()
		{
			Point Position = CurrentTetris.getCurrentPosition();
			Point[] S = new Point[4];
			Point[] Shape = CurrentTetris.getCurrentShape();
			bool Move = true;
			Shape.CopyTo(S,0);
			CurrentTetrisErase();
			for (int i = 0; i < S.Length; i++)
			{
				double x = S[i].X;
				S[i].X = S[i].Y * -1;
				S[i].Y = x;
				if (((int)((S[i].Y + Position.Y) + 2)) >= Rows)
				{
					Move = false;
				}
				else if (((int)(S[i].X + Position.X) + ((Cols / 2) - 1)) < 0)
				{
					Move = false;
				}
				else if (((int)(S[i].X + Position.X) + ((Cols / 2) - 1)) >= Rows)
				{
					Move = false;
				}
				else if (BlockControls[((int)(S[i].X + Position.X) + ((Cols / 2) - 1)), (int)(S[i].Y + Position.Y) + 2].Background != NoBrush)
				{
					Move = false;
				}
			}
			if (Move)
			{
				CurrentTetris.MoveRotate();
				CurrentTetrisDraw();
			}
			else CurrentTetrisDraw();
		}
	}
	
	public class Tetriss
	{

		private Point CurrentPosition;
		private Point[] CurrentShape;
		private Brush CurrentColor;
		private bool Rotate;

		public Tetriss()
		{
			CurrentPosition = new Point(0, 0);
			CurrentColor = Brushes.Transparent;
			CurrentShape = setRandomShape();
		}

		public Brush getCurrentColor()
		{
			return CurrentColor;
		}

		public Point getCurrentPosition()
		{
			return CurrentPosition;
		}

		public Point[] getCurrentShape()
		{
			return CurrentShape;
		}

		public void MoveLeft()
		{
			CurrentPosition.X--;
		}

		public void MoveRight()
		{
			CurrentPosition.X++;
		}

		public void MoveDown()
		{
			CurrentPosition.Y++;
		}

		public void MoveRotate()
		{
			if (Rotate)
			{
				for (int i = 0; i < CurrentShape.Length; i++)
				{
					double x = CurrentShape[i].X;
					CurrentShape[i].X = CurrentShape[i].Y * -1;
					CurrentShape[i].Y = x;
				}
			}
		}

		private Point[] setRandomShape()
		{
			Random rand = new Random();
			switch (rand.Next() % 7)
			{
				case 0:
					Rotate = true;
					CurrentColor = Brushes.LightBlue;
					return new Point[]
						{
							new Point(0,0),
							new Point(-1,0),
							new Point(1,0),
							new Point(2,0)
						};
				case 1:
					Rotate = true;
					CurrentColor = Brushes.Yellow;
					return new Point[]
						{
							new Point(0,0),
							new Point(-1,0),
							new Point(1,0),
							new Point(1,1)
						};
				case 2:
					Rotate = true;
					CurrentColor = Brushes.Brown;
					return new Point[]
						{
							new Point(1,-1),
							new Point(-1,0),
							new Point(0,0),
							new Point(1,0)
						};
				case 3:
					Rotate = true;
					CurrentColor = Brushes.Green;
					return new Point[]
						{
							new Point(0,0),
							new Point(-1,0),
							new Point(0,-1),
							new Point(-1,-1)
						};
				case 4:
					Rotate = true;
					CurrentColor = Brushes.Red;
					return new Point[]
						{
							new Point(0,0),
							new Point(-1,0),
							new Point(0,1),
							new Point(1,1)
						};
				case 5:
					Rotate = false;
					CurrentColor = Brushes.Blue;
					return new Point[]
						{
							new Point(0,0),
							new Point(0,1),
							new Point(1,0),
							new Point(1,1)
						};
				case 6:
					Rotate = true;
					CurrentColor = Brushes.Purple;
					return new Point[]
						{
							new Point(0,0),
							new Point(-1,0),
							new Point(0,-1),
							new Point(1,0)
						};

				default: return null;
			}

		}
	}

	public partial class MainWindow : Window
	{
		DispatcherTimer Timer;
		Board myBoard;
		
		public MainWindow()
		{
			InitializeComponent();
		Timer = new DispatcherTimer();
			Timer.Tick += new EventHandler(GameTick);
			Timer.Interval = new TimeSpan(0, 0, 0, 0, 400);}

		void MainWindow_Initilizer(object sender, EventArgs e)
		{
			
			GameStart();
		}

		private void GameStart()
		{
			TetrisGrid.Children.Clear();
			myBoard = new Board(TetrisGrid);
			Timer.Start();
		}

		void GameTick(object semder, EventArgs e)
		{
			Score.Text = myBoard.getScore().ToString("0000000000");
			Lines.Text = myBoard.getLines().ToString("0000");
			myBoard.CurrentTetrisMoveDown();
		}
		private void HandleKeyDown(object Sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Left:
					if (Timer.IsEnabled) myBoard.CurrentTetrisMoveLeft();
					break;
				case Key.Right:
					if (Timer.IsEnabled) myBoard.CurrentTetrisMoveRight();
					break;
				case Key.Down:
					if (Timer.IsEnabled) myBoard.CurrentTetrisMoveDown();
					break;
				case Key.Up:
					if (Timer.IsEnabled) myBoard.CurrentTetrisMoveRotate();
					break;
				case Key.S:
					GameStart();
					break;
				default: break;
			}

		}

		private void ButtonClick(object Sender, RoutedEventArgs e)
		{
			GameStart();	
		}
	}
}

