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
using System.Drawing.Text;
using System.IO;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.SkiaSharp.Utilities;

namespace TheArtOfDev.HtmlRenderer.SkiaSharp.Adapters
{
    /// <summary>
    /// Adapter for SkiaSharp library platform.
    /// </summary>
    internal sealed class SkiaSharpAdapter : RAdapter
    {
        #region Fields and Consts

        /// <summary>
        /// Singleton instance of global adapter.
        /// </summary>
        private static readonly SkiaSharpAdapter _instance = new SkiaSharpAdapter();

        #endregion


        /// <summary>
        /// Init color resolve.
        /// </summary>
        private SkiaSharpAdapter()
        {
            AddFontFamilyMapping("monospace", "Courier New");
            AddFontFamilyMapping("Helvetica", "Arial");

           /* var families = new InstalledFontCollection();

            foreach (var family in families.Families)
            {
                AddFontFamily(new FontFamilyAdapter(new SKFontFamily(family.Name)));
            }*/
        }

        /// <summary>
        /// Singleton instance of global adapter.
        /// </summary>
        public static SkiaSharpAdapter Instance
        {
            get { return _instance; }
        }

        protected override RColor GetColorInt(string colorName)
        {
            try
            {
                var color = Color.FromKnownColor((KnownColor)System.Enum.Parse(typeof(KnownColor), colorName, true));
                return Utils.Convert(color);
            }
            catch
            {
                return RColor.Empty;
            }
        }

        protected override RPen CreatePen(RColor color)
        {
            return new PenAdapter(new SKPaint { Color = Utils.Convert(color) });
        }

        protected override RBrush CreateSolidBrush(RColor color)
        {
            SKPaint solidBrush;
            if (color == RColor.White)
                solidBrush = new SKPaint { Color = SKColor.Parse("FFFFFFFF") };// XBrushes.White;
            else if (color == RColor.Black)
                solidBrush = new SKPaint { Color = SKColor.Parse("FF000000") };// XBrushes.White; // XBrushes.Black;
            else if (color.A < 1)
                solidBrush = new SKPaint { Color = SKColor.Parse("00000000") };// XBrushes.Transparent;
            else
                solidBrush = new SKPaint { Color = Utils.Convert(color) };

            return new BrushAdapter(solidBrush);
        }

        protected override RBrush CreateLinearGradientBrush(RRect rect, RColor color1, RColor color2, double angle)
        {
            //XLinearGradientMode mode;
            //if (angle < 45)
            //    mode = XLinearGradientMode.ForwardDiagonal;
            //else if (angle < 90)
            //    mode = XLinearGradientMode.Vertical;
            //else if (angle < 135)
            //    mode = XLinearGradientMode.BackwardDiagonal;
            //else
            //    mode = XLinearGradientMode.Horizontal;
            //return new BrushAdapter(new XLinearGradientBrush(Utils.Convert(rect), Utils.Convert(color1), Utils.Convert(color2), mode));

            var skRect = Utils.Convert(rect);
            var start = skRect.Location;
            var end = new SKPoint(skRect.Right, skRect.Bottom);
            var paint = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(start, end, new[] { Utils.Convert(color1), Utils.Convert(color2) }, SKShaderTileMode.Clamp)
            };

            return new BrushAdapter(paint);
        }

        protected override RImage ConvertImageInt(object image)
        {
            return image != null ? new ImageAdapter((SKImage)image) : null;
        }

        protected override RImage ImageFromStreamInt(Stream memoryStream)
        {
            var image = SKImage.FromEncodedData(memoryStream);
            if (image == null)
            {
                //Maybe an SVG?  probably a better way than doing this, as we've basically failed to load into SKImage first.
                //Perhaps the html renderer can be changed to pass in the content-type.  TODO: wwmd? (what would mozilla do?)
                memoryStream.Seek(0, SeekOrigin.Begin);
                var skSvg = new global::SkiaSharp.Extended.Svg.SKSvg();
                var picture = skSvg.Load(memoryStream);
                //using (var ms = new MemoryStream())
                //{
                //    //skSvg.Picture.ToImage(ms, SKColors.Empty, SKEncodedImageFormat.Png, 100, 1f, 1f, SKColorType.Rgba8888, SKAlphaType.Unknown, SKColorSpace.CreateSrgb());

                //    ms.Seek(0, SeekOrigin.Begin);
                //    image = SKImage.FromEncodedData(ms);
                //}
                image = SKImage.FromBitmap(new SKBitmap((int)skSvg.CanvasSize.Width, (int)skSvg.CanvasSize.Height));
                return new ImageAdapter(image, picture);
            }
            else
            {
                return new ImageAdapter(image);
            }
        }

        protected override RFont CreateFontInt(string family, double size, RFontStyle style)
        {
            SKFontStyleWeight weight = SKFontStyleWeight.Normal;
            SKFontStyleWidth width = SKFontStyleWidth.Normal;
            SKFontStyleSlant slant = SKFontStyleSlant.Upright;

            if (style.HasFlag(RFontStyle.Bold))
            {
                weight = SKFontStyleWeight.Bold;
            }
            if (style.HasFlag(RFontStyle.Italic))
            {
                slant = SKFontStyleSlant.Italic;
            }
            if (style.HasFlag(RFontStyle.Strikeout))
            {

            }
            if (style.HasFlag(RFontStyle.Underline))
            {

            }

            var typeFace = SKTypeface.FromFamilyName(family, weight, width, slant);
            //convert from points to pixels.
            var font = new SKFont(typeFace, (float)size);
            return new FontAdapter(font);
        }

        protected override RFont CreateFontInt(RFontFamily family, double size, RFontStyle style)
        {
            return CreateFontInt(family.Name, size, style);
        }
    }
}