using WordPredictor.Models;

namespace WordPredictor
{
    public enum PredictionEngine
    {
        BST,
        LLM
    }

    public class NextWordPredictor
    {
        private const string modelSystemPrompt = "Provide predictions without explanations, answering only the predicted word. ONLY ONE WORD";
        private const string modelUserPrompt = "Given this input string '{0}' consisting of words provided on each line, predict the complete word from these words {1}";

        private static readonly object lockObject = new();
        private static NextWordPredictor instance;

        private List<string> ContextWords { get; } = [];

        public static NextWordPredictor Instance
        {
            get
            {
                lock (lockObject)
                {
                    instance ??= new NextWordPredictor();
                    return instance;
                }
            }
        }

        private BST BST { get; } = new();

        public void AddContextWord(string word)
        {
            ContextWords.Add(word);
            BST.Insert(word);
        }

        public void RemoveContextWord(string word)
        {
            ContextWords.Remove(word);
            BST.Remove(word);
        }

        public void ClearContextWords()
        {
            ContextWords.Clear();
            BST.Clear();
        }

        public async Task<string> Predict(string inputText, PredictionEngine predictionEngine)
        {
            switch (predictionEngine)
            {
                case PredictionEngine.BST:
                    return BST.PredictiveSearch(inputText);

                case PredictionEngine.LLM:

                    var userPrompt = string.Format(modelUserPrompt, inputText, string.Join('-', ContextWords));
                    var conversation = new Conversation();
                    conversation.AddSystemMessage(modelSystemPrompt);
                    conversation.AddUserMessage(userPrompt);

                    return await LLM.GetResponse(conversation: conversation, temperature: 0.2);

                default:
                    throw new ArgumentOutOfRangeException(nameof(predictionEngine), predictionEngine, null);
            }
        }

    }
}
