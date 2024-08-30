using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;

namespace LunchLineTycoonRemake
{
    public class Level
    {

        //Width and height of the level
        public const int TileMapWidth = 14;
        public const int TileMapHeight = 10;

        //max amount of layer depths
        public const int maxLayerDepths = 8;

        //Width and height of a tile
        public int tileWidth;
        public int tileHeight;

        //Level file contains a series of 2 digit numbers matching the TileMapWidth and TileMapHeight ie 00 02 21 (To denote what type of tile it is and what sprite to load)
        public Tile[,] tileMap;

        //List of all stations to make updating easier
        public List<Station> stations;

        //state of game
        public GameState gameState;

        //Array of textures used for tiles
        public Texture2D[] tileTextures;

        public List<Child> children;
        public List<Player> players;

        public Texture2D[] foodTextures;
        public Texture2D[] ingredientTextures;

        public List<Item> items;

        public List<CookingStation> cookingStations;

        public List<Conveyor> conveyers;

        public List<Counter> counters;

        public List<TrashCan> trashCans;

        public Random rnd;

        //Money that the entire team made
        public double money;

        public ContentManager content;

        public GraphicsDevice graphics;

        public Dictionary<String, float> layerDepths;

        //Sound Effects
        public SoundEffect ding;
        public SoundEffect partDing;
        public SoundEffect badDing;

        public int musicTimer;
        public int musicLength;
        public SoundPlayer music;

        public SpriteFont font;

        //Creates a new instance of the level class. Generates the children in the level and contains player data
        public Level(String fileName, ContentManager content, GraphicsDevice graphics, GameState gs)
        {
            this.content = content;
            this.graphics = graphics;
            Random rnd = new Random();
            gameState = gs;
            setLayerDepths();
            tileMap = new Tile[TileMapWidth, TileMapHeight];
            stations = new List<Station>();
            players = new List<Player>();
            items = new List<Item>();
            conveyers = new List<Conveyor>();
            cookingStations = new List<CookingStation>();
            counters = new List<Counter>();
            trashCans = new List<TrashCan>();
            children = new List<Child>();
            tileTextures = new Texture2D[100];
            tileWidth = graphics.Viewport.Width / TileMapWidth;
            tileHeight = graphics.Viewport.Height / TileMapHeight;
            foodTextures = new Texture2D[3];
            ingredientTextures = new Texture2D[4];
            money = 0;
            this.rnd = rnd;
            LoadContent(content);
            LoadLevel(fileName, content);

            musicLength = 60 * (60 + 44);
            musicTimer = musicLength + 1;

            for (int i = 0; i < 4; i++)
            {
                //Creates a random type of food using the Food class' foodTypeArray
                FoodType type = (FoodType)Food.foodTypeArray.GetValue(rnd.Next(Food.foodTypeArray.Length));
                //TO IMPLEMENT LATER
                Food food = new Food(Rectangle.Empty, null, Vector2.Zero, type, layerDepths["Items"]);
                children.Add(randomChild(new Random()));
            }
            for (int i = 0; i < children.Count(); i++)
            {
                stations[0].childrenWaiting.Enqueue(children[i]);
            }
        }

