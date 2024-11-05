using DotNetEnv;
using DutyAssignment.Enum;
using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;

namespace DutyAssignment.Services;

public class ExcelService : IExcelService
{
    private readonly static string KadroExcelPath = Env.GetString("KADRO_EXCEL_PATH");
    private readonly static string SubeExcelPath = Env.GetString("SUBE_EXCEL_PATH");
    private readonly static string CevikExcelPath = Env.GetString("CEVIK_EXCEL_PATH");
    private readonly static string PaymentExcelPath = Env.GetString("PAYMENT_EXCEL_PATH");

    public ExcelService()
    {
    }
    private string GetExcelPath(int type = 1) {
        if (type == (int)PersonnelTypeEnum.KADRO) {
            return KadroExcelPath;
        } else if (type == (int)PersonnelTypeEnum.SUBE) {
            return SubeExcelPath;
        } else if (type == (int)PersonnelTypeEnum.CEVIK) {
            return CevikExcelPath;
        }
        return KadroExcelPath;
    }
    public List<string> FindFiles(int type)
    {
        string path = GetExcelPath(type);
        DirectoryInfo folder = new DirectoryInfo(path);
        if (folder.Exists) // else: Invalid folder!
        {
            FileInfo[] files = folder.GetFiles("*.xlsx");
            // return files names as list
            return files.AsQueryable().Select(f => f.Name.Replace(".xlsx", "").Trim()).ToList();
        }
        else
        {
            // Return an empty list if the folder does not exist
            return new List<string>();
        }
    }
    public List<string> FindPaymentFiles()
    {
        DirectoryInfo folder = new DirectoryInfo(PaymentExcelPath);
        if (folder.Exists) // else: Invalid folder!
        {
            FileInfo[] files = folder.GetFiles("*.xlsx");
            // return files names as list
            return files.AsQueryable().Select(f => f.Name).ToList();
        }
        else
        {
            // Return an empty list if the folder does not exist
            return new List<string>();
        }
    }

    // Read the excel file and return the list of PersonalExcel objects
    public List<PaidPersonnelInDuty> ProcessPaymentFileExcelAndReturnPersonnel() {
        List<string> files = FindPaymentFiles();
        List<PaidPersonnelInDuty> personnel = new List<PaidPersonnelInDuty>();
        foreach (var file in files)
        {
            string FilePath = Path.Combine(PaymentExcelPath, file);
            FileInfo existingFile = new FileInfo(FilePath);
            using (ExcelPackage package = new ExcelPackage(existingFile)) {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                // find rows which A column trimmed value is "EK GÖSTERGESİ 3600(dahil)-6400(hariç) OLAN KADROLARDA BULUNANLAR"
                // and which which A column trimmed value is "AYLIK/KADRO DERECESİ 1-4 OLANLAR"
                // then get the rows between two indices
                // var personnelList = Enumerable.Range(1, worksheet.Dimension.Rows)
                //     .SkipWhile(row => worksheet.Cells[row, 1].Value?.ToString().Trim() != "EK GÖSTERGESİ 3600(dahil)-6400(hariç) OLAN KADROLARDA BULUNANLAR")
                //     .Skip(1)
                //     .TakeWhile(row => worksheet.Cells[row, 1].Value?.ToString().Trim() != "AYLIK/KADRO DERECESİ 1-4 OLANLAR")
                //     .Select(row => worksheet.Cells[row, 2].Value?.ToString().Trim())
                //     // select only non-empty values
                //     .Where(x => !string.IsNullOrEmpty(x))
                //     .ToList();

                // find rows which A column trimmed value is "EK GÖSTERGESİ 3600(dahil)-6400(hariç) OLAN KADROLARDA BULUNANLAR"
                // and which which A column trimmed value is "AYLIK/KADRO DERECESİ 1-4 OLANLAR"
                // then get the rows between two indices
                // then start from index of "EK GÖSTERGESİ 3600(dahil)-6400(hariç) OLAN KADROLARDA BULUNANLAR"
                // and stop when find first empty B cell value
                var personnelList = Enumerable.Range(1, worksheet.Dimension.Rows)
                    .SkipWhile(row => worksheet.Cells[row, 1].Value?.ToString().Trim() != "EK GÖSTERGESİ 3600(dahil)-6400(hariç) OLAN KADROLARDA BULUNANLAR")
                    .Skip(1)
                    .TakeWhile(row => !string.IsNullOrEmpty(worksheet.Cells[row, 2].Value?.ToString().Trim()))
                    .Select(row => worksheet.Cells[row, 3].Value?.ToString().Trim())
                    // select only non-empty values
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                personnel.Add(new PaidPersonnelInDuty{
                    Personnel = personnelList,
                    DutyId = file.Split("-")[0].Trim()});
            }
        }
        return personnel;
    }

