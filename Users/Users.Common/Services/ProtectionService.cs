using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Users.Common.Services
{
    public static class ProtectionService
    {
        public static string Encrypt(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            data = new SHA256Managed().ComputeHash(data);
            return Encoding.UTF8.GetString(data);
        }
    }
}
