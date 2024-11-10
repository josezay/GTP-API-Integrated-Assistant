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

    private const string ReportToolFunctionName = "report_tool";
    private FunctionToolDefinition _reportTool;

    public AIService(
        IOptions<OpenAISettings> settings)
    {
        _openAISettings = settings.Value;
        _client = new AssistantClient(apiKey: _openAISettings.ApiKey);

        InitializeAssistant();
    }

    private void InitializeAssistant()
    {
        var reportTool = new FunctionToolDefinition
        {
            FunctionName = ReportToolFunctionName,
            Description = "Process either a nutrient report, a weight update, or an activity report",
            Parameters = BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "reportType": {
                        "type": "string",
                        "description": "The type of report, either 'nutrient', 'weight', or 'activity'."
                    },
                    "nutrientReport": {
                        "type": "array",
                        "items": {
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
                                    "description": "The approximated number of calories in the food item."
                                },
                                "proteins": {
                                    "type": "number",
                                    "description": "The approximated number of proteins in the food item."
                                }
                            },
                            "required": [ "name", "quantity", "unit", "calories", "proteins" ],
                            "additionalProperties": false
                        }
                    },
                    "weightReport": {
                        "type": "object",
                        "properties": {
                            "weight": {
                                "type": "number",
                                "description": "The weight converted in grams, of the user provided weight."
                            }
                        },
                        "required": [ "weight" ],
                        "additionalProperties": false,
                        "nullable": true
                    },
                    "activityReport": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "name": {
                                    "type": "string",
                                    "description": "The name of the activity."
                                },
                                "durationInMinutes": {
                                    "type": "number",
                                    "description": "The duration of the activity in minutes."
                                },
                                "caloriesBurned": {
                                    "type": "number",
                                    "description": "The number of calories burned during the activity."
                                }
                            },
                            "required": [ "name", "durationInMinutes", "caloriesBurned" ],
                            "additionalProperties": false,
                            "nullable": true
                        }
                    }
                },
                "required": [ "reportType" ],
                "additionalProperties": false
            }
        """)
        };

        // Assign the combined tool to a class-level variable if needed
        _reportTool = reportTool;
    }


    public ErrorOr<CreateAssistantResponse> CreateAssistant()
    {
        string assistantInstruction = $"""
            Este assistente tem como objetivo receber reportes de quando o usuário se alimentar, fazer uma pesagem dele mesmo (usuário e não do alimento), ou registrar uma atividade física realizada pelo usuário para salvar no sistema.
            - Caso a entrada seja de um alimento consumido, calcule o consumo de nutrientes com base nas entradas de alimentação fornecidas pelo usuário como 'Comi um repolho',
            estimando (não calculando) quanto de proteínas e calorias o alimento informado possui chamando a função (tool) {ReportToolFunctionName} com o tipo de relatório (reportType) 'nutrient'.
            # Steps
            1. Receba a lista de alimentos (ou um alimento) consumidos pelo usuário.
            2. Caso não seja informada a quantidade (peso, volume) desses alimentos, estime-a.
            3. Identifique as calorias, proteínas e suas quantidades estimadas em cada alimento chamando a função (tool) {ReportToolFunctionName}, passando o nome da comida consumida, a quantidade (volume, peso) estimada consumida, e as quantidades de proteínas e calorias totais que provavelmente o alimento possui dentro do objeto nutrientReport.
            4. Utilize uma base de dados confiável para obter as informações nutricionais dos alimentos
            5. Chamar a tool {ReportToolFunctionName} passando o peso convertido em gramas dentro do objeto nutrientReport.
            6. Defina a propriedade reportType como 'nutrient' e preencha o objeto nutrientReport com as informações do alimento consumido.
            - Caso a entrada seja de um informe de atualização de pesagem (da pessoa usuária que está perguntando, e não do alimento), como 'hoje me pesei e estou com 97 quilos' ou 'estou me pesando mais ou menos 50kg', 
            1. Chamar a tool {ReportToolFunctionName} passando o peso convertido em gramas dentro do objeto weightReport.
            2. Defina a propriedade reportType como 'weight' e preencha o objeto weightReport com o peso convertido em gramas.
            - Caso a entrada seja de uma atividade física realizada pelo usuário, como 'corri 5km em 30 minutos' ou 'fiz 1 hora de musculação',
            1. Chamar a tool {ReportToolFunctionName} passando o nome da atividade, a duração em minutos e as calorias consumidas (gastas) dentro do objeto activityReport.
            2. Defina a propriedade reportType como 'activity' e preencha o objeto activityReport com as informações da atividade realizada.s
            - Caso a entrada não seja de acordo com nenhuma das opções anteriores e a mensagem não for compreendida: 
            1. Chamar a tool {ReportToolFunctionName} deixe todas as propriedades nulas, e reportType com o valor 'erro'
            """;

        var reportTool = new FunctionToolDefinition
        {
            FunctionName = ReportToolFunctionName,
            Description = "Process either a nutrient report, a weight update, or an activity report",
            Parameters = BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "reportType": {
                        "type": "string",
                        "description": "The type of report, either 'nutrient', 'weight', or 'activity'."
                    },
                    "nutrientReport": {
                        "type": "array",
                        "items": {
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
                                    "description": "The approximated number of calories in the food item."
                                },
                                "proteins": {
                                    "type": "number",
                                    "description": "The approximated number of proteins in the food item."
                                }
                            },
                            "required": [ "name", "quantity", "unit", "calories", "proteins" ],
                            "additionalProperties": false
                        }
                    },
                    "weightReport": {
                        "type": "object",
                        "properties": {
                            "weight": {
                                "type": "number",
                                "description": "The weight converted in grams, of the user provided weight."
                            }
                        },
                        "required": [ "weight" ],
                        "additionalProperties": false,
                        "nullable": true
                    },
                    "activityReport": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "name": {
                                    "type": "string",
                                    "description": "The name of the activity."
                                },
                                "durationInMinutes": {
                                    "type": "number",
                                    "description": "The duration of the activity in minutes."
                                },
                                "caloriesBurned": {
                                    "type": "number",
                                    "description": "The number of calories burned during the activity."
                                }
                            },
                            "required": [ "name", "durationInMinutes", "caloriesBurned" ],
                            "additionalProperties": false,
                            "nullable": true
                        }
                    }
                },
                "required": [ "reportType" ],
                "additionalProperties": false
            }
        """)
        };

        // Assign the combined tool to a class-level variable if needed
        _reportTool = reportTool;

        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Nutrient Calculation Assistant, Weight report updater, or Activity logger",
            Instructions = assistantInstruction,
            Tools = { _reportTool }
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
                        case ReportToolFunctionName:
                            {
                                var reportDto = JsonSerializer.Deserialize<ReportDto>(argumentsJson.RootElement.GetRawText(), options);

                                if (reportDto != null)
                                {
                                    createdItems.Add(reportDto);
                                    toolOutputs.Add(new ToolOutput(action.ToolCallId, "Report processed successfully."));
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