    public PersonalInDuty ReadDutyFile(string FileName, int type)
    {
        string path = GetExcelPath(type);
        string FilePath = Path.Combine(path, FileName);
        FileInfo existingFile = new FileInfo(FilePath);
        using (ExcelPackage package = new ExcelPackage(existingFile))
        {
            //get the first worksheet in the workbook
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            // first two rows are headers
            worksheet.DeleteRow(1, 2);
            try
            {
                // RESPONSIBLE MANAGERS
                var responsibleManagers = Enumerable.Range(1, worksheet.Dimension.Rows)
                // stop if the row value is "GÖREVLİ PERSONEL"
                .TakeWhile(row => worksheet.Cells[row, 1].Value?.ToString() != "GÖREVLİ PERSONEL")
                .Select(row => new PersonalExcel
                {
                    Sicil = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? string.Empty,
                    TcKimlik = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? string.Empty,
                    Ad = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? string.Empty,
                    Soyad = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? string.Empty,
                    Rutbe = worksheet.Cells[row, 6].Value?.ToString()?.Trim() ?? string.Empty,
                    Birim = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? string.Empty,
                    Nokta = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? string.Empty,
                    Grup = worksheet.Cells[row, 9].Value?.ToString()?.Trim() ?? string.Empty,
                    Tel = worksheet.Cells[row, 10].Value?.ToString()?.Trim() ?? string.Empty,
                    Iban = worksheet.Cells[row, 11].Value?.ToString()?.Trim() ?? string.Empty,
                    Type = type,
                    Id = ObjectId.GenerateNewId().ToString(),
                    Duties = new List<string>(),
                    PaidDuties = new List<string>()


                }
                )
                .ToList();
                // add +2 to skip the row last manager row and with "GÖREVLİ PERSONEL"
                //  start from 15th row to the end

                var policeAttendant = Enumerable.Range(responsibleManagers.Count + 2, worksheet.Dimension.Rows - responsibleManagers.Count - 1)
                .Select(row => new PersonalExcel
                {
                    Sicil = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? string.Empty,
                    TcKimlik = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? string.Empty,
                    Ad = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? string.Empty,
                    Soyad = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? string.Empty,
                    Rutbe = worksheet.Cells[row, 6].Value?.ToString()?.Trim() ?? string.Empty,
                    Birim = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? string.Empty,
                    Nokta = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? string.Empty,
                    Grup = worksheet.Cells[row, 9].Value?.ToString()?.Trim() ?? string.Empty,
                    Tel = worksheet.Cells[row, 10].Value?.ToString()?.Trim() ?? string.Empty,
                    Type = type,
                    Iban = worksheet.Cells[row, 11].Value?.ToString()?.Trim() ?? string.Empty,
                    Id = ObjectId.GenerateNewId().ToString(),
                    Duties = new List<string>(),
                    PaidDuties = new List<string>()
                }
                )
                .ToList();
                return new PersonalInDuty { ResponsibleManagers = responsibleManagers, PoliceAttendants = policeAttendant };
            }
            catch (Exception e)
            {
                throw new Exception("Error reading row: " + e);
            }
        }
    }
    public byte[] DownloadExcel(IEnumerable<IPersonalExcel> personalList)
    {
        // Excel şablonunun yolunu belirt (eğer var olan bir şablonu kullanıyorsan)
        // Ana dizinde Templates klasörü altında report_template.xlsx dosyası var, buraya yolunu belirtiyoruz
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "report_template.xlsx");
        using (var package = new ExcelPackage(new FileInfo(templatePath)))
        {
            var worksheet = package.Workbook.Worksheets[0]; // İlk çalışma sayfasını al


            int startRow = 4; // 5. sıradan itibaren doldur
            worksheet.InsertRow(startRow, personalList.Count());
            var personalArray = personalList.ToArray();
            for (int i = 0; i < personalArray.Length; i++)
            {
                worksheet.Cells[startRow + i, 1].Value = i + 1; // İsim

                long tcKimlikValue;
                worksheet.Cells[startRow + i, 2].Value = Int64.TryParse(personalArray[i].TcKimlik, out tcKimlikValue) ? tcKimlikValue : 0;
                worksheet.Cells[startRow + i, 2].Style.Numberformat.Format = "0";
                worksheet.Cells[startRow + i, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                long sicilValue;
                worksheet.Cells[startRow + i, 3].Value = Int64.TryParse(personalArray[i].Sicil, out sicilValue) ? sicilValue : 0;
                worksheet.Cells[startRow + i, 3].Style.Numberformat.Format = "0";
                worksheet.Cells[startRow + i, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[startRow + i, 4].Value = personalArray[i].Ad;
                worksheet.Cells[startRow + i, 5].Value = personalArray[i].Soyad;
                worksheet.Cells[startRow + i, 6].Value = personalArray[i].Rutbe;
                worksheet.Cells[startRow + i, 7].Value = personalArray[i].Iban;
                worksheet.Cells[startRow + i, 8].Value = "";
                worksheet.Cells[startRow + i, 9].Value = personalArray[i].Birim;

                worksheet.Cells[startRow + i, 1].Style.Font.Bold = true;
                worksheet.Cells[startRow + i, 1, startRow + i, 9].Style.Font.Name = "Times New Roman";  // Yazı tipi
                worksheet.Cells[startRow + i, 1, startRow + i, 9].Style.Font.Size = 16;  // Yazı boyutu
            }
            var border = worksheet.Cells[5, 1, personalList.Count() + 5, 12];
            border.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            border.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            border.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            border.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            return package.GetAsByteArray();
        }
    }

}