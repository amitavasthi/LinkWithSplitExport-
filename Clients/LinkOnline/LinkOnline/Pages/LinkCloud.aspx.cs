using Crosstables.Classes.ReportDefinitionClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LinkOnline.Pages
{
    public partial class LinkCloud : WebUtilities.BasePage
    {
        #region Properties

        public LibraryOrderField OrderField { get; set; }
        public LibraryOrderDirection OrderDirection { get; set; }

        public string RootDirectory
        {
            get
            {
                return Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Fileadmin",
                    "Cloud",
                    Global.Core.ClientName
                );
            }
        }

        public string SelectedDirectory
        {
            get
            {
                if (HttpContext.Current.Session["LinkCloudSelectedDirectory"] == null)
                    return null;

                return HttpContext.Current.Session["LinkCloudSelectedDirectory"].ToString();
            }
            set
            {
                HttpContext.Current.Session["LinkCloudSelectedDirectory"] = value;
            }
        }

        public bool SelectedDirectoryIsSavedReports
        {
            get
            {
                return this.IsSelectedDirectoryIsSavedReports(this.SelectedDirectory);
            }
        }

        public bool SelectedDirectoryIsSavedReportsRoot
        {
            get
            {
                return this.IsSelectedDirectoryIsSavedReportsRoot(this.SelectedDirectory);
            }
        }

        public List<string> SearchedDirectories { get; set; }

        #endregion


        #region Constructor

        public LinkCloud()
            : base()
        { }

        #endregion


        #region Methods

        private int OrderFile(string file1, string file2)
        {
            if (this.OrderDirection == LibraryOrderDirection.Descending)
            {
                string temp = file1;

                file1 = file2;
                file2 = temp;
            }

            switch (this.OrderField)
            {
                case LibraryOrderField.Name:
                    return file1.CompareTo(file2);
                case LibraryOrderField.Type:
                    if (File.Exists(file1) || File.Exists(file2))
                        return Global.LanguageManager.GetText("FileFormat" + new FileInfo(file1).Extension).CompareTo(
                            Global.LanguageManager.GetText("FileFormat" + new FileInfo(file2).Extension)
                        );
                    else
                        return Global.LanguageManager.GetText("FileFormat" + new FileInfo(file1).Extension).CompareTo(
                        Global.LanguageManager.GetText("FileFormat" + new FileInfo(file2).Extension)
                    );
                case LibraryOrderField.Size:

                    return new FileInfo(file1).Length.CompareTo(new FileInfo(file2).Length);

                case LibraryOrderField.CreationDate:
                    return new FileInfo(file1).CreationTime.CompareTo(new FileInfo(file2).CreationTime);
                case LibraryOrderField.ModificationDate:
                    return new FileInfo(file1).LastWriteTime.CompareTo(new FileInfo(file2).LastWriteTime);
                case LibraryOrderField.Author:

                    string author1 = "";
                    string author2 = "";

                    if (File.Exists(file1 + ".LiNK_fileinfo"))
                        author1 = Global.GetNiceUsername(new Guid(File.ReadAllBytes(file1 + ".LiNK_fileinfo")));
                    if (File.Exists(file2 + ".LiNK_fileinfo"))
                        author2 = Global.GetNiceUsername(new Guid(File.ReadAllBytes(file2 + ".LiNK_fileinfo")));

                    return author1.CompareTo(author2);
            }

            return 0;
        }
        private int OrderDirectory(string directory1, string directory2)
        {
            if (this.OrderDirection == LibraryOrderDirection.Descending)
            {
                string temp = directory1;

                directory1 = directory2;
                directory2 = temp;
            }

            switch (this.OrderField)
            {
                case LibraryOrderField.Name:
                case LibraryOrderField.Type:
                    return directory1.CompareTo(directory2);
                case LibraryOrderField.Size:
                    return GetDirectorySize(directory1).CompareTo(GetDirectorySize(directory2));
                case LibraryOrderField.CreationDate:
                    return new DirectoryInfo(directory1).CreationTime.CompareTo(
                        new DirectoryInfo(directory2).CreationTime);
                case LibraryOrderField.ModificationDate:
                    return new DirectoryInfo(directory1).LastWriteTime.CompareTo(
                        new DirectoryInfo(directory2).LastWriteTime);
            }

            return 0;
        }
        private int OrderSavedReport(string report1, string report2)
        {
            if (this.OrderDirection == LibraryOrderDirection.Descending)
            {
                string temp = report1;

                report1 = report2;
                report2 = temp;
            }


            Guid guidOutput = Guid.Empty;
            //check is report directory is guid or no.
            //if (!Guid.TryParse(Path.GetFileName(report2), out guidOutput) || !Guid.TryParse(Path.GetFileName(report1), out guidOutput))
            //return 0;

            switch (this.OrderField)
            {
                case LibraryOrderField.Name:
                    if (File.Exists(Path.Combine(report1, "Info.xml")) && File.Exists(Path.Combine(report2, "Info.xml")))
                        return new ReportDefinitionInfo(Path.Combine(report1, "Info.xml")).Name.CompareTo(
                            new ReportDefinitionInfo(Path.Combine(report2, "Info.xml")).Name
                        );
                    else if (File.Exists(Path.Combine(report1, "Info.xml")) && !File.Exists(Path.Combine(report2, "Info.xml")))
                        return new ReportDefinitionInfo(Path.Combine(report1, "Info.xml")).Name.CompareTo(
                            Path.GetFileName(report2)
                        );
                    else if (!File.Exists(Path.Combine(report1, "Info.xml")) && File.Exists(Path.Combine(report2, "Info.xml")))
                        return Path.GetFileName(report1).CompareTo(
                            new ReportDefinitionInfo(Path.Combine(report2, "Info.xml")).Name
                        );
                    else
                        return Path.GetFileName(report1).CompareTo(
                            Path.GetFileName(report2));
                case LibraryOrderField.Type:
                    if (File.Exists(Path.Combine(report1, "Info.xml")) && File.Exists(Path.Combine(report2, "Info.xml")))
                        return new ReportDefinitionInfo(Path.Combine(report1, "Info.xml")).Name.CompareTo(
                            new ReportDefinitionInfo(Path.Combine(report2, "Info.xml")).Name
                        );
                    else if (File.Exists(Path.Combine(report1, "Info.xml")) && !File.Exists(Path.Combine(report2, "Info.xml")))
                        return -1;
                    else if (!File.Exists(Path.Combine(report1, "Info.xml")) && File.Exists(Path.Combine(report2, "Info.xml")))
                        return 1;
                    else
                        return Path.GetDirectoryName(report1).CompareTo(Path.GetDirectoryName(report2));
                case LibraryOrderField.Size:
                    return GetDirectorySize(report1).CompareTo(GetDirectorySize(report2));
                case LibraryOrderField.CreationDate:
                    if (File.Exists(Path.Combine(report1, "Info.xml")) && File.Exists(Path.Combine(report2, "Info.xml")))
                        return File.GetCreationTime(Path.Combine(report1, "Info.xml")).CompareTo(
                      File.GetCreationTime(Path.Combine(report1, "Info.xml")));
                    else if (File.Exists(Path.Combine(report1, "Info.xml")) && !File.Exists(Path.Combine(report2, "Info.xml")))
                        return File.GetCreationTime(Path.Combine(report1, "Info.xml")).CompareTo(
                      new DirectoryInfo(report2).CreationTime);
                    else if (!File.Exists(Path.Combine(report1, "Info.xml")) && File.Exists(Path.Combine(report2, "Info.xml")))
                        return new DirectoryInfo(report1).CreationTime.CompareTo(
                      File.GetCreationTime(Path.Combine(report2, "Info.xml")));
                    else
                        return new DirectoryInfo(report1).CreationTime.CompareTo(
                            new DirectoryInfo(report2).CreationTime);
                case LibraryOrderField.ModificationDate:
                    if (File.Exists(Path.Combine(report1, "Info.xml")) && File.Exists(Path.Combine(report2, "Info.xml")))
                        return File.GetLastWriteTime(Path.Combine(report1, "Info.xml")).CompareTo(
                      File.GetLastWriteTime(Path.Combine(report1, "Info.xml")));
                    else if (File.Exists(Path.Combine(report1, "Info.xml")) && !File.Exists(Path.Combine(report2, "Info.xml")))
                        return File.GetLastWriteTime(Path.Combine(report1, "Info.xml")).CompareTo(
                      new DirectoryInfo(report2).LastWriteTime);
                    else if (!File.Exists(Path.Combine(report1, "Info.xml")) && File.Exists(Path.Combine(report2, "Info.xml")))
                        return new DirectoryInfo(report1).LastWriteTime.CompareTo(
                      File.GetLastWriteTime(Path.Combine(report2, "Info.xml")));
                    else
                        return new DirectoryInfo(report1).LastWriteTime.CompareTo(
                            new DirectoryInfo(report2).LastWriteTime);
            }

            return 0;
        }


        /// <summary>
        /// Builds a tree view with all folders shown.
        /// </summary>
        private void BuildTree()
        {
            // Build the full path to the client's cloud directory.
            string rootDirectory = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "Cloud",
                Global.Core.ClientName
            );

            // Check if there is a directory selected.
            if (this.SelectedDirectory == null)
            {
                // Set the currently selected
                // directory to the root directory.
                this.SelectedDirectory = rootDirectory;
            }

            // Check if the client's cloud directory exists.
            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);

            // Create a new tree view.
            TreeView treeView = new TreeView();

            treeView.NodeStyle.CssClass = "FileExplorerItem Color1";
            treeView.SelectedNodeStyle.CssClass = "FileExplorerItem_Active Color2";

            // Set the tree view's selected node changed event.
            treeView.SelectedNodeChanged += treeView_SelectedNodeChanged;

            // Build the tree node for the root directory and
            // add it to the tree view at root level.
            treeView.Nodes.Add(
                BuildTreeNode(rootDirectory)
            );

            pnlExplorer.Controls.Add(treeView);
        }

        /// <summary>
        /// Builds a tree view node of a directory.
        /// </summary>
        /// <param name="path">The full path to the directory.</param>
        private TreeNode BuildTreeNode(string path)
        {
            // Create a new tree node.
            TreeNode result = new TreeNode();

            // Check if the tree node is the currently selected tree node,
            // and if the current http request is a post back.
            if (path == this.SelectedDirectory && IsPostBack == false)
            {
                // Select the tree node.
                result.Select();
            }

            // Set the tree node's text to the directory's name.
            result.Text = new DirectoryInfo(path).Name;

            // Set the tree node's value to the full path to the directory.
            result.Value = path;

            // Run through all sub directories of the directory.
            foreach (string directory in Directory.GetDirectories(path))
            {
                // Build the tree node for the sub directory
                // and add it to the tree node's sub nodes.
                result.ChildNodes.Add(
                    BuildTreeNode(directory)
                );
            }

            return result;
        }


        private void RenderSavedReportsDirectory()
        {
            HttpContext.Current.Session["RenderValues"] = true;
            if (this.SelectedDirectory != this.RootDirectory)
                return;

            // Build the full path to the current users's saved reports directory.
            string directory = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedReports",
                Global.Core.ClientName
            );

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            Panel pnlDirectory = new Panel();
            pnlDirectory.CssClass = "CloudItem Directory";

            ImageButton imgDirectory = new ImageButton();
            imgDirectory.ID = "imgDirectory" + "SavedReports";
            imgDirectory.CssClass = "BackgroundColor1";
            imgDirectory.Attributes.Add("Source", directory);
            imgDirectory.ImageUrl = "/Images/Icons/Cloud/Directory.png";
            imgDirectory.Click += imgDirectory_Click;

            imgDirectory.CssClass = "BackgroundColor2";

            imgDirectory.Attributes.Add(
                "oncontextmenu",
                "return false;"
            );

            imgDirectory.Attributes.Add(
                "onmouseover",
                "this.className = 'BackgroundColor20'"
            );

            imgDirectory.Attributes.Add(
                "onmouseout",
                "this.className = 'BackgroundColor2';"
            );

            DirectoryInfo dirInfo = new DirectoryInfo(directory);

            DatabaseCore.Items.User user = Global.User;

            if (user.HasPermission(94))
            {
                pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                    "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                    dirInfo.CreationTime.ToFormattedString()
                )));
                pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                    "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                    dirInfo.LastWriteTime.ToFormattedString()
                )));
            }

            if (user.HasPermission(93))
            {
                pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                    "<div class=\"GridViewDetail\" style=\"width:60px;\">{0}</div>",
                    GetFileSizeStr(GetDirectorySize(dirInfo.FullName))
                )));
            }

            if (user.HasPermission(92))
            {
                pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                    "<div class=\"GridViewDetail\" style=\"width: 150px\">{0}</div>",
                    Global.LanguageManager.GetText("FileFormatSavedReports")
                )));
            }

            pnlDirectory.Controls.Add(imgDirectory);
            //pnlDirectory.Controls.Add(lblDirectory);
            pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                "<div class=\"DirectoryName\">{0}</div>",
                Global.LanguageManager.GetText("SavedReports")
            )));

            pnlFiles.Controls.Add(pnlDirectory);
        }

        private void RenderSavedReports()
        {
            // Build the full path to the current users's saved reports directory.
            string rootDirectory = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "SavedReports",
                Global.Core.ClientName
            );

            // Get all the users of the client.
            List<object[]> users = Global.Core.Users.GetValues(
                new string[] { "Id", "FirstName", "LastName" },
                new string[] { },
                new object[] { }
            );
            List<object[]> userTemp = new List<object[]>();
            if (this.SearchedDirectories != null)
            {
                foreach (string path in this.SearchedDirectories)
                {
                    foreach (object[] user in users)
                    {
                        if (Guid.Parse(Path.GetFileName(path)) == Guid.Parse(user[0].ToString()))
                            userTemp.Add(user);
                    }
                    //if (users.Select(x => Guid.Parse(x[0].ToString()) == Guid.Parse(Path.GetFileName(path))).FirstOrDefault())
                    //    userTemp.Add(users.Find(x => Guid.Parse(x[0].ToString()) == Guid.Parse(Path.GetFileName(path))));
                }
            }
            if (userTemp.Count > 0)
                users = userTemp;
            //if (Global.User.Role.Id.ToString() != "00000000-0000-0000-0000-000000000000")
            //{
            //    users.RemoveAll(x => x[0].ToString() != Global.User.Id.ToString());
            //}
            // Run through all users of the client.
            foreach (object[] user in users)
            {
                string directory = Path.Combine(
                    rootDirectory,
                    user[0].ToString()
                );

                if (!Directory.Exists(directory))
                    continue;

                DirectoryInfo dirInfo = new DirectoryInfo(directory);

                Panel pnlDirectory = new Panel();
                pnlDirectory.CssClass = "CloudItem Directory";

                ImageButton imgDirectory = new ImageButton();
                imgDirectory.ID = "imgDirectory" + user[0].ToString();
                imgDirectory.CssClass = "BackgroundColor1";
                imgDirectory.Attributes.Add("Source", directory);
                imgDirectory.ImageUrl = "/Images/Icons/Cloud/Directory.png";
                imgDirectory.Click += imgDirectory_Click;

                imgDirectory.Attributes.Add(
                    "onmouseover",
                    "this.className = 'BackgroundColor5'"
                );

                if ((Guid)user[0] == Global.IdUser.Value)
                {
                    imgDirectory.Attributes.Add(
                        "onmouseout",
                        "this.className = 'BackgroundColor2';this.src='/Images/Icons/Cloud/" + (this.SelectedDirectoryIsSavedReports ? ".lor" : "Directory") +
                        ".png';document.forms[0].action = document.forms[0].action.split('?')[0];"
                    );

                    imgDirectory.CssClass = "BackgroundColor2";

                    DatabaseCore.Items.User _user = Global.User;

                    if (_user.HasPermission(94))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                            dirInfo.CreationTime.ToFormattedString()
                        )));
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                           "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                           dirInfo.LastWriteTime.ToFormattedString()
                       )));
                    }

                    if (_user.HasPermission(93))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width:60px;\">{0}</div>",
                            GetFileSizeStr(GetDirectorySize(dirInfo.FullName))
                        )));
                    }

                    if (_user.HasPermission(92))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width: 150px\">{0}</div>",
                            Global.LanguageManager.GetText("FileFormatSavedReports")
                        )));
                    }

                    pnlDirectory.Controls.Add(imgDirectory);
                    //pnlDirectory.Controls.Add(lblDirectory);
                    pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                        "<div class=\"DirectoryName\">{0}</div>",
                        Global.LanguageManager.GetText("MySavedReports")
                    )));

                    pnlFiles.Controls.AddAt(0, pnlDirectory);
                }
                else
                {
                    imgDirectory.Attributes.Add(
                        "onmouseout",
                        "this.className = 'BackgroundColor1';this.src='/Images/Icons/Cloud/" + (this.SelectedDirectoryIsSavedReports ? ".lor" : "Directory") +
                        ".png';document.forms[0].action = document.forms[0].action.split('?')[0];"
                    );

                    DatabaseCore.Items.User _user = Global.User;

                    if (_user.HasPermission(94))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                            dirInfo.CreationTime.ToFormattedString()
                        )));
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                            dirInfo.LastWriteTime.ToFormattedString()
                        )));
                    }

                    if (_user.HasPermission(93))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width:60px;\">{0}</div>",
                            GetFileSizeStr(GetDirectorySize(dirInfo.FullName))
                        )));
                    }

                    if (_user.HasPermission(92))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width: 150px\">{0}</div>",
                            Global.LanguageManager.GetText("FileFormatSavedReports")
                        )));
                    }

                    pnlDirectory.Controls.Add(imgDirectory);
                    //pnlDirectory.Controls.Add(lblDirectory);
                    pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                        "<div class=\"DirectoryName\">{0}</div>",
                        string.Format(Global.LanguageManager.GetText(
                            "UsersSavedReports"),
                            Global.GetNiceUsername((Guid)user[0], (string)user[1])
                        )
                    )));

                    pnlFiles.Controls.Add(pnlDirectory);
                }
            }
        }

        private void RenderSelectedDirectory()
        {

            if (this.SelectedDirectoryIsSavedReportsRoot)
            {
                RenderSavedReports();
                return;
            }
            List<string> directories;
            if (this.SearchedDirectories != null)
                directories = this.SearchedDirectories;
            else
                directories = Directory.GetDirectories(this.SelectedDirectory).ToList();

            string isDirectoryPrivate = "";
            if (this.SelectedDirectory.IndexOf(IdUser.ToString()) == -1)
            {
                isDirectoryPrivate = "NA";
            }


            if (this.OrderField != LibraryOrderField.Type)
                if (this.SelectedDirectoryIsSavedReports)
                    directories.Sort(OrderSavedReport);
                else
                    directories.Sort(OrderDirectory);

            List<string> directories1 = new List<string>();
            List<string> reports = new List<string>();

            if (this.OrderField == LibraryOrderField.Type && this.SelectedDirectoryIsSavedReports)
            {
                foreach (string directory in directories)
                {
                    if (File.Exists(Path.Combine(directory, "Info.xml")))
                        reports.Add(directory);
                    else
                        directories1.Add(directory);
                }

                this.OrderField = LibraryOrderField.Name;
                directories1.Sort(OrderSavedReport);
                reports.Sort(OrderSavedReport);


                directories.Clear();
                directories.AddRange(directories1);
                directories.AddRange(reports);
            }
            //create new folder.
            if (this.SelectedDirectoryIsSavedReports)
            {
                Panel pnlDirectory = new Panel();
                pnlDirectory.CssClass = "CloudItem Directory";
                ImageButton imgDirectory = new ImageButton();
                imgDirectory.ID = "imgCreateDirectory";
                if (directories.Count > 0) imgDirectory.Attributes.Add("Source", this.SelectedDirectory);
                imgDirectory.ImageUrl = "/Images/Icons/Cloud/NewDirectory.png";
                imgDirectory.Click += imgCreateFolder_Click;

                imgDirectory.Attributes.Add(
                        "onmouseover",
                        "this.src='/Images/Icons/Cloud/NewDirectory_Hover.png'"
                    );
                imgDirectory.Attributes.Add(
                  "onmouseout",
                  "this.src='/Images/Icons/Cloud/NewDirectory" +
                  ".png';"
              );

                string[] allDirectories = this.SelectedDirectory.Split('\\');
                string idUser = "";
                int i = 0;
                while (allDirectories[i] != "SavedReports")
                {
                    i++;
                }
                if (allDirectories.Length > (i + 2))
                    idUser = allDirectories[i + 2];
                else
                    idUser = "";
                if (idUser == "" || Guid.Parse(idUser) == Global.IdUser.Value)
                {
                    pnlDirectory.Controls.Add(imgDirectory);
                    //pnlDirectory.Controls.Add(lblDirectory);
                    pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                          "<div onmouseover=\"this.title=this.innerText\" style=\"max-height:70px;max-width:350px;overflow: hidden; text-overflow: ellipsis;\"; class=\"DirectoryName\">{0}</div>",
                        PrepareFileName("New Folder")
                    )));

                    pnlFiles.Controls.Add(pnlDirectory);
                }

                pnlFiles.Attributes.Add("oncontextmenu", string.Format(
                      "RenderDirectoryOptions(this, '{0}','{1}'); return false;",
                      this.SelectedDirectory.Replace("\\", "/"),
                      isDirectoryPrivate
                  ));
            }

            // Run through all sub directories of the selected directory.
            foreach (string directory in directories)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);

                Panel pnlDirectory = new Panel();
                pnlDirectory.CssClass = "CloudItem Directory";

                ImageButton imgDirectory = new ImageButton();
                imgDirectory.ID = "imgDirectory" + dirInfo.Name;
                imgDirectory.CssClass = "BackgroundColor1";
                imgDirectory.Attributes.Add("Source", directory);
                imgDirectory.ImageUrl = "/Images/Icons/Cloud/Directory.png";
                imgDirectory.Click += imgDirectory_Click;


                Guid guidOutput = Guid.Empty;
                if (this.SelectedDirectoryIsSavedReports)
                {
                    string isPrivate = "";
                    ReportDefinitionInfo info = null;
                    if (Guid.TryParse(Path.GetFileName(directory), out guidOutput))
                    {
                        info = new ReportDefinitionInfo(Path.Combine(
                           directory,
                           "Info.xml"
                       ));

                        if (info.GetReports(Global.Core, Global.IdUser.Value).Count == 0)
                            continue;

                        isPrivate = info.IsPublic;


                        if (isPrivate == null) { isPrivate = "false"; }

                        if ((dirInfo.FullName.IndexOf(Global.User.Id.ToString()) == -1) && (isPrivate == "true"))
                            continue;

                        if ((dirInfo.FullName.IndexOf(Global.User.Id.ToString()) == -1) && (isPrivate == "false"))
                            isPrivate = "NA";



                        imgDirectory.ImageUrl = "/Images/Icons/Cloud/.lor.png";

                        string[] allDirectories = dirInfo.Parent.FullName.Split('\\');
                        string idUser = "";
                        int i = 0;
                        while (allDirectories[i] != "SavedReports")
                        {
                            i++;
                        }
                        idUser = allDirectories[i + 2];

                        imgDirectory.Attributes.Add("oncontextmenu", string.Format(
                            "RenderSavedReportOptions(this, '{0}', '{1}','{2}'); return false;",
                            directory.Replace("\\", "/"),
                            idUser + dirInfo.Name,
                            isPrivate
                        ));
                    }
                    else
                    {
                        imgDirectory.ImageUrl = "/Images/Icons/Cloud/Directory.png";
                        pnlDirectory.Attributes.Add("oncontextmenu", string.Format(
                      "RenderDirectoryOptions(this, '{0}', '{1}'); return false;",
                      directory.Replace("\\", "/"),
                      isDirectoryPrivate
                  ));
                    }
                }
                else
                {
                    /*imgDirectory.Attributes.Add(
                        "oncontextmenu",
                        "this.src=\"/Images/Icons/Cloud/Delete.png\";document.forms[0].action += '?DeleteDirectory=" +
                        HttpUtility.UrlEncode(directory) + "'; return false;"
                    );*/
                    pnlDirectory.Attributes.Add("oncontextmenu", string.Format(
                        "RenderDirectoryOptions(this, '{0}','{1}'); return false;",
                        directory.Replace("\\", "/"),
                        isDirectoryPrivate
                    ));
                }
                string tooltip = directory.Substring(directory.LastIndexOf(Global.Core.ClientName.ToLower()),
                                    directory.Length - directory.LastIndexOf(Global.Core.ClientName.ToLower()));
                if (this.SelectedDirectoryIsSavedReports)
                {
                    string userId = tooltip.Substring(Global.Core.ClientName.ToLower().Length + 1, 36);
                    //remove client name from path.
                    tooltip = tooltip.Substring(tooltip.LastIndexOf(Global.Core.ClientName.ToLower()) + Global.Core.ClientName.Length + 1,
                        tooltip.Length - (tooltip.LastIndexOf(Global.Core.ClientName.ToLower()) + Global.Core.ClientName.Length + 1));
                    tooltip = tooltip.Replace("\\", "/");
                    if (Guid.TryParse(userId, out guidOutput))
                    {
                        tooltip = tooltip.Replace(guidOutput.ToString(), string.Format(Global.LanguageManager.GetText(
                    "UsersSavedReports"),
                    Global.GetNiceUsername(guidOutput)
                ));
                    }
                }

                if (this.SelectedDirectoryIsSavedReports)
                {
                    guidOutput = Guid.Empty;
                    if (Guid.TryParse(Path.GetFileName(directory), out guidOutput))
                    {
                        ReportDefinitionInfo rInfo = new ReportDefinitionInfo(Path.Combine(directory,
                            "Info.xml"));
                        if (rInfo != null)
                            tooltip = tooltip.Replace(Path.GetFileName(directory), rInfo.Name);
                        imgDirectory.Attributes.Add(
                          "onmouseover",
                          "this.src='/Images/Icons/Cloud/Run.png';ShowToolTip(this, 'Path: (" + HttpUtility.HtmlEncode(tooltip) + ")', undefined, 'Right');"
                      );
                    }
                    else
                    {
                        guidOutput = Guid.Empty;
                        imgDirectory.ImageUrl = "/Images/Icons/Cloud/Directory.png";
                        imgDirectory.Attributes.Add(
                        "onmouseover",
                        "ShowToolTip(this, 'Path: (" + HttpUtility.HtmlEncode(tooltip) + ")', undefined, 'Right');");
                    }
                }
                else
                {
                    imgDirectory.Attributes.Add(
                        "onmouseover",
                        "this.className = 'BackgroundColor5';ShowToolTip(this, 'Path: (" + HttpUtility.HtmlEncode(tooltip) + ")', undefined, 'Right');"
                    );
                }

                guidOutput = Guid.Empty;
                if (Guid.TryParse(Path.GetFileName(directory), out guidOutput))
                    imgDirectory.Attributes.Add(
                    "onmouseout",
                    "this.className = 'BackgroundColor1';this.src='/Images/Icons/Cloud/" + (this.SelectedDirectoryIsSavedReports ? ".lor" : "Directory") +
                    ".png';document.forms[0].action = document.forms[0].action.split('?')[0];"
                );

                //Label lblDirectory = new Label();
                string directoryName = "";

                if (this.SelectedDirectoryIsSavedReports)
                {
                    ReportDefinitionInfo info = null;
                    FileInfo fInfo = null;
                    guidOutput = Guid.Empty;
                    if (Guid.TryParse(Path.GetFileName(directory), out guidOutput))
                    {
                        info = new ReportDefinitionInfo(Path.Combine(
                          dirInfo.FullName,
                          "Info.xml"
                      ));

                        fInfo = new FileInfo(Path.Combine(
                            dirInfo.FullName,
                            "Info.xml"
                        ));

                        //string isPrivate = info.IsPublic;

                        //if ((dirInfo.FullName.IndexOf(Global.User.Id.ToString()) == -1) && (info.IsPublic == "true"))
                        //    continue;

                        directoryName = info.Name;
                    }
                    else
                    {

                        directoryName = Path.GetFileName(dirInfo.FullName);
                    }
                    DatabaseCore.Items.User user = Global.User;

                    if (user.HasPermission(94))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                           (fInfo != null ? fInfo.CreationTime.ToFormattedString() : dirInfo.CreationTime.ToFormattedString())
                        )));
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                           (fInfo != null ? fInfo.LastWriteTime.ToFormattedString() : dirInfo.LastWriteTime.ToFormattedString())
                        )));
                    }

                    if (user.HasPermission(93))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width:60px;\">{0}</div>",
                            GetFileSizeStr(GetDirectorySize(dirInfo.FullName))
                        )));
                    }

                    if (user.HasPermission(92))
                    {
                        guidOutput = Guid.Empty;
                        if (Guid.TryParse(Path.GetFileName(directory), out guidOutput))
                            pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width: 150px\">{0}</div>",
                            Global.LanguageManager.GetText("FileFormatSavedReport")
                        )));
                        else
                            pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width: 150px\">{0}</div>",
                            Global.LanguageManager.GetText("FileFormatDirectory")
                        )));
                    }

                    if (user.HasPermission(91))
                    {

                        string[] allDirectories = directory.Split('\\');
                        string idUser = "";
                        int i = 0;
                        while (allDirectories[i] != "SavedReports")
                        {
                            i++;
                        }
                        idUser = allDirectories[i + 2];
                        guidOutput = Guid.Empty;
                        if (Guid.TryParse(Path.GetFileName(directory), out guidOutput))
                            pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width: 150px\">{0}</div>",
                            Global.GetNiceUsername(Guid.Parse(idUser))
                        )));
                        else
                            pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width: 150px\">{0}</div>",
                            Global.GetNiceUsername(Guid.Parse(idUser))
                        )));
                    }
                }
                else
                {
                    string author = "";

                    if (File.Exists(directory + ".LiNK_dirinfo"))
                    {
                        author = Global.GetNiceUsername(new Guid(File.ReadAllBytes(directory + ".LiNK_dirinfo")));
                    }

                    DatabaseCore.Items.User user = Global.User;

                    if (user.HasPermission(94))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                            dirInfo.CreationTime.ToFormattedString()
                        )));
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                           "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                           dirInfo.LastWriteTime.ToFormattedString()
                        )));
                    }

                    if (user.HasPermission(93))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width:60px;\">{0}</div>",
                            GetFileSizeStr(GetDirectorySize(dirInfo.FullName))
                        )));
                    }

                    if (user.HasPermission(92))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\" style=\"width: 150px\">{0}</div>",
                            Global.LanguageManager.GetText("FileFormatDirectory")
                        )));
                    }

                    if (user.HasPermission(91))
                    {
                        pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                            "<div class=\"GridViewDetail\">{0}</div>",
                            author
                        )));
                    }

                    directoryName = dirInfo.Name;
                }

                pnlDirectory.Controls.Add(imgDirectory);
                //pnlDirectory.Controls.Add(lblDirectory);
                pnlDirectory.Controls.Add(new LiteralControl(string.Format(
                      "<div onmouseover=\"this.title=this.innerText\" style=\"max-height:70px;max-width:350px;overflow: hidden; text-overflow: ellipsis;\"; class=\"DirectoryName\">{0}</div>",
                    PrepareFileName(directoryName)
                )));

                pnlFiles.Controls.Add(pnlDirectory);
            }

            List<string> files = Directory.GetFiles(this.SelectedDirectory).ToList();
            files.Sort(OrderFile);

            // Run through all files of the selected directory.
            foreach (string file in files)
            {
                FileInfo fInfo = new FileInfo(file);

                if (fInfo.Extension == ".LiNK_fileinfo" || fInfo.Extension == ".LiNK_dirinfo")
                    continue;

                Panel pnlFile = new Panel();
                pnlFile.CssClass = "CloudItem File";

                ImageButton imgFile = new ImageButton();
                imgFile.ID = "imgFile" + fInfo.Name.Split('.')[0];
                imgFile.CssClass = "BackgroundColor1";
                imgFile.Attributes.Add("Source", file);

                imgFile.Attributes.Add(
                     "onmousedown",
                     "showSubmitLoading = false;"
                );

                string imageName = fInfo.Extension + ".png";

                string imgFileName = Path.Combine(
                    Request.PhysicalApplicationPath,
                    "Images",
                    "Icons",
                    "Cloud",
                    imageName
                );

                // Check if the file is of a known file type.
                if (!File.Exists(imgFileName))
                    imageName = "Unknown.png";

                imgFile.ImageUrl = "/Images/Icons/Cloud/" + imageName;

                pnlFile.Attributes.Add("oncontextmenu", string.Format(
                    "RenderFileOptions(this, '{0}', '{1}'); return false;",
                    HttpUtility.UrlEncode(file),
                    HttpUtility.HtmlEncode(fInfo.Name.Replace("\n", "").Replace("\r", "").Trim())
                ));

                // Check if the file is a saved report.
                if (fInfo.Extension == ".lor")
                {
                    imgFile.Attributes.Add(
                        "onmouseover",
                        "this.src = '/Images/Icons/Cloud/Run.png'"
                    );

                    imgFile.Attributes.Add(
                        "onmouseout",
                        "this.src = '/Images/Icons/Cloud/" + imageName + "';document.forms[0].action = document.forms[0].action.split('?')[0];"
                    );

                    imgFile.Click += SavedReport_Click;
                }
                else
                {
                    imgFile.Click += imgFile_Click;

                    imgFile.Attributes.Add(
                        "onmouseover",
                        "this.src = '/Images/Icons/Cloud/Download.png'"
                    );

                    imgFile.Attributes.Add(
                        "onmouseout",
                        "this.src = '/Images/Icons/Cloud/" + imageName + "';document.forms[0].action = document.forms[0].action.split('?')[0];"
                    );
                }

                /*Label lblFile = new Label();
                lblFile.Text = "<br />" + PrepareFileName(fInfo.Name);*/

                string author = "";

                if (File.Exists(file + ".LiNK_fileinfo"))
                {
                    author = Global.GetNiceUsername(new Guid(File.ReadAllBytes(file + ".LiNK_fileinfo")));
                }

                string fileType = Global.LanguageManager.GetText("FileFormat" + fInfo.Extension.ToLower());

                if (fileType == "FileFormat" + fInfo.Extension.ToLower())
                {
                    fileType = string.Format(
                        Global.LanguageManager.GetText("FileFormatUnknown"),
                        fInfo.Extension
                    );
                }

                DatabaseCore.Items.User user = Global.User;

                if (user.HasPermission(94))
                {
                    pnlFile.Controls.Add(new LiteralControl(string.Format(
                        "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                        fInfo.CreationTime.ToFormattedString()
                    )));
                    pnlFile.Controls.Add(new LiteralControl(string.Format(
                       "<div class=\"GridViewDetail\" style=\"width:130px;\">{0}</div>",
                       fInfo.LastWriteTime.ToFormattedString()
                   )));
                }

                if (user.HasPermission(93))
                {
                    pnlFile.Controls.Add(new LiteralControl(string.Format(
                        "<div class=\"GridViewDetail\" style=\"width:60px;\">{0}</div>",
                        GetFileSizeStr(fInfo.Length)
                    )));
                }

                if (user.HasPermission(92))
                {
                    pnlFile.Controls.Add(new LiteralControl(string.Format(
                        "<div class=\"GridViewDetail\" style=\"width: 150px\">{0}</div>",
                        fileType
                    )));
                }

                if (user.HasPermission(91))
                {
                    pnlFile.Controls.Add(new LiteralControl(string.Format(
                        "<div class=\"GridViewDetail\">{0}</div>",
                        author
                    )));
                }

                pnlFile.Controls.Add(imgFile);
                pnlFile.Controls.Add(new LiteralControl(string.Format(
                    "<div class=\"DirectoryName\">{0}</div>",
                    PrepareFileName(fInfo.Name)
                )));

                pnlFiles.Controls.Add(pnlFile);
            }

            if (!this.SelectedDirectoryIsSavedReports)
            {
                Panel pnlNewDirectory = new Panel();
                pnlNewDirectory.CssClass = "CloudItem File";

                ImageButton imgNewDirectory = new ImageButton();
                imgNewDirectory.ID = "imgNewDirectory";
                imgNewDirectory.CssClass = "";
                imgNewDirectory.Click += imgNewDirectory_Click;
                imgNewDirectory.ImageUrl = "/Images/Icons/Cloud/NewDirectory.png";

                imgNewDirectory.Attributes.Add(
                    "onmouseover",
                    "this.src = '/Images/Icons/Cloud/NewDirectory_Hover.png'"
                );

                imgNewDirectory.Attributes.Add(
                    "onmouseout",
                    "this.src = '/Images/Icons/Cloud/NewDirectory.png';"
                );

                pnlNewDirectory.Controls.Add(imgNewDirectory);

                pnlFiles.Controls.Add(pnlNewDirectory);
            }
        }

        private long GetDirectorySize(string directoryName)
        {
            long result = 0;

            foreach (string file in Directory.GetFiles(directoryName))
            {
                result += new FileInfo(file).Length;
            }

            foreach (string directory in Directory.GetDirectories(directoryName))
            {
                result += GetDirectorySize(directory);
            }

            return result;
        }

        private string GetFileSizeStr(long length)
        {
            int index = 0;

            while (length > 1024)
            {
                length /= 1024;
                index++;
            }

            string[] sizeSuffixes = new string[]
            {
                "B",
                "KB",
                "MB",
                "GB",
                "TB"
            };

            return length + "&nbsp;" + sizeSuffixes[index];
        }

        private string PrepareFileName(string name)
        {
            string result = name;

            int c = 0;
            for (int i = 0; i < name.Length; i++)
            {
                if (i != 0 && i % 15 == 0)
                {
                    result = result.Insert(i + c, "<br />");

                    c += 6;
                }
            }

            return result;
        }


        protected void imgFile_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgFile = (ImageButton)sender;

            string file = imgFile.Attributes["Source"];

            FileInfo fInfo = new FileInfo(file);

            string mimeType = "text/plain";

            if (fInfo.Extension.Length > 0)
                mimeType = base.GetMimeType(fInfo.Extension.Remove(0, 1));

            base.WriteFileToResponse(file, fInfo.Name, mimeType, false);
        }

        int explorerNavigationItemCount = 0;
        private void RenderExplorerNavigation(string directory)
        {
            // Get the directory info for the directory.
            DirectoryInfo dirInfo = new DirectoryInfo(directory);

            if (directory == Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "Cloud"
            ))
            {
                pnlExplorerNavigation.Controls.Add(
                    new LiteralControl("<div style=\"clear:both;\"></div>")
                );

                return;
            }

            if (directory.EndsWith("Fileadmin\\SavedReports"))
            {
                RenderExplorerNavigation(this.RootDirectory);
                return;
            }

            Panel pnlExplorerNavigationItem = new Panel();
            pnlExplorerNavigationItem.CssClass = "FileExplorerNavigationItem";

            if (directory == this.SelectedDirectory)
                pnlExplorerNavigationItem.CssClass += " BackgroundColor9";

            LinkButton lnkExplorerNavigationItem = new LinkButton();
            lnkExplorerNavigationItem.ID = "lnkExplorerNavigationItem" + explorerNavigationItemCount++;
            lnkExplorerNavigationItem.Text = dirInfo.Name;
            lnkExplorerNavigationItem.Click += lnkExplorerNavigationItem_Click;

            if (this.IsSelectedDirectoryIsSavedReportsRoot(directory))
            {
                lnkExplorerNavigationItem.Text = Global.LanguageManager.GetText("SavedReports");
            }
            else if (this.IsSelectedDirectoryIsSavedReports(directory))
            {
                Guid guidOutput = Guid.Empty;
                Guid idUser = Guid.Empty;
                if (Guid.TryParse(dirInfo.Name, out guidOutput))
                    idUser = Guid.Parse(dirInfo.Name);

                if (idUser == Global.IdUser.Value)
                {
                    lnkExplorerNavigationItem.Text = Global.LanguageManager.GetText("MySavedReports");
                }
                else if (this.SelectedDirectoryIsSavedReports && !Guid.TryParse(dirInfo.Name, out guidOutput))
                {
                    lnkExplorerNavigationItem.Text = dirInfo.Name;
                }
                else
                {
                    lnkExplorerNavigationItem.Text = string.Format(Global.LanguageManager.GetText(
                        "UsersSavedReports"),
                        Global.GetNiceUsername(idUser)
                    );
                }
            }

            lnkExplorerNavigationItem.Attributes.Add(
                "Source",
                directory
            );

            pnlExplorerNavigationItem.Controls.Add(lnkExplorerNavigationItem);

            pnlExplorerNavigation.Controls.AddAt(0, pnlExplorerNavigationItem);

            if (dirInfo.Parent.FullName != Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin"
            ))
            {
                RenderExplorerNavigation(dirInfo.Parent.FullName);
            }
            else
            {
                RenderExplorerNavigation(this.RootDirectory);
            }
        }

        private void DeleteDirectory(string directory)
        {
            // Run through all sub directories of the directory.
            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                // Delete the sub directory.
                DeleteDirectory(subDirectory);
            }

            // Delete the directory.
            Directory.Delete(directory, true);

            if (File.Exists(directory + ".LiNK_dirinfo"))
            {
                File.Delete(directory + ".LiNK_dirinfo");
            }
        }


        private bool IsSelectedDirectoryIsSavedReports(string directory)
        {
            return directory.Contains("\\Fileadmin\\SavedReports\\" + Global.Core.ClientName + "\\");
        }

        private bool IsSelectedDirectoryIsSavedReportsRoot(string directory)
        {
            return directory.EndsWith("\\Fileadmin\\SavedReports\\" + Global.Core.ClientName);
        }


        private void RenameReport()
        {
            string DestinationName = Request.Params["Name"];
            string path = Request.Params["Path"];
            bool nameExist = false;
            string directory;
            if (!File.Exists(path))
                return;

            directory = Path.Combine(
                                        Request.PhysicalApplicationPath,
                                        "Fileadmin",
                                        "SavedReports",
                                        Global.Core.ClientName
                                                                      );


            string[] files = Directory.GetFiles(directory, "*Info.xml", SearchOption.AllDirectories);

            foreach (string txtName in files)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(txtName);
                XmlNode node = xml.SelectSingleNode("Info/Name");
                if (node != null)
                {
                    if (node.InnerText == DestinationName)
                    {
                        return;
                    }
                }
            }

            if (nameExist) { return; }
            else
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                XmlNode node = xml.SelectSingleNode("Info/Name");
                node.InnerText = DestinationName;
                xml.Save(path);
            }
        }
        private void PopulateTreeView(DirectoryInfo dirInfo, TreeNode treeNode,string userPath)
        {
            TreeView1.NodeStyle.CssClass = "FileExplorerItem Color1";
            TreeView1.SelectedNodeStyle.CssClass = "FileExplorerItem_Active Color2";


            if (!Directory.Exists(userPath))
            {
                TreeNode directoryNode = new TreeNode
                {
                    Text = "my saved reports",
                    Value = userPath
                };

                directoryNode.NavigateUrl = "javascript: ManualSaveReportFolderSelect(this,'" + directoryNode.Value.Replace("\\", "/") + "'); ";

                TreeView1.Nodes.Add(directoryNode);

            }
            else
            {

                foreach (DirectoryInfo directory in dirInfo.GetDirectories())
            {

                //if (directory.FullName.IndexOf(IdUser.ToString()) == -1)
                //    continue;

                Guid guidOutput = Guid.Empty;
                if (Guid.TryParse(directory.Name, out guidOutput))
                {
                    if (!(directory.Name == IdUser.ToString()))
                    {
                        continue;
                    }
                }

                string Text = directory.Name;
                if (Text == IdUser.ToString())
                {
                    Text = "my saved reports";
                }
                else
                {
                    Text = directory.Name;
                }

                TreeNode directoryNode = new TreeNode
                {
                    Text = Text,
                    Value = directory.FullName
                };

                directoryNode.NavigateUrl = "javascript: ManualSelect(this,'" + directoryNode.Value.Replace("\\", "/") + "'); ";

                if (treeNode == null)
                {
                    //If Root Node, add to TreeView.
                    TreeView1.Nodes.Add(directoryNode);
                }
                else
                {
                    //If Child Node, add to Parent Node.
                    treeNode.ChildNodes.Add(directoryNode);
                }

                //Get all files in the Directory.
                //foreach (FileInfo file in directory.GetFiles())
                //{
                //    //Add each file as Child Node.
                //    TreeNode fileNode = new TreeNode
                //    {
                //        Text = file.Name,
                //        Value = file.FullName,
                //        Target = "_blank",
                //        NavigateUrl = (new Uri(Server.MapPath("~/"))).MakeRelativeUri(new Uri(file.FullName)).ToString()
                //    };
                //    directoryNode.ChildNodes.Add(fileNode);
                //}

                PopulateTreeView(directory, directoryNode, userPath);
            }
            }
        }
        private void updateSelectedtreeNode()
        {
            string destination = Request.Params["destination"];
            HttpContext.Current.Session["moveReportDestination"] = HttpUtility.UrlDecode(Request.Params["destination"]).ToString();
        }
        private void MoveReport()
        {
            string source = Request.Params["source"];
            string dest = HttpUtility.UrlDecode(Request.Params["destination"].ToString());

            if (dest == "null")
            {
                dest = HttpContext.Current.Session["LinkCloudSelectedDirectory"].ToString();
            }

            if (dest == "null" || source == "null")
                return;

            FileAttributes attr = File.GetAttributes(source);
            if (Path.GetDirectoryName(source) != dest)
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    DeepSearch(new DirectoryInfo(source), new DirectoryInfo(dest).CreateSubdirectory(source.Substring(source.LastIndexOf("\\"), source.Length - source.LastIndexOf("\\")).Trim('\\')));

                    //DeepSearch(new DirectoryInfo(source), new DirectoryInfo(dest).CreateSubdirectory(source.Substring(source.LastIndexOf("/"), source.Length - source.LastIndexOf("/")).Trim('/')));
                }
                else
                { //string directory = Path.GetDirectoryName(source);
                    string filename = Path.GetFileNameWithoutExtension(source);
                    string extension = Path.GetExtension(source);
                    dest = Path.Combine(dest
                                    , filename + extension)
                                    .Replace("\\", "/");
                    if (File.Exists(source))
                    {
                        File.Move(source, dest);
                    }
                }
        }

        private void ShowTreeview()
        {
            box1.Visible = true;

            string path = Path.Combine(
                          HttpContext.Current.Request.PhysicalApplicationPath,
                          "Fileadmin",
                          "SavedReports",
                           Global.Core.ClientName
                      //, IdUser.ToString()
                      );
            string userPath= path + "/" + IdUser.ToString();

            DirectoryInfo rootInfo = new DirectoryInfo(path);

            TreeView1.Nodes.Clear();
            // ImageMove.Attributes["source"]
            HttpContext.Current.Session["moveReportSource"] = Request.Params["source"];
            this.PopulateTreeView(rootInfo, null, userPath);


        }

        private void DeepSearch(DirectoryInfo source, DirectoryInfo dest)
        {
            // Recursively call the DeepSearch Method for each Directory
            foreach (DirectoryInfo dir in source.GetDirectories())
                DeepSearch(dir, dest.CreateSubdirectory(dir.Name));

            // Go ahead and copy each file in "source" to the "target" directory
            foreach (FileInfo file in source.GetFiles())
                file.MoveTo(Path.Combine(dest.FullName, file.Name));
            //delete folder with guid
            source.Delete(true);
            if (File.Exists(Directory.GetParent(dest.FullName) + "\\Info.xml"))
            {
                File.Delete(Directory.GetParent(dest.FullName) + "\\Info.xml");
            }
            if (File.Exists(Directory.GetParent(source.FullName) + "\\Info.xml"))
            {
                File.Delete(Directory.GetParent(source.FullName) + "\\Info.xml");
            }
        }

        private void Recursive(string path, string key)
        {
            Guid guidOutput = Guid.Empty;
            if (!Guid.TryParse(Path.GetFileName(path), out guidOutput))
            {
                if (this.SelectedDirectoryIsSavedReports)
                {
                    if (Path.GetFileName(path).ToLower().Contains(key.ToLower()))
                        this.SearchedDirectories.Add(path);
                    string[] directories = Directory.GetDirectories(path);
                    foreach (string dir in directories)
                    {
                        Recursive(dir, key.Trim());
                    }
                }
            }
            else
            {
                string file = Path.Combine(
                        path,
                        "Info.xml");

                ReportDefinitionInfo info = null;
                if (File.Exists(file))
                {
                    info = new ReportDefinitionInfo(file);
                    if (info.Name.ToLower().Contains(key.ToLower()))
                        this.SearchedDirectories.Add(path);
                }
                else
                {
                    if (this.SelectedDirectoryIsSavedReportsRoot)
                    {
                        List<object[]> users = Global.Core.Users.GetValues(
                new string[] { "Id", "FirstName", "LastName" },
                new string[] { },
                new object[] { }
            );
                        // Build the full path to the current users's saved reports directory.
                        string rootDirectory = Path.Combine(
                            Request.PhysicalApplicationPath,
                            "Fileadmin",
                            "SavedReports",
                            Global.Core.ClientName
                        );
                        foreach (object[] user in users)
                        {
                            string directory = Path.Combine(
                    rootDirectory,
                    user[0].ToString()
                );
                            if (!Directory.Exists(directory))
                                continue;

                            string usersSavedReport = string.Format(Global.LanguageManager.GetText(
                           "UsersSavedReports"),
                           Global.GetNiceUsername((Guid)user[0], (string)user[1])
                       );
                            guidOutput = Guid.Empty;
                            if (usersSavedReport.ToLower().Contains(key.ToLower()) && !this.SearchedDirectories.Contains(directory))
                            {
                                if (Guid.TryParse(Path.GetFileName(directory), out guidOutput))
                                    this.SearchedDirectories.Add(directory);
                            }
                        }
                    }
                }
            }
        }

        private void MakeReportPrivate()
        {
            string path = Request.Params["Path"];

            if (path.IndexOf(Global.User.Id.ToString()) != -1)
            {

                string directory;
                if (!File.Exists(path))
                    return;

                FileInfo fInfo = new FileInfo(path);


                directory = Path.GetDirectoryName(path);

                string[] files = Directory.GetFiles(directory, "*Info.xml", SearchOption.AllDirectories);

                foreach (string txtName in files)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(txtName);
                    XmlNode node = xml.SelectSingleNode("Info/Private");
                    if (node != null)
                    {
                        node.InnerXml = "true";
                    }
                    else
                    {

                        XmlElement newNode = xml.CreateElement("Private");
                        newNode.InnerXml = "true";
                        xml.SelectSingleNode("Info").AppendChild(newNode);

                    }
                    xml.Save(path);
                }
            }
        }
        private void MakeReportPublic()
        {
            string path = Request.Params["Path"];

            if (path.IndexOf(Global.User.Id.ToString()) != -1)
            {

                string directory;
                if (!File.Exists(path))
                    return;

                directory = Path.GetDirectoryName(path);

                string[] files = Directory.GetFiles(directory, "*Info.xml", SearchOption.AllDirectories);

                foreach (string txtName in files)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(txtName);
                    XmlNode node = xml.SelectSingleNode("Info/Private");
                    if (node != null)
                    {
                        node.InnerXml = "false";
                    }
                    else
                    {

                        XmlElement newNode = xml.CreateElement("Private");
                        newNode.InnerXml = "false";
                        xml.SelectSingleNode("Info").AppendChild(newNode);

                    }
                    xml.Save(path);
                }
            }
        }
        private void RenameDirectory()
        {
            // Get the new name for the element
            // from the http request's parameters.
            string name = Request.Params["Name"];
            name = name.Trim();
            // Get if the element to rename is a file or a
            // directory from the http request's parameters.
            bool isFile = bool.Parse(Request.Params["IsFile"]);

            // Get the full path to the element in the file
            // system from the http request's parameters.
            string path = Request.Params["Path"];

            if (path.Length > 260)
            {
                base.ShowMessage(Global.LanguageManager.GetText("CreateDirectoryErrorDirectoryFullMaxLength"), WebUtilities.MessageType.Error);

                return;
            }

            if (isFile)
            {
                if (!File.Exists(path))
                    return;

                string destination = Path.Combine(
                    Path.GetDirectoryName(path),
                    name
                ).Replace("\\", "/");

                if (path == destination)
                    return;

                if (File.Exists(destination))
                    return;
                File.Move(path, destination);

                if (File.Exists(path + ".LiNK_fileinfo"))
                {
                    File.Move(path + ".LiNK_fileinfo", destination + ".LiNK_fileinfo");
                }
            }
            else
            {
                if (!Directory.Exists(path))
                    return;

                string destination = Path.Combine(
                    new DirectoryInfo(path).Parent.FullName,
                    name
                ).Replace("\\", "/");

                if (path == destination)
                    return;

                if (Directory.Exists(destination))
                    return;
                Directory.Move(path, destination);

                if (File.Exists(path + ".LiNK_dirinfo"))
                {
                    File.Move(path + ".LiNK_dirinfo", destination + ".LiNK_dirinfo");
                }
            }
        }

        private void DuplicateFile()
        {
            // Get the full path to the element in the file
            // system from the http request's parameters.
            string path = Request.Params["Path"];

            FileInfo fInfo = new FileInfo(path);

            string name = Path.Combine(
                Path.GetDirectoryName(path),
                fInfo.Name.Replace(fInfo.Extension, "") + " - Copy"
            );

            if (File.Exists(name + fInfo.Extension))
            {
                int i = 2;
                while (File.Exists(name + i + fInfo.Extension))
                {
                    i++;
                }

                name += i;
            }

            if (File.Exists(path + ".LiNK_fileinfo"))
            {
                if (File.Exists(name + fInfo.Extension + ".LiNK_fileinfo"))
                    File.Delete(name + fInfo.Extension + ".LiNK_fileinfo");
                File.Copy(path + ".LiNK_fileinfo", name + fInfo.Extension + ".LiNK_fileinfo");
            }

            File.Copy(path, name + fInfo.Extension);

        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Handles the page's load event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The current event's arguments.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            box1.Visible = true;
            string path = Path.Combine(
                          HttpContext.Current.Request.PhysicalApplicationPath,
                          "Fileadmin",
                          "SavedReports",
                          Global.Core.ClientName
                          //,IdUser.ToString()
                      );
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}
            string userPath = path + "/" + IdUser.ToString();
            DirectoryInfo rootInfo = new DirectoryInfo(path);

            if (!IsPostBack)
            {
                TreeView1.Nodes.Clear();
                this.PopulateTreeView(rootInfo, null, userPath);
            }


            this.OrderField = (LibraryOrderField)Enum.Parse(
                typeof(LibraryOrderField),
                Global.UserDefaults["LibraryOrderField", "Name"]
            );
            this.OrderDirection = (LibraryOrderDirection)Enum.Parse(
                typeof(LibraryOrderDirection),
                Global.UserDefaults["LibraryOrderDirection", LibraryOrderDirection.Ascending.ToString()]
            );
            if (Session["LibrarySort"] == null && this.SelectedDirectory != null && this.SelectedDirectoryIsSavedReports)
            {
                Global.UserDefaults["LibraryOrderField", "Name"] = "Type";
                Session["LibrarySort"] = "Type";
                this.OrderField = (LibraryOrderField)Enum.Parse(
                typeof(LibraryOrderField),
                Global.UserDefaults["LibraryOrderField", "Name"]

            );

                this.OrderDirection = LibraryOrderDirection.Ascending;
            }
            if (Request.Params["Method"] != null)
            {
                Response.Clear();

                switch (Request.Params["Method"])
                {
                    case "RenameDirectory":
                        RenameDirectory();
                        break;
                    case "RenameReport":
                        RenameReport();
                        break;
                    case "MoveReport":
                        MoveReport();
                        break;
                    case "ShowTreeview":
                        ShowTreeview();
                        break;
                    case "updateSelectedtreeNode":
                        updateSelectedtreeNode();
                        break;
                    case "MakeReportPrivate":
                        MakeReportPrivate();
                        break;
                    case "MakeReportPublic":
                        MakeReportPublic();
                        break;
                    case "DuplicateFile":
                        DuplicateFile();
                        break;
                    case "SetLibraryViewState":
                        Global.UserDefaults["LibraryViewState", "Grid"] = Request.Params["Value"];
                        break;
                    case "SetLibraryOrderField":
                        if (Global.UserDefaults["LibraryOrderField", "Name"] == Request.Params["Value"])
                        {
                            if (this.OrderDirection == LibraryOrderDirection.Ascending)
                            {
                                Global.UserDefaults["LibraryOrderDirection", LibraryOrderDirection.Ascending.ToString()] = LibraryOrderDirection.Descending.ToString();
                            }
                            else
                            {
                                Global.UserDefaults["LibraryOrderDirection", LibraryOrderDirection.Ascending.ToString()] = LibraryOrderDirection.Ascending.ToString();
                            }
                        }
                        else
                        {
                            Global.UserDefaults["LibraryOrderField", "Name"] = Request.Params["Value"];
                        }
                        break;
                    case "Search":
                        if (this.SelectedDirectory != null && Request.Params["Value"] != null)
                        {
                            Guid guidOutput = Guid.Empty;
                            if (this.SearchedDirectories == null)
                                this.SearchedDirectories = new List<string>();
                            else
                                this.SearchedDirectories.Clear();
                            string[] directroies = Directory.GetDirectories(this.SelectedDirectory);
                            if (Request.Params["Value"] != "")
                                foreach (string dir in directroies)
                                {
                                    Recursive(dir, Request.Params["Value"].Trim());
                                }
                            else
                                this.SearchedDirectories = null;
                            //if (this.SearchedDirectories.Count > 0)
                            {
                                RenderSelectedDirectory();
                                System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                                pnlFiles.RenderControl(htmlWrite);
                                Response.Write(Request.Params["Value"] + "###SPLIT###" + stringWrite.ToString());
                                Response.End();
                            }
                        }
                        break;
                    case "SearchedFile":
                        if (this.SelectedDirectory != null && Request.Params["Path"] != null && Request.Params["Path"] != "")
                        {
                            Guid guidOutput = Guid.Empty;
                            DirectoryInfo dirInfo = new DirectoryInfo(Request.Params["Path"]);

                            HttpContext.Current.Session["LinkCloudSelectedReportUrl"] = Request.Params["Path"].ToString();

                            if (Guid.TryParse(dirInfo.Name, out guidOutput))
                                if (this.SelectedDirectoryIsSavedReports)
                                {
                                    guidOutput = guidOutput = Guid.Empty;
                                    string directoryName = "";
                                    string userId = "";
                                    if (Guid.TryParse(dirInfo.Parent.Name, out guidOutput))
                                    {
                                        directoryName = Path.Combine(
                                              HttpContext.Current.Request.PhysicalApplicationPath,
                                              "Fileadmin",
                                              "Temp",
                                              "OpenSavedReports",
                                              HttpContext.Current.Session.SessionID,
                                              dirInfo.Parent.Name + dirInfo.Name
                                          );
                                        userId = dirInfo.Parent.Name;
                                    }
                                    else
                                    {
                                        //string prevPath = "";
                                        //prevPath = dirInfo.Parent.FullName;
                                        //Directory.GetDirectories(prevPath);
                                        //guidOutput = Guid.Empty;
                                        //while (!Guid.TryParse(Path.GetFileName(prevPath), out guidOutput))
                                        //{
                                        //    guidOutput = Guid.Empty;
                                        //    prevPath = prevPath.Substring(0, prevPath.LastIndexOf("\\"));
                                        //}

                                        string[] allDirectories = dirInfo.Parent.FullName.Split('\\');
                                        string idUser = "";
                                        int i = 0;
                                        while (allDirectories[i] != "SavedReports")
                                        {
                                            i++;
                                        }
                                        idUser = allDirectories[i + 2];

                                        directoryName = Path.Combine(
                                          HttpContext.Current.Request.PhysicalApplicationPath,
                                          "Fileadmin",
                                          "Temp",
                                          "OpenSavedReports",
                                          HttpContext.Current.Session.SessionID,
                                          idUser + dirInfo.Name
                                      );
                                        userId = idUser;
                                    }

                                    if (Directory.Exists(directoryName))
                                    {
                                        Directory.Delete(directoryName, true);
                                    }

                                    Response.Write("/Pages/LinkReporter/Crosstabs.aspx?SavedReport=" + userId + dirInfo.Name);
                                    Response.End();
                                }

                            // Set the selected directory.
                            this.SelectedDirectory = Request.Params["Path"];

                            // Self redirect to diplay the updated values.
                            Response.Redirect(
                                Request.Url.ToString()
                            );
                        }
                        break;
                    case "CopyHyperlinkSuccess":
                        if (Request.Params["Value"] != null && Request.Params["Path"] != null)
                        {
                            Guid idUser = Guid.Parse(Request.Params["Value"].Substring(0, 36));
                            Guid idReport = Guid.Parse(Request.Params["Value"].Substring(36, 36));
                            
                            string linkCloudSelectedReportUrl = "";

                            linkCloudSelectedReportUrl = Path.Combine(Request.Params["Path"].ToString());

                            if (linkCloudSelectedReportUrl != null && linkCloudSelectedReportUrl != "")
                                HttpContext.Current.Session["LinkCloudSelectedReportUrl"] = linkCloudSelectedReportUrl;
                        }
                        break;
                }

                Response.End();

                return;
            }


            DatabaseCore.Items.User user = Global.User;

            pnlGridViewHeadlineItemAuthor.Visible = false;
            pnlGridViewHeadlineItemType.Visible = false;
            pnlGridViewHeadlineItemSize.Visible = false;
            pnlGridViewHeadlineItemCreationDate.Visible = false;
            pnlGridViewHeadlineItemModificationDate.Visible = false;

            if (user.HasPermission(91)) pnlGridViewHeadlineItemAuthor.Visible = true;
            if (user.HasPermission(92)) pnlGridViewHeadlineItemType.Visible = true;
            if (user.HasPermission(93)) pnlGridViewHeadlineItemSize.Visible = true;
            if (user.HasPermission(94)) pnlGridViewHeadlineItemCreationDate.Visible = true;
            if (user.HasPermission(94)) pnlGridViewHeadlineItemModificationDate.Visible = true;
            hdfLibraryViewState.Value = Global.UserDefaults["LibraryViewState", "Grid"];

            // Check if the current request is a delete directory action.
            if (Request.Params["DeleteDirectory"] != null)
            {
                // Delete the directory.
                DeleteDirectory(Request.Params["DeleteDirectory"]);

                Response.Redirect(Request.Url.ToString().Split('?')[0]);
            }

            // Check if the current request is a delete file action.
            if (Request.Params["DeleteFile"] != null)
            {
                // Delete the file.
                File.Delete(Request.Params["DeleteFile"]);

                if (File.Exists(Request.Params["DeleteFile"] + ".LiNK_dirinfo"))
                {
                    File.Delete(Request.Params["DeleteFile"] + ".LiNK_dirinfo");
                }

                Response.Redirect(Request.Url.ToString().Split('?')[0]);
            }

            // Build the tree view to navigate
            // through the client's cloud directories.
            BuildTree();

            RenderSavedReportsDirectory();

            // Render the currently selected directory's items.
            RenderSelectedDirectory();

            // Render the explorer navigation.
            RenderExplorerNavigation(this.SelectedDirectory);


            boxChooseNewAction.Position = WebUtilities.Controls.BoxPosition.Center;
            // TEMP:
            boxChooseNewAction.Visible = false;
            boxCreateDirectory.Visible = false;
            boxCreateFolder.Visible = false;
            boxUploadFile.Visible = false;
            txtSearch.Text = "";
        }


        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
               server control at run time. */
        }
        /// <summary>
        /// Handles the tree view's selected node changed event.
        /// </summary>
        /// <param name="sender">The sending tree view.</param>
        /// <param name="e">The current event's arguments.</param>
        protected void treeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            // Cast the sending object as tree view.
            TreeView treeView = (TreeView)sender;

            // Set the selected directory.
            this.SelectedDirectory = treeView.SelectedValue;

            // Self redirect to diplay the updated values.
            Response.Redirect(
                Request.Url.ToString()
            );
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {

        }
        protected void lnkExplorerNavigationItem_Click(object sender, EventArgs e)
        {
            // Cast the sending object as link button.
            LinkButton lnkExplorerNavigationItem = (LinkButton)sender;

            // Set the selected directory.
            this.SelectedDirectory = lnkExplorerNavigationItem.Attributes["Source"];

            // Self redirect to diplay the updated values.
            Response.Redirect(
                Request.Url.ToString()
            );
        }

        protected void imgDirectory_Click(object sender, ImageClickEventArgs e)
        {
            // Cast the sending object as image button.
            ImageButton imgDirectory = (ImageButton)sender;
            Guid guidOutput = Guid.Empty;
            DirectoryInfo dirInfo = new DirectoryInfo(imgDirectory.Attributes["Source"]);

            HttpContext.Current.Session["LinkCloudSelectedReportUrl"] = imgDirectory.Attributes["Source"].ToString();

            if (Guid.TryParse(dirInfo.Name, out guidOutput))
                if (this.SelectedDirectoryIsSavedReports)
                {
                    guidOutput = guidOutput = Guid.Empty;
                    string directoryName = "";
                    string userId = "";
                    if (Guid.TryParse(dirInfo.Parent.Name, out guidOutput))
                    {
                        directoryName = Path.Combine(
                              HttpContext.Current.Request.PhysicalApplicationPath,
                              "Fileadmin",
                              "Temp",
                              "OpenSavedReports",
                              HttpContext.Current.Session.SessionID,
                              dirInfo.Parent.Name + dirInfo.Name
                          );
                        userId = dirInfo.Parent.Name;
                    }
                    else
                    {
                        //string prevPath = "";
                        //prevPath = dirInfo.Parent.FullName;
                        //Directory.GetDirectories(prevPath);
                        //guidOutput = Guid.Empty;
                        //while (!Guid.TryParse(Path.GetFileName(prevPath), out guidOutput))
                        //{
                        //    guidOutput = Guid.Empty;
                        //    prevPath = prevPath.Substring(0, prevPath.LastIndexOf("\\"));
                        //}

                        string[] allDirectories = dirInfo.Parent.FullName.Split('\\');
                        string idUser = "";
                        int i = 0;
                        while (allDirectories[i] != "SavedReports")
                        {
                            i++;
                        }
                        idUser = allDirectories[i + 2];

                        directoryName = Path.Combine(
                          HttpContext.Current.Request.PhysicalApplicationPath,
                          "Fileadmin",
                          "Temp",
                          "OpenSavedReports",
                          HttpContext.Current.Session.SessionID,
                          idUser + dirInfo.Name
                      );
                        userId = idUser;
                    }

                    if (Directory.Exists(directoryName))
                    {
                        Directory.Delete(directoryName, true);
                    }

                    Response.Redirect("/Pages/LinkReporter/Crosstabs.aspx?SavedReport=" + userId + dirInfo.Name);
                    return;
                }

            // Set the selected directory.
            this.SelectedDirectory = imgDirectory.Attributes["Source"];

            // Self redirect to diplay the updated values.
            Response.Redirect(
                Request.Url.ToString()
            );
        }

        protected void SavedReport_Click(object sender, ImageClickEventArgs e)
        {
            // Cast the sending object as image button.
            ImageButton imgDirectory = (ImageButton)sender;

            string reportDefinitionFileName = Path.Combine(
                Request.PhysicalApplicationPath,
                "Fileadmin",
                "ReportDefinitions",
                Global.Core.ClientName,
                Global.User.Id + ".xml"
            );

            File.Copy(
                imgDirectory.Attributes["Source"],
                reportDefinitionFileName,
                true
            );

            Response.Redirect(
                "/Pages/LinkReporter/Crosstabs.aspx"
            );
        }


        protected void btnCreateDirectory_Click(object sender, EventArgs e)
        {
            boxCreateDirectory.Visible = true;
        }

        protected void ImageMove_Click(object sender, EventArgs e)
        {
            string source = HttpContext.Current.Session["moveReportSource"].ToString();// ImageMove.Attributes["source"];
            source = Directory.GetParent(source).ToString();
            string dest = HttpContext.Current.Session["moveReportDestination"].ToString();
            dest = dest.Replace("/", "//");
            if (dest == "null")
            {
                dest = HttpContext.Current.Session["LinkCloudSelectedDirectory"].ToString();
            }

            if (dest == "null" || source == "null")
                return;

            if (Path.GetFullPath(Directory.GetParent(source).FullName) == Path.GetFullPath(dest))
                return;


            FileAttributes attr = File.GetAttributes(source);
            if (Path.GetDirectoryName(source) != dest)
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    DeepSearch(new DirectoryInfo(source), new DirectoryInfo(dest).CreateSubdirectory(source.Substring(source.LastIndexOf("\\"), source.Length - source.LastIndexOf("\\")).Trim('\\')));

                    //DeepSearch(new DirectoryInfo(source), new DirectoryInfo(dest).CreateSubdirectory(source.Substring(source.LastIndexOf("/"), source.Length - source.LastIndexOf("/")).Trim('/')));
                }
                else
                { //string directory = Path.GetDirectoryName(source);
                    string filename = Path.GetFileNameWithoutExtension(source);
                    string extension = Path.GetExtension(source);
                    dest = Path.Combine(dest
                                    , filename + extension)
                                    .Replace("\\", "/");
                    if (File.Exists(source))
                    {
                        File.Move(source, dest);
                    }
                }

            //set null once moved
            Session["moveReportDestination"] = null;
            Response.Redirect(Request.RawUrl);
        }

        protected void btnCreateDirectoryConfirm_Click(object sender, EventArgs e)
        {
            // Check if a directory name is given.
            if (txtCreateDirectoryName.Text.Trim() == "")
            {


                boxCreateDirectory.ShowError(string.Format(
                    Global.LanguageManager.GetText("CreateDirectoryErrorName")
                ));

                boxCreateDirectory.Visible = true;

                return;
            }

            // Build the full path of the new directory.
            string directory = Path.Combine(
                this.SelectedDirectory,
                txtCreateDirectoryName.Text
            );

            if (txtCreateDirectoryName.Text.Length > 248)
            {
                boxCreateDirectory.ShowError(string.Format(
                    Global.LanguageManager.GetText("CreateDirectoryErrorDirectoryMaxLength")
                ));

                boxCreateDirectory.Visible = true;

                return;
            }

            if (directory.Length > 260)
            {
                boxCreateDirectory.ShowError(string.Format(
                    Global.LanguageManager.GetText("CreateDirectoryErrorDirectoryFullMaxLength")
                ));

                boxCreateDirectory.Visible = true;

                return;
            }

            // Check if the directory already exists.
            if (Directory.Exists(directory))
            {
                boxCreateDirectory.ShowError(string.Format(
                    Global.LanguageManager.GetText("CreateDirectoryErrorAlreadyExists"),
                    txtCreateDirectoryName.Text
                ));

                boxCreateDirectory.Visible = true;

                return;
            }

            // Create the new directory.
            if (txtCreateDirectoryName.Text != "saved reports")
            {
                Directory.CreateDirectory(directory);

                File.WriteAllBytes(
                    directory + ".LiNK_dirinfo",
                    Global.IdUser.Value.ToByteArray()
                );
                // Self redirect to diplay the updated values.
                Response.Redirect(
                    Request.Url.ToString()
                );
            }
            else
            {
                base.ShowMessage(string.Format(Global.LanguageManager.GetText("CreateDirectoryErrorAlreadyExists"),
                                    txtCreateDirectoryName.Text), WebUtilities.MessageType.Error);
            }

        }

        protected void boxCreateFolderConfirm_Click(object sender, EventArgs e)
        {
            // Check if a directory name is given.
            if (txtCreateFolderName.Text.Trim() == "")
            {


                boxCreateFolder.ShowError(string.Format(
                    Global.LanguageManager.GetText("CreateDirectoryErrorName")
                ));

                boxCreateFolder.Visible = true;

                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtCreateFolderName.Text.Trim(), "^[a-zA-Z0-9\x20]+$"))
            {
                boxCreateFolder.ShowError(string.Format(
                    Global.LanguageManager.GetText("CreateDirectorySpecialCharacter")
                ));

                boxCreateFolder.Visible = true;

                return;
            }

            // Build the full path of the new directory.
            string directory = Path.Combine(
                this.SelectedDirectory,
                txtCreateFolderName.Text
            );

            if (txtCreateFolderName.Text.Length > 248)
            {
                boxCreateFolder.ShowError(string.Format(
                    Global.LanguageManager.GetText("CreateDirectoryErrorDirectoryMaxLength")
                ));

                boxCreateFolder.Visible = true;

                return;
            }

            if (directory.Length > 260)
            {
                boxCreateFolder.ShowError(string.Format(
                    Global.LanguageManager.GetText("CreateDirectoryErrorDirectoryFullMaxLength")
                ));

                boxCreateFolder.Visible = true;

                return;
            }
            // Check if the directory already exists.
            if (Directory.Exists(directory))
            {
                boxCreateFolder.ShowError(string.Format(
                    Global.LanguageManager.GetText("CreateDirectoryErrorAlreadyExists"),
                    txtCreateFolderName.Text
                ));

                boxCreateFolder.Visible = true;

                return;
            }

            // Create the new directory.
            if (txtCreateDirectoryName.Text != "saved reports")
            {
                Directory.CreateDirectory(directory);

                // Self redirect to diplay the updated values.
                Response.Redirect(
                    Request.Url.ToString()
                );
            }
            else
            {
                base.ShowMessage(string.Format(Global.LanguageManager.GetText("CreateDirectoryErrorAlreadyExists"),
                                    txtCreateDirectoryName.Text), WebUtilities.MessageType.Error);
            }

        }


        protected void btnUpload_Click(object sender, EventArgs e)
        {
            boxUploadFile.Visible = true;
        }

        protected void btnUploadFileConfirm_Click(object sender, EventArgs e)
        {
            if (!fuUploadFile.HasFile)
            {
                boxUploadFile.ShowError(string.Format(
                    Global.LanguageManager.GetText("SelectAFile")
                ));
                return;
            }


            fuUploadFile.SaveAs(Path.Combine(
                this.SelectedDirectory,
                fuUploadFile.FileName
            ));

            File.WriteAllBytes(Path.Combine(
                this.SelectedDirectory,
                fuUploadFile.FileName + ".LiNK_fileinfo"
            ), Global.IdUser.Value.ToByteArray());

            // Self redirect to diplay the updated values.
            Response.Redirect(
                Request.Url.ToString()
            );
        }


        protected void imgNewDirectory_Click(object sender, ImageClickEventArgs e)
        {
            boxChooseNewAction.Visible = true;
        }

        protected void imgCreateFolder_Click(object sender, ImageClickEventArgs e)
        {
            boxCreateFolder.Visible = true;
            txtCreateFolderName.Text = "";
            txtCreateFolderName.Focus();
            Global.UserDefaults["LibraryOrderField", "Name"] = "Type";
        }

        #endregion
    }

    public enum LibraryOrderField
    {
        Name,
        Type,
        Size,
        CreationDate,
        ModificationDate,
        Author
    }
    public enum LibraryOrderDirection
    {
        Ascending,
        Descending
    }
}