using DocumentFormat.OpenXml.Wordprocessing;
using Helpers.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Shared;

namespace ViewModels
{
    public class InventoryModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier, IFileModel
    {
        public string? SystemGeneratedId { get; set; }
        [Display(Name = "Description", Prompt = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Item #", Prompt = "Item #")]
        [Remote(action: "IsItemNoUnique", controller: "Inventory", AdditionalFields = "Id,ItemNo", ErrorMessage = "Item # already in use.")]
        public string ItemNo { get; set; }
        public double Quantity { get; set; }
        [Required]
        [Display(Name = "Minimum Quantity", Prompt = "Minimum Quantity")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum Quantity must be a positive number.")]
        public double? MinimumQuantity { get; set; }
        [Required]
        [Display(Name = "Part Number", Prompt = "Part Number")]
        public string? PartNumber { get; set; }
        [Required]
        [Display(Name = "Unit Cost", Prompt = "Unit Cost")]
        [Range(0, double.MaxValue, ErrorMessage = "Unit Cost must be a positive number.")]
        public decimal? UnitCost { get; set; }
        [Required]
        [Display(Name = "Reorder Level", Prompt = "Reorder Level")]
        [Range(0, double.MaxValue, ErrorMessage = "Reorder Level must be a positive number.")]
        public double? ReOrderLevel { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? File { get; set; }
        public CategoryBriefViewModel Category { get; set; } = new();
        public SubCategoryBriefViewModel SubCategory { get; set; } = new();
        public ManufacturerBriefViewModel Manufacturer { get; set; } = new();
        public UOMBriefViewModel UOM { get; set; } = new();
        public MUTCDBriefViewModel MUTCD { get; set; } = new();
        public LocationBriefViewModel Location { get; set; } = new();

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

    }
}

