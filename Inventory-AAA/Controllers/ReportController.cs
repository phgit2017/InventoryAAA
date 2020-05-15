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

        public ActionResult GenerateSalesReport(DateTime startDate, DateTime endDate)
        {
            var salesReportGenerationFile = SalesReportGeneration(startDate, endDate);
            return salesReportGenerationFile;
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
            var fileNameGenerated = string.Format("{0}_{1}{2}", LookupKey.ReportFileName.SalesReport, DateTime.Now.ToString("MMddyyyy"), ".xlsx");

            var contentType = "application/vnd.ms-excel";

            //var templateFile = new FileInfo(path);
            //var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add(LookupKey.ReportFileName.SalesReport);

            workSheet.Cells["A1"].Value = ds.Tables[0].Rows[0][0].ToString();
            workSheet.Cells["B1"].Value = ds.Tables[0].Rows[0][1].ToString();

            workSheet.Cells["A2"].Value = ds.Tables[1].Rows[0][0].ToString();
            workSheet.Cells["B2"].Value = ds.Tables[1].Rows[0][1].ToString();

            workSheet.Cells["A3"].Value = ds.Tables[2].Rows[0][0].ToString();
            workSheet.Cells["B3"].Value = ds.Tables[2].Rows[0][1].ToString();


            rowId = 5;
            DataTable dtReportColumns = new DataTable();
            dtReportColumns = ds.Tables[4];
            for (int i = 0; i <= dtReportColumns.Columns.Count - 1; i++)
            {
                colId = i + 1;
                workSheet.Cells[rowId, colId].Value = dtReportColumns.Rows[0][i].ToString();
            }


            rowId = 6;
            DataTable dtReportRows = new DataTable();
            dtReportRows = ds.Tables[5];
            for (int i = 0; i <= dtReportRows.Rows.Count - 1; i++)
            {
                for (int iCol = 0; iCol <= dtReportRows.Columns.Count - 1; iCol++)
                {
                    colId = iCol + 1;
                    workSheet.Cells[rowId, colId].Value = dtReportRows.Rows[i][iCol].ToString();
                }
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