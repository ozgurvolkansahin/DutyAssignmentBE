using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.DTOs
{
    public class DashboardDTO : IDashboardDTO
    {
        public IDashboard SystemResponsible { get; set; }
        public IEnumerable<IBranchInfo> BranchesInfo { get; set; }
        public DashboardDTO(IDashboard systemResponsible, IEnumerable<IBranchInfo> branchesInfo)
        {
            SystemResponsible = systemResponsible;
            BranchesInfo = branchesInfo;
        }
    }


}
