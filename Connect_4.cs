using System;
using WMPLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



// Hello! Welcome to my connect 4! If you havent seen my naughts and crosses I suggest you check it out.
// What i think stands out about my program now is it runs off my own algorithm and scoring based system - i have research nothing into past solutions unlike all my classmates so feel free to  check out my weighted grid subroutines
// I hope you can understand my code as it is all commented (fairly well i hope), and as of writing this it is all grey boxes (don't tell Mr Albonozzo) so hopefully it looks better for you now as i just finished all my logic and play testing. Enjoy!



using System.Threading;

namespace Connect4
{
    public partial class Form1 : Form
    {

        // Variables ---------------------------------------------------------------------------------------------------------------------------------------

        WindowsMediaPlayer player = new WindowsMediaPlayer();

        int[,] Grid;                            // Creating a 2d array for the grid, and weighted grid (used by the AI to decide where to go)
        int[] WeightedGrid;
        int[] TempCheck = new int[] { 0, 0 };                 // A temp for the current counter
        int[] ComputerCounter = new int[] { 0, 0 };           // Temp counter for computers to use
        string Player1, Player2, Winner = "";
        bool P1sGo = true, WaitingForCounter = false;         // A bool so they cannot place their counter whilst the other is falling
        int HeightGap, WidthGap, MouseX, CollumnWidth, CircleHeight, CircleWidth, x, y, GridSizeX, GridSizeY, LengthOfCheck;       // Basic data about the grid and circles

        Brush Colour;                           //
        System.Drawing.Graphics graphics;       // For drawing circles
        Pen PenColour = new Pen(Color.White);   //

        bool Win = false;
        Random Rand = new Random();
        bool ComputerGame = false, ComputerGameWon = false, NormalGameWon = false, NormalGame = false;              // Have a go at putting the computer against itself
        int Volume, RowTotal, ColumnTotal, TotalFilled, ImageP1, ImageP2, HighestX, HighestY, HighestWeight, ColumnChosen;


        // Subroutines ---------------------------------------------------------------------------------------------------------------------------------------

        public Form1()
        {
            InitializeComponent();
        }


