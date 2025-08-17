using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Ordered")]
        [Description("Order has been placed")]
        Ordered,

        [Display(Name = "Delivery Scheduled")]
        [Description("Delivery is scheduled")]
        DeliveryScheduled,

        [Display(Name = "Delivered")]
        [Description("Order has been delivered")]
        Delivered,

        [Display(Name = "Past Due")]
        [Description("Order is past due")]
        PastDue,

        [Display(Name = "Canceled")]
        [Description("Order has been canceled")]
        Canceled,

        [Display(Name = "On Hold")]
        [Description("Order is on hold")]
        OnHold,

        [Display(Name = "Approved")]
        [Description("Order has been approved")]
        Approved,

        [Display(Name = "Submitted")]
        [Description("Order has been submitted")]
        Submitted,

        [Display(Name = "Rejected")]
        [Description("Order has been rejected")]
        Rejected,
        [Display(Name = "Issued")]
        [Description("Order has been issued")]
        Issued

        //     [Display(Name = "Pending Approval")]
        //[Description("Order is pending approval")]
        //PendingApproval = 1,

        //[Display(Name = "Scheduled")]
        //[Description("Order is scheduled")]
        //Scheduled,

        //[Display(Name = "Delivered")]
        //[Description("Order has been delivered")]
        //Delivered,

        //[Display(Name = "On Rent")]
        //[Description("Order is currently on rent")]
        //OnRent,

        //[Display(Name = "Off Rent")]
        //[Description("Order is off rent")]
        //OffRent,

        //[Display(Name = "Returned")]
        //[Description("Order has been returned")]
        //Returned
    }

}

