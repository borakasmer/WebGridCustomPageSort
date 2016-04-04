using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TeklifFormu.Models;

namespace TeklifFormu.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        int rowPerPage = 25;
        [HttpGet]
        public JsonResult EfficientPaging(int? page, string sort, string sortdir)
        {
            using (NorthWindContext dbContext = new NorthWindContext())
            {
                sort = sort?.Trim().Replace(" ", "");

                int skip = page.HasValue ? page.Value - 1 : 0;
                var data = new List<Customer>();
                //var data = dbContext.Customers.OrderBy(o => o.CustomerID).Skip(skip * rowPerPage).Take(rowPerPage).ToList();
                if (sort != null && sort.Trim() != "")
                    data = dbContext.Customers.SqlQuery("select * from Customers order by " + sort + " " + sortdir + " OFFSET " + (skip * rowPerPage) + " ROWS FETCH NEXT " + rowPerPage + " ROWS ONLY").ToList();
                else
                    data = dbContext.Customers.OrderBy(o => o.CustomerID).Skip(skip * rowPerPage).Take(rowPerPage).ToList();

                var grid = new WebGrid(data, canPage: false, canSort: false, rowsPerPage: rowPerPage);
                var htmlString = grid.GetHtml(tableStyle: "webGrid",
             
                

                headerStyle: "webgrid-header",
                footerStyle: "webgrid-footer",
                alternatingRowStyle: "webgrid-alternating-row",
                selectedRowStyle: "webgrid-selected-row",
                rowStyle: "webgrid-row-style",
                htmlAttributes: new { id = "grid" },
                columns: grid.Columns(
                    //grid.Column("CustomerID", "ID"),
                    grid.Column(
                    header: "Siparisler",
                    format: (item) => new HtmlString("<input type='button' id='btnDetail' value='Detay' onclick=\"getDetail('" + item.CustomerID + "')\" />")),                
                    grid.Column("CustomerID", "CustomerID"),
                    grid.Column("CompanyName", "Company Name"),
                    grid.Column("ContactName", "Contact Name"),
                    grid.Column("Address"),
                    grid.Column("City"),
                    grid.Column("Country"),
                    grid.Column("Phone"))
                    );
                // Eger bir satirdaki row count toplam satir sayisi ile bolumunda tam sayi ise direk pagecount alinir degil ise 1 arttirilir.
                var _count = dbContext.Customers.Count() % rowPerPage == 0 ? dbContext.Customers.Count() / rowPerPage : (dbContext.Customers.Count() / rowPerPage) + 1;
                return Json(new
                {
                    Data = htmlString.ToHtmlString(),
                    Count = _count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EfficientSorting(string sort, string sortdir, string presort)
        {
            using (NorthWindContext dbContext = new NorthWindContext())
            {
                List<Customer> data;
                sort = sort.Trim().Replace(" ", "");
                presort = presort.Trim().Replace(" ", "");
                if (sortdir == "desc" && presort != sort) { sortdir = "asc"; }

                //var propertyInfo = typeof(Customer).GetProperty(sort);
                //data = dbContext.Customers.ToList();
                data = dbContext.Customers.SqlQuery("select top " + rowPerPage + " * from Customers order by " + sort + " " + sortdir).ToList();
                /*
                if (sortdir == "ascending")
                    //data = dbContext.Customers.OrderBy(cus => propertyInfo.GetValue(cus, null)).Take(rowPerPage).ToList();
                    data = data.OrderBy(cus => propertyInfo.GetValue(cus, null)).Take(rowPerPage).ToList();
                else
                    //data = dbContext.Customers.OrderByDescending(cus => propertyInfo.GetValue(cus, null)).Take(rowPerPage).ToList();
                    data = data.OrderByDescending(cus => propertyInfo.GetValue(cus, null)).Take(rowPerPage).ToList();
                */
                var grid = new WebGrid(data, canPage: false, canSort: false, rowsPerPage: rowPerPage);
                var htmlString = grid.GetHtml(tableStyle: "webGrid",

                headerStyle: "webgrid-header",
                footerStyle: "webgrid-footer",
                alternatingRowStyle: "webgrid-alternating-row",
                selectedRowStyle: "webgrid-selected-row",
                rowStyle: "webgrid-row-style",
                htmlAttributes: new { id = "grid" },
                columns: grid.Columns(
                    grid.Column(
                    header: "Siparisler",
                    format: (item) => new HtmlString("<input type='button' id='btnDetail' value='Detay' onclick=\"getDetail('" + item.CustomerID + "')\" />")),
                    //grid.Column("CustomerID", "ID"),
                    grid.Column("CustomerID", "CustomerID"),
                    grid.Column("CompanyName", "Company Name"),
                    grid.Column("ContactName", "Contact Name"),
                    grid.Column("Address"),
                    grid.Column("City"),
                    grid.Column("Country"),
                    grid.Column("Phone"))
                    );

                // Eger bir satirdaki row count toplam satir sayisi ile bolumunda tam sayi ise direk pagecount alinir degil ise 1 arttirilir.
                var _count = dbContext.Customers.Count() % rowPerPage == 0 ? dbContext.Customers.Count() / rowPerPage : (dbContext.Customers.Count() / rowPerPage) + 1;

                return Json(new
                {
                    Data = htmlString.ToHtmlString(),
                    Count = _count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /*private static object GetPropertyValue(object obj, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }*/

        public ActionResult GenerateExcel()
        {
            return View();
        }

        public ActionResult Order(string CustomerID)
        {
            using (NorthWindContext dbContext = new NorthWindContext())
            {
                var model = dbContext.Orders.Where(or => or.CustomerID == CustomerID).OrderByDescending(or=>or.Freight).ToList();
                return View(model);
            }
        }
    }
}