        void DrawCircles()
        {
            // Deciding Player 1
            if (lbxP1.SelectedIndex == 1)
            {
                Player1 = "EasyBot";
            }
            else if (lbxP1.SelectedIndex == 2)
            {
                Player1 = "MediumBot";
            }
            else if (lbxP1.SelectedIndex == 3)
            {
                Player1 = "HardBot";
            }
            else
            {
                Player1 = "User";
            }

            // Deciding Player 2
            if (lbxP2.SelectedIndex == 1)
            {
                Player2 = "EasyBot";
            }
            else if (lbxP2.SelectedIndex == 2)
            {
                Player2 = "MediumBot";
            }
            else if (lbxP2.SelectedIndex == 3)
            {
                Player2 = "HardBot";
            }
            else
            {
                Player2 = "User";
            }

            pnlGameBoard.Refresh();         // Clear the current board

            try
            {
                WidthGap = Int32.Parse(txtXGap.Text);
                HeightGap = Int32.Parse(txtYGap.Text);
                GridSizeX = Int32.Parse(txtGridWidth.Text);
                GridSizeY = Int32.Parse(txtGridHeight.Text);
                LengthOfCheck = Int32.Parse(txtLengthOfCheck.Text);

                graphics = pnlGameBoard.CreateGraphics();

                Grid = new int[GridSizeX, GridSizeY];               // Subract 1 for each one as grids start on 0
                WeightedGrid = new int[GridSizeX];

                x = WidthGap / 2;                   // Starting x value
                y = HeightGap / 2;                  // Starting y value

                CircleHeight = ((pnlGameBoard.Height / GridSizeY) - HeightGap);         // Calculate The height of each circle based on the height gaps, how many circles there are, and the size of the box
                CircleWidth = ((pnlGameBoard.Width / GridSizeX) - WidthGap);            // Calculate The width of each circle based on the width gaps, how many circles there are, and the size of the box

                for (int CirclesX = 0; CirclesX <= GridSizeX - 1; CirclesX++)           // Nested for loop to display all circles
                {
                    for (int CirclesY = 0; CirclesY <= GridSizeY - 1; CirclesY++)
                    {
                        graphics.FillEllipse(Brushes.Gray, x, y, CircleWidth, CircleHeight);           // Display circle
                        y += CircleHeight + HeightGap;          // Add space between circles in y direction each loop
                    }

                    y = HeightGap / 2;              // Gap between circles in y direction (resets once each line is done)
                    x += CircleWidth + WidthGap;    // Gap between circles in x direction
                }

                CollumnWidth = CircleWidth + WidthGap;          // Get collumn width after calculations for new grid is done


                // Starting the game

                CheckComputerMove();

                if ((Player1 == "EasyBot" || Player1 == "MediumBot" || Player1 == "HardBot") && (Player2 == "EasyBot" || Player2 == "Mediumbot" || Player2 == "HardBot")) // If p1 and p2 are computers...
                {
                    ComputerGame = true;
                }
                else
                {
                    ComputerGame = false;
                }

                ComputerGameWon = false;        // Set wins to false
                NormalGameWon = false;

                // Otherwise wait for the user to play
            }
            catch
            {
                MessageBox.Show("Please fill in all boxes");
            }
            
        }

        
        void CheckGrid(int StartX, int StartY)          // Check the current 4x4 grid for a win
        {
            // Check rows
            TempCheck[1] = StartY;

            for (int Row = 0; Row <= LengthOfCheck-1; Row++)
            {
                TempCheck[0] = StartX;

                RowTotal = 0;            // Reset total to 0 for each row

                for (int Column = 0; Column <= LengthOfCheck-1; Column++)
                {
                    RowTotal += Grid[TempCheck[0], TempCheck[1]];
                    TempCheck[0]++;
                }

                if (RowTotal == 40)
                {
                    // Player 1 wins

                    Winner = "Player 1";
                    GameWon();
                    return;
                }
                else if (RowTotal == 4)
                {
                    // Player 2 wins

                    Winner = "Player 2";
                    GameWon();
                    return;
                }

                TempCheck[1]++;
            }


            // Check columns for a win
            TempCheck[0] = StartX;

            for (int Column = 0; Column <= LengthOfCheck-1; Column++)
            {
                TempCheck[1] = StartY;

                ColumnTotal = 0;         // Reset total to 0 for each column

                for (int Row = 0; Row <= LengthOfCheck-1; Row++)
                {
                    ColumnTotal += Grid[TempCheck[0], TempCheck[1]];
                    TempCheck[1]++;
                }

                if (ColumnTotal == 40)
                {
                    // Player 1 wins

                    Winner = "Player 1";
                    GameWon();
                    return;
                }
                else if (ColumnTotal == 4)
                {
                    // Player 2 wins

                    Winner = "Player 2";
                    GameWon();
                    return;
                }

                TempCheck[0]++;
            }

            // Check diagonals for win
            int Diagonal1Total = Grid[StartX, StartY] + Grid[StartX + 1, StartY + 1] + Grid[StartX + 2, StartY + 2] + Grid[StartX + 3, StartY + 3];
            int Diagonal2Total = Grid[StartX + 3, StartY] + Grid[StartX + 2, StartY + 1] + Grid[StartX + 1, StartY + 2] + Grid[StartX, StartY + 3];

            if ((Diagonal1Total == (LengthOfCheck * 10)) || (Diagonal2Total == (LengthOfCheck * 10)))
            {
                // Player 1 wins

                Winner = "Player 1";
                GameWon();
                return;
            }
            else if ((Diagonal1Total == LengthOfCheck) || (Diagonal2Total == LengthOfCheck))
            {
                // Player 2 wins

                Winner = "Player 2";
                GameWon();
                return;
            }
        }

        void GetWeightedGrid()
        {

            for (int Loop = 0; Loop <= GridSizeX - 1; Loop++)
            {
                WeightedGrid[Loop] = 0;
            }

            // Loop through the list of positions (eg; only 7 positions on a 7 wide grid) and fill in weighted grid on a points system on how good they are

            for (int PieceX = 0; PieceX <= (GridSizeX - 1); PieceX++)
            {
                if (Grid[PieceX, 0] == 0)           // If the top piece in that column is free then there must be at least 1 free space
                {
                    for (int PieceY = (GridSizeY - 1); PieceY >= 0; PieceY--)           // Find possible place on grid starting from bottom, going up
                    {
                        if (Grid[PieceX, PieceY] == 0)           // Found free space in first column. Now analyse
                        {
                            AddWeightedTotal(-1, 1, PieceX, PieceY);          // Diagonal 1 (top right to bottom left)
                            AddWeightedTotal(1, 1, PieceX, PieceY);           // Diagonal 2 (top left to bottom right)
                            AddWeightedTotal(1, 0, PieceX, PieceY);           // Row (left to right)
                            AddWeightedTotal(0, 1, PieceX, PieceY);           // Column (only need to check below)

                            break;          // Break out of loop once youve found the first free space
                        }
                    }
                }
                else
                {
                    WeightedGrid[PieceX] = -1;          // If the computer cannot go here then set that column to -1
                }
            }
        }


