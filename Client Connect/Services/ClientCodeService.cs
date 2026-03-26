using Client_Connect.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Client_Connect.Services
{
    public class ClientCodeService
    {
        private readonly IClientRepository _clientRepo;

        public ClientCodeService(IClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
        }

        public string Generate(string clientName)
        {
            string alpha = BuildAlphaPart(clientName);

            for (int i = 1; i <= 999; i++)
            {
                string code = $"{alpha}{i:D3}";
                if (!_clientRepo.CodeExists(code))
                    return code;
            }

            throw new InvalidOperationException(
                $"Could not generate a unique client code for alpha prefix '{alpha}'.");
        }

        private static readonly HashSet<string> ConnectorWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "and", "or", "the", "of", "in", "at", "by", "for",
            "nor", "but", "so", "yet", "a", "an", "&"
        };

        private static string BuildAlphaPart(string name)
        {
            string cleaned = new string(name.Where(c => char.IsLetterOrDigit(c) || c == ' ').ToArray());

            string[] words = cleaned.ToUpperInvariant()
                                     .Split(new[] { ' ', '-', '_', '.' },
                                            StringSplitOptions.RemoveEmptyEntries);

            string[] meaningful = words
                .Where(w => char.IsLetter(w[0]) && !ConnectorWords.Contains(w))
                .ToArray();

            string letters = new string(meaningful.Select(w => w[0]).ToArray());

            if (letters.Length >= 3)
                return letters.Substring(0, 3);

            if (meaningful.Length >= 2)
            {
                string secondWord = meaningful[1];
                foreach (char c in secondWord)
                {
                    if (letters.Length >= 3) break;
                    if (!letters.Contains(c))
                        letters += c;
                }
            }

            if (letters.Length < 3)
            {
                string allLetters = new string(
                    cleaned.ToUpperInvariant()
                           .Where(char.IsLetter)
                           .ToArray());
                foreach (char c in allLetters)
                {
                    if (letters.Length >= 3) break;
                    if (!letters.Contains(c))
                        letters += c;
                }
            }

            char pad = 'A';
            while (letters.Length < 3)
            {
                if (!letters.Contains(pad))
                    letters += pad;
                pad = (char)(pad + 1);
            }

            return letters.Substring(0, 3);
        }
    }
}