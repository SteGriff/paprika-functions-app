namespace PaprikaFunctionsApp.Common
{
    public class CodeAndMessage
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public CodeAndMessage(int code, string message)
        {
            Code = code;
            Message = message;
        }
        
    }
}
