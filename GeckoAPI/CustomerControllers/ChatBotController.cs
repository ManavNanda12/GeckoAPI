using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.cart;
using GeckoAPI.Service.category;
using GeckoAPI.Service.customer;
using GeckoAPI.Service.order;
using GeckoAPI.Service.plan;
using GeckoAPI.Service.product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/chatbot")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        #region Fields
        public readonly IPlanService _planService;
        public readonly IProductService _productService;
        public readonly ICategoryService _categoryService;
        public readonly ICustomerService _customerService;
        public readonly IOrderService _orderService;
        public readonly ICartService _cartService;
        public readonly IConfiguration _configuration;
        public readonly string _apiKey;
        private readonly string _modelName = "llama-3.3-70b-versatile";
        #endregion

        #region Constructor
        public ChatBotController(IPlanService planService, IProductService productService, ICategoryService categoryService,
                                 ICustomerService customerService, IOrderService orderService, ICartService cartService,
                                 IConfiguration configuration)
        {
            _planService = planService;
            _productService = productService;
            _categoryService = categoryService;
            _customerService = customerService;
            _orderService = orderService;
            _cartService = cartService;
            _apiKey = _configuration.GetValue<string>("ChatBotKeys:GrokKey");
        }
        #endregion

        #region Chatbot API
        [HttpPost("chat-agent")]
        public async Task<BaseAPIResponse<string>> ChatBot([FromBody] ChatRequest request)
        {
            var response = new BaseAPIResponse<string>();
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(60);

                var url = "https://api.groq.com/openai/v1/chat/completions";
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var messages = new List<object>();

                // System message
                // System message
                messages.Add(new
                {
                    role = "system",
                    content = $"You are a helpful gym ecommerce assistant. " +
                             $"The current user's customerId is {request.CustomerId}. " +
                             $"Remember everything the user tells you during this conversation, including their name or any personal details they share. " +
                             $"You have access to the following tools: " +
                             $"- get_plan_list: returns ALL available plans. Each plan has isCurrentPlan (bool), currentPeriodStart, currentPeriodEnd fields. " +
                             $"- get_active_plan: returns ONLY the user's current active plan with start date, expiry date, and plan details. " +
                             $"- get_products: returns the current active in stock product we have. " +
                             $"- get_categories: returns current active product categories we have. " +
                             $"- get_customer_detail: returns current logged in customer details. " +
                             $"- get_order_detail: returns current orders of customer. " +
                             $"- get_cart_detail: returns current active cart of customer. " +
                             $"IMPORTANT RULES: " +
                             $"1. If the user asks about their current plan, active plan, subscription, expiry date, start date, renewal date — ALWAYS call get_active_plan tool. " +
                             $"2. DO NOT call tools unless the user specifically asks for information that requires them. For greetings like 'hello' or 'hi', just respond naturally without calling any tools. " +
                             $"3. Never say you don't have access to plan or date information. You have tools for this — use them ONLY when needed. " +
                             $"4. currentPeriodStart is the plan start date. currentPeriodEnd is the expiry/renewal date. " +
                             $"5. Format dates in a human-friendly way like 'March 3, 2026'. " +
                             $"6. If isCurrentPlan is true, that is the user's active plan. " +
                             $"7. Be conversational and friendly. Don't volunteer information unless asked."
                });
                // Add conversation history
                if (request.History != null && request.History.Count > 0)
                {
                    var historyToAdd = request.History.Take(request.History.Count - 1).ToList();
                    foreach (var item in historyToAdd)
                    {
                        messages.Add(new
                        {
                            role = item.Role,
                            content = item.Content
                        });
                    }
                }

                // Add current message
                messages.Add(new
                {
                    role = "user",
                    content = request.Message
                });

                var maxIterations = 5;
                var iteration = 0;

                while (iteration < maxIterations)
                {
                    var requestBody = new
                    {
                        model = _modelName,
                        messages = messages,
                        tools = new[]
                        {
                            new
                            {
                                type = "function",
                                function = new
                                {
                                    name = "get_plan_list",
                                    description = "Get all available membership plans. Each plan has IsCurrentPlan field to identify active subscription.",
                                    parameters = new
                                    {
                                        type = "object",
                                        properties = new { },
                                        required = new string[] { }
                                    }
                                }
                            },
                            new
                            {
                                type = "function",
                                function = new
                                {
                                    name = "get_active_plan",
                                    description = "Get user's current active plan with start date, expiry date, and benefits.",
                                    parameters = new
                                    {
                                        type = "object",
                                        properties = new { },
                                        required = new string[] { }
                                    }
                                }
                            },
                            new
                            {
                                type = "function",
                                function = new
                                {
                                    name = "get_products",
                                    description = "Get all available products in stock.",
                                    parameters = new
                                    {
                                        type = "object",
                                        properties = new { },
                                        required = new string[] { }
                                    }
                                }
                            },
                            new
                            {
                                type = "function",
                                function = new
                                {
                                    name = "get_categories",
                                    description = "Get all product categories.",
                                    parameters = new
                                    {
                                        type = "object",
                                        properties = new { },
                                        required = new string[] { }
                                    }
                                }
                            },
                            new
                            {
                                type = "function",
                                function = new
                                {
                                    name = "get_customer_detail",
                                    description = "Get current logged in customer details.",
                                    parameters = new
                                    {
                                        type = "object",
                                        properties = new { },
                                        required = new string[] { }
                                    }
                                }
                            },
                            new
                            {
                                type = "function",
                                function = new
                                {
                                    name = "get_order_detail",
                                    description = "Get customer's order history.",
                                    parameters = new
                                    {
                                        type = "object",
                                        properties = new { },
                                        required = new string[] { }
                                    }
                                }
                            },
                            new
                            {
                                type = "function",
                                function = new
                                {
                                    name = "get_cart_detail",
                                    description = "Get current shopping cart details.",
                                    parameters = new
                                    {
                                        type = "object",
                                        properties = new { },
                                        required = new string[] { }
                                    }
                                }
                            }
                        },
                        tool_choice = "auto",
                        temperature = 0.7,
                        max_tokens = 1000
                    };

                    var jsonContent = new StringContent(
                        JsonSerializer.Serialize(requestBody),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var apiResponse = await httpClient.PostAsync(url, jsonContent);
                    var responseContent = await apiResponse.Content.ReadAsStringAsync();

                    if (!apiResponse.IsSuccessStatusCode)
                    {
                        response.Success = false;
                        response.Message = $"API error: {responseContent}";
                        return response;
                    }

                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var choice = result.GetProperty("choices")[0];
                    var message = choice.GetProperty("message");

                    // Check if there are tool calls
                    if (message.TryGetProperty("tool_calls", out var toolCalls) && toolCalls.GetArrayLength() > 0)
                    {
                        var toolCall = toolCalls[0];
                        var functionName = toolCall.GetProperty("function").GetProperty("name").GetString();
                        var toolCallId = toolCall.GetProperty("id").GetString();

                        // Execute function
                        var functionResult = await ExecuteFunction(functionName, request);
                        var functionResultJson = JsonSerializer.Serialize(functionResult);

                        // Add assistant message with tool call
                        messages.Add(new
                        {
                            role = "assistant",
                            content = (string)null,
                            tool_calls = new[]
                            {
                                new
                                {
                                    id = toolCallId,
                                    type = "function",
                                    function = new
                                    {
                                        name = functionName,
                                        arguments = "{}"
                                    }
                                }
                            }
                        });

                        // Add tool result
                        messages.Add(new
                        {
                            role = "tool",
                            tool_call_id = toolCallId,
                            content = functionResultJson
                        });

                        iteration++;
                    }
                    else if (message.TryGetProperty("content", out var content) && !content.ValueEquals("null"))
                    {
                        // Got final response
                        response.Data = content.GetString();
                        response.Success = true;
                        response.Message = "AI bot replied successfully.";
                        return response;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Unexpected response format";
                        return response;
                    }
                }

                response.Success = false;
                response.Message = "Max iterations reached without final response";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        private async Task<object> ExecuteFunction(string functionName, ChatRequest request)
        {
            try
            {
                switch (functionName)
                {
                    case "get_plan_list":
                    case "get_active_plan":
                        var plans = await _planService.GetPlanList(request.CustomerId);
                        if (plans == null || !plans.Any())
                            return new { message = "No plans available", data = new List<object>() };
                        return new { data = plans };

                    case "get_products":
                        var products = await _productService.GetCustomerProductList(0, 0);
                        if (products == null || !products.Any())
                            return new { message = "No products available", data = new List<object>() };
                        return new { data = products };

                    case "get_categories":
                        var categories = await _categoryService.GetCategoryList();
                        if (categories == null || !categories.Any())
                            return new { message = "No categories available", data = new List<object>() };
                        return new { data = categories };

                    case "get_customer_detail":
                        if (request.CustomerId == 0 || request.CustomerId == null)
                            return new { message = "Please login to check customer details." };
                        var customer = await _customerService.GetCustomerById(request.CustomerId);
                        if (customer == null)
                            return new { message = "Customer not found" };
                        return new { data = customer };

                    case "get_order_detail":
                        if (request.CustomerId == 0 || request.CustomerId == null)
                            return new { message = "Please login to check order details." };
                        var orders = await _orderService.GetOrderList(request.CustomerId);
                        if (orders == null || !orders.Any())
                            return new { message = "No orders found", data = new List<object>() };
                        return new { data = orders };

                    case "get_cart_detail":
                        var cart = await _cartService.GetCartContents(request.CartId, request.CustomerId);
                        if (cart == null)
                            return new { message = "Cart is empty" };
                        return new { data = cart };

                    default:
                        return new { error = $"Unknown function: {functionName}" };
                }
            }
            catch (Exception ex)
            {
                return new { error = $"Function execution error: {ex.Message}" };
            }
        }
        #endregion
    }
}