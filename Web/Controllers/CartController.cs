using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Repositories.Common;
using ViewModels;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> _inventoryService;
        private readonly ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> _transactionService;
        private readonly IEquipmentTransactionService<EquipmentTransactionModifyViewModel, EquipmentTransactionModifyViewModel, EquipmentTransactionDetailViewModel> _equipmentTransactionService;

        //private readonly IDashboardService _service;

        public CartController(
            ILogger<CartController> logger
            , IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> inventoryService
            , ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> transactionService
            , IEquipmentTransactionService<EquipmentTransactionModifyViewModel, EquipmentTransactionModifyViewModel, EquipmentTransactionDetailViewModel> equipmentTransactionService
            )
        {
            _logger = logger;
            _inventoryService = inventoryService;
            _transactionService = transactionService;
            _equipmentTransactionService = equipmentTransactionService;
        }

        public async Task<ActionResult> Index()
        {
            var response = new OrderModifyViewModel();
            try
            {
                var cart = GetCartFromSession();
                var model = new OrderModifyViewModel();
                var inventoryIds = cart.InventoryItems.Select(x => x.InventoryId).ToList();
                if (inventoryIds?.Count > 0)
                {
                    var orderedInventories = await _transactionService.GetGroupedTransactionsByItemsForOrder(inventoryIds);
                    foreach (var o in orderedInventories)
                    {
                        response.OrderItems.Add(new OrderItemModifyViewModel
                        {
                            Inventory = o.Inventory,
                            OHQuantity = (long)o.Quantity,
                            Quantity = cart.InventoryItems.Where(x => x.InventoryId == o.Inventory.Id).Select(x => x.Quantity).FirstOrDefault(),
                        });
                    }
                }
                var equipmentIds = cart.EquipmentItems.Select(x => x.EquipmentId).ToList();
                if (equipmentIds?.Count > 0)
                {
                    var orderedInventories = await _equipmentTransactionService.GetGroupedTransactionsByItemsForOrder(equipmentIds);
                    foreach (var o in orderedInventories)
                    {
                        response.OrderItems.Add(new OrderItemModifyViewModel
                        {
                            Equipment = o.Equipment,
                            OHQuantity = (long)o.Quantity,
                            Quantity = cart.EquipmentItems.Where(x => x.EquipmentId == o.Equipment.Id).Select(x => x.Quantity).FirstOrDefault(),
                        });
                    }
                }

                if (equipmentIds?.Count < 1 && inventoryIds?.Count < 1)
                {
                    ModelState.AddModelError("", "Add some items in the cart first!");
                }
                return View(response);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong, please try again later.");
                _logger.LogError(ex, ex.Message);
                return View(response);
            }
        }

        public async Task<ActionResult> Step1()
        {
            var response = new OrderModifyViewModel
            {
                OrderItems = new List<OrderItemModifyViewModel>()
            };

            var cart = GetCartFromSession();
            var inventoryIds = cart.InventoryItems.Select(x => x.InventoryId).ToList();
            if (inventoryIds.Any())
            {
                var orderedInventories = await _transactionService.GetGroupedTransactionsByItemsForOrder(inventoryIds);
                foreach (var o in orderedInventories)
                {
                    response.OrderItems.Add(new OrderItemModifyViewModel
                    {
                        Inventory = o.Inventory,
                        OHQuantity = (long)o.Quantity,
                        Quantity = cart.InventoryItems
                            .Where(x => x.InventoryId == o.Inventory.Id)
                            .Select(x => x.Quantity)
                            .FirstOrDefault()
                    });
                }
            }
            var equipmentIds = cart.EquipmentItems.Select(x => x.EquipmentId).ToList();
            if (equipmentIds.Any())
            {
                var orderedEquipments = await _equipmentTransactionService.GetGroupedTransactionsByItemsForOrder(equipmentIds);
                foreach (var o in orderedEquipments)
                {
                    response.OrderItems.Add(new OrderItemModifyViewModel
                    {
                        Equipment = o.Equipment,
                        OHQuantity = (long)o.Quantity,
                        Quantity = cart.EquipmentItems
                            .Where(x => x.EquipmentId == o.Equipment.Id)
                            .Select(x => x.Quantity)
                            .FirstOrDefault()
                    });
                }
            }
            if (!equipmentIds.Any() && !inventoryIds.Any())
            {
                ModelState.AddModelError("", "Add some items in the cart first!");
            }

            return View("Step1", response);
        }

        public async Task<ActionResult> Step2()
        {
            return View("Step2");
        }

        public async Task<ActionResult> Step3()
        {
            return View("Step3");
        }
        public async Task<ActionResult> Step4()
        {
            return View("Step4");
        }

        [HttpPost]
        public async Task<JsonResult> DeleteInventoryFromCart(int id)
        {
            try
            {
                var cart = GetCartFromSession();
                var cartItem = cart?.InventoryItems.FirstOrDefault(x => x.InventoryId == id);
                if (cartItem != null)
                {
                    cart?.InventoryItems.Remove(cartItem);
                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
                    return Json(new { success = true, redirectUrl = Url.Action("Step1", "Cart") });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteEquipmentFromCart(int id)
        {
            try
            {
                var cart = GetCartFromSession();
                var cartItem = cart?.EquipmentItems.FirstOrDefault(x => x.EquipmentId == id);
                if (cartItem != null)
                {
                    cart?.EquipmentItems.Remove(cartItem);
                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
                    return Json(new { success = true, redirectUrl = Url.Action("Step1", "Cart") });
                }

                return Json(new { success = true, redirectUrl = Url.Action("Step1", "Cart") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { success = false });
            }
        }

        public PartialViewResult ShowShoppingCart()
        {

            var cart = GetCartFromSession();
            return PartialView("_ShowShoppingCart", cart);
        }

        [HttpPost]
        public async Task<JsonResult> AddInventoryToCart(long id)
        {
            try
            {
                var cart = GetCartFromSession();

                var existinginventoryinfo = cart.InventoryItems.Where(x => x.InventoryId == id).FirstOrDefault();
                if (existinginventoryinfo != null)
                {
                    ++existinginventoryinfo.Quantity;
                }
                else
                {
                    CartItem cartitem = new CartItem()
                    {
                        InventoryId = id,
                        Quantity = 1,
                    };
                    cart.InventoryItems.Add(cartitem);
                }
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<JsonResult> AddEquipmentToCart(long id)
        {
            try
            {
                var cart = GetCartFromSession();

                var existinginventoryinfo = cart.EquipmentItems.Where(x => x.EquipmentId == id).FirstOrDefault();
                if (existinginventoryinfo != null)
                {
                    ++existinginventoryinfo.Quantity;
                }
                else
                {
                    CartItem cartitem = new CartItem()
                    {
                        EquipmentId = id,
                        Quantity = 1,
                    };
                    cart.EquipmentItems.Add(cartitem);
                }
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return Json(true);
        }

        private CartViewModel GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            CartViewModel cart = new();
            if (cartJson != null)
            {
                cart = JsonConvert.DeserializeObject<CartViewModel>(cartJson) ?? new();
                // Now you can use the 'cart' object
            }
            return cart;
        }

        //public class Step1Model : PageModel
        //{
        //    public IActionResult OnPost()
        //    {
        //        return RedirectToPage("Step2");
        //    }
        //}
        //public class Step2Model : PageModel
        //{
        //    [BindProperty] public string ProjectName { get; set; }
        //    [BindProperty] public string SpecialInstructions { get; set; }

        //    public void OnGet()
        //    {
        //        // load if coming back
        //        ProjectName = TempData.Peek("ProjectName") as string;
        //        SpecialInstructions = TempData.Peek("SpecialInstructions") as string;
        //    }

        //    public IActionResult OnPost()
        //    {
        //        TempData["ProjectName"] = ProjectName;
        //        TempData["SpecialInstructions"] = SpecialInstructions;
        //        return RedirectToPage("Step3");
        //    }
        //}
        //public class Step3Model : PageModel
        //{
        //    public string ProjectName { get; set; }
        //    public string SpecialInstructions { get; set; }

        //    public void OnGet()
        //    {
        //        ProjectName = TempData.Peek("ProjectName") as string;
        //        SpecialInstructions = TempData.Peek("SpecialInstructions") as string;
        //    }

        //    public IActionResult OnPost()
        //    {
        //        // Save to DB here...
        //        TempData.Clear();
        //        return RedirectToPage("OrderConfirmation");
        //    }
        //}

        [HttpPost]
        public IActionResult CalculateRent(DateTime startDate, DateTime endDate, decimal dailyRate, decimal weeklyRate, decimal monthlyRate)
        {
            // Ensure end date is not before start date
            if (endDate < startDate)
            {
                return Json(new { totalRent = 0, frequency = "Invalid" });
            }

            var totalDays = (endDate - startDate).TotalDays + 1; // +1 to include start date

            string frequency;
            decimal totalRent = 0;

            if (totalDays <= 5)
            {
                frequency = "Daily";
               // totalRent = (decimal)totalDays * dailyRate;
            }
            else if (totalDays >= 6 && totalDays < 30)
            {
                frequency = "Weekly";
                var totalWeeks = Math.Ceiling(totalDays / 7); // round up partial weeks
               // totalRent = totalWeeks * weeklyRate;
            }
            else
            {
                frequency = "Monthly";
                var totalMonths = Math.Ceiling(totalDays / 30); // round up partial months
               // totalRent = totalMonths * monthlyRate;
            }

            return Json(new { totalRent, frequency });
        }


    }
}