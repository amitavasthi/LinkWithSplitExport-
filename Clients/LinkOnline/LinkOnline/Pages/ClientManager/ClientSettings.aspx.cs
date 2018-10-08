using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Image = System.Drawing.Image;

namespace LinkOnline.Pages.ClientManager
{
    public partial class ClientSettings : WebUtilities.BasePage
    {
        #region Properties

        #endregion

        #region Constructor

        #endregion
        #region Methods
        private void GetClientDetails()
        {
            var clientManager = new ApplicationUtilities.Classes.ClientManager
            {
                //FileName = ConfigurationManager.AppSettings["ClientDetailsRootPath"]
                FileName = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "Clients.xml")
            };

            //Initializing the Client Details
            var clientDetails = clientManager.GetSingle(Global.Core.ClientName);
            HttpContext.Current.Session["ClientName"] = clientDetails.Name;
            colorpickerField1.Value = clientDetails.Color1;
            colorpickerField2.Value = clientDetails.Color2;
            divMainColor.Style.Add("background-color", clientDetails.Color1);
            div2ndColor.Style.Add("background-color", clientDetails.Color2);
            // Getting the Client Logo 
            imgbtnChangeLogo.ImageUrl = "/Images/ClientLogos/" + clientDetails.Name + ".png?" + DateTime.Now.ToString("ddyyhhmmss");
            var imageFilepath = Path.Combine(Request.PhysicalApplicationPath, "Images", "ClientLogos", clientDetails.Name + ".png");
            if (File.Exists(imageFilepath))
            {
                using (var img = System.Drawing.Image.FromFile(imageFilepath))
                {
                    if (img.Height > 150) { imgbtnChangeLogo.Height = 150; }
                    if (img.Width > 150) { imgbtnChangeLogo.Width = 150; }
                }
            }
            else
            {
                imgbtnChangeLogo.ImageUrl = "/Images/ClientLogos/default.png?" + DateTime.Now.ToString("ddyyhhmmss");
                var noimage = Path.Combine(Request.PhysicalApplicationPath, "Images", "ClientLogos", "default.png");
                using (var img = System.Drawing.Image.FromFile(noimage))
                {
                    if (img.Height > 150) { imgbtnChangeLogo.Height = 100; }
                    if (img.Width > 150) { imgbtnChangeLogo.Width = 100; }
                }
            }

        }
        private void UploadImage()
        {
            if (HttpContext.Current.Session["ClientName"] != null)
            {
                if (!string.IsNullOrEmpty(fileUploadImage.FileName))
                {
                    string fileName = Path.Combine(
                        Request.PhysicalApplicationPath,
                        "Images",
                        "ClientLogos",
                         HttpContext.Current.Session["ClientName"] + new FileInfo(fileUploadImage.FileName).Extension
                    );

                    if ((fileUploadImage.PostedFile.ContentType.ToLower() == "image/jpg") ||
                        (fileUploadImage.PostedFile.ContentType.ToLower() == "image/jpeg") ||
                        (fileUploadImage.PostedFile.ContentType.ToLower() == "image/pjpeg") ||
                        (fileUploadImage.PostedFile.ContentType.ToLower() == "image/gif") ||
                        (fileUploadImage.PostedFile.ContentType.ToLower() == "image/x-png") ||
                        (fileUploadImage.PostedFile.ContentType.ToLower() == "image/png"))
                    {
                        string fileName2 = Path.Combine(
                            Request.PhysicalApplicationPath,
                            "Images",
                            "ClientLogos",
                             HttpContext.Current.Session["ClientName"] + ".png"
                        );

                        fileUploadImage.PostedFile.SaveAs(fileName);

                        if (fileName.EndsWith(".png"))
                            return;

                        //var bitmap = new Bitmap(fileName);

                        //bitmap.Save(
                        //    fileName2,
                        //    System.Drawing.Imaging.ImageFormat.Png
                        //);
                        //bitmap.Dispose();

                        using (var img = Image.FromFile(fileName))
                        {
                            if (img.Height > 150 || img.Width > 150)
                            {
                                var newImage = ResizeImage(img, 150, 150);
                                newImage.Save(fileName2, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                        File.Delete(fileName);
                        boxLogo.Visible = false;

                        // Getting the Client Logo
                        imgbtnChangeLogo.ImageUrl = "/Images/ClientLogos/" + HttpContext.Current.Session["ClientName"] + ".png?" + DateTime.Now.ToString("ddyyhhmmss");
                    }
                    else
                    {
                        Response.Redirect("ClientSettings.aspx?msg=2");
                    }
                }
            }
            else
            {
                Response.Redirect("~/Pages/Login.aspx");
            }
        }
        public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        private void SaveToXml()
        {
            var clientManager = new ApplicationUtilities.Classes.ClientManager();
            // Get the Clients XML file
            //string clientFileName = ConfigurationManager.AppSettings["ClientDetailsRootPath"];
            clientManager.FileName = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "Clients.xml");

            //Initializing the Client Details
            var clientDetails = clientManager.GetSingle(Global.Core.ClientName);
            //Assign the values to the Client 
            clientDetails.Color1 = "#" + colorpickerField1.Value.Trim().Replace("#", "");
            clientDetails.Color2 = "#" + colorpickerField2.Value.Trim().Replace("#", "");
            clientDetails.Save();
        }
        private void ResetColors()
        {
            var clientManager = new ApplicationUtilities.Classes.ClientManager();
            // Get the Clients XML file
            //string clientFileName = ConfigurationManager.AppSettings["ClientDetailsRootPath"];
            clientManager.FileName = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "Clients.xml");

            //Initializing the Client Details
            var clientDetails = clientManager.GetSingle(Global.Core.ClientName);

            string color1, color2;
            if (ConfigurationManager.AppSettings["DefaultColor1"] != null)
            {
                color1 = ConfigurationManager.AppSettings["DefaultColor1"].Trim();
            }
            else
            {
                color1 = "#6CAEE0";
            }
            if (ConfigurationManager.AppSettings["DefaultColor2"] != null)
            {
                color2 = ConfigurationManager.AppSettings["DefaultColor2"].Trim();
            }
            else
            {
                color2 = "#FCB040";
            }
            //Assign the values to the Client 
            clientDetails.Color1 = color1;
            clientDetails.Color2 = color2;
            clientDetails.Save();
            string existFile = Path.Combine(
                        Request.PhysicalApplicationPath,
                        "Images",
                        "ClientLogos"
                    // HttpContext.Current.Session["ClientName"] + ".png"
                    );
            string[] files = Directory.GetFiles(existFile);
            existFile = Array.Find(files, n => n.Contains(HttpContext.Current.Session["ClientName"].ToString()));
            if (File.Exists(existFile))
            {
                File.Delete(existFile);
                File.Copy(Path.Combine(
                            Request.PhysicalApplicationPath,
                            "Images",
                            "ClientLogos",
                             "default.png"
                        ), existFile);
            }
        }
        #endregion
        #region EventHandlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["msg"] != null)
                {
                    if (Request.QueryString["msg"].Trim() == "1")
                    {
                        base.ShowMessage(string.Format(
                        Global.LanguageManager.GetText("SuccessMsg"),
                        Global.LanguageManager.GetText("SuccessMsg")), WebUtilities.MessageType.Success);
                    }
                    if (Request.QueryString["msg"].Trim() == "2")
                    {
                        base.ShowMessage(string.Format(
                        Global.LanguageManager.GetText("NotImage"),
                        Global.LanguageManager.GetText("NotImage")), WebUtilities.MessageType.Error);
                    }
                }

                GetClientDetails();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveToXml();
            Response.Redirect("ClientSettings.aspx?msg=1", false);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetColors();
            Response.Redirect("ClientSettings.aspx", false);
        }

        protected void btnChangeLogo_Click(object sender, EventArgs e)
        {
            boxLogo.Visible = true;
        }

        protected void btnUpdateImage_Click(object sender, EventArgs e)
        {
            SaveToXml();
            UploadImage();
            Response.Redirect("ClientSettings.aspx");
        }
        #endregion
    }
}