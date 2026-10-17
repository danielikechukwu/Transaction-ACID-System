using EF_TransactionImplementation.Data;
using EF_TransactionImplementation.DTOs;
using EF_TransactionImplementation.Entities;
using EF_TransactionImplementation.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EF_TransactionImplementation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        private readonly EF_TransactionDbContext _context;

        public OrdersController(EF_TransactionDbContext context)
        {
            _context = context;
        }

        // POST api/<OrdersController>
        [HttpPost("Place")]
        public async Task<IActionResult> PlaceOrderAsync([FromBody] OrderCreateDTO dto)
        {
            try
            {
                // STEP 1: Validate incoming request body
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Details = string.Join(" | ", errors)
                    });
                }

                // STEP 2: Load Customer + Address details
                var customer = await _context.Customers.Include(c => c.Addresses).FirstOrDefaultAsync(c => c.Id == dto.CustomerId && c.IsActive == true);

                if(customer == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Customer with ID {dto.CustomerId} not found or is inactive."
                    });
                };

                // Get shipping & billing addresses from customer's saved address list
                var shippingAddress = customer.Addresses.FirstOrDefault(a => a.Id == dto.ShippingAddressId);

                var billingAddress = customer.Addresses.FirstOrDefault(a => a.Id == dto.BillingAddressId);

                if(shippingAddress == null || billingAddress == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Invalid shipping or billing address."
                    });
                }

                // STEP 3: Load all required products in ONE database call
                var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();

                var products = await _context.Products.Where(p => productIds.Contains(p.Id) && p.IsActive == true).ToListAsync();

                if(products.Count != productIds.Count)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Some products do not exist or are inactive."
                    });
                }

                // STEP 4: Validate stock & prepare OrderItem list
                decimal orderTotal = 0; 

                List<OrderItem> orderItems = new List<OrderItem>();

                foreach (var dtoItem in dto.Items)
                {
                    var product = products.First(p => p.Id == dtoItem.ProductId);

                    if (product.StockQuantity < dtoItem.Quantity)
                    {
                        return BadRequest(new
                        {
                            Success = false,
                            Message = $"Insufficient stock for product {product.Name}"
                        });
                    }

                    // Calculate price for each line item
                    var lineTotal = product.FinalPrice * dtoItem.Quantity;

                    orderTotal += lineTotal;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = (int)dtoItem.Quantity,
                        UnitPrice = product.FinalPrice,
                        TotalPrice = lineTotal
                    });

                    // Reduce stock in-memory (DB update happens during SaveChanges)
                    product.StockQuantity -= (int)dtoItem.Quantity;
                }

                // STEP 5: Build Order entity
                var order = new Order
                {
                    CustomerId = customer.Id,
                    OrderNumber = Guid.NewGuid().ToString()[..8].ToUpper(),
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = orderTotal,
                    OrderStatus = OrderStatus.Pending,
                    ShippingAddress =
                        $"{shippingAddress.Line1}, {shippingAddress.City}, {shippingAddress.State}, {shippingAddress.Country}, {shippingAddress.ZipCode}",
                    BillingAddress =
                        $"{billingAddress.Line1}, {billingAddress.City}, {billingAddress.State}, {billingAddress.Country}, {billingAddress.ZipCode}"
                };

                await _context.Orders.AddAsync(order);

                foreach (var item in orderItems)
                {
                    item.Order = order; // Associate OrderItem with the Order
                    await _context.OrderItems.AddAsync(item);
                }

                // STEP 6: Create Payment entry (initially Pending)
                var payment = new Payment
                {
                    Order = order,
                    Amount = orderTotal,
                    PaymentStatus = PaymentStatus.Pending,
                    PaymentMethod = dto.Payment.PaymentMethod,
                    TransactionId = string.IsNullOrWhiteSpace(dto.Payment.TransactionId)
                        ? Guid.NewGuid().ToString()
                        : dto.Payment.TransactionId,
                    PaymentDate = DateTime.UtcNow
                };

                await _context.Payments.AddAsync(payment);

                // STEP 7: Add initial OrderHistory (Pending → Pending)
                await _context.OrderHistories.AddAsync(new OrderHistory
                {
                    Order = order,
                    OldStatus = OrderStatus.Pending,
                    NewStatus = OrderStatus.Pending,
                    Remarks = "Order created successfully"
                });

                // STEP 8: FIRST SaveChanges() → IMPLICIT TRANSACTION #1
                await _context.SaveChangesAsync();

                // STEP 9: Update payment + order status after payment confirmation
                var oldOrderStatus = order.OrderStatus;

                payment.PaymentStatus = PaymentStatus.Paid;

                order.OrderStatus = OrderStatus.PaymentReceived;

                await _context.OrderHistories.AddAsync(new OrderHistory
                {
                    Order = order,
                    OldStatus = OrderStatus.Pending,
                    NewStatus = OrderStatus.Pending,
                    Remarks = "Order created successfully"
                });

                // STEP 10: Teaching scenario — simulate failure
                if (dto.MakeSecondSaveFail)
                {
                    throw new Exception("Simulated failure after first SaveChanges.");
                }

                // STEP 11: SECOND SaveChanges() → IMPLICIT TRANSACTION #2
                await _context.SaveChangesAsync();

                // FINAL SUCCESS RESPONSE
                return Ok(new OrderResponseDTO
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    Message = "Order placed successfully using Implicit Transaction."
                });

            }
            catch (Exception ex)
            {
                // STEP 12: Log failure details into database
                try
                {
                    var logEntry = new FailureLog
                    {
                        OrderId = null,
                        MethodName = "PlaceOrderAsync",
                        ClassName = "OrdersController",
                        ErrorMessage = ex.Message,
                        StackTrace = ex.StackTrace ?? "",
                        RequestPayload = JsonSerializer.Serialize(dto),
                        Severity = "Error",
                        CustomerId = dto.CustomerId
                    };

                    _context.FailureLogs.Add(logEntry);

                    await _context.SaveChangesAsync();

                }
                catch
                {
                    // If logging fails we never throw — avoid recursive exceptions
                }

                // STEP 13: Send error response to client
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred while placing the order.",
                    Details = ex.Message
                });
            }
        }

    }
}
