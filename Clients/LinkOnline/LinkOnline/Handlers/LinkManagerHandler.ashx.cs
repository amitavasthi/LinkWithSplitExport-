using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkOnline.Handlers
{
public class LinkManagerHandler : WebUtilities.BaseHandler
{
    #region Properties

    #endregion


    #region Constructor

    public LinkManagerHandler()
        : base(true)
    {
        base.Methods.Add("MethodName", MethodName);
    }

    #endregion


    #region Methods

    #endregion


    #region Event Handlers

    /// <summary>
    /// Sample method description.
    /// </summary>
    /// <param name="context">The current http context.</param>
    private void MethodName(HttpContext context)
    {

    }

    #endregion
}
}