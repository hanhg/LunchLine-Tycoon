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
    public class Player : Sprite
    {
        //Controls the state of the player
        public PlayerState playerState;

        //Which controller the player uses, if applicable
        public PlayerIndex playerIndex;

        //Helps with stick drift and so that small inputs aren't accounted for
        public const double deadZone = .1;

        //Amount of pixels per frame that the player moves
        public int pixelsPerFrame = 10;

        //Used to store the more precise double value of the player's position
        public double xPrecise, yPrecise;

        //Distance to interact with a station in pixels
        public int interactionDistance;

        //Button used for interacting with a station
        public const Buttons interactButton = Buttons.X;

        //Rectangle for hitbox
        Rectangle hitbox;

        //Used for one frame events
        public GamePadState oldPad;
        public KeyboardState oldKb;

        //Used for ice mechanics
        public int direction;
        public int timer, oldTimer;

        //Backing level
        Level level;

        //State of game
        public GameState gameState;

        //Textures for front, back, and side to side
        Texture2D[] textures;
        //for his shadow
        public Item shadow;

        //Is this player facing left or right?
        SpriteEffects spriteEffects;

        //Money that this player made
        public double money;

        //Money that you earn per order completed.
        public double moneyPerOrder;

        //Player interaction variable
        public Boolean playerInteractedWithStation;
        public Station interactedStation;

        public Item heldItem;
        public float closestInteractableDistance;
        public Sprite closestInteractable;

        //button mashing
        public bool actionLock;
        public int timesButtonPressed = 0;
        public const int buttonPressedRequired = 11;
        //holds the food the player is making until they finish making it
        public Food waiting;
        public CookingStation stationInUse;

        //Random arrow key buttons
        public bool randomLock;
        public Queue<Direction> randomDirections;

        public Player(Rectangle rect, Vector2 origin,
            PlayerIndex playerIndex, Level level, ContentManager content, float layerDepth) : base(rect, null, origin, layerDepth)
        { //can use for when there are multiple levels?
            pixelsPerFrame = 5 * (level.graphics.Viewport.Width / 1008);
            interactionDistance = level.tileWidth;
            hitbox = new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height / 2);
            shadow = new Item(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + rect.Height / 2), null, Vector2.Zero, level.layerDepths["Shadow"]);
            textures = new Texture2D[3];
            LoadContent(content);
            this.playerIndex = playerIndex;
            this.level = level;
            gameState = level.gameState;
            playerState = PlayerState.Walking;
            heldItem = null;
            closestInteractable = null;
            closestInteractableDistance = int.MaxValue;
            actionLock = false;
            randomLock = false;
            direction = 1;
            timer = 0;
            oldTimer = 0;

        }

        public void LoadContent(ContentManager content)
        {
            textures[0] = content.Load<Texture2D>("Player/sam");
            textures[1] = content.Load<Texture2D>("Player/Backwards Sam");
            textures[2] = content.Load<Texture2D>("Player/Side Sam");
            texture = textures[0];

            shadow.texture = content.Load<Texture2D>("Player/Shadow");

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, null, Color.White, 0, origin, spriteEffects, 0);
            if (actionLock && level.gameState == GameState.level1)
            {
                spriteBatch.DrawString(level.font, "MASH X!", getCenter(), Color.White);
            }
        }

        public override void Update(GameTime gameTime, SpriteBatch spriteBatch)
        {

            //Loops through the values on the 2d array of tiles, and calls adjust player if the tile is not passable
            GamePadState pad = GamePad.GetState(playerIndex);
            KeyboardState kb = Keyboard.GetState();

            if (playerInteractedWithStation)
            {
                playerInteractedWithStation = false;
            }
            GetInput();

            for (int x = 0; x < level.tileMap.GetLength(0); x++)
            {
                for (int y = 0; y < level.tileMap.GetLength(1); y++)
                {
                    if (level.tileMap[x, y].canCollide)
                    {
                        AdjustPlayer(level.tileMap[x, y].hitBox);
                    }
                }
            }
            UpdateHitbox(direction);
            UpdateShadow();
            oldPad = pad;
            oldKb = kb;
            timer++;
        }

        //Used to control the player using the keyboard and controller
        public void GetInput()
        {
            KeyboardState kb = Keyboard.GetState();
            GamePadState pad = GamePad.GetState(playerIndex);
            if (actionLock)
            {
                if ((kb.IsKeyDown(Keys.F) && !oldKb.IsKeyDown(Keys.F)) || (pad.IsButtonDown(Buttons.X) && !oldPad.IsButtonDown(Buttons.X)))
                    timesButtonPressed++;
                if (timesButtonPressed > buttonPressedRequired)
                {
                    level.items.Remove(heldItem);
                    actionLock = false;
                    heldItem = waiting;
                    waiting = null;
                    stationInUse.ChangeUser(null);
                    stationInUse = null;
                    level.items.Add(heldItem);
                    level.partDing.Play();
                }
            }
            else
            {
                if (gameState == GameState.level2 && timer - oldTimer < 60)
                {
                    switch (direction)
                    {
                        case 1:
                            rectangle.X += (int)(pixelsPerFrame * 1.5 / (timer - oldTimer));
                            break;
                        case 2:
                            rectangle.X -= (int)(pixelsPerFrame * 1.5 / (timer - oldTimer));
                            break;
                        case 3:
                            rectangle.Y -= (int)(pixelsPerFrame * 1.5 / (timer - oldTimer));
                            break;
                        case 4:
                            rectangle.Y += (int)(pixelsPerFrame * 1.5 / (timer - oldTimer));
                            break;
                    }
                }

                if ((pad.ThumbSticks.Left.X > deadZone || kb.IsKeyDown(Keys.D))
                    && rectangle.X + rectangle.Width < level.tileWidth * level.tileMap.GetLength(0))
                {
                    rectangle.X += pixelsPerFrame;
                    texture = textures[2];
                    spriteEffects = SpriteEffects.FlipHorizontally;
                    direction = 1;
                    ResizeHitbox(direction);
                    oldTimer = timer;
                }
                if ((pad.ThumbSticks.Left.X < -deadZone || kb.IsKeyDown(Keys.A))
                    && rectangle.X > 0)
                {
                    rectangle.X -= pixelsPerFrame;
                    texture = textures[2];
                    spriteEffects = SpriteEffects.None;
                    direction = 2;
                    ResizeHitbox(direction);
                    oldTimer = timer;
                }
                if ((pad.ThumbSticks.Left.Y > deadZone || kb.IsKeyDown(Keys.W))
                    && rectangle.Y > 0)
                {
                    rectangle.Y -= pixelsPerFrame;
                    direction = 3;
                    ResizeHitbox(direction);
                    oldTimer = timer;
                    texture = textures[1];

                }
                if ((pad.ThumbSticks.Left.Y < -deadZone || kb.IsKeyDown(Keys.S))
                    && rectangle.Y + rectangle.Height < level.tileMap.GetLength(1) * level.tileHeight)
                {
                    rectangle.Y += pixelsPerFrame;
                    direction = 4;
                    ResizeHitbox(direction);
                    oldTimer = timer;
                    texture = textures[0];
                }
                AdjustItemsLayerDepth();


                if (heldItem != null)
                {
                    if (texture == textures[0])
                        heldItem.rectangle.X = rectangle.X;
                    else
                        heldItem.rectangle.X = rectangle.X + rectangle.Width / 2;
                    heldItem.rectangle.Y = rectangle.Y + rectangle.Height / 2;
                }

                if ((kb.IsKeyDown(Keys.F) && !oldKb.IsKeyDown(Keys.F)) || (pad.IsButtonDown(Buttons.X) && !oldPad.IsButtonDown(Buttons.X)))
                {
                    getItemsWithinRange();
                    //if item in range, pickup item
                    if (closestInteractable != null && closestInteractableDistance < interactionDistance)
                    //check if interact exists and is within range
                    {
                        if (closestInteractable is Item)
                        {
                            PickUpItem();
                            playerState = PlayerState.Carrying;
                            heldItem = (Item)closestInteractable;
                        }
                        else if (closestInteractable is Station && playerState == PlayerState.Carrying)
                        {
                            //loop through station list
                            //check if its the same station
                            //interact with it (.add or .remove food)\
                            if (heldItem != null && heldItem is Food)
                            {
                                ((Station)closestInteractable).AddFood((Food)heldItem);
                                level.items.Remove(heldItem);
                                heldItem = null;
                                playerState = PlayerState.Walking;
                            }
                        }
                        else if (closestInteractable is Station && playerState == PlayerState.Walking)
                        {
                            playerInteractedWithStation = true;
                            interactedStation = (Station)closestInteractable;
                        }
                        else if (closestInteractable is CookingStation && playerState == PlayerState.Carrying)
                        {
                            ((CookingStation)closestInteractable).ChangeUser(this);
                            ((CookingStation)closestInteractable).MakeFood();
                            stationInUse = ((CookingStation)closestInteractable);
                        }
                        else if (closestInteractable is Counter && playerState == PlayerState.Carrying)
                        {
                            ((Counter)closestInteractable).addItem(this);
                            playerState = PlayerState.Walking;
                        }
                        else if (closestInteractable is TrashCan && playerState == PlayerState.Carrying)
                        {
                            ((TrashCan)closestInteractable).ThrowAway(this);
                            playerState = PlayerState.Walking;
                        }
                    }
                }
            }
        }

        public void AdjustItemsLayerDepth()
        {
            if (heldItem != null)
            {
                if (texture == textures[1] || texture == textures[2] && spriteEffects == SpriteEffects.None)
                {
                    heldItem.layerDepth = level.layerDepths["Humanoid"] - 1;
                }
                else
                {
                    heldItem.layerDepth = level.layerDepths["Humanoid"] + 1;
                }
            }
        }

        public void getItemsWithinRange()
        {
            closestInteractable = null;
            closestInteractableDistance = level.tileWidth * 14;
            for (int x = 0; x < level.stations.Count; x++)
            {
                float distance = Vector2.Distance(level.stations[x].getCenter(), getCenter());
                if (distance < closestInteractableDistance && level.stations[x].childrenWaiting.Count > 0)
                {
                    //can only interact if food is in hand and it is the food the child wants
                    if (heldItem is Food)
                    {
                        if (level.stations[x].childrenWaiting.Peek().order.foodRequirements[0].equals((Food)heldItem))
                        {
                            closestInteractable = level.stations[x];
                            closestInteractableDistance = distance;
                        }
                    }
                    else if (heldItem == null && level.stations[x].foodBeingHeld != null)
                    {
                        if (level.stations[x].foodBeingHeld.equals(level.stations[x].childrenWaiting.Peek().order.foodRequirements[0]))
                        {
                            closestInteractable = level.stations[x];
                            closestInteractableDistance = distance;
                        }
                    }
                }
            }
            for (int x = 0; x < level.trashCans.Count; x++)
            {
                float distance = Vector2.Distance(level.trashCans[x].getCenter(), getCenter());
                if (distance < closestInteractableDistance)
                {
                    //can only interact if food is in hand and it is the food the child wants
                    if (heldItem != null)
                    {
                        closestInteractable = level.trashCans[x];
                        closestInteractableDistance = distance;
                    }
                }
            }
            for (int x = 0; x < level.counters.Count; x++)
            {
                float distance = Vector2.Distance(level.counters[x].getCenter(), getCenter());
                if (distance < closestInteractableDistance)
                {
                    //can only interact if food is in hand and it is the food the child wants
                    if ((heldItem != null && level.counters[x].held == null) || (heldItem != null && level.counters[x].held == null))
                    {
                        closestInteractable = level.counters[x];
                        closestInteractableDistance = distance;
                    }
                }
            }

            for (int x = 0; x < level.cookingStations.Count; x++)
            {
                float distance = Vector2.Distance(level.cookingStations[x].getCenter(), getCenter());
                if (distance < closestInteractableDistance)
                {
                    //can only interact if you have food and it fulfulls the right cooking station
                    if (heldItem is Ingredient)
                    {
                        if (level.cookingStations[x].type == CookingStationType.blend && ((Ingredient)heldItem).type == IngredientType.Potato)
                        {
                            closestInteractable = level.cookingStations[x];
                            closestInteractableDistance = distance;
                        }
                        else if (level.cookingStations[x].type == CookingStationType.chop
                            && (((Ingredient)heldItem).type == IngredientType.Tomato || ((Ingredient)heldItem).type == IngredientType.Lettuce))
                        {
                            closestInteractable = level.cookingStations[x];
                            closestInteractableDistance = distance;
                        }
                        else if (level.cookingStations[x].type == CookingStationType.boil
                            && (((Ingredient)heldItem).type == IngredientType.Tomato || ((Ingredient)heldItem).type == IngredientType.UncookedNoodle))
                        {
                            closestInteractable = level.cookingStations[x];
                            closestInteractableDistance = distance;
                        }
                    }

                }
            }

            if (heldItem == null)
            {
                for (int x = 0; x < level.items.Count; x++)
                {
                    float distance = Vector2.Distance(level.items[x].getCenter(), getCenter());
                    if (distance < closestInteractableDistance)
                    {
                        closestInteractable = level.items[x];
                        closestInteractableDistance = distance;
                    }
                }
            }

        }

        public void UpdateShadow()
        {
            shadow.rectangle.X = rectangle.X - rectangle.Width / 2;
            shadow.rectangle.Y = rectangle.Y - rectangle.Height / 2;
        }
        public void UpdateHitbox(int direction)
        {
            if (direction == 1 || direction == 2)
                hitbox.X = hitbox.X = rectangle.X + rectangle.Width / 4;
            else
                hitbox.X = rectangle.X;
            hitbox.Y = rectangle.Y + rectangle.Height / 2;
        }
        public void ResizeHitbox(int direction)
        {
            if (direction == 1 || direction == 2)
            {
                hitbox.X = rectangle.X + rectangle.Width / 4;
                hitbox.Width = rectangle.Width / 2;
            }
            else
            {
                hitbox.X = rectangle.X;
                hitbox.Width = rectangle.Width;
            }
        }

        //Adjusts the player based on the intersection depth of the other rectangle
        public void AdjustPlayer(Rectangle other)
        {
            Vector2 adjustments = RectangleExtensions.GetIntersectionDepth(hitbox, other);
            if (Math.Abs(adjustments.X) < Math.Abs(adjustments.Y))
            {
                rectangle.X += (int)adjustments.X;
            }
            if (Math.Abs(adjustments.X) > Math.Abs(adjustments.Y))
            {
                rectangle.Y += (int)adjustments.Y;
            }
            UpdateHitbox(direction);
            UpdateShadow();
        }

        public void PickUpItem()
        {
            GamePadState pad = GamePad.GetState(playerIndex);

            if (playerState == PlayerState.Walking && pad.Buttons.X == ButtonState.Pressed && oldPad.Buttons.X == ButtonState.Released)
            {
                if (closestInteractable is Item item)
                {
                    playerState = PlayerState.Carrying;
                    item.isBeingHeld = true;
                    if (item.isOnConveyer)
                    {
                        item.isOnConveyer = false;
                        item.conveyer.hasFood = false;
                    }
                    for (int j = 0; j < level.counters.Count; j++)
                    {
                        if (level.counters[j].held == item)
                        {
                            level.counters[j].held = null;
                        }
                    }

                }
            }
        }
        public void ButtonMash()
        {
            timesButtonPressed = 0;
            actionLock = true;
        }

        public void RandomButton()
        {
            Random ran = new Random();

            GamePadState pad = GamePad.GetState(playerIndex);

            randomDirections = new Queue<Direction>();

            for (int i = 0; i < buttonPressedRequired; i++)
            {
                randomDirections.Enqueue((Direction)ran.Next(4));
            }


            while (timesButtonPressed < buttonPressedRequired)
            {
                int temp = ran.Next(4);
                switch (temp)
                {
                    case (0):
                        if (pad.DPad.Up == ButtonState.Pressed)
                        {
                            //level.partDing.Play();
                            timesButtonPressed++;
                        }
                        //else if (pad.DPad != null)
                        //level.badDing.Play();
                        break;
                    case (1):
                        if (pad.DPad.Down == ButtonState.Pressed)
                        {
                            level.partDing.Play();
                            timesButtonPressed++;
                        }
                        //else if (pad.DPad != null)
                        //level.badDing.Play();
                        break;
                    case (2):
                        if (pad.DPad.Left == ButtonState.Pressed)
                        {
                            //level.partDing.Play();
                            timesButtonPressed++;
                        }
                        //else if (pad.DPad != null)
                        //level.badDing.Play();
                        break;
                    case (3):
                        if (pad.DPad.Right == ButtonState.Pressed)
                        {
                            //level.partDing.Play();
                            timesButtonPressed++;
                        }
                        //else if (pad.DPad != null)
                        //level.badDing.Play();
                        break;
                }
            }
        }
    }
}
