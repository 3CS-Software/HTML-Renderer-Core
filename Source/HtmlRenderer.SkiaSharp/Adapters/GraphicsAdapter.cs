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
using TheArtOfDev.HtmlRenderer.Core.Utils;
using TheArtOfDev.HtmlRenderer.SkiaSharp.Utilities;

namespace TheArtOfDev.HtmlRenderer.SkiaSharp.Adapters
{
    /// <summary>
    /// Adapter for WinForms Graphics for core.
    /// </summary>
    internal sealed class GraphicsAdapter : RGraphics
    {
        #region Fields and Consts

        /// <summary>
        /// The wrapped WinForms graphics object
        /// </summary>
        private readonly SKCanvas _canvas;
        private readonly SKBitmap _bitmap;

        /// <summary>
        /// if to release the graphics object on dispose
        /// </summary>
        private readonly bool _releaseGraphics;

        /// <summary>
        /// Used to measure and draw strings
        /// </summary>
        private static readonly SKPaint _stringFormat;

        #endregion


        static GraphicsAdapter()
        {
            _stringFormat = new SKPaint();
            _stringFormat.TextAlign = SKTextAlign.Left;
        }

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="g">the win forms graphics object to use</param>
        /// <param name="releaseGraphics">optional: if to release the graphics object on dispose (default - false)</param>
        public GraphicsAdapter(SKCanvas g, SKBitmap bitmap, bool releaseGraphics = false)
            : base(SkiaSharpAdapter.Instance, new RRect(0, 0, double.MaxValue, double.MaxValue))
        {
            ArgChecker.AssertArgNotNull(g, "g");
            
            _canvas = g;
            _bitmap = bitmap;

            _releaseGraphics = releaseGraphics;
        }

        public override void PopClip()
        {
            _clipStack.Pop();
            _canvas.Restore();
        }

        public override void PushClip(RRect rect)
        {
            _clipStack.Push(rect);
            _canvas.Save();
            _canvas.ClipRect(Utils.Convert(rect));
        }

        public override void PushClipExclude(RRect rect)
        { }

        bool _antiAlias = true;
        public override Object SetAntiAliasSmoothingMode()
        {
            var prevMode = _antiAlias;
            _antiAlias = true;
            return prevMode;
            //var prevMode = _g.SmoothingMode;
            //_g.SmoothingMode = XSmoothingMode.AntiAlias;
            //return prevMode;
        }

        public override void ReturnPreviousSmoothingMode(Object prevMode)
        {
            if (prevMode != null)
            {
                _antiAlias = (bool)prevMode;
            }
        }

        public override RSize MeasureString(string str, RFont font)
        {
            var fontAdapter = (FontAdapter)font;
            var realFont = fontAdapter.Font;

            var sf = new SKPaint(realFont);
            
            //TODO: need to restrict this skpaint to the width of the canvas.
            var boundingRect = new SKRect();

            var width = sf.MeasureText(str, ref boundingRect);
            var height = -realFont.Metrics.Top + realFont.Metrics.Descent;
            //var size = _g.MeasureString(str, realFont, _stringFormat);

            //The returned width includes whitespace, which we want.  The bounding rect will exclude white space.
            var size = new SKSize(width, height);

            if (font.Height < 0)
            {
                fontAdapter.SetMetrics((int)Math.Round(height, MidpointRounding.AwayFromZero), (int)Math.Round((height - realFont.Metrics.Descent + 1f), MidpointRounding.AwayFromZero));
            }

            return Utils.Convert(size);
        }

        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
        {
            // there is no need for it - used for text selection
            throw new NotSupportedException();
        }

