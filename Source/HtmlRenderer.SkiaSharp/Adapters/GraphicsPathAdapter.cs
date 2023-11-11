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
using System;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace TheArtOfDev.HtmlRenderer.SkiaSharp.Adapters
{
    /// <summary>
    /// Adapter for WinForms graphics path object for core.
    /// </summary>
    internal sealed class GraphicsPathAdapter : RGraphicsPath
    {
        /// <summary>
        /// The actual PdfSharp graphics path instance.
        /// </summary>
        private readonly SKPath _graphicsPath = new SKPath();

        /// <summary>
        /// The actual PdfSharp graphics path instance.
        /// </summary>
        public SKPath GraphicsPath
        {
            get { return _graphicsPath; }
        }

        public override void Start(double x, double y)
        {
            _graphicsPath.MoveTo((float)x, (float)y);
        }

        public override void LineTo(double x, double y)
        {
            _graphicsPath.LineTo((float)x, (float)y);
        }

        public override void ArcTo(double x, double y, double size, Corner corner)
        {

            switch (corner)
            {
                case Corner.TopLeft:
                    break;
                case Corner.TopRight:
                    break;
                case Corner.BottomLeft:
                    break;
                case Corner.BottomRight:
                    break;
                default:
                    break;
            }

            float left = (float)(Math.Min(x, _graphicsPath.LastPoint.X) - (corner == Corner.TopRight || corner == Corner.BottomRight ? size : 0));
            float top = (float)(Math.Min(y, _graphicsPath.LastPoint.Y) - (corner == Corner.BottomLeft || corner == Corner.BottomRight ? size : 0));

            _graphicsPath.AddArc(new SKRect(left, top, left + ((float)size * 2), top + ((float)size * 2)), GetStartAngle(corner), 90);
            _graphicsPath.MoveTo((float)x, (float)y);
        }

        public override void Dispose()
        { }

        /// <summary>
        /// Get arc start angle for the given corner.
        /// </summary>
        private static int GetStartAngle(Corner corner)
        {
            int startAngle;
            switch (corner)
            {
                case Corner.TopLeft:
                    startAngle = 180;
                    break;
                case Corner.TopRight:
                    startAngle = 270;
                    break;
                case Corner.BottomLeft:
                    startAngle = 90;
                    break;
                case Corner.BottomRight:
                    startAngle = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("corner");
            }
            return startAngle;
        }
    }
}