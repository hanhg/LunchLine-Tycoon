using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LunchLineTycoonRemake
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Keyboard controls for starting and restarting the game
        KeyboardState kb, oldKb;
        GamePadState oldGp;

        //Controls the game state
        GameState gameState;

        Texture2D startScreen;

        public Rectangle window;

        //The level
        Level level;

        //Timers for text throughout level
        int timer, oldTimer;

        //For text
        SpriteFont font;

        //text for resizing screen size
        String resizeText;

        double earnings;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            DisplayMode desktop = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            graphics.PreferredBackBufferWidth = 1008;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            SoundEffect.MasterVolume = 0.5f;
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

            window = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            timer = 0;
            oldTimer = 0;
            resizeText = "Resize The Screen:\nRight Trigger : Increase Screen Size\nLeft Trigger : Decrease Screen Size";
            oldGp = GamePad.GetState(PlayerIndex.One);
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

            startScreen = Content.Load<Texture2D>("Start Screen");

            font = Content.Load<SpriteFont>("GameFont");
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
            timer++;
            kb = Keyboard.GetState();
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();


            //TODO: Add your update logic here
            if (gameState == GameState.startScreen)
            {
                if (pad.IsButtonDown(Buttons.RightTrigger) && !oldGp.IsButtonDown(Buttons.RightTrigger))
                {
                    IncreaseScreen();
                }
                if (pad.IsButtonDown(Buttons.LeftTrigger) && !oldGp.IsButtonDown(Buttons.LeftTrigger))
                {
                    DecreaseScreen();
                }
                if ((kb.IsKeyDown(Keys.Enter) && !oldKb.IsKeyDown(Keys.Enter)) || pad.IsButtonDown(Buttons.Start))
                {
                    gameState = GameState.level1;
                    level = new Level(@"Content/Levels/level01.txt", Content, GraphicsDevice, gameState);
                    oldTimer = timer;
                }

            }
            if (gameState == GameState.level1)
            {
                //level.Update(gameTime, spriteBatch);
                if (level.IsLevelComplete())
                {
                    earnings = level.money;
                    level.music.Stop();
                    level.ding.Play();
                    gameState = GameState.endScreen;
                    level = new Level(@"Content/Levels/level02.txt", Content, GraphicsDevice, gameState);
                }
                else
                {
                    level.Update(gameTime, spriteBatch);
                }
            }
            if (gameState == GameState.level2)
            {
                //level.Update(gameTime, spriteBatch);
                if (level.IsLevelComplete())
                {
                    earnings = level.money;
                    level.music.Stop();
                    level.ding.Play();
                    gameState = GameState.endScreen;
                    level = new Level(@"Content/Levels/level02.txt", Content, GraphicsDevice, gameState);
                }
                else
                {
                    level.Update(gameTime, spriteBatch);
                }
            }
            if (gameState == GameState.endScreen)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(0).IsButtonDown(Buttons.Start))
                {
                    gameState = GameState.level2;
                }
            }
            oldGp = pad;

            oldKb = kb;
            base.Update(gameTime);
        }

        public void IncreaseScreen()
        {
            int increment = 20;
            if (window.Width < 1920)
            {
                window.Width += increment;
                window.Height = (int)(window.Width / 1.4);
                resizeScreen(window.Width, window.Height);
            }
        }
        public void DecreaseScreen()
        {
            int increment = 20;
            if (window.Width > 720)
            {
                window.Width -= increment;
                window.Height = (int)(window.Width / 1.4);
                resizeScreen(window.Width, window.Height);
            }
        }

        public void resizeScreen(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (gameState == GameState.startScreen)
            {
                spriteBatch.Draw(startScreen, window, Color.White);
                spriteBatch.DrawString(font, resizeText, Vector2.Zero, Color.Black, 0f, Vector2.Zero, (float)(window.Width / 1.5 / 1008), SpriteEffects.None, 0);
            }
            if (gameState == GameState.level1)
            {
                level.Draw(gameTime, spriteBatch);
                if (timer - oldTimer < 300)
                {
                    spriteBatch.DrawString(font, "Press X to interact with stations\nKeep the stations filled with food from the conveyor belt" +
                        "\nServe students at stations quickly to keep them happy\nGood luck", new Vector2(GraphicsDevice.Viewport.Width / 2 - 200, 10), Color.Black,
                        0f, Vector2.Zero, (float)(window.Width / 1.5 / 1008), SpriteEffects.None, 0);
                }
            }
            if (gameState == GameState.level2)
            {
                level.Draw(gameTime, spriteBatch);
                if (timer - oldTimer < 300)
                {
                    //    spriteBatch.DrawString(font, "Press X to interact with stations\nKeep the stations filled with food from the conveyor belt" +
                    //        "\nServe students at stations quickly to keep them happy\nGood luck", new Vector2(GraphicsDevice.Viewport.Width / 2 - 200, 10), Color.White);
                }
            }
            if (gameState == GameState.endScreen)
            {
                spriteBatch.DrawString(font, "You have earned $" + earnings + " dollars", new Vector2(GraphicsDevice.Viewport.Width / 2 - GraphicsDevice.Viewport.Width / 10,
                    GraphicsDevice.Viewport.Height / 2), Color.White, 0f, Vector2.Zero, (float)(window.Width / 1.2 / 1008), SpriteEffects.None, 0);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
