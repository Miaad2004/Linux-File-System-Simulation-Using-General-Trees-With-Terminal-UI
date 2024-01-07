namespace WordPredictor.Models
{
    public class Conversation
    {
        public List<object> messages;

        public Conversation()
        {
            messages = [];
        }

        public void AddSystemMessage(string content)
        {
            messages.Add(new { role = "system", content });
        }

        public void AddUserMessage(string content)
        {
            messages.Add(new { role = "user", content });
        }

        public void AddAssistantMessage(string content)
        {
            messages.Add(new { role = "assistant", content });
        }
    }
}
