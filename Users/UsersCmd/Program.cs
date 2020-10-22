using System;
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
            var filePathRegister = @".\Data\UserRegister.csv";
            var filePathCredentials = @".\Data\UserCredentials.csv";
            var userService = new UserService(filePathRegister, filePathCredentials);
            bool userNotInlogged = true;
            while (userNotInlogged)
            {
                Console.Write("Vill du logga in? (ja, nej): ");
                if (Console.ReadLine() == "ja")
                {
                    Console.Write("Skriv din mejladress: ");
                    var userInputEmail = Console.ReadLine();
                    if (userService.Get(userInputEmail) == null)
                    {
                        Console.WriteLine("Mejladressen finns inte i register, vill du registrera dig först? (ja, nej): ");
                        if (Console.ReadLine() == "ja")
                            RegisterNewUser(userService);
                    }
                    else
                    {
                        LogginUser(userService, userInputEmail, ref userNotInlogged);
                    }
                }
                else
                {
                    Console.Write("Vill du registrera dig? (ja, nej): ");
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
            var newUser = new User();
            do
            {
                newUser.Id = Guid.NewGuid();
                Console.Write("Skriv ditt namn: ");
                newUser.Name = Console.ReadLine();
                Console.Write("Skriv din mejladress: ");
                newUser.Email = Console.ReadLine();

                if (userService.Get(newUser.Email) != null)
                {
                    Console.WriteLine("Mejladressen finns redan registrerad");
                    registerRespons = new ServiceResponse() { Success = false };
                    continue;
                }
                Console.WriteLine("Lösenord.");

                userService.SetPassword(newUser.Id, ProtectionService.Encrypt(PasswordFactory(newUser.Email)));

                Console.Write("Skriv din telefonnummer: ");
                newUser.Phone = Console.ReadLine();
                Console.Write("Vill du prenumerera på nyhetsbrev? (ja, nej): ");
                newUser.SubscribeToNewsletter = Console.ReadLine() == "ja" ? true : false;

                if (userService.Get(newUser.Email) == null)
                {
                    registerRespons = userService.Register(newUser);
                    if (registerRespons.Success)
                        Console.WriteLine("Du är registrerad");
                }
                else
                {
                    Console.WriteLine("Mejladressen finns redan registrerad");
                    registerRespons = new ServiceResponse() { Success = false };
                    continue;
                }

            } while (!registerRespons.Success);
        }

        private static void LogginUser(UserService userService, string userInputEmail, ref bool userNotInlogged)
        {
            Console.Write("Skriv din lösenord: ");
            var userInputPassword = ProtectionService.Encrypt(MaskedPasswordInput());
            var loginRespons = userService.Login(userInputEmail, userInputPassword);

            while (!loginRespons.Success)
            {
                Console.Write("Det gick inte att logga in, vill du prova igen? (ja, nej): ");
                if (Console.ReadLine() == "ja")
                {
                    Console.Write("Skriv din lösenord: ");
                    userInputPassword = ProtectionService.Encrypt(MaskedPasswordInput());
                    loginRespons = userService.Login(userInputEmail, userInputPassword);
                }
                else break;
            }
            if (loginRespons.Success)
            {
                userNotInlogged = false;
                Console.WriteLine("Du är nu inloggad");
            }
        }

        private static string PasswordFactory(string email)
        {
            string newPassword = string.Empty;
            var passwordService = new PasswordService();
            Console.Write("Vill du generera lösenord?(ja, nej): ");
            if (Console.ReadLine() == "ja")
            {
                newPassword = passwordService.GeneratePassword(8);
                SimulateEmail(newPassword, email);
                return newPassword;
            }
            else
            {
                bool passwordNotSpellChecked = true;
                while (passwordNotSpellChecked)
                {
                    Console.Write("Skriv lösenord (8 tecken, 1 versal, 1 siffra och 1 special tecken): ");
                    newPassword = MaskedPasswordInput();
                    if (!passwordService.ValidatePassword(newPassword, 8).Success)
                    {
                        Console.WriteLine("Lösenord håller inte krav på 8 tecken, 1 versal, 1 siffra och 1 special tecken");
                        continue;
                    }
                    Console.Write("Skriv din lösenord igen: ");
                    if (newPassword == MaskedPasswordInput())
                        passwordNotSpellChecked = false;
                }
                return newPassword;
            }
        }

        private static string MaskedPasswordInput()
        {
            var password = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            Console.Write("\n");
            return password;
        }

        private static void SimulateEmail(string password, string email)
        {
            Console.WriteLine($"Skickar e-post till {email} med din lösenord som är {password}");
        }
    }
}