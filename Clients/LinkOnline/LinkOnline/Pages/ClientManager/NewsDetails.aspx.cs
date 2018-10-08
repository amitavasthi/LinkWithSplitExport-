using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ApplicationUtilities.Classes;
using CsQuery.Web;
using DatabaseCore.Items;
using LinkOnline.Classes;
using WebUtilities.Classes.Controls.GridClasses;
using WebUtilities.Controls;
using System.IO;

namespace LinkOnline.Pages.ClientManager
{
    public partial class NewsDetails : WebUtilities.BasePage
    {
        #region Properties

        private Grid gridNews;
        private User user;

        #endregion

        #region Methods

        private void CreateGridNews()
        {
            try
            {
                gridNews = new Grid { ID = "gridNews" };

                GridHeadline headline = new GridHeadline(gridNews);
                headline.Items.Add(new GridHeadlineItem(headline, 0, "news heading", new GridHeadlineItemWidth(20)));
                headline.Items.Add(new GridHeadlineItem(headline, 1, "news description", new GridHeadlineItemWidth(60)));
                headline.Items.Add(new GridHeadlineItem(headline, 2, "created user", new GridHeadlineItemWidth(10)));
                headline.Items.Add(new GridHeadlineItem(headline, 3, "created date", new GridHeadlineItemWidth(10)));
                gridNews.GridHeadline = headline;

                NewsManager newsManager = new NewsManager();
                // Get the Clients XML file 
                //string fileName = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "News", "News.xml");
                string fileName = Path.Combine(Request.PhysicalApplicationPath, "Fileadmin", "News", Global.Core.ClientName + ".xml");
                newsManager.FileName = fileName;

                XmlNodeList xmlNewsList = newsManager.GetNewsAll(Global.Core.ClientName);

                if (xmlNewsList != null)
                {
                    foreach (XmlNode xn in xmlNewsList)
                    {
                        var xmlElement = xn;
                        if (xmlElement != null)
                        {
                            GridRow row = new GridRow(gridNews, xmlElement.Attributes["Id"].Value);

                            GridRowItem itemHeading = new GridRowItem(row, xmlElement.Attributes["Heading"].Value);
                            itemHeading.Width = 20;
                            GridRowItem itemDesc = new GridRowItem(row, xmlElement.Attributes["Description"].Value);
                            itemDesc.Width = 40;
                            itemDesc.CssClass = "gridWrap";
                            string name = "";
                            if (xmlElement.Attributes["UserId"] != null)
                            {
                                User user = null;
                                user = Global.Core.Users.GetSingle(Guid.Parse(xmlElement.Attributes["UserId"].Value.Trim()));
                                if (user != null)
                                {
                                    name = user.FirstName + "&nbsp;" + user.LastName;
                                }
                            }
                            GridRowItem itemUser = new GridRowItem(row, name);
                            itemUser.Width = 20;

                            GridRowItem itemMc = xmlElement.Attributes["CreatedDate"] != null ? new GridRowItem(row, Convert.ToDateTime(xmlElement.Attributes["CreatedDate"].Value).ToString("MM-dd-yyyy")) : new GridRowItem(row, "");
                            itemMc.Width = 20;
                            row.Items.Add(itemHeading);
                            row.Items.Add(itemDesc);
                            row.Items.Add(itemUser);
                            row.Items.Add(itemMc);
                            gridNews.Rows.Add(row);
                        }
                    }
                }
               
                pnlnewsManagement.Controls.Add(gridNews);

                if (HttpContext.Current.Session["newsId"] != null)
                {
                    gridNews.SelectedItem = HttpContext.Current.Session["newsId"].ToString();
                }
                if (gridNews.SelectedItem == null || gridNews.SelectedItem.ToString() != string.Empty)
                {
                    if (gridNews.Rows.Count != 0)
                        gridNews.SelectedItem = gridNews.Rows[0].Identity;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("/Pages/ErrorPage.aspx");
            }

        }

        #endregion
        #region EventHandlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
                        case "6":
                            base.ShowMessage("no news available for modify / delete", WebUtilities.MessageType.Error);
                            break;
                    }
                }
            }           
            CreateGridNews();
        }


        protected void btnCreate_OnClick(object sender, EventArgs e)
        {
            try
            {
                NewsManager newsManager = new NewsManager();
                // Get the Clients XML file 
                //string fileName = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "News", "News.xml");//ConfigurationManager.AppSettings["NewsDetailsRootPath"];
                string fileName = Path.Combine(Request.PhysicalApplicationPath, "Fileadmin", "News", Global.Core.ClientName + ".xml");
                newsManager.FileName = fileName;
                newsManager.Client = Global.Core.ClientName;
                newsManager.Guid = Guid.NewGuid().ToString();
                newsManager.Heading = txtnewsHeading.Text.Trim();
                newsManager.Description = txtDescription.Text.Trim();
                newsManager.UserId = Global.User.Id.ToString();
                newsManager.CreatedDate = DateTime.Now.ToString();//DateTime.Now.ToString("MM-dd-yyyy");
                newsManager.Save();

                Response.Redirect("NewsDetails.aspx?msg=3", false);
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
                lblError.Name = "";
                lblError.Visible = false;
                // Get the Clients XML file 
                //var fileName = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "News", "News.xml");//ConfigurationManager.AppSettings["NewsDetailsRootPath"];
                var fileName = Path.Combine(Request.PhysicalApplicationPath, "Fileadmin", "News", Global.Core.ClientName + ".xml");
                NewsManager nM = new NewsManager
                {
                    Client = Global.Core.ClientName,
                    FileName = fileName
                };
                if (gridNews.SelectedItem == null)
                {
                    if (gridNews.Rows.Count != 0)
                    {
                        gridNews.SelectedItem = gridNews.Rows[0].Identity;
                    }
                }

                var newsId = gridNews.SelectedItem;
                if (newsId != null)
                {
                    HttpContext.Current.Session["newsId"] = gridNews.SelectedItem;
                    XmlNode node = nM.GetNews(newsId.ToString());
                    if (node != null)
                    {
                        if (node.Attributes != null)
                        {
                            //txtMHeading.Text = node.Attributes["Heading"].Value;
                            //txtMDescription.Text = node.Attributes["Description"].Value;
                            if (node.Attributes["UserId"].Value.Trim() == Global.User.Id.ToString().Trim())
                            {
                                HttpContext.Current.Response.Redirect("ManageNews.aspx", false);

                            }
                            else
                            {
                                HttpContext.Current.Response.Redirect("NewsDetails.aspx?msg=1", false);
                            }
                        }

                        // boxModify.Visible = true;
                    }
                }
                else
                {
                    HttpContext.Current.Response.Redirect("NewsDetails.aspx?msg=6", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("/Pages/ErrorPage.aspx");
            }
        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            //lblError.Visible = false;
            //lblError.Name = "";
            //boxAddNews.Visible = true;
            HttpContext.Current.Session["newsId"] = null;
            Response.Redirect("ManageNews.aspx", false);
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            try
            {
                lblError.Visible = false;
                // Get the Clients XML file 
                //var fileName = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "News", "News.xml");//ConfigurationManager.AppSettings["NewsDetailsRootPath"];
                var fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "News", Global.Core.ClientName + ".xml");
                var nM = new NewsManager
                {
                    Client = Global.Core.ClientName,
                    FileName = fileName
                };
                var newsId = gridNews.SelectedItem;
                if (newsId != null)
                {
                    XmlNode node = nM.GetNews(newsId.ToString());
                    if (node.Attributes != null)
                    {
                        if (node.Attributes["UserId"].Value.Trim() != Global.User.Id.ToString().Trim())
                        {
                            HttpContext.Current.Response.Redirect("NewsDetails.aspx?msg=2", false);
                        }
                        else
                        {
                            cbRemove.Visible = true;

                            cbRemove.Text = string.Format(
                                Global.LanguageManager.GetText("RemoveNewsConfirmMessage")
                            );
                            cbRemove.Confirm = delegate()
                                   {
                                       nM.Delete(newsId.ToString());
                                       if (gridNews != null)
                                       {
                                           gridNews.SelectedItem = gridNews.Rows[0].Identity;
                                       }
                                       HttpContext.Current.Response.Redirect("NewsDetails.aspx?msg=4", false);
                                   };
                        }
                    }
                }
                else
                {
                    HttpContext.Current.Response.Redirect("NewsDetails.aspx?msg=6", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                HttpContext.Current.Response.Redirect("/Pages/ErrorPage.aspx");
            }

        }
        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            try
            {
                // Get the Clients XML file 
                //string fileName = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "News", "News.xml");//ConfigurationManager.AppSettings["NewsDetailsRootPath"];
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
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex.Message;
                Response.Redirect("/Pages/ErrorPage.aspx");
            }
        }
        #endregion
    }
}