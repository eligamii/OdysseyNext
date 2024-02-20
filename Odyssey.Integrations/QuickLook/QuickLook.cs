using System.IO;
using System.IO.Pipes;
using System.Security.Principal;

// This is a slightly modified version of the Bridge.cs (from https://github.com/QL-Win/QuickLook/wiki/Develop%2C-build-and-integrate)
// To be easier to integrate into Odyssey and other projects.

// All the credit of this goes to Paddy Xu, Alexender Eder and every people who contribued to the QuickLook project
// See https://github.com/QL-Win and https://github.com/QL-Win/QuickLook/wiki/Develop%2C-build-and-integrate

namespace Odyssey.Integrations.QuickLook
{
    internal static class QuickLook
    {
        private static readonly string PipeName = "QuickLook.App.Pipe." + WindowsIdentity.GetCurrent().User?.Value;
        private const string Toggle = "QuickLook.App.PipeMessages.Toggle";

        public static bool Launch(string path)
        {
            string pipeMessage = Toggle;

            try
            {
                using (var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
                {
                    client.Connect(1000);

                    using (var writer = new StreamWriter(client))
                    {
                        writer.WriteLine($"{pipeMessage}|{path}");
                        writer.Flush();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}