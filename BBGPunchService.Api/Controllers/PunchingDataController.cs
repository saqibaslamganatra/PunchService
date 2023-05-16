using BBGPunchService.Api.Dto;
using BBGPunchService.Core.Model.API;
using BBGPunchService.Core.Model.TargetEntity;
using BBGPunchService.Infrastructure.Service.Handler.Interface;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NLog;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text;


namespace BBGPunchService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PunchingDataController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICacheService _cacheService;
        Guid uniqueFolder = Guid.Empty;
        public PunchingDataController(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetPunchingData(PunchDataDto punchingData)
        {
            try
            {
                IQueryable<PunchingData> getPunchData = _unitOfWork.IPunchingDataRepo.GetPunchingDataQueryable();

                if (!string.IsNullOrEmpty(punchingData.startDate) && !string.IsNullOrEmpty(punchingData.endDate) && Convert.ToDateTime(punchingData.startDate) > Convert.ToDateTime(punchingData.endDate))
                {
                    Logger.Error("Start date cannot be greater than end date.");
                    return BadRequest("Start date cannot be greater than end date.");
                }

                if (!string.IsNullOrEmpty(punchingData.startDate) && DateTime.TryParse(punchingData.startDate, out DateTime startDate))
                {
                    startDate = startDate.Date;
                    getPunchData = getPunchData.Where(p => p.PunchDateTime >= startDate);
                }

                
                if (!string.IsNullOrEmpty(punchingData.endDate) && DateTime.TryParse(punchingData.endDate, out DateTime endDate))
                {
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    getPunchData = getPunchData.Where(p => p.PunchDateTime <= endDate);
                }

                
                if (!string.IsNullOrEmpty(punchingData.searchText))
                {
                    getPunchData = getPunchData.Where(p => p.EnrolNo.Trim() == punchingData.searchText.Trim());
                }

                
                string? totalItemsCacheKey = $"punchingdata_totalitems_{punchingData.startDate}_{punchingData.endDate}_{punchingData.searchText}";
                int totalItems = await _cacheService.GetOrAddAsync(totalItemsCacheKey, async () => await getPunchData.CountAsync(), TimeSpan.FromMinutes(10));

                
                int totalPages = (int)Math.Ceiling((double)totalItems / punchingData.pageSize);

               
                int startIndex = (punchingData.pageNumber - 1) * punchingData.pageSize;
                int itemsToReturn = Math.Min(punchingData.pageSize, totalItems - startIndex);

                
                List<PunchingData>? result = await getPunchData
                    .OrderByDescending(p => p.PunchDateTime)
                    .Skip(startIndex)
                    .Take(itemsToReturn)
                    .ToListAsync();


                PaginationInfo? pagination = new PaginationInfo
                {
                    TotalPages = totalPages,
                    CurrentPage = punchingData.pageNumber,
                    PageSize = punchingData.pageSize,
                    TotalItems = totalItems
                };

                
                return Ok(new ApiResponse { Result = result, Pagination = pagination });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred while getting punching data.");
                return BadRequest();
            }
        }

        [HttpPost("Export")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExportPunchingData([FromBody] PunchDataDto punchingData, string type)
        {
            int pageSize = 5000;
            int pageNumber = punchingData.pageNumber;
            uniqueFolder = Guid.NewGuid();
            try
            {
                IQueryable<PunchingData> getPunchData = _unitOfWork.IPunchingDataRepo.GetPunchingDataQueryable();
                
                if (!string.IsNullOrEmpty(punchingData.startDate) && !string.IsNullOrEmpty(punchingData.endDate) && Convert.ToDateTime(punchingData.startDate) > Convert.ToDateTime(punchingData.endDate))
                {
                    Logger.Error("Start date cannot be greater than end date.");
                    return BadRequest("Start date cannot be greater than end date.");
                }

                if (!string.IsNullOrEmpty(punchingData.startDate) && DateTime.TryParse(punchingData.startDate, out DateTime startDate))
                {
                    startDate = startDate.Date;
                    getPunchData = getPunchData.Where(p => p.PunchDateTime >= startDate);
                }

                if (!string.IsNullOrEmpty(punchingData.endDate) && DateTime.TryParse(punchingData.endDate, out DateTime endDate))
                {
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    getPunchData = getPunchData.Where(p => p.PunchDateTime <= endDate);
                }

                if (!string.IsNullOrEmpty(punchingData.searchText))
                {
                    getPunchData = getPunchData.Where(p => p.EnrolNo.Trim() == punchingData.searchText.Trim());
                }

                // Add this line here to sort the data in descending order
                getPunchData = getPunchData.OrderByDescending(p => p.PunchNumber);


                string fileName = $"punch-data-{DateTime.Now.ToString("yyyyMMddHHmmss")}.{type}";

                byte[] fileBytes = null;

                switch (type.ToLower())
                {
                    case "pdf":
                        // generate PDF file
                        List<string> fileChunkPaths = await GeneratePdfFileChunks(getPunchData, pageSize);
                        fileBytes = MergePdfFiles(fileChunkPaths);

                        string? tempDirectory = fileChunkPaths.Any() ? System.IO.Path.GetDirectoryName(fileChunkPaths.First()) : null;

                        if (!string.IsNullOrEmpty(tempDirectory))
                        {
                            Directory.Delete(tempDirectory, true);
                        }

                        //foreach (string chunkFilePath in fileChunkPaths)
                        //{
                        //    System.IO.File.Delete(chunkFilePath);
                        //}


                        break;
                    case "xlsx":
                        // generate XLSX file
                        fileBytes = GenerateXlsxFile(getPunchData);
                        break;
                    case "csv":
                        // generate CSV file
                        fileBytes = await GenerateCsvFile(getPunchData);
                        break;
                    default:
                        Logger.Error("Invalid export type specified.");
                        return BadRequest("Invalid export type specified.");
                }

                return File(fileBytes, $"application/{type}", fileName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred while exporting punching data.");
                return BadRequest();
            }
        }
        private async Task<List<string>> GeneratePdfFileChunks(IQueryable<PunchingData> data, int pageSize)
        {
            int totalItems = data.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            List<Task<string>> pdfGenerationTasks = new List<Task<string>>();
            for (int i = 0; i < totalPages; i++)
            {
                int currentPage = i;
                // Make a snapshot of the data for the current page
                var currentPageData = data.Skip(currentPage * pageSize).Take(pageSize).ToList();
                pdfGenerationTasks.Add(Task.Run(() => GeneratePdfPageAndStore(currentPageData, currentPage)));
            }

            List<string> storedFiles = (await Task.WhenAll(pdfGenerationTasks)).ToList();


            return storedFiles;
        }
        private string GeneratePdfPageAndStore(IEnumerable<PunchingData> data, int pageNumber)
        {
            byte[] pdfBytes = GeneratePdfPage(data);
            string chunkFileName = $"punch-data-chunk-{pageNumber}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";


            string storageDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pdf-"+DateTime.Now.ToString("yyyyMMddHH")+"-"+ uniqueFolder);


            if (!System.IO.File.Exists(storageDirectory))
            {
                Directory.CreateDirectory(storageDirectory);

            }
            string chunkFilePath = System.IO.Path.Combine(storageDirectory, chunkFileName);
            System.IO.File.WriteAllBytes(chunkFilePath, pdfBytes);
            return chunkFilePath;
        }
        private byte[] GeneratePdfPage(IEnumerable<PunchingData> data)
        {
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = new PdfWriter(memoryStream);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2, 2, 1.5f, 1 }));
            table.AddHeaderCell("Enrol No");
            table.AddHeaderCell("Full Name");
            table.AddHeaderCell("Punch Date Time");
            table.AddHeaderCell("Punch Direction");
            table.AddHeaderCell("Punch Number");

            foreach (PunchingData punchingData in data)
            {
                table.AddCell(punchingData.EnrolNo);
                table.AddCell(punchingData.FullName);
                table.AddCell(punchingData.PunchDateTime.ToString());
                table.AddCell(punchingData.PunchDirection);
                table.AddCell(punchingData.PunchNumber.ToString());
            }

            document.Add(table);
            document.Close();

            return memoryStream.ToArray();
        }
        private byte[] MergePdfFiles(List<string> fileChunkPaths)
        {
            MemoryStream output = new MemoryStream();
            PdfWriter outputWriter = new PdfWriter(output);
            PdfDocument mergedPdf = new PdfDocument(outputWriter);

            PdfMerger merger = new PdfMerger(mergedPdf);

            foreach (string chunkFilePath in fileChunkPaths)
            {
                PdfDocument pdf = new PdfDocument(new PdfReader(chunkFilePath));
                merger.Merge(pdf, 1, pdf.GetNumberOfPages());
                pdf.Close();
            }

            mergedPdf.Close();

            return output.ToArray();
        }
        private byte[] GenerateXlsxFile(IQueryable<PunchingData> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create a new Excel package
            using var excelPackage = new ExcelPackage();

            // Add a worksheet to the package
            var worksheet = excelPackage.Workbook.Worksheets.Add("Punching Data");

            // Set up the header row
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Height = 20;
            headerRow.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Add the column headers
            worksheet.Cells[1, 1].Value = "EnrolNo";
            worksheet.Cells[1, 2].Value = "FullName";
            worksheet.Cells[1, 3].Value = "PunchDateTime";
            worksheet.Cells[1, 4].Value = "PunchDirection";
            worksheet.Cells[1, 5].Value = "PunchNumber";

            // Format the column headers
            for (int i = 1; i <= 5; i++)
            {
                worksheet.Column(i).Width = 15;
            }

            // Use keyset seek pagination to fetch data in small batches
            int pageSize = 5000;
            int pageNumber = 1;
            int? lastPunchNumber = null;

            while (true)
            {
                
                var query = data;
                if (lastPunchNumber.HasValue)
                {
                    query = query.Where(p => p.PunchNumber < lastPunchNumber.Value);
                }
                var records = query.OrderByDescending(p => p.PunchNumber).Take(pageSize).ToList();

             
                if (records.Count == 0)
                {
                    break;
                }

                
                for (int i = 0; i < records.Count; i++)
                {
                    var item = records[i];
                    var row = pageNumber * pageSize - (pageSize - i) + 1;
                    worksheet.Cells[row, 1].Value = item.EnrolNo;
                    worksheet.Cells[row, 2].Value = item.FullName;
                    worksheet.Cells[row, 3].Value = item.PunchDateTime.Value.ToString("dd/MM/yyyy hh:mm:ss tt");
                    worksheet.Cells[row, 4].Value = item.PunchDirection;
                    worksheet.Cells[row, 5].Value = item.PunchNumber;
                }

                lastPunchNumber = records.Last().PunchNumber;

                records.Clear();

                pageNumber++;
            }

           
            worksheet.Cells.AutoFitColumns();

           
            return excelPackage.GetAsByteArray();
        }


        private async Task<byte[]> GenerateCsvFile(IQueryable<PunchingData> data)
        {
            var stringBuilder = new StringBuilder();

            // write header row
            stringBuilder.AppendLine("EnrolNo,FullName,PunchDateTime,PunchDirection,PunchNumber");

            // write data rows
            foreach (var item in data)
            {
                stringBuilder.AppendLine($"{item.EnrolNo},{item.FullName},{item.PunchDateTime},{item.PunchDirection},{item.PunchNumber}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());

            return await Task.FromResult(csvBytes);
        }

    }
}
