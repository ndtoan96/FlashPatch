using System.Text.RegularExpressions;

namespace FlashPatch
{
    internal class InnerRemover
    {
        private readonly string prefix;
        private readonly char openChar;
        private readonly char closeChar;
        public InnerRemover(string prefix, char openChar, char closeChar)
        {
            this.prefix = prefix;
            this.openChar = openChar;
            this.closeChar = closeChar;
        }

        public string Process(string input)
        {
            Regex pattern = new(prefix + @"\s*" + Regex.Escape(openChar.ToString()));
            int startMatch = 0;
            string output = input;
            while (pattern.IsMatch(output, startMatch))
            {
                var match = pattern.Match(output, startMatch);
                int startPos = match.Index + match.Length;
                int endPos = FindCloseCharPosition(output, openChar, closeChar, startPos);
                if (endPos != -1)
                {
                    output = output.Remove(startPos, endPos - startPos);
                }
                startMatch = startPos;
            }
            return output;
        }

        private static int FindCloseCharPosition(string input, char openChar, char closeChar, int start)
        {
            int counter = 1;
            while (counter > 0 && start < input.Length)
            {
                if (input[start] == openChar)
                {
                    counter++;
                }
                else if (input[start] == closeChar)
                {
                    counter--;
                }
                start++;
            }
            if (counter == 0)
            {
                return start - 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
