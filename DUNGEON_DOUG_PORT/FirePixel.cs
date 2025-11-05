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
    class FirePixel {

        private Color color;
        private Rectangle rect;
        private Texture2D text;
        private Random rg;
        private float r, g, b, xg, xb;

        public FirePixel(Rectangle rect, Texture2D texture, float g, float b) {
            rg = new Random();
            this.rect = rect;
            text = texture;

            r = 255;
            this.g = g;
            this.b = b;
            color = new Color((int)r, (int)g, (int)b);

            xg = (float)(Math.Asin((double)((g - 166) / 38)) / 0.01);
            xb = (float)(Math.Asin((double)((b - 77) / 77)) / 0.01);
        }

        public Color getColor() { return color; }
        public Rectangle getRect() { return rect; }

        public void Update() {
            xg++;
            xb++;
            g = (float)(38 * Math.Sin(0.01 * xg) + 166);
            b = (float)(77 * Math.Sin(0.01 * xb) + 77);
            color = new Color((int)r, (int)g, (int)b);
        }

        public void Draw(SpriteBatch sb) {
            sb.Draw(text, rect, color * 0.8f);
        }
    }
}
