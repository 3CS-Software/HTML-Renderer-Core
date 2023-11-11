// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using SkiaSharp;
using System.Drawing;

namespace TheArtOfDev.HtmlRenderer.SkiaSharp
{
    public enum PageSizeType
    {
        A4,
        Letter
    }

    /// <summary>
    /// The settings for generating PDF using <see cref="PdfGenerator"/>
    /// </summary>
    public sealed class PdfGenerateConfig
    {
        /// <summary>
        /// the page size to use for each page in the generated pdf
        /// </summary>
        public PageSizeType? PageSize { get; set; }

        /// <summary>
        /// if the page size is undefined this allow you to set manually the page size
        /// </summary>
        public SizeF ManualPageSize { get; set; }

        /// <summary>
        /// TODO: use an enum for portrait/landscape.
        /// </summary>
        public bool IsLandscape { get; set; } = false;

        /// <summary>
        /// the top margin in pixels between the page start and the text.
        /// </summary>
        public int MarginTop { get; set; }

        /// <summary>
        /// the bottom margin in pixels between the page end and the text
        /// </summary>
        public int MarginBottom { get; set; }

        /// <summary>
        /// the left margin in pixels between the page start and the text
        /// </summary>
        public int MarginLeft { get; set; }

        /// <summary>
        /// the right margin in pixels between the page end and the text
        /// </summary>
        public int MarginRight { get; set; }

        /// <summary>
        /// Set all 4 margins to the given value.
        /// </summary>
        /// <param name="value"></param>
        public void SetMargins(int value)
        {
            if (value > -1)
                MarginBottom = MarginLeft = MarginTop = MarginRight = value;
        }

        // The international definitions are:
        //   1 inch == 25.4 mm
        //   1 inch == 72 point

        /// <summary>
        /// Convert the units passed in milimiters to the units used in PdfSharp
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static SKSize MilimitersToPixels(float width, float height)
        {
            return new SKSize(width * 3.779527559f, height * 3.779527559f);
        }

        //TODO: single util.
        public static int MilimitersToPixels(float mm)
            => (int)(mm * 3.779527559f * 72 / 96);
    }
}