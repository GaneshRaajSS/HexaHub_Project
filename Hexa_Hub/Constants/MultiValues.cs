using System;

namespace Hexa_Hub.Models;
public class MultiValues
{
    public enum AssetStatus
    {
        OpenToRequest,
        Allocated,
        UnderMaintenance
    }

    public enum UserType
    {
        Employee,
        Admin
    }

    public enum RequestStatus
    {
        Pending,
        Allocated,
        Rejected
    }

    public enum IssueType
    {
        Malfunction,
        Repair,
        Installation
    }

    public enum AuditStatus
    {
        Sent,
        Completed
    }

    public enum ServiceReqStatus
    {
        UnderReview,
        Approved,
        Completed
    }

    public enum ReturnReqStatus
    {
        Sent,
        Approved,
        Returned,
        Rejected
    }

}
