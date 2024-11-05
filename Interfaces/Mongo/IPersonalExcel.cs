using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Interfaces
{
    public interface IPersonalExcel : IEntity<string>
    {
        string Sicil { get; set; }
        string TcKimlik { get; set; }
        string Ad { get; set; }
        string Soyad { get; set; }
        string Rutbe { get; set; }
        string Birim { get; set; }
        string Nokta { get; set; }
        string Grup { get; set; }
        string Tel { get; set; }
        string Iban { get; set; }
        int Type { get; set; }
        IEnumerable<string> Duties { get; set; }
        IEnumerable<string> PaidDuties { get; set; }
        int DutiesCount { get; set; }
        int PaidDutiesCount { get; set; }
        bool Priority { get; set; }
    }
}
