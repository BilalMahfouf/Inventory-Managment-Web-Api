using Isopoh.Cryptography.Argon2;


internal class Program
{

    public  static string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }
    public static bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return Argon2.Verify(hashedPassword, providedPassword);
    }
    public static void Main(string[] args)
    {
        Console.WriteLine(HashPassword("1234"));
        Console.WriteLine(VerifyPassword("ok", "1234"));
    }
}