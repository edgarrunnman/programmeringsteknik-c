using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vowelapp
{
    class Program
    {
        static void Main(string[] args)
        {
            HashSet<char> vowels = new HashSet<char> { 'a', 'e', 'o', 'i', 'å', 'ö', 'ä', 'y', 'u'};
            while (true)
            {
                Console.WriteLine("Sriv ett ord eller mening!");
                string input = Console.ReadLine();
                StringBuilder output = new StringBuilder();
                int removedCount = 0;
                foreach (char letter in input)
                {
                    if (!vowels.Contains(char.ToLower(letter)))
                        output.Append(letter);
                    else removedCount++;
                }

                Console.WriteLine(output.ToString());
                Console.WriteLine(removedCount);
            }
            
        }
    }
}
