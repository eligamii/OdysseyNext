namespace Odyssey.QuickActions.Objects
{
    public class Res
    {
        public bool Success { get; set; } = false;
        public string Output { get; set; } = null;
        public string Message { get; set; } = null;

        /// <summary>
        /// The result of a command
        /// </summary>
        /// <param name="success"></param>
        /// <param name="output">The output as a re-usable variable</param>
        /// <param name="message">Remarks or error message</param>
        public Res(bool success, string output = null, string message = null)
        {
            Success = success;
            Output = output;
            Message = message;
        }
    }
}
