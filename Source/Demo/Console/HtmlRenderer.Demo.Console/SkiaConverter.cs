using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Demo.Common;
using TheArtOfDev.HtmlRenderer.SkiaSharp;

namespace HtmlRenderer.Demo.Console
{
    public class SkiaConverter : SampleConverterBase
    {
        public SkiaConverter(string sampleRunIdentifier, string basePath) : base(sampleRunIdentifier, basePath)
        {
        }

        public void GenerateSample(HtmlSample sample)
        {
            var config = new PdfGenerateConfig();

            config.PageSize = PageSizeType.A4;
            //TODO: 'Units' on config, rather than this c/p 🤮
            config.MarginLeft = PdfGenerateConfig.MilimitersToPixels(0);
            config.MarginRight = PdfGenerateConfig.MilimitersToPixels(0);
            config.MarginTop = PdfGenerateConfig.MilimitersToPixels(0);
            config.MarginBottom = PdfGenerateConfig.MilimitersToPixels(0);

            var pdf = PdfGenerator.GeneratePdf(sample.Html, config, base.CssData, base.StylesheetLoad, base.ImageLoad);
            using (var fileStream = File.Open(GetSamplePath(sample), FileMode.CreateNew))
            {
                pdf.CopyTo(fileStream);
                fileStream.Flush();
            }
        }
    }
}
