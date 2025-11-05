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
    class Player {

        private Rectangle rect;
        private List<Texture2D> textures, weaponTextures;
        private List<SoundEffect> soundEffects;
        private Texture2D heart, emptyHeart, coin, text, currentTexture, currentSwordTexture;
        private int mapX, mapY, facing, speed, health, maxHealth, coins, swordBonus, speedBonus, timer, hitsTaken, enemiesKilled, coinsSpent, feetTraveled, startingMaxHealth;
        private Rectangle hurtbox, spell1Hurtbox, spell2Hurtbox;
        private Rectangle hitbox;
        private int swordTimer, swordFacing, hurtboxNum, itimer, dooritimer, projectilePosition, projectileX, projectileY, projectile2X, projectile2Y, spell1Facing, spell1Timer, spell2Facing, spell2Timer, damage, luck;
        private bool invincible, doorInvincible;

        private List<Texture2D> aCards, bCards;
        private List<int> aCardsPlayerHas, bCardsPlayerHas;
        private Keys[] attackKeys;
        private string[] keyStrings;
        private bool[] hasACards;
        private int[] hasBCards;
        private List<bool> inAction;
        private List<int> cooldownTimers;
        private bool inAttack, castingSpell1, castingSpell2;
        KeyboardState oldkb;

        public Player(List<SoundEffect> sonds, Texture2D text, List<Texture2D> t, List<Texture2D> w, List<Texture2D> acards, List<Texture2D> bcards, List<Texture2D> hearts, Texture2D c) {
            rect = new Rectangle(150, 150, 52, 52);
            this.text = text;
            textures = t;
            mapX = 0;
            mapY = 0;
            facing = 2;
            swordFacing = 2;
            currentTexture = textures[8];
            weaponTextures = w;
            currentSwordTexture = weaponTextures[0];

            speed = 3;

            soundEffects = sonds;

            health = 3;
            startingMaxHealth = 3;
            maxHealth = startingMaxHealth;

            swordTimer = 0;
            spell1Timer = 0;
            spell2Timer = 0;

            damage = 35;
            speedBonus = 0;

            hitbox = rect;
            hurtbox = new Rectangle();
            spell1Hurtbox = new Rectangle();
            spell2Hurtbox = new Rectangle();
            hurtboxNum = 0;
            oldkb = new KeyboardState();

            this.aCards = acards;
            this.bCards = bcards;
            hasACards = new bool[5] { false, false, false, false, false};
            hasBCards = new int[6] { 0, 0, 0, 0, 0, 0};

            inAction = new List<bool>();

            aCardsPlayerHas = new List<int>();
            bCardsPlayerHas = new List<int>();
            cooldownTimers = new List<int>();



            swordBonus = 0;
            inAttack = false;
            castingSpell1 = false;
            castingSpell2 = false;

            attackKeys = new Keys[5] { Keys.M, Keys.N, Keys.B, Keys.V, Keys.C };
            keyStrings = new string[5] { "m", "n", "b", "v", "c" };

            coins = 0;

            heart = hearts[0];
            emptyHeart = hearts[1];
            coin = c;

            invincible = false;
            doorInvincible = false;
            itimer = 0;
            dooritimer = 0;

            projectilePosition = 0;
            projectileX = 0;
            projectileY = 0;
            projectile2X = 0;
            projectile2Y = 0;

            timer = 0;

            hitsTaken = 0;
            enemiesKilled = 0;
            coinsSpent = 0;
            feetTraveled = 0;

        }

        public List<int> getACardsPlayerHas() { return aCardsPlayerHas; }

        public int getMapX() { return mapX; }
        public int getMapY() { return mapY; }
        public int getFacing() { return facing; }
        public int getSwordFacing() { return swordFacing; }
        public int getSwordTimer() { return swordTimer; }
        public Rectangle getRect() { return rect; }
        public Rectangle getHurtbox() { return hurtbox; }
        public Rectangle getSpell1Hurtbox() { return spell1Hurtbox; }
        public Rectangle getSpell2Hurtbox() { return spell2Hurtbox; }
        public Rectangle getHitbox() { return hitbox; }
        public int getHurtboxNum() { return hurtboxNum;  }
        public void playSlimeHit() { soundEffects[8].Play(); }

        public int getHitsTaken() { return hitsTaken; }
        public int getEnemiesKilled() { return enemiesKilled; }
        public void addEnemyKilled() { enemiesKilled++; }
        public int getCoinsSpent() { return coinsSpent; }
        public int getFeetTraveled() { return feetTraveled / 20; }

        public int getLuck() { return luck; }
        public int getDamage() { return damage; }
        public int getHealth() { return health; }
        public void damagePlayer() { health--; hitsTaken++; }
        public void addCoins() { coins++; soundEffects[1].Play(); }

        public void Update(KeyboardState kb, Map map) {

            //bCards
            swordBonus = hasBCards[0] * 20;
            damage = 35 + (hasBCards[2] * 15);
            luck = hasBCards[1];
            speedBonus = hasBCards[3];
            maxHealth = startingMaxHealth + hasBCards[4];

            // movements
            if (timer > 240 && map.getPlayerRoom().getType() != 4) {
                if ((kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up)) && !rect.Intersects(new Rectangle(map.getPlayerRoom().getWallRects()[0].X, map.getPlayerRoom().getWallRects()[0].Y - 40, map.getPlayerRoom().getWallRects()[0].Width, map.getPlayerRoom().getWallRects()[0].Height))) {
                    rect.Y -= speed + speedBonus;
                    feetTraveled += speed + speedBonus;
                    facing = 0;
                }
                if ((kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)) && !rect.Intersects(map.getPlayerRoom().getWallRects()[2])) {
                    rect.Y += speed + speedBonus;
                    feetTraveled += speed + speedBonus;
                    facing = 2;
                }
                if ((kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)) && !rect.Intersects(map.getPlayerRoom().getWallRects()[3])) {
                    rect.X -= speed + speedBonus;
                    feetTraveled += speed + speedBonus;
                    facing = 3;
                }
                if ((kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) && !rect.Intersects(map.getPlayerRoom().getWallRects()[1])) {
                    rect.X += speed + speedBonus;
                    feetTraveled += speed + speedBonus;
                    facing = 1;
                }
            }

            hitbox = rect;

            // door
            
            if (rect.Intersects(map.getPlayerRoom().getDoorRects()[0]) && map.getPlayerRoom().getSideStatus(0) == 1 && map.getPlayerRoom().isLocked(0) == false) {
                mapY -= 1;
                map.updatePlayerRoom(map.getMap()[mapY, mapX]);
                rect.X = 375;
                rect.Y = 575;
                doorInvincible = true;
                dooritimer = 0;

                for (int i = 0; i < inAction.Count(); i++) {
                    inAction[i] = false;
                }
                hurtbox = new Rectangle();
                spell1Hurtbox = new Rectangle();
                spell2Hurtbox = new Rectangle();
                inAttack = false;
                swordTimer = 0;
                castingSpell1 = false;
                spell1Timer = 0;
                projectilePosition = 0;
                castingSpell2 = false;
                spell2Timer = 0;
            }
            if (rect.Intersects(map.getPlayerRoom().getDoorRects()[1]) && map.getPlayerRoom().getSideStatus(1) == 1 && map.getPlayerRoom().isLocked(1) == false) {
                if (!(map.getPlayerRoom().getRowCol(map)[0] == 7 && map.getPlayerRoom().getRowCol(map)[1] == 6 && hasBCards[5] == 0)) {
                    mapX += 1;
                    map.updatePlayerRoom(map.getMap()[mapY, mapX]);
                    rect.X = 125;
                    rect.Y = 375;
                    doorInvincible = true;
                    dooritimer = 0;

                  
                }

                for (int i = 0; i < inAction.Count(); i++) {
                    inAction[i] = false;
                }
                hurtbox = new Rectangle();
                spell1Hurtbox = new Rectangle();
                spell2Hurtbox = new Rectangle();
                inAttack = false;
                swordTimer = 0;
                castingSpell1 = false;
                spell1Timer = 0;
                projectilePosition = 0;
                castingSpell2 = false;
                spell2Timer = 0;

            }
            if (rect.Intersects(map.getPlayerRoom().getDoorRects()[2]) && map.getPlayerRoom().getSideStatus(2) == 1 && map.getPlayerRoom().isLocked(2) == false) {

                if (!(map.getPlayerRoom().getRowCol(map)[0] == 6 && map.getPlayerRoom().getRowCol(map)[1] == 7 && hasBCards[5] == 0)) {

                    mapY += 1;
                    map.updatePlayerRoom(map.getMap()[mapY, mapX]);
                    rect.X = 375;
                    rect.Y = 125;
                    doorInvincible = true;
                    dooritimer = 0;

                    
                }

                for (int i = 0; i < inAction.Count(); i++) {
                    inAction[i] = false;
                }
                hurtbox = new Rectangle();
                spell1Hurtbox = new Rectangle();
                spell2Hurtbox = new Rectangle();
                inAttack = false;
                swordTimer = 0;
                castingSpell1 = false;
                spell1Timer = 0;
                projectilePosition = 0;
                castingSpell2 = false;
                spell2Timer = 0;

            }
            if (rect.Intersects(map.getPlayerRoom().getDoorRects()[3]) && map.getPlayerRoom().getSideStatus(3) == 1 && map.getPlayerRoom().isLocked(3) == false) {
                mapX -= 1;
                map.updatePlayerRoom(map.getMap()[mapY, mapX]);
                rect.X = 625;
                rect.Y = 375;
                doorInvincible = true;
                dooritimer = 0;

                for (int i = 0; i < inAction.Count(); i++) {
                    inAction[i] = false;
                }
                hurtbox = new Rectangle();
                spell1Hurtbox = new Rectangle();
                spell2Hurtbox = new Rectangle();
                inAttack = false;
                swordTimer = 0;
                castingSpell1 = false;
                spell1Timer = 0;
                projectilePosition = 0;
                castingSpell2 = false;
                spell2Timer = 0;
            }

            if (map.getPlayerRoom().getEntered() == false) {
                map.getPlayerRoom().updatedEntered();
                map.getPlayerRoom().initializeEnemies(this);
            }

            // basic attack

            if (hasACards[0] && kb.IsKeyDown(attackKeys[aCardsPlayerHas.IndexOf(0)]) && !oldkb.IsKeyDown(attackKeys[aCardsPlayerHas.IndexOf(0)]) && !inAttack && cooldownTimers[aCardsPlayerHas.IndexOf(0)] == 0) {
                inAction[aCardsPlayerHas.IndexOf(0)] = true;
                inAttack = true;
                cooldownTimers[aCardsPlayerHas.IndexOf(0)] = 25;
                soundEffects[4].Play();
            }

            if (hasACards[0] && inAction[aCardsPlayerHas.IndexOf(0)]) {
                swordTimer++;

                if (swordTimer == 1) {
                    swordFacing = facing;
                }

                if (swordFacing == 0) {
                    if (swordTimer > 0 && swordTimer <= 8) {
                        hurtbox = new Rectangle(rect.X - 20, rect.Y - 50 - swordBonus, 32, 60 + swordBonus);
                        hurtboxNum = 1;
                    }
                    if (swordTimer > 8 && swordTimer <= 16) {
                        hurtbox = new Rectangle(rect.X + 10, rect.Y - 60 - swordBonus, 32, 60 + swordBonus);
                        hurtboxNum = 2;
                    }
                    if (swordTimer > 16 && swordTimer <= 24) {
                        hurtbox = new Rectangle(rect.X + 40, rect.Y - 50 - swordBonus, 32, 60 + swordBonus);
                        hurtboxNum = 3;
                    }
                    if (swordTimer == 25) {
                        hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(0)] = false;
                        inAttack = false;
                        swordTimer = 0;
                        hurtboxNum = 0;
                    }
                }

                if (swordFacing == 1) {
                    if (swordTimer > 0 && swordTimer <= 8) {
                        hurtbox = new Rectangle(rect.X + 40, rect.Y - 20, 60 + swordBonus, 32);
                        hurtboxNum = 4;
                    }
                    if (swordTimer > 8 && swordTimer <= 16) {
                        hurtbox = new Rectangle(rect.X + 50, rect.Y + 10, 60 + swordBonus, 32);
                        hurtboxNum = 5;
                    }
                    if (swordTimer > 16 && swordTimer <= 24) {
                        hurtbox = new Rectangle(rect.X + 40, rect.Y + 40, 60 + swordBonus, 32);
                        hurtboxNum = 6;
                    }
                    if (swordTimer == 25) {
                        hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(0)] = false;
                        inAttack = false;
                        swordTimer = 0;
                        hurtboxNum = 0;
                    }
                }

                if (swordFacing == 2) {
                    if (swordTimer > 0 && swordTimer <= 8) {
                        hurtbox = new Rectangle(rect.X - 20, rect.Y + 40, 32, 60 + swordBonus);
                        hurtboxNum = 7;
                    }
                    if (swordTimer > 8 && swordTimer <= 16) {
                        hurtbox = new Rectangle(rect.X + 10, rect.Y + 50, 32, 60 + swordBonus);
                        hurtboxNum = 8;
                    }
                    if (swordTimer > 16 && swordTimer <= 24) {
                        hurtbox = new Rectangle(rect.X + 40, rect.Y + 40, 32, 60 + swordBonus);
                        hurtboxNum = 9;
                    }
                    if (swordTimer == 25) {
                        hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(0)] = false;
                        inAttack = false;
                        swordTimer = 0;
                        hurtboxNum = 0;
                    }
                }

                if (swordFacing == 3) {
                    if (swordTimer > 0 && swordTimer <= 8) {
                        hurtbox = new Rectangle(rect.X - 40 - swordBonus, rect.Y - 20, 60 + swordBonus, 32);
                        hurtboxNum = 10;
                    }
                    if (swordTimer > 8 && swordTimer <= 16) {
                        hurtbox = new Rectangle(rect.X - 50 - swordBonus, rect.Y + 10, 60 + swordBonus, 32);
                        hurtboxNum = 11;
                    }
                    if (swordTimer > 16 && swordTimer <= 24) {
                        hurtbox = new Rectangle(rect.X - 40 - swordBonus, rect.Y + 40, 60 + swordBonus, 32);
                        hurtboxNum = 12;
                    }
                    if (swordTimer == 25) {
                        hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(0)] = false;
                        inAttack = false;
                        swordTimer = 0;
                        hurtboxNum = 0;
                    }
                }

            } 
          

            // spin attack

            if (hasACards[1] && kb.IsKeyDown(attackKeys[aCardsPlayerHas.IndexOf(1)]) && !oldkb.IsKeyDown(attackKeys[aCardsPlayerHas.IndexOf(1)]) && !inAttack && cooldownTimers[aCardsPlayerHas.IndexOf(1)] == 0) {
                inAction[aCardsPlayerHas.IndexOf(1)] = true;
                inAttack = true;
                cooldownTimers[aCardsPlayerHas.IndexOf(1)] = 50;
                soundEffects[4].Play();
            }

            if (hasACards[1] && inAction[aCardsPlayerHas.IndexOf(1)]) {
                swordTimer++;
                if (swordTimer > 0 && swordTimer <= 2) {
                    hurtbox = new Rectangle(rect.X + 10, rect.Y - 60 - swordBonus, 32, 60 + swordBonus);
                    hurtboxNum = 2;
                }
                if (swordTimer > 2 && swordTimer <= 4) {
                    hurtbox = new Rectangle(rect.X + 40, rect.Y - 50 - swordBonus, 32, 60 + swordBonus);
                    hurtboxNum = 3;
                }
                if (swordTimer > 4 && swordTimer <= 6) {
                    hurtbox = new Rectangle(rect.X + 40, rect.Y - 20, 60 + swordBonus, 32);
                    hurtboxNum = 4;
                }
                if (swordTimer > 6 && swordTimer <= 8) {
                    hurtbox = new Rectangle(rect.X + 50, rect.Y + 10, 60 + swordBonus, 32);
                    hurtboxNum = 5;
                }
                if (swordTimer > 8 && swordTimer <= 10) {
                    hurtbox = new Rectangle(rect.X + 40, rect.Y + 40, 60 + swordBonus, 32);
                    hurtboxNum = 6;
                }
                if (swordTimer > 10 && swordTimer <= 12) {
                   
                    hurtbox = new Rectangle(rect.X + 40, rect.Y + 40, 32, 60 + swordBonus);
                    hurtboxNum = 9;
                }
                if (swordTimer > 12 && swordTimer <= 14) {
                    hurtbox = new Rectangle(rect.X + 10, rect.Y + 50, 32, 60 + swordBonus);
                    hurtboxNum = 8;
                }
                if (swordTimer > 14 && swordTimer <= 16) {
                    hurtbox = new Rectangle(rect.X - 20, rect.Y + 40, 32, 60 + swordBonus);
                    hurtboxNum = 7;
                }
                if (swordTimer > 16 && swordTimer <= 18) {
                
                    hurtbox = new Rectangle(rect.X - 40 - swordBonus, rect.Y + 40, 60 + swordBonus, 32);
                    hurtboxNum = 12;
                }
                if (swordTimer > 18 && swordTimer <= 20) {
                    hurtbox = new Rectangle(rect.X - 50 - swordBonus, rect.Y + 10, 60 + swordBonus, 32);
                    hurtboxNum = 11;
                }
                if (swordTimer > 20 && swordTimer <= 22) {
                    hurtbox = new Rectangle(rect.X - 40 - swordBonus, rect.Y - 20, 60 + swordBonus, 32);
                    hurtboxNum = 10;
                }
                if (swordTimer > 22 && swordTimer <= 24) {
                    hurtbox = new Rectangle(rect.X - 20, rect.Y - 50 - swordBonus, 32, 60 + swordBonus);
                    hurtboxNum = 1;
                }
                if (swordTimer == 25) {
                    hurtbox = new Rectangle();
                    inAction[aCardsPlayerHas.IndexOf(1)] = false;
                    inAttack = false;
                    swordTimer = 0;
                    hurtboxNum = 0;
                }
            }

            //dash attack

            if (hasACards[2] && kb.IsKeyDown(attackKeys[aCardsPlayerHas.IndexOf(2)]) && !oldkb.IsKeyDown(attackKeys[aCardsPlayerHas.IndexOf(2)]) && !inAttack && cooldownTimers[aCardsPlayerHas.IndexOf(2)] == 0) {
                inAction[aCardsPlayerHas.IndexOf(2)] = true;
                inAttack = true;
                cooldownTimers[aCardsPlayerHas.IndexOf(2)] = 50;
                soundEffects[4].Play();
            }

            if (hasACards[2] && inAction[aCardsPlayerHas.IndexOf(2)]) {

                swordTimer++;
               

                if (swordTimer == 1) {
                    swordFacing = facing;
                    doorInvincible = true; 
                    dooritimer = 0;
                }

                if (swordFacing == 0) {

                    if (swordTimer < 25) {
                        rect.Y-=5;
                        hurtbox = new Rectangle(rect.X + 10, rect.Y - 60 - swordBonus, 32, 60 + swordBonus);
                        hurtboxNum = 2;
                    }

                    if (swordTimer == 25) {
                        hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(2)] = false;
                        inAttack = false;
                        swordTimer = 0;
                        hurtboxNum = 0;
                    }
                }

                if (swordFacing == 1) {

                    if (swordTimer < 25) {
                        rect.X+=5;
                        hurtbox = new Rectangle(rect.X + 50, rect.Y + 10, 60 + swordBonus, 32);
                        hurtboxNum = 5;
                    }

                    if (swordTimer == 25) {
                        hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(2)] = false;
                        inAttack = false;
                        swordTimer = 0;
                        hurtboxNum = 0;
                    }
                }

                if (swordFacing == 2) {

                    if (swordTimer < 25) {
                        rect.Y+=5;
                        hurtbox = new Rectangle(rect.X + 10, rect.Y + 50, 32, 60 + swordBonus);
                        hurtboxNum = 8;
                    }

                    if (swordTimer == 25) {
                        hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(2)] = false;
                        inAttack = false;
                        swordTimer = 0;
                        hurtboxNum = 0;
                    }
                }

                if (swordFacing == 3) {

                    if (swordTimer < 25) {
                        rect.X-=5;
                        hurtbox = new Rectangle(rect.X - 60 - swordBonus, rect.Y + 10, 60 + swordBonus, 32);
                        hurtboxNum = 11;
                    }

                    if (swordTimer == 25) {
                        hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(2)] = false;
                        inAttack = false;
                        swordTimer = 0;
                        hurtboxNum = 0;
                    }
                }

            }

            if (hurtboxNum >= 1 && hurtboxNum <= 3)
                currentSwordTexture = weaponTextures[0];
            if (hurtboxNum >= 4 && hurtboxNum <= 6)
                currentSwordTexture = weaponTextures[1];
            if (hurtboxNum >= 7 && hurtboxNum <= 9)
                currentSwordTexture = weaponTextures[2];
            if (hurtboxNum >= 10 && hurtboxNum <= 12)
                currentSwordTexture = weaponTextures[3];

            //light spell

            if (hasACards[3] && kb.IsKeyDown(attackKeys[aCardsPlayerHas.IndexOf(3)]) && !oldkb.IsKeyDown(attackKeys[aCardsPlayerHas.IndexOf(3)]) && !castingSpell1 && cooldownTimers[aCardsPlayerHas.IndexOf(3)] == 0) {
                inAction[aCardsPlayerHas.IndexOf(3)] = true;
                castingSpell1 = true;
                cooldownTimers[aCardsPlayerHas.IndexOf(3)] = 100;
                soundEffects[5].Play();
            }

            if (hasACards[3] && inAction[aCardsPlayerHas.IndexOf(3)]) {

                spell1Timer++;
                projectilePosition+=4;

                if (spell1Timer == 1) {
                    spell1Facing = facing;
                    projectileY = rect.Y;
                    projectileX = rect.X;
                }

                if (spell1Facing == 0) {

                    if (spell1Timer < 50) {
                        spell1Hurtbox = new Rectangle(projectileX + 10, projectileY - 30 - projectilePosition, 32, 32);
                    }

                    if (spell1Timer == 50) {
                        spell1Hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(3)] = false;
                        castingSpell1 = false;
                        spell1Timer = 0;
                        projectilePosition = 0;
                    }
                }

                if (spell1Facing == 1) {

                    if (spell1Timer < 50) {
                        spell1Hurtbox = new Rectangle(projectileX + 50 + projectilePosition, projectileY + 10, 32, 32);
                    }

                    if (spell1Timer == 50) {
                        spell1Hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(3)] = false;
                        castingSpell1 = false;
                        spell1Timer = 0;
                        projectilePosition = 0;
                    }
                }

                if (spell1Facing == 2) {

                    if (spell1Timer < 50) {
                        spell1Hurtbox = new Rectangle(projectileX + 10, projectileY + 50 + projectilePosition, 32, 32);
                    }

                    if (spell1Timer == 50) {
                        spell1Hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(3)] = false;
                        castingSpell1 = false;
                        spell1Timer = 0;
                        projectilePosition = 0;
                    }
                }

                if (spell1Facing == 3) {

                    if (spell1Timer < 50) {
                        spell1Hurtbox = new Rectangle(projectileX - 30 - projectilePosition, projectileY + 10, 32, 32);
                    }

                    if (spell1Timer == 50) {
                        spell1Hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(3)] = false;
                        castingSpell1 = false;
                        spell1Timer = 0;
                        projectilePosition = 0;
                    }
                }

            }

            //heavy spell

            if (hasACards[4] && kb.IsKeyDown(attackKeys[aCardsPlayerHas.IndexOf(4)]) && !oldkb.IsKeyDown(attackKeys[aCardsPlayerHas.IndexOf(4)]) && !castingSpell2 && cooldownTimers[aCardsPlayerHas.IndexOf(4)] == 0) {
                inAction[aCardsPlayerHas.IndexOf(4)] = true;
                castingSpell2 = true;
                cooldownTimers[aCardsPlayerHas.IndexOf(4)] = 400;
                soundEffects[6].Play();
            }

            if (hasACards[4] && inAction[aCardsPlayerHas.IndexOf(4)]) {

                spell2Timer++;

                if (spell2Timer == 1) {
                    spell2Facing = facing;
                    projectile2Y = rect.Y;
                    projectile2X = rect.X;
                }

                if (spell2Facing == 0) {

                    if (spell2Timer < 100) {
                        spell2Hurtbox = new Rectangle(projectile2X - 72, projectile2Y - 200, 200, 200);
                    }

                    if (spell2Timer == 100) {
                        spell2Hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(4)] = false;
                        castingSpell2 = false;
                        spell2Timer = 0;
                    }
                }

                if (spell2Facing == 1) {

                    if (spell2Timer < 100) {
                        spell2Hurtbox = new Rectangle(projectile2X + 56, projectile2Y - 72, 200, 200);
                    }

                    if (spell2Timer == 100) {
                        spell2Hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(4)] = false;
                        castingSpell2 = false;
                        spell2Timer = 0;
                    }
                }

                if (spell2Facing == 2) {

                    if (spell2Timer < 100) {
                        spell2Hurtbox = new Rectangle(projectile2X - 72, projectile2Y + 56, 200, 200);
                    }

                    if (spell2Timer == 100) {
                        spell2Hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(4)] = false;
                        castingSpell2 = false;
                        spell2Timer = 0;
                    }
                }

                if (spell2Facing == 3) {

                    if (spell2Timer < 100) {
                        spell2Hurtbox = new Rectangle(projectile2X - 200, projectile2Y - 76, 200, 200);
                    }

                    if (spell2Timer == 100) {
                        spell2Hurtbox = new Rectangle();
                        inAction[aCardsPlayerHas.IndexOf(4)] = false;
                        castingSpell2 = false;
                        spell2Timer = 0;
                    }
                }

            }

            if (!inAttack) {
                swordFacing = facing;
            }
            if (!castingSpell1)
                spell1Facing = facing;
            if (!castingSpell2)
                spell2Facing = facing;

            // enemy collision
            foreach (Enemy e in map.getPlayerRoom().getEnemies()) {
                if (e.getType() >= 0 && hitbox.Intersects(e.getHurtbox()) && !invincible && !doorInvincible && !e.isInvincible()) {
                    damagePlayer();
                    soundEffects[7].Play();
                    invincible = true;
                    itimer = 0;
                }

                if (e.getType() < 0 && hitbox.Intersects(e.getHurtbox()) && (map.getPlayerRoom().getType() != 3 || (map.getPlayerRoom().getType() == 3 && e.getPrice() <= coins))) {

                    if (e.getType() > -6) {
                        if (e.getType() == -1) {
                            aCardsPlayerHas.Add(0);
                            hasACards[0] = true;

                        } else if (e.getType() == -2) {
                            aCardsPlayerHas.Add(1);
                            hasACards[1] = true;
                            
                        } else if (e.getType() == -3) {
                            aCardsPlayerHas.Add(2);
                            hasACards[2] = true;
                        } else if (e.getType() == -4) {
                            aCardsPlayerHas.Add(3);
                            hasACards[3] = true;
                        } else if (e.getType() == -5) {
                            aCardsPlayerHas.Add(4);
                            hasACards[4] = true;
                        }
                        inAction.Add(false);
                        cooldownTimers.Add(0);
                        soundEffects[2].Play();

                    } else {
                        if (e.getType() == -6) {
                            bCardsPlayerHas.Add(0);
                            hasBCards[0]++;
                            soundEffects[2].Play();
                        } else if (e.getType() == -7) {
                            bCardsPlayerHas.Add(1);
                            hasBCards[1]++;
                            soundEffects[2].Play();
                        } else if (e.getType() == -8) {
                            bCardsPlayerHas.Add(2);
                            hasBCards[2]++;
                            soundEffects[2].Play();
                        } else if (e.getType() == -9) {
                            bCardsPlayerHas.Add(3);
                            hasBCards[3]++;
                            soundEffects[2].Play();
                        } else if (e.getType() == -10) {
                            bCardsPlayerHas.Add(4);
                            hasBCards[4]++;
                            health++;
                            soundEffects[2].Play();
                        } else if (e.getType() == -11) {
                            bCardsPlayerHas.Add(5);
                            hasBCards[5]++;
                            soundEffects[2].Play();

                        } else if (e.getType() == -12) {
                            coins += 4;
                            soundEffects[0].Play();
                        } else if (e.getType() == -13) {
                            if (health < maxHealth) {
                                health++;
                                soundEffects[3].Play();
                            }
                        }
                    }
                    e.kill();

                    if (map.getPlayerRoom().getType() == 3) {
                        coins -= e.getPrice();
                        coinsSpent += e.getPrice();
                    }

                    break;
                }
            }

            if (invincible) {
                itimer++;
            }

            if (doorInvincible) {
                dooritimer++;
            }

            if (itimer > 45) {
                invincible = false;
                itimer = 0;
            }
            if (dooritimer > 60) {
                doorInvincible = false;
                dooritimer = 0;
            }

            // cooldowns

            for (int i = 0; i < cooldownTimers.Count(); i++) {
                if (cooldownTimers[i] > 0)
                    cooldownTimers[i]--;
            }

            // collision
            if (rect.X > 668)
                rect.X = 668;
            if (rect.X < 80)
                rect.X = 80;
            if (rect.Y > 668)
                rect.Y = 668;
            if (rect.Y < 40)
                rect.Y = 40;

            if (timer % 120 > 60) {
                currentTexture = textures[facing * 2];

            } else {
                currentTexture = textures[facing * 2 + 1];
            }

            if (timer < 120) {
                currentTexture = textures[8];
            }
            if (timer == 120)
                soundEffects[9].Play();

            if (map.getPlayerRoom().getType() == 4) {
                currentTexture = textures[8];
            }

            timer++;
            oldkb = kb;

        }

        public void Draw(SpriteBatch sb, SpriteFont font, Map m) {

            sb.Draw(currentSwordTexture, hurtbox, Color.White);
            sb.Draw(weaponTextures[4], spell1Hurtbox, Color.White);
            sb.Draw(weaponTextures[5], spell2Hurtbox, Color.White * 0.5f);

            if ((!doorInvincible && !invincible) || m.getPlayerRoom().getType() == 4)
                sb.Draw(currentTexture, hitbox, Color.White);
            else if (invincible)
                sb.Draw(currentTexture, hitbox, Color.Red);
            else
                sb.Draw(currentTexture, hitbox, Color.CornflowerBlue);



            for (int i = 0; i < aCardsPlayerHas.Count(); i++) {
                sb.Draw(aCards[aCardsPlayerHas[i]], new Rectangle(700 - (i * 50), 680, 80, 100), Color.White * 0.8f);
                if (aCardsPlayerHas[i] == 0)
                    sb.Draw(text, new Rectangle(700 - (i * 50), 680, 80, cooldownTimers[i] * 4), Color.Gray * 0.4f);
                else if (aCardsPlayerHas[i] == 1)
                    sb.Draw(text, new Rectangle(700 - (i * 50), 680, 80, cooldownTimers[i] * 2), Color.Gray * 0.4f);
                else if (aCardsPlayerHas[i] == 2)
                    sb.Draw(text, new Rectangle(700 - (i * 50), 680, 80, cooldownTimers[i] * 2), Color.Gray * 0.4f);
                else if (aCardsPlayerHas[i] == 3)
                    sb.Draw(text, new Rectangle(700 - (i * 50), 680, 80, cooldownTimers[i]), Color.Gray * 0.4f);
                else if (aCardsPlayerHas[i] == 4)
                    sb.Draw(text, new Rectangle(700 - (i * 50), 680, 80, cooldownTimers[i] / 4), Color.Gray * 0.4f);
                sb.DrawString(font, keyStrings[i], new Vector2(735 - (i * 50), 780), Color.White);
            }
            if (bCardsPlayerHas.Count() <= 6) {
                for (int i = 0; i < bCardsPlayerHas.Count(); i++) {
                    sb.Draw(bCards[bCardsPlayerHas[i]], new Rectangle(20 + (i * 50), 680, 80, 100), Color.White * 0.8f);
                }
            } else if (bCardsPlayerHas.Count() <= 10) {
                for (int i = 0; i < bCardsPlayerHas.Count(); i++) {
                    sb.Draw(bCards[bCardsPlayerHas[i]], new Rectangle(20 + (i * 32), 680, 80, 100), Color.White * 0.8f);
                }
            } else {
                for (int i = 0; i < bCardsPlayerHas.Count(); i++) {
                    sb.Draw(bCards[bCardsPlayerHas[i]], new Rectangle(20 + (i * 20), 680, 80, 100), Color.White * 0.8f);
                }
            }

            for (int i = 0; i < maxHealth; i++) { 
                if (health > i)
                    sb.Draw(heart, new Rectangle(752 - (i * 32), 20, 28, 24), Color.White);
                else
                    sb.Draw(emptyHeart, new Rectangle(752 - (i * 32), 20, 28, 24), Color.White);
            }
            for (int i = 0; i <= coins; i++) {
                if (i != coins)
                    sb.Draw(coin, new Rectangle(20 + i * 8, 20, 32, 32), Color.White);
                else if (coins != 0)
                    sb.DrawString(font, coins.ToString(), new Vector2(20 + i * 8, 25), Color.Black);
            }

        }



    }
}
