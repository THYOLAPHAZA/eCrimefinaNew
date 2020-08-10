using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

public partial class Analytics : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        lblCurDate.InnerHtml = String.Format("{0:dd MMMM, yyyy}", DateTime.Now);
        mainlogo.InnerHtml = Class1.LogoText;
        title.InnerHtml = Class1.MainHeaderText;
        // --> Checking Cookie
        HttpCookie ckType = HttpContext.Current.Request.Cookies["ckUserType"];
        String UserType = "";
        if (ckType != null)
            UserType = ckType.Value.ToString();
        linksmenu.InnerHtml = Class1.CreateMenu(UserType);
        HttpCookie ckName = HttpContext.Current.Request.Cookies["ckUserName"];
        String UserName = "";
        if (ckName != null && ckName.Value.ToString() != "")
            UserName = ckName.Value.ToString() + "." + " (" + ckType.Value.ToString() + ")";
        lblusername.InnerHtml = UserName;
        footer.InnerHtml = Class1.FooterText;
        if (Class1.sUserName() == "")
            Response.Redirect("Login.aspx");

        // Bind Charts  
        Literal0.Text = "<script type='text/javascript'>google.load('visualization', '1', { packages:['corechart']}); </script>";
        ltScripts.Text=BindChart("piechart_3d", "select FirType,count(*) as FirCount from FirLogDetails group by FirType", "FIR by Type","2");
        ltScripts2.Text=BindChart("piechart_3d2", "select LastStatusUpdStatus,count(*) as FirCount from FirLogDetails group by LastStatusUpdStatus", "FIR by Status","3");

    }



    private String BindChart(String id, String query, String name, String num)
    {
        DataTable dsChartData = new DataTable();
        StringBuilder strScript = new StringBuilder();

        try
        {
            dsChartData = GetChartData(query);
            string v = "<script type = 'text/javascript'> function drawChart"+num+"() {var data"+num+" = google.visualization.arrayToDataTable([[";

        strScript.Append(@v);

            
            foreach (DataColumn row in dsChartData.Columns)
            {
                strScript.Append("'" + row.ColumnName + "',");
            }
            strScript.Remove(strScript.Length - 1, 1);
            strScript.Append("],");
            foreach (DataRow row in dsChartData.Rows)
            {
                strScript.Append("['" + row.ItemArray[0] + "'," + row.ItemArray[1] + "],");
            }
            strScript.Remove(strScript.Length - 1, 1);
            strScript.Append("]);");

            String y = "var options"+num+" = {title: '"+name+"',is3D: true}; ";
            strScript.Append(@y);

            String x = "var chart"+num+" = new google.visualization.PieChart(document.getElementById('"+id+"'));" +
                "chart"+num+".draw(data"+num+", options"+num+");" +
                "}" +
                "google.setOnLoadCallback(drawChart"+num+");";

            strScript.Append(@x);
            strScript.Append(" </script>");

        }
        catch
        {
            int i = 0;
        }
        finally
        {
            dsChartData.Dispose();
            //strScript.Clear();
        }
        return strScript.ToString();
    }

    private DataTable GetChartData(String query)
    {
        DataSet dsData = new DataSet();
        DataTable dt = new DataTable();
        try
        {
            SqlConnection sqlCon = new SqlConnection();
            Class1.OpenConn(sqlCon);
            SqlCommand Cmd = new SqlCommand(query, sqlCon);
            
            SqlDataReader Dr = Cmd.ExecuteReader();
            
            dt.Load(Dr);

            sqlCon.Close();
        }
        catch
        {
            throw;
        }
        return dt;
    }
}
