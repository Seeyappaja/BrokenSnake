using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
///█ ■
////https://www.youtube.com/watch?v=SGZgvMwjq2U
namespace Snake
{
    class Program
    {
        public static int maxTimeToReact = 500;
        public static int screenWidth = 32;
        public static int screenHeight = 16;

        static void Main(string[] args)
        {
            Console.WindowHeight = screenHeight;
            Console.WindowWidth = screenWidth;

            Random randomNumber = new Random(); //Used to randomize Berry position
            int gameScore = 5;  //Also dictates Snake's length

            SnakeHeadPixel snakeHead = new SnakeHeadPixel(screenWidth / 2, screenHeight / 2, ConsoleColor.Red);
            string movementInput = "RIGHT";
            List<Tuple<int, int>> snakeBody = new List<Tuple<int, int>>();

            Berry snakeBerry = new Berry();
            RepositionBerry(randomNumber, snakeBerry);

            DrawMap(screenWidth, screenHeight); //Draws outer bounds of game map

            while (true)    //This is the game loop
            {
                if (snakeBerry.xPosition == snakeHead.xPosition && snakeBerry.yPosition == snakeHead.yPosition)
                {
                    gameScore++;
                    RepositionBerry(randomNumber, snakeBerry);
                }
                DeleteSnakeTail(snakeBody, gameScore);  //Deletes the tip of Snake's tail, both in List and visually
                DrawPixel(snakeHead.xPosition, snakeHead.yPosition, snakeHead.pixelColor);  //Draw new head position
                snakeBody.Add(new Tuple<int, int>(snakeHead.xPosition, snakeHead.yPosition));//Create new Head Tuple
                if (CheckWallCollision(snakeBody) || CheckHeadOnBodyCollision(snakeBody))   //Check for Border/Body collisions + Redraw Snake Body 
                {
                    ResolveGameOver(gameScore);
                    break;
                }
                DrawPixel(snakeBerry.xPosition, snakeBerry.yPosition, ConsoleColor.Cyan);   //Draws Berry last to overlay on the Snake Body
                movementInput = GetMovementInput(DateTime.Now, movementInput);
                ChangeSnakeHeadPosition(movementInput, snakeHead);
            }
        }
        private static void DeleteSnakeTail(List<Tuple<int, int>> snakeBody, int gameScore)
        {
            if (snakeBody.Count >= gameScore)   //Tail is deleted only when needed
            {
                Console.SetCursorPosition(snakeBody[0].Item1, snakeBody[0].Item2);
                Console.Write(" ");
                snakeBody.RemoveAt(0);
            }
        }
        private static void DrawPixel(int pixelPositionX, int pixelPositionY, ConsoleColor pixelColor)
        {
            Console.SetCursorPosition(pixelPositionX, pixelPositionY);
            Console.ForegroundColor = pixelColor;
            Console.Write("■");
        }
        private static void ChangeSnakeHeadPosition(string movementInput, SnakeHeadPixel snakeHead)
        {
            switch (movementInput)
            {
                case "UP":
                    snakeHead.yPosition--;
                    break;
                case "DOWN":
                    snakeHead.yPosition++;
                    break;
                case "LEFT":
                    snakeHead.xPosition--;
                    break;
                case "RIGHT":
                    snakeHead.xPosition++;
                    break;
            }
        }
        private static string GetMovementInput(DateTime timeAtStart, string oldMovementInput)
        {
            string newMovementInput = oldMovementInput;
            while (true)
            {
                if (DateTime.Now.Subtract(timeAtStart).TotalMilliseconds > maxTimeToReact) { return newMovementInput; }
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo playerKeyInformation = Console.ReadKey(true);
                    if (playerKeyInformation.Key.Equals(ConsoleKey.UpArrow) && oldMovementInput != "DOWN")
                    {
                        newMovementInput = "UP";
                    }
                    else if (playerKeyInformation.Key.Equals(ConsoleKey.DownArrow) && oldMovementInput != "UP")
                    {
                        newMovementInput = "DOWN";
                    }
                    else if (playerKeyInformation.Key.Equals(ConsoleKey.LeftArrow) && oldMovementInput != "RIGHT")
                    {
                        newMovementInput = "LEFT";
                    }
                    else if (playerKeyInformation.Key.Equals(ConsoleKey.RightArrow) && oldMovementInput != "LEFT")
                    {
                        newMovementInput = "RIGHT";
                    }
                    else
                    {
                        newMovementInput = oldMovementInput;
                    }
                }
            }
        }
        private static void ResolveGameOver(int gameScore)
        {
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Game over, Score: " + gameScore);
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2 + 1);
        }
        private static bool CheckWallCollision(List<Tuple<int, int>> snakeBody)
        {
            if (snakeBody[snakeBody.Count - 1].Item1 == screenWidth - 1 || snakeBody[snakeBody.Count - 1].Item1 == 0
            || snakeBody[snakeBody.Count - 1].Item2 == screenHeight - 1 || snakeBody[snakeBody.Count - 1].Item2 == 0)
            {
                return true;
            }
            return false;
        }
        private static bool CheckHeadOnBodyCollision(List<Tuple<int, int>> snakeBody)
        {
            if (snakeBody.Count() > 1)
            {
                for (int i = 0; i < snakeBody.Count() - 1; i++)
                {
                    DrawPixel(snakeBody[i].Item1, snakeBody[i].Item2, ConsoleColor.Green);

                    if (snakeBody[snakeBody.Count() - 1].Item1 == snakeBody[i].Item1 && snakeBody[snakeBody.Count() - 1].Item2 == snakeBody[i].Item2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static void RepositionBerry(Random randomNumber, Berry snakeBerry)
        {
            snakeBerry.xPosition = randomNumber.Next(1, screenWidth - 1);
            snakeBerry.yPosition = randomNumber.Next(1, screenHeight - 1);
        }
        private static void DrawMap(int widthOfMap, int heightOfMap)
        {
            for (int i = 0; i < screenWidth; i++)   //Draw Outer bounds on the X axis
            {
                DrawPixel(i, 0, ConsoleColor.Gray);
                DrawPixel(i, screenHeight - 1, ConsoleColor.Gray);
            }

            for (int i = 1; i < screenHeight; i++)  //Draw Outer bounds on the Y axis
            {
                DrawPixel(0, i, ConsoleColor.Gray);
                DrawPixel(screenWidth - 1, i, ConsoleColor.Gray);
            }
        }
        class SnakeHeadPixel
        {
            public int xPosition { get; set; }
            public int yPosition { get; set; }
            public ConsoleColor pixelColor { get; set; }

            public SnakeHeadPixel(int xPosition, int yPosition, ConsoleColor pixelColor)
            {
                this.xPosition = xPosition;
                this.yPosition = yPosition;
                this.pixelColor = pixelColor;
            }
        }
        class Berry
        {
            public int xPosition { set; get; }
            public int yPosition { set; get; }
        }
    }
}