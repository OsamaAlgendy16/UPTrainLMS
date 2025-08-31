using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using UPTrain.Models;

namespace UPTrain.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PaymentsController : Controller
    {
        private readonly StripeSettings _stripeSettings;

        public PaymentsController(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession(int courseId, string courseName, decimal amount)
        {
            var domain = "https://localhost:7205";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(amount * 100), 
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = courseName,
                    },
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = $"{domain}/Customer/Payments/Success?courseId={courseId}",
                CancelUrl = $"{domain}/Customer/Payments/Cancel",
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }
        public IActionResult Success(int courseId)
        {
        
            ViewBag.Message = $"Payment successful for Course ID: {courseId}";
            return View();
        }

        public IActionResult Cancel()
        {
            ViewBag.Message = "Payment was cancelled.";
            return View();
        }
    }
}

