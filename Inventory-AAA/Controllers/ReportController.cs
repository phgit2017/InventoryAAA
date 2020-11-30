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
        private readonly IOrderServices _orderServices;
        public ReportController(
            IProductServices productServices,
            IUserServices userServices,
            IOrderServices orderServices)
        {
            this._productServices = productServices;
            this._userServices = userServices;
            this._orderServices = orderServices;
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

        public ActionResult GeneratePurchaseAndSalesReport(DateTime startDate, DateTime endDate)
        {
            var purchaseAndSalesReportGenerationFile = PurchaseAndSalesReportGeneration(startDate, endDate);
            return purchaseAndSalesReportGenerationFile;
        }

        public ActionResult GenerateSalesOrder(int reportSalesType, string salesNo = "", long customerId = 0, long salesOrderId = 0, long categoryId = 0,
            DateTime? startDate = null, DateTime? endDate = null,
            int salesOrderStatusId = 0)
        {
            var request = new SalesOrderReportRequest()
            {
                ReportSalesType = reportSalesType,
                SalesNo = salesNo,
                CustomerId = customerId,
                SalesOrderId = salesOrderId,
                CategoryId = categoryId,
                DateFrom = startDate,
                DateTo = endDate,
                SalesOrderStatusId = salesOrderStatusId
            };
            var salesOrderReportGeneration = SalesOrderReportGeneration(request);
            return salesOrderReportGeneration;
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

            workSheet.Cells["A4"].Value = ds.Tables[3].Rows[0][0].ToString();
            workSheet.Cells["B4"].Value = ds.Tables[3].Rows[0][1].ToString();


            rowId = 6;
            DataTable dtReportColumns = new DataTable();
            dtReportColumns = ds.Tables[5];
            for (int i = 0; i <= dtReportColumns.Columns.Count - 1; i++)
            {
                colId = i + 1;
                workSheet.Cells[rowId, colId].Value = dtReportColumns.Rows[0][i].ToString();
            }


            rowId = 7;
            DataTable dtReportRows = new DataTable();
            dtReportRows = ds.Tables[6];
            for (int i = 0; i <= dtReportRows.Rows.Count - 1; i++)
            {
                for (int iCol = 0; iCol <= dtReportRows.Columns.Count - 1; iCol++)
                {
                    colId = iCol + 1;
                    workSheet.Cells[rowId, colId].Value = dtReportRows.Rows[i][iCol].ToString();
                }
                rowId++;
            }

            workSheet.Cells.AutoFitColumns();
            var memoryStream = new MemoryStream();
            //package.Save();
            package.SaveAs(memoryStream);
            memoryStream.Position = 0;

            return File(memoryStream, contentType, fileNameGenerated);
        }

        private FileResult PurchaseAndSalesReportGeneration(DateTime startDate, DateTime endDate)
        {
            List<PurchaseAndReportDetail> purchaseAndReportDetails = new List<PurchaseAndReportDetail>();
            List<ProductDetail> productDetails = new List<ProductDetail>();

            productDetails = _productServices.GetAll().Where(p => p.IsActive).ToList();

            if (productDetails.Count > 0)
            {
                purchaseAndReportDetails = CommonExtensions.ConvertDataTable<PurchaseAndReportDetail>
                                                        (_productServices.PurchaseandSalesReport(startDate, endDate));

            }


            int rowId = 0;
            var fileNameGenerated = string.Format("{0}_{1}{2}", LookupKey.ReportFileName.PurchaseAndSalesReport,
                                                                DateTime.Now.ToString("MMddyyyy"),
                                                                ".xlsx");

            var contentType = "application/vnd.ms-excel";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add(LookupKey.ReportFileName.PurchaseAndSalesReport);

            rowId = 0;
            if (productDetails.Count > 0)
            {
                foreach (var detail in productDetails)
                {
                    var purchaseAndReportResult = purchaseAndReportDetails.Where(m => m.ProductID == detail.ProductId);
                    if (purchaseAndReportResult.ToList().Count > 0)
                    {
                        rowId = rowId + 1;
                        workSheet.Cells[rowId, 1].Value = detail.ProductCode.ToUpper() + " " + detail.ProductDescription.ToString();
                        workSheet.Cells[rowId, 1].Style.Font.Bold = true;

                        rowId = rowId + 1;
                        workSheet.Cells[rowId, 1].Value = "PREVIOUS PURCHASE QTY";
                        workSheet.Cells[rowId, 2].Value = "PURCHASE QTY";
                        workSheet.Cells[rowId, 3].Value = "PREVIOUS SALES QTY";
                        workSheet.Cells[rowId, 4].Value = "SALES QTY";
                        workSheet.Cells[rowId, 5].Value = "PREVIOUS CORRECTION QTY";
                        workSheet.Cells[rowId, 6].Value = "CORRECTION QTY";
                        workSheet.Cells[rowId, 7].Value = "TRANSACTION TYPE";
                        workSheet.Cells[rowId, 8].Value = "TRANSACTION DATE";
                        workSheet.Cells[rowId, 9].Value = "CREATED BY";
                        workSheet.Cells[rowId, 10].Value = "REMARKS";


                        foreach (var item in purchaseAndReportResult)
                        {
                            rowId = rowId + 1;
                            workSheet.Cells[rowId, 1].Value = Convert.ToInt64(item.PreviousPurchaseQty);
                            workSheet.Cells[rowId, 2].Value = Convert.ToInt64(item.PurchaseQty);
                            workSheet.Cells[rowId, 3].Value = Convert.ToInt64(item.PreviousSalesQty);
                            workSheet.Cells[rowId, 4].Value = Convert.ToInt64(item.SalesQty);
                            workSheet.Cells[rowId, 5].Value = Convert.ToInt64(item.PreviousCorrectionQty);
                            workSheet.Cells[rowId, 6].Value = Convert.ToInt64(item.CorrectionQty);
                            workSheet.Cells[rowId, 7].Value = item.TransactionType;
                            workSheet.Cells[rowId, 8].Value = item.TransactionDate.ToString("MM/dd/yyyy hh:mm");
                            workSheet.Cells[rowId, 9].Value = item.CreatedBy;
                            workSheet.Cells[rowId, 10].Value = item.Remarks;
                        }
                    }
                }
            }

            workSheet.Cells.AutoFitColumns();


            var memoryStream = new MemoryStream();
            //package.Save();
            package.SaveAs(memoryStream);
            memoryStream.Position = 0;

            return File(memoryStream, contentType, fileNameGenerated);
        }

        private FileResult SalesOrderReportGeneration(SalesOrderReportRequest request)
        {
            DataTable dt = new DataTable();

            switch (request.ReportSalesType)
            {
                case 1:
                    dt = _productServices.SalesReportPerSalesNo(request.SalesNo, 0);
                    break;
                case 2:
                    dt = _productServices.SalesReportPerCustomerId(request.DateFrom, request.DateTo, request.CustomerId, request.SalesOrderStatusId);
                    break;
                case 3:
                    dt = _productServices.SalesReportPerCategoryAndDate(request.DateFrom, request.DateTo, 0, request.SalesOrderStatusId);
                    break;
                case 4:
                    dt = _productServices.SalesReportPerCategoryAndDate(request.DateFrom, request.DateTo, request.CategoryId, request.SalesOrderStatusId);
                    break;
                default:
                    break;
            }

            int rowId = 0;
            var fileNameGenerated = string.Format("{0}_{1}{2}", LookupKey.ReportFileName.SalesReport, DateTime.Now.ToString("MMddyyyy"), ".xlsx");

            var contentType = "application/vnd.ms-excel";

            //var templateFile = new FileInfo(path);
            //var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add(LookupKey.ReportFileName.SalesReport);

            if (dt.Rows.Count <= 0)
            {
                rowId = 1;
                workSheet.Cells[rowId, 1].Value = "No Result(s) Found";
            }
            else
            {
                if (request.ReportSalesType == 1)
                {
                    rowId = 1;
                    workSheet.Cells[rowId, 1].Value = "Sales No";
                    workSheet.Cells[rowId, 2].Value = dt.Rows[0]["SalesNo"].ToString();

                    workSheet.Cells[rowId, 4].Value = "Mode of Payment";
                    workSheet.Cells[rowId, 5].Value = dt.Rows[0]["ModeOfPayment"].ToString();

                    rowId = 2;
                    workSheet.Cells[rowId, 1].Value = "Customer";
                    workSheet.Cells[rowId, 2].Value = dt.Rows[0]["CustomerFullDetails"].ToString();

                    workSheet.Cells[rowId, 4].Value = "Address";
                    workSheet.Cells[rowId, 5].Value = dt.Rows[0]["FullAddress"].ToString();


                    rowId = 5;
                    workSheet.Cells[rowId, 1].Value = "PRODUCT CODE";
                    workSheet.Cells[rowId, 2].Value = "PRODUCT DESCRIPTION";
                    workSheet.Cells[rowId, 3].Value = "SALES QUANTITY";
                    workSheet.Cells[rowId, 4].Value = "PRICE";
                    workSheet.Cells[rowId, 5].Value = "SUBTOTAL";



                    rowId = rowId + 1;
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {

                        workSheet.Cells[rowId, 1].Value = dt.Rows[i]["ProductCode"].ToString();
                        workSheet.Cells[rowId, 2].Value = dt.Rows[i]["ProductDescription"].ToString();
                        workSheet.Cells[rowId, 3].Value = Convert.ToInt64(dt.Rows[i]["Quantity"].ToString());
                        workSheet.Cells[rowId, 4].Value = Convert.ToDecimal(dt.Rows[i]["UnitPrice"]).ToString("N");
                        workSheet.Cells[rowId, 5].Value = Convert.ToDecimal(dt.Rows[i]["Subtotal"]).ToString("N");
                        rowId = rowId + 1;
                    }

                    rowId = rowId + 2;
                    workSheet.Cells[rowId, 4].Value = "Shipping Fee";
                    workSheet.Cells[rowId, 5].Value = string.Format("{0:#,0.00}", Convert.ToDecimal(dt.Rows[0]["ShippingFee"]));
                    rowId = rowId + 1;
                    workSheet.Cells[rowId, 4].Value = "Total";
                    workSheet.Cells[rowId, 5].Value = string.Format("{0:#,0.00}", Convert.ToDecimal(dt.Rows[0]["TotalAmount"]));

                    workSheet.Cells.AutoFitColumns();
                }
                else if (request.ReportSalesType == 3 || request.ReportSalesType == 4)
                {
                    rowId = 1;
                    workSheet.Cells[rowId, 1].Value = "SALES NUMBER";
                    workSheet.Cells[rowId, 2].Value = "CUSTOMER";
                    workSheet.Cells[rowId, 3].Value = "PRODUCT CODE";
                    workSheet.Cells[rowId, 4].Value = "PRODUCT DESCRIPTION";
                    workSheet.Cells[rowId, 5].Value = "CATEGORY NAME";
                    workSheet.Cells[rowId, 6].Value = "PREVIOUS QUANTITY";
                    workSheet.Cells[rowId, 7].Value = "SALES QUANTITY";
                    workSheet.Cells[rowId, 8].Value = "PRICE";
                    workSheet.Cells[rowId, 9].Value = "TRANSACTION DATE";
                    workSheet.Cells[rowId, 10].Value = "CREATED BY";

                    rowId = rowId + 1;
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        workSheet.Cells[rowId, 1].Value = dt.Rows[i]["SalesNo"] == null ? "" : "'" + dt.Rows[i]["SalesNo"].ToString();
                        workSheet.Cells[rowId, 2].Value = dt.Rows[i]["CustomerFullDetails"].ToString();
                        workSheet.Cells[rowId, 3].Value = dt.Rows[i]["ProductCode"].ToString();
                        workSheet.Cells[rowId, 4].Value = dt.Rows[i]["ProductDescription"].ToString();
                        workSheet.Cells[rowId, 5].Value = dt.Rows[i]["CategoryName"].ToString();
                        workSheet.Cells[rowId, 6].Value = Convert.ToInt64(dt.Rows[i]["PreviousQuantity"]);
                        workSheet.Cells[rowId, 7].Value = Convert.ToInt64(dt.Rows[i]["Quantity"]);
                        workSheet.Cells[rowId, 8].Value = Convert.ToDecimal(dt.Rows[i]["UnitPrice"].ToString()).ToString("N");
                        workSheet.Cells[rowId, 9].Value = dt.Rows[i]["TransactionTime"].ToString();
                        workSheet.Cells[rowId, 10].Value = dt.Rows[i]["UserFullName"].ToString();
                        rowId = rowId + 1;
                    }
                }
                else
                {
                    rowId = 1;
                    workSheet.Cells[rowId, 1].Value = "SALES NUMBER";
                    workSheet.Cells[rowId, 2].Value = "CUSTOMER";
                    workSheet.Cells[rowId, 3].Value = "PRODUCT CODE";
                    workSheet.Cells[rowId, 4].Value = "PRODUCT DESCRIPTION";
                    workSheet.Cells[rowId, 5].Value = "CATEGORY NAME";
                    workSheet.Cells[rowId, 6].Value = "PREVIOUS QUANTITY";
                    workSheet.Cells[rowId, 7].Value = "SALES QUANTITY";
                    workSheet.Cells[rowId, 8].Value = "PRICE";
                    workSheet.Cells[rowId, 9].Value = "TRANSACTION DATE";
                    workSheet.Cells[rowId, 10].Value = "CREATED BY";

                    rowId = rowId + 1;
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        workSheet.Cells[rowId, 1].Value = dt.Rows[i]["SalesNo"] == null ? "" : "'" + dt.Rows[i]["SalesNo"].ToString();
                        workSheet.Cells[rowId, 2].Value = dt.Rows[i]["CustomerFullDetails"].ToString();
                        workSheet.Cells[rowId, 3].Value = dt.Rows[i]["ProductCode"].ToString();
                        workSheet.Cells[rowId, 4].Value = dt.Rows[i]["ProductDescription"].ToString();
                        workSheet.Cells[rowId, 5].Value = dt.Rows[i]["CategoryName"].ToString();
                        workSheet.Cells[rowId, 6].Value = Convert.ToInt64(dt.Rows[i]["PreviousQuantity"]);
                        workSheet.Cells[rowId, 7].Value = Convert.ToInt64(dt.Rows[i]["Quantity"]);
                        workSheet.Cells[rowId, 8].Value = Convert.ToDecimal(dt.Rows[i]["UnitPrice"].ToString()).ToString("N");
                        workSheet.Cells[rowId, 9].Value = dt.Rows[i]["TransactionTime"].ToString();
                        workSheet.Cells[rowId, 10].Value = dt.Rows[i]["UserFullName"].ToString();
                        rowId = rowId + 1;
                    }
                }
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