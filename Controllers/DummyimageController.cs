using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DummyimageController : Controller
    {
        [HttpGet("{text}")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string text) {
            Image img = await ImageUtils.GetImage(text);
            MemoryStream ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);
            var imgBytes = ms.ToArray();
            img.Dispose();
            ms.Dispose();
            return File(imgBytes, "image/png");
        }
    }

    public class ImageRequest
    {
        public string ImageText { get; set; }
    }
    public static class ImageUtils
    {
        static Font _font = new Font(
            new FontFamily($"Roboto, Arial, Liberation Sans, Verdana, Bitstream Vera Sans, {FontFamily.GenericSansSerif.Name}"),
            300,
            FontStyle.Regular,
            GraphicsUnit.Pixel);
        public static async Task<Image> GetImage(string text)
        {
            Image txtImage = await DrawText(text, _font, Color.Cyan, Color.Black);
            Image img = new Bitmap(418, 150);
            Graphics drawing = Graphics.FromImage(img);
            Brush fillBrush = new SolidBrush(Color.Black);

            const decimal innerScale = (decimal) (418 - 10) / (decimal) (150 - 10);
            decimal txtScale = (decimal) txtImage.Width / (decimal) txtImage.Height;

            Rectangle r;
            if (innerScale > txtScale)
            {
                decimal scaleDown = (decimal) 130 / (decimal) txtImage.Height;
                decimal width = txtImage.Width * scaleDown;
                decimal xOffset = ((decimal)398 - width) / (decimal)2;
                r = new Rectangle(10+(int)xOffset, 10, (int)width, 130);
            }
            else
            {
                decimal scaleDown = (decimal) 398 / (decimal) txtImage.Width;
                decimal height = txtImage.Height * scaleDown;
                decimal yOffset = ((decimal) 130 - height) / (decimal) 2;
                r = new Rectangle(10, 10+(int)yOffset, 398, (int)height);
            }

            drawing.FillRegion(fillBrush, new Region(new Rectangle(0, 0, 418, 150)));
            drawing.DrawImage(txtImage, r);
            drawing.Save();
            
            drawing.Dispose();
            txtImage.Dispose();
            fillBrush.Dispose();

            return img;
        }

        private static async Task<Image> DrawText(String text, Font font, Color textColor, Color backColor)
        {
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            
            SizeF textSize = drawing.MeasureString(text, font);
            
            img.Dispose();
            drawing.Dispose();
            img = new Bitmap((int) textSize.Width == 0 ? 1 : (int)textSize.Width, (int)textSize.Height == 0 ? 1 : (int)textSize.Height);
        
            drawing = Graphics.FromImage(img);
        
            drawing.Clear(backColor);
        
            Brush textBrush = new SolidBrush(textColor);
        
            drawing.DrawString(text, font, textBrush, 0, 0);
        
            drawing.Save();
        
            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }
    }
}
