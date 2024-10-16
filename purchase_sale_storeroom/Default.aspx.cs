using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace purchase_sale_storeroom
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpBrowserCapabilities hbc = Request.Browser;
            Response.Write(hbc.Browser.ToString() + "<br/>"); //取得瀏覽器名稱
            Response.Write(hbc.Version.ToString() + "<br/>"); //取得瀏覽器版本號
            Response.Write(hbc.Platform.ToString() + "<br/>");     //取得作業系統名稱

        }
    }
}