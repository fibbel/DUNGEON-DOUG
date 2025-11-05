using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DUNGEON_DOUG_PORT {
    class Room {

        Random rg;
        private List<Texture2D> walls;
        private List<Texture2D> doors;
        private List<Texture2D> enemyTextures;
        private List<Texture2D> aCardTextures;
        private List<Texture2D> bCardTextures;
        private List<Rectangle> wallRects;
        private List<Rectangle> doorRects;
        private List<Fire> fires;
        private int[] sides;
        private bool[] locked;
        private bool cleared, entered;
        private int type; // -1 = inaccessible, 0 = start, 1 = enemies, 2 = treasure, 3 = shop, 4 = end
        private int diff; // 0 = easy, 1 = medium, 2 = kinda hard, 3 = hard | treasure: 0 = attribute room, 1 = attack room, 2 = coin/health room | shop: 0 = normal, 1 = rare
        private Texture2D pixel;

        private List<Enemy> enemies;

        public Room(List<Texture2D> wa, List<Texture2D> d, List<Texture2D> ene, List<Texture2D> acards, List<Texture2D> bcards, Texture2D pixel, int n, int e, int s, int w, int t, int di) {
            this.pixel = pixel;
            walls = new List<Texture2D>();
            doors = new List<Texture2D>();
            enemyTextures = new List<Texture2D>();
            aCardTextures = new List<Texture2D>();
            bCardTextures = new List<Texture2D>();
            wallRects = new List<Rectangle>();
            doorRects = new List<Rectangle>();

            fires = new List<Fire>();

            walls = wa;
            doors = d;
            aCardTextures = acards;
            bCardTextures = bcards;
            wallRects.Add(new Rectangle(0, 0, 800, 80));
            wallRects.Add(new Rectangle(720, 0, 80, 800));
            wallRects.Add(new Rectangle(0, 720, 800, 80));
            wallRects.Add(new Rectangle(0, 0, 80, 800));

            doorRects.Add(new Rectangle(336, 0, 124, 80));
            doorRects.Add(new Rectangle(720, 336, 80, 124));
            doorRects.Add(new Rectangle(340, 720, 124, 80));
            doorRects.Add(new Rectangle(0, 336, 80, 128));

            fires.Add(new Fire(pixel, 45, 45));
            fires.Add(new Fire(pixel, 755, 45));
            fires.Add(new Fire(pixel, 45, 755));
            fires.Add(new Fire(pixel, 755, 755));
            if (n == 0)
                fires.Add(new Fire(pixel, 400, 45));
            if (e == 0)
                fires.Add(new Fire(pixel, 755, 400));
            if (s == 0)
                fires.Add(new Fire(pixel, 400, 755));
            if (w == 0)
                fires.Add(new Fire(pixel, 45, 400));

            diff = di;

            enemyTextures = ene;

            sides = new int[4];
            locked = new bool[4];

            sides[0] = n;
            sides[1] = e;
            sides[2] = s;
            sides[3] = w;

            locked[0] = true;
            locked[1] = true;
            locked[2] = true;
            locked[3] = true;

            type = t;

            cleared = false;
            entered = false;

            enemies = new List<Enemy>();
            rg = new Random(Guid.NewGuid().GetHashCode());
        }

        public void initializeEnemies(Player p) {
            int numEnemies = rg.Next(2, 5);
            int numType;
            int card = rg.Next(1, 5);
            int card2 = rg.Next(1, 6);
            int card3 = rg.Next(1, 6);
            if (type == 0) { 
                enemies.Add(new Enemy(pixel, aCardTextures, 360, 400, -1));
                enemies.Add(new Enemy(pixel, bCardTextures, 460, 400, (-1 * card2) - 5));
            }

            if (type == 2) {
                if (diff == 0) {
                    card = rg.Next(1, 6);
                    while (card == card2) {
                        card = rg.Next(1, 6);
                    }
                    enemies.Add(new Enemy(pixel, bCardTextures, 360, 400, (-1 * card) - 5));
                    enemies.Add(new Enemy(pixel, bCardTextures, 460, 400, (-1 * card2) - 5));
                }
                if (diff == 1) {
                    card2 = rg.Next(1, 5);
                    while (p.getACardsPlayerHas().Contains(card - 1)) {
                        card = rg.Next(1, 5);
                    }
                    if (p.getACardsPlayerHas().Count() < 4) {
                        while (p.getACardsPlayerHas().Contains(card2 - 1) || card == card2) {
                            card2 = rg.Next(1, 5);
                        }
                    } else {
                        while (p.getACardsPlayerHas().Contains(card2 - 1) ) {
                            card2 = rg.Next(1, 5);
                        }
                    }
                    enemies.Add(new Enemy(pixel, aCardTextures, 360, 400, (-1 * card)));
                    enemies.Add(new Enemy(pixel, aCardTextures, 460, 400, (-1 * card2)));
                }
                if (diff == 2) {
                    enemies.Add(new Enemy(pixel, enemyTextures, 360, 400, -12));
                    enemies.Add(new Enemy(pixel, enemyTextures, 460, 400, -13));
                }
            }

            if (type == 3) {
                if (diff == 0) {
                    card = rg.Next(1, 6);
                    enemies.Add(new Enemy(pixel, bCardTextures, 300, 400, (-1 * card) - 5));
                    enemies.Add(new Enemy(pixel, bCardTextures, 400, 400, (-1 * card2) - 5));
                    enemies.Add(new Enemy(pixel, bCardTextures, 500, 400, (-1 * card3) - 5));
                }
                if (diff == 1) {
                    while (p.getACardsPlayerHas().Contains(card - 1)) {
                        card = rg.Next(1, 5);
                    }
                    enemies.Add(new Enemy(pixel, aCardTextures, 300, 400, -1 * card));
                    enemies.Add(new Enemy(pixel, aCardTextures, 400, 400, -5));
                    enemies.Add(new Enemy(pixel, bCardTextures, 500, 400, -11));
                }
                unlockAllDoors();
            }

            if (type == 1) {
                if (diff == 0) {
                    for (int i = 0; i < numEnemies; i++) {
                        enemies.Add(new Enemy(pixel, enemyTextures, rg.Next(100, 600), rg.Next(100, 600), 0));
                    }
                }
                if (diff == 1) {
                    numEnemies = rg.Next(2, 6);
                    numType = rg.Next(0, 2);
                    for (int i = 0; i < numEnemies - numType; i++) {
                        enemies.Add(new Enemy(pixel, enemyTextures, rg.Next(100, 600), rg.Next(100, 600), 1));
                    }
                    for (int i = 0; i < numType; i++) {
                        enemies.Add(new Enemy(pixel, enemyTextures, rg.Next(100, 600), rg.Next(100, 600), 0));
                    }
                }
                if (diff == 2) {
                    numEnemies = rg.Next(2, 7);
                    numType = rg.Next(1, numEnemies);
                    for (int i = 0; i < numEnemies - numType; i++) {
                        enemies.Add(new Enemy(pixel, enemyTextures, rg.Next(100, 600), rg.Next(100, 600), 2));
                    }
                    for (int i = 0; i < numType; i++) {
                        enemies.Add(new Enemy(pixel, enemyTextures, rg.Next(100, 600), rg.Next(100, 600), 1));
                    }
                }
                if (diff == 3) {
                    numEnemies = rg.Next(4, 7);
                    for (int i = 0; i < numEnemies; i++) {
                        enemies.Add(new Enemy(pixel, enemyTextures, rg.Next(100, 600), rg.Next(100, 600), 2));
                    }
                }
            }
        }
        
        public void updateEnemies(Player p, Map m) {
            if (enemies.Count() > 0) {
                int enemyType;
                for (int i = 0; i < enemies.Count(); i++) {
                    enemyType = enemies[i].getType();
                    enemies[i].Update(p, m);
                    if (enemies[i].getHealth() <= 0) {
                        enemies.Remove(enemies[i]);
                        if (type == 2) {
                            for (int j = 0; j < enemies.Count(); j++) {
                                enemies.Remove(enemies[j]);
                                j--;
                            }
                        }
                        i--;
                        if (p.getLuck() < 2 && rg.Next(1, 5 - (p.getLuck() * 2)) == 1 && enemyType >= 0)
                            p.addCoins();
                        else if (p.getLuck() >= 2 && enemyType >= 0)
                            p.addCoins();
                        if (enemyType >= 0)
                            p.addEnemyKilled();
                    }

                }
            }
        }

        public int getSideStatus(int side) {
            return sides[side];
        }
        public int getNumDoors() { return sides[0] + sides[1] + sides[2] + sides[3]; }
        public int getNumEnemies() { return enemies.Count(); }
        public List<Enemy> getEnemies() { return enemies; }

        public int getType() { return type; }
        public string getTypeString() {
            if (type == -1)
                return "inaccessible";
            else if (type == 0)
                return "start";
            else if (type == 1)
                return "enemies";
            else if (type == 2)
                return "treasure";
            else if (type == 3)
                return "shop";
            else
                return "end";
        }
        public void updateType(int t) { type = t; }
        public void updateDiff(int d) { diff = d; }
        public int getDiff() { return diff; }
        public bool getEntered() { return entered; }
        public void updatedEntered() { entered = true; }
        public bool getCleared() { return cleared; }

        public int[] getRowCol(Map m) {
            int[] result = new int[2] { -1, -1 };

            for (int r = 0; r < 8; r++) {
                for (int c = 0; c < 8; c++) {
                    if (m.getRoom(r, c).Equals(this)) {
                        result[0] = r;
                        result[1] = c;
                        break;
                    }
                }
            }

            return result;
        }
        public int getSum(Map m) {
            int result = -1;

            for (int r = 0; r < 8; r++) {
                for (int c = 0; c < 8; c++) {
                    if (m.getRoom(r, c).Equals(this)) {
                        result = r + c;
                        break;
                    }
                }
            }

            return result;
        }

        public List<Rectangle> getWallRects() { return wallRects; }
        public List<Rectangle> getDoorRects() { return doorRects; }

        public void unlockAllDoors() {
            for (int i = 0; i < 4; i++) {
                if (sides[i] == 1)
                    locked[i] = false;
            }
        }
        public void unlockDoor(int s) { locked[s] = false; }
        public bool isLocked(int s) { return locked[s]; }

        public void Update(Player p, Map m) {
            foreach (Fire temp in fires) {
                temp.Update();
            }
            updateEnemies(p, m);
            if (enemies.Count() == 0) {
                cleared = true;
                unlockAllDoors();
            }
            if (type == 3 && diff == 1) {
                foreach (Enemy e in this.getEnemies()) {
                    if (e.getType() < 0 && e.getType() > -6 && p.getACardsPlayerHas().Contains((e.getType() * -1) - 1)) {
                        int newCard = rg.Next(0, 4);
                        while (p.getACardsPlayerHas().Contains(newCard)) {
                            newCard = rg.Next(0, 4);
                        }
                        e.updateType((newCard * -1) - 1);
                    }
                }
            }

        }

        public void Draw(SpriteBatch sb, SpriteFont font, Map m) {
            for (int i = 0; i < 4; i++) {
                if (getSideStatus(i) == 0) {
                    sb.Draw(walls[i], wallRects[i], Color.White);
                } else {
                    sb.Draw(walls[i + 4], wallRects[i], Color.White);
                    if (locked[i])
                        sb.Draw(doors[i + 4], doorRects[i], Color.White);
                    if (m.getPlayerRoom().getRowCol(m)[0] == 6 && m.getPlayerRoom().getRowCol(m)[1] == 7 && i == 2) {
                        sb.Draw(doors[i], doorRects[i], Color.White);
                    }
                    if (m.getPlayerRoom().getRowCol(m)[0] == 7 && m.getPlayerRoom().getRowCol(m)[1] == 6 && i == 1) {
                        sb.Draw(doors[i], doorRects[i], Color.White);
                    }
                }

            }
            foreach (Fire temp in fires) {
                temp.Draw(sb);
            }

            for (int r = 0; r < 8; r++) {
                for (int c = 0; c < 8; c++) {
                    if (m.getRoom(r, c).getEntered()) {
                        if (m.getRoom(r, c).getType() == 1)
                            sb.Draw(pixel, new Rectangle(100 + (12 * c), 100 + (12 * r), 8, 8), Color.White);
                        if (m.getRoom(r, c).getType() == 2)
                            sb.Draw(pixel, new Rectangle(100 + (12 * c), 100 + (12 * r), 8, 8), Color.CornflowerBlue);
                        if (m.getRoom(r, c).getType() == 3)
                            sb.Draw(pixel, new Rectangle(100 + (12 * c), 100 + (12 * r), 8, 8), Color.Purple);

                        if (m.getRoom(r, c).getSideStatus(0) == 1)
                            sb.Draw(pixel, new Rectangle(100 + (12 * c) + 2, 100 + (12 * r) - 4, 4, 4), Color.White);
                        if (m.getRoom(r, c).getSideStatus(1) == 1)
                            sb.Draw(pixel, new Rectangle(100 + (12 * c) + 8, 100 + (12 * r) + 2, 4, 4), Color.White);
                        if (m.getRoom(r, c).getSideStatus(2) == 1)
                            sb.Draw(pixel, new Rectangle(100 + (12 * c) + 2, 100 + (12 * r) + 8, 4, 4), Color.White);
                        if (m.getRoom(r, c).getSideStatus(3) == 1)
                            sb.Draw(pixel, new Rectangle(100 + (12 * c) - 4, 100 + (12 * r) + 2, 4, 4), Color.White);
                    }
                }
            }
            sb.Draw(pixel, new Rectangle(100, 100, 8, 8), Color.Green);
            sb.Draw(pixel, new Rectangle(100 + (12 * 7), 100 + (12 * 7), 8, 8), Color.Green);
            sb.Draw(pixel, new Rectangle(100 + (12 * m.getPlayerRoom().getRowCol(m)[1]), 100 + (12 * m.getPlayerRoom().getRowCol(m)[0]), 8, 8), Color.Red);

            if (type == 0) {
                
                sb.DrawString(font, "good luck, DOUG.", new Vector2(320, 300), Color.White);
            }

            if (type == 2) {
                if (diff == 0)
                    sb.DrawString(font, "choose a new attribute.", new Vector2(280, 300), Color.White);
                if (diff == 1)
                    sb.DrawString(font, "choose a new attack.", new Vector2(298, 300), Color.White);
                if (diff == 2)
                    sb.DrawString(font, "choose wisely.", new Vector2(320, 300), Color.White);
            }

            if (type == 3) {
                if (diff == 0)
                    sb.DrawString(font, "welcome to the shop.", new Vector2(290, 300), Color.White);
                if (diff == 1)
                    sb.DrawString(font, "welcome to the special shop.", new Vector2(270, 300), Color.White);
            }

            foreach (Enemy e in enemies) {
                e.Draw(sb, font, m);
            }
        }

    }
}
