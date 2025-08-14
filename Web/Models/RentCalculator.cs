public static class RentCalculator
{
    public static decimal CalculateRent(DateTime startDate, DateTime endDate, decimal dailyRate, decimal weeklyRate, decimal monthlyRate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.");

        decimal totalRent = 0;

        int months = 0;
        DateTime tempDate = startDate;

        while (tempDate.AddMonths(1) <= endDate)
        {
            months++;
            tempDate = tempDate.AddMonths(1);
        }

        int remainingDays = (endDate - tempDate).Days;
        int weeks = 0;
        int days = 0;

        if (months == 0 && remainingDays > 0)
        {
            if (remainingDays <= 5)
            {
                days = remainingDays;
            }
            else if (remainingDays >= 6 && remainingDays <= 29)
            {
                weeks = remainingDays / 7;
                days = remainingDays % 7;
            }
        }
        else
        {
            weeks = remainingDays / 7;
            days = remainingDays % 7;
        }

        totalRent = (months * monthlyRate) + (weeks * weeklyRate) + (days * dailyRate);
        return totalRent;
    }
}