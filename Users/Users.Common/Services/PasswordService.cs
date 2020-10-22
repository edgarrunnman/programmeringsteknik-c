using System;
using System.Data;
using System.Linq;
using Users.Common.Models;


namespace Users.Common.Services
{
    public class PasswordService : IPasswordService
    {
        private const string _letters = "abcdefghijklmnopqrstuvwxyz";
        private const string _numbers = "1234567890";
        private const string _special = "$%#@!*?;:^&";

        public string GeneratePassword(uint length, uint capitalLetters = 1, uint numbers = 1, uint specialChars = 1)
        {
            if (capitalLetters + numbers + specialChars > length)
                throw new ArgumentOutOfRangeException("Capital letters, numbers and special characters exceed length.");

            var buffer = new char[length];

            for (var i = 0; i < buffer.Length; i++)
            {
                var currentCharacter = GetRandomCharacter(_letters);

                if (i < capitalLetters)
                {
                    currentCharacter = char.ToUpper(currentCharacter);
                }
                else if (i >= length - specialChars)
                {
                    currentCharacter = GetRandomCharacter(_special);
                }
                else if (i >= length - numbers - specialChars)
                {
                    currentCharacter = GetRandomCharacter(_numbers);
                }
                buffer[i] = currentCharacter;
            }
            return new string(buffer);
        }

        public IServiceResponse ValidatePassword(string password, uint length, uint capitalLetters = 1, uint numbers = 1, uint specialChars = 1)
        {
            if (password.Length < length)
                return new ServiceResponse()
                {
                    Success = false
                };
            int countCapitalLetters = (int)capitalLetters;
            int countNumber = (int)numbers;
            int countSpecialChars = (int)specialChars;
            foreach (var charackter in password)
            {
                if (_letters.ToCharArray().Contains(charackter))
                    countCapitalLetters--;
                else if (_numbers.ToCharArray().Contains(charackter))
                    countNumber--;
                else if (_special.ToCharArray().Contains(charackter))
                    countSpecialChars--;
            }
            if (countCapitalLetters <= 0 && countNumber <= 0 && countSpecialChars <= 0)
                return new ServiceResponse()
                {
                    Success = true
                };
            return new ServiceResponse()
            {
                Success = false
            };
        }

        private char GetRandomCharacter(string input)
        {
            var position = new Random().Next(0, input.Length);
            return input[position];
        }
        
        
    }
}