using Radio7.Todo.Server;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Radio7.Todo.Tests
{
    public class ParserTests
    {
        [Theory,
            InlineData("Title ends on new line or sentence? More on the first line.\r\nBody.", "Title ends on new line or sentence?"),
            InlineData("Title ends on new line or sentence! More on the first line.\r\nBody.", "Title ends on new line or sentence!"),
            InlineData("Title ends on new line or sentence. More on the first line.\r\nBody.", "Title ends on new line or sentence."),
            InlineData("#Title cannot be markdown\r\nBody.", "#Title cannot be markdown"),
            InlineData("Title can end on newline\r\nbody.", "Title can end on newline"),
            InlineData("Title is fragment", "Title is fragment"),
            InlineData("Title", "Title")]
        public void Parser_ParsesTitleAsFirstSentenceOrLine(string value, string expected)
        {
            // arrange
            var sut = new Parser();

            // act
            var actual = sut.Parse(value);

            // assert
            Assert.Equal(expected, actual.Title);
        }

        [Theory,
            InlineData("This is a title. these are #tag1 and #tag2\r\nand #tag3 is here too.", new [] { "TAG1", "TAG2", "TAG3" })
            InlineData("This is a #titleTag in the title. these are #tag1 and #tag2\r\nand #tag3 is here too.", new[] { "TITLETAG", "TAG1", "TAG2", "TAG3" })
            InlineData("This is a #tag1 in the title. these are #tag1 and #tag1\r\nand #tag1 is here again. tag is only found once.", new[] { "TAG1" })]
        public void Parser_ParsesFindsTagsOnlyOnce(string value, IEnumerable<object> expected)
        {
            // arrange
            var sut = new Parser();

            // act
            var actual = sut.Parse(value);

            // assert
            foreach(var expectedTag in expected.Cast<string>())
            {
                // tag is found
                Assert.Contains(expectedTag, actual.Tags);
                // tag occurs once
                Assert.Equal(1, actual.Tags.Count(x => x == expectedTag));
            }
        }

        [Theory,
            InlineData("#tag1,#tag2.#tag3!,#tag4?#tag5 #tag6\r\n#tag7 and a #taginline in a sentence", new[] { "TAG1", "TAG2", "TAG3", "TAG4", "TAG5", "TAG6","TAG7", "TAGINLINE" })]
        public void Parser_ParsesFindsTagsSeperatedBy(string value, IEnumerable<object> expected)
        {
            // arrange
            var sut = new Parser();

            // act
            var actual = sut.Parse(value);

            // assert
            foreach (var expectedTag in expected.Cast<string>())
            {
                // tag is found
                Assert.Contains(expectedTag, actual.Tags);
                // tag occurs once
                Assert.Equal(1, actual.Tags.Count(x => x == expectedTag));
            }
        }
    }
}
