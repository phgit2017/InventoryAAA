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
using Infrastructure.Utilities;
using OfficeOpenXml;

namespace Inventory_AAA.Controllers
{
    public class ReportController : Controller
    {
        private readonly IProductServices _productServices;
        private readonly IUserServices _userServices;
        public ReportController(
            IProductServices productServices,
            IUserServices userServices)
        {
            this._productServices = productServices;
            this._userServices = userServices;
        }

        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ReportIndex()
        {
            #region Authorize
            var authorizeMenuAccessResult = AuthorizeMenuAccess(LookupKey.Menu.ReportMenuId);
            if (!authorizeMenuAccessResult.IsSuccess)
            {

                return Json(new
                {
                    isSuccess = authorizeMenuAccessResult.IsSuccess,
                    messageAlert = authorizeMenuAccessResult.MessageAlert
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            var response = new
            {
                isSuccess = true,
                messageAlert = string.Empty
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateSalesReport(DateTime startDate, DateTime endDate)
        {
            var salesReportGenerationFile = SalesReportGeneration(startDate, endDate);
            return salesReportGenerationFile;
        }

        public AuthorizationDetail AuthorizeMenuAccess(int menuId)
        {

            AuthorizationDetail response = new AuthorizationDetail();
            long userSessionId = 0;

            try
            {
                userSessionId = Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);

                if (userSessionId == 0)
                {
                    response.MessageAlert = Messages.SessionUnavailable;
                    return response;
                }
            }
            catch (Exception)
            {
                response.MessageAlert = Messages.SessionUnavailable;
                return response;
            }


            var userId = userSessionId;
            var userResult = _userServices.GetAllUserDetails().Where(m => m.UserId == userId).FirstOrDefault();

            #region Menu Role
            var userMenuRoleResult = _userServices.GetAllUserMenuRoleDetails().Where(m => m.MenuId == menuId
                                                        && m.RoleId == userResult.UserRoleId).FirstOrDefault();

            if (userMenuRoleResult.IsNull())
            {
                response.MessageAlert = Messages.UnauthorizeAccess;
            }
            #endregion


            if (string.IsNullOrEmpty(response.MessageAlert))
                response.IsSuccess = true;


            return response;

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