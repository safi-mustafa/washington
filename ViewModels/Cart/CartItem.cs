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
    }
}

