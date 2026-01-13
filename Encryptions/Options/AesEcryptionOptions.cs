using Microsoft.AspNetCore.Mvc;

namespace Encryptions.Options;

public class AesEncryptionOptions
{
    public const string Position = "AesEncryption";
    public string Key { get; set; }
    public string Iv { get; set; }
    public string Salt { get; set; }
    public int Iterations { get; set; }

    public byte[] KeyHash { get; private set; }
    public byte[] IvHash { get; private set; }

    public void HashKeyIv(Func<int,string, byte[]> hasher)
    {
        KeyHash = hasher.Invoke(16, Key);
        IvHash = hasher.Invoke(16, Iv);
    }

    public static AesEncryptionOptions Default()
    {
        //Note: These are just example values for simplicity. Do not use in production.
        //Always use securely generated keys, IVs, and salts stored in a safe manner, i.e. user sectret manager or cloud key vaults.
        //See SeidoStackSimple project for correct implementation examples.
        return new AesEncryptionOptions
        {
            Key = "Vavpy2-kiwcus-hofqyz",
            Iv = "cawsys-rikWu8-hyvxob",
            Salt = "j78Y#p)/saREN!y3@",
            Iterations = 100
        };
    }
}