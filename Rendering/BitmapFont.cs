using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoulderDashSnilku.Rendering
{
    public class BitmapFont
    {
        private const int GlyphWidth = 8;
        private const int GlyphHeight = 8;
        private const int GlyphsPerRow = 16;

        private readonly Texture2D texture;
        private readonly Texture2D pixel;

        public BitmapFont(Texture2D originalTexture)
        {
            texture = CreateTransparentFontTexture(originalTexture);

            pixel = new Texture2D(originalTexture.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color characterColor, Color? backgroundColor = null)
        {
            int startX = (int)position.X;
            int drawX = startX;
            int drawY = (int)position.Y;

            foreach (char character in text)
            {
                if ( character == '\n')
                {
                    drawX = startX;
                    drawY += GlyphHeight;
                    continue;
                }
                Rectangle destination = new Rectangle(drawX, drawY, GlyphWidth, GlyphHeight);
                
                if (backgroundColor.HasValue)
                {
                    spriteBatch.Draw(pixel, destination, backgroundColor.Value);
                }
                if (character != ' ')
                {
                    byte code = GetCodePage437Code(character);

                    int sourceX = (code % GlyphsPerRow) * GlyphWidth;
                    int sourceY = (code / GlyphsPerRow) * GlyphHeight;

                    Rectangle source = new Rectangle(sourceX, sourceY, GlyphWidth, GlyphHeight);

                    spriteBatch.Draw(texture, destination, source, characterColor);
                }
                drawX += GlyphWidth;
            }
        }

        public void DrawCenteredText(SpriteBatch spriteBatch, string text, float y, float screenWidth, Color characterColor, Color? backgroundColor = null)
        {
            Vector2 textSize = MeasureString(text);
            Vector2 position = new Vector2(screenWidth - textSize.X / 2, y);
            DrawText(spriteBatch, text, position, characterColor, backgroundColor);
        }

        public Vector2 MeasureString(string text)
        {
            int currentWidth = 0;
            int maxWidth = 0;
            int lines = 1;

            foreach (char character in text)
            {
                if (character == '\n')
                {
                    maxWidth = Math.Max(maxWidth, currentWidth);
                    currentWidth = 0;
                    lines++;
                }
                else
                {
                    currentWidth += GlyphWidth;
                }
            }

            maxWidth = Math.Max(maxWidth, currentWidth);
            return new Vector2(maxWidth, lines * GlyphHeight);
        }

        private Texture2D CreateTransparentFontTexture(Texture2D originalTexture)
        {
            Color[] pixels = new Color[originalTexture.Width * originalTexture.Height];
            originalTexture.GetData(pixels);

            for (int i = 0; i < pixels.Length; i++)
            {
                Color sourceColor = pixels[i];

                if (sourceColor.R < 10 && sourceColor.G < 10 && sourceColor.B < 10)
                {
                    pixels[i] = Color.Transparent;
                }
                else
                {
                    pixels[i] = Color.White;
                }
            }
            Texture2D transparentTexture = new Texture2D(originalTexture.GraphicsDevice, originalTexture.Width, originalTexture.Height);
            transparentTexture.SetData(pixels);

            return transparentTexture;
        }

        private static byte GetCodePage437Code(char character)
        {
            if (character >= 32 && character <= 126)
            {
                return (byte)character;
            }

            return character switch
            {
                '☺' => 1,
                '☻' => 2,
                '♥' => 3,
                '♦' => 4,
                '♣' => 5,
                '♠' => 6,
                '•' => 7,
                '◘' => 8,
                '○' => 9,
                '◙' => 10,
                '♂' => 11,
                '♀' => 12,
                '♪' => 13,
                '♫' => 14,
                '☼' => 15,
                '►' => 16,
                '◄' => 17,
                '↕' => 18,
                '‼' => 19,
                '¶' => 20,
                '§' => 21,
                '▬' => 22,
                '↨' => 23,
                '↑' => 24,
                '↓' => 25,
                '→' => 26,
                '←' => 27,
                '∟' => 28,
                '↔' => 29,
                '▲' => 30,
                '▼' => 31,

                '⌂' => 127,
                'Ç' => 128,
                'ü' => 129,
                'é' => 130,
                'â' => 131,
                'ä' => 132,
                'ů' => 133,
                'ć' => 134,
                'ç' => 135,
                'ł' => 136,
                'ë' => 137,
                'Ő' => 138,
                'ő' => 139,
                'î' => 140,
                'Ź' => 141,
                'Ä' => 142,
                'Ć' => 143,
                'É' => 144,
                'Ĺ' => 145,
                'ĺ' => 146,
                'ô' => 147,
                'ö' => 148,
                'Ľ' => 149,
                'ľ' => 150,
                'Ś' => 151,
                'ś' => 152,
                'Ö' => 153,
                'Ü' => 154,
                'Ť' => 155,
                'ť' => 156,
                'Ł' => 157,
                '×' => 158,
                'č' => 159,
                'á' => 160,
                'í' => 161,
                'ó' => 162,
                'ú' => 163,
                'Ą' => 164,
                'ą' => 165,
                'Ž' => 166,
                'ž' => 167,
                'Ę' => 168,
                'ę' => 169,
                '¬' => 170,
                'ź' => 171,
                'Č' => 172,
                'ş' => 173,
                '«' => 174,
                '»' => 175,
                '░' => 176,
                '▒' => 177,
                '▓' => 178,
                '│' => 179,
                '┤' => 180,
                'Á' => 181,
                'Â' => 182,
                'Ě' => 183,
                'Ş' => 184,
                '╣' => 185,
                '║' => 186,
                '╗' => 187,
                '╝' => 188,
                'Ż' => 189,
                'ż' => 190,
                '┐' => 191,
                '└' => 192,
                '┴' => 193,
                '┬' => 194,
                '├' => 195,
                '─' => 196,
                '┼' => 197,
                'Ă' => 198,
                'ă' => 199,
                '╚' => 200,
                '╔' => 201,
                '╩' => 202,
                '╦' => 203,
                '╠' => 204,
                '═' => 205,
                '╬' => 206,
                '¤' => 207,
                'đ' => 208,
                'Đ' => 209,
                'Ď' => 210,
                'Ë' => 211,
                'ď' => 212,
                'Ň' => 213,
                'Í' => 214,
                'Î' => 215,
                'ě' => 216,
                '┘' => 217,
                '┌' => 218,
                '█' => 219,
                '▄' => 220,
                'Ţ' => 221,
                'Ů' => 222,
                '▀' => 223,
                'Ó' => 224,
                'ß' => 225,
                'Ô' => 226,
                'Ń' => 227,
                'ń' => 228,
                'ň' => 229,
                'Š' => 230,
                'š' => 231,
                'Ŕ' => 232,
                'Ú' => 233,
                'ŕ' => 234,
                'Ű' => 235,
                'ý' => 236,
                'Ý' => 237,
                'ţ' => 238,
                '´' => 239,
                '­' => 240,
                '˝' => 241,
                '˛' => 242,
                'ˇ' => 243,
                '˘' => 244,
                //'§' => 245,
                '÷' => 246,
                '¸' => 247,
                '°' => 248,
                '¨' => 249,
                '˙' => 250,
                'ű' => 251,
                'Ř' => 252,
                'ř' => 253,
                '■' => 254,
                ' ' => 255,


                _ => (byte)'?'
            };
        }
    }
}
