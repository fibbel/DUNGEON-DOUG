using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Net;
using Microsoft.VisualBasic;

namespace DUNGEON_DOUG_PORT;

public class Game1 : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch sb;
    private SpriteFont font;
    private KeyboardState kb, oldkb;
    private Player player;
    private Texture2D pixel, coin, titleScreen, bg, helmet, bigdoug, flower, dead;
    private List<Texture2D> walls;
    private List<Texture2D> doors;
    private List<Texture2D> acards, bcards;
    private List<Texture2D> hearts;
    private List<Texture2D> enemies;
    private List<Texture2D> doug;
    private List<Texture2D> weapons;
    private List<Rectangle> wallRects;
    private List<Fire> fires;

    private Map map;

    private int gameState, gameTimer, gameMins, gameSecs, victoryPlayed, currentSong, queuedSong;
    private bool muted;

    private SoundEffectInstance menuMusic;
    private List<SoundEffect> soundEffects;
    private List<SoundEffectInstance> music;

    private Random rg = new Random();


    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        graphics.PreferredBackBufferHeight = 800;
        graphics.PreferredBackBufferWidth = 800;

        Window.Title = "DUNGEON DOUG.";
    }

    protected override void Initialize()
    {
        fires = new List<Fire>();
        walls = new List<Texture2D>();
        doors = new List<Texture2D>();
        acards = new List<Texture2D>();
        bcards = new List<Texture2D>();
        hearts = new List<Texture2D>();
        enemies = new List<Texture2D>();
        doug = new List<Texture2D>();
        weapons = new List<Texture2D>();
        music = new List<SoundEffectInstance>();

        wallRects = new List<Rectangle>();
        wallRects.Add(new Rectangle(0, 0, 800, 80));
        wallRects.Add(new Rectangle(720, 0, 80, 800));
        wallRects.Add(new Rectangle(0, 720, 800, 80));
        wallRects.Add(new Rectangle(0, 0, 80, 800));

        gameState = -1;
        gameTimer = 0;
        gameMins = 0;
        gameSecs = 0;
        victoryPlayed = 0;
        currentSong = -1;
        queuedSong = 0;
        muted = false;

        soundEffects = new List<SoundEffect>();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        sb = new SpriteBatch(GraphicsDevice);

        font = Content.Load<SpriteFont>("font");

        pixel = Content.Load<Texture2D>("pixel");
        walls.Add(Content.Load<Texture2D>("wallTop"));
        walls.Add(Content.Load<Texture2D>("wallRight"));
        walls.Add(Content.Load<Texture2D>("wallBottom"));
        walls.Add(Content.Load<Texture2D>("wallLeft"));
        walls.Add(Content.Load<Texture2D>("wall with door 0"));
        walls.Add(Content.Load<Texture2D>("wall with door 1"));
        walls.Add(Content.Load<Texture2D>("wall with door 2"));
        walls.Add(Content.Load<Texture2D>("wall with door 3"));
        doors.Add(Content.Load<Texture2D>("locked door 0"));
        doors.Add(Content.Load<Texture2D>("locked door 1"));
        doors.Add(Content.Load<Texture2D>("locked door 2"));
        doors.Add(Content.Load<Texture2D>("locked door 3"));
        doors.Add(Content.Load<Texture2D>("unlocked door 0"));
        doors.Add(Content.Load<Texture2D>("unlocked door 1"));
        doors.Add(Content.Load<Texture2D>("unlocked door 2"));
        doors.Add(Content.Load<Texture2D>("unlocked door 3"));

        acards.Add(Content.Load<Texture2D>("basic attack"));
        acards.Add(Content.Load<Texture2D>("spin attack"));
        acards.Add(Content.Load<Texture2D>("dash attack"));
        acards.Add(Content.Load<Texture2D>("light spell"));
        acards.Add(Content.Load<Texture2D>("heavy spell"));

        bcards.Add(Content.Load<Texture2D>("longsword"));
        bcards.Add(Content.Load<Texture2D>("luck"));
        bcards.Add(Content.Load<Texture2D>("attack"));
        bcards.Add(Content.Load<Texture2D>("speed"));
        bcards.Add(Content.Load<Texture2D>("heartcontainer"));
        bcards.Add(Content.Load<Texture2D>("key"));

        hearts.Add(Content.Load<Texture2D>("full heart"));
        hearts.Add(Content.Load<Texture2D>("empty heart"));

        enemies.Add(Content.Load<Texture2D>("slime1up"));
        enemies.Add(Content.Load<Texture2D>("slime1down"));
        enemies.Add(Content.Load<Texture2D>("blue slime up"));
        enemies.Add(Content.Load<Texture2D>("blue slime down"));
        enemies.Add(Content.Load<Texture2D>("yellow slime up"));
        enemies.Add(Content.Load<Texture2D>("yellow slime down"));
        enemies.Add(Content.Load<Texture2D>("coin bag"));
        enemies.Add(Content.Load<Texture2D>("full heart"));

        doug.Add(Content.Load<Texture2D>("doug0up"));
        doug.Add(Content.Load<Texture2D>("doug0down"));
        doug.Add(Content.Load<Texture2D>("doug1up"));
        doug.Add(Content.Load<Texture2D>("doug1down"));
        doug.Add(Content.Load<Texture2D>("doug2up"));
        doug.Add(Content.Load<Texture2D>("doug2down"));
        doug.Add(Content.Load<Texture2D>("doug3up"));
        doug.Add(Content.Load<Texture2D>("doug3down"));
        doug.Add(Content.Load<Texture2D>("doug start"));

        weapons.Add(Content.Load<Texture2D>("sword0"));
        weapons.Add(Content.Load<Texture2D>("sword1"));
        weapons.Add(Content.Load<Texture2D>("sword2"));
        weapons.Add(Content.Load<Texture2D>("sword3"));
        weapons.Add(Content.Load<Texture2D>("small spell"));
        weapons.Add(Content.Load<Texture2D>("bigspell"));

        soundEffects.Add(Content.Load<SoundEffect>("coinbag sound"));
        soundEffects.Add(Content.Load<SoundEffect>("getting coin sound"));
        soundEffects.Add(Content.Load<SoundEffect>("picking up card"));
        soundEffects.Add(Content.Load<SoundEffect>("getting heart"));
        soundEffects.Add(Content.Load<SoundEffect>("sword slash sound"));
        soundEffects.Add(Content.Load<SoundEffect>("spell 1 sound"));
        soundEffects.Add(Content.Load<SoundEffect>("spell 2 sound"));
        soundEffects.Add(Content.Load<SoundEffect>("hurt sound"));
        soundEffects.Add(Content.Load<SoundEffect>("slimehit"));
        soundEffects.Add(Content.Load<SoundEffect>("visor"));
        soundEffects.Add(Content.Load<SoundEffect>("victory"));

        music.Add(Content.Load<SoundEffect>("1").CreateInstance()); // 0
        music.Add(Content.Load<SoundEffect>("1.5").CreateInstance());
        music.Add(Content.Load<SoundEffect>("2").CreateInstance());
        music.Add(Content.Load<SoundEffect>("2.5").CreateInstance());
        music.Add(Content.Load<SoundEffect>("3").CreateInstance());
        music.Add(Content.Load<SoundEffect>("3.5").CreateInstance());
        music.Add(Content.Load<SoundEffect>("4").CreateInstance());
        music.Add(Content.Load<SoundEffect>("4.5").CreateInstance());
        music.Add(Content.Load<SoundEffect>("5").CreateInstance());
        music.Add(Content.Load<SoundEffect>("5.5").CreateInstance());
        music.Add(Content.Load<SoundEffect>("6").CreateInstance());
        music.Add(Content.Load<SoundEffect>("6.5").CreateInstance());
        music.Add(Content.Load<SoundEffect>("7").CreateInstance());
        music.Add(Content.Load<SoundEffect>("7.5").CreateInstance());
        music.Add(Content.Load<SoundEffect>("8").CreateInstance()); // 14


        coin = Content.Load<Texture2D>("coin");

        fires.Add(new Fire(pixel, 45, 45));
        fires.Add(new Fire(pixel, 755, 45));
        fires.Add(new Fire(pixel, 400, 45));

        titleScreen = Content.Load<Texture2D>("title screen");
        bg = Content.Load<Texture2D>("bg");
        helmet = Content.Load<Texture2D>("helmet");
        bigdoug = Content.Load<Texture2D>("bigdoug");
        flower = Content.Load<Texture2D>("flower");
        dead = Content.Load<Texture2D>("dead");

        menuMusic = Content.Load<SoundEffect>("menuMusic").CreateInstance();
        menuMusic.IsLooped = true;

        map = new Map(walls, doors, enemies, acards, bcards, pixel, 8, 8);
        map.getRoom(0, 0).unlockAllDoors();

        player = new Player(soundEffects, pixel, doug, weapons, acards, bcards, hearts, coin);
    }

    protected override void Update(GameTime gameTime)
    {
        oldkb = kb;
        kb = Keyboard.GetState();
        if (gameState == -2) {
            menuMusic.Play();
            if (kb.IsKeyDown(Keys.M)) {
                gameState = 0;
                menuMusic.Stop();
            }
        }
        if (gameState == -1) {
            menuMusic.Play();
            if (kb.IsKeyDown(Keys.M)) {
                gameState = 0;
                menuMusic.Stop();
            }
            if (kb.IsKeyDown(Keys.I)) {
                gameState = -2;
            }
        }
        if (gameState == 0) {

            if (gameTimer % 480 == 240) {
                if (currentSong != queuedSong) {
                    if (currentSong != -1)
                        music[currentSong].Stop();
                    currentSong = queuedSong;
                    music[currentSong].Volume = 0.5f;
                    music[currentSong].IsLooped = true;
                    music[currentSong].Play();
                } else {
                    currentSong = queuedSong;

                }
                
                

                if (queuedSong % 2 == 1)
                    queuedSong += 1;
                
            }

            if (map.getPlayerRoom().getSum(map) >= 2 && currentSong == 0 && queuedSong != 1)
                queuedSong = 1;
            if (map.getPlayerRoom().getSum(map) >= 4 && currentSong == 2 && queuedSong != 3)
                queuedSong = 3;
            if (map.getPlayerRoom().getSum(map) >= 6 && currentSong == 4 && queuedSong != 5)
                queuedSong = 5;
            if (map.getPlayerRoom().getSum(map) >= 8 && currentSong == 6 && queuedSong != 7)
                queuedSong = 7;
            if (map.getPlayerRoom().getSum(map) >= 10 && currentSong == 8 && queuedSong != 9)
                queuedSong = 9;
            if (map.getPlayerRoom().getSum(map) >= 12 && currentSong == 10 && queuedSong != 11)
                queuedSong = 11;
            if (map.getPlayerRoom().getSum(map) >= 14 && currentSong == 12 && queuedSong != 13)
                queuedSong = 13;





            map.getPlayerRoom().Update(player, map);

            map.getRoom(0, 0).unlockAllDoors();

            player.Update(kb, map);

            if (player.getHealth() <= 0) {
                music[currentSong].Stop();
                gameState = 2;
            }
            if (map.getPlayerRoom().getRowCol(map)[0] == 7 && map.getPlayerRoom().getRowCol(map)[1] == 7) {
                music[currentSong].Stop();
                gameState = 1;
            }
            if (kb.IsKeyDown(Keys.R)) {
                if (currentSong != -1)
                    music[currentSong].Stop();
                Initialize();
                map = new Map(walls, doors, enemies, acards, bcards, pixel, 8, 8);
                map.getRoom(0, 0).unlockAllDoors();
                player = new Player(soundEffects, pixel, doug, weapons, acards, bcards, hearts, coin);
            }
            gameTimer++;
        } else if (gameState == 1 || gameState == 2) {

            if (gameState == 2) {
                menuMusic.Play();
            }
            if (gameState == 1 && victoryPlayed == 0) {
                soundEffects[10].Play();
                victoryPlayed = 1;
            }

            gameSecs = gameTimer / 60;
            gameMins = gameSecs / 60;
            gameSecs = gameSecs - (gameMins * 60);


            if (gameState == 1) {
                map.getPlayerRoom().Update(player, map);
                player.Update(kb, map);
            }

            if (kb.IsKeyDown(Keys.R)) {
                menuMusic.Stop();
                Initialize();
                map = new Map(walls, doors, enemies, acards, bcards, pixel, 8, 8);
                map.getRoom(0, 0).unlockAllDoors();
                player = new Player(soundEffects, pixel, doug, weapons, acards, bcards, hearts, coin);
            }
        
        }

        if (kb.IsKeyDown(Keys.Escape)) {
                this.Exit();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

       
        /*
        var scaleX = (float)GraphicsDevice.Viewport.Height / (float)800;
        var scaleY = (float)GraphicsDevice.Viewport.Height / (float)800;

        var _screenScale = new Vector3(scaleX, scaleY, 1.0f);     

        var scaleMatrix = Matrix.CreateScale(_screenScale);

        sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, scaleMatrix);
        */
        sb.Begin();

        GraphicsDevice.Clear(Color.Black);

     

        if (gameState == -2) {
            sb.Draw(bg, new Rectangle(0, 0, 800, 800), Color.White);
            sb.DrawString(font, "DUNGEON DOUG.", new Vector2(20, 20), Color.White);
            sb.DrawString(font, "DOUG is in a randomly generated DUNGEON.", new Vector2(20, 75), Color.White);
            sb.DrawString(font, "every time, it will be different. but also the same.", new Vector2(20, 100), Color.White);
            sb.DrawString(font, "DOUG must reach the final room. it is marked on DOUG map.", new Vector2(20, 125), Color.White);
            sb.DrawString(font, "to do this, DOUG must find a way through the DUNGEON.", new Vector2(20, 150), Color.White);
            sb.DrawString(font, "WASD moves DOUG. M makes DOUG attack.", new Vector2(20, 200), Color.White);
            sb.DrawString(font, "when DOUG gains new attacks, the action key will be shown.", new Vector2(20, 225), Color.White);
            sb.DrawString(font, "throughout the DUNGEON, DOUG will find cards and coins.", new Vector2(20, 275), Color.White);
            sb.DrawString(font, "cards will give DOUG a new attack or attribute.", new Vector2(20, 300), Color.White);
            sb.DrawString(font, "DOUG can use coins at shops to buy cards.", new Vector2(20, 325), Color.White);
            sb.DrawString(font, "sometimes, DOUG must make a choice. DOUG must choose wisely.", new Vector2(20, 350), Color.White);
            sb.DrawString(font, "there is a special shop in the DUNGEON. DOUG must find it.", new Vector2(20, 600), Color.White);
            sb.DrawString(font, "DOUG can purchase very special things there.", new Vector2(20, 625), Color.White);
            sb.DrawString(font, "good luck, DOUG.", new Vector2(20, 650), Color.White);
            sb.DrawString(font, "press m to play.", new Vector2(20, 700), Color.White);

            sb.DrawString(font, "some attack cards.", new Vector2(40, 520), Color.White);
            sb.DrawString(font, "some attribute cards.", new Vector2(280, 520), Color.White);
            sb.DrawString(font, "coins.", new Vector2(590, 520), Color.White);

            sb.Draw(acards[4], new Rectangle(140, 400, 80, 100), Color.White);
            sb.Draw(acards[3], new Rectangle(90, 400, 80, 100), Color.White);
            sb.Draw(acards[1], new Rectangle(40, 400, 80, 100), Color.White);

            sb.Draw(bcards[4], new Rectangle(400, 400, 80, 100), Color.White);
            sb.Draw(bcards[3], new Rectangle(350, 400, 80, 100), Color.White);
            sb.Draw(bcards[1], new Rectangle(300, 400, 80, 100), Color.White);

            sb.Draw(enemies[6], new Rectangle(600, 430, 40, 40), Color.White);

            sb.Draw(bigdoug, new Rectangle(400, 700, 60, 44), Color.White);
            sb.Draw(helmet, new Rectangle(468, 714, 44, 32), Color.White);
        }

        if (gameState == -1) {

            sb.Draw(titleScreen, new Rectangle(0, 0, 800, 800), Color.White);

            //sb.DrawString(font, "a finnegan blake game.", new Vector2(550, 10), Color.White);

            sb.DrawString(font, "press m to play.", new Vector2(300, 600), Color.White);
            sb.DrawString(font, "press i for manual.", new Vector2(300, 700), Color.White);

            sb.Draw(bigdoug, new Rectangle(300, 460, 60, 44), Color.White);
            sb.Draw(helmet, new Rectangle(480, 474, 44, 32), Color.White);
            sb.DrawString(font, "?", new Vector2(350, 440), Color.White);

            sb.DrawString(font, "version 1.2 - music, bug fixes", new Vector2(5, 780), Color.White);
        }

        if (gameState == 0) {
            map.getPlayerRoom().Draw(sb, font, map);

            player.Draw(sb, font, map);

            /*
            sb.DrawString(font, map.getPlayerRoom().isLocked(0).ToString(), new Vector2(100, 50), Color.White);
            sb.DrawString(font, map.getPlayerRoom().isLocked(1).ToString(), new Vector2(125, 75), Color.White);
            sb.DrawString(font, map.getPlayerRoom().isLocked(2).ToString(), new Vector2(100, 100), Color.White);
            sb.DrawString(font, map.getPlayerRoom().isLocked(3).ToString(), new Vector2(75, 75), Color.White);

            sb.DrawString(font, "type: " + map.getPlayerRoom().getTypeString(), new Vector2(500, 75), Color.White);
            
            sb.DrawString(font, "facing: " + player.getFacing(), new Vector2(200, 75), Color.White);
            sb.DrawString(font, "swordFacing: " + player.getSwordFacing(), new Vector2(200, 100), Color.White);
            sb.DrawString(font, "swordTimer: " + player.getSwordTimer(), new Vector2(200, 125), Color.White);
            */


            //sb.DrawString(font, "room diff: " + map.getPlayerRoom().getDiff(), new Vector2(200, 75), Color.White);
            /*
            for (int r = 0; r < 8; r++) {
                for (int c = 0; c < 8; c++) {
                    sb.DrawString(font, map.getRoom(r, c).getType(w).ToString(), new Vector2(300 + (55 * c), 250 + (55 * r)), Color.White);
                }
            }
            sb.DrawString(font, map.getRoom(player.getMapY(), player.getMapX()).getType().ToString(), new Vector2(300 + (55 * player.getMapX()), 250 + (55 * player.getMapY())), Color.Red);
            */
            //sb.DrawString(font, "enemy type: " + map.getPlayerRoom().getEnemies()[0].getType(), new Vector2(400, 75), Color.White);
            //sb.DrawString(font, "enemy health: " + map.getPlayerRoom().getEnemies()[0].getHealth(), new Vector2(400, 100), Color.White);
            //sb.DrawString(font, "room diff: " + map.getPlayerRoom().getDiff(), new Vector2(400, 125), Color.White);

            //sb.DrawString(font, "current song: " + currentSong, new Vector2(400, 100), Color.White);
            //sb.DrawString(font, "queued song: " + queuedSong, new Vector2(400, 125), Color.White);
            //sb.DrawString(font, "timer: " + gameTimer % 480, new Vector2(400, 175), Color.White);

        } else if (gameState == 1) {
            map.getPlayerRoom().Draw(sb, font, map);
            player.Draw(sb, font, map);

            sb.DrawString(font, "good job, DOUG.", new Vector2(250, 200), Color.White);
            sb.DrawString(font, "DOUG has cleared the DUNGEON.", new Vector2(250, 225), Color.White);
            if (gameMins != 1 && gameSecs != 1)
                sb.DrawString(font, "it took DOUG " + gameMins + " minutes and " + gameSecs + " seconds.", new Vector2(250, 250), Color.White);
            else if (gameMins == 1 && gameSecs != 1)
                sb.DrawString(font, "it took DOUG " + gameMins + " minute and " + gameSecs + " seconds.", new Vector2(250, 250), Color.White);
            else if (gameMins != 1 && gameSecs == 1)
                sb.DrawString(font, "it took DOUG " + gameMins + " minutes and " + gameSecs + " second.", new Vector2(250, 250), Color.White);
            else
                sb.DrawString(font, "it took DOUG " + gameMins + " minute and " + gameSecs + " second.", new Vector2(250, 250), Color.White);

            sb.DrawString(font, "press r to try again.", new Vector2(250, 300), Color.White);

            sb.Draw(flower, new Rectangle(380, 380, 40, 40), Color.White);

            sb.DrawString(font, "DOUG stats.", new Vector2(250, 500), Color.White);
            sb.DrawString(font, "feet traveled: " + player.getFeetTraveled(), new Vector2(250, 525), Color.White);
            sb.DrawString(font, "enemies slain: " + player.getEnemiesKilled(), new Vector2(250, 550), Color.White);
            sb.DrawString(font, "hits taken: " + player.getHitsTaken(), new Vector2(250, 575), Color.White);
            sb.DrawString(font, "coins spent: " + player.getCoinsSpent(), new Vector2(250, 600), Color.White);
        } else if (gameState == 2) {
            sb.Draw(dead, new Rectangle(0, 0, 800, 800), Color.White);
            sb.DrawString(font, "DOUG is dead.", new Vector2(300, 600), Color.White);
            sb.DrawString(font, "press r to restart.", new Vector2(300, 700), Color.White);
        }

        sb.End();

        base.Draw(gameTime);
    }
}
