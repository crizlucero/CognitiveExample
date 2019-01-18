namespace CognitiveExample
{
    public class Constants
    {
        public static readonly string AuthenticationTokenEndpoint = "https://api.cognitive.microsoft.com/sts/v1.0";

        public static readonly string BingSpeechApiKey = "<APIKEY>";
        //Revisar la región del servicio
        public static readonly string SpeechRecognitionEndpoint = "https://westus.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1";
        public static readonly string AudioContentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";
        //Revisar la región del servicio
        public static readonly string SpeechAuthenticationTokenEndpoint = "https://westus.api.cognitive.microsoft.com/sts/v1.0";

        public static readonly string BingSpellCheckApiKey = "<APIKEY>";
        public static readonly string BingSpellCheckEndpoint = "https://api.cognitive.microsoft.com/bing/v7.0/SpellCheck";

        public static readonly string TextTranslatorApiKey = "<APIKEY>";
        public static readonly string TextTranslatorEndpoint = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0";

        public static readonly string FaceApiKey = "<APIKEY>";
        //Revisar la región del servicio
        public static readonly string FaceEndpoint = "https://eastus.api.cognitive.microsoft.com/face/v1.0";

        public static readonly string AudioFilename = "Todo.wav";

        public static readonly string JsonContentType = "application/json";
        public static readonly string OctetStreamContentType = "application/octet-stream";
    }
}
