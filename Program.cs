using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Snake
{
    internal class Program
    {
        public static List<string[]> Arena = new List<string[]>();
        public static List<Body> BodyList = new List<Body>();

        public static Player player;

        public static int Score = 0;
        public static int BestScore;
        public static int NumberOfPoints;
        public static int PlayerLenght;

        public static float TimePerTickBacic = 300f;
        public static float TimePerTick;

        public static StreamWriter Writer;
        public static StreamReader Reader;

        public static bool GameOn = false;

        public enum Direction { Up, Down, Left, Right, }

        static void Main(string[]  args)
        {
            if (!GameOn)
            {
                GenerateWorld(100, 25);
                TimePerTick = TimePerTickBacic;

                try
                {
                    Reader = new StreamReader("score.txt");

                    if (Reader != null)
                    {
                        BestScore = int.Parse(Reader.ReadLine());
                    }

                    Reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            while (GameOn) 
            {    
               if (TimePerTick <= 0)
               {
                    ReWrite();
                    TimePerTick = TimePerTickBacic;
               }
               else { TimePerTick -= 0.1f; }

               if (Console.KeyAvailable)
               {
                    var key = Console.ReadKey(true);
 
                    switch (key.Key)
                    {
                        case ConsoleKey.W:
                            player.PlayerDirection = Direction.Up;
                            break;
                        case ConsoleKey.S:
                            player.PlayerDirection = Direction.Down; 
                            break;
                        case ConsoleKey.A:
                            player.PlayerDirection = Direction.Left;
                            break; 
                        case ConsoleKey.D:
                            player.PlayerDirection = Direction.Right;
                            break;
                    }
               }

                if (NumberOfPoints <= 0)
                {
                    GameOn = false;
                }
            }

            if (Score > BestScore)
            {
                try
                {
                    Writer = new StreamWriter("score.txt");

                    if (Writer != null)
                    {
                        Writer.WriteLine(Score.ToString());
                    }

                    Writer.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                   BestScore = Score;
                }
            }

            Console.Clear();

            if (NumberOfPoints <= 0)
            {
                Console.WriteLine("\n YOU WIN");
            }
            else
            {
                Console.WriteLine("\n GAME OVER");
            }
            
            Console.WriteLine($"\n SCORE: {Score}");
            Console.WriteLine($"\n BEST SCORE: {BestScore}");
        }

        static void GenerateWorld(int worldWidth, int worldHeight)
        {
            Arena.Clear();

            for (int i = 0; i < worldHeight;i++) //Line Create
            {
                var newLine = new string[worldWidth];

                for (int x = 1; x < newLine.Length - 1; x++)
                {
                    newLine[x] = " ";
                }

                newLine[0] = "|";
                newLine[^1] = "|";
                Arena.Add(newLine);
            }

            #region TOP / BOT Border 
            string[] TopBotBorder = new string[worldWidth];
            
            TopBotBorder[0] = "|";
            TopBotBorder[^1] = "|";

            for (int i = 1;i < TopBotBorder.Length - 1; i++)
            {
                TopBotBorder[i] = "-";
            }

            TopBotBorder[worldWidth / 2] = Score.ToString();

            Arena[0] = TopBotBorder;
            Arena[^1] = TopBotBorder;
            #endregion

            SpawnPoits(new Random().Next(25, 40));

            ReWrite();//First Generate

            GameOn = true;
        }

        static void SpawnPoits(int pointNumber)
        {
            int i = 0;

            while (i < pointNumber)
            {
                int randomLine = new Random().Next(1, 24);
                int randomPosition = new Random().Next(1,99);

                var line = Arena[randomLine];
                line[randomPosition] = "P";
                Arena[randomLine] = line;
                i++;
            }

            NumberOfPoints = pointNumber;
        }

        static void ReWrite()
        {
            Console.Clear();

            ManageMovement(12, 50);

            if (Score > 9)
            {
                char[] score = Score.ToString().ToCharArray();
                var TopLine = Arena[0];

                TopLine[49] = score[0].ToString();
                TopLine[50] = score[1].ToString();

                Arena[0] = TopLine;
            } 
            else
            {
               var score = Arena[0];
                score[50] = Score.ToString();
            }//Score ReWrite

            foreach (string[] Line in Arena)
            {
                for (int i = 0; i < Line.Length; i++)
                {
                    if(Line[i].ToString() == "P")
                    {
                        Console.ForegroundColor = ConsoleColor.Green; 
                    }else if (Line[i].ToString() == "X" || Line[i].ToString() == "O")
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    Console.Write(Line[i]);
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine();
            } //ArenaLoad
        }

        static void ManageMovement(int playerLayer, int playerPosition)
        {
            if (!GameOn)
            { 
                player = new Player(playerLayer, playerPosition);
                player.PlayerDirection = Direction.Right;

                var layer = Arena[player.Layer];
                layer[player.Position] = "X";
                layer[player.Position - 1] = "O";
                BodyList.Add(new Body(player.Layer, player.Position - 1));
            }
            else
            {
                Move();
            }
        }

        static void Move()
        {
            int newLayer = player.Layer;
            int newPosition = player.Position;
            int lastLayer = 0;
            int lastPosition = 0;
           
            foreach (Body bodyPart in BodyList)
            {
                lastLayer = bodyPart.Layer;
                lastPosition = bodyPart.Position;

                bodyPart.Layer = newLayer;
                bodyPart.Position = newPosition;

                newLayer = lastLayer;
                newPosition = lastPosition;

                var redrawSpace = Arena[bodyPart.Layer];
                redrawSpace[bodyPart.Position] = "O";
                Arena[bodyPart.Layer] = redrawSpace;
            } // BodyMove and Write

            var clearSpace = Arena[lastLayer];
            clearSpace[lastPosition] = " ";
            Arena[lastLayer] = clearSpace;

            player.NewDirection();

            var redrawPlayer = Arena[player.Layer];

            if (redrawPlayer[player.Position] != " ")
            {
                if (redrawPlayer[player.Position] == "P")
                {
                    player.Lenght++;
                    Score++;
                    redrawPlayer[player.Position] = "X";
                    NumberOfPoints--;
                    BodyList.Add(new Body(BodyList[^1].Layer, BodyList[^1].Position));
                }
                else { GameOn = false; }
            }
            else 
            { redrawPlayer[player.Position] = "X"; }

            Arena[player.Layer] = redrawPlayer;
        }

        public class Snake
        {
            public int Position { get; set; }
            public int Layer { get; set; }
        }

        public class Player : Snake
        {
            public int Lenght { get; set; }
            public Direction PlayerDirection { get; set; }

            public Player(int layer, int position)
            {
                Position = position;
                Layer = layer;
                Lenght = 1;
                PlayerLenght = Lenght;
            }

            public void NewDirection()
            {
                switch (player.PlayerDirection)
                {
                    case Direction.Up:
                        player.Layer--;
                        break;
                    case Direction.Down:
                        player.Layer++;
                        break;
                    case Direction.Left:
                        player.Position--;
                        break;
                    case Direction.Right:
                        player.Position++;
                        break;
                }
            }
        }

        public class Body : Snake
        {
            public Body(int layer, int position)
            {
                Layer = layer;
                Position = position;
            }
        }
    }
}
