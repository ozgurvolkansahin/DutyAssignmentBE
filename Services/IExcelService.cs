using DutyAssignment.Interfaces;
using DutyAssignment.Models;

namespace DutyAssignment.Services
{
    public interface IExcelService
    {
        void ReadPersonalExcel();
        List<string> FindFiles();
        PersonalInDuty ReadDutyFile(string filePath);
    }
}