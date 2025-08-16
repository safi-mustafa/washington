namespace ViewModels
{
    public class CartItem
    {
        public long InventoryId { get; set; }

        public long EquipmentId { get; set; }

        public long Quantity { get; set; }
        public decimal DefaultRentalRateDaily { get; set; }
        public decimal DefaultRentalRateMonthly { get; set; }
        public decimal DefaultRentalRateWeekly { get; set; }
        
        // Step1 form data
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal CalculatedTotal { get; set; }
        public string RentalFrequency { get; set; }
    }
}

