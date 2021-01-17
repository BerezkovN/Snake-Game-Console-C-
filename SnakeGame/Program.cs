using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace Snake_Game
{
	struct Body //Head and tail
	{
		public int x, y;
	}

	public static class Program
	{
		//Optimal size (7 ≤ heigth,width ≤ 27) if your font size is 28
		static int height = 29, width = 29; //You can change this variables (there can be not square) 
		static int center_allign = ((Console.WindowWidth / 2) - ((width * 2 + 1) / 2)); //It needs to print our game in center allign
		static int size_empty = (height - 2) * (width - 2);
		static int count_body = 3;
		static int speed = 115; //Thread.Sleep(speed)
		static int[][] field; //I used jagged array, because it is the easiest way (Look at Boundaries())
		static char[] bound = { '─', '│', '┌', '┐', '└', '┘' };
		static Body head, tail;
		static ConsoleKey current_direct;
		static List<ConsoleKey> commands = new List<ConsoleKey>(); //Cleaner

		public static void Main()
		{
			//Settings
			Console.CursorVisible = false;
			Console.OutputEncoding = System.Text.Encoding.UTF8; //It needs to print ▲

			if (center_allign < 0)
				Error();
			else
				Snake();

			Console.ReadLine();
		}

		static void Snake()
		{
			PrintBoundaries();

			/*Body of snake*/
			head = new Body { x = (int)Math.Floor((double)width / 2), y = (int)Math.Floor((double)height / 2) - 1 };
			tail = new Body { x = head.x, y = head.y + 2 };
			current_direct = ConsoleKey.UpArrow;

			/*Initializating field*/
			Boundaries(InitializeArray());
			SetSnake();
			SetApple();
			for (int i = 0; i < count_body - 1; i++) //Becuse cleaner don't know how many it need to remove
				commands.Add(ConsoleKey.UpArrow);

			/*Waiting player*/
			Waiting("Press any key to start");

			/*Moving*/

			//Variables
			int temp = 0;
			bool isapple = false;
			bool isgameover = false;
			ConsoleKey current_key = current_direct;

			while (!isgameover)
			{
				Thread.Sleep(speed);

				//Checking input
				if (Console.KeyAvailable)
					current_key = Console.ReadKey(true).Key;

				if (current_key != current_direct && CheckTurn(current_key)) //If we pressed some key
				{
					CheckDirect(current_key, ref head);
					commands.Add(current_key);
					current_direct = current_key;
				}
				else //If we don't do nothing
				{
					CheckDirect(current_direct, ref head);
					commands.Add(current_direct);
				}

				//Checking encounter
				temp = field[head.y][head.x];

				switch (temp)
				{
					//If you touch your body or bound
					case 1:
					case 2:
						isgameover = true;
						break;
					//If you touch apple
					case 3:
						isapple = true;
						break;
				}

				//Set head
				field[head.y][head.x] = 2;

				//Visualization of ↑
				PrintChar(head.x, head.y, '■');

				//Clear tail
				if (!isapple)
				{
					//Visualization of ↓
					PrintChar(tail.x, tail.y, ' ');

					//Cleaner
					field[tail.y][tail.x] = 0;
					CheckDirect(commands[0], ref tail);
					commands.RemoveAt(0);
				}
				else //If snake eats apple, we don't use cleaner
				{
					isapple = false;
					SetApple();
					count_body++;
				}

				if (count_body == size_empty)
					break;
			}

			//End
			Console.Clear();
			if (isgameover) //We need to write two slashes to print one slash
			{
				//strange arrays :)
				//We can't use center allign for string with multiple lines therefore I used array
				string[] gameover = {
					"  _____                                                            ",
					" / ____|                                                           ",
					"| |  __    __ _   _ __ ___     ___      ___   __   __   ___   _ __ ",
					"| | |_ |  / _` | | '_ ` _ \\   / _ \\    / _ \\  \\ \\ / /  / _ \\ | '__|",
					"| |__| | | (_| | | | | | | | |  __/   | (_) |  \\ V /  |  __/ | |   ",
					" \\_____|  \\__,_| |_| |_| |_|  \\___|    \\___/    \\_/    \\___| |_|   "};
				foreach (string s in gameover)
					PrintCenter(s, true);
				Console.WriteLine();
			}
			else
			{
				string[] win = {
					"__     __                               _         ",
					"\\ \\   / /                              (_)        ",
					" \\ \\_/ /    ___    _   _    __      __  _   _ __  ",
					"  \\   /    / _ \\  | | | |   \\ \\ /\\ / / | | | '_ \\ ",
					"   | |    | (_) | | |_| |    \\ V  V /  | | | | | |",
					"   |_|     \\___/   \\__,_|     \\_/\\_/   |_| |_| |_|"
				};

				foreach (string s in win)
					PrintCenter(s, true);
			}

			//Score
			string score = string.Format("Your score: {0}", count_body - 3);
			PrintCenter(score, true);
		}

		static void Waiting(string s)
		{
			string press_any_key = s;

			Console.SetCursorPosition(0, height + 2);
			PrintCenter(press_any_key, false); //New line moves cursor

			Console.ReadKey(true);
			Console.SetCursorPosition(Console.CursorLeft - s.Length, Console.CursorTop);
			Console.Write(new string(' ', s.Length)); //Deleting what void has wrote
			Console.SetCursorPosition(0, 0);

		}

		static void PrintCenter(string s, bool new_line) //Center allign
		{
			Console.SetCursorPosition((Console.WindowWidth / 2) - s.Length / 2, Console.CursorTop);
			Console.Write(s + (new_line ? "\n" : ""));

		}

		static void PrintBoundaries() //I print this one because we don't need to work with this void again
		{
			string str = "",
			horisontal,
			allign;
			int width_str = ((width - 2) * 2) + 1;

			horisontal = new string(bound[0], width_str);

			allign = new string(' ', center_allign);

			str += allign + bound[2] + horisontal + bound[3] + "\n";

			for (int i = 0; i < height - 2; i++)
			{
				str += allign + bound[1];
				for (int j = 0; j < width_str; j++)
					str += " ";
				str += bound[1] + "\n";
			}

			str += allign + bound[4] + horisontal + bound[5];
			Console.WriteLine(str);
		}

		static void SetSnake() //Snake in the beginning
		{
			int x = head.x,
			y = head.y;
			for (int i = 0; i < 3; i++, y++)
			{
				field[y][x] = 2;

				//Visualization of ↑
				PrintChar(x, y, '■');
			}
		}

		static void PrintChar(int x, int y, char c) //Print or remove cube (triangle)
		{
			try
			{
				Console.SetCursorPosition(x * 2 + center_allign, y);
				Console.Write("\b "); //I back up on charepter due to fix bug
				Console.Write(c);
			}
			catch { Error(); }
		}

		static void Boundaries(int[][] initialized) //Setting boundaries by linq
		{
			field = Enumerable.
				Range(0, initialized.Length).
				Select((row, y) =>
					Enumerable.Range(0, width).
					Select((el, x) =>
						((x == 0 || y == 0 ||
							x == width - 1 || y == height - 1) ? 1 : 0)
					).ToArray()
				).ToArray();
		}

		static void SetApple() //Random position of apple
		{
			Random rnd = new Random();
			int rnd_num = rnd.Next(1, size_empty - count_body),
			real_num = 0,
			count = 0,
			x = 0, y = 0;

			for (; y < height - 2 && count != rnd_num; y++)
				for (x = 0; x < width - 2 && count != rnd_num; x++, real_num++)
					if (field[y + 1][x + 1] == 0)
						count++;

			field[y][x] = 3;

			//Visualization
			PrintChar(x, y, '▲');
		}

		static bool CheckTurn(ConsoleKey current_key) //We can't turn back
		{
			if (current_direct == ConsoleKey.UpArrow && current_key == ConsoleKey.DownArrow ||
				current_direct == ConsoleKey.DownArrow && current_key == ConsoleKey.UpArrow)
				return false;
			else if (current_direct == ConsoleKey.LeftArrow && current_key == ConsoleKey.RightArrow ||
				current_direct == ConsoleKey.RightArrow && current_key == ConsoleKey.LeftArrow)
				return false;
			else
				return true;
		}

		static void CheckDirect(ConsoleKey key, ref Body body) //Translating keys
		{
			if (key == ConsoleKey.UpArrow)
				body.y--;
			else if (key == ConsoleKey.DownArrow)
				body.y++;
			else if (key == ConsoleKey.LeftArrow)
				body.x--;
			else if (key == ConsoleKey.RightArrow)
				body.x++;
		}

		static int[][] InitializeArray()
		{
			int[][] ret = new int[height][];
			for (int i = 0; i < height; i++)
				ret[i] = new int[width];
			return ret;
		}

		static void Error()
		{
			Console.Clear();
			Console.WriteLine("Height or width too big");
		}

		static void PrintArray(int[][] arr) //First version of printing
		{
			string str = "";
			foreach (int[] a in arr)
			{
				foreach (int b in a)
					str += a.ToString() + " ";
				str += "\n";
			}
			Console.WriteLine(str);
		}
	}
}
