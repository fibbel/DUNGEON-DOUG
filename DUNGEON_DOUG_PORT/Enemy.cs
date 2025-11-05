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
    class Enemy {

        private Rectangle rect;
        private Texture2D text;
        private List<Texture2D> textures;
        private Rectangle hurtbox, hitbox;
        private int health, type, state, timer, itimer, price;
        private Random rg;
        private double speed1, speed2, speed3, xPos, yPos;
        private bool invincible, wasHitBySpell;

        private int taskX, taskY, taskTimer;

        private Color color;

        /* TYPES
         * 0 = green slime 
         * 1 = blue slime
         * 2 = yellow slime
         * 3 = ???
         * -1 = card1
         * -2 = card2
         * ''''
         * -6 = longsword
         * -7 = luck
         * -8 = attack
         * -9 = speed
         * -10 = heartcontainer
         * -11 = boss key
         * -12 = coin bag?
         * -13 = heart?
         */



        public Enemy(Texture2D t, List<Texture2D> texts, int x, int y, int type) {

            rg = new Random(Guid.NewGuid().GetHashCode());

            textures = texts;

            text = t;

            this.type = type;
            if (type == -12) {
                rect = new Rectangle(x - 40, y - 50, 40, 40);
                state = 0;
                speed1 = 0;
                speed2 = 0;
                speed3 = 0;
                health = 1;
            } else if (type == -13) {
                rect = new Rectangle(x - 40, y - 42, 28, 24);
                state = 0;
                speed1 = 0;
                speed2 = 0;
                speed3 = 0;
                health = 1;
            } else if (type < 0) {
                rect = new Rectangle(x - 40, y - 50, 80, 100);
                state = 0;
                speed1 = 0;
                speed2 = 0;
                speed3 = 0;
                health = 1;
            }
            if (type >= 0 && type <= 2) {
                rect = new Rectangle(x, y, 40, 32);
            }
            if (type == 0) {
                state = 0;
                speed1 = 0.3;
                speed2 = 0.8;
                speed3 = 5;
                health = 100;
            }
            if (type == 1) {
                state = 0;
                speed1 = 0.6;
                speed2 = 1.2;
                speed3 = 5;
                health = 135;
            }
            if (type == 2) {
                state = 1;
                speed1 = 1;
                speed2 = 2;
                speed3 = 5;
                health = 170;
            }

            if (type == -6 || type == -9) // long sword, speed
                price = 3;
            if (type == -10) // heart
                price = 5;
            if (type == -2 || type == -3 || type == -4 || type == -8) // base attacks, damage
                price = 7;
            if (type == -5 || type == -7) // big spell, luck
                price = 10;
            if (type == -11) // boss key
                price = 12;

            xPos = x;
            yPos = y;

            hurtbox = rect;
            hitbox = rect;

            taskTimer = 0;
            timer = 0;
            taskX = rg.Next(100, 600);
            taskY = rg.Next(100, 600);
            invincible = false;
            itimer = 0;

            wasHitBySpell = false;

            color = Color.White;
        }

        public bool isInvincible() { return invincible; }
        public int getPrice() { return price; }
        public int getTaskTimer() { return taskTimer; }
        public int getType() { return type; }
        public void updateType(int t) { type = t; }
        public int getState() { return state; }
        public void updateState(int s) { state = s; }
        public int getHealth() { return health; }
        public void kill() { health = -1; }
        public int getTaskY() { return taskY; }
        public int getTaskX() { return taskX; }
        public Rectangle getHurtbox() { return hurtbox; }

        public void Update(Player p, Map m) {
            if (type >= 0) {
                //behavior
                if (state == 0) {

                    if (taskTimer == 200) {
                        taskTimer = 0;
                    }

                    if (taskTimer == 0) {
                        taskTimer = rg.Next(0, 100);
                        if (rg.Next(1, 5) == 1) {
                            taskX = rg.Next(100, 600);
                            taskY = rg.Next(100, 600);
                        }
                    }

                    if (taskTimer < 200) {
                        if (rect.X < taskX) {
                            xPos += speed1;
                        }
                        if (rect.X > taskX) {
                            xPos -= speed1;
                        }
                        if (rect.Y < taskY) {
                            yPos += speed1;
                        }
                        if (rect.Y > taskY) {
                            yPos -= speed1;
                        }
                    }
                }  
                if (state == 1) {   
                    taskX = p.getRect().X;
                    taskY = p.getRect().Y;

                    if (rect.X < taskX) {
                        xPos += speed2;
                    }
                    if (rect.X > taskX) {
                        xPos -= speed2;
                    }
                    if (rect.Y < taskY) {
                        yPos += speed2;
                    }
                    if (rect.Y > taskY) {
                        yPos -= speed2;
                    }

                }

                //hit detection
                if ((p.getHurtbox().Intersects(hitbox) || p.getSpell1Hurtbox().Intersects(hitbox) || p.getSpell2Hurtbox().Intersects(hitbox)) && !invincible) {
                    if (type == 0 && (state == 0 || state == 1))
                        state = 2;
                    if ((type == 1 || type == 2) && (state == 0 || state == 1)) {
                        foreach (Enemy e in m.getPlayerRoom().getEnemies()) {
                            if ((e.getType() == 1 || e.getType() == 2) && e.getState() != 2) {
                                e.updateState(1);
                            }
                        }
                        state = 2;
                    }
                    p.playSlimeHit();
                    health -= p.getDamage();
                    invincible = true;
                    wasHitBySpell = false;
                    if (p.getSpell1Hurtbox().Intersects(hitbox) || p.getSpell2Hurtbox().Intersects(hitbox)) {
                        wasHitBySpell = true;
                        health -= p.getDamage();
                    }
                }

                if (invincible) {
                    itimer++;
                    color = Color.Red;
                }

                if (itimer == 45) {
                    itimer = 0;
                    invincible = false;
                    color = Color.White;
                }

                if (state == 2) {
                    if (wasHitBySpell)
                        state = 1;
                    else {
                        if (itimer == 1) {

                            if (p.getHurtboxNum() == 1 || p.getHurtboxNum() == 10) {
                                taskX = p.getRect().X - 200;
                                taskY = p.getRect().Y - 200;
                            }
                            if (p.getHurtboxNum() == 2) {
                                taskX = p.getRect().X;
                                taskY = p.getRect().Y - 200;
                            }
                            if (p.getHurtboxNum() == 3 || p.getHurtboxNum() == 4) {
                                taskX = p.getRect().X + 200;
                                taskY = p.getRect().Y - 200;
                            }
                            if (p.getHurtboxNum() == 5) {
                                taskX = p.getRect().X + 200;
                                taskY = p.getRect().Y;
                            }
                            if (p.getHurtboxNum() == 6 || p.getHurtboxNum() == 9) {
                                taskX = p.getRect().X + 200;
                                taskY = p.getRect().Y + 200;
                            }
                            if (p.getHurtboxNum() == 8) {
                                taskX = p.getRect().X;
                                taskY = p.getRect().Y + 200;
                            }
                            if (p.getHurtboxNum() == 7 || p.getHurtboxNum() == 12) {
                                taskX = p.getRect().X - 200;
                                taskY = p.getRect().Y + 200;
                            }
                            if (p.getHurtboxNum() == 11) {
                                taskX = p.getRect().X - 200;
                                taskY = p.getRect().Y;
                            }


                        }
                        if (itimer < 20) {
                            if (rect.X < taskX) {
                                xPos += speed3;
                            }
                            if (rect.X > taskX) {
                                xPos -= speed3;
                            }
                            if (rect.Y < taskY) {
                                yPos += speed3;
                            }
                            if (rect.Y > taskY) {
                                yPos -= speed3;
                            }
                        }
                        if (itimer == 20) {
                            taskX = (int)xPos;
                            taskY = (int)yPos;
                        }
                        if (itimer == 44)
                            state = 1;
                    }
                }



                rect.X = (int)xPos;
                rect.Y = (int)yPos;

                if (rect.X > 720)
                    rect.X = 720;
                if (rect.X < 80)
                    rect.X = 80;
                if (rect.Y > 700)
                    rect.Y = 700;
                if (rect.Y < 50)
                    rect.Y = 50;
            }


            taskTimer++;
            timer++;

            hurtbox = rect;
            hitbox = rect;
        }

        public void Draw(SpriteBatch sb, SpriteFont font, Map m) {
            if (type < 0 && type >= -5) { 

                sb.Draw(textures[(-1 * type) - 1], hurtbox, color);
            }
            if (type < -5) {
                sb.Draw(textures[(-1 * type) - 6], hurtbox, color);
            }
            if (type < 0 && m.getPlayerRoom().getType() == 3) {
                sb.DrawString(font, price.ToString(), new Vector2(hurtbox.X + 40, hurtbox.Y + 110), Color.White);
            }
            if (type >= 0 && type <= 2) {
                if (timer % 120 > 60)
                    sb.Draw(textures[type * 2], hurtbox, color);
                else
                    sb.Draw(textures[type * 2 + 1], hurtbox, color);
            }
        }

    }
}
