// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using osu.Framework.Graphics.Rendering.Dummy;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Text;
using osuTK;

namespace osu.Framework.Tests.Text
{
    [TestFixture]
    public class TextBuilderTest
    {
        private const float font_size = 1;

        private const float x_offset = 1;
        private const float y_offset = 2;
        private const float x_advance = 3;
        private const float width = 4;
        private const float baseline = 5;
        private const float height = 6;
        private const float kerning = -7;

        private const float b_x_offset = 8;
        private const float b_y_offset = 9;
        private const float b_x_advance = 10;
        private const float b_width = 11;
        private const float b_baseline = 12;
        private const float b_height = 13;
        private const float b_kerning = -14;

#pragma warning disable IDE1006 // Naming style
        // m_ recognized as prefix instead of part of the name

        private const float m_x_offset = 15;
        private const float m_y_offset = 16;
        private const float m_x_advance = 17;
        private const float m_width = 18;
        private const float m_baseline = 19;
        private const float m_height = 20;
        private const float m_kerning = -21;

#pragma warning restore IDE1006

        private const float p_x_offset = 1;
        private const float p_y_offset = 2;
        private const float p_x_advance = 3;
        private const float p_width = 4;
        private const float p_baseline = 6;
        private const float p_height = 7;
        private const float p_kerning = -7;

        private const float q_x_offset = 1;
        private const float q_y_offset = 2;
        private const float q_x_advance = 3;
        private const float q_width = 4;
        private const float q_baseline = 7;
        private const float q_height = 8;
        private const float q_kerning = -7;

        private static readonly Vector2 spacing = new Vector2(22, 23);

        private static readonly TestFontUsage normal_font = new TestFontUsage("test");
        private static readonly TestFontUsage fixed_width_font = new TestFontUsage("test-fixedwidth", fixedWidth: true);

        private readonly TestStore fontStore;

        public TextBuilderTest()
        {
            fontStore = new TestStore(
                new GlyphEntry(normal_font, new TestGlyph('a', x_offset, y_offset, x_advance, width, baseline, height, kerning)),
                new GlyphEntry(normal_font, new TestGlyph('b', b_x_offset, b_y_offset, b_x_advance, b_width, b_baseline, b_height, b_kerning)),
                new GlyphEntry(normal_font, new TestGlyph('m', m_x_offset, m_y_offset, m_x_advance, m_width, m_baseline, m_height, m_kerning)),
                new GlyphEntry(normal_font, new TestGlyph('p', p_x_offset, p_y_offset, p_x_advance, p_width, p_baseline, p_height, p_kerning)),
                new GlyphEntry(normal_font, new TestGlyph('q', q_x_offset, q_y_offset, q_x_advance, q_width, q_baseline, q_height, q_kerning)),
                new GlyphEntry(fixed_width_font, new TestGlyph('a', x_offset, y_offset, x_advance, width, baseline, height, kerning)),
                new GlyphEntry(fixed_width_font, new TestGlyph('b', b_x_offset, b_y_offset, b_x_advance, b_width, b_baseline, b_height, b_kerning)),
                new GlyphEntry(fixed_width_font, new TestGlyph('m', m_x_offset, m_y_offset, m_x_advance, m_width, m_baseline, m_height, m_kerning))
            );
        }

        /// <summary>
        /// Tests that the size of a fresh text builder is zero.
        /// </summary>
        [Test]
        public void TestInitialSizeIsZero()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            Assert.That(builder.Bounds, Is.EqualTo(Vector2.Zero));
        }

        /// <summary>
        /// Tests that the first added character is correctly marked as being on a new line.
        /// </summary>
        [Test]
        public void TestFirstCharacterIsOnNewLine()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("a");

