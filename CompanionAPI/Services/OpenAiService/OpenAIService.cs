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
        _client = new AssistantClient(apiKey: _openAISettings.ApiKey);
    }

    public string CallAI(string message)
    {
        // Define the initial prompt and context instructions  
        string initialPrompt = "Calcule o consumo de nutrientes com base nas entradas fornecidas pelo usuário, e estimando (Não calculando) quanto de cada nutriente o alimento informado possui.\r\n\r\n# Steps\r\n\r\n1. Receba a lista de alimentos consumidas pelo usuário.\r\n2. Caso não seja informada a quantidade desses alimentos, faça uma estimativa da mesma.\r\n3. Identifique os nutrientes e suas quantidades estimadas em cada alimento, utilizando uma base de referência nutricional.\r\n4. Calcule o total de cada nutriente com base nas informações fornecidas.\r\n5. Compare o consumo total de nutrientes com as necessidades diárias recomendadas.\r\n\r\n# Output Format\r\n\r\nApós descoberta uma entrada de nutrientes forneça uma resposta em padrão JSON que inclua: \r\n- Nome do alimento.\r\n- Quantidade consumida.\r\n- Quantidades de cada nutriente.\r\n\r\n{\r\n  \"datetime\": \"2024-10-01T12:00:00Z\",\r\n  \"name\": \"Chicken Breast\",\r\n  \"quantity\": \"200g\",\r\n  \"calories\": 330,\r\n  \"proteins\": 62\r\n}\r\n\r\n# Examples\r\n\r\n**Input:** \r\n- Alimentos: [Maçã, Frango, 200g; Pizza]\r\n\r\n**Processamento**\r\n-Maçã tem 200g (estimativa), frango com 200g, e uma pizza tem 400g (estimativa de uma pizza inteira).\r\n\r\n**Output:**\r\n- Nutriente: Vitamina C\r\n  - Quantidade consumida: X mg\r\n  - Porcentagem das necessidades diárias: Y%\r\n- Nutriente: Proteína\r\n  - Quantidade consumida: X g\r\n  - Porcentagem das necessidades diárias: Y%\r\n- Nutriente: Ferro\r\n  - Quantidade consumida: X mg\r\n  - Porcentagem das necessidades diárias: Y%\r\n\r\n# Notes\r\n\r\n- Não responder a nada não relacionado aos tópicos acima.\r\n";
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
                       "datetime": {  
                           "type": "string",  
                           "description": "The date and time of the food consumption."  
                       },  
                       "name": {  
                           "type": "string",  
                           "description": "The name of the food item."  
                       },  
                       "quantity": {  
                           "type": "string",  
                           "description": "The amount of food consumed."  
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
                   "required": [ "datetime", "name", "quantity", "calories", "proteins" ],  
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

                                var nutrientResponse = JsonSerializer.Deserialize<NutrientResponse>(argumentsJson.RootElement.GetRawText(), options);

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
}

// Define the NutrientResponse class to match the expected JSON structure
public class NutrientResponse
{
    public DateTime DateTime { get; set; }
    public string Name { get; set; }
    public string Quantity { get; set; }
    public double Calories { get; set; }
    public double Proteins { get; set; }
}