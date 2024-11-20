using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;

namespace DutyAssignment.Models;

public class BranchData : IBranchData
{
    public int TotalDuties { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalPayments { get; set; }
    public int TotalAssignedPersonal { get; set; }
    public int TotalPaymentsDone { get; set; }
    // constructor
    public BranchData(int totalDuties,
    int totalAssignments,
    int totalPayments,
    int totalAssignedPersonal,
    int totalPaymentsDone)
    {
        TotalDuties = totalDuties;
        TotalAssignments = totalAssignments;
        TotalPayments = totalPayments;
        TotalAssignedPersonal = totalAssignedPersonal;
        TotalPaymentsDone = totalPaymentsDone;
    }
}

public class BranchInfo : IBranchInfo
{
    public string BranchName { get; set; }
    public IBranchData Data { get; set; }
    public BranchInfo(string branchName, IBranchData data)
    {
        BranchName = branchName;
        Data = data;
    }
}