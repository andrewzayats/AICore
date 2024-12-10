using Newtonsoft.Json.Linq;
using System.Text;

namespace AiCoreApi.Common
{
    public class JsonToMarkdownConverter: IJsonToMarkdownConverter
    {
        public string ConvertJsonToMarkdown(string json)
        {
            var sb = new StringBuilder();
            var jToken = JToken.Parse(json);
            ProcessToken(jToken, sb, 0);
            return sb.ToString();
        }

        private void ProcessToken(JToken token, StringBuilder sb, int indentLevel)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    ProcessObject((JObject)token, sb, indentLevel);
                    break;
                case JTokenType.Array:
                    ProcessArray((JArray)token, sb, indentLevel);
                    break;
                default:
                    ProcessValue(token, sb, indentLevel);
                    break;
            }
        }

        private void ProcessObject(JObject obj, StringBuilder sb, int indentLevel)
        {
            foreach (var property in obj.Properties())
            {
                AppendIndent(sb, indentLevel);
                sb.Append("- **").Append(property.Name).Append("**: ");
                ProcessToken(property.Value, sb, indentLevel + 1);
            }
        }

        private void ProcessArray(JArray array, StringBuilder sb, int indentLevel)
        {
            foreach (var item in array)
            {
                AppendIndent(sb, indentLevel);
                sb.Append("- ");
                ProcessToken(item, sb, indentLevel + 1);
            }
        }

        private void ProcessValue(JToken token, StringBuilder sb, int indentLevel)
        {
            sb.Append(token.ToString()).AppendLine();
        }

        private void AppendIndent(StringBuilder sb, int indentLevel)
        {
            sb.Append(new string(' ', indentLevel * 2));
        }
    }

    public interface IJsonToMarkdownConverter
    {
        string ConvertJsonToMarkdown(string json);
    }

}
