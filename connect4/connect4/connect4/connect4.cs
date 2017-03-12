// Author: Siddharth Surana
// File Name: connect4.cs
// Project Name: connect4
// Creation Date: Dec. 9, 2015
// Modified Date: Dec. 15, 2015
// Description: This program is a two player Connect 4 game where the player clicks a particular column to drop their coin
//              in and then alternates turns to the other player. When 4 colours are placed in a sequence, that player wins.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace connect4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class connect4 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // define the game board background storage variable and bounds
        Texture2D gameBoard;
        Rectangle gameBoardBounds;

        // define the yellow coins image storage variable, x and y coordinate variables and its bound array
        Texture2D yellowCoin;
        int yellowCoinX;
        int yellowCoinY;
        Rectangle[] yellowCoinBounds = new Rectangle[21];

        // define the red coins image storage variable, x and y coordinate variables and its bound array
        Texture2D redCoin;
        int redCoinX;
        int redCoinY;
        Rectangle[] redCoinBounds = new Rectangle[21];

        // define a randomizing variable and a variable to hold
        Random rnd = new Random();
        byte chooseCurrentColour;

        // hold which colour is currently playing and which player just went
        string currentPlayerColour;
        string oldPlayerColour;

        // hold variable that states if the game is running or not
        bool isGameDone = false;

        // store title font type and the titles size
        SpriteFont titleFont;
        Vector2 titleFontBounds = new Vector2(235, 10);

        // bounds for each type of ending game message
        Vector2 redVictoryBounds = new Vector2(122, 200);
        Vector2 yellowVictoryBounds = new Vector2(100, 200);
        Vector2 noVictoryBounds = new Vector2(125, 200);

        // store the current and previous mouse state and mouse position
        MouseState mouse;
        MouseState oldMouse;
        Vector2 mousePos;

        // hold the data of what is stored in each spot of the board in an array 
        string[,] boardSpots = new string[6, 7];

        // variable to hold the background music and state if it is playing or not
        Song backgroundMusic;
        bool isBackgroundMusicPlayed = false;

        // define boards top and left dimentions such as the margins, height, spot diameter and various column start and end coordinates
        const int TOP_MARGIN = 72;
        const int LEFT_MARGIN = 182;
        const int BOARD_HEIGHT = 373;
        const int SPOT_DIAMETER = 62; 
        int colXStart;
        int colXEnd;
        int colYStart;
        int colYEnd;

        // variable to hold the number or coins placed for each colour
        byte placedRedCoins;
        byte placedYellowCoins;

        // hold what the state of the victory is in a variable
        int victoryState;

        // store the amount the coins base y location should change for every coin defined
        int coinMove;
        const int COIN_MOVE_CHANGE = 21;

        public connect4()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // state that the cursor is visable in screen
            IsMouseVisible = true;

            //randonly choose which player colour is going first
            chooseCurrentColour = (byte)rnd.Next(1,3);
            if (chooseCurrentColour == 1)
            {
                currentPlayerColour = "red";
            }
            else
            {
                currentPlayerColour = "yellow";
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load the board image into the game and define its bounds
            gameBoard = Content.Load<Texture2D>(@"Images\Backgrounds\BoardBackground");
            gameBoardBounds = new Rectangle(0, 0, gameBoard.Width, gameBoard.Height);

            // load the background music into the game
            backgroundMusic = Content.Load<Song>(@"Audio\Music\Spectre");

            // load the images of both the red and yellow coins and store it
            yellowCoin = Content.Load<Texture2D>(@"Images\Sprites\YellowCoin");
            redCoin = Content.Load<Texture2D>(@"Images\Sprites\RedCoin");

            // save and store the game font of the game 
            titleFont = Content.Load<SpriteFont>(@"Images\Sprites\TitleFont");

            // define the bounds for each coin by running through each 
            for (int coinCount = 0; coinCount < redCoinBounds.Length; coinCount++)
            {
                yellowCoinBounds[coinCount] = new Rectangle(yellowCoinX, yellowCoinY + coinMove, yellowCoin.Width, yellowCoin.Height);
                redCoinBounds[coinCount] = new Rectangle(Window.ClientBounds.Width - 62, redCoinY + coinMove, redCoin.Width, redCoin.Height);

                coinMove = coinMove + COIN_MOVE_CHANGE;
            }      
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // play background music if it is currently not playing
            if (isBackgroundMusicPlayed == false)
            {
                MediaPlayer.Play(backgroundMusic);
                MediaPlayer.IsRepeating = true; 

                // the music is played on repeat
                isBackgroundMusicPlayed = true;
            }

            // assign current mouse state to oldMouse and get the new mouse state and location
            oldMouse = mouse;
            mouse = Mouse.GetState();
            mousePos = new Vector2(mouse.X, mouse.Y);

            // if the button was just pressed, send parameters of each column to the checkClickLocation procedure
            if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)        
            {
                // define the x start, y start and y end parameters 
                colXStart = LEFT_MARGIN;
                colYStart = TOP_MARGIN;
                colYEnd = TOP_MARGIN + BOARD_HEIGHT;

                // for each column, send the parameters to the CheckClickLocaiton procedure 
                for (byte colNum = 0; colNum <= boardSpots.GetLength(1) - 1; colNum++)
                {
                    // send the parameters to the CheckClickLocation only if the game is still being played
                    if (isGameDone == false)
                    {
                        //find teh x value of the columns end
                        colXEnd = colXStart + SPOT_DIAMETER;

                        // send the parameters to the CheckClickLocaiton procedure
                        CheckClickLocation(colXStart, colXEnd, colYStart, colYEnd, colNum);

                        // assign the new columns x start value
                        colXStart += SPOT_DIAMETER;
                    }
                }
            }

            // check what the previous players coin colour was
            if (currentPlayerColour == "red")
            {
                oldPlayerColour = "yellow";
            }
            else
            {
                oldPlayerColour = "red";
            }

            //check if the previous player completed won the game with a 4 coins in a row horizontally
            if (victoryState != 1 && victoryState != 2 && victoryState != -2)
            {
                victoryState = CheckForHorizontalWin(oldPlayerColour);
            }
            //check if the previous player completed won the game with a 4 coins in a row vertically
            if (victoryState != 1 && victoryState != 2 && victoryState != -2)
            {
                victoryState = CheckForVerticalWin(oldPlayerColour);
            }
            //check if the previous player completed won the game with a 4 coins in a row diagonally
            if (victoryState != 1 && victoryState != 2 && victoryState != -2)
            {
                victoryState = CheckForDiagonalWin(oldPlayerColour);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // start the sprite batch 
            spriteBatch.Begin();

            //draw the gameboard image on the screen
            spriteBatch.Draw(gameBoard, gameBoardBounds, Color.White);

            // draw the title on screen
            spriteBatch.DrawString(titleFont, "Connect 4", titleFontBounds, Color.White);

            //display all the coins on the screen based on what has been placed
            for (int displayEachCoin = 0; displayEachCoin <= yellowCoinBounds.Length - 1; displayEachCoin++)
            {
                spriteBatch.Draw(yellowCoin, yellowCoinBounds[displayEachCoin], Color.White);
                spriteBatch.Draw(redCoin, redCoinBounds[displayEachCoin], Color.White);
            }
          
            //if the red player won, display it to the users
            if (victoryState == 1)
            {
                spriteBatch.DrawString(titleFont, "Red Player Won!", redVictoryBounds, Color.White);
            }
            // if the yellow player won, display it to the users
            else if (victoryState == 2)
            {
                spriteBatch.DrawString(titleFont, "Yellow Player Won!", yellowVictoryBounds, Color.White);
            }
            // if the board is full and no player won, display it to the user
            else if (victoryState == -2)
            {
                spriteBatch.DrawString(titleFont, "No Player Won!", noVictoryBounds, Color.White);
            }

            // end the sprite batch
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // Pre: the current column's start values and end values, and the column number
        // Post: N/A
        // Description: check where the player clicked and based on this, drop a coin in that slot if possible 
        private void CheckClickLocation(int colXStart, int colXEnd, int colYStart, int colYEnd, byte colNum)
        {
            // if the button was pressed inside the bounds from the parameters, enter into the if statement 
            if (mouse.LeftButton == ButtonState.Pressed && 
                mouse.X >= colXStart && 
                mouse.X < colXEnd && 
                mouse.Y >= colYStart && 
                mouse.Y <= colYEnd)
            {
                // run loop for the number of rows that exist on the game board
                for (int rowNum = boardSpots.GetLength(0) - 1; rowNum >= 0; rowNum--)
                {
                    // if the current board spot is empty, input the coin in that location
                    if (boardSpots[rowNum, colNum] == null)
                    {
                        // if the current player is the yellow player, draw a yellow coin in the current board spot and 
                        // add a yellow coin in the data of the current location. Also, switch current player to red
                        if (currentPlayerColour == "yellow")
                        {
                            yellowCoinX = colXStart;
                            yellowCoinY = colYStart + SPOT_DIAMETER * (rowNum);
                            yellowCoinBounds[placedYellowCoins] = new Rectangle(yellowCoinX, yellowCoinY, yellowCoin.Width, yellowCoin.Height);
                            placedYellowCoins++;
                            boardSpots[rowNum, colNum] = "yellow";
                            currentPlayerColour = "red";
                        }
                        // if the current player is the red player, draw a red coin in the current board spot and 
                        // add a red coin in the data of the current location. Also, switch current player to yellow
                        else
                        {
                            redCoinX = colXStart;
                            redCoinY = colYStart + SPOT_DIAMETER * (rowNum);
                            redCoinBounds[placedRedCoins] = new Rectangle(redCoinX, redCoinY, redCoin.Width, redCoin.Height);
                            placedRedCoins++;
                            boardSpots[rowNum, colNum] = "red";
                            currentPlayerColour = "yellow";
                        }

                        // if a coin was dropped, dont check for empty spaces to add any more coins
                        break;
                    }
                }
            }   
        }

        // Pre: the chip colour of the current player
        // Post: the number that represents which player won in a horizontal fashion, if any
        // Description: check if any player won with a 4 coins in a horizontal order 
        private int CheckForHorizontalWin(string colour)
        {
            // define the row end and the column end to check for a win within
            byte rowCheckEnd = 5;
            byte colCheckEnd = 3;

            // run the inside code for the first row to the final row where a horizontal win can exist
            for (byte row = 0; row <= rowCheckEnd; row++)
            {
                // run the inside code for the first column to the final column where a horizontal win can exist
                for (byte column = 0; column <= colCheckEnd; column++)
                {
                    // if there is a horizontal victory, end the game and return which player won
                    if (boardSpots[row, column] == colour && 
                        boardSpots[row, column + 1] == colour && 
                        boardSpots[row, column + 2] == colour && 
                        boardSpots[row, column + 3] == colour)
                    {
                        isGameDone = true;

                        if (colour == "red")
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }
            }

            // if no one won and there are still empty spaces, return a -1
            if ((placedYellowCoins + placedRedCoins) < (yellowCoinBounds.Length + redCoinBounds.Length))
            {
                return -1;
            }
            // if no one won and there are no empty spaces, return a -2 and end game
            else
            {
                isGameDone = true;
                return -2;
            }
        }

        // Pre: the chip colour of the current player
        // Post: the number that represents which player won in a vertical fashion , if any
        // Description: check if any player won with a 4 coins in a vertical order 
        private int CheckForVerticalWin(string colour)
        {
            // define the row end and the column end to check for a win within
            byte rowCheckEnd = 2;
            byte colCheckEnd = 6;

            // run the inside code for the first row to the final row where a vertical win can exist
            for (byte row = 0; row <= rowCheckEnd; row++)
            {
                // run the inside code for the first column to the final column where a vertical win can exist
                for (byte column = 0; column <= colCheckEnd; column++)
                {
                    // if there is a vertical victory, end the game and return which player won
                    if (boardSpots[row, column] == colour &&
                        boardSpots[row + 1, column] == colour &&
                        boardSpots[row + 2, column] == colour &&
                        boardSpots[row + 3, column] == colour)
                    {
                        isGameDone = true;

                        if (colour == "red")
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }
            }

            // if no one won and there are still empty spaces, return a -1
            if ((placedYellowCoins + placedRedCoins) < (yellowCoinBounds.Length + redCoinBounds.Length))
            {
                return -1;
            }
            // if no one won and there are no empty spaces, return a -2 and end game
            else
            {
                isGameDone = true;
                return -2;
            }
        }

        // Pre: the chip colour of the current player
        // Post: the number that represents which player won in a diagonal fashion, if any
        // Description: check if any player won with a 4 coins in a diagonal order 
        private int CheckForDiagonalWin(string colour)
        {
            // define the rows end and the column end to check for a win within
            byte rowCheckEndUp = 5;
            byte rowCheckEndDown = 2;
            byte colCheckEnd = 3;

            // run the inside code for the first column to the final column where a diagonal win can exist
            for (byte column = 0; column <= colCheckEnd; column++)
            {
                // run the inside code for the first row to the final row where a downward right diagonal win can exist
                for (byte row = 0; row <= rowCheckEndDown; row++)
                {
                    // if there is a downward diagonal victory, end the game and return which player won
                    if (boardSpots[row, column] == colour &&
                        boardSpots[row + 1, column + 1] == colour &&
                        boardSpots[row + 2, column + 2] == colour &&
                        boardSpots[row + 3, column + 3] == colour)
                    {
                        isGameDone = true;

                        if (colour == "red")
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }

                // run the inside code for the 3rd row to the final row where a upward right diagonal win can exist
                for (byte row = 3; row <= rowCheckEndUp; row++)
                {
                    // if there is a downward diagonal victory, end the game and return which player won
                    if (boardSpots[row, column] == colour &&
                        boardSpots[row - 1, column + 1] == colour &&
                        boardSpots[row - 2, column + 2] == colour &&
                        boardSpots[row - 3, column + 3] == colour)
                    {
                        isGameDone = true;

                        if (colour == "red")
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }
            }

            // if no one won and there are still empty spaces, return a -1
            if ((placedYellowCoins + placedRedCoins) < (yellowCoinBounds.Length + redCoinBounds.Length))
            {
                return -1;
            }
            // if no one won and there are no empty spaces, return a -2 and end game
            else
            {
                isGameDone = true;
                return -2;
            }
        }
    }
}
