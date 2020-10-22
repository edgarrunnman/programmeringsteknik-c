using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Users.Common.Extensions;
using Users.Common.Models;

namespace Users.Common.Services
{
    public class UserService : IUserService
    {
        private string _filePathRegister { get; set; }
        private string _filePathCredentials { get; set; }

        public UserService(string filePathRegister, string filePathCredentials)
        {
            _filePathRegister = filePathRegister;
            _filePathCredentials = filePathCredentials;
        }

        public IUser Get(string email)
        {
            var users = ReadFile<User>(_filePathRegister);
            foreach (var user in users)
            {
                if (user.Email == email)
                    return user;
            }
            return null;
        }

        public IUser Get(Guid userId)
        {
            var users = ReadFile<User>(_filePathRegister);
            foreach (var user in users)
            {
                if (user.Id == userId)
                    return user;
            }
            return null;
        }

        public IServiceResponse Login(string email, string password)
        {
            var user = Get(email);
            var credentials = ReadFile<Credentials>(_filePathCredentials);
            foreach (var credential in credentials)
            {
                if (credential.Id == user.Id
                    && credential.Password == password)
                {
                    return new ServiceResponse()
                    {
                        Success = true
                    };
                }
            }
            return new ServiceResponse()
            {
                Success = false
            };
        }

        public IServiceResponse Register(IUser user)
        {

            WriteFile(_filePathRegister, user);
            return new ServiceResponse()
            {
                Success = true
            };
        }

        public IServiceResponse SetPassword(Guid userId, string password)
        {
            var credentials = new Credentials()
            {
                Id = userId,
                Password = password
            };
            WriteFile(_filePathCredentials, credentials);
            return new ServiceResponse()
            {
                Success = true
            };
        }

        private List<T> ReadFile<T>(string filePath)
        {
            List<T> items;
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ";";
                items = csv.GetRecords<T>().ToList();
            }
            return items;
        }
        private void WriteFile(string filePath, object item)
        {
            using (Stream stream = File.Open(filePath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ";";
                csv.Configuration.HasHeaderRecord = false;
                csv.WriteRecord(item);
                csv.NextRecord();
            }
        }

    }
}