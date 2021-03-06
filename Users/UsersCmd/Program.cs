﻿using System;
using Users.Common.Models;
using Users.Common.Services;

namespace Users.Cmd
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Med testdriven utvecklingsmetodik.
            // Skriv ett program som:
            // 1. Loggar in en användare.
            // 2. Om användaren inte existerar, registrerar användaren via inmatning och
            //    låter användaren ange in ett lösenord,
            //    alt. genererar ett lösenord.
            // 3. Direkt efter ny användare registrerats, skall användaren kunna logga in (inloggning får ej ske automatiskt).
            // 4. Om lösenord lagras i fil får detta ej ske i klartext.

            // Tilläggsvis är detta ett grovt förenklat sätt att jobba med användare.
            // Det är inte represe  ntativt för verkligheten, men innehåller delar som är jämförbara.

            // I verkligheten gäller:
            // Lösenord måste krypteras och får ej lagras i fritextform.
            // Lösenord får inte visas på skärm.
            // Lösenord har vanligtvis löjliga/svåra lösenordsregelkrav pga brute force-algoritmer.
            // Normalt skickar man en epost med länk till användaren som registrerats.
            // Användaren får efter klick på verifieringslänk ange ett lösenord i en maskad inmatning.

            var userService = new UserService();
            while (true)
            {
                Console.Write("Vill du logga in? (ja, nej): ");
                if (Console.ReadLine() == "ja")
                {
                    Console.Write("Skriv din epost: ");
                    var userInputEmail = Console.ReadLine();
                    if (userService.Get(userInputEmail) == null)
                    {
                        Console.WriteLine("Epost finns inte i register, vill du registrera dig först? (ja, nej): ");
                        if (Console.ReadLine() == "ja")
                            RegisterNewUser(userService);
                    }
                    else
                    {
                        LogginUser(userService, userInputEmail);
                    }
                }
                else
                {
                    Console.Write("Vill du registrera dig? (ja, nej) ");
                    if (Console.ReadLine() == "ja")
                    {
                        RegisterNewUser(userService);
                    }
                }
            }
        }

        private static void RegisterNewUser(UserService userService)
        {
            IServiceResponse registerRespons;
            do
            {
                var newUser = new User();
                Console.Write("Skriv din Namn: ");
                newUser.Name = Console.ReadLine();
                Console.Write("Skriv din epost: ");
                newUser.Email = Console.ReadLine();
                Console.Write("Skriv din lösenord: ");
                newUser.Password = PasswordFactory(newUser.Email);
                Console.Write("Skriv din telefonnummer: ");
                newUser.Phone = Console.ReadLine();
                Console.Write("Vill du prenumera på nyhetsbrev? (ja, nej): ");
                newUser.SubscribeToNewsletter = Console.ReadLine() == "ja" ? true : false;

                if (userService.Get(newUser.Email) == null)
                {
                    registerRespons = userService.Register(newUser);
                    if (registerRespons.Success)
                        Console.WriteLine("Du är registrerad");
                }
                else
                {
                    Console.WriteLine("Epost finns redan registrerad");
                    registerRespons = new ServiceResponse() { Success = false };
                }
            } while (!registerRespons.Success);
        }
        private static void LogginUser(UserService userService, string userInputEmail)
        {
            Console.Write("Skriv din lösenord: ");
            var userInputPassword = MaskedPasswordInput();
            var loginRespons = userService.Login(userInputEmail, userInputPassword);
            while (!loginRespons.Success)
            {
                Console.WriteLine("Det gick inte att logga in, prova igen");
                Console.Write("Skriv din lösenord: ");
                userInputPassword = Console.ReadLine();
                loginRespons = userService.Login(userInputEmail, userInputPassword);
            }
            Console.WriteLine("Du är inlogad");
        }

        private static string PasswordFactory(string email)
        {
            string newPassword;
            Console.Write("Vill du autogenerera lösenord? (ja, nej)");
            if (Console.ReadLine() == "ja")
            {
                var passwordService = new PasswordService();
                newPassword = passwordService.GeneratePassword(8);
                SimulateEmail(newPassword, email);
                return newPassword;
            }
            else
            {
                do
                {
                    Console.Write("Skriv lösenord (minst 8 tecken...)");
                    newPassword = MaskedPasswordInput();
                    Console.Write("Skriv din lösenord igen");
                } while (newPassword != MaskedPasswordInput());
                return newPassword;
            }
        }
        private static string MaskedPasswordInput()
        {
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            Console.Write("\n");
            return pass;
        }

        private static void SimulateEmail(string password, string email)
        {
            Console.WriteLine($"Skickade melj till {email} med din lösenord som är {password}");
        }
            
    }
}