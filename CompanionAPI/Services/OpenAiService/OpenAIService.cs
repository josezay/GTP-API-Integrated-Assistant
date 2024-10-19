using CompanionAPI.Contracts.AppSettings;
using CompanionAPI.Services.ReportService;
using Microsoft.Extensions.Options;
using OpenAI.Assistants;
using OpenAI.Chat;
using System.Text.Json;

namespace CompanionAPI.Services.OpenAiService;

public class OpenAIService : IOpenAiService
{

#pragma warning disable OPENAI001
    private readonly AssistantClient _client;
    private readonly OpenAISettings _openAISettings;

    public OpenAIService(IOptions<OpenAISettings> settings)
    {
        _openAISettings = settings.Value;
        _client = new AssistantClient( apiKey: _openAISettings.ApiKey);
    }

    public async Task<string> CallAIAsync(string message)
    {
        // Define the initial prompt and context instructions
        string initialPrompt = "Calcule o consumo de nutrientes com base nas entradas fornecidas pelo usuário, e estimando (Não calculando) quanto de cada nutriente o alimento informado possui.\r\n\r\n# Steps\r\n\r\n1. Receba a lista de alimentos consumidas pelo usuário.\r\n2. Caso não seja informada a quantidade desses alimentos, faça uma estimativa da mesma.\r\n3. Identifique os nutrientes e suas quantidades estimadas em cada alimento, utilizando uma base de referência nutricional.\r\n4. Calcule o total de cada nutriente com base nas informações fornecidas.\r\n5. Compare o consumo total de nutrientes com as necessidades diárias recomendadas.\r\n\r\n# Output Format\r\n\r\nApós descoberta uma entrada de nutrientes forneça uma resposta em padrão JSON que inclua: \r\n- Nome do alimento.\r\n- Quantidade consumida.\r\n- Quantidades de cada nutriente.\r\n\r\n{\r\n  \"name\": \"nutrient_response\",\r\n  \"strict\": true,\r\n  \"schema\": {\r\n    \"type\": \"object\",\r\n    \"properties\": {\r\n      \"food_name\": {\r\n        \"type\": \"string\",\r\n        \"description\": \"The name of the food item.\"\r\n      },\r\n      \"quantity_consumed\": {\r\n        \"type\": \"number\",\r\n        \"description\": \"The amount of food consumed.\"\r\n      },\r\n      \"nutrients\": {\r\n        \"type\": \"object\",\r\n        \"description\": \"Nutrient content of the food.\",\r\n        \"properties\": {\r\n          \"calories\": {\r\n            \"type\": \"number\",\r\n            \"description\": \"The number of calories in the consumed quantity.\"\r\n          },\r\n          \"protein\": {\r\n            \"type\": \"number\",\r\n            \"description\": \"The amount of protein in the consumed quantity.\"\r\n          },\r\n          \"carbohydrates\": {\r\n            \"type\": \"number\",\r\n            \"description\": \"The amount of carbohydrates in the consumed quantity.\"\r\n          },\r\n          \"fats\": {\r\n            \"type\": \"number\",\r\n            \"description\": \"The amount of fats in the consumed quantity.\"\r\n          }\r\n        },\r\n        \"required\": [\r\n          \"calories\",\r\n          \"protein\",\r\n          \"carbohydrates\",\r\n          \"fats\"\r\n        ],\r\n        \"additionalProperties\": false\r\n      }\r\n    },\r\n    \"required\": [\r\n      \"food_name\",\r\n      \"quantity_consumed\",\r\n      \"nutrients\"\r\n    ],\r\n    \"additionalProperties\": false\r\n  }\r\n}\r\n\r\n# Examples\r\n\r\n**Input:** \r\n- Alimentos: [Maçã, Frango, 200g; Pizza]\r\n\r\n**Processamento**\r\n-Maçã tem 200g (estimativa), frango com 200g, e uma pizza tem 400g (estimativa de uma pizza inteira).\r\n\r\n**Output:**\r\n- Nutriente: Vitamina C\r\n  - Quantidade consumida: X mg\r\n  - Porcentagem das necessidades diárias: Y%\r\n- Nutriente: Proteína\r\n  - Quantidade consumida: X g\r\n  - Porcentagem das necessidades diárias: Y%\r\n- Nutriente: Ferro\r\n  - Quantidade consumida: X mg\r\n  - Porcentagem das necessidades diárias: Y%\r\n\r\n# Notes\r\n\r\n- Não responder a nada não relacionado aos tópicos acima.\r\n";
        string fullPrompt = $"{initialPrompt}\n\nUsuário: {message}\nNutricionista:";

        // Define the function tool for saving nutrient report
        const string SaveNutrientReportFunctionName = "save_nutrient_report";

        FunctionToolDefinition saveNutrientReportTool = new()
        {
            FunctionName = SaveNutrientReportFunctionName,
            Description = "Save the nutrient report",
            Parameters = BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "food_name": {
                        "type": "string",
                        "description": "The name of the food item."
                    },
                    "quantity_consumed": {
                        "type": "number",
                        "description": "The amount of food consumed."
                    },
                    "nutrients": {
                        "type": "object",
                        "description": "Nutrient content of the food.",
                        "properties": {
                            "calories": {
                                "type": "number",
                                "description": "The number of calories in the consumed quantity."
                            },
                            "protein": {
                                "type": "number",
                                "description": "The amount of protein in the consumed quantity."
                            },
                            "carbohydrates": {
                                "type": "number",
                                "description": "The amount of carbohydrates in the consumed quantity."
                            },
                            "fats": {
                                "type": "number",
                                "description": "The amount of fats in the consumed quantity."
                            }
                        },
                        "required": [ "calories", "protein", "carbohydrates", "fats" ],
                        "additionalProperties": false
                    }
                },
                "required": [ "food_name", "quantity_consumed", "nutrients" ],
                "additionalProperties": false
            }
            """)
        };

        // Create an assistant that can call the function tools
        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Nutrient Calculation Assistant",
            Instructions = "Don't make assumptions about what values to plug into functions. Ask for clarification if a user request is ambiguous.",
            Tools = { saveNutrientReportTool }
        };

        Assistant assistant = _client.CreateAssistant(_openAISettings.Model, assistantOptions);

        // Create a thread with an initial user message and run it
        ThreadCreationOptions threadOptions = new()
        {
            InitialMessages = { fullPrompt }
        };

        ThreadRun run = await _client.CreateThreadAndRunAsync(assistant.Id, threadOptions);

        // Poll the run until it is no longer queued or in progress
        while (!run.Status.IsTerminal)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            run = await _client.GetRunAsync(run.ThreadId, run.Id);

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
                                var nutrientResponse = JsonSerializer.Deserialize<NutrientResponse>(argumentsJson.RootElement.GetRawText());

                                // Save the nutrient report
                                //_reportService.SaveNutrientReport(nutrientResponse);

                                toolOutputs.Add(new ToolOutput(action.ToolCallId, "Nutrient report saved successfully."));
                                break;
                            }

                        default:
                            {
                                // Handle other or unexpected calls
                                throw new NotImplementedException();
                            }
                    }
                }

                // Submit the tool outputs to the assistant, which returns the run to the queued state
                run = await _client.SubmitToolOutputsToRunAsync(run.ThreadId, run.Id, toolOutputs);
            }
        }

        // With the run complete, return the final message content
        if (run.Status == RunStatus.Completed)
        {
            var messages = _client.GetMessagesAsync(run.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });

            //foreach (var message in messages)
            //{
            //    if (message.Role == MessageRole.Assistant)
            //    {
            //        return message.Content[0].Text;
            //    }
            //}
        }

        throw new NotImplementedException(run.Status.ToString());
    }
}

// Define the NutrientResponse class to match the expected JSON structure
public class NutrientResponse
{
    public string FoodName { get; set; }
    public double QuantityConsumed { get; set; }
    public Nutrients Nutrients { get; set; }
}

public class Nutrients
{
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Carbohydrates { get; set; }
    public double Fats { get; set; }
}
