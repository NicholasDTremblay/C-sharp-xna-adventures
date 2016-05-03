using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ThePowerof2XNA
{
    public class Board
    {
        //List<Tile> myTiles; // 10 x 10 (at least for now)
        public placement[,] myPlacements;

        public Boolean boardChanged;

        public int playerScore;

        public int gravity;

        public Random myRandom;

        public Texture2D[] myTileImages;

        public int boardWidth;
        public int boardHeight;

        public int pointPerTile;

        public Boolean gravityHorizontal;

        public Board(Texture2D[] myTextures)
        {
            pointPerTile = 50;
            playerScore = 0;
            boardWidth = 10;
            boardHeight = 10;
            myRandom = new Random();
            myPlacements = new placement[boardWidth, boardHeight];

            myTileImages = myTextures;

            int tempRand;

            gravityHorizontal = false;

            //Tile tempTile;

            for (int w = 0; w < boardWidth; w++)
            {
                for (int h = 0; h < boardHeight; h++)
                {
                    myPlacements[w, h] = new placement();
                    tempRand = myRandom.Next(0, 5);
                    //tempTile = 
                    myPlacements[w, h].theTile = new Tile(w, h, myTileImages[tempRand], tempRand);
                    myPlacements[w, h].occupied = true;
                }
            }

            gravity = 0;
            playerScore = 0;
            boardChanged = false;
        }

        public void reEvalDirection()
        {
            if (gravity < 2)
            {
                gravityHorizontal = false;
            }
            else
            {
                gravityHorizontal = true;
            }
        }

        public void fillEmpty()
        {
            int tempRand = 0;
            for (int w = 0; w < boardWidth; w++)
            {
                for (int h = 0; h < boardHeight; h++)
                {
                    if (myPlacements[w, h].occupied == false)
                    {
                        tempRand = myRandom.Next(0, 5);
                        myPlacements[w, h].theTile = new Tile(w, h, myTileImages[tempRand], tempRand);
                        myPlacements[w, h].occupied = true;
                    }
                }
            }
        }

        public Tile makeMeATile(int whereX, int whereY)
        {
            int tempRand = myRandom.Next(5);

            Tile tempTile;

            tempTile = new Tile(whereX, whereY, myTileImages[tempRand], tempRand);

            return tempTile;
        }

        public void destroyTiles()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int k = 0; k < 10; k++)
                {
                    if (myPlacements[i, k].occupied)
                    {
                        if (myPlacements[i, k].theTile.flagged)
                        {
                            myPlacements[i, k].theTile = null;
                            myPlacements[i, k].occupied = false;
                            playerScore += pointPerTile;
                        }
                    }
                }
            }
        }

        public Boolean fall(int rowColNum)
        {
            Boolean stillMoving = true;
            int moveCount = 0;
            Boolean hasMoved = false;

            switch (gravity)
            {
                case 0://down
                    for (int i = 9; i >= 0; i--)
                    {
                        if (myPlacements[rowColNum, i].occupied)
                        {
                            do
                            {
                                if (i + moveCount + 1 > 9)
                                {
                                    stillMoving = false;
                                    break;
                                }
                                if (myPlacements[rowColNum, i + moveCount + 1].occupied == false)
                                {
                                    moveCount++;
                                }
                                else
                                {
                                    stillMoving = false;
                                }


                            } while (stillMoving == true);

                            if (moveCount > 0)
                            {
                                myPlacements[rowColNum, i + moveCount].theTile = myPlacements[rowColNum, i].theTile;
                                myPlacements[rowColNum, i + moveCount].occupied = true;

                                myPlacements[rowColNum, i].theTile = null;
                                myPlacements[rowColNum, i].occupied = false;

                                hasMoved = true;
                            }

                            stillMoving = true;
                            moveCount = 0;
                        }
                    }
                    break;
                case 1://up
                    for (int i = 0; i < 10; i++)
                    {
                        if (myPlacements[rowColNum, i].occupied)
                        {
                            do
                            {
                                if (i - moveCount - 1 < 0)
                                {
                                    stillMoving = false;
                                    break;
                                }
                                if (myPlacements[rowColNum, i - moveCount - 1].occupied == false)
                                {
                                    moveCount++;
                                }
                                else
                                {
                                    stillMoving = false;
                                }


                            } while (stillMoving == true);

                            if (moveCount > 0)
                            {
                                myPlacements[rowColNum, i - moveCount].theTile = myPlacements[rowColNum, i].theTile;
                                myPlacements[rowColNum, i - moveCount].occupied = true;

                                myPlacements[rowColNum, i].theTile = null;
                                myPlacements[rowColNum, i].occupied = false;

                                hasMoved = true;
                            }

                                stillMoving = true;
                                moveCount = 0;
                        }
                    }
                    break;
                case 2://left
                    for (int i = 0; i < 10; i++)
                    {
                        if (myPlacements[i, rowColNum].occupied)
                        {
                            do
                            {
                                if (i - moveCount - 1 < 0)
                                {
                                    stillMoving = false;
                                    break;
                                }
                                if (myPlacements[i - moveCount - 1, rowColNum].occupied == false)
                                {
                                    moveCount++;
                                }
                                else
                                {
                                    stillMoving = false;
                                }


                            } while (stillMoving == true);

                            if (moveCount > 0)
                            {
                                myPlacements[i - moveCount, rowColNum].theTile = myPlacements[i, rowColNum].theTile;
                                myPlacements[i - moveCount, rowColNum].occupied = true;

                                myPlacements[i, rowColNum].theTile = null;
                                myPlacements[i, rowColNum].occupied = false;

                                hasMoved = true;
                            }
                            
                            stillMoving = true;
                            moveCount = 0;
                        }
                    }
                    break;
                case 3://right
                    for (int i = 9; i >= 0; i--)
                    {
                        if (myPlacements[i, rowColNum].occupied)
                        {
                            do
                            {
                                if (i + moveCount + 1 > 9)
                                {
                                    stillMoving = false;
                                    break;
                                }
                                if (myPlacements[i + moveCount + 1, rowColNum].occupied == false)
                                {
                                    moveCount++;
                                }
                                else
                                {
                                    stillMoving = false;
                                }


                            } while (stillMoving == true);

                            if (moveCount > 0)
                            {
                                myPlacements[i + moveCount, rowColNum].theTile = myPlacements[i, rowColNum].theTile;
                                myPlacements[i + moveCount, rowColNum].occupied = true;

                                myPlacements[i, rowColNum].theTile = null;
                                myPlacements[i, rowColNum].occupied = false;

                                hasMoved = true;
                            }

                            stillMoving = true;
                            moveCount = 0;
                        }
                    }
                    break;
                default:
                    hasMoved = false;
                    break;
            }
            return hasMoved;
        }

        public void matchTiles(int rowColNum) //true is row, false is column
        {
            int count = 0;
            Boolean stillMatching = true;

            Boolean matchingUp = true;
            Boolean matchingDown = true;

            Boolean rowCol = gravityHorizontal;

            int upCount = 0;
            int downCount = 0;

            //int superCount = 0;

            if (rowCol)
            {
                for (int i = 0; i < 10; i++) // check left and right
                {
                    count = 0;
                    stillMatching = true;
                    upCount = 0;
                    downCount = 0;
                    matchingDown = true;
                    matchingUp = true;

                    do
                    {
                        if (i + count + 1 > 9)
                        {
                            stillMatching = false;
                            break;
                        }
                        stillMatching = myPlacements[i + count, rowColNum].theTile.matchUp(myPlacements[i + count + 1, rowColNum].theTile); // if next tile is matching
                        if (stillMatching)
                        {
                            count++;
                        }

                    } while (stillMatching == true);

                    if (count >= 2)
                    {
                        for (int matched = 0; matched <= count; matched++)
                        {
                            myPlacements[i + matched, rowColNum].theTile.flagged = true;
                        }
                        boardChanged = true;
                    }

                    count = 0;
                    stillMatching = true;

                    do // check up and down
                    {
                        if (rowColNum + downCount + 1 > 9)
                        {
                            matchingDown = false;
                        }

                        if (rowColNum - upCount - 1 < 0)
                        {
                            matchingUp = false;
                        }


                        if (matchingDown)
                        {
                            matchingDown = myPlacements[i, rowColNum + downCount].theTile.matchUp(myPlacements[i, rowColNum + downCount + 1].theTile); // if next tile is matching
                            if (matchingDown)
                            {
                                downCount++;
                            }
                        }
                        if (matchingUp)
                        {
                            matchingUp = myPlacements[i, rowColNum - upCount].theTile.matchUp(myPlacements[i, rowColNum - upCount - 1].theTile); // if next tile is matching
                            if (matchingUp)
                            {
                                upCount++;
                            }
                        }

                        if (matchingDown == false && matchingUp == false)
                        {
                            stillMatching = false;
                            //break;
                        }

                    } while (stillMatching == true);

                    if (downCount + upCount >= 2)
                    {
                        boardChanged = true;
                        for (int matched = 0; matched <= downCount + upCount; matched++)
                        {
                            myPlacements[i, rowColNum - upCount + matched].theTile.flagged = true;
                        }
                    }

                    //superCount++;
                }
            }
            else
            {
                for (int i = 0; i < 10; i++) // check up and down
                {
                    count = 0;
                    stillMatching = true;
                    downCount = 0;
                    upCount = 0;
                    matchingDown = true;
                    matchingUp = true;

                    do
                    {
                        if (i + count + 1 > 9)
                        {
                            stillMatching = false;
                            break;
                        }
                        stillMatching = myPlacements[rowColNum, i + count].theTile.matchUp(myPlacements[rowColNum, i + count + 1].theTile); // if next tile is matching
                        if (stillMatching)
                        {
                            count++;
                        }

                    } while (stillMatching == true);

                    if (count >= 2)
                    {
                        boardChanged = true;
                        for (int matched = 0; matched <= count; matched++)
                        {
                            myPlacements[rowColNum, i + matched].theTile.flagged = true;
                        }
                    }

                    count = 0;
                    stillMatching = true;

                    do // check left and right
                    {
                        if (rowColNum + downCount + 1 > 9)
                        {
                            matchingDown = false;
                        }

                        if (rowColNum - upCount - 1 < 0)
                        {
                            matchingUp = false;
                        }


                        if (matchingDown)
                        {
                            matchingDown = myPlacements[rowColNum + downCount, i].theTile.matchUp(myPlacements[rowColNum + downCount + 1, i].theTile); // if next tile is matching
                            if (matchingDown)
                            {
                                downCount++;
                            }
                        }
                        if (matchingUp)
                        {
                            matchingUp = myPlacements[rowColNum - upCount, i].theTile.matchUp(myPlacements[rowColNum - upCount - 1, i].theTile); // if next tile is matching
                            if (matchingUp)
                            {
                                upCount++;
                            }
                        }

                        if (matchingDown == false && matchingUp == false)
                        {
                            stillMatching = false;
                            //break;
                        }

                    } while (stillMatching == true);

                    if (downCount + upCount >= 2)
                    {
                        boardChanged = true;
                        for (int matched = 0; matched <= downCount + upCount; matched++)
                        {
                            myPlacements[rowColNum - upCount + matched, i].theTile.flagged = true;
                        }
                    }

                }
            }
        }
    }
}
