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
        
        static void Main(string[] args)
        {
            const int maxTimeToReact = 500;
            const int screenWidth = 32;
            const int screenHeight = 16;

            Console.WindowHeight = screenHeight;
            Console.WindowWidth = screenWidth;

            Random randomNumber = new Random(); //Used to randomize Berry position
            uint gameScore = 5;  //Also dictates Snake's length

            SnakeHead snakeHead = new SnakeHead(Console.WindowWidth / 2, Console.WindowHeight / 2, ConsoleColor.Red);
            string movementInput = "RIGHT";
            List<SnakeBody> snakeBody = new List<SnakeBody>();

            Berry snakeBerry = new Berry();
            RepositionBerry(randomNumber, snakeBerry);

            DrawMap(); //Draws outer bounds of game map

            while (true)    //This is the game loop
            {
                if (snakeBerry.xPosition == snakeHead.xPosition && snakeBerry.yPosition == snakeHead.yPosition)
                {
                    gameScore++;
                    RepositionBerry(randomNumber, snakeBerry);
                }
                DeleteSnakeTail(snakeBody, gameScore);  //Deletes the tip of Snake's tail, both in List and visually
                DrawPixel(snakeHead.xPosition, snakeHead.yPosition, snakeHead.pixelColor);  //Draw new head position
                snakeBody.Add(new SnakeBody(snakeHead.xPosition, snakeHead.yPosition)); //Include new Head in body list
                if (CheckWallCollision(snakeBody) || CheckHeadOnBodyCollision(snakeBody))   //Check for Border/Body collisions + Redraw Snake Body 
                {
                    ResolveGameOver(gameScore);
                    break;
                }
                DrawPixel(snakeBerry.xPosition, snakeBerry.yPosition, ConsoleColor.Cyan);   //Draws Berry last to overlay on the Snake Body
                movementInput = GetMovementInput(DateTime.Now, movementInput,maxTimeToReact);
                ChangeSnakeHeadPosition(movementInput, snakeHead);
            }
        }
        private static void DeleteSnakeTail(List<SnakeBody> snakeBody, uint gameScore)
        {
            if (snakeBody.Count >= gameScore)   //Tail is deleted only when needed
            {
                Console.SetCursorPosition(snakeBody[0].xPosition, snakeBody[0].yPosition);
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
        private static void ChangeSnakeHeadPosition(string movementInput, SnakeHead snakeHead)
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
        private static string GetMovementInput(DateTime timeAtStart, string oldMovementInput, int timeToReact)
        {
            string newMovementInput = oldMovementInput;
            while (DateTime.Now.Subtract(timeAtStart).TotalMilliseconds < timeToReact)
            {
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
            return newMovementInput;
        }
        private static void ResolveGameOver(uint gameScore)
        {
            Console.SetCursorPosition(Console.WindowWidth / 5, Console.WindowHeight / 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Game over, Score: " + (gameScore - 5));
            Console.SetCursorPosition(Console.WindowWidth / 5, Console.WindowHeight / 2 + 1);
        }
        private static bool CheckWallCollision(List<SnakeBody> snakeBody)
        {
            if (snakeBody[snakeBody.Count - 1].xPosition == Console.WindowWidth - 1 || snakeBody[snakeBody.Count - 1].xPosition == 0
            || snakeBody[snakeBody.Count - 1].yPosition == Console.WindowHeight - 1 || snakeBody[snakeBody.Count - 1].yPosition == 0)
            {
                return true;
            }
            return false;
        }
        private static bool CheckHeadOnBodyCollision(List<SnakeBody> snakeBody)
        {
            if (snakeBody.Count() > 1)
            {
                for (int i = 0; i < snakeBody.Count() - 1; i++)
                {
                    DrawPixel(snakeBody[i].xPosition, snakeBody[i].yPosition, ConsoleColor.Green);

                    if (snakeBody[snakeBody.Count() - 1].xPosition == snakeBody[i].xPosition && snakeBody[snakeBody.Count() - 1].yPosition == snakeBody[i].yPosition)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static void RepositionBerry(Random randomNumber, Berry snakeBerry)
        {
            snakeBerry.xPosition = randomNumber.Next(1, Console.WindowWidth - 1);
            snakeBerry.yPosition = randomNumber.Next(1, Console.WindowHeight - 1);
        }
        private static void DrawMap()
        {
            for (int i = 0; i < Console.WindowWidth; i++)   //Draw Outer bounds on the X axis
            {
                DrawPixel(i, 0, ConsoleColor.Gray);
                DrawPixel(i, Console.WindowHeight - 1, ConsoleColor.Gray);
            }

            for (int i = 1; i < Console.WindowHeight; i++)  //Draw Outer bounds on the Y axis
            {
                DrawPixel(0, i, ConsoleColor.Gray);
                DrawPixel(Console.WindowWidth - 1, i, ConsoleColor.Gray);
            }
        }
        class SnakeHead
        {
            public int xPosition { get; set; }
            public int yPosition { get; set; }
            public ConsoleColor pixelColor { get; set; }

            public SnakeHead(int xPosition, int yPosition, ConsoleColor pixelColor)
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

        class SnakeBody
        {
            public int xPosition { set; get; }
            public int yPosition { set; get; }

            public SnakeBody(int xPosition, int yPosition)
            {
                this.xPosition = xPosition;
                this.yPosition = yPosition;
            }
        }
    }
}