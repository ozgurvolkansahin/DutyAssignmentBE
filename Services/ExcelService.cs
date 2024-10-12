using DotNetEnv;
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
}