        //Updates each child as well (Add things like level time in future)
        public void Update(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < stations.Count(); i++)
            {
                stations[i].Update(gameTime, spriteBatch);
            }
            for (int i = 0; i < players.Count(); i++)
            {

                if (players[i].playerInteractedWithStation)
                {

                    for (int j = 0; j < stations.Count(); j++)
                    {
                        if (stations[j].childrenWaiting.Count() > 0)
                        {
                            if (stations[j].equals(new Rectangle(players[i].interactedStation.rectangle.X,
                                players[i].interactedStation.rectangle.Y, 0, 0)))
                            {
                                Child tempChild = stations[j].childrenWaiting.Dequeue();
                                tempChild.order.foodRequirements.RemoveAt(0);
                                if (tempChild.order.IsComplete())
                                {
                                    money += tempChild.pay();
                                }
                                stations[j].foodBeingHeld = null;
                                if (j < stations.Count() - 1 && !tempChild.order.IsComplete())
                                {
                                    tempChild.rectangle = new Rectangle(stations[j + 1].rectangle.X, stations[j + 1].rectangle.Y - tileHeight - 5, tileWidth, tileHeight);
                                    stations[j + 1].childrenWaiting.Enqueue(tempChild);
                                }
                            }
                        }
                    }
                }
                players[i].Update(gameTime, spriteBatch);
            }
            for (int i = 0; i < stations.Count(); i++)
            {
                Child[] arr = stations[i].childrenWaiting.ToArray();
                for (int j = 0; j < arr.Length; j++)
                {
                    arr[j].Update(gameTime, spriteBatch);
                }
            }
            for (int i = 0; i < conveyers.Count(); i++)
            {
                conveyers[i].Update(gameTime, spriteBatch);
            }

            //for music to repeat
            if (musicTimer > musicLength)
            {
                music.Stop();
                music.Play();
                musicTimer = 0;
            }
            else
                musicTimer++;

        }

        public void LoadContent(ContentManager content)
        {
            tileTextures[0] = content.Load<Texture2D>("Tiles/Wood");
            tileTextures[1] = content.Load<Texture2D>("Tiles/Stations/Empty Station");
            tileTextures[2] = content.Load<Texture2D>("Tiles/Counters/Front Facing Counter");
            tileTextures[3] = content.Load<Texture2D>("Tiles/Counters/Side Counter");
            tileTextures[4] = content.Load<Texture2D>("Tiles/Counters/Corner Counter");
            tileTextures[5] = content.Load<Texture2D>("Tiles/Tile");
            tileTextures[6] = content.Load<Texture2D>("Tiles/Ice");
            tileTextures[7] = content.Load<Texture2D>("Tiles/Back Counter Conveyor");

            tileTextures[8] = content.Load<Texture2D>("Tiles/Stations/Cooking Stations/Side Counter Blender");
            tileTextures[9] = content.Load<Texture2D>("Tiles/Stations/Cooking Stations/Side Counter Cutting Board");
            tileTextures[10] = content.Load<Texture2D>("Tiles/Stations/Cooking Stations/Side Counter Pot");
            tileTextures[11] = content.Load<Texture2D>("Tiles/Stations/Cooking Stations/Side Counter Blender In Use");
            tileTextures[12] = content.Load<Texture2D>("Tiles/Stations/Cooking Stations/Side Counter Cutting Board In Use");
            tileTextures[13] = content.Load<Texture2D>("Tiles/Stations/Cooking Stations/Side Counter Pot In Use");

            tileTextures[14] = content.Load<Texture2D>("Tiles/Counters/Back Facing Counter");

            tileTextures[15] = content.Load<Texture2D>("Tiles/Counters/Trash");


            ingredientTextures[0] = content.Load<Texture2D>("Ingredients/Sack of Lettuce");
            ingredientTextures[1] = content.Load<Texture2D>("Ingredients/Sack of Tomatoes");
            ingredientTextures[2] = content.Load<Texture2D>("Ingredients/Sack of Uncooked Pasta");
            ingredientTextures[3] = content.Load<Texture2D>("Ingredients/Sack of Potatoes");

            foodTextures[0] = content.Load<Texture2D>("Food/Salad Bowl");
            foodTextures[1] = content.Load<Texture2D>("Food/Pasta Bowl");
            foodTextures[2] = content.Load<Texture2D>("Food/Mashed Potato Bowl");

            ding = content.Load<SoundEffect>("Sounds/ding");
            partDing = content.Load<SoundEffect>("Sounds/ding2");
            badDing = content.Load<SoundEffect>("Sounds/badDing");
            music = new SoundPlayer(@"Content/Sounds/music.wav");

            font = content.Load<SpriteFont>("GameFont");
        }

