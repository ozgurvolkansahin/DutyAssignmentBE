using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Models;

public class PersonalExcel : IPersonalExcel
{
    public required string SN { get; set; }
    public required string Sicil { get; set; }
    public required string TcKimlik { get; set; }
    public required string Ad { get; set; }
    public required string Soyad { get; set; }
    public required string Rutbe { get; set; }
    public required string Birim { get; set; }
    public required string Nokta { get; set; }
    public required string Grup { get; set; }
    public required string Tel { get; set; }
    public required string Iban { get; set; }
    public required IEnumerable<string> Duties { get; set; }
    public required IEnumerable<string> PaidDuties { get; set; }
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
}

public class PersonalInDuty
{
    public required IEnumerable<PersonalExcel> ResponsibleManagers { get; set; }
    public required IEnumerable<PersonalExcel> PoliceAttendants { get; set; }
}