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
            // Remove non-alpha characters and upper-case
            string letters = new string(
                name.ToUpperInvariant()
                    .Where(char.IsLetter)
                    .ToArray());

            if (letters.Length >= 3)
                return letters.Substring(0, 3);

            // Pad with A, B, C, ... until we have 3 chars
            char pad = 'A';
            while (letters.Length < 3)
            {
                letters += pad;
                pad = (char)(pad + 1);
            }

            return letters;
        }
    }
}