﻿// "Therefore those skilled at the unorthodox
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

using TheArtOfDev.HtmlRenderer.Adapters;
using SkiaSharp;

namespace TheArtOfDev.HtmlRenderer.SkiaSharp.Adapters
{
    /// <summary>
    /// Adapter for WinForms Image object for core.
    /// </summary>
    internal sealed class ImageAdapter : RImage
    {
        /// <summary>
        /// the underline win-forms image.
        /// </summary>
        private readonly SKImage _image;
        private readonly SKPicture? _picture;

        public ImageAdapter(SKImage image)
        {
            _image = image;
            _picture = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ImageAdapter(SKImage image, SKPicture picture)
        {
            _image = image;
            _picture = picture;
        }

        /// <summary>
        /// the underline win-forms image.
        /// </summary>
        public SKImage Image
        {
            get { return _image; }
        }

        public SKPicture? Picture
        {
            get { return _picture; }
        }

        public override double Width
        {
            get { return _image.Width; }
        }

        public override double Height
        {
            get { return _image.Height; }
        }

        public override void Dispose()
        {
            _image.Dispose();
        }
    }
}