            Assert.That(builder.Characters[0].OnNewLine, Is.True);
        }

        /// <summary>
        /// Tests that the first added fixed-width character metrics match the glyph's.
        /// </summary>
        [Test]
        public void TestFirstCharacterRectangleIsCorrect()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("a");

            Assert.That(builder.Characters[0].DrawRectangle.Left, Is.EqualTo(x_offset));
            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset));
            Assert.That(builder.Characters[0].DrawRectangle.Width, Is.EqualTo(width));
            Assert.That(builder.Characters[0].DrawRectangle.Height, Is.EqualTo(height));
        }

        /// <summary>
        /// Tests that the first added character metrics match the glyph's.
        /// </summary>
        [Test]
        public void TestFirstFixedWidthCharacterRectangleIsCorrect()
        {
            var builder = new TextBuilder(fontStore, fixed_width_font);

            builder.AddText("a");

            Assert.That(builder.Characters[0].DrawRectangle.Left, Is.EqualTo((m_width - width) / 2));
            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset));
            Assert.That(builder.Characters[0].DrawRectangle.Width, Is.EqualTo(width));
            Assert.That(builder.Characters[0].DrawRectangle.Height, Is.EqualTo(height));
        }

        /// <summary>
        /// Tests that the current position is advanced after a character is added.
        /// </summary>
        [Test]
        public void TestCurrentPositionAdvancedAfterCharacter()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("a");
            builder.AddText("a");

            Assert.That(builder.Characters[1].DrawRectangle.Left, Is.EqualTo(x_advance + kerning + x_offset));
            Assert.That(builder.Characters[1].DrawRectangle.Top, Is.EqualTo(y_offset));
            Assert.That(builder.Characters[1].DrawRectangle.Width, Is.EqualTo(width));
            Assert.That(builder.Characters[1].DrawRectangle.Height, Is.EqualTo(height));
        }

        /// <summary>
        /// Tests that the current position is advanced after a fixed width character is added.
        /// </summary>
        [Test]
        public void TestCurrentPositionAdvancedAfterFixedWidthCharacter()
        {
            var builder = new TextBuilder(fontStore, fixed_width_font);

            builder.AddText("a");
            builder.AddText("a");

            Assert.That(builder.Characters[1].DrawRectangle.Left, Is.EqualTo(m_width + (m_width - width) / 2));
            Assert.That(builder.Characters[1].DrawRectangle.Top, Is.EqualTo(y_offset));
            Assert.That(builder.Characters[1].DrawRectangle.Width, Is.EqualTo(width));
            Assert.That(builder.Characters[1].DrawRectangle.Height, Is.EqualTo(height));
        }

        /// <summary>
        /// Tests that a new line added to an empty builder always uses the font height.
        /// </summary>
        [Test]
        public void TestNewLineOnEmptyBuilderOffsetsPositionByFontSize()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddNewLine();
            builder.AddText("a");

            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(font_size + y_offset));
        }

        /// <summary>
        /// Tests that a new line added to an empty line always uses the font height.
        /// </summary>
        [Test]
        public void TestNewLineOnEmptyLineOffsetsPositionByFontSize()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddNewLine();
            builder.AddNewLine();
            builder.AddText("a");

            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset + y_offset));
        }

        /// <summary>
        /// Tests that a new line added to a builder that is using the font height as size offsets the y-position by the font size and not the glyph size.
        /// </summary>
        [Test]
        public void TestNewLineUsesFontHeightWhenUsingFontHeightAsSize()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("a");
            builder.AddText("b");
            builder.AddNewLine();
            builder.AddText("a");

            Assert.That(builder.Characters[2].DrawRectangle.Top, Is.EqualTo(font_size + y_offset));
        }

        /// <summary>
        /// Tests that a new line added to a builder that is not using the font height as size offsets the y-position by the glyph size and not the font size.
        /// </summary>
        [Test]
        public void TestNewLineUsesGlyphHeightWhenNotUsingFontHeightAsSize()
        {
            var builder = new TextBuilder(fontStore, normal_font, useFontSizeAsHeight: false);

            builder.AddText("a");
            builder.AddText("b");
            builder.AddNewLine();
            builder.AddText("a");

            // b is the larger glyph
            Assert.That(builder.Characters[2].DrawRectangle.Top, Is.EqualTo(b_height));
        }

        /// <summary>
        /// Tests that the bounds are set correctly when useFontSizeAsHeight is false
        /// </summary>
        [Test]
        public void TestBoundsWhenNotUsingFontHeightAsSize()
        {
            var builder = new TextBuilder(fontStore, normal_font, useFontSizeAsHeight: false);

            builder.AddText("am");
            builder.AddNewLine();
            builder.AddText("ab");

            // b and m are the larger glyph in their line
            Assert.That(builder.Bounds,
                Is.EqualTo(new Vector2(
                    MathF.Max(
                        x_advance + MathF.Max(m_x_advance + m_kerning, 0),
                        x_advance + MathF.Max(b_x_advance + b_kerning, 0)),
                    m_height + b_height)
                )
            );
        }

        /// <summary>
        /// Tests that the first added character on a new line is correctly marked as being on a new line.
        /// </summary>
        [Test]
        public void TestFirstCharacterOnNewLineIsOnNewLine()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("a");
            builder.AddNewLine();
            builder.AddText("a");

            Assert.That(builder.Characters[1].OnNewLine, Is.True);
        }

        /// <summary>
        /// Tests that no kerning is added for the first character of a new line.
        /// </summary>
        [Test]
        public void TestFirstCharacterOnNewLineHasNoKerning()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("a");
            builder.AddNewLine();
            builder.AddText("a");

            Assert.That(builder.Characters[1].DrawRectangle.Left, Is.EqualTo(x_offset));
        }

        /// <summary>
        /// Tests that a character with a lower baseline moves the previous character down to align with the new character.
        /// </summary>
        [Test]
        public void TestCharacterWithLowerBaselineOffsetsPreviousCharacters()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("a");

            Assert.That(builder.LineBaseHeight, Is.EqualTo(baseline));

            builder.AddText("b");

            Assert.That(builder.LineBaseHeight, Is.EqualTo(b_baseline));
            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset + (b_baseline - baseline)));

            builder.AddText("m");

            Assert.That(builder.LineBaseHeight, Is.EqualTo(m_baseline));
            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset + (m_baseline - baseline)));
            Assert.That(builder.Characters[1].DrawRectangle.Top, Is.EqualTo(b_y_offset + (m_baseline - b_baseline)));
        }

        /// <summary>
        /// Tests that a character with a higher (lesser in value) baseline gets moved down to align with the previous characters.
        /// </summary>
        [Test]
        public void TestCharacterWithHigherBaselineGetsOffset()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("m");

            Assert.That(builder.LineBaseHeight, Is.EqualTo(m_baseline));

            builder.AddText("b");

            Assert.That(builder.LineBaseHeight, Is.EqualTo(m_baseline));
            Assert.That(builder.Characters[1].DrawRectangle.Top, Is.EqualTo(b_y_offset + (m_baseline - b_baseline)));

            builder.AddText("a");

            Assert.That(builder.LineBaseHeight, Is.EqualTo(m_baseline));
            Assert.That(builder.Characters[1].DrawRectangle.Top, Is.EqualTo(b_y_offset + (m_baseline - b_baseline)));
            Assert.That(builder.Characters[2].DrawRectangle.Top, Is.EqualTo(y_offset + (m_baseline - baseline)));
        }

        /// <summary>
        /// Tests that baseline alignment adjustments only affect the line the new character was placed on.
        /// </summary>
        [Test]
        public void TestBaselineAdjustmentAffectsRelevantLineOnly()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("a");
            builder.AddNewLine();
            builder.AddText("ab");
            builder.AddNewLine();

            builder.AddText("ab");

            assertOtherCharactersNotAffected();
            Assert.That(builder.Characters[3].DrawRectangle.Top, Is.EqualTo(font_size * 2 + y_offset + (b_baseline - baseline)));
            Assert.That(builder.Characters[4].DrawRectangle.Top, Is.EqualTo(font_size * 2 + b_y_offset));

            builder.AddText("m");

            assertOtherCharactersNotAffected();
            Assert.That(builder.Characters[3].DrawRectangle.Top, Is.EqualTo(font_size * 2 + y_offset + (m_baseline - baseline)));
            Assert.That(builder.Characters[4].DrawRectangle.Top, Is.EqualTo(font_size * 2 + b_y_offset + (b_baseline - baseline)));
            Assert.That(builder.Characters[5].DrawRectangle.Top, Is.EqualTo(font_size * 2 + m_y_offset));

            void assertOtherCharactersNotAffected()
            {
                Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset));
                Assert.That(builder.Characters[1].DrawRectangle.Top, Is.EqualTo(font_size + y_offset + (b_baseline - baseline)));
                Assert.That(builder.Characters[2].DrawRectangle.Top, Is.EqualTo(font_size + b_y_offset));
            }
        }

        /// <summary>
        /// Tests that baseline alignment adjustments are done correctly and only affect the line the new character was placed on.
        /// </summary>
        [Test]
        public void TestBaselineAdjustmentWhenNotUsingFontSizeAsHeight()
        {
            var builder = new TextBuilder(fontStore, normal_font, useFontSizeAsHeight: false);

            // test baseline adjustment on the first line
            builder.AddText("a");
            builder.AddText("p");
            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(p_baseline - baseline));

            // test baseline adjustment affects relevant line only
            builder.AddNewLine();
            builder.AddText("a");
            builder.AddText("q");
            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(p_baseline - baseline));
            Assert.That(builder.Characters[2].DrawRectangle.Top, Is.EqualTo(p_height + (q_baseline - baseline)));
        }

        /// <summary>
        /// Tests that accessing <see cref="TextBuilder.LineBaseHeight"/> while the builder has multiline text throws.
        /// </summary>
        [Test]
        public void TestLineBaseHeightThrowsOnMultiline()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("a");
            builder.AddNewLine();
            builder.AddText("b");

            Assert.Throws<InvalidOperationException>(() => _ = builder.LineBaseHeight);
        }

        /// <summary>
        /// Tests that the current position and "line base height" are correctly reset when the first character is removed.
        /// </summary>
        [Test]
        public void TestRemoveFirstCharacterResetsCurrentPositionAndLineBaseHeight()
        {
            var builder = new TextBuilder(fontStore, normal_font, spacing: spacing);

            builder.AddText("a");
            builder.RemoveLastCharacter();

            Assert.That(builder.LineBaseHeight, Is.EqualTo(0f));
            Assert.That(builder.Bounds, Is.EqualTo(Vector2.Zero));

            builder.AddText("a");

            Assert.That(builder.LineBaseHeight, Is.EqualTo(baseline));
            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset));
            Assert.That(builder.Characters[0].DrawRectangle.Left, Is.EqualTo(x_offset));
        }

        /// <summary>
        /// Tests that the current position is moved backwards and the character is removed when a character is removed.
        /// </summary>
        [Test]
        public void TestRemoveCharacterOnSameLineRemovesCharacter()
        {
            var builder = new TextBuilder(fontStore, normal_font, spacing: spacing);

            builder.AddText("a");
            builder.AddText("a");
            builder.RemoveLastCharacter();

            Assert.That(builder.Bounds, Is.EqualTo(new Vector2(x_advance, font_size)));

            builder.AddText("a");

            Assert.That(builder.Characters[1].DrawRectangle.Top, Is.EqualTo(y_offset));
            Assert.That(builder.Characters[1].DrawRectangle.Left, Is.EqualTo(x_advance + spacing.X + kerning + x_offset));
        }

        /// <summary>
        /// Tests that the current position is moved to the end of the previous line, and that the character + new line is removed when a character is removed.
        /// </summary>
        [Test]
        public void TestRemoveCharacterOnNewLineRemovesCharacterAndLine()
        {
            var builder = new TextBuilder(fontStore, normal_font, spacing: spacing);

            builder.AddText("a");
            builder.AddNewLine();
            builder.AddText("a");
            builder.RemoveLastCharacter();

            Assert.That(builder.Bounds, Is.EqualTo(new Vector2(x_advance, font_size)));

            builder.AddText("a");

            Assert.That(builder.Characters[1].DrawRectangle.TopLeft, Is.EqualTo(new Vector2(x_advance + spacing.X + kerning + x_offset, y_offset)));
            Assert.That(builder.Bounds, Is.EqualTo(new Vector2(x_advance + spacing.X + kerning + x_advance, font_size)));
        }

        /// <summary>
        /// Tests that removing a character adjusts the baseline of the relevant line.
        /// </summary>
        [Test]
        public void TestRemoveCharacterAdjustsCharactersBaselineOfRelevantLine()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("a");
            builder.AddNewLine();
            builder.AddText("ab");
            builder.AddNewLine();
            builder.AddText("abm");

            builder.RemoveLastCharacter();

            assertOtherCharactersNotAffected();
            Assert.That(builder.Characters[3].DrawRectangle.Top, Is.EqualTo(font_size * 2 + y_offset + (b_baseline - baseline)));
            Assert.That(builder.Characters[4].DrawRectangle.Top, Is.EqualTo(font_size * 2 + b_y_offset));

            builder.RemoveLastCharacter();

            assertOtherCharactersNotAffected();
            Assert.That(builder.Characters[3].DrawRectangle.Top, Is.EqualTo(font_size * 2 + y_offset));

            void assertOtherCharactersNotAffected()
            {
                Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset));
                Assert.That(builder.Characters[1].DrawRectangle.Top, Is.EqualTo(font_size + y_offset + (b_baseline - baseline)));
                Assert.That(builder.Characters[2].DrawRectangle.Top, Is.EqualTo(font_size + b_y_offset));
            }
        }

        /// <summary>
        /// Tests that removing a character behaves correctly.
        /// </summary>
        [Test]
        public void TestRemoveCharacterWhenNotUsingFontSizeAsHeight()
        {
            var builder = new TextBuilder(fontStore, normal_font, useFontSizeAsHeight: false);

            builder.AddText("ap");
            builder.AddNewLine();
            builder.AddText("aq");

            Assert.That(builder.Characters[2].DrawRectangle.Top, Is.EqualTo(p_height + (q_baseline - baseline)));
            Assert.That(builder.Bounds,
                Is.EqualTo(
                    new Vector2(
                        MathF.Max(
                            x_advance + MathF.Max(p_x_advance + p_kerning, 0),
                            x_advance + MathF.Max(q_x_advance + q_kerning, 0)),
                        p_height + q_height)
                )
            );

            // tests that removing a character resets bounds, and the baseline of the relevant line correctly
            builder.RemoveLastCharacter();

            Assert.That(builder.Characters[2].DrawRectangle.Top, Is.EqualTo(p_height));
            Assert.That(builder.Bounds,
                Is.EqualTo(
                    new Vector2(
                        x_advance + MathF.Max(p_x_advance + p_kerning, 0),
                        p_height + height)
                )
            );

            // tests that removing a character on a new line resets the current line to the previous line correctly (tests baseline and bounds after adding a new character)
            builder.RemoveLastCharacter();
            builder.AddText("a");
            Assert.That(builder.Characters[2].DrawRectangle.Top, Is.EqualTo(p_baseline - baseline));
            Assert.That(builder.Bounds,
                Is.EqualTo(
                    new Vector2(
                        x_advance + MathF.Max(p_x_advance + p_kerning, 0) + MathF.Max(x_advance + kerning, 0),
                        p_height)
                )
            );
        }

        /// <summary>
        /// Tests that removing a character from a text that still has another character with the same baseline doesn't affect the alignment.
        /// </summary>
        [Test]
        public void TestRemoveSameBaselineCharacterDoesntAffectAlignment()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("abb");

            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset + (b_baseline - baseline)));

            builder.RemoveLastCharacter();

            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset + (b_baseline - baseline)));
        }

        /// <summary>
        /// Tests that removing the first character of a line doesn't affect the baseline alignment of the line above it.
        /// </summary>
        [Test]
        public void TestRemoveFirstCharacterOnNewlineDoesntAffectLastLineAlignment()
        {
            var builder = new TextBuilder(fontStore, normal_font);

            builder.AddText("ab");
            builder.AddNewLine();
            builder.AddText("m");
            builder.RemoveLastCharacter();

            Assert.That(builder.Characters[0].DrawRectangle.Top, Is.EqualTo(y_offset + (b_baseline - baseline)));
            Assert.That(builder.Characters[1].DrawRectangle.Top, Is.EqualTo(b_y_offset));
        }

        /// <summary>
        /// Tests that the custom user-provided spacing is added for a new character/line.
        /// </summary>
        [Test]
        public void TestSpacingAdded()
        {
            var builder = new TextBuilder(fontStore, normal_font, spacing: spacing);

            builder.AddText("a");
            builder.AddText("a");
            builder.AddNewLine();
            builder.AddText("a");

            Assert.That(builder.Characters[0].DrawRectangle.Left, Is.EqualTo(x_offset));
            Assert.That(builder.Characters[1].DrawRectangle.Left, Is.EqualTo(x_advance + spacing.X + kerning + x_offset));
            Assert.That(builder.Characters[2].DrawRectangle.Left, Is.EqualTo(x_offset));
            Assert.That(builder.Characters[2].DrawRectangle.Top, Is.EqualTo(font_size + spacing.Y + y_offset));
        }

        /// <summary>
        /// Tests that glyph lookup falls back to using the same character with no font name.
        /// </summary>
        [Test]
        public void TestSameCharacterFallsBackWithNoFontName()
        {
            var font = new TestFontUsage("test");
            var nullFont = new TestFontUsage(null);
            var builder = new TextBuilder(new TestStore(
                new GlyphEntry(font, new TestGlyph('b', 0, 0, 0, 0, 0, 0, 0)),
                new GlyphEntry(nullFont, new TestGlyph('a', 0, 0, 0, 0, 0, 0, 0)),
                new GlyphEntry(font, new TestGlyph('?', 0, 0, 0, 0, 0, 0, 0)),
                new GlyphEntry(nullFont, new TestGlyph('?', 0, 0, 0, 0, 0, 0, 0))
            ), font);

            builder.AddText("a");

            Assert.That(builder.Characters[0].Character, Is.EqualTo('a'));
        }

        /// <summary>
        /// Tests that glyph lookup falls back to using the fallback character with the provided font name.
        /// </summary>
        [Test]
        public void TestFallBackCharacterFallsBackWithFontName()
        {
            var font = new TestFontUsage("test");
            var nullFont = new TestFontUsage(null);
            var builder = new TextBuilder(new TestStore(
                new GlyphEntry(font, new TestGlyph('b', 0, 0, 0, 0, 0, 0, 0)),
                new GlyphEntry(nullFont, new TestGlyph('b', 0, 0, 0, 0, 0, 0, 0)),
                new GlyphEntry(font, new TestGlyph('?', 0, 0, 0, 0, 0, 0, 0)),
                new GlyphEntry(nullFont, new TestGlyph('?', 1, 0, 0, 0, 0, 0, 0))
            ), font);

            builder.AddText("a");

            Assert.That(builder.Characters[0].Character, Is.EqualTo('?'));
            Assert.That(builder.Characters[0].XOffset, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that glyph lookup falls back to using the fallback character with no font name.
        /// </summary>
        [Test]
        public void TestFallBackCharacterFallsBackWithNoFontName()
        {
            var font = new TestFontUsage("test");
            var nullFont = new TestFontUsage(null);
            var builder = new TextBuilder(new TestStore(
                new GlyphEntry(font, new TestGlyph('b', 0, 0, 0, 0, 0, 0, 0)),
                new GlyphEntry(nullFont, new TestGlyph('b', 0, 0, 0, 0, 0, 0, 0)),
                new GlyphEntry(font, new TestGlyph('b', 0, 0, 0, 0, 0, 0, 0)),
                new GlyphEntry(nullFont, new TestGlyph('?', 1, 0, 0, 0, 0, 0, 0))
            ), font);

            builder.AddText("a");

            Assert.That(builder.Characters[0].Character, Is.EqualTo('?'));
            Assert.That(builder.Characters[0].XOffset, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that a null glyph is correctly handled.
        /// </summary>
        [Test]
        public void TestFailedCharacterLookup()
        {
            var font = new TestFontUsage("test");
            var builder = new TextBuilder(new TestStore(), font);

            builder.AddText("a");

            Assert.That(builder.Bounds, Is.EqualTo(Vector2.Zero));
        }

        private readonly struct TestFontUsage
        {
            private readonly string family;
            private readonly string weight;
            private readonly bool italics;
            private readonly bool fixedWidth;

            public TestFontUsage(string family = null, string weight = null, bool italics = false, bool fixedWidth = false)
            {
                this.family = family;
                this.weight = weight;
                this.italics = italics;
                this.fixedWidth = fixedWidth;
            }

            public static implicit operator FontUsage(TestFontUsage tfu)
                => new FontUsage(tfu.family, font_size, tfu.weight, tfu.italics, tfu.fixedWidth);
        }

        private class TestStore : ITexturedGlyphLookupStore
        {
            private readonly GlyphEntry[] glyphs;

            public TestStore(params GlyphEntry[] glyphs)
            {
                this.glyphs = glyphs;
            }

            public ITexturedCharacterGlyph Get(string fontName, char character)
            {
                if (string.IsNullOrEmpty(fontName))
                    return glyphs.FirstOrDefault(g => g.Glyph.Character == character).Glyph;

                return glyphs.FirstOrDefault(g => g.Font.FontName == fontName && g.Glyph.Character == character).Glyph;
            }

            public Task<ITexturedCharacterGlyph> GetAsync(string fontName, char character) => throw new NotImplementedException();
        }

        private readonly struct GlyphEntry
        {
            public readonly FontUsage Font;
            public readonly ITexturedCharacterGlyph Glyph;

            public GlyphEntry(FontUsage font, ITexturedCharacterGlyph glyph)
            {
                Font = font;
                Glyph = glyph;
            }
        }

        private readonly struct TestGlyph : ITexturedCharacterGlyph
        {
            public Texture Texture => new DummyRenderer().CreateTexture(1, 1);
            public float XOffset { get; }
            public float YOffset { get; }
            public float XAdvance { get; }
            public float Width { get; }
            public float Baseline { get; }
            public float Height { get; }
            public char Character { get; }

            private readonly float glyphKerning;

            public TestGlyph(char character, float xOffset, float yOffset, float xAdvance, float width, float baseline, float height, float kerning)
            {
                glyphKerning = kerning;
                Character = character;
                XOffset = xOffset;
                YOffset = yOffset;
                XAdvance = xAdvance;
                Width = width;
                Baseline = baseline;
                Height = height;
            }

            public float GetKerning<T>(T lastGlyph)
                where T : ICharacterGlyph
                => glyphKerning;
        }
    }
}
