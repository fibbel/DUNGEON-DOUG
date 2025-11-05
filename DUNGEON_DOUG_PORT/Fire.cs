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
    class Fire {
        private Texture2D pix;
        private int x, y;
        private List<FirePixel> fps;
        private Random rg;
        private float[] opacities;
        private Color lightColor;
        private List<Rectangle> pixels;
        private List<float> pixelOpacities;

        public Fire(Texture2D p, int x, int y) {
            opacities = new float[5];
            for (int i = 0; i < 5; i++) {
                opacities[i] = (float)(0.65 - (i * 0.05));
            }
            rg = new Random(Guid.NewGuid().GetHashCode());
            fps = new List<FirePixel>();
            lightColor = Color.LightYellow;
            this.x = x;
            this.y = y;
            this.pix = p;
            /*
            List<Rectangle> pixs = new List<Rectangle>();
            pixels = new List<Rectangle>();
            pixelOpacities = new List<float>();
            for (int o = 0; o < opacities.Length; o++) {
                pixs = pixelsTouchingCircumference(x, y, 8 + (o * 4));
                foreach (Rectangle temp in pixs) {
                    pixels.Add(temp);
                    pixelOpacities.Add(opacities[o]);
                }
            }*/

            for (int i = 0; i < 3; i++) {
                fps.Add(new FirePixel(new Rectangle(x + ((i * 4) - 4), y + 4, 4, 4), pix, rg.Next(128, 205), rg.Next(0, 154)));
                fps.Add(new FirePixel(new Rectangle(x + ((i * 4) - 4), y, 4, 4), pix, rg.Next(128, 205), rg.Next(0, 154)));
                fps.Add(new FirePixel(new Rectangle(x + ((i * 4) - 4), y - 4, 4, 4), pix, rg.Next(128, 205), rg.Next(0, 154)));
            }
        }

        public List<Rectangle> pixelsTouchingCircumference(int originx, int originy, int r) {

            List<Rectangle> result = new List<Rectangle>();
            List<Rectangle> allNearPixels = new List<Rectangle>();
            List<double> xValues = new List<double>();

            for (double i = -1 * r; i <= r; i += 0.1) {
                xValues.Add(i);
            }


            for (int i = r * -1; i <= 0; i += 4) {
                for (int j = r * -1; j <= (int)((r * -1) / 1.414); j += 4) {
                    allNearPixels.Add(new Rectangle(i, j, 4, 4));
                    allNearPixels.Add(new Rectangle(j, i, 4, 4));
                }
            }

            double x, y;
            int counter = 0;
            while (counter < xValues.Count) {
                x = xValues[counter];
                y = (Math.Sqrt(Math.Pow(r, 2) - Math.Pow(x, 2)));
                foreach (Rectangle temp in allNearPixels) {
                    if (temp.Intersects(new Rectangle((int)x, (int)y, 1, 1)) || temp.Intersects(new Rectangle((int)x, (int)-y, 1, 1))) {
                        result.Add(new Rectangle(temp.X + originx, temp.Y + originy, 4, 4));
                        result.Add(new Rectangle(temp.X + originx, -temp.Y + originy, 4, 4));
                        result.Add(new Rectangle(-temp.X + originx, temp.Y + originy, 4, 4));
                        result.Add(new Rectangle(-temp.X + originx, -temp.Y + originy, 4, 4));
                        counter++;
                    }
                }
            }

            return result;
        }

        public void Update() {
            foreach (FirePixel temp in fps) {
                temp.Update();
            }
        }

        public void Draw(SpriteBatch sb) {

            sb.Draw(pix, new Rectangle(x, y + 8, 4, 4), Color.Brown);
            sb.Draw(pix, new Rectangle(x + 4, y + 8, 4, 4), Color.Brown);
            sb.Draw(pix, new Rectangle(x - 4, y + 8, 4, 4), Color.Brown);
            sb.Draw(pix, new Rectangle(x - 8, y + 4, 4, 4), Color.Brown);
            sb.Draw(pix, new Rectangle(x + 8, y + 4, 4, 4), Color.Brown);

            foreach (FirePixel temp in fps) {
                temp.Draw(sb);
            }
            /*
            for (int i = 0; i < pixels.Count; i++) {
                sb.Draw(pix, pixels[i], lightColor * pixelOpacities[i]);
            }*/
        }


    }
}
