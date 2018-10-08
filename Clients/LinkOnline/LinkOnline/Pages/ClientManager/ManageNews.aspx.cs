using DatabaseCore.Items;
using LinkOnline.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkOnline.Pages.ClientManager
{
    public partial class ManageNews : WebUtilities.BasePage
    {
        #region Properties
        private User user;
        #endregion

        #region Methods

        private void FillDetails()
        {
            if (HttpContext.Current.Session["newsId"] != null)
            {
                var fileName = Path.Combine(Request.PhysicalApplicationPath, "Fileadmin", "News", Global.Core.ClientName + ".xml");
                NewsManager nM = new NewsManager
                {
                    Client = Global.Core.ClientName,
                    FileName = fileName
                };

                var newsId = HttpContext.Current.Session["newsId"];
                XmlNode node = nM.GetNews(newsId.ToString());
                if (node != null)
                {
                    if (node.Attributes != null)
                    {
                        txtMHeading.Text = node.Attributes["Heading"].Value;
                        txtMDescription.Text = node.Attributes["Description"].Value;
                    }
                }
            }
        }

        #endregion

        #region EventHandlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HttpContext.Current.Session["newsId"] != null)
                {
                    FillDetails();
                    divModify.Visible = true;
                    divCreate.Visible = false;
                }
                else
                {
                    divCreate.Visible = true;
                    divModify.Visible = false;
                }
                if (Request.QueryString["msg"] != null)
                {
                    switch (Request.QueryString["msg"].Trim())
                    {
                        case "1":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("NewsModifyMsg"),
                            Global.LanguageManager.GetText("NewsModifyMsg")), WebUtilities.MessageType.Error);
                            break;
                        case "2":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("NewsDelteMsg"),
                            Global.LanguageManager.GetText("NewsDelteMsg")), WebUtilities.MessageType.Error);
                            break;
                        case "3":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("AddNewsMsg"),
                            Global.LanguageManager.GetText("AddNewsMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "4":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("DeleteNewsMsg"),
                            Global.LanguageManager.GetText("DeleteNewsMsg")), WebUtilities.MessageType.Success);
                            break;
                        case "5":
                            base.ShowMessage(string.Format(
                            Global.LanguageManager.GetText("ModifyNewsMsg"),
                            Global.LanguageManager.GetText("ModifyNewsMsg")), WebUtilities.MessageType.Success);
                            break;
                    }
                }
            }
        }
        protected void btnSaveDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.User != null)
                {
                    NewsManager newsManager = new NewsManager();
                    // Get the Clients XML file                
                    string fileName = Path.Combine(Request.PhysicalApplicationPath, "Fileadmin", "News", Global.Core.ClientName + ".xml");
                    newsManager.FileName = fileName;
                    newsManager.Client = Global.Core.ClientName;
                    newsManager.Guid = Guid.NewGuid().ToString();
                    newsManager.Heading = txtnewsHeading.Text.Trim();
                    newsManager.Description = txtDescription.Text.Trim();
                    newsManager.UserId = Global.User.Id.ToString();
                    newsManager.CreatedDate = DateTime.Now.ToString();//DateTime.Now.ToString("MM-dd-yyyy");
                    newsManager.Save();
                    if (chkMultiple.Checked)
                    {
                        Response.Redirect("ManageNews.aspx?msg=3", false);
                    }
                    else
                    {
                        Response.Redirect("NewsDetails.aspx?msg=3", false);
                    }
                }
                else
                {
                    Response.Redirect("/Pages/Login.aspx", false);
                }

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("/Pages/ErrorPage.aspx");
            }
        }

        protected void btnModify_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.User != null)
                {
                    // Get the Clients XML file 
                    string fileName = Path.Combine(Request.PhysicalApplicationPath, "Fileadmin", "News", Global.Core.ClientName + ".xml");
                    if (HttpContext.Current.Session["newsId"] != null)
                    {
                        var newsManager = new NewsManager
                        {
                            FileName = fileName,
                            Client = Global.Core.ClientName,
                            Guid = HttpContext.Current.Session["newsId"].ToString(),
                            Heading = txtMHeading.Text.Trim(),
                            Description = txtMDescription.Text.Trim(),
                            UserId = Global.User.Id.ToString(),
                            CreatedDate = DateTime.Now.ToString()
                        };
                        newsManager.Modify(HttpContext.Current.Session["newsId"].ToString());
                        Response.Redirect("NewsDetails.aspx?msg=5", false);
                    }
                }
                else
                {
                    Response.Redirect("/Pages/Login.aspx", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("/Pages/ErrorPage.aspx");
            }
        }
        #endregion
    }
}