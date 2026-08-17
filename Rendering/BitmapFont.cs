using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoulderDashSnilku.Rendering
{
    /// <summary>
    /// Draws text using 8x8 bitmap char atlas.
    /// Supports foreground coloring and optional backgrounds.
    /// Provides centered text.
    /// </summary>
    public class BitmapFont
    {
        private const int GlyphWidth = 8;
        private const int GlyphHeight = 8;
        private const int GlyphsPerRow = 16;
        // Chars 0-31 | 0 is unused.
        private const string ControlChars =
            " ☺☻♥♦♣♠•◘○◙♂♀♪♫☼" +
            "►◄↕‼¶§▬↨↑↓→←∟↔▲▼";
        // Chars 127-255 -> matching bitmap atlas
        private const string ExtendedChars =
           "⌂ÇüéâäůćçłëŐőîŹÄĆ" +
            "ÉĹĺôöĽľŚśÖÜŤťŁ×č" +
            "áíóúĄąŽžĘę¬źČş«»" +
            "░▒▓│┤ÁÂĚŞ╣║╗╝Żż┐" +
            "└┴┬├─┼Ăă╚╔╩╦╠═╬¤" +
            "đĐĎËďŇÍÎě┘┌█▄ŢŮ▀" +
            "ÓßÔŃńňŠšŔÚŕŰýÝţ´" +
            "­˝˛ˇ˘§÷¸°¨˙űŘř■ ";
        private readonly Texture2D texture;
        private readonly Texture2D pixel;

        public BitmapFont(Texture2D originalTexture)
        {
            texture = CreateTransparentFontTexture(originalTexture);
            pixel = new Texture2D(originalTexture.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Draw bitmap text from supplied position using requested colors.
        /// Background remains transparent when backgroundColor is null.
        /// </summary>
        public void DrawText(SpriteBatch spriteBatch, string text,
            Vector2 position, Color characterColor, Color? backgroundColor = null)
        {
            int startX = (int)position.X;
            int drawX = startX;
            int drawY = (int)position.Y;
            foreach (char character in text)
            {
                if (character == '\n')
                {
                    drawX = startX;
                    drawY += GlyphHeight;
                }
                else
                {
                    Rectangle destination = new Rectangle(drawX, drawY,
                        GlyphWidth, GlyphHeight);
                    if (backgroundColor.HasValue) spriteBatch.Draw(
                        pixel, destination, backgroundColor.Value);
                    if (character != ' ')
                    {
                        byte code = GetCodePage437Code(character);
                        Rectangle source = new Rectangle(
                            (code % GlyphsPerRow) * GlyphWidth,
                            (code / GlyphsPerRow) * GlyphHeight,
                            GlyphWidth, GlyphHeight);
                        spriteBatch.Draw(texture, destination, source, characterColor);
                    }
                    drawX += GlyphWidth;
                }
            }
        }

        /// <summary>
        /// Draw text centered horisontally based on given screen width.
        /// </summary>
        public void DrawCenteredText(SpriteBatch spriteBatch, string text,
            float y, float screenWidth, Color characterColor, Color? backgroundColor = null)
        {
            Vector2 textSize = MeasureString(text);
            Vector2 position = new Vector2((screenWidth - textSize.X) / 2, y);
            DrawText(spriteBatch, text, position, characterColor, backgroundColor);
        }

        /// <summary>
        /// Calculate logical pixel dimentions required to draw given text.
        /// Handles multiple lines using fixed 8x8 glyph size.
        /// </summary>
        /// <returns>Width and Height of given rendered text in logical pixels.</returns>
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
                else currentWidth += GlyphWidth;
            }
            maxWidth = Math.Max(maxWidth, currentWidth);
            return new Vector2(maxWidth, lines * GlyphHeight);
        }

        /// <summary>
        /// Create copy of font atlas where black pixels become transparent.
        /// Character pixels are become white. (for further tinting when drawing)
        /// </summary>
        /// <returns>2D transparent Texture from original texture.</returns>
        private static Texture2D CreateTransparentFontTexture(Texture2D originalTexture)
        {
            Color[] pixels = new Color[originalTexture.Width * originalTexture.Height];
            originalTexture.GetData(pixels);
            for (int i = 0; i < pixels.Length; i++)
            {
                Color sourceColor = pixels[i];
                if (sourceColor.R < 10 && sourceColor.G < 10 && sourceColor.B < 10)
                    pixels[i] = Color.Transparent;
                else pixels[i] = Color.White;
            }
            Texture2D transparentTexture = new Texture2D(originalTexture.GraphicsDevice,
                originalTexture.Width, originalTexture.Height);
            transparentTexture.SetData(pixels);
            return transparentTexture;
        }

        /// <summary>
        /// Convert Unicode character to its corresponding position in bitmap atlas.
        /// Unsuported chars are replaced with '?'
        /// </summary>
        /// <returns>character byte in bitmap atlas | '?'.</returns>
        private static byte GetCodePage437Code(char character) {
            int code = '?';
            int index;
            if (character >= 32 && character <= 126) code = character;
            else if ((index = ControlChars.IndexOf(character)) >= 0)
                code = index;
            else if ((index = ExtendedChars.IndexOf(character)) >= 0)
                code = index + 127;
            return (byte)code;
        }
    }
}