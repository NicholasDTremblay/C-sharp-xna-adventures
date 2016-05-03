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
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization; 

namespace ThePowerof2XNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Random myRandom;

        SpriteFont font;

        //Texture2D blueImage;
        //Texture2D redImage;
        //Texture2D greenImage;
        //Texture2D purpleImage;
        //Texture2D yellowImage;

        int myHighScore;

        String showHighScore;
        Vector2 highScoreVec;

        StorageDevice device;

        string containerName = "MyGamesStorage";
        string filename = "mysave.sav";
        [Serializable]
        public struct SaveGame
        {
            public int highScore;
        }

        private void InitiateSave()
        {
            if (!Guide.IsVisible)
            {
                device = null;
                StorageDevice.BeginShowSelector(PlayerIndex.One, this.SaveToDevice, null);
            }
        }

        void SaveToDevice(IAsyncResult result)
        {
            device = StorageDevice.EndShowSelector(result);
            if (device != null && device.IsConnected)
            {
                SaveGame SaveData = new SaveGame()
                {
                    highScore = theBoard.playerScore,
                };
                IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = device.EndOpenContainer(r);
                if (container.FileExists(filename))
                    container.DeleteFile(filename);
                Stream stream = container.CreateFile(filename);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
                serializer.Serialize(stream, SaveData);
                stream.Close();
                container.Dispose();
                result.AsyncWaitHandle.Close();
            }
        }

        private void InitiateLoad()
        {
            if (!Guide.IsVisible)
            {
                device = null;
                StorageDevice.BeginShowSelector(PlayerIndex.One, this.LoadFromDevice, null);
            }
        }

        void LoadFromDevice(IAsyncResult result)
        {
            device = StorageDevice.EndShowSelector(result);
            IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(r);
            result.AsyncWaitHandle.Close();
            if (container.FileExists(filename))
            {
                Stream stream = container.OpenFile(filename, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
                SaveGame SaveData = (SaveGame)serializer.Deserialize(stream);
                stream.Close();
                container.Dispose();
                //Update the game based on the save game file
                myHighScore = SaveData.highScore;
            }
        }

        int gameState;

        const int starting = 0;
        const int playing = 1;
        const int animating = 2;
        const int falling = 3;
        const int gameOver = 4;

        Texture2D gameOverTexture;
        Rectangle gameOverRect;

        Texture2D[] myTileTextures;

        Texture2D startImage;

        Texture2D backgroundImage;
        Rectangle backgroundRect;

        Texture2D behindImage;
        Rectangle behindRect;

        Board theBoard;

        Boolean[] whatHasFell;

        Rectangle[,] placementRects;

        Rectangle[] animationRect;

        MouseState myMouseState;

        Texture2D[] myArrowTextures;
        Rectangle[] myArrowRects;
        Rectangle[] myArrowFrame;

        String playerScore;
        Vector2 scoreVector;

        int fallCount;

        float myTime;

        int timeLeft;

        String showTime;

        Vector2 timeVector;

        Boolean startUp;

        int timeLimit;

        SoundEffect popSound;
        SoundEffect[] boomSounds;

        Song startSong;
        Song playingSong;
        Song gameOverSong;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 800;

            Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("SpriteFont1");

            myRandom = new Random();

            gameState = starting;

            gameOverTexture = Content.Load<Texture2D>("gameOver");

            gameOverRect = new Rectangle(25, 75, 750, 450);

            myArrowTextures = new Texture2D[4];

            myArrowTextures[0] = Content.Load<Texture2D>("downArrow");
            myArrowTextures[1] = Content.Load<Texture2D>("upArrow");
            myArrowTextures[2] = Content.Load<Texture2D>("leftArrow");
            myArrowTextures[3] = Content.Load<Texture2D>("rightArrow");

            myArrowRects = new Rectangle[4];

            myArrowRects[0] = new Rectangle(400, 550, 50, 50);
            myArrowRects[1] = new Rectangle(500, 550, 50, 50);
            myArrowRects[2] = new Rectangle(600, 550, 50, 50);
            myArrowRects[3] = new Rectangle(700, 550, 50, 50);

            myArrowFrame = new Rectangle[4];

            myArrowFrame[0] = new Rectangle(50, 0, 50, 50);
            myArrowFrame[1] = new Rectangle(0, 0, 50, 50);
            myArrowFrame[2] = new Rectangle(0, 0, 50, 50);
            myArrowFrame[3] = new Rectangle(0, 0, 50, 50);

            myTileTextures = new Texture2D[5];

            myTileTextures[0] = Content.Load<Texture2D>("blue");
            myTileTextures[1] = Content.Load<Texture2D>("red");
            myTileTextures[2] = Content.Load<Texture2D>("green");
            myTileTextures[3] = Content.Load<Texture2D>("purple");
            myTileTextures[4] = Content.Load<Texture2D>("yellow");

            backgroundImage = Content.Load<Texture2D>("Background");

            startImage = Content.Load<Texture2D>("start");
            backgroundRect = new Rectangle(0, 0, 800, 800);

            behindImage = Content.Load<Texture2D>("behindTiles");
            behindRect = new Rectangle(300, 0, 500, 500);

            theBoard = new Board(myTileTextures);

            whatHasFell = new Boolean[theBoard.boardWidth];

            for (int i = 0; i < whatHasFell.Length; i++)
            {
                whatHasFell[i] = true;
            }

            playerScore = "Score:" + theBoard.playerScore;
            scoreVector = new Vector2(400, 650);

            placementRects = new Rectangle[theBoard.boardWidth, theBoard.boardHeight];

            for (int width = 0; width < theBoard.boardWidth; width++)
            {
                for (int height = 0; height < theBoard.boardHeight; height++)
                {
                    placementRects[width, height] = new Rectangle(50 * width + 300, 50 * height, 50, 50);
                }
            }

            animationRect = new Rectangle[4];

            animationRect[0] = new Rectangle(0, 0, 50, 50);
            animationRect[1] = new Rectangle(50, 0, 50, 50);
            animationRect[2] = new Rectangle(100, 0, 50, 50);
            animationRect[3] = new Rectangle(150, 0, 50, 50);

            fallCount = 0;

            startUp = true;

            timeLimit = 60;

            myTime = 0;
            timeLeft = timeLimit;

            showTime = "Time Left: " + timeLeft;

            timeVector = new Vector2(400, 700);

            popSound = Content.Load<SoundEffect>("pop");

            boomSounds = new SoundEffect[3];

            boomSounds[0] = Content.Load<SoundEffect>("boom1");
            boomSounds[1] = Content.Load<SoundEffect>("boom2");
            boomSounds[2] = Content.Load<SoundEffect>("boom3");

            startSong = Content.Load<Song>("starting");

            playingSong = Content.Load<Song>("playing");

            gameOverSong = Content.Load<Song>("gameOverSong");

            InitiateLoad();

            showHighScore = "High Score: " + myHighScore;

            highScoreVec = new Vector2(400, 750);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            myMouseState = Mouse.GetState();
            if (gameState == starting)
            {
                myTime += gameTime.ElapsedGameTime.Milliseconds;

                if (!(MediaPlayer.State == MediaState.Playing))
                {
                    MediaPlayer.Play(startSong);
                }
                if (myTime > 1000)
                {
                    if (myMouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = falling;
                        myTime = 0;
                        timeLeft = timeLimit;
                        theBoard.playerScore = 0;
                        startUp = true;
                        fallCount = 0;
                        for (int i = 0; i < whatHasFell.Length; i++)
                        {
                            whatHasFell[i] = true;
                        }
                        for (int w = 0; w < theBoard.boardWidth; w++)
                        {
                            for (int h = 0; h < theBoard.boardHeight; h++)
                            {
                                theBoard.myPlacements[w, h].emptyOut();
                                theBoard.myPlacements[w, h].theTile = theBoard.makeMeATile(w, h);
                                theBoard.myPlacements[w, h].occupied = true;
                            }
                        }
                        MediaPlayer.Stop();
                    }
                }
            }
            else if (gameState == playing)
            {

                if (!(MediaPlayer.State == MediaState.Playing))
                {
                    MediaPlayer.Play(playingSong);
                }

                myTime += gameTime.ElapsedGameTime.Milliseconds;

                timeLeft = timeLimit - (int)(myTime / 1000);

                showTime = "Time Left: " + timeLeft;

                if (myMouseState.LeftButton == ButtonState.Pressed)
                {
                    for (int arrowCount = 0; arrowCount < 4; arrowCount++)
                    {
                        if (isClick(myMouseState, myArrowRects[arrowCount]))
                        {
                            for (int otherArrow = 0; otherArrow < 4; otherArrow++)
                            {
                                if (otherArrow != arrowCount)
                                {
                                    myArrowFrame[otherArrow].X = 0;
                                }
                                else
                                {
                                    myArrowFrame[otherArrow].X = 50;
                                    theBoard.gravity = otherArrow;
                                    theBoard.reEvalDirection();
                                }
                            }
                            break;
                        }
                    }
                    //this.Exit();
                    for (int w = 0; w < theBoard.boardWidth; w++)
                    {
                        for (int h = 0; h < theBoard.boardHeight; h++)
                        {
                            if (isClick(myMouseState, placementRects[w, h]))
                            {
                                if (theBoard.myPlacements[w, h].theTile != null)
                                {
                                    theBoard.myPlacements[w, h].theTile.flagged = true;
                                    gameState = animating;
                                    popSound.Play();
                                }
                            }
                        }
                    }
                }

                if (timeLeft <= 0)
                {
                    gameState = gameOver;
                    MediaPlayer.Stop();
                }
            }
            else if (gameState == animating)
            {
                myTime += gameTime.ElapsedGameTime.Milliseconds;

                timeLeft = timeLimit - (int)(myTime / 1000);

                showTime = "Time Left: " + timeLeft;

                bool done = true;

                //theBoard.destroyTiles();
                
                for (int w = 0; w < theBoard.boardWidth; w++)
                {
                    for (int h = 0; h < theBoard.boardHeight; h++)
                    {
                        if (theBoard.myPlacements[w, h].theTile != null)
                        {
                            if (theBoard.myPlacements[w, h].theTile.flagged)
                            {

                                theBoard.myPlacements[w, h].theTile.animate();
                                done = false;
                                if (theBoard.myPlacements[w, h].theTile.animationFrame > 11)
                                {
                                    theBoard.playerScore += theBoard.pointPerTile;
                                    theBoard.myPlacements[w, h].emptyOut();
                                }
                            }
                        }
                        
                    }             
                }

                playerScore = "Score:" + theBoard.playerScore;

                if (done)
                {
                    gameState = falling;
                    //gameState = gameOver;
                }
            }
            else if (gameState == falling)
            {
                myTime += gameTime.ElapsedGameTime.Milliseconds;

                timeLeft = timeLimit - (int)(myTime / 1000);

                showTime = "Time Left: " + timeLeft;

                if (!startUp)
                {
                    fallCount++;

                    for (int x = 0; x < theBoard.boardWidth; x++)
                    {
                        whatHasFell[x] = theBoard.fall(x);
                    }

                    theBoard.fillEmpty();

                    for (int y = 0; y < whatHasFell.Length; y++)
                    {
                        if (whatHasFell[y] == true)
                        {
                            theBoard.matchTiles(y);
                            whatHasFell[y] = false;
                        }
                    }

                    if (theBoard.boardChanged == true)
                    {
                        theBoard.boardChanged = false;
                        gameState = animating;
                        int tempRand = myRandom.Next(0, 3);
                        boomSounds[tempRand].Play();
                    }
                    else
                    {
                        if (fallCount > 1)
                        {
                            gameState = playing;
                            fallCount = 0;
                        }
                        else
                        {
                            gameState = gameOver;
                            fallCount = 0;
                            MediaPlayer.Stop();
                        }
                    }
                }
                else
                {
                    fallCount++;

                    //theBoard.fillEmpty();

                    for (int y = 0; y < whatHasFell.Length; y++)
                    {
                        if (whatHasFell[y] == true)
                        {
                            theBoard.matchTiles(y);
                            whatHasFell[y] = false;
                        }
                    }

                    if (theBoard.boardChanged == true)
                    {
                        theBoard.boardChanged = false;
                        gameState = animating;
                    }
                    
                    startUp = false;
                }
            }
            else if (gameState == gameOver)
            {
                if (!(MediaPlayer.State == MediaState.Playing))
                {
                    MediaPlayer.Play(gameOverSong);
                }

                if (myMouseState.LeftButton == ButtonState.Pressed)
                {
                    gameState = starting;
                    myTime = 0;
                    MediaPlayer.Stop();
                    if (myHighScore < theBoard.playerScore)
                    {
                        myHighScore = theBoard.playerScore;
                    }
                    InitiateSave();
                    showHighScore = "High Score: " + myHighScore;
                }
            }
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            if (gameState == starting)
            {
                spriteBatch.Draw(startImage, backgroundRect, Color.White);
            }
            else
            {
                spriteBatch.Draw(backgroundImage, backgroundRect, Color.White);
                spriteBatch.Draw(behindImage, behindRect, Color.White);

                for (int width = 0; width < theBoard.boardWidth; width++)
                {
                    for (int height = 0; height < theBoard.boardHeight; height++)
                    {
                        if (theBoard.myPlacements[width, height].occupied == true)
                        {
                            spriteBatch.Draw(theBoard.myPlacements[width, height].theTile.myImage, placementRects[width, height], animationRect[theBoard.myPlacements[width, height].theTile.animationFrame / 3], Color.White);
                        }
                    }
                }

                for (int arrowCount = 0; arrowCount < 4; arrowCount++)
                {
                    spriteBatch.Draw(myArrowTextures[arrowCount], myArrowRects[arrowCount], myArrowFrame[arrowCount], Color.White);
                }

                spriteBatch.DrawString(font, playerScore, scoreVector, Color.White);
                spriteBatch.DrawString(font, showTime, timeVector, Color.White);
                spriteBatch.DrawString(font, showHighScore, highScoreVec, Color.White);

                if (gameState == gameOver)
                {
                    spriteBatch.Draw(gameOverTexture, gameOverRect, Color.White);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool isClick(MouseState myState, Rectangle myRect)
        {
            if (myState.X > myRect.X + myRect.Width || myState.X < myRect.X || myState.Y > myRect.Y + myRect.Height || myState.Y < myRect.Y)
            {
                return false;
            }

            return true;
        }
    }
}
