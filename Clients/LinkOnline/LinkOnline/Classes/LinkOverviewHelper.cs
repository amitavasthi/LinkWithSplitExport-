using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkOnline.Classes
{
    public class LinkOverviewHelper
    {
        public int linkedVariables { get; set; }
        public int unLinkedVariables { get; set; }
        public int linkedCategories { get; set; }
        public int unLinkedCategories { get; set; }
        public int studies { get; set; }
        public int projectDeatils { get; set; }
    }
    public class RootObject
    {
        public List<LinkOverviewHelper> values { get; set; }
    }

    public class StudyRepondents
    {
        public string study { get; set; }
        public int responseCount { get; set; }
    }

    public class RootRespondents
    {
        public List<StudyRepondents> values { get; set; }
    }
}