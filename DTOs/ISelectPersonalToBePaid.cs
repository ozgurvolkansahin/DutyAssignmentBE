using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.DTOs
{
    public interface ISelectPersonalToBePaid
    {
        IEnumerable<string> dutyIds { get; set; }
        int assignmentCount { get; set; }
    }
    public interface IGetAssignedPersonalByDutyIdWithPaginationPostObject
    {
        string dutyId { get; set; }
        int page { get; set; }
        int pageSize { get; set; }
    }
    public interface IGetAssignedPersonalByDutyIdWithPaginationResult<T>
    {
        IEnumerable<T> data { get; set; }
        int total { get; set; }
    }
    public interface IFilterPersonnel {
        string sicil { get; set; }
        string tcKimlik { get; set; }
        string isim { get; set; }
        string rutbe { get; set; }
        string birim { get; set; }
        string nokta { get; set; }
        string grup { get; set; }
        string tel { get; set; }
        string iban { get; set; }
        int page { get; set; }
        int pageSize { get; set; }
    }
}
