using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Business.AAA.Core;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;
using OfficeOpenXml;

namespace Inventory_AAA.Controllers
{
    public class ReportController : Controller
    {
        private readonly IProductServices _productServices;
        public ReportController(
            IProductServices productServices)
        {
            this._productServices = productServices;
        }

        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GenerateSalesReport()
        {
            return View();
        }

        #region Private methods
        private FileResult SalesReportGeneration(DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            
            ds = _productServices.SalesReport(startDate, endDate);

            int rowId = 0;
            int colId = 0;
            int maxColCount = 0;
            var fileNameGenerated = string.Format("{0}_{1}{2}", LookupKey.ReportFileName.SalesReport, "MMddyyyy", ".xlsx");
            
            var contentType = "application/vnd.ms-excel";

            //var templateFile = new FileInfo(path);
            //var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);

            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add(LookupKey.ReportFileName.SalesReport);

            workSheet.Cells["A1"].Value = ds.Tables[0];
            workSheet.Cells["A2"].Value = ds.Tables[1];
            workSheet.Cells["A3"].Value = ds.Tables[2];
            

            colId = 5;
            DataTable dtReportColumns = new DataTable();
            dtReportColumns = ds.Tables[4];
            //var char = ((char)65).ToString();
            for (int i = 0; i < dtReportColumns.Columns.Count - 1; i++)
            {

                workSheet.Cells[((char)i).ToString() + colId].Value = dtReportColumns.Rows[0][i].ToString();
                workSheet.Cells["A" + colId].Value = dtReportColumns.Rows[0][i].ToString();
            }


            rowId = 6;
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {

                //workSheet.Cells["A" + rowId].Value = dt.Rows[i][Constants.DateValue].ToString();
                //workSheet.Cells["B" + rowId].Value = dt.Rows[i][Constants.TotalAmount].ToString();


                rowId++;

            }


            var memoryStream = new MemoryStream();
            //package.Save();
            package.SaveAs(memoryStream);
            memoryStream.Position = 0;

            return File(memoryStream, contentType, fileNameGenerated);
        }
        #endregion
    }
}