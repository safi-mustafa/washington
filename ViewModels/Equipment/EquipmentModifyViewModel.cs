using Helpers.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Common.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class EquipmentModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier, IFileModel
    {
        [DisplayName("ID #")]
        public string? SystemGeneratedId { get; set; }

        //[Display(Name = "Model")]
        //public string? EquipmentModel { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Hourly Rate")]
        [Required]
        public double HourlyRate { get; set; } = 1;

        [Display(Name = "Total Value")]
        public float TotalValue { get; set; }

        [Required]
        [Display(Name = "Model", Prompt = "Model #")]
        [Remote(action: "IsItemNoUnique", controller: "Equipment", AdditionalFields = "Id,ItemNo", ErrorMessage = "Model # already in use.")]
        public string ItemNo { get; set; }


        public string? ImageUrl { get; set; }
        public IFormFile? File { get; set; }
        public CategoryBriefViewModel Category { get; set; } = new();
        public SubCategoryBriefViewModel SubCategory { get; set; } = new();
        public ManufacturerBriefViewModel Manufacturer { get; set; } = new();
        public UOMBriefViewModel UOM { get; set; } = new();

        public List<EquipmentSubLineViewModel> EquipmentSublines { get; set; } = new();

        public string? Type { get; set; }
        public string GetBaseFolder()
        {
            var ext = Type;
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            {
                return "Images";
            }
            if (ext == ".mp4")
            {
                return "Videos";
            }
            if (ext == ".pdf" || ext == ".docx" || ext == ".xlsx" || ext == ".txt")
            {
                return "Documents";
            }
            return "Others";
        }

        [Required]
        [Display(Name = "Default Rental Rate One Time")]
        public string? DefaultRentalRateOneTime { get; set; } = "100.00";

        [Required]
        [Display(Name = "Default Rental Rate Daily")]
        public string? DefaultRentalRateDaily { get; set; }

        [Required]
        [Display(Name = "Default Rental Rate Weekly")]
        public string? DefaultRentalRateWeekly { get; set; }

        [Required]
        [Display(Name = "Default Rental Rate Monthly")]
        public string? DefaultRentalRateMonthly { get; set; }

        [Required]
        [Display(Name = "Useful Life Months")]
        public string? UsefulLifeMonths { get; set; }
    }
}