        public override void DrawString(string str, RFont font, RColor color, RPoint point, RSize size, bool rtl)
        {
            var skiaBrush = ((BrushAdapter)_adapter.GetSolidBrush(color)).Brush as SKPaint;
            var skiaFont = ((FontAdapter)font).Font;
            var skiaPoint = Utils.Convert(point);
            //TODO: use the size.
            var skiaSize = Utils.Convert(size);

            _canvas.DrawText(str, skiaPoint.X, skiaPoint.Y -skiaFont.Metrics.Ascent, skiaFont, skiaBrush);
        }

        public override RBrush GetTextureBrush(RImage image, RRect dstRect, RPoint translateTransformLocation)
        {
            return new BrushAdapter(new TextureBrush(((ImageAdapter)image).Image, Utils.Convert(dstRect), Utils.Convert(translateTransformLocation)));
        }

        public override RGraphicsPath GetGraphicsPath()
        {
            return new GraphicsPathAdapter();
        }

        public override void Dispose()
        {
            if (_releaseGraphics)
                _canvas.Dispose();
        }


        #region Delegate graphics methods

        public override void DrawLine(RPen pen, double x1, double y1, double x2, double y2)
        {
            _canvas.DrawLine((float)x1, (float)y1, (float)x2, (float)y2, ((PenAdapter)pen).Pen);
        }

        public override void DrawRectangle(RPen pen, double x, double y, double width, double height)
        {
            _canvas.DrawRect((float)x, (float)y, (float)width, (float)height, ((PenAdapter)pen).Pen);
        }

        public override void DrawRectangle(RBrush brush, double x, double y, double width, double height)
        {
            var xBrush = ((BrushAdapter)brush).Brush;
            var xTextureBrush = xBrush as TextureBrush;
            if (xTextureBrush != null)
            {
                xTextureBrush.DrawRectangle(_canvas, x, y, width, height);
            }
            else
            {
                _canvas.DrawRect((float)x, (float)y, (float)width, (float)height, (SKPaint)xBrush);
            }
        }

        public override void DrawImage(RImage image, RRect destRect, RRect srcRect)
        {
            var adapter = (ImageAdapter)image;
            if (adapter.Picture != null)
            {
                //TODO: the resizing part..
                _canvas.DrawPicture(adapter.Picture, Utils.Convert(destRect).Location);
            }
            else
            {
                _canvas.DrawImage(adapter.Image, Utils.Convert(destRect), Utils.Convert(srcRect));
            }
            //_canvas.DrawImage(((ImageAdapter)image).Image, Utils.Convert(destRect), Utils.Convert(srcRect), XGraphicsUnit.Point);
        }

        public override void DrawImage(RImage image, RRect destRect)
        {
            var adapter = (ImageAdapter)image;
            if (adapter.Picture != null)
            {
                //TODO: the resizing part..
                _canvas.DrawPicture(adapter.Picture, Utils.Convert(destRect).Location);
            }
            else
            {
                _canvas.DrawImage(((ImageAdapter)image).Image, Utils.Convert(destRect));
            }

        }

        public override void DrawPath(RPen pen, RGraphicsPath path)
        {
            _canvas.DrawPath(((GraphicsPathAdapter)path).GraphicsPath, ((PenAdapter)pen).Pen);
        }

        public override void DrawPath(RBrush brush, RGraphicsPath path)
        {
            var paint = ((BrushAdapter)brush).Brush as SKPaint;

            if (paint == null)
            {
                //Could be a textured path.  We don't support it right now :/
                return;
            }

            _canvas.DrawPath(((GraphicsPathAdapter)path).GraphicsPath, paint);
        }

        public override void DrawPolygon(RBrush brush, RPoint[] points)
        {
            var paint = ((BrushAdapter)brush).Brush as SKPaint;

            if (paint == null)
            {
                //Could be a textured path.  We don't support it right now :/
                return;
            }

            if (points != null && points.Length > 0)
            {
                var path = new SKPath();
                var skPoints = points.Select(n => Utils.Convert(n)).ToArray();
                path.AddPoly(skPoints);
                _canvas.DrawPath(path, paint);
            }
        }

        #endregion
    }
}