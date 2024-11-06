using CompanionAPI.Contracts.AIContracts;
using CompanionAPI.Contracts.AIContracts.Dtos;
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

    private const string UpdateWeightReportFunctionName = "update_weight_report";
    private FunctionToolDefinition updateWeightReportTool;

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

        updateWeightReportTool = new()
        {
            FunctionName = UpdateWeightReportFunctionName,
            Description = "Update the user weight in grams",
            Parameters = BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "weight": {
                        "type": "number",
                        "description": "The weight converted in grams, of the user provided weight."
                    }
                },
                "required": [ "weight" ],
                "additionalProperties": false
            }
            """)
        };
    }

    public ErrorOr<CreateAssistantResponse> CreateAssistant()
    {
        string assistantInstruction = "" +
            "Este assistente tem como objetivo receber reportes de quando o usuário se alimentar, ou fazer uma pesagem dele mesmo (usuário e não do alimento) para salvar no sistema o alimento que ingeriu, ou o peso atual do usuário para fins de atualização de cálculo de IMC." +
            "-Caso a entrada seja de um alimento consumido, calcule o consumo de nutrientes com base nas entradas de alimentação fornecidas pelo usuário, " +
            "estimando (não calculando) quanto de proteínas e calorias o alimento informado possui chamando save_nutrient_report. " +
            "# Steps " +
            "1. Receba a lista de alimentos (ou um alimento) consumidos pelo usuário. " +
            "2. Caso não seja informada a quantidade (peso, volume) desses alimentos, estime-a. " +
            "3. Identifique as calorias, proteínas e suas quantidades estimadas em cada alimento chamando a função save_nutrient_report, mas chamar save_nutrient_report somente se uma informação de consumo de alimento for informada, " +
            "passando o nome da comida consumida, a quantidade (volume, peso) estimada consumida, e as quantidades de proteínas e calorias totais que provavelmente o alimento possui. " +
            "4. Utilize uma base de dados confiável para obter as informações nutricionais dos alimentos. " +
            "-Caso a entrada seja de um informe de atualização de pesagem (da pessoa usuária que está perguntando, e não do alimento), como 'hoje me pesei e estou com 97 quilos' ou 'estou me pesando mais ou menos 50kg', chamar a função update_weight_report passando o peso convertido em gramas somente se uma informação de pesagem do usuário for informada. " +
            "Não chame a função update_weight_report se a entrada não estiver relacionada a uma atualização de peso da pessoa. Está tendo um bug, onde esta AI está chamando update_weight_report em uma query 'Comi um repolho' o que é incorreto.";


        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Nutrient Calculation Assistant, or Weigth report updater",
            Instructions = assistantInstruction,
            Tools = { saveNutrientReportTool, updateWeightReportTool }
        };

        var createAssistantResponse = new CreateAssistantResponse(_client.CreateAssistant(_openAISettings.Model, assistantOptions).Value.Id);

        return createAssistantResponse;
    }

    public async Task<ErrorOr<object>> CallAI(User user, string message)
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
                    using JsonDocument argumentsJson = JsonDocument.Parse(action.FunctionArguments);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    switch (action.FunctionName)
                    {
                        case SaveNutrientReportFunctionName:
                            {


                                var mealDto = JsonSerializer.Deserialize<MealDto>(argumentsJson.RootElement.GetRawText(), options);

                                if (mealDto != null)
                                {
                                    createdItems.Add(mealDto);
                                    toolOutputs.Add(new ToolOutput(action.ToolCallId, "Nutrient report saved successfully."));
                                }
                                break;
                            }

                        case UpdateWeightReportFunctionName:
                            {
                                var weightDto = JsonSerializer.Deserialize<WeightDto>(argumentsJson.RootElement.GetRawText(), options);


                                if (weightDto is not null && weightDto.weight is not null)
                                {
                                    createdItems.Add(weightDto);
                                    toolOutputs.Add(new ToolOutput(action.ToolCallId, "Weight report updated successfully."));
                                }

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

                if (toolOutputs.Any())
                {
                    run = _client.SubmitToolOutputsToRun(run.ThreadId, run.Id, toolOutputs);
                }
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
}