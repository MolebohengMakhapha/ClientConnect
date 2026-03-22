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

        private static string BuildAlphaPart(string name)
        {
            // Step 1 — Split into words and take the first letter of each word
            string[] words = name.ToUpperInvariant()
                                 .Split(new[] { ' ', '-', '_', '.' },
                                        StringSplitOptions.RemoveEmptyEntries);

            string letters = new string(
                words.Where(w => char.IsLetter(w[0]))
                     .Select(w => w[0])
                     .ToArray());

            // Step 2 — If we already have 3 or more initials, take the first 3
            if (letters.Length >= 3)
                return letters.Substring(0, 3);

            // Step 3 — If initials gave us fewer than 3 chars, 
            // pull in more letters from the full name to top up
            string allLetters = new string(
                name.ToUpperInvariant()
                    .Where(char.IsLetter)
                    .ToArray());

            foreach (char c in allLetters)
            {
                if (letters.Length >= 3) break;
                if (!letters.Contains(c))
                    letters += c;
            }

            // Step 4 — Still short? Pad with A, B, C...
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