using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for Images
    /// </summary>
    public class Images : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string path = context.Request.Params["Path"];

            path = Path.Combine(
                context.Request.PhysicalApplicationPath,
                path.Replace("/", "\\")
            );

            if (!File.Exists(path))
            {
                context.Response.BinaryWrite(new byte[0]);
                return;
            }

            context.Response.ContentType = "image/" + path.Split('.')[1];

            if (context.Request.Params["Width"] != null || context.Request.Params["Height"] != null)
            {
                int width = int.Parse(context.Request.Params["Width"]);
                int height = int.Parse(context.Request.Params["Height"]);

                Image image = Image.FromFile(path);

                image = RezizeImage(image, width, height);

                MemoryStream stream = new MemoryStream();

                image.Save(stream, ImageFormat.Png);

                context.Response.ContentType = "image/png";
                context.Response.BinaryWrite(stream.ToArray());
                return;
            }

            context.Response.BinaryWrite(File.ReadAllBytes(path));
        }

        private Image RezizeImage(Image img, int maxWidth, int maxHeight)
        {
            if (img.Height < maxHeight && img.Width < maxWidth) return img;
            using (img)
            {
                Double xRatio = (double)img.Width / maxWidth;
                Double yRatio = (double)img.Height / maxHeight;
                Double ratio = Math.Max(xRatio, yRatio);
                int nnx = (int)Math.Floor(img.Width / ratio);
                int nny = (int)Math.Floor(img.Height / ratio);
                Bitmap cpy = new Bitmap(nnx, nny, PixelFormat.Format32bppArgb);
                using (Graphics gr = Graphics.FromImage(cpy))
                {
                    gr.Clear(Color.Transparent);

                    // This is said to give best quality when resizing images
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    gr.DrawImage(img,
                        new Rectangle(0, 0, nnx, nny),
                        new Rectangle(0, 0, img.Width, img.Height),
                        GraphicsUnit.Pixel);
                }
                return cpy;
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}