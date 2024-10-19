using CompanionAPI.Contracts.AppSettings;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace CompanionAPI.Services.OpenAiService;

public class OpenIaService
{
    private readonly ChatClient _client;

    public OpenIaService(IOptions<OpenAISettings> settings)
    {
        var openAISettings = settings.Value;
        _client = new ChatClient(model: openAISettings.Model, apiKey: openAISettings.ApiKey);
    }

    public string GetResponse(string message)
    {
        // Define the initial prompt and context instructions
        string initialPrompt = "Calcule o consumo de nutrientes com base nas entradas fornecidas pelo usuário, e estimando (Não calculando) quanto de cada nutriente o alimento informado possui.\r\n\r\n# Steps\r\n\r\n1. Receba a lista de alimentos consumidas pelo usuário.\r\n2. Caso não seja informada a quantidade desses alimentos, faça uma estimativa da mesma.\r\n3. Identifique os nutrientes e suas quantidades estimadas em cada alimento, utilizando uma base de referência nutricional.\r\n4. Calcule o total de cada nutriente com base nas informações fornecidas.\r\n5. Compare o consumo total de nutrientes com as necessidades diárias recomendadas.\r\n\r\n# Output Format\r\n\r\nApós descoberta uma entrada de nutrientes forneça uma resposta em padrão JSON que inclua: \r\n- Nome do alimento.\r\n- Quantidade consumida.\r\n- Quantidades de cada nutriente.\r\n\r\n{\r\n  \"name\": \"nutrient_response\",\r\n  \"strict\": true,\r\n  \"schema\": {\r\n    \"type\": \"object\",\r\n    \"properties\": {\r\n      \"food_name\": {\r\n        \"type\": \"string\",\r\n        \"description\": \"The name of the food item.\"\r\n      },\r\n      \"quantity_consumed\": {\r\n        \"type\": \"number\",\r\n        \"description\": \"The amount of food consumed.\"\r\n      },\r\n      \"nutrients\": {\r\n        \"type\": \"object\",\r\n        \"description\": \"Nutrient content of the food.\",\r\n        \"properties\": {\r\n          \"calories\": {\r\n            \"type\": \"number\",\r\n            \"description\": \"The number of calories in the consumed quantity.\"\r\n          },\r\n          \"protein\": {\r\n            \"type\": \"number\",\r\n            \"description\": \"The amount of protein in the consumed quantity.\"\r\n          },\r\n          \"carbohydrates\": {\r\n            \"type\": \"number\",\r\n            \"description\": \"The amount of carbohydrates in the consumed quantity.\"\r\n          },\r\n          \"fats\": {\r\n            \"type\": \"number\",\r\n            \"description\": \"The amount of fats in the consumed quantity.\"\r\n          }\r\n        },\r\n        \"required\": [\r\n          \"calories\",\r\n          \"protein\",\r\n          \"carbohydrates\",\r\n          \"fats\"\r\n        ],\r\n        \"additionalProperties\": false\r\n      }\r\n    },\r\n    \"required\": [\r\n      \"food_name\",\r\n      \"quantity_consumed\",\r\n      \"nutrients\"\r\n    ],\r\n    \"additionalProperties\": false\r\n  }\r\n}\r\n\r\n# Examples\r\n\r\n**Input:** \r\n- Alimentos: [Maçã, Frango, 200g; Pizza]\r\n\r\n**Processamento**\r\n-Maçã tem 200g (estimativa), frango com 200g, e uma pizza tem 400g (estimativa de uma pizza inteira).\r\n\r\n**Output:**\r\n- Nutriente: Vitamina C\r\n  - Quantidade consumida: X mg\r\n  - Porcentagem das necessidades diárias: Y%\r\n- Nutriente: Proteína\r\n  - Quantidade consumida: X g\r\n  - Porcentagem das necessidades diárias: Y%\r\n- Nutriente: Ferro\r\n  - Quantidade consumida: X mg\r\n  - Porcentagem das necessidades diárias: Y%\r\n\r\n# Notes\r\n\r\n- Não responder a nada não relacionado aos tópicos acima.\r\n";
        string fullPrompt = $"{initialPrompt}\n\nUsuário: {message}\nNutricionista:";

        // Create a chat request with the full prompt
        ChatCompletion completion = _client.CompleteChat(fullPrompt);

        return completion.Content[0].Text;
    }
}
