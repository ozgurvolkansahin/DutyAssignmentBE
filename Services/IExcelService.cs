using DutyAssignment.Interfaces;

namespace DutyAssignment.Services
{
    public interface IExcelService
    {
        void ReadPersonalExcel();
        List<string> FindFiles();
    }
}