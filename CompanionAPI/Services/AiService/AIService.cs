using CompanionAPI.Contracts.AIContracts;
using CompanionAPI.Entities;
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

    private const string SaveNutrientReportFunctionName = "save_nutrient_report";
    private FunctionToolDefinition saveNutrientReportTool;

    public AIService(
        IOptions<OpenAISettings> settings)
    {
        _openAISettings = settings.Value;
        _client = new AssistantClient(apiKey: _openAISettings.ApiKey);
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
                        "description": "The amount (in provided unit) of food consumed."  
                    },
                    "unit": {  
                        "type": "string",  
                        "description": "The unit of the quantity (e.g., ml, g)."  
                    },
                    "calories": {  
                        "type": "number",  
                        "description": "The aproximated number of calories in the food item."  
                    },  
                    "proteins": {  
                        "type": "number",  
                        "description": "The aproximated number of proteins in the food item."  
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
            "Calcule o consumo de nutrientes com base nas entradas de alimentação fornecidas pelo usuário, " +
            "estimando (não calculando) quanto de proteínas e calorias o alimento informado possui. " +
            "# Steps " +
            "1. Receba a lista de alimentos (ou um alimento) consumidos pelo usuário. " +
            "2. Caso não seja informada a quantidade (peso, volume) desses alimentos, estime-a. " +
            "3. Identifique as calorias, proteínas e suas quantidades estimadas em cada alimento chamando saveNutrientReportTool, " +
            "passando o nome da comida consumida, a quantidade (volume, peso) estimada consumida, e as quantidades de proteínas e calorias totais que provavelmente o alimento possui. " +
            "4. Utilize uma base de dados confiável para obter as informações nutricionais dos alimentos. " +
            "5. Apenas chame a saveNutrientReportTool e nada mais.\r\n";


        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Nutrient Calculation Assistant",
            Instructions = assistantInstruction,
            Tools = { saveNutrientReportTool }
        };

        var createAssistantResponse = new CreateAssistantResponse(_client.CreateAssistant(_openAISettings.Model, assistantOptions).Value.Id);

        return createAssistantResponse;
    }

    public async Task<ErrorOr<object>> CallAI(string message)
    {
        string fullPrompt = $"Usuário: {message}\nNutricionista:";

        Assistant assistant = _client.GetAssistant(_openAISettings.AssistantId);

        // Create a thread with an initial user message and run it  
        ThreadCreationOptions threadOptions = new()
        {
            InitialMessages = { fullPrompt }
        };

        ThreadRun run = _client.CreateThreadAndRun(assistant.Id, threadOptions);

        List<object> createdItems = new();

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

                                var createdMeal = Meal.Create(
                                        name: mealDto.Name,
                                        quantity: mealDto.Quantity,
                                        calories: mealDto.Calories,
                                        proteins: mealDto.Proteins,
                                        unit: mealDto.Unit);

                                createdItems.Add(createdMeal);

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

        if (run.Status == RunStatus.Completed)
        {
            if (createdItems.Any())
            {
                return createdItems;
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