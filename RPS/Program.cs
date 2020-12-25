using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace RPS
{
    class Program
    {
        private static bool CheckStartArgs(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Not enough arguments");
                return false;
            }
            if (args.Length % 2 == 0)
            {
                Console.WriteLine("Even args amount");
                return false;
            }
            if (args.Distinct().Count() != args.Length)
            {
                Console.WriteLine("Equal args");
                return false;
            }
            return true;
        }
        private static byte[] GenerateKey(int size)
        {
            var generator = RandomNumberGenerator.Create();
            var key = new byte[size];
            generator.GetBytes(key);
            return key;
        }
        private static int GenerateMove(int argsLength)
        {
            var generator = new Random();
            return generator.Next(argsLength);
        }
        private static byte[] StringEncode(string text)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(text);
        }
        private static byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }
        private static void ShowHash(byte[] hash)
        {
            Console.WriteLine("HMAC: " + BitConverter.ToString(hash).Replace("-", String.Empty));
        }
        private static void ShowKey(byte[] key)
        {
            Console.WriteLine("HMAC key: " + BitConverter.ToString(key).Replace("-", String.Empty));
        }
        private static int UserPart(string[] args)
        {
            while (true)
            {
                UserMenu(args);
                int userMove = UserAnswer(args.Length);
                if (userMove < 0) continue;
                if (userMove == 0) Environment.Exit(0);
                return userMove - 1;
            }            
        }
        private static void UserMenu(string[] args)
        {
            Console.WriteLine("Avaible moves:");
            for (int i = 0; i < args.Length; i++)
                Console.WriteLine("{0} - {1}", i + 1, args[i]);
            Console.WriteLine("0 - exit");
            Console.Write("Enter your move: ");
        }
        private static int UserAnswer(int max)
        {
            try
            {
                string input = Console.ReadLine();
                int answer = Convert.ToInt32(input);                
                if (answer > -1 && answer <= max) return answer;
                return -1;
            }
            catch
            {
                return -1;
            }
            
        }
        private static void Compare(int userMove, int machineMove, string[] args)
        {
            Console.WriteLine("Your move: {0}", args[userMove]);
            Console.WriteLine("Computer move: {0}", args[machineMove]);

            ShowResult(Result(userMove, machineMove, args.Length));
        }
        private static int Result(int userMove, int machineMove, int length)
        {
            if (userMove == machineMove) return 0;
            if (userMove > machineMove)
            {
                if (userMove - machineMove <= length / 2) return 1;
                else return -1;
            }
            if (machineMove - userMove <= length / 2) return -1;
            else return 1;
        }
        private static void ShowResult(int key)
        {
            Dictionary<int, string> resultStrings = new Dictionary<int, string>
            {
                [-1] = "You lose.",
                [0] = "Draw.",
                [1] = "You win!"
            };
            Console.WriteLine(resultStrings[key]);
        }
        static void Main(string[] args)
        {
            if (CheckStartArgs(args) == false) Environment.Exit(0);

            while (true)
            {
                var key = GenerateKey(16);
                var machineMove = GenerateMove(args.Length);
                var hash = HashHMAC(key, StringEncode(args[machineMove]));
                ShowHash(hash);
                var userMove = UserPart(args);
                Compare(userMove, machineMove, args);
                ShowKey(key);
            }
        }
    }
}
