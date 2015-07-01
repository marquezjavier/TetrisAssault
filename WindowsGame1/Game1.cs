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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace TetrisAssault
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Objects
        GraphicsDeviceManager graphics;
        SpriteFont gameFont;
        SpriteFont instructions;
        SpriteFont gameOverScore;
        SpriteBatch spriteBatch;
        Target target;
        Texture2D background;
        Texture2D startScreen;
        Texture2D failScreen;

        Random randomNum = new Random();        //Random number generator

        //Set the sound effects to use
        SoundEffect switchSound;
        SoundEffect matchSound;
        Song bgMusic;

        //Array of blocks
        Block[,] Blocks = new Block[6, 13];     

        //For Single Key Strokes
        bool canMoveUp = true;
        bool canMoveDown = true;
        bool canMoveLeft = true;
        bool canMoveRight = true;
        bool canSpace = true;

        //Tracks score and link
        int score;
        int link;

        //Keeps track of events
        bool songstart = false;
        float timer = 0f;
        float interval = 10f; //10 Seconds, new Row

        byte state;           /*Controls which state the game is in
                               * 0 is the start page
                               * 1 is game play
                               * 2 is game over screen */

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Window Size
            graphics.PreferredBackBufferWidth = 400;
            graphics.PreferredBackBufferHeight = 550;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Set defaults
            state = 0;
            score = 0;
            link = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Load Music
            switchSound = Content.Load<SoundEffect>("switch");
            matchSound = Content.Load<SoundEffect>("match");
            bgMusic = Content.Load<Song>("daydreaming");
            MediaPlayer.IsRepeating = true; 
            
            //Load game fonts
            gameFont = Content.Load<SpriteFont>("SpriteFont1");
            gameOverScore = Content.Load<SpriteFont>("gameoverScore");
            instructions = Content.Load<SpriteFont>("instructions");

            //Load 2D textures
            target = new Target(Content.Load<Texture2D>("target"), new Vector2(15f, 14f), new Vector2(82f, 42f), new Vector2(0f, 0f));
            background = Content.Load<Texture2D>("backgroundTemplate");
            startScreen = Content.Load<Texture2D>("startScreen");
            failScreen = Content.Load<Texture2D>("failScreen");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            target.texture.Dispose();
            
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (state == 0)
            {
                //Start Screen State
                if (keyboardState.IsKeyUp(Keys.Space))
                    canSpace = true;

                if (keyboardState.IsKeyDown(Keys.Space)&& canSpace)
                {
                    state = 1;

                    //fills block array
                    resetGame();
                    canSpace = false;
                }
            }
            else if (state == 1)
            {
                //GamePlay State
                
                //Starts Playing the song
                if (!songstart)
                {
                    MediaPlayer.Play(bgMusic);
                    songstart = true;
                }

                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                // TODO: Add your update logic here

                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Add new row every 10 seconds
                if (timer > interval)
                {
                    addNewRow();
                    timer = 0f;
                }

                //Reset when keys are up
                if (keyboardState.IsKeyUp(Keys.Up))
                    canMoveUp = true;
                if (keyboardState.IsKeyUp(Keys.Down))
                    canMoveDown = true;
                if (keyboardState.IsKeyUp(Keys.Left))
                    canMoveLeft = true;
                if (keyboardState.IsKeyUp(Keys.Right))
                    canMoveRight = true;
                if (keyboardState.IsKeyUp(Keys.Space))
                    canSpace = true;


                //Move target

                //Up
                if (keyboardState.IsKeyDown(Keys.Up) && canMoveUp)
                {
                    if (target.Position.Y > 14)
                    {
                        target.Move(1, 40);
                        target.CurrentTargetY -= 1;
                    }
                    canMoveUp = false;
                }
                //Down
                if (keyboardState.IsKeyDown(Keys.Down) && canMoveDown)
                {
                    if (target.Position.Y + target.Size.Y < 536)
                    {
                        target.Move(0, 40);
                        target.CurrentTargetY += 1;
                    }
                    canMoveDown = false;
                }
                //Right
                if (keyboardState.IsKeyDown(Keys.Right) && canMoveRight)
                {
                    if (target.Position.X + target.Size.X < 257)
                    {
                        target.Move(2, 40);
                        target.CurrentTargetX += 1;
                    }
                    canMoveRight = false;
                }
                //Left
                if (keyboardState.IsKeyDown(Keys.Left) && canMoveLeft)
                {
                    if (target.Position.X > 15)
                    {
                        target.Move(3, 40);
                        target.CurrentTargetX -= 1;
                    }
                    canMoveLeft = false;
                }
                //Space
                if (keyboardState.IsKeyDown(Keys.Space) && canSpace)
                {
                    switchSound.Play();
                    //Switches blocks in array
                    Block tempBlock;

                    tempBlock = Blocks[target.CurrentTargetX, target.CurrentTargetY];
                    Blocks[target.CurrentTargetX, target.CurrentTargetY] = Blocks[target.CurrentTargetX + 1, target.CurrentTargetY];
                    Blocks[target.CurrentTargetX + 1, target.CurrentTargetY] = tempBlock;

                    //Updates the position of the blocks
                    Blocks[target.CurrentTargetX, target.CurrentTargetY].Position = new Vector2((Blocks[target.CurrentTargetX, target.CurrentTargetY].Size.X * target.CurrentTargetX + 16), (Blocks[target.CurrentTargetX, target.CurrentTargetY].Size.Y * target.CurrentTargetY + 15));
                    Blocks[target.CurrentTargetX + 1, target.CurrentTargetY].Position = new Vector2((Blocks[target.CurrentTargetX + 1, target.CurrentTargetY].Size.X * (target.CurrentTargetX + 1) + 16), (Blocks[target.CurrentTargetX + 1, target.CurrentTargetY].Size.Y * (target.CurrentTargetY) + 15));

                    canSpace = false;
                }

                //Fade matching blocks
                for (int i = 0; i < 13; i++)
                {
                    for (int r = 0; r < 6; r++)
                    {
                        if (Blocks[r, i].Visable)
                            Blocks[r, i].isDying();
                    }
                }

                //Calls gravity and match check
                gravity();
                checkForMatches();
            }
            else
            {
                //Lost Game State

                songstart = false;
                MediaPlayer.Pause();

                if (keyboardState.IsKeyUp(Keys.Space))
                    canSpace = true;

                if (keyboardState.IsKeyDown(Keys.Space)&& canSpace)
                {
                    resetGame();
                    state = 1;
                }

            }

            //Escape Program
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (state == 0)
            {
                //Draw start screen with instructions
                spriteBatch.Draw(startScreen, new Rectangle(0, 0, 400, 550), Color.LightGray);
                spriteBatch.DrawString(instructions, "- Use arrow Keys to move the target\n- Space switches Blocks in the targets" +
                 "\n- Get combinations of 3 or more blocks\n- Get more points for bigger cominations\n- Blocks fall down" +
                "\n- Do not let blocks reach the bottom",
                    new Vector2(45, 290), Color.White);
            }
            else if (state == 1)
            {
                //Render the game pay

                spriteBatch.Draw(background, new Rectangle(0, 0, 400, 550), Color.LightGray);

                //Draw score
                spriteBatch.DrawString(gameFont, score.ToString(),
                    new Vector2(293, 40), Color.White);

                //Draw link
                spriteBatch.DrawString(gameFont, link.ToString(),
                    new Vector2(305, 100), Color.White);

                //Render Blocks
                for (int i = 0; i < 13; i++)
                {
                    for (int r = 0; r < 6; r++)
                    {
                        if (Blocks[r, i].Visable)
                            Blocks[r, i].Draw(spriteBatch);
                    }
                }

                target.Draw(spriteBatch);
            }
            else
            {
                //Render lost screen

                spriteBatch.Draw(failScreen, new Rectangle(0, 0, 400, 550), Color.LightGray);
                spriteBatch.DrawString(gameOverScore, "Points: " + score.ToString(),
                    new Vector2(80, 350), Color.White);
            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        //used to check if blocks match sequencially
        public void checkForMatches()
        {
            int currentColor;

            for (int i = 0; i < 13; i++)
            {
                for (int r = 0; r < 6; r++)
                {
                    if (!Blocks[r, i].isFalling)
                    {
                        currentColor = Blocks[r, i].Color;
                        //checks for 5 matching
                        if (i < 9 && Blocks[r, i].Visable && (Blocks[r, i + 1].Color == currentColor && Blocks[r, i + 2].Color == currentColor && Blocks[r, i + 3].Color == currentColor && Blocks[r, i + 4].Color == currentColor) &&
                           (Blocks[r, i + 1].Visable && Blocks[r, i + 2].Visable && Blocks[r, i + 3].Visable && Blocks[r, i + 4].Visable) &&
                            !(Blocks[r, i + 1].isDying() || Blocks[r, i + 2].isDying() || Blocks[r, i + 3].isDying() || Blocks[r, i + 4].isDying()))
                        {
                            Blocks[r, i].setDying(true);
                            Blocks[r, i + 1].setDying(true);
                            Blocks[r, i + 2].setDying(true);
                            Blocks[r, i + 3].setDying(true);
                            Blocks[r, i + 4].setDying(true);
                            score += 20;
                            matchSound.Play();
                        }
                        //checks for 4 matching
                        if (i < 10 && Blocks[r, i].Visable && (Blocks[r, i + 1].Color == currentColor && Blocks[r, i + 2].Color == currentColor && Blocks[r, i + 3].Color == currentColor) &&
                                (Blocks[r, i + 1].Visable && Blocks[r, i + 2].Visable && Blocks[r, i + 3].Visable) &&
                            !(Blocks[r, i + 1].isDying() || Blocks[r, i + 2].isDying() || Blocks[r, i + 3].isDying()))
                        {
                            Blocks[r, i].setDying(true);
                            Blocks[r, i + 1].setDying(true);
                            Blocks[r, i + 2].setDying(true);
                            Blocks[r, i + 3].setDying(true);
                            score += 10;
                            matchSound.Play();
                        }
                        //checks 3 blocks right
                        if (r < 4 && Blocks[r, i].Visable && (Blocks[r + 1, i].Color == currentColor && Blocks[r + 2, i].Color == currentColor) &&
                                (Blocks[r + 1, i].Visable && Blocks[r + 2, i].Visable) &&
                            !(Blocks[r + 1, i].isDying() || Blocks[r + 2, i].isDying())
                            && !Blocks[r + 1, i].isFalling && !Blocks[r + 2, i].isFalling)
                        {
                            Blocks[r, i].setDying(true);
                            Blocks[r + 1, i].setDying(true);
                            Blocks[r + 2, i].setDying(true);
                            score += 5;
                            matchSound.Play();
                        }

                        //checks 3 blocks down
                        if (i < 11 && Blocks[r, i].Visable && (Blocks[r, i + 1].Color == currentColor && Blocks[r, i + 2].Color == currentColor) &&
                                (Blocks[r, i + 1].Visable && Blocks[r, i + 2].Visable) &&
                            !(Blocks[r, i + 1].isDying() || Blocks[r, i + 2].isDying()))
                        {
                            Blocks[r, i].setDying(true);
                            Blocks[r, i + 1].setDying(true);
                            Blocks[r, i + 2].setDying(true);
                            score += 5;
                            matchSound.Play();
                        }
                    }
                }
            }
        }

        //Makes the blocks fall up
        public void gravity()
        {
            for (int i = 0; i < 13; i++)
            {
                for (int r = 0; r < 6; r++)
                {
                    if (i > 0 && Blocks[r, i].Visable)
                    {
                        if (!(Blocks[r, i - 1].Visable))
                        {
                            Blocks[r, i].isFalling = true;
                            Block tempBlock;

                            tempBlock = Blocks[r, i];
                            Blocks[r, i] = Blocks[r, i - 1];
                            Blocks[r, i - 1] = tempBlock;

                            //Updates the position of the blocks
                            Blocks[r, i].Position = new Vector2((Blocks[r, i].Size.X * r + 16), (Blocks[r, i].Size.Y * i + 15));
                            Blocks[r, i - 1].Position = new Vector2((Blocks[r, i - 1].Size.X * r + 16), (Blocks[r, i - 1].Size.Y * (i - 1) + 15));
                        }
                        else
                            Blocks[r, i].isFalling = false;
                    }
                    else if (i == 0)
                        Blocks[r, i].isFalling = false;     //Top row blocks cannot fall
                }
            }
        }

        //Adds a new row at the bottom
        public void addNewRow()
        {
            //If bottom row is empty
            if (!(Blocks[0, 12].Visable || Blocks[1, 12].Visable || Blocks[2, 12].Visable || Blocks[3, 12].Visable ||
                Blocks[4, 12].Visable || Blocks[5, 12].Visable))
            {
                int preColor;
                preColor = randomNum.Next(0, 5);
                bool match = true;

                for (int r = 0; r < 6; r++)
                {
                    //Sets block's color

                    match = true;
                    int tempColor;
                    tempColor = randomNum.Next(0, 5);
                    while (match)
                    {
                        if (tempColor == preColor)
                        {

                            tempColor = randomNum.Next(0, 5);
                        }
                        else if (12 != 0 && Blocks[r, (12 - 1)].Color == tempColor)
                        {
                            tempColor = randomNum.Next(0, 5);
                        }
                        else
                        {
                            Blocks[r, 12].Color = tempColor;
                            match = false;
                        }
                    }
                    preColor = tempColor;
                    Blocks[r, 12].Alpha = 200;
                    Blocks[r, 12].Visable = true;

                    //Sets block's Position
                    Blocks[r, 12].Position = new Vector2((Blocks[r, 12].Size.X * r + 16), (Blocks[r, 12].Size.Y * 12 + 15));

                    switch (Blocks[r, 12].Color)
                    {
                        case 0:
                            Blocks[r, 12].texture = Content.Load<Texture2D>("redBlock");
                            break;
                        case 1:
                            Blocks[r, 12].texture = Content.Load<Texture2D>("blueBlock");
                            break;
                        case 2:
                            Blocks[r, 12].texture = Content.Load<Texture2D>("greenBlock");
                            break;
                        case 3:
                            Blocks[r, 12].texture = Content.Load<Texture2D>("purpBlock");
                            break;
                        case 4:
                            Blocks[r, 12].texture = Content.Load<Texture2D>("tealBlock");
                            break;
                        case 5:
                            Blocks[r, 12].texture = Content.Load<Texture2D>("greyBlock");
                            break;
                    }
                }
            }
            else
            {
                //Lost the game
                state = 2;
            }
        }

        //Resets the game
        public void resetGame()
        {
            score = 0;
            link = 0;
            target.Position = new Vector2(15f, 14f);
            target.CurrentTargetX = 0;
            target.CurrentTargetY = 0;
            
            //Fill array with blocks
            int preColor;
            bool match = true;
            for (int i = 0; i < 13; i++)
            {
                preColor = randomNum.Next(0, 5);

                for (int r = 0; r < 6; r++)
                {

                    //sets block's color

                    match = true;
                    int tempColor;
                    Blocks[r, i] = new Block();
                    tempColor = randomNum.Next(0, 5);
                    while (match)
                    {
                        if (tempColor == preColor)
                        {

                            tempColor = randomNum.Next(0, 5);
                        }
                        else if (i != 0 && Blocks[r, (i - 1)].Color == tempColor)
                        {
                            tempColor = randomNum.Next(0, 5);
                        }
                        else
                        {
                            Blocks[r, i].Color = tempColor;
                            match = false;
                        }
                    }
                    preColor = tempColor;

                    Blocks[r, i].Alpha = 200;

                    //sets block's Position
                    Blocks[r, i].Position = new Vector2((Blocks[r, i].Size.X * r + 16), (Blocks[r, i].Size.Y * i + 15));

                    if (i > 7)
                        Blocks[r, i].Visable = false;

                    switch (Blocks[r, i].Color)
                    {
                        case 0:
                            Blocks[r, i].texture = Content.Load<Texture2D>("redBlock");
                            break;
                        case 1:
                            Blocks[r, i].texture = Content.Load<Texture2D>("blueBlock");
                            break;
                        case 2:
                            Blocks[r, i].texture = Content.Load<Texture2D>("greenBlock");
                            break;
                        case 3:
                            Blocks[r, i].texture = Content.Load<Texture2D>("purpBlock");
                            break;
                        case 4:
                            Blocks[r, i].texture = Content.Load<Texture2D>("tealBlock");
                            break;
                        case 5:
                            Blocks[r, i].texture = Content.Load<Texture2D>("greyBlock");
                            break;
                    }
                }
            }
        }
    }
}
