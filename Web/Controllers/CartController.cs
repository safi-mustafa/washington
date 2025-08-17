using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Repositories.Common;
using ViewModels;
using ViewModels.Cart;
using ViewModels.CustomerProject;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> _inventoryService;
        private readonly ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> _transactionService;
        private readonly ICustomerProjectService<CustomerProjectModifyViewModel, CustomerProjectModifyViewModel, CustomerProjectDetailViewModel> _customerProjectService;
        private readonly IEquipmentTransactionService<EquipmentTransactionModifyViewModel, EquipmentTransactionModifyViewModel, EquipmentTransactionDetailViewModel> _equipmentTransactionService;

        //private readonly IDashboardService _service;

        public CartController(
            ILogger<CartController> logger
            , IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> inventoryService
            , ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> transactionService
            , ICustomerProjectService<CustomerProjectModifyViewModel, CustomerProjectModifyViewModel, CustomerProjectDetailViewModel> customerProjectService
            , IEquipmentTransactionService<EquipmentTransactionModifyViewModel, EquipmentTransactionModifyViewModel, EquipmentTransactionDetailViewModel> equipmentTransactionService
            )
        {
            _logger = logger;
            _inventoryService = inventoryService;
            _transactionService = transactionService;
            _customerProjectService = customerProjectService;
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
            CustomerProjectInformation customerProjectInformation = GetCustomerProjectInformation().Result;

            var sessionData = HttpContext.Session.GetString("Step2Data");
            if (!string.IsNullOrEmpty(sessionData))
            {
                var step2Data = JsonConvert.DeserializeObject<Step2Data>(sessionData);
                ViewBag.Step2Data = step2Data;
            }
            
            return View("Step2", customerProjectInformation);
        }

        public async Task<ActionResult> Step3()
        {
            Step3ViewData step3ViewData = new Step3ViewData();
            var step1SessionData = HttpContext.Session.GetString("Step1FormData");
            if (!string.IsNullOrEmpty(step1SessionData))
            {
                var formData = JsonConvert.DeserializeObject<Step1FormData>(step1SessionData);
                var orderItemIndexes = formData.Items
                .Select(item =>
                {
                    // Match pattern OrderItems[n]
                    var match = System.Text.RegularExpressions.Regex.Match(item.Name, @"OrderItems\[(\d+)\]");
                    return match.Success ? int.Parse(match.Groups[1].Value) : (int?)null;
                })
                .Where(index => index.HasValue)
                .Select(index => index.Value)
                .Distinct()
                .Count();
                step3ViewData.Items = orderItemIndexes;
                step3ViewData.EstimatedTotal = formData.TotalCost;

                step3ViewData.OrderData = new List<OrderData>();
                for (int i = 0; i < orderItemIndexes; i++)
                {
                    OrderData order = new OrderData();
                    order.ItemName = formData.Items.FirstOrDefault(x => x.Name == $"OrderItems[{i}].ItemName")?.Value;
                    order.Qty = int.Parse(formData.Items.FirstOrDefault(x => x.Name == $"OrderItems[{i}].Quantity")?.Value ?? "0");
                    order.ItemPrice = decimal.Parse(formData.Items.FirstOrDefault(x => x.Name == $"cartTotal_{i}")?.Value?.Replace("$", "") ?? "0");
                    order.Frequency = formData.Items.FirstOrDefault(x => x.Name == $"OrderItems[{i}].Frequency")?.Value;
                    if (DateTime.TryParse(formData.Items.FirstOrDefault(x => x.Name == $"OrderItems[{i}].StartDate")?.Value, out DateTime startDate))
                        order.StartDate = startDate;
                    if (DateTime.TryParse(formData.Items.FirstOrDefault(x => x.Name == $"OrderItems[{i}].EndDate")?.Value, out DateTime endDate))
                        order.EndDate = endDate;
                    order.EndDate = order.EndDate == null ? order.StartDate : order.EndDate;
                    step3ViewData.OrderData.Add(order);
                }

            }
            var step2sessionData = HttpContext.Session.GetString("Step2Data");
            if (!string.IsNullOrEmpty(step2sessionData))
            {
                var step2Data = JsonConvert.DeserializeObject<Step2Data>(step2sessionData);
                step3ViewData.JobName = step2Data.Project.JobName;
                step3ViewData.JobCode = step2Data.Project.JobCode;
            }
            return View("Step3",step3ViewData);
        }
        public async Task<ActionResult> Step4()
        {
            return View("Step4");
        }

        [HttpPost]
        public async Task<ActionResult> SubmitOrder()
        {
            try
            {
                // Get session data
                var step1SessionData = HttpContext.Session.GetString("Step1FormData");
                var step2SessionData = HttpContext.Session.GetString("Step2Data");
                
                if (string.IsNullOrEmpty(step1SessionData) || string.IsNullOrEmpty(step2SessionData))
                {
                    TempData["Error"] = "Session data is missing. Please start over.";
                    return RedirectToAction("Step1");
                }
                
                var step1Data = JsonConvert.DeserializeObject<Step1FormData>(step1SessionData);
                var step2Data = JsonConvert.DeserializeObject<Step2Data>(step2SessionData);
                
                // Create order model
                var orderModel = new OrderModifyViewModel
                {
                    Notes = step2Data.SpecialInstructions,
                    OrderItems = new List<OrderItemModifyViewModel>()
                };
                
                // Parse order items from step1 data
                var orderItemIndexes = step1Data.Items
                    .Select(item =>
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(item.Name, @"OrderItems\[(\d+)\]");
                        return match.Success ? int.Parse(match.Groups[1].Value) : (int?)null;
                    })
                    .Where(index => index.HasValue)
                    .Select(index => index.Value)
                    .Distinct();
                double? totalCost = 0;
                foreach (var index in orderItemIndexes)
                {
                    double? individualCost = 0;
                    var inventoryIdStr = step1Data.Items.FirstOrDefault(x => x.Name == $"OrderItems[{index}].Inventory.Id")?.Value;
                    var equipmentIdStr = step1Data.Items.FirstOrDefault(x => x.Name == $"OrderItems[{index}].Equipment.Id")?.Value;
                    var quantityStr = step1Data.Items.FirstOrDefault(x => x.Name == $"OrderItems[{index}].Quantity")?.Value;
                    var tempPrice = decimal.Parse(step1Data.Items.FirstOrDefault(x => x.Name == $"cartTotal_{index}")?.Value?.Replace("$", "") ?? "0");
                    var frequency = step1Data.Items.FirstOrDefault(x => x.Name == $"OrderItems[{index}].Frequency")?.Value;

                    DateTime? startDate = null;
                    DateTime? endDate = null;

                    if (DateTime.TryParse(step1Data.Items.FirstOrDefault(x => x.Name == $"OrderItems[{index}].StartDate")?.Value, out DateTime sDate))
                        startDate = sDate;

                    if (DateTime.TryParse(step1Data.Items.FirstOrDefault(x => x.Name == $"OrderItems[{index}].EndDate")?.Value, out DateTime eDate))
                        endDate = eDate;

                    endDate ??= startDate; // if EndDate missing, use StartDate

                    if (long.TryParse(quantityStr, out long quantity) && quantity > 0)
                    {
                        var orderItem = new OrderItemModifyViewModel
                        {
                            Quantity = quantity,
                            StartDate = startDate,
                            EndDate = endDate
                        };

                        if (long.TryParse(inventoryIdStr, out long inventoryId) && inventoryId > 0)
                        {
                            orderItem.Inventory = new InventoryDetailViewModel { Id = inventoryId };
                        }
                        else if (long.TryParse(equipmentIdStr, out long equipmentId) && equipmentId > 0)
                        {
                            orderItem.Equipment = new EquipmentDetailViewModel { Id = equipmentId };
                        }
                        individualCost = (double?)tempPrice;
                        totalCost += individualCost;
                        orderModel.OrderItems.Add(orderItem);
                    }
                }
                orderModel.Cost = totalCost;
                string attachMentUrl = string.Empty;
                if(step2Data.AttachmentCount > 0)
                {
                    attachMentUrl = step2Data.AttachmentUrls[0];
                }
                long workStepid = step2Data.WorkOrderId;
                // Create order
                var result = await _customerProjectService.CreateOrder(orderModel, step2Data.CustomerProjectId, workStepid, attachMentUrl);
                
                if (result != null)
                {
                    // Clear session data
                    HttpContext.Session.Remove("Step1FormData");
                    HttpContext.Session.Remove("Step2Data");
                    HttpContext.Session.Remove("Cart");
                    
                    TempData["Success"] = "Order submitted successfully!";
                    return RedirectToAction("Step4");
                }
                else
                {
                    TempData["Error"] = "Failed to submit order. Please try again.";
                    return RedirectToAction("Step3");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting order");
                TempData["Error"] = "An error occurred while submitting the order.";
                return RedirectToAction("Step3");
            }
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
        public IActionResult CalculateRent(DateTime startDate, DateTime endDate,
    decimal onetime, decimal dailyRate, decimal weeklyRate, decimal monthlyRate,
    int quantity, string page)
        {
            decimal totalRent = 0;
            string frequency;

            if (page == "Inventory")
            {
                totalRent = quantity * onetime;
                frequency = "One-Time";
            }
            else
            {
                if (endDate < startDate)
                {
                    return Json(new { totalRent = 0, frequency = "Invalid" });
                }

                int totalDays = (int)((endDate - startDate).TotalDays) + 1; // inclusive

                if (totalDays <= 5)
                {
                    frequency = "Daily";
                    totalRent = totalDays * dailyRate;
                }
                else if (totalDays >= 6 && totalDays < 30)
                {
                    int fullWeeks = totalDays / 7;
                    int remainingDays = totalDays % 7;

                    frequency = "Weekly";
                    totalRent = (fullWeeks * weeklyRate) + (remainingDays * dailyRate);
                }
                else
                {
                    int fullMonths = totalDays / 30;
                    int remainingDays = totalDays % 30;

                    if (remainingDays == 0)
                    {
                        totalRent = fullMonths * monthlyRate;
                    }
                    else if (remainingDays >= 7)
                    {
                        int fullWeeks = remainingDays / 7;
                        int leftoverDays = remainingDays % 7;

                        totalRent = (fullMonths * monthlyRate)
                                  + (fullWeeks * weeklyRate)
                                  + (leftoverDays * dailyRate);
                    }
                    else
                    {
                        totalRent = (fullMonths * monthlyRate)
                                  + (remainingDays * dailyRate);
                    }
                    frequency = "Monthly";
                }
            }

            return Json(new { totalRent, frequency });
        }

        [HttpPost]
        public IActionResult SaveStep1Data([FromBody] List<Step1ItemData> items)
        {
            try
            {
                _logger.LogInformation($"Received {items?.Count ?? 0} items");
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        _logger.LogInformation($"Item: InvId={item.InventoryId}, EqId={item.EquipmentId}, Qty={item.Quantity}");
                    }
                }
                
                var cart = GetCartFromSession();
                
                foreach (var item in items)
                {
                    CartItem cartItem = null;
                    
                    if (item.InventoryId > 0)
                    {
                        cartItem = cart.InventoryItems.FirstOrDefault(x => x.InventoryId == item.InventoryId);
                    }
                    else if (item.EquipmentId > 0)
                    {
                        cartItem = cart.EquipmentItems.FirstOrDefault(x => x.EquipmentId == item.EquipmentId);
                    }
                    
                    if (cartItem != null)
                    {
                        cartItem.Quantity = item.Quantity;
                        cartItem.StartDate = item.StartDate;
                        cartItem.EndDate = item.EndDate;
                        cartItem.CalculatedTotal = item.CalculatedTotal;
                        cartItem.RentalFrequency = item.RentalFrequency;
                    }
                }
                
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost]
        public IActionResult SaveStep1DataSimple([FromBody] Step1FormData formData)
        {
            try
            {
                _logger.LogInformation($"Received form data with {formData?.Items?.Count ?? 0} items");
                _logger.LogInformation($"Total cost: {formData?.TotalCost}");
                
                if (formData?.Items != null)
                {
                    HttpContext.Session.SetString("Step1FormData", JsonConvert.SerializeObject(formData));
                }
                
                return Json(new { success = true, itemCount = formData?.Items?.Count ?? 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpGet]
        public IActionResult GetStep1Data()
        {
            try
            {
                var sessionData = HttpContext.Session.GetString("Step1FormData");
                if (!string.IsNullOrEmpty(sessionData))
                {
                    var formData = JsonConvert.DeserializeObject<Step1FormData>(sessionData);
                    return Json(new { success = true, data = formData });
                }
                return Json(new { success = false, message = "No session data found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveStep2Data(long CustomerId, long CustomerProjectId, long WorkOrderId, string SpecialInstructions, IFormFileCollection attachments)
        {
            try
            {
                _logger.LogInformation($"Saving Step2 data - Project: {CustomerProjectId}, Instructions: {SpecialInstructions}");
                
                var step2Data = new Step2Data
                {
                    CustomerId = CustomerId,
                    CustomerProjectId = CustomerProjectId,
                    WorkOrderId = WorkOrderId,
                    SpecialInstructions = SpecialInstructions,
                    AttachmentCount = attachments?.Count ?? 0
                };
                
                step2Data.Project = await _customerProjectService.GetProjectByid(step2Data.CustomerProjectId);
                
                // Save attachments and store URLs
                if (attachments != null && attachments.Count > 0)
                {
                    step2Data.AttachmentUrls = await SaveAttachments(attachments);
                }
                
                HttpContext.Session.SetString("Step2Data", JsonConvert.SerializeObject(step2Data));
                
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        private async Task<List<string>> SaveAttachments(IFormFileCollection attachments)
        {
            var attachmentUrls = new List<string>();
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Storgae", "uploads", "attachments");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            
            foreach (var file in attachments)
            {
                if (file.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    
                    // Store relative URL path
                    var urlPath = $"/Storage/uploads/attachments/{fileName}";
                    attachmentUrls.Add(urlPath);
                    
                    _logger.LogInformation($"Saved attachment: {fileName} at {urlPath}");
                }
            }
            
            return attachmentUrls;
        }
        
        [HttpGet]
        public IActionResult GetStep2Data()
        {
            try
            {
                var sessionData = HttpContext.Session.GetString("Step2Data");
                if (!string.IsNullOrEmpty(sessionData))
                {
                    var step2Data = JsonConvert.DeserializeObject<Step2Data>(sessionData);
                    return Json(new { success = true, step2Data = new { customerId = step2Data.CustomerId, customerProjectId = step2Data.CustomerProjectId, workOrderId = step2Data.WorkOrderId, specialInstructions = step2Data.SpecialInstructions, attachmentUrls = step2Data.AttachmentUrls } });
                }
                return Json(new { success = false, message = "No Step2 data found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ClearSessionData(long? inventoryId = null, long? equipmentId = null)
        {
            try
            {
                var cart = GetCartFromSession();
                bool hasItems = cart.InventoryItems.Any() || cart.EquipmentItems.Any();
                
                if (!hasItems)
                {
                    HttpContext.Session.Remove("Step1FormData");
                    HttpContext.Session.Remove("Step2Data");
                }
                else
                {
                    // Clear Step1 session data completely - it will be rebuilt when user navigates
                    HttpContext.Session.Remove("Step1FormData");
                }
                
                return Json(new { success = true, cartEmpty = !hasItems });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<CustomerProjectInformation> GetCustomerProjectInformation()
        {
            var customerProjectInformation = new CustomerProjectInformation();
            var companies = await _customerProjectService.GetCompanies();
            var projects = await _customerProjectService.GetProjects();
            var workOrders = await _customerProjectService.GetWorkOrders();
            return new CustomerProjectInformation
            {
                Companies = companies,
                Projects = projects,
                WorkOrders = workOrders
            };
        }
    }
    
    public class Step1ItemData
    {
        public long InventoryId { get; set; }
        public long EquipmentId { get; set; }
        public long Quantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal CalculatedTotal { get; set; }
        public string RentalFrequency { get; set; }
    }
    
    public class Step1FormData
    {
        public List<FormItem> Items { get; set; } = new();
        public string TotalCost { get; set; }
    }
    
    public class FormItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
    
    public class Step2Data
    {
        public long CustomerProjectId { get; set; }
        public long CustomerId { get; set; }
        public long WorkOrderId { get; set; }
        public string SpecialInstructions { get; set; }
        public int AttachmentCount { get; set; }
        public List<string> AttachmentUrls { get; set; } = new List<string>();
        public CustomerProject Project { get; set; }
    }

    public class Step3ViewData
    {
        public string JobName { get; set; } 
        public string JobCode { get; set; } 
        public int Items { get; set; } 
        public string EstimatedTotal { get; set; }
        public List<OrderData> OrderData { get; set; }  
    }
    public class OrderData
    {
        public string ItemName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Frequency { get; set; }
        public int Qty { get; set; }
        public decimal ItemPrice { get; set; }
    }
}