        //    //Loads the level from a file of numbers
        public void LoadLevel(String fileName, ContentManager content)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    int height = 0;
                    int playerIndex = 0;
                    while (!reader.EndOfStream)
                    {
                        String[] line;
                        do
                        {
                            line = reader.ReadLine().Split(' ');
                            if (line.Length == 2)
                            {
                                players.Add(new Player(new Rectangle(Int32.Parse(line[0]) * tileWidth, Int32.Parse(line[1]) * tileHeight, tileWidth, tileHeight),
                                    Vector2.Zero, (PlayerIndex)playerIndex, (Level)this.MemberwiseClone(), content, layerDepths["Humanoid"]));
                                playerIndex++;
                            }
                            if (reader.EndOfStream)
                                goto Out;
                        } while (line.Length == 2);
                        for (int i = 0; i < TileMapWidth; i++)
                        {
                            int tileType = Int32.Parse(line[i]);
                            switch (tileType)
                            {
                                case 0:
                                    tileMap[i, height] = new Tile(tileWidth, tileHeight, i, height, false, content, tileTextures[0], layerDepths["Floor"]);
                                    break;
                                case 1:
                                    //temp food capacity number
                                    int temp = 4;
                                    tileMap[i, height] = new Station(tileWidth, tileHeight, i, height, temp, content, tileTextures[1], layerDepths["Counters"]);
                                    stations.Add((Station)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    break;
                                case 2:
                                    tileMap[i, height] = new Counter(tileWidth, tileHeight, i, height, content, tileTextures[2], layerDepths["Counters"]);
                                    counters.Add((Counter)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    break;
                                case 3:
                                    tileMap[i, height] = new Counter(tileWidth, tileHeight, i, height, content, tileTextures[3], layerDepths["Counters"]);
                                    counters.Add((Counter)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    break;
                                case 4:
                                    tileMap[i, height] = new Counter(tileWidth, tileHeight, i, height, content, tileTextures[3], layerDepths["Counters"]);
                                    counters.Add((Counter)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    tileMap[i, height].effect = SpriteEffects.FlipHorizontally;
                                    break;
                                case 5:
                                    tileMap[i, height] = new Counter(tileWidth, tileHeight, i, height, content, tileTextures[4], layerDepths["Counters"]);
                                    counters.Add((Counter)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    break;
                                case 6:
                                    tileMap[i, height] = new Counter(tileWidth, tileHeight, i, height, content, tileTextures[4], layerDepths["Counters"]);
                                    counters.Add((Counter)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    tileMap[i, height].effect = SpriteEffects.FlipHorizontally;
                                    break;
                                case 7:
                                    tileMap[i, height] = new Tile(tileWidth, tileHeight, i, height, false, content, tileTextures[5], layerDepths["Floor"]);
                                    break;
                                case 8: //Cooking Station - Blender
                                    tileMap[i, height] = new CookingStation(tileWidth, tileHeight, i, height, content, tileTextures[8], layerDepths["Counters"]);
                                    cookingStations.Add((CookingStation)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    cookingStations[cookingStations.Count - 1].type = CookingStationType.blend;
                                    break;
                                case 9: //Conveyer
                                    tileMap[i, height] = new Conveyor(tileWidth, tileHeight, i, height, tileTextures[7], new Random(), (Level)this.MemberwiseClone(), content, layerDepths["Counters"]);
                                    conveyers.Add((Conveyor)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    break;
                                case 10: //Ice
                                    tileMap[i, height] = new Tile(tileWidth, tileHeight, i, height, false, content, tileTextures[6], layerDepths["Floor"]);
                                    break;
                                case 11: //Bottom Countertop
                                    tileMap[i, height] = new Counter(tileWidth, tileHeight, i, height, content, tileTextures[14], layerDepths["Counters"]);
                                    counters.Add((Counter)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    break;
                                case 12: //Cooking Station - Cutting Board
                                    tileMap[i, height] = new CookingStation(tileWidth, tileHeight, i, height, content, tileTextures[9], layerDepths["Counters"]);
                                    cookingStations.Add((CookingStation)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    cookingStations[cookingStations.Count - 1].type = CookingStationType.chop;
                                    break;
                                case 13: //Cooking Station - Pot
                                    tileMap[i, height] = new CookingStation(tileWidth, tileHeight, i, height, content, tileTextures[10], layerDepths["Counters"]);
                                    cookingStations.Add((CookingStation)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    cookingStations[cookingStations.Count - 1].type = CookingStationType.boil;
                                    break;
                                case 14:
                                    tileMap[i, height] = new TrashCan(tileWidth, tileHeight, i, height, content, tileTextures[15], layerDepths["Counters"]);
                                    trashCans.Add((TrashCan)tileMap[i, height]);
                                    tileMap[i, height].canCollide = true;
                                    break;

                                    //Later add other tiles, player, and children loading
                            }
                            tileMap[i, height].level = this;
                        }
                        height++;
                    }
                Out:
                    ;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            List<Sprite> pictures = new List<Sprite>();
            for (int x = 0; x < tileMap.GetLength(0); x++)
            {
                for (int y = 0; y < tileMap.GetLength(1); y++)
                {
                    pictures.Add(tileMap[x, y]);
                }
            }
            for (int x = 0; x < players.Count; x++)
            {
                pictures.Add(players[x].shadow);
            }


            for (int x = 0; x < stations.Count; x++)
            {
                //pictures.Add(stations[x]);
                for (int i = stations[x].childrenWaiting.Count() - 1; i >= 0; i--)
                {
                    pictures.Add(stations[x].childrenWaiting.ToArray()[i]);
                }
            }

            for (int x = 0; x < players.Count; x++)
            {
                pictures.Add(players[x]);
            }
            for (int i = 0; i < items.Count; i++)
            {
                pictures.Add(items[i]);
            }

            //Draw pictures in order according to layer depth
            for (int j = 0; j <= maxLayerDepths; j++)
            {
                for (int i = 0; i < pictures.Count; i++)
                {
                    if (pictures[i].layerDepth == j)
                        pictures[i].Draw(gameTime, spriteBatch);
                }
            }
        }

        public bool IsLevelComplete()
        {
            bool complete = true;
            for (int i = 0; i < stations.Count(); i++)
            {
                if (stations[i].childrenWaiting.Count() > 0)
                {
                    complete = false;
                }
            }
            return complete;
        }

        //Randomly creates a random set of children with random tasks
        public Child randomChild(Random rnd)
        {

            Child rtn = new Child(new Task(Rectangle.Empty, null, Vector2.Zero, layerDepths["Items"]), (Level)this.MemberwiseClone(), content, tileWidth, tileHeight, 1, 1, layerDepths["Floor"]);

            int numberOfFood = rnd.Next(stations.Count - 1) + 2;

            for (int i = 0; i < numberOfFood; i++)
            {
                FoodType food = Food.RandomFoodType(rnd);
                rtn.order.AddFood(new Food(Rectangle.Empty, null, Vector2.Zero, food, layerDepths["Items"]));
            }

            rtn.rectangle = new Rectangle(stations[0].rectangle.X, stations[0].rectangle.Y - rtn.rectangle.Height - 5, rtn.rectangle.Width, rtn.rectangle.Height);

            return rtn;
        }

        public void setLayerDepths()
        {
            layerDepths = new Dictionary<string, float>();
            layerDepths.Add("Floor", 0);
            layerDepths.Add("Shadow", 1);
            layerDepths.Add("Counters", 2);
            layerDepths.Add("Items", 3);
            layerDepths.Add("Humanoid", 5);

        }
    }
}
