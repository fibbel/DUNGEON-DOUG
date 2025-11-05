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
    class Map {

        Room[,] map;
        Random rg;
        int rows;
        int cols;
        Room playerRoom;

        public Map(List<Texture2D> wallSpriteList, List<Texture2D> doorSpriteList, List<Texture2D> enemySpriteList, List<Texture2D> acards, List<Texture2D> bcards, Texture2D pixel, int rs, int cs) {

            rows = rs;
            cols = cs;

            rg = new Random(Guid.NewGuid().GetHashCode());

            int sum;
            int diff;
            int randD;
            bool valid = false;
            bool firstPass;
            map = new Room[rows, cols];
            int[] currentRoom;

            while (!valid) {

                for (int r = 0; r < rows; r++) {
                    for (int c = 0; c < cols; c++) {
                        map[r, c] = null;
                    }
                }

                map[0, 0] = (new Room(wallSpriteList, doorSpriteList, enemySpriteList, acards, bcards, pixel, 0, 1, 1, 0, 0, 0));
                firstPass = true;

                for (int r = 0; r < rows; r++) {
                    for (int c = 0; c < cols; c++) {

                        if (!firstPass) {

                            currentRoom = new int[] { 0, 0, 0, 0 };
                            for (int i = 0; i < 4; i++) {
                                currentRoom[i] = rg.Next(0, 2);
                            }

                            if (r != 0)
                                currentRoom[0] = map[r - 1, c].getSideStatus(2);
                            else
                                currentRoom[0] = 0;

                            if (c == cols - 1)
                                currentRoom[1] = 0;
                            if (r == rows - 1)
                                currentRoom[2] = 0;

                            if (c != 0)
                                currentRoom[3] = map[r, c - 1].getSideStatus(1);
                            else
                                currentRoom[3] = 0;

                            sum = r + c;
                            diff = 0;
                       
                            if (sum >= 3 && sum <= 6) {
                                randD = rg.Next(1, 4);
                                if (randD != 1)
                                    diff = 1;
                            }
                            if (sum >= 7 && sum <= 10) {
                                randD = rg.Next(1, 5);
                                if (randD == 1)
                                    diff = 1;
                                if (randD > 1)
                                    diff = 2;
                            }
                            if (sum >= 11) {
                                randD = rg.Next(1, 4);
                                if (randD == 1)
                                    diff = 2;
                                else
                                    diff = 3;
                            }

                            map[r, c] = new Room(wallSpriteList, doorSpriteList, enemySpriteList, acards, bcards, pixel, currentRoom[0], currentRoom[1], currentRoom[2], currentRoom[3], -1, diff);

                            if ((currentRoom[0] == 1 && map[r - 1, c].getType() != -1) || (currentRoom[3] == 1 && map[r, c - 1].getType() != -1))
                                map[r, c].updateType(1);
                        }

                        firstPass = false;
                    }
                }

                // accessibility run-throughs
                map[0, 0].updateType(1);
                map[rows - 1, cols - 1].updateType(1);
                for (int r = rows - 1; r >= 0; r--) {
                    for (int c = cols - 1; c >= 0; c--) {
                        if ((map[r, c].getSideStatus(0) == 1 && map[r - 1, c].getType() != -1) || (map[r, c].getSideStatus(3) == 1 && map[r, c - 1].getType() != -1) || (map[r, c].getSideStatus(2) == 1 && map[r + 1, c].getType() != -1) || (map[r, c].getSideStatus(1) == 1 && map[r, c + 1].getType() != -1))
                            map[r, c].updateType(1);
                    }
                    for (int c = 0; c < cols; c++) {
                        if ((map[r, c].getSideStatus(0) == 1 && map[r - 1, c].getType() != -1) || (map[r, c].getSideStatus(3) == 1 && map[r, c - 1].getType() != -1) || (map[r, c].getSideStatus(2) == 1 && map[r + 1, c].getType() != -1) || (map[r, c].getSideStatus(1) == 1 && map[r, c + 1].getType() != -1))
                            map[r, c].updateType(1);
                    }
                }
                for (int r = 0; r < rows; r++) {
                    for (int c = cols - 1; c >= 0; c--) {
                        if ((map[r, c].getSideStatus(0) == 1 && map[r - 1, c].getType() != -1) || (map[r, c].getSideStatus(3) == 1 && map[r, c - 1].getType() != -1) || (map[r, c].getSideStatus(2) == 1 && map[r + 1, c].getType() != -1) || (map[r, c].getSideStatus(1) == 1 && map[r, c + 1].getType() != -1))
                            map[r, c].updateType(1);
                    }
                    for (int c = 0; c < cols; c++) {
                        if ((map[r, c].getSideStatus(0) == 1 && map[r - 1, c].getType() != -1) || (map[r, c].getSideStatus(3) == 1 && map[r, c - 1].getType() != -1) || (map[r, c].getSideStatus(2) == 1 && map[r + 1, c].getType() != -1) || (map[r, c].getSideStatus(1) == 1 && map[r, c + 1].getType() != -1))
                            map[r, c].updateType(1);
                    }
                }
                for (int c = 0; c < cols; c++) {
                    for (int r = rows - 1; r >= 0; r--) {
                        if ((map[r, c].getSideStatus(0) == 1 && map[r - 1, c].getType() != -1) || (map[r, c].getSideStatus(3) == 1 && map[r, c - 1].getType() != -1) || (map[r, c].getSideStatus(2) == 1 && map[r + 1, c].getType() != -1) || (map[r, c].getSideStatus(1) == 1 && map[r, c + 1].getType() != -1))
                            map[r, c].updateType(1);
                    }
                }


                // check validity

                int currentCol = 0;
                int currentRow = 0;
                int timesPassedStart = 0;
                int facing = 1;

                while (timesPassedStart < 3 && valid == false) {

                    if (map[currentRow, currentCol].getSideStatus(facing) == 1) {

                        if (facing == 0)
                            currentRow -= 1;
                        if (facing == 1)
                            currentCol += 1;
                        if (facing == 2)
                            currentRow += 1;
                        if (facing == 3)
                            currentCol -= 1;
                        facing -= 1;
                        if (facing == -1)
                            facing = 3;

                    } else {
                        facing += 1;
                        if (facing == 4)
                            facing = 0;
                    }

                    if (currentRow == 0 && currentCol == 0)
                        timesPassedStart += 1;

                    if (currentRow == rows - 1 && currentCol == cols - 1)
                        valid = true;
                }

                int numInaccessible = 0;
                for (int r = 0; r < rows; r++) {
                    for (int c = 0; c < cols; c++) {
                        if (map[r, c].getType() == -1)
                            numInaccessible++;
                    }
                }
                if (numInaccessible > 10)
                    valid = false;

            }

            playerRoom = map[0, 0];

            map[0, 0].updateType(0);
            map[rows - 1, cols - 1].updateType(4);

            // type assigning

            List<Room> allRooms = new List<Room>();

            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < cols; c++) {
                    if (map[r, c].getType() == 1 && !(r == 1 && c == 0) && !(r == 0 && c == 1) && !(r == rows - 2 && c == cols - 1) && !(r == rows - 1 && c == cols - 2))
                        allRooms.Add(map[r, c]);
                }
            }

            for (int i = allRooms.Count() - 1; i > 0; i--) {
                int k = rg.Next(i + 1);
                Room value = allRooms[k];
                allRooms[k] = allRooms[i];
                allRooms[i] = value;
            }

            int counter = 0;
            bool found = false;
            int[] rowCol;
            int sum2;
            while (!found) {
                rowCol = allRooms[counter].getRowCol(this);
                sum2 = rowCol[0] + rowCol[1];
                if (sum2 >= 5 && sum2 <= 11 && (rowCol[0] >= 5 || rowCol[0] < rows) && (rowCol[1] >= 5 || rowCol[1] < cols)) {
                    found = true;
                    allRooms[counter].updateType(3);
                    allRooms[counter].updateDiff(1);
                    allRooms.Remove(allRooms[counter]);
                }
                counter++;
            }

            allRooms[0].updateType(2);
            allRooms[0].updateDiff(0);
            allRooms[1].updateType(2);
            allRooms[1].updateDiff(0);
            allRooms[2].updateType(2);
            allRooms[2].updateDiff(0);
            allRooms[3].updateType(2);
            allRooms[3].updateDiff(0);
            allRooms[4].updateType(2);
            allRooms[4].updateDiff(1);
            allRooms[5].updateType(2);
            allRooms[5].updateDiff(1);
            allRooms[6].updateType(2);
            allRooms[6].updateDiff(2);
            allRooms[11].updateType(2);
            allRooms[11].updateDiff(2);
            allRooms[12].updateType(2);
            allRooms[12].updateDiff(2);
            allRooms[13].updateType(2);
            allRooms[13].updateDiff(0);
            allRooms[14].updateType(2);
            allRooms[14].updateDiff(0);

            allRooms[7].updateType(3);
            allRooms[7].updateDiff(0);
            allRooms[8].updateType(3);
            allRooms[8].updateDiff(0);
            allRooms[9].updateType(3);
            allRooms[9].updateDiff(0);
            if (!found) {
                allRooms[10].updateType(3);
                allRooms[10].updateDiff(1);
            }

        }
    

        public Room getRoom(int r, int c) { return map[r, c]; }
        public Room getPlayerRoom() { return playerRoom; }

        public Room[,] getMap() { return map; }

        public void updatePlayerRoom(int r, int c) { playerRoom = map[r, c]; }
        public void updatePlayerRoom(Room room) { playerRoom = room; }

    }
}
