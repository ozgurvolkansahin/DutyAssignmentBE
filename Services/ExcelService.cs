using DotNetEnv;
using DutyAssignment.Models;
using MongoDB.Bson;
using OfficeOpenXml;
using System.IO;

namespace DutyAssignment.Services;

public class ExcelService : IExcelService
{
    private readonly static string ExcelPath = Env.GetString("PERSONAL_EXCEL_PATH");

    public ExcelService()
    {
    }
    public void ReadPersonalExcel()
    {
        List<string> files = FindFiles();
        Console.WriteLine(files);
        // using (var package = new ExcelPackage(@ExcelPath))
        // {
        //     var sheet = package.Workbook.Worksheets.Add("My Sheet");
        //     sheet.Cells["A1"].Value = "Hello World!";

        //     // Save to file
        //     package.Save();
        // }
    }

    public List<string> FindFiles()
    {
        DirectoryInfo folder = new DirectoryInfo(ExcelPath);
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

    public PersonalInDuty ReadDutyFile(string FileName)
    {
        string FilePath = Path.Combine(ExcelPath, FileName);
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
                    SN = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty,
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
                    SN = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty,
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
}