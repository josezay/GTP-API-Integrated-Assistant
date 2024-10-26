using CompanionAPI.Contracts.AIContracts;
using CompanionAPI.Contracts.MealContracts;
using CompanionAPI.Entities;
using CompanionAPI.Services.UserServices.GoalService;
using CompanionAPI.Settings.AppSettings;
using ErrorOr;
using Microsoft.Extensions.Options;
using OpenAI.Assistants;
using System.Text.Json;

namespace CompanionAPI.Services.AiService;

public class AIService : IAIService
{
#pragma warning disable OPENAI001
    private readonly AssistantClient _client;
    private readonly OpenAISettings _openAISettings;
    private readonly IMealService _mealService;

    private const string SaveNutrientReportFunctionName = "save_nutrient_report";
    private FunctionToolDefinition saveNutrientReportTool;

    public AIService(
        IOptions<OpenAISettings> settings,
        IMealService mealService)
    {
        _openAISettings = settings.Value;
        _client = new AssistantClient(apiKey: _openAISettings.ApiKey);
        _mealService = mealService;
        InitializeAssistant();
    }

    private void InitializeAssistant()
    {
        saveNutrientReportTool = new()
        {
            FunctionName = SaveNutrientReportFunctionName,
            Description = "Save the nutrient report",
            Parameters = BinaryData.FromString("""  
            {  
               "type": "object",  
               "properties": {
                    "name": {  
                        "type": "string",  
                        "description": "The name of the food item."  
                    },  
                    "quantity": {  
                        "type": "number",  
                        "description": "The amount of food consumed."  
                    },
                    "unit": {  
                        "type": "string",  
                        "description": "The unit of the quantity (e.g., ml, g)."  
                    },
                    "calories": {  
                        "type": "number",  
                        "description": "The number of calories in the consumed quantity."  
                    },  
                    "proteins": {  
                        "type": "number",  
                        "description": "The amount of protein in the consumed quantity."  
                    }
               },  
               "required": [ "name", "quantity", "unit", "calories", "proteins" ],  
               "additionalProperties": false  
            }  
            """)
        };
    }

    public ErrorOr<CreateAssistantResponse> CreateAssistant()
    {
        string assistantInstruction = "" +
            "Calcule o consumo de nutrientes com base nas entradas fornecidas pelo usuário, " +
            " e estimando (Não calculando) quanto de cada nutriente o alimento informado possui. " +
            "# Steps " +
            "1. Receba a lista de alimentos consumidas pelo usuário. " +
            "2. Caso não seja informada a quantidade desses alimentos, as estime. " +
            "3. Identifique as calorias, proteínas e suas quantidades estimadas em cada alimento chamando saveNutrientReportTool " +
            "4. Apenas chamar a saveNutrientReportTool e mais nada além disso.\r\n";

        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Nutrient Calculation Assistant",
            Instructions = assistantInstruction,
            Tools = { saveNutrientReportTool }
        };

        var createAssistantResponse = new CreateAssistantResponse(_client.CreateAssistant(_openAISettings.Model, assistantOptions).Value.Id);

        return createAssistantResponse;
    }

    public async Task<ErrorOr<string>> CallAI(string userId, string message)
    {
        string fullPrompt = $"Usuário: {message}\nNutricionista:";

        Assistant assistant = _client.GetAssistant(_openAISettings.AssistantId);

        // Create a thread with an initial user message and run it  
        ThreadCreationOptions threadOptions = new()
        {
            InitialMessages = { fullPrompt }
        };

        ThreadRun run = _client.CreateThreadAndRun(assistant.Id, threadOptions);

        // Poll the run until it is no longer queued or in progress  
        while (!run.Status.IsTerminal)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            run = _client.GetRun(run.ThreadId, run.Id);

            // If the run requires action, resolve them  
            if (run.Status == RunStatus.RequiresAction)
            {
                List<ToolOutput> toolOutputs = new();

                foreach (RequiredAction action in run.RequiredActions)
                {
                    switch (action.FunctionName)
                    {
                        case SaveNutrientReportFunctionName:
                            {
                                using JsonDocument argumentsJson = JsonDocument.Parse(action.FunctionArguments);
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                };

                                var mealDto = JsonSerializer.Deserialize<MealDto>(argumentsJson.RootElement.GetRawText(), options);

                                // Use the MealDto to create a Meal instance
                                var mealrequest = new AddMealRequest(userId, mealDto.Name, mealDto.Quantity, mealDto.Calories, mealDto.Proteins, mealDto.Unit);

                                // Save the nutrient report 
                                await _mealService.AddMeal(userId, mealrequest);

                                toolOutputs.Add(new ToolOutput(action.ToolCallId, "Nutrient report saved successfully."));
                                break;
                            }

                        default:
                            {
                                var a = JsonSerializer.Deserialize<Dictionary<string, object>>(action.FunctionArguments);
                                var b = action.FunctionName;
                                throw new NotImplementedException();
                            }
                    }
                }

                // Submit the tool outputs to the assistant, which returns the run to the queued state  
                run = _client.SubmitToolOutputsToRun(run.ThreadId, run.Id, toolOutputs);
            }
        }

        // With the run complete, return the final message content  
        if (run.Status == RunStatus.Completed)
        {
            var messages = _client.GetMessages(run.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });

            foreach (var msg in messages)
            {
                if (msg.Role == MessageRole.Assistant)
                {
                    return msg.Content[0].Text;
                }
            }
        }

        throw new NotImplementedException(run.Status.ToString());
    }

    public class MealDto
    {
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Proteins { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
    }
}