using DutyAssignment.Interfaces;
using DutyAssignment.Models;

namespace DutyAssignment.Services
{
    public interface IExcelService
    {
        List<string> FindFiles(int type);
        PersonalInDuty ReadDutyFile(string filePath, int type);
        byte[] DownloadExcel(IEnumerable<IPersonalExcel> personalList);
        List<PaidPersonnelInDuty> ProcessPaymentFileExcelAndReturnPersonnel();
    }
}