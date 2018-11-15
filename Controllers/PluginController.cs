//-----------------------------------------------------------------------
// <copyright file="PluginController.cs" company="None">
//     Copyright (c) Allow to distribute this code.
// </copyright>
// <author>Asma Khalid</author>
//-----------------------------------------------------------------------

namespace PluginIntegration_1.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using PluginIntegration_1.Models;


    public class ColModel
    {
        public string data { get; set; }        
    }
    /// <summary>
    /// Plugin class.
    /// </summary>
    public class PluginController : Controller
    {
        #region Index method

        /// <summary>
        /// GET: Plugin method.
        /// </summary>
        /// <returns>Returns - index view page</returns> 
        public ActionResult Index()
        {
            // Info.
            return this.View();
        }

        #endregion

        #region Get data method.

        public ActionResult GetData2()
        {
            string text = System.IO.File.ReadAllText(@"Content\files\data.json");

            return this.Json(new { draw = Convert.ToInt32(1), recordsTotal = 57, recordsFiltered = 57, data = text }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GET: /Plugin/GetData
        /// </summary>
        /// <returns>Return data</returns>

        
        public ActionResult GetColumnsDefinition()
        {
            var colModel = new List<ColModel>();
            colModel.Add(new ColModel { data = "Sr" });
            colModel.Add(new ColModel { data = "OrderTrackNumber" });
            colModel.Add(new ColModel { data = "Quantity" });
            colModel.Add(new ColModel { data = "ProductName" });
            colModel.Add(new ColModel { data = "SpecialOffer" });
            colModel.Add(new ColModel { data = "UnitPrice" });
            colModel.Add(new ColModel { data = "UnitPriceDiscount" });           

            return this.Json(new { data = colModel.ToArray() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetData()
        {
            // Initialization.
            JsonResult result = new JsonResult();

            try
            {
                // Initialization.
                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                // Loading.
                List<SalesOrderDetail> data = this.LoadData();

                // Total record count.
                int totalRecords = data.Count;

                // Verification.
                if (!string.IsNullOrEmpty(search) &&
                    !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search
                    data = data.Where(p => p.Sr.ToString().ToLower().Contains(search.ToLower()) ||
                                           p.OrderTrackNumber.ToLower().Contains(search.ToLower()) ||
                                           p.Quantity.ToString().ToLower().Contains(search.ToLower()) ||
                                           p.ProductName.ToLower().Contains(search.ToLower()) ||
                                           p.SpecialOffer.ToLower().Contains(search.ToLower()) ||
                                           p.UnitPrice.ToString().ToLower().Contains(search.ToLower()) ||
                                           p.UnitPriceDiscount.ToString().ToLower().Contains(search.ToLower())).ToList();
                }

                // Sorting.
                data = this.SortByColumnWithOrder(order, orderDir, data);

                // Filter record count.
                int recFilter = data.Count;

                // Apply pagination.
                data = data.Skip(startRec).Take(pageSize).ToList();
                //data = data.ToList();

                // Loading drop down lists.
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Return info.
            return result;
        }

        #endregion

        #region Helpers

        #region Load Data

        /// <summary>
        /// Load data method.
        /// </summary>
        /// <returns>Returns - Data</returns>
        private List<SalesOrderDetail> LoadData()
        {
            // Initialization.
            List<SalesOrderDetail> lst = new List<SalesOrderDetail>();

            try
            {
                // Initialization.
                string line = string.Empty;
                string srcFilePath = "content/files/SalesOrderDetail.txt";
                var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                var fullPath = Path.Combine(rootPath, srcFilePath);
                string filePath = new Uri(fullPath).LocalPath;
                StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));

                // Read file.
                while ((line = sr.ReadLine()) != null)
                {
                    // Initialization.
                    SalesOrderDetail infoObj = new SalesOrderDetail();
                    string[] info = line.Split(',');

                    // Setting.
                    infoObj.Sr = Convert.ToInt32(info[0].ToString());
                    infoObj.OrderTrackNumber = info[1].ToString();
                    infoObj.Quantity = Convert.ToInt32(info[2].ToString());
                    infoObj.ProductName = info[3].ToString();
                    infoObj.SpecialOffer = info[4].ToString();
                    infoObj.UnitPrice = Convert.ToDouble(info[5].ToString());
                    infoObj.UnitPriceDiscount = Convert.ToDouble(info[6].ToString());

                    // Adding.
                    lst.Add(infoObj);
                }

                // Closing.
                sr.Dispose();
                sr.Close();
            }
            catch (Exception ex)
            { 
                // info.
                Console.Write(ex);
            }

            // info.
            return lst;
        }

        #endregion

        #region Sort by column with order method

        /// <summary>
        /// Sort by column with order method.
        /// </summary>
        /// <param name="order">Order parameter</param>
        /// <param name="orderDir">Order direction parameter</param>
        /// <param name="data">Data parameter</param>
        /// <returns>Returns - Data</returns>
        private List<SalesOrderDetail> SortByColumnWithOrder(string order, string orderDir, List<SalesOrderDetail> data)
        {
            // Initialization.
            List<SalesOrderDetail> lst = new List<SalesOrderDetail>();

            try
            {
                // Sorting
                switch (order)
                {
                    case "0":
                        // Setting.
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Sr).ToList()
                                                                                                 : data.OrderBy(p => p.Sr).ToList();
                        break;

                    case "1":
                        // Setting.
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.OrderTrackNumber).ToList()
                                                                                                 : data.OrderBy(p => p.OrderTrackNumber).ToList();
                        break;

                    case "2":
                        // Setting.
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Quantity).ToList()
                                                                                                 : data.OrderBy(p => p.Quantity).ToList();
                        break;

                    case "3":
                        // Setting.
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.ProductName).ToList()
                                                                                                 : data.OrderBy(p => p.ProductName).ToList();
                        break;

                    case "4":
                        // Setting.
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.SpecialOffer).ToList()
                                                                                                   : data.OrderBy(p => p.SpecialOffer).ToList();
                        break;

                    case "5":
                        // Setting.
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.UnitPrice).ToList()
                                                                                                 : data.OrderBy(p => p.UnitPrice).ToList();
                        break;

                    case "6":
                        // Setting.
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.UnitPriceDiscount).ToList()
                                                                                                 : data.OrderBy(p => p.UnitPriceDiscount).ToList();
                        break;

                    default:

                        // Setting.
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Sr).ToList() 
                                                                                                 : data.OrderBy(p => p.Sr).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                // info.
                Console.Write(ex);
            }

            // info.
            return lst;
        }

        #endregion

        #endregion
    }
}