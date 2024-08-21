using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using WebApplication2.Entities;
using WebApplication2.Models;
using Newtonsoft.Json;
using OpenAI_API.Chat;

namespace WebApplication2.Services
{
    public class OpenAIService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OpenAIService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<string> GetTestResponse(string userInput, List<ChatMessage> conversationHistory)
        {
            var scope = _scopeFactory.CreateScope();
            var api = scope.ServiceProvider.GetRequiredService<OpenAIAPI>();

            conversationHistory.Add(new ChatMessage(ChatMessageRole.System, "Respond with a name for the bar."));

            var chatResponse = await api.Chat.CreateChatCompletionAsync(
                new ChatRequest
                {
                    Messages = conversationHistory
                }
            );

            var response = chatResponse.Choices[0].Message.Content;

            return response;
        }

        public async Task<string> GetRecipesResponse(string userInput, List<ChatMessage> conversationHistory)
        {
            var scope = _scopeFactory.CreateScope();
            var api = scope.ServiceProvider.GetRequiredService<OpenAIAPI>();

            List<ChatMessage> additionMessages = 
            [
                new ChatMessage(ChatMessageRole.System, "You can provide recipes for both well-known and classic cocktails, as well as create new, innovative recipes. Be creative and flexible in your suggestions."),
                new ChatMessage(ChatMessageRole.System, "Categories to consider for tagging recipes include: Classic, Tiki, Sours, Highballs, Lowballs, Martinis, Fizz/Collins, Punches, After-dinner, Shooter/Shot, and Modern/Signature."),
                new ChatMessage(ChatMessageRole.System, "Provide recipes in HTML format using the following structure: <html><body><!---Your recipe here including name, description, ingredients, instructions, and tags---></body></html>."),
                new ChatMessage(ChatMessageRole.User, userInput)
            ];
            
            conversationHistory.AddRange(additionMessages);

            var chatResponse = await api.Chat.CreateChatCompletionAsync(
                new ChatRequest
                {
                    Messages = conversationHistory
                }
            );

            var response = chatResponse.Choices[0].Message.Content;

            return response;
        }

        public async Task<string> SaveResponseToDatabase(string userInput)
        {
            var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var api = scope.ServiceProvider.GetRequiredService<OpenAIAPI>();

            List<ChatMessage> conversationHistory =
            [
                    new ChatMessage(ChatMessageRole.User, userInput),
                    new ChatMessage(ChatMessageRole.User, "Please convert these recipes in the form of a valid JSON object with the following structure: " +
                                                           "{\"recipes\": [{\"name\": \"\", \"description\": \"\", \"ingredients\": [\"\"], \"instructions\": [\"\"], \"tags\": [\"\"]}]}.")
            ];

            var chatResponse = await api.Chat.CreateChatCompletionAsync(
                new ChatRequest
                {
                    Messages = conversationHistory
                }
            );

            var response = chatResponse.Choices[0].Message.Content;

            List<RecipeModel>? recipes = null;
            try
            {
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, List<RecipeModel>>>(response);
                recipes = jsonObject["recipes"];
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                Console.WriteLine($"Response: {response}");
            }

            if (recipes != null)
            {
                foreach (var recipe in recipes)
                {
                    Recipe newRecipe = new Recipe
                    {
                        RecipeId = Guid.NewGuid(),
                        Name = recipe.Name,
                        Description = recipe.Description,
                        Ingredients = recipe.Ingredients,
                        Instructions = recipe.Instructions,
                        Tags = recipe.Tags,
                        Status = Entities.RecipeStatusType.Testing
                    };
                    
                    dbContext.Recipe.Add(newRecipe);
                    await dbContext.SaveChangesAsync();
                }
            }
            
            return "Data saved.";
        }

        public async Task<string> GetSalesInsight(string userInput, List<ChatMessage> conversationHistory)
        {
            var scope = _scopeFactory.CreateScope();
            var api = scope.ServiceProvider.GetRequiredService<OpenAIAPI>();
            
            string salesData = await LoadSalesData(15);

            List<ChatMessage> additionMessages =
            [
                new ChatMessage(ChatMessageRole.System, salesData),
                new ChatMessage(ChatMessageRole.System, "These are the sales data in recent days."),
                new ChatMessage(ChatMessageRole.User, "Based on the sales data," + userInput)
            ];
            
            conversationHistory.AddRange(additionMessages);

            var chatResponse = await api.Chat.CreateChatCompletionAsync(
                new ChatRequest
                {
                    Messages = conversationHistory
                }
            );

            var response = chatResponse.Choices[0].Message.Content;
            
            return response;
        }

        public async Task<string> LoadSalesData(int days)
        {
            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();

            var startDate = DateTime.UtcNow.AddDays(-days);

            var records = await context.CustomerOrder
                .Where(co => co.CreatedDate >= startDate)
                .Include(co => co.Table)
                .Include(co => co.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();

            string jsonData = JsonConvert.SerializeObject(records, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            
            return jsonData;
        }
    }
}
