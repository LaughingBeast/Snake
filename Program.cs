using System.Runtime.CompilerServices;

namespace Snake
{
    internal class Program
    {
        public static List<string[]> Arena = new List<string[]>();
        public static List<Body> BodyList = new List<Body>();


        public static Player player;

        public static int Score = 0;

        public static bool GameOn = false;

        public enum Direction { Up, Down, Left, Right, }

        static void Main(string[]  args)
        {
            if (!GameOn)
            {
                WordGenarete(100, 25);

                Console.WriteLine("ARENA 100x25");
               

                Console.ReadLine();
            }

            while (GameOn) 
            {    
                Regenerate();

                string answer = Console.ReadLine();

                switch (answer.ToLower())
                {
                    case "up":
                        Player.PlayerDirection = Direction.Up;
                        break;
                    case "down":
                        Player.PlayerDirection = Direction.Down;
                        break;
                    case "left":
                        Player.PlayerDirection = Direction.Left;
                        break;
                    case "right":
                        Player.PlayerDirection = Direction.Right;
                        break;
                }
            }

            Console.Clear();
            Console.WriteLine("\n GAME OVER");
        }

        static void WordGenarete(int wordWidth,int wordHeight)
        {
            Arena.Clear();

            for (int i = 0; i < wordHeight;i++) //Line Create
            {
                var newLine = new string[wordWidth];

                for (int x = 1; x < newLine.Length - 1; x++)
                {
                    newLine[x] = " ";
                }

                newLine[0] = "|";
                newLine[^1] = "|";
                Arena.Add(newLine);
            }

            #region TOP / BOT Border 
            string[] TopBotBorder = new string[wordWidth];
            
            TopBotBorder[0] = "|";
            TopBotBorder[^1] = "|";

            for (int i = 1;i < TopBotBorder.Length - 1; i++)
            {
                TopBotBorder[i] = "-";
            }

            TopBotBorder[wordWidth / 2] = Score.ToString();

            Arena[0] = TopBotBorder;
            Arena[^1] = TopBotBorder;
            #endregion

            PoitSpawner(new Random().Next(15, 25));

            Regenerate();//First Generate

            GameOn = true;

        }

        static void PoitSpawner(int pointNumber)
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

        }

        static void Regenerate()
        {
            Console.Clear();

            PlayerManager(12, 50);

            if (Score > 9)
            {
                char[] score = Score.ToString().ToCharArray();
                var TopLine = Arena[0];

                TopLine[49] = score[0].ToString();
                TopLine[50] = score[1].ToString();

                Arena[0] = TopLine;
            } else
            {
               var score = Arena[0];
                score[50] = Score.ToString();
            }//Score ReWrite

            foreach (string[] Line in Arena)
            {
                for (int i = 0; i < Line.Length; i++)
                {
                    Console.Write(Line[i]);
                }

                Console.WriteLine();
            } //ArenaLoad
        }

        static void PlayerManager(int playerLayer, int playerPosition)
        {

            if (!GameOn)
            {
                Player.PlayerDirection = Direction.Right;
                player = new Player(playerLayer, playerPosition);
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
            int lastLayer;
            int lastPosition;
           
            foreach (Body bodyPart in BodyList)
            {
                lastLayer = bodyPart.Layer;
                lastPosition = bodyPart.Position;
                bodyPart.Layer = newLayer;
                bodyPart.Position = newPosition;

                var clearSpace = Arena[lastLayer];
                clearSpace[lastPosition] = " ";
                Arena[lastLayer] = clearSpace;

                var redrawSpace = Arena[bodyPart.Layer];
                redrawSpace[bodyPart.Position] = "O";
                Arena[bodyPart.Layer] = redrawSpace;
            } // BodyMove and Write

            player.NewDirection();

            var redrawPlayer = Arena[player.Layer];

            if (redrawPlayer[player.Position] != " ")
            {
                if  (redrawPlayer[player.Position] == "P")
                {
                    player.Lenght++;
                    Score++;
                }
                else { GameOn = false; }
            }
            else { redrawPlayer[player.Position] = "X"; }

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
            public static Direction PlayerDirection { get; set; }

            public Player(int layer, int position)
            {
                Position = position;
                Layer = layer;
                Lenght = 1;

            }

            public void NewDirection()
            {
                switch (Player.PlayerDirection)
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