        void AddWeightedTotal(int IncrementX, int IncrementY, int PieceX, int PieceY)
        {
            int PieceTesting = -1;          // As 0 represents a blank we cannot set this to 0, but we must have it available in the loop so we must initialise it here

            int TotalComputer = 0;
            int TotalOpponent = 0;

            int OverallTotalComputer = 0;
            int OverallTotalOpponent = 0;

            for (int Loop = 1; Loop <= LengthOfCheck-1; Loop++)           // Testing line right or down
            {

                // If the piece is inside the grid
                if ((PieceX + (Loop * IncrementX) <= GridSizeX - 1)  &&  (PieceX + (Loop * IncrementX) >= 0)  &&  (PieceY + (Loop * IncrementY) <= GridSizeY - 1)  &&  (PieceY + (Loop * IncrementY) >= 0))
                {
                    PieceTesting = Grid[PieceX + (Loop * IncrementX), PieceY + (Loop * IncrementY)];            // Get score of grid at counter check position
                }
                else
                {
                    break;          // Stop testing
                }


                if ((PieceTesting == 10 && P1sGo) || (PieceTesting == 1 && !P1sGo))          // If its our piece
                {
                    if(TotalOpponent == 0)
                    {
                        TotalComputer += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                else if ((PieceTesting == 10 && !P1sGo) || (PieceTesting == 1 && P1sGo))          // If its our opponent's piece
                {
                    if (TotalComputer == 0)
                    {
                        TotalOpponent += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // ----------------------------------------------------------------------------------------------------------

            OverallTotalComputer += TotalComputer;
            OverallTotalOpponent += TotalOpponent;

            TotalComputer = 0;
            TotalOpponent = 0;


            for (int Loop2 = 1; Loop2 <= LengthOfCheck-1; Loop2++)           // Testing line left or up
            {
                // If the piece is inside the grid
                if ((PieceX - (Loop2 * IncrementX) <= GridSizeX - 1)  &&  (PieceX - (Loop2 * IncrementX) >= 0)  &&  (PieceY - (Loop2 * IncrementY) <= GridSizeY - 1)  &&  (PieceY - (Loop2 * IncrementY) >= 0))
                {
                    PieceTesting = Grid[PieceX - (Loop2 * IncrementX), PieceY - (Loop2 * IncrementY)];            // Get score of grid at counter check position
                }
                else
                {
                    break;          // Stop testing
                }


                if ((PieceTesting == 10 && P1sGo) || (PieceTesting == 1 && !P1sGo))          // If its our piece
                {
                    if (TotalOpponent == 0)
                    {
                        TotalComputer += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                else if ((PieceTesting == 10 && !P1sGo) || (PieceTesting == 1 && P1sGo))          // If its our opponent's piece
                {
                    if (TotalComputer == 0)
                    {
                        TotalOpponent += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            OverallTotalComputer += TotalComputer;
            OverallTotalOpponent += TotalOpponent;

            CalculateScore(OverallTotalComputer, OverallTotalOpponent, PieceX);            // Add on score for this line as we are now done adding totals
        }

        void CalculateScore(int TotalComputer, int TotalOpponent, int PieceX)
        {
            // Moving on to point scoring system

            /* Points System
             * 
                +1 for placing away from edges

                +8 for making a line of 2
                +4 for blocking a line of 2

                +40 for making a line of 3
                +12 for blocking a line of 3

                +2000 if computer can win
                +400 if computer must defend
            */

            if (TotalComputer == 3)         // If there are 3 next to computer, definitely go here as you can win
            {
                WeightedGrid[PieceX] += 2000;
            }
            else if (TotalComputer == 2)
            {
                WeightedGrid[PieceX] += 40;
            }
            else if (TotalComputer == 1)
            {
                WeightedGrid[PieceX] += 8;
            }

            if (TotalOpponent == 3)            // If there are 3 of the opponents pieces next to each other we must defend (if we cannot win)
            {
                WeightedGrid[PieceX] += 400;            // (Meaning it gets a high score, but lower than winning - in theory if the opponent stacked more than 5 wins on one piece, the AI would choose to go there instead)
            }
            else if (TotalOpponent == 2)
            {
                WeightedGrid[PieceX] += 12;
            }
            else if (TotalOpponent == 1)
            {
                WeightedGrid[PieceX] += 4;
            }

            if (PieceX >= 3 && PieceX <= GridSizeX-4)         // Add on a small score to place the piece in the middle of the grid (away from edges)
            {
                WeightedGrid[PieceX] += 1;
            }
        }

        void GameWon()
        {

            if (ComputerGame)
            {
                ComputerGameWon = true;
            }
            else
            {
                NormalGameWon = true;
            }

            // set next round button to visible = true          to do
            
            MessageBox.Show(Winner + " won!!");

        }

        void PlaceCounter(int CollumnChosen)
        {
            if(Grid[CollumnChosen, 0] == 0)
            {
                for (int CheckY = 0; CheckY <= GridSizeY - 1; CheckY++)       // To do: loop through the other way (ground up) when looking for free spots
                {
                    if (Grid[CollumnChosen, CheckY] != 0)
                    {
                        AnimateCounter(CollumnChosen, CheckY - 1);

                        // Place counter at Grid [CollumnChosen, CheckY - 1]
                        if (P1sGo)
                        {
                            Grid[CollumnChosen, CheckY - 1] = 10;
                            break;
                        }
                        else
                        {
                            Grid[CollumnChosen, CheckY - 1] = 1;
                            break;
                        }
                    }
                    else if (CheckY == (GridSizeY - 1))
                    {
                        AnimateCounter(CollumnChosen, CheckY);

                        // No counters placed in this collumn yet, so place counter here
                        if (P1sGo)
                        {
                            Grid[CollumnChosen, CheckY] = 10;
                            break;
                        }
                        else
                        {
                            Grid[CollumnChosen, CheckY] = 1;
                            break;
                        }
                    }
                }

                // Check for a win if a piece has been placed

                // Subract 4 as this is the starting position for the check, we do not want to start the check so that it then attempts to check a row
                // that is not actually in the grid (eg; if you tried to do a row from the right starting from the rightmost cell)
                for (int StartCheckX = 0; StartCheckX <= GridSizeX - 4; StartCheckX++)
                {
                    for (int StartCheckY = 0; StartCheckY <= GridSizeY - 4; StartCheckY++)
                    {
                        CheckGrid(StartCheckX, StartCheckY);
                    }
                }
            }

            P1sGo = !P1sGo;

            CheckComputerMove();
        }


        void CheckComputerMove()
        {
            // Only place a computer move if the game hasnt been won
            if ((!ComputerGameWon && !NormalGameWon)  && ((P1sGo && Player1 != "User") || (!P1sGo && Player2 != "User")))
            {
                // If its player 2's go
                if (!P1sGo)
                {
                    if (Player2 == "EasyBot")
                    {
                        // Random
                        EasyBotMove();
                    }
                    else if (Player2 == "MediumBot")
                    {
                        // Random between best move to make, and easy bot move
                        MediumBotMove();
                    }
                    else if (Player2 == "HardBot")
                    {
                        // As good as i could make it
                        HardBotMove();
                    }
                }

                // If its player 1's go
                else if (P1sGo)
                {
                    if (Player1 == "EasyBot")
                    {
                        // Random
                        EasyBotMove();
                    }
                    else if (Player1 == "MediumBot")
                    {
                        // Random between best move to make, and easy bot move
                        MediumBotMove();
                    }
                    else if (Player1 == "HardBot")
                    {
                        // As good as i could make it
                        HardBotMove();
                    }
                }
            }
        }

        bool CheckWinOrDefend() // return as 1) 1 if can win or 0 if cant 2) x-coordinate if can win 3) y-coordinate if can win
        {
            int RowTemp = 0;
            int ColumnTemp = 0;

            for (int Column = 0; Column <= GridSizeY - 1; Column++)
            {
                for (int Row = 0; Row <= GridSizeX - 4; Row++)
                {
                    ColumnTotal = 0;
                    RowTotal = 0;            // Reset total to 0 for each row

                    // Total up rows and columns
                    for (int Loop = 0; Loop <= 3; Loop++)           // Loop 4 times to add that total
                    {
                        RowTotal += Grid[Row + Loop, Column];
                        ColumnTotal += Grid[Column, Row + Loop];

                        if (Grid[Row + Loop, Column] == 0)
                        {
                            RowTemp = Row + Loop;
                        }

                        if (Grid[Column, Row + Loop] == 0)
                        {
                            ColumnTemp = Column;
                        }
                    }

                    // Then check if we can defend instead

                    if ((RowTotal == 30 && !P1sGo) || (RowTotal == 3 && P1sGo))         // Rows
                    {
                        // Can defend in that row
                        PlaceCounter(RowTemp);
                        return true;
                    }

                    if ((ColumnTotal == 30 && !P1sGo) || (ColumnTotal == 3 && P1sGo))       // Columns
                    {
                        // Can defend in that row
                        PlaceCounter(ColumnTemp);
                        return true;
                    }


                    // move up to do
                    // Checking if we can win first

                    if ((RowTotal == 30 && P1sGo) || (RowTotal == 3 && !P1sGo))         // Rows
                    {
                        // Can win in that row
                        PlaceCounter(RowTemp);
                        return true;
                    }

                    if ((ColumnTotal == 30 && P1sGo) || (ColumnTotal == 3 && !P1sGo))       // Columns
                    {
                        // Can win in that row
                        PlaceCounter(ColumnTemp);
                        return true;
                    }
                }
            }

            // Otherwise return false
            return false;

        }

        void CheckForDraw()
        {
            int Counter = 0;

            for (int Loop = 0; Loop <= GridSizeY - 1; Loop++)
            {
                if(Grid[Loop , 0] != 0)
                {
                    Counter++;
                }
            }

            if(Counter == GridSizeX)
            {
                Winner = "Draw";
                GameWon();
            }
        }



        void EasyBotMove()          // Random
        {
            int ChosenCollumn = Rand.Next(GridSizeX - 1);           // Random collumn

            // Check that line is not full
            while (Grid[ChosenCollumn,0] != 0)
            {
                ChosenCollumn = Rand.Next(GridSizeX - 1);
            }
            
            PlaceCounter(ChosenCollumn);     // Generates a random num between 0 and 1 less than grid size
        }

        void MediumBotMove()        // Random between best move to make, and easy bot move
        {
            if(Rand.Next(1) == 0)   // Generate random number between 0 and 1, if 0 then..
            {
                EasyBotMove();
            }
            else                    // Otherwise...
            {
                HardBotMove();
            }
        }

        void HardBotMove()         // Check for if he can win, then check defence - otherwise place piece optimally
        {
            GetWeightedGrid();

            int TempHighest = 0;
            int TempColumn = 0;

            var BestMoves = new List<dynamic>();

            for (int Loop = 0; Loop <= GridSizeX-1; Loop++)
            {
                if (WeightedGrid[Loop] > TempHighest)
                {
                    BestMoves.Clear();
                    TempHighest = WeightedGrid[Loop];
                    TempColumn = Loop;
                    BestMoves.Add(Loop);
                }
                else if (WeightedGrid[Loop] == TempHighest)
                {
                    BestMoves.Add(Loop);
                }

                Console.WriteLine(WeightedGrid[Loop] + "  ");
            }

            PlaceCounter(BestMoves[Rand.Next(BestMoves.Count)]);

            Console.WriteLine("");
        }


        void AnimateCounter(int EndX,int EndY)
        {
            // First figure out starting x position for all circles (as this will not change as it animates down)
            int Xpos = (CollumnWidth * EndX) + (WidthGap / 2);
            int Ypos = HeightGap / 2;

            int TimeToWait = Convert.ToInt32((2.4 / GridSizeY) * 1000);             // How long to wait for each piece in ms (this relationship is inversely prop. so a = k/b)

            if (P1sGo)
            {
                Colour = Brushes.Red;
            }
            else
            {
                Colour = Brushes.Yellow;
            }
            
            // Now loop through how many circles it needs to animate and add onto the y value each time - making it the player's colour and filling it back to white after
            for (int Loop = 0; Loop <= EndY - 1; Loop++)
            {
                graphics.FillEllipse(Colour, Xpos, Ypos, CircleWidth, CircleHeight);            // Fill circle with player's colour

                WaitingForCounter = true;
                Thread.Sleep(TimeToWait);          //will sleep for 0.5 sec
                WaitingForCounter = false;

                graphics.FillEllipse(Brushes.Gray, Xpos, Ypos, CircleWidth, CircleHeight);     // Fill circle back to white

                Ypos += (CircleHeight + HeightGap);         // Now move down and repeat
            }

            // Now fill in circle that is to be permenantly in place
            graphics.FillEllipse(Colour, Xpos, Ypos, CircleWidth, CircleHeight);
        }

        string ChangeInXO(int num)    // To return a -, x or o depending on the grid number
        {
            if(num == 0)
            {
                return "-";
            }
            else if(num == 10)
            {
                return "x";          // Where player 1 represents x
            }
            else
            {
                return "o";          // Where player 2 represents o
            }
        }

        void PrintGrid()            // Printing the grid routine in a nested for loop
        {
            for(int y = 0; y<= GridSizeY - 1; y++)
            {
                for (int x = 0; x <= GridSizeX - 1; x++)
                {
                    if(x != GridSizeX - 1)
                    {
                        Console.Write(ChangeInXO(Grid[x, y]) + " ");
                    }
                    else
                    {
                        Console.Write(ChangeInXO(Grid[x, y]) + " \n");
                    }
                }
            }

            Console.WriteLine("");
        }



        // Visual studio triggers -----------------------------------------------------------------------

        private void pnlGameBoard_Click(object sender, EventArgs e)
        {
            if(((P1sGo && Player1 == "User") || (!P1sGo && Player2 == "User"))) // && !WaitingForCounter to do      // As long as it is the users turn, and we are not waiting for another counter to fall    
            {
                MouseEventArgs Mouse = (MouseEventArgs)e;               // Get mouse's X position within picturebox
                MouseX = Mouse.X;

                int UpperXCollumnCheck = CollumnWidth;                  // The upperbound of the current collumn (Starts as width of collumn)
                int LowerXCollumnCheck = 0;                             // The lowerbound of the current collumn (Starts as 0)

                for (int CollumnChosen = 0; CollumnChosen <= (GridSizeX - 1); CollumnChosen++)          // Check each collumn
                {
                    if (MouseX >= LowerXCollumnCheck && MouseX < UpperXCollumnCheck)            // if they click between the coordinates of the border
                    {
                        PlaceCounter(CollumnChosen);            // Place counter in collumn
                        break;
                    }

                    UpperXCollumnCheck += CollumnWidth;         // Add the collumn width to the area we're now looking for on the mouse click
                    LowerXCollumnCheck += CollumnWidth;
                }
            }
        }


        private void txtGridWidth_TextChanged(object sender, EventArgs e)           // Some user validation
        {
            try
            {
                if (Int32.Parse(txtGridWidth.Text) > 100)
                {
                    txtGridWidth.Text = "100";
                }
            }
            catch
            {
                if(txtGridWidth.Text != "")
                {
                    MessageBox.Show("Please enter an integer");
                    txtGridWidth.Text = "";
                }
            }
        }

        private void txtGridHeight_TextChanged(object sender, EventArgs e)           // Some user validation
        {
            try
            {
                if (Int32.Parse(txtGridHeight.Text) > 100)
                {
                    txtGridHeight.Text = "100";
                }
            }
            catch
            {
                if (txtGridHeight.Text != "")
                {
                    MessageBox.Show("Please enter an integer");
                    txtGridHeight.Text = "";
                }
            }
        }

        private void txtLengthOfCheck_TextChanged(object sender, EventArgs e)           // Some user validation
        {
            try
            {
                if (Int32.Parse(txtLengthOfCheck.Text) > 10)
                {
                    txtLengthOfCheck.Text = "10";
                }
            }
            catch
            {
                if (txtLengthOfCheck.Text != "")
                {
                    MessageBox.Show("Please enter an integer");
                    txtLengthOfCheck.Text = "";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)          // Defaults button
        {
            txtLengthOfCheck.Text = "4";
            txtGridHeight.Text = "6";
            txtGridWidth.Text = "7";
            txtXGap.Text = "8";
            txtYGap.Text = "12";
        }


        private void btnNextGame_Click(object sender, EventArgs e)
        {
            DrawCircles();
        }


        private void button1_Click(object sender, EventArgs e)  // to do
        {
            DrawCircles();
        }

        private void btnMinimise_Click(object sender, EventArgs e)      // Minimise button
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)          // Close button
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player.URL = "Downforce.mp3";
            player.controls.play();
            player.settings.volume = 25;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e) { }
    }
}