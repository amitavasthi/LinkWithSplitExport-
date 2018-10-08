using DatabaseCore.Items;
using LinkOnline.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WebUtilities.Controls;

namespace LinkOnline.Pages.ClientManager
{
    public partial class ChartsDetails : WebUtilities.BasePage
    {
        #region Properties
        #endregion
        #region Methods

        private void FillDetails()
        {
            if (HttpContext.Current.Session["chartId"] != null)
            {
                HomeManager homeManager = new HomeManager();

                string fileName = Path.Combine(Request.PhysicalApplicationPath, "Fileadmin", "Home", Global.Core.ClientName + "HomePage.xml");//Path.Combine(Request.PhysicalApplicationPath, "App_Data", "HomePage.xml");
                homeManager.FileName = fileName;

                XmlNode xmlNewsList = homeManager.ChartDetails(HttpContext.Current.Session["chartId"].ToString());

                if (xmlNewsList != null)
                {
                    if (xmlNewsList.Attributes["heading"] != null)
                        txtMCHeading.Text = xmlNewsList.Attributes["heading"].Value;
                    if (xmlNewsList.Attributes["url"] != null)
                        txtMChartURL.Text = xmlNewsList.Attributes["url"].Value;
                }
            }
        }
        protected bool CheckUrlStatus(string Website)
        {
            try
            {
                if (Website.Split('.')[0] == "www")
                {
                    Website = "http://" + Website;
                }
                var request = WebRequest.Create(Website) as HttpWebRequest;
                request.Method = "HEAD";
                request.Accept = "*/*";
                request.Method = "GET"; request.UserAgent = "Foo"; request.Accept = "text/html";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
        #region EventHandlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HttpContext.Current.Session["chartId"] != null)
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
            }
            if (Request.QueryString["msg"] != null)
            {
                switch (Request.QueryString["msg"].Trim())
                {
                    case "1":
                        base.ShowMessage(string.Format(
                        Global.LanguageManager.GetText("ValidURL"),
                        Global.LanguageManager.GetText("ValidURL")), WebUtilities.MessageType.Error);
                        break;
                    case "2":
                        base.ShowMessage(string.Format(
                        Global.LanguageManager.GetText("ChartSuccess"),
                        Global.LanguageManager.GetText("ChartSuccess")), WebUtilities.MessageType.Success);
                        break;
                    case "3":
                        base.ShowMessage(string.Format(
                        Global.LanguageManager.GetText("ValidURL"),
                        Global.LanguageManager.GetText("ValidURL")), WebUtilities.MessageType.Error);
                        break;
                }
            }
        }

        protected void btnSaveDetails_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtChartURL.Text))
            {
                var hManager = new HomeManager();
                hManager.FileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", Global.Core.ClientName + "HomePage.xml");// Path.Combine(Request.PhysicalApplicationPath, "App_Data", "HomePage.xml");
                hManager.GuId = Guid.NewGuid().ToString();
                hManager.NewsFile = "News.xml";
                hManager.RssFeedUrl = "http://blueoceanmi.com/newsroom";
                hManager.ChartHeading = txtCheading.Text.Trim();
                if (CheckUrlStatus(txtChartURL.Text.Trim()))
                {
                    hManager.ChartsURL = txtChartURL.Text.Trim();
                    hManager.ClientName = Global.Core.ClientName;
                    hManager.CreatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                    hManager.UserId = Global.User.Id.ToString();
                    hManager.Save();
                    if (chkMultiple.Checked)
                    {
                        Response.Redirect("ChartsDetails.aspx?msg=2", false);
                    }
                    else
                    {
                        Response.Redirect("HomeManagement.aspx?msg=2", false);
                    }
                }
                else
                {
                    Response.Redirect("ChartsDetails.aspx?msg=1", false);
                }
            }
        }

        protected void btnModify_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMChartURL.Text))
            {
                if (HttpContext.Current.Session["chartId"] != null)
                {
                    var hManager = new HomeManager();
                    hManager.FileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Fileadmin", "Home", Global.Core.ClientName + "HomePage.xml");//Path.Combine(Request.PhysicalApplicationPath, "App_Data", "HomePage.xml");
                    hManager.GuId = Guid.NewGuid().ToString();
                    hManager.NewsFile = "News.xml";
                    hManager.RssFeedUrl = "http://blueoceanmi.com/newsroom";
                    hManager.ChartHeading = txtMCHeading.Text.Trim();
                    if (CheckUrlStatus(txtMChartURL.Text.Trim()))
                    {
                        hManager.ChartsURL = txtMChartURL.Text.Trim();
                        hManager.ClientName = Global.Core.ClientName;
                        hManager.CreatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                        hManager.UserId = Global.User.Id.ToString();
                        hManager.Modify(HttpContext.Current.Session["chartId"].ToString());
                        Response.Redirect("HomeManagement.aspx?msg=2", false);
                    }
                    else
                    {
                        Response.Redirect("ChartsDetails.aspx?msg=3", false);
                    }
                }
            }
        }
        #endregion
    }
}