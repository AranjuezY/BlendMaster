using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using OpenAI_API;
using OpenAI_API.Chat;

namespace WebApplication2.Services
{
    public class ChatHub : Hub
    {
        private readonly OpenAIService _openAiService;
        private readonly OpenAIAPI _openAIAPI;
        private static ConcurrentDictionary<string, List<ChatMessage>> userConversations = new();

        public ChatHub(OpenAIAPI openAIAPI, OpenAIService openAiService)
        {
            _openAIAPI = openAIAPI;
            _openAiService = openAiService;
        }

        public async Task SendMessage(string userInput)
        {
            var connectionId = Context.ConnectionId;

            // Initialize conversation history if not present
            userConversations.GetOrAdd(connectionId, _ => new List<ChatMessage>
            {
                new ChatMessage(ChatMessageRole.System, "You are an experienced Bartender, a helpful assistant to provide instructions about cocktail making and bar management knowledge."),
            });

            // Add user's message to conversation history
            var conversationHistory = userConversations[connectionId];

            // Determine intent
            string intent = await DetermineIntent(userInput);
            
            string? botResponse;
            switch (intent)
            {
                case "RecipeRequest":
                    botResponse = await _openAiService.GetRecipesResponse(userInput, conversationHistory);
                    break;
                case "DatabaseSavingRequest":
                    var data = conversationHistory.Last().TextContent;
                    botResponse = await _openAiService.SaveResponseToDatabase(data);
                    break;
                case "GeneralQuery":
                    botResponse = await _openAiService.GetSalesInsight(userInput, conversationHistory);
                    break;
                default:
                    botResponse = "Not understood.";
                    break;
            }

            conversationHistory.Add(new ChatMessage(ChatMessageRole.Assistant, botResponse));
            
            foreach (var message in conversationHistory)
            {
                Console.WriteLine(message.Role + ": " + message.TextContent);
            }

            await Clients.Caller.SendAsync("ReceiveMessage", "Bartender", botResponse);
        }

        private async Task<string> DetermineIntent(string userInput)
        {
            // Use OpenAI API to classify the intent based on the user input
            var intentRequest = new ChatRequest
            {
                Messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, "You are a classifier that detects the user's intent based on their input. "),
                    new ChatMessage(ChatMessageRole.System, "A user's intent may be one of these options: " +
                                                            "asking for recipes, saving recipes into database, or other intents."),
                    new ChatMessage(ChatMessageRole.System, "Give your response as simple classification."),
                    new ChatMessage(ChatMessageRole.User, userInput)
                }
            };

            var intentResponse = await _openAIAPI.Chat.CreateChatCompletionAsync(intentRequest);
            var intentContent = intentResponse.Choices[0].Message.Content;
            Console.WriteLine("DetermineIntent: " + intentContent);

            // Simplify the response to match expected intents
            if (intentContent.ToLower().Contains("save") || intentContent.ToLower().Contains("saving"))
            {
                return "DatabaseSavingRequest";
            }
            
            if (intentContent.ToLower().Contains("recipe"))
            {
                return "RecipeRequest";
            }
            
            return "GeneralQuery";
        }
    }
}
