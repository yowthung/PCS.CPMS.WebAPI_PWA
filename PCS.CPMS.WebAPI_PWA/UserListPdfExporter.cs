using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PCS.CPMS.BL.Models;
using PCS.CPMS.BL.OthersModel;
using PCS.CPMS.DAL;



namespace PCS.CPMS.WebAPI_PWA
{
    public class UserListPdfExporter : ITransientDependency
    {
        private readonly tng_cpmsContext _db;
        public static ReceiptModel receipt;
        // private readonly IRepository<User, long> _userRepository;
        private readonly IConverter _converter;
        public UserListPdfExporter(tng_cpmsContext context, IConverter converter)
        {
            _db = context;
            _converter = converter;
        }

        public UserListPdfExporter()
        {
        }

        public async Task<FileDto> GetUsersAsPdfAsync([FromForm] int invoiceid)
        {
            // var users = await _userRepository.GetAllListAsync();
            var html = ""; //ConvertUserListToHtmlTable(invoiceid);
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = html
                    }
                }
            };
            return new FileDto("UserList.pdf", _converter.Convert(doc));
        }
        //private string ConvertUserListToHtmlTable(List<User> users)
        //{
        //    var header1 = "<th>Username</th>";
        //    var header2 = "<th>Name</th>";
        //    var header3 = "<th>Surname</th>";
        //    var header4 = "<th>Email Address</th>";
        //    var headers = $"<tr>{header1}{header2}{header3}{header4}</tr>";
        //    var rows = new StringBuilder();
        //    foreach (var user in users)
        //    {
        //        var column1 = $"<td>{user.UserName}</td>";
        //        var column2 = $"<td>{user.Name}</td>";
        //        var column3 = $"<td>{user.Surname}</td>";
        //        var column4 = $"<td>{user.EmailAddress}</td>";
        //        var row = $"<tr>{column1}{column2}{column3}{column4}</tr>";
        //        rows.Append(row);
        //    }
        //    return $"<table>{headers}{rows.ToString()}</table>";
        //}
        public void GetReceiptData(int invoiceId)
        {
            List<SeasonInvoice> invoiceList = _db.SeasonInvoice.Where(x => x.Id == invoiceId).ToList();
            if (invoiceList.Count > 0)
            {
                //var transResult = _context.SeasonTransaction.Where(y => y.InvoiceId == invoiceResult.Id && y.Status == 1).FirstOrDefault();
                foreach (SeasonInvoice dr in invoiceList)
                {
                    List<SeasonTransaction> transDt = _db.SeasonTransaction.Where(x => x.Id == dr.Id).ToList();  //GetTransData(Convert.ToInt32(dr["ID"].ToString()));
                                                                                                                 //var userResult = _context.SeasonUser.Where(u => u.Id == invoiceResult.SeasonUserId).FirstOrDefault();
                    List<SeasonUser> userDt = _db.SeasonUser.Where(x => x.Id == dr.SeasonUserId).ToList(); //   DataTable userDt = GetUserData(Convert.ToInt32(dr["SeasonUserID"].ToString()));
                                                                                                           //var cardResult = _context.SeasonCard.Where(p => p.Id == invoiceResult.SeasonCardId).FirstOrDefault();
                    List<SeasonCard> cardDt = _db.SeasonCard.Where(x => x.Id == dr.SeasonCardId).ToList();  //DataTable cardDt = GetCardData(Convert.ToInt32(dr["SeasonCardID"].ToString()));

                    //var parkingResult = _context.ParkingLevel.Where(c => c.ParkingLevelId == cardResult.ParkingLevelId).FirstOrDefault();
                    List<ParkingLevel> parkingDt = _db.ParkingLevel.Where(x => x.ParkingLevelId == cardDt.FirstOrDefault().ParkingLevelId).ToList();  //DataTable parkingDt = GetParkingLevelData(Convert.ToInt32(cardDt.Rows[0]["ParkingLevelID"].ToString()));
                                                                                                                                                      //  List<ParkingLevel> companyDt = _db.ParkingLevel.Where(x => x.ParkingLevelId == cardDt.FirstOrDefault().ParkingLevelId).ToList();
                    List<Company> companyDt = _db.Company.Where(x => x.CompanyId == Convert.ToInt32(parkingDt.FirstOrDefault().CompanyCode)).ToList();
                    receipt = new ReceiptModel();
                    receipt.CompanyName = companyDt.FirstOrDefault().CompanyName.ToString();
                    receipt.CompanyAddress = companyDt.FirstOrDefault().CompanyAddress.ToString();
                    receipt.CompanyContactNo = companyDt.FirstOrDefault().ContactNo.ToString();
                    //logo
                    //   receipt.CompanyLogo = System.Text.Encoding.Unicode.GetBytes(companyDt.FirstOrDefault().Photo.ToString());
                    receipt.CustomerName = userDt.FirstOrDefault().BillingName.ToString();
                    receipt.CustomerEmail = userDt.FirstOrDefault().EmailAddress.ToString();
                    receipt.CustomerContactNo = userDt.FirstOrDefault().ContactNo.ToString();
                    receipt.CustomerAddress = userDt.FirstOrDefault().BillingAddress.ToString();
                    receipt.InvoiceDate = DateTime.Parse(invoiceList.FirstOrDefault().InvoiceDate.ToString());
                    receipt.InvoiceNo = invoiceList.FirstOrDefault().InvoiceNo.ToString();
                    receipt.TransactionTime = DateTime.Parse(transDt.FirstOrDefault().TransactionTime.ToString());
                    //    string paymentId = Enum.GetName(typeof(Constant.Ipay88PaymentId), Convert.ToInt32(transDt.Rows[0]["PaymentId"].ToString()));
                    //    receipt.PaymentMethod = paymentId;
                    receipt.ProdDesc = transDt.FirstOrDefault().ProdDesc.ToString();
                    receipt.SeasonCardNo = cardDt.FirstOrDefault().CardNo.ToString();
                    receipt.Amount = Decimal.Parse(transDt.FirstOrDefault().Amount.ToString());
                }
            }
        }
        //  public string ConvertUserListToHtmlTable(Dictionary<int, string> trans, int price, string subtotal, string total, DateTime datetime, int batchid,decimal finaltotal,Site sitedetails ) {

        public async Task<string> ConvertUserListToHtmlTable(string batchlist)
        {
            try { 
                  dynamic propertyValues = new ExpandoObject();
                IDictionary<string, object> models =null;
                propertyValues = JsonConvert.DeserializeObject<IEnumerable<ExpandoObject>>(batchlist);
                foreach (var item in propertyValues)
                {
                    models = item;

                    //foreach (var property in propertyValues.Keys)
                    //{

                    //}
                }
             
             //   List < IDictionary<string, object> > propertyValues
          string refno = models["referenceNo"].ToString();

            List<BatchCreationLog> Batches = _db.BatchCreationLog.Where(b => b.ReferenceNo == refno).ToList();
            Payout Payout_ = _db.Payout.Where(p => p.ReferenceNo == refno).FirstOrDefault();
         

            string ReceiptPath = Payout_.Receipt;
            string SettlementPath = Payout_.Settlement;

            Dictionary<int, string> Trans = new Dictionary<int, string>();
            //List<int> totalprice = new List<int>();

           // var blist = JsonConvert.DeserializeObject<IEnumerable<BatchCreationLog>>(batchlist);
            var price = 0;
            var subtotal = "";
            var batchid = 0;
            //var location = "";
            var siteid = 0;
            //var sitename = ""; 

            foreach (BatchCreationLog batch in Batches)
            {
                var batch_no = "FN" + batch.BatchNumber.PadLeft(5, '0');
                var sum = _db.Transaction.
                    Where(t => t.BatchNo == batch_no && t.SiteId == batch.SiteId && t.Spid == batch.Spid && t.ExitLane == batch.TerminalId && t.TransactionAmount > 0).Select(t => t.TransactionAmount).Sum();

                Trans.Add(batch.BatchId, (sum * 0.01)?.ToString("C"));

                price += Convert.ToInt32(sum * 0.01);
                batchid = batch.BatchId;
                //location = batch.LocationID;
                siteid = batch.SiteId;
                //sitename = _db.Site.Where(s => s.Name == batch.LocationID).Select(s => s.Name).FirstOrDefault();
            }

            var sitembr = 100 - (_db.Site.Where(s => s.Id == siteid).Select(s => s.Mdr).FirstOrDefault());

            var total = ((decimal)(price * sitembr / 100)).ToString("C");
            DateTime datetime = DateTime.Now;

            //var refno = "REF-"+ datetime.ToString("yyyyMMdd");

            subtotal = price.ToString("C");
            var finaltotal = (decimal)(price * sitembr / 100);

            //get site details 
            var sitedetails = _db.Site.Where(s => s.Id == siteid).FirstOrDefault();


            //     return "";
            //GetReceiptData(invoiceid);
            var sb = new StringBuilder();


                sb.Append(@"<html>
                            <head>
                                <title>Payoout Record  Testing</title>
                            </head>
                            <body style='font-family:calibri;'>");
            // if (receipt.CompanyLogo.Length > 0 && receipt.CompanyLogo != null)
            //{
            //    sb.Append(@" < img src = data:image / jpeg; base64," + Convert.ToBase64String(receipt.CompanyLogo) + " style = 'width:100%;' > ");
            //}
            //else
            //{
                sb.Append(@"<table cellpadding ='0' cellspacing='0' style='width: 100%;'><tr><td><img src='file:///C:/Images/PCS_Logo.jpg' width=320 height: 320 align=left ></td><td align='right'> <table cellpadding ='0' cellspacing='0' style='width: 100%;' ><tr><td style='font-weight:bold; text-align:right; color:#0000FF; font-size: 40px; ' >Official Receipt</d></tr> <tr><td style='font-weight:bold; text-align:right'>Ref No. : " + refno?.ToString() + "</d></tr> <tr><td style='font-weight:bold; text-align:right'>Date : " + Payout_.CreatedDate?.ToString() + " </d></tr></table> </td></tr></table>");
            //}

                sb.Append(@" <br /><br /><br /> <table cellpadding ='0' cellspacing='0' style='width: 100%;'>" +
                                        "<tr style='width:100%;' >" +
                                        "<th></th>" +
                                        "<th></th>" +
                                        "<th></th>" +
                                        "<th></th>" +
                                        "</tr>" +
                                        "<tr>"+
                                        "<td colspan='4'>");
            //sb.Append(@"<img src='" + receipt.CompanyLogo + "' style='width:100%; max-width:300px;'>");
          

            sb.Append(@"</td></tr><tr style='font-family:calibri;'><td style=' text-align:left' >" + sitedetails.Name + "</td><td style=' text-align:right' ></td><td style=' text-align:right' > </td><td style=' text-align:right'>" + sitedetails.SiteContactPersonName + "</td></tr>" +
            "<tr><td style=' text-align:left' >Company Address</td><td style=' text-align:left' ></td><td style=' text-align:right' ></td><td style=' text-align:right' >Client Address</td></tr>" +
            "<tr><td style=' text-align:left' >Compnay Phone No.</td><td style=' text-align:left' ></td><td style=' text-align:right' ></td><td style=' text-align:right' >" + sitedetails.SiteContactPersonEmail + "</td></tr>"+
            "<tr><td style=' text-align:left' ></td><td style=' text-align:left' ></td><td style=' text-align:right' ></td><td style=' text-align:right' >" + sitedetails.SiteContactPersonTelNo + "</td></tr></table><br /><br /><br />");
         
            sb.Append(@"<table style='width:100%; '> " +
                                    "<tr style='style='width:100%; font-family:calibri;  border-collapse:collapse'>" +
                                        "<th style='background-color:#00008b; border-collapse:collapse; text-align:left; color : white;'>BATCH NUMBER</th>" +
                                        "<th style='background-color:#00008b; border-collapse:collapse; text-align:left; color : white;'>BATCH DATE</th>" +
                                        "<th style='background-color:#00008b; border-collapse:collapse; text-align:left; color : white;'>LOCATION/TERMINAL ID</th>" +
                                        "<th style='background-color:#00008b; border-collapse:collapse; text-align:left; color : white;'>TRANSACTION RECORD</th>" +
                                        "<th style='background-color:#00008b; border-collapse:collapse; text-align:left; color : white;'>TRANSACTION AMOUNT</th>" +
                                    "</tr>" +
                                "");
                int counter_ = 1;
                int lastrow = Batches.Count;
            foreach (BatchCreationLog batch in Batches)
            {
                sb.Append(@"<tr style='width:100%; font-family:calibri; border-collapse:collapse;  border-collapse:collapse'>" +
                 "<td style='border-collapse:collapse; text-align:left; border-spacing: 0;'>" + batch.BatchNumber.PadLeft(5, '0') + "</td>" +
                 "<td style='border-collapse:collapse; text-align:left; border-spacing: 0;'>" + batch.BatchDate?.ToString("dd/MM/yyyy") + "</td>" +
                 "<td style='border-collapse:collapse; text-align:left; border-spacing: 0;'>" + @batch.LocationId + "/" + @batch.TerminalId + "</td>" +
                 "<td style='border-collapse:collapse; text-align:left; border-spacing: 0;'>" + @batch.TerminalId + "</td>" +
                 "<td style='border-collapse:collapse; text-align:left; border-spacing: 0;'></td>" +
              "</tr>");

                    if(counter_ == lastrow)
                    {
                        sb.Append(@"<tr style='width:100%; font-family:calibri; border-collapse:collapse;  border-collapse:collapse'>" +
                   "<td colspan=5 style=' border-collapse:collapse; text-align:right;'> Total Amount : " + Payout_.Amount?.ToString() + "</td>" +             
                "</tr>");
                    }
                    counter_ += 1;
            }
            sb.Append(@"</table>");




            sb.Append(@"<br /><br /><br /> <div> " +
                "<p style='text-align:center;'>This is computer generated receipt and no signature is required.</p>" +
                "</div>" +
                "</body>" +
                "</html>");
            return sb.ToString();

        }
            catch (Exception ex)
            { throw ex; }

}
    }
    public class FileDto
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public FileDto(string fileName, byte[] fileBytes)
        {
            FileName = fileName;
            FileBytes = fileBytes;
        }
    }
}