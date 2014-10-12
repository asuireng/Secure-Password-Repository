﻿using CryptSharp.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Secure_Password_Repository.Settings;

namespace Secure_Password_Repository.Extensions
{
    public class EncryptionAndHashing
    {

        //used to store the Public and Private Keys after calling Generate_NewRSAKeys()
        //this is so that Retrieve_PublicKey and Retrieve_PrivateKey can be called
        private static RSACryptoServiceProvider rsakeys;

        ///<summary>
        ///Generates a new Public Key and a new corresponding Private Key for retrieval
        ///</summary>
        public static void Generate_NewRSAKeys()
        {
            rsakeys = new RSACryptoServiceProvider(1024);
            rsakeys.PersistKeyInCsp = false;
        }

        ///<summary>
        ///Destroys the RSA keys generated with Generate_NewRSAKeys()
        ///</summary>
        public static void Destroy_RSAKeys()
        {
            rsakeys.Clear();
            rsakeys.Dispose();
        }

        ///<summary>
        ///Retrieves the Public Key that was generated with Generate_NewRSAKeys()
        ///</summary>
        ///<returns>
        ///A XML formatted Public Key for use with the RSA algoritm
        ///</returns>
        public static async Task<string> Retrieve_PublicKey()
        {
            //if the rsa keys havent been generated, then generate a new set
            if (rsakeys == null)
                Generate_NewRSAKeys();

            return await Task.Run<string>(() => rsakeys.ToXmlString(false));
        }

        ///<summary>
        ///Retrieves the Private Key that was generated with Generate_NewRSAKeys()
        ///</summary>
        ///<returns>
        ///A XML formatted Private Key for use with the RSA algoritm
        ///</returns>
        public static async Task<string> Retrieve_PrivateKey()
        {
            //if the rsa keys havent been generated, then generate a new set
            if (rsakeys == null)
                Generate_NewRSAKeys();

            return await Task.Run<string>(() => rsakeys.ToXmlString(true));
        }

        ///<summary>
        ///Encrypts the supplied text using the RSA algorithm and the Public Key provided
        ///</summary>
        ///<param name="PlainText">
        ///The text to be encrypted
        ///</param>
        ///<param name="PublicKey">
        ///The public key to encrypt the data with
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public static string Encrypt_RSA(string PlainText, string PublicKey)
        {
            string encryptedtext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //import the provided public key
            rsa.FromXmlString(PublicKey);

            encryptedtext = rsa.Encrypt(PlainText.ToBytes(), true).ConvertToString();

            rsa.Clear();

            return encryptedtext;
        }

        ///<summary>
        ///Encrypts the supplied text using the RSA algorithm and the Public Key provided
        ///</summary>
        ///<param name="PlainText">
        ///Bytes of data to be encrypted
        ///</param>
        ///<param name="PublicKey">
        ///The public key to encrypt the data with
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public static string Encrypt_RSA(byte[] PlainText, string PublicKey)
        {
            string encryptedtext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //import the provided public key
            rsa.FromXmlString(PublicKey);

            encryptedtext = rsa.Encrypt(PlainText, true).ConvertToString();

            rsa.Clear();

            return encryptedtext;
        }

        ///<summary>
        ///Encrypts the supplied text using the RSA algorithm and the Public Key provided
        ///</summary>
        ///<param name="PlainText">
        ///The text to be encrypted
        ///</param>
        ///<param name="PublicKey">
        ///The public key to encrypt the data with
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public static byte[] Encrypt_RSA_To_Byte(string PlainText, string PublicKey)
        {
            byte[] encryptedtext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //import the provided public key
            rsa.FromXmlString(PublicKey);

            encryptedtext = rsa.Encrypt(PlainText.ToBytes(), true);

            rsa.Clear();

            return encryptedtext;
        }

        ///<summary>
        ///Encrypts the supplied text using the RSA algorithm and the Public Key provided
        ///</summary>
        ///<param name="PlainText">
        ///Bytes of data to be encrypted
        ///</param>
        ///<param name="PublicKey">
        ///The public key to encrypt the data with
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public static byte[] Encrypt_RSA_To_Bytes(byte[] PlainText, string PublicKey)
        {
            byte[] encryptedtext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //import the provided public key
            rsa.FromXmlString(PublicKey);

            encryptedtext = rsa.Encrypt(PlainText, true);

            rsa.Clear();

            return encryptedtext;
        }

        ///<summary>
        ///Decrypts the supplied text using the RSA algorithm and the Private Key provided
        ///</summary>
        ///<param name="EncryptedText">
        ///The text to be decrypted
        ///</param>
        ///<param name="PrivateKey">
        ///The private key to decrypt the data with
        ///</param>
        ///<returns>
        ///An string of decrypted data
        ///</returns>
        public static string Decrypt_RSA(string EncryptedText, string PrivateKey)
        {
            string plaintext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //load in the private key for decrypting
            rsa.FromXmlString(PrivateKey);

            plaintext = rsa.Encrypt(EncryptedText.ToBytes(), true).ConvertToString();

            rsa.Clear();

            return plaintext;
        }

        ///<summary>
        ///Decrypts the supplied text using Data Protection API.
        ///Important note: the supplied text is decrypted in-memory, so the maintain security Encrypt_DPAPI should be called after the decrypted text has been used.
        ///</summary>
        ///<param name="EncryptedText">
        ///The plain text to be EncryptedText
        ///</param>
        public static void Decrypt_DPAPI(ref byte[] EncryptedText)
        {
            ProtectedMemory.Unprotect(EncryptedText, MemoryProtectionScope.SameProcess);
        }

        ///<summary>
        ///Encrypts the supplied text using Data Protection API, the supplied text is decrypted in-memory.
        ///</summary>
        ///<param name="PlainText">
        ///The plain text to be encrypted
        ///</param>
        public static void Encrypt_DPAPI(ref byte[] PlainText)
        {

            //if the private key length isnt a multiple of 16, then make it so (requirment of DPAPI)
            if (PlainText.Length % 16 != 0)
                EncryptionAndHashing.Add_BytePadding(ref PlainText, 16 - (PlainText.Length % 16));

            ProtectedMemory.Protect(PlainText, MemoryProtectionScope.SameProcess);
        }

        ///<summary>
        ///Encrypts the supplied string using the AES 256 algoritm and returns a string of encrypted text
        ///</summary>
        ///<param name="PlainText">
        ///The plain text to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The key used to encrypt the text
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public static string Encrypt_AES256(string PlainText, string EncryptionKey)
        {

            byte[] BytesToEncrypt = PlainText.ToBytes();

            //clear the original text (for security)
            //PlainText = string.Empty;

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                //we need the key to be 32 chars long (256 bits)
                Add_StringPadding(ref EncryptionKey, 32 - EncryptionKey.Length);

                aesAlg.Key = EncryptionKey.ToBytes();
                aesAlg.IV = ApplicationSettings.Default.SystemInitilisationVector.ToBytes();
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, encryptor, CryptoStreamMode.Write))
                        {
                            //encrypt the bytes
                            CryptoStream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);
                            CryptoStream.FlushFinalBlock();
                            BytesToEncrypt = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                aesAlg.Clear();
            }

            return BytesToEncrypt.ConvertToString();
        }

        ///<summary>
        ///Encrypts the supplied string using the AES 256 algoritm and returns a string of encrypted text
        ///</summary>
        ///<param name="PlainText">
        ///The plain text to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///Byte array of the key used to encrypt the text
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public static string Encrypt_AES256(string PlainText, byte[] EncryptionKey)
        {

            byte[] BytesToEncrypt = PlainText.ToBytes();

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {

                if (EncryptionKey.Length > 32)
                    Array.Resize(ref EncryptionKey, 32);

                //we need the key to be 32 chars long (256 bits)
                Add_BytePadding(ref EncryptionKey, 32 - EncryptionKey.Length);

                aesAlg.Key = EncryptionKey;
                aesAlg.IV = ApplicationSettings.Default.SystemInitilisationVector.ToBytes();
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, encryptor, CryptoStreamMode.Write))
                        {
                            //encrypt the bytes
                            CryptoStream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);
                            CryptoStream.FlushFinalBlock();
                            BytesToEncrypt = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                aesAlg.Clear();
            }

            return BytesToEncrypt.ConvertToString();
        }

        ///<summary>
        ///Encrypts the supplied bytes using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToEncrypt">
        ///The bytes to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The key used to encrypt the text
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public static string Encrypt_AES256(byte[] BytesToEncrypt, string EncryptionKey)
        {
            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {

                //we need the key to be 32 chars long (256 bits)
                if (EncryptionKey.Length < 32)
                    Add_StringPadding(ref EncryptionKey, 32 - EncryptionKey.Length);
                else if (EncryptionKey.Length > 32)
                    EncryptionKey = EncryptionKey.Substring(0, 32);

                aesAlg.Key = EncryptionKey.ToBytes();
                aesAlg.IV = ApplicationSettings.Default.SystemInitilisationVector.ToBytes();
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, encryptor, CryptoStreamMode.Write))
                        {
                            //encrypt the bytes
                            CryptoStream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);
                            CryptoStream.FlushFinalBlock();
                            BytesToEncrypt = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                aesAlg.Clear();
            }

            return BytesToEncrypt.ConvertToString();
        }

        ///<summary>
        ///Encrypts the supplied bytes using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToEncrypt">
        ///The bytes to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The byte array of the key used to encrypt the text
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public static string Encrypt_AES256(byte[] BytesToEncrypt, byte[] EncryptionKey)
        {
            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {

                if (EncryptionKey.Length > 32)
                    Array.Resize(ref EncryptionKey, 32);

                //we need the key to be 32 chars long (256 bits)
                Add_BytePadding(ref EncryptionKey, 32 - EncryptionKey.Length);

                aesAlg.Key = EncryptionKey;
                aesAlg.IV = ApplicationSettings.Default.SystemInitilisationVector.ToBytes();
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, encryptor, CryptoStreamMode.Write))
                        {
                            //encrypt the bytes
                            CryptoStream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);
                            CryptoStream.FlushFinalBlock();
                            BytesToEncrypt = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                aesAlg.Clear();
            }

            return BytesToEncrypt.ConvertToString();
        }

        ///<summary>
        ///Encrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="PlainText">
        ///The plain text to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The key used to encrypt the text
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public static byte[] Encrypt_AES256_To_Bytes(string PlainText, string EncryptionKey)
        {
            byte[] BytesToEncrypt = PlainText.ToBytes();

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {

                //we need the key to be 32 chars long (256 bits)
                if (EncryptionKey.Length < 32)
                    Add_StringPadding(ref EncryptionKey, 32 - EncryptionKey.Length);
                else if (EncryptionKey.Length > 32)
                    EncryptionKey = EncryptionKey.Substring(0, 32);

                aesAlg.Key = EncryptionKey.ToBytes();
                aesAlg.IV = ApplicationSettings.Default.SystemInitilisationVector.ToBytes();
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, encryptor, CryptoStreamMode.Write))
                        {
                            //encrypt the bytes
                            CryptoStream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);
                            CryptoStream.FlushFinalBlock();
                            BytesToEncrypt = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                aesAlg.Clear();
            }

            return BytesToEncrypt;
        }

        ///<summary>
        ///Encrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="PlainText">
        ///The plain text to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///Byte array of the key used to encrypt the text
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public static byte[] Encrypt_AES256_To_Bytes(string PlainText, byte[] EncryptionKey)
        {

            byte[] BytesToEncrypt = PlainText.ToBytes();

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {

                if (EncryptionKey.Length > 32)
                    Array.Resize(ref EncryptionKey, 32);

                //we need the key to be 32 chars long (256 bits)
                Add_BytePadding(ref EncryptionKey, 32 - EncryptionKey.Length);

                aesAlg.Key = EncryptionKey;
                aesAlg.IV = ApplicationSettings.Default.SystemInitilisationVector.ToBytes();
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, encryptor, CryptoStreamMode.Write))
                        {
                            //encrypt the bytes
                            CryptoStream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);
                            CryptoStream.FlushFinalBlock();
                            BytesToEncrypt = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                aesAlg.Clear();
            }

            return BytesToEncrypt;
        }

        ///<summary>
        ///Encrypts the supplied bytes using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToEncrypt">
        ///The bytes to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The key used to encrypt the text
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public static byte[] Encrypt_AES256_To_Bytes(byte[] BytesToEncrypt, string EncryptionKey)
        {
            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {

                //we need the key to be 32 chars long (256 bits)
                if (EncryptionKey.Length < 32)
                    Add_StringPadding(ref EncryptionKey, 32 - EncryptionKey.Length);
                else if (EncryptionKey.Length > 32)
                    EncryptionKey = EncryptionKey.Substring(0, 32);

                aesAlg.Key = EncryptionKey.ToBytes();
                aesAlg.IV = ApplicationSettings.Default.SystemInitilisationVector.ToBytes();
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, encryptor, CryptoStreamMode.Write))
                        {
                            //encrypt the bytes
                            CryptoStream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);
                            CryptoStream.FlushFinalBlock();
                            BytesToEncrypt = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                aesAlg.Clear();
            }

            return BytesToEncrypt;
        }


        ///<summary>
        ///Encrypts the supplied bytes using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToEncrypt">
        ///The bytes to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The byte array of the key used to encrypt the text
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public static byte[] Encrypt_AES256_To_Bytes(byte[] BytesToEncrypt, byte[] EncryptionKey)
        {
            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {

                if (EncryptionKey.Length > 32)
                    Array.Resize(ref EncryptionKey, 32);

                //we need the key to be 32 chars long (256 bits)
                Add_BytePadding(ref EncryptionKey, 32 - EncryptionKey.Length);

                aesAlg.Key = EncryptionKey;
                aesAlg.IV = ApplicationSettings.Default.SystemInitilisationVector.ToBytes();
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, encryptor, CryptoStreamMode.Write))
                        {
                            //encrypt the bytes
                            CryptoStream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);
                            CryptoStream.FlushFinalBlock();
                            BytesToEncrypt = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                aesAlg.Clear();
            }

            return BytesToEncrypt;
        }


        ///<summary>
        ///Decrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="EncryptedText">
        ///The encrypted text to be decrypted
        ///</param>
        ///<param name="DecryptionKey">
        ///The key used to decrypt the text
        ///</param>
        ///<returns>
        ///A string of decrypted data
        ///</returns>
        public static string Decrypt_AES256(string EncryptedText, string DecryptionKey)
        {

            byte[] BytesToDecrypted = EncryptedText.ToBytes();

            int ByteCount = 0;

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {

                //we need the key to be 32 chars long (256 bits)
                if (DecryptionKey.Length < 32)
                    Add_StringPadding(ref DecryptionKey, 32 - DecryptionKey.Length);
                else if (DecryptionKey.Length > 32)
                    DecryptionKey = DecryptionKey.Substring(0, 32);

                aesAlg.Key = DecryptionKey.ToBytes();
                aesAlg.IV = ApplicationSettings.Default.SystemInitilisationVector.ToBytes();
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, decryptor, CryptoStreamMode.Read))
                        {
                            //decrypt the bytes
                            ByteCount = CryptoStream.Read(BytesToDecrypted, 0, BytesToDecrypted.Length);
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                aesAlg.Clear();
            }

            return BytesToDecrypted.ConvertToString();
        }

        ///<summary>
        ///Decrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToDecrypted">
        ///The encrypted bytes to be decrypted
        ///</param>
        ///<param name="DecryptionKey">
        ///The key used to decrypt the text
        ///</param>
        ///<returns>
        ///A string of decrypted data
        ///</returns>
        public static string Decrypt_AES256(byte[] BytesToDecrypted, string DecryptionKey)
        {

            int ByteCount = 0;

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                //we need the key to be 32 chars long (256 bits)
                if (DecryptionKey.Length < 32)
                    Add_StringPadding(ref DecryptionKey, 32 - DecryptionKey.Length);
                else if (DecryptionKey.Length > 32)
                    DecryptionKey = DecryptionKey.Substring(0, 32);

                aesAlg.Key = DecryptionKey.ToBytes();
                aesAlg.IV = ApplicationSettings.Default.SystemInitilisationVector.ToBytes();
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, decryptor, CryptoStreamMode.Read))
                        {
                            //decrypt the bytes
                            ByteCount = CryptoStream.Read(BytesToDecrypted, 0, BytesToDecrypted.Length);
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                aesAlg.Clear();
            }

            return BytesToDecrypted.ConvertToString();
        }

        ///<summary>
        ///Generates and returns a hash of the provided string using the SCrypt hashing algorithm
        ///</summary>
        ///<param name="Password">
        ///Password to be converted into a hash
        ///</param>
        ///<returns>
        ///A string of hash data
        ///</returns>
        public static string Hash_SCrypt(string Password)
        {
            byte[] saltbytes = ApplicationSettings.Default.SystemSalt.ToBytes();
            byte[] textbytes = Password.ToBytes();

            //generate the hash
            byte[] SCryptHash = SCrypt.ComputeDerivedKey(textbytes, saltbytes, 32768, 8, 1, null, ApplicationSettings.Default.SCryptHashCost.ToInt());

            //convert the text to an SCrypt hash, and then convert the hash to string
            return SCryptHash.ConvertToString();
        }

        ///<summary>
        ///Generates and returns a HMAC of the provided string using the SCrypt algorithm
        ///</summary>
        ///<param name="Password">
        ///Password to be converted into a HMAC
        ///</param>
        ///<param name="Salt">
        ///The salt, a unique salt means a unique SCrypt string
        ///</param>
        ///<returns>
        ///A HMAC converted to a string
        ///</returns>
        public static string Hash_SCrypt(string Password, string Salt)
        {
            byte[] saltbytes = Salt.ToBytes();
            byte[] textbytes = Password.ToBytes();

            //generate the hash
            byte[] SCryptHash = SCrypt.ComputeDerivedKey(textbytes, saltbytes, 32768, 8, 1, null, ApplicationSettings.Default.SCryptHashCost.ToInt());

            //convert the text to an SCrypt hash, and then convert the hash to string
            return SCryptHash.ConvertToString();
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Password to be converted into a HMAC
        /// </param>
        /// <returns>
        /// A HMAC converted to a string
        /// </returns>
        public static string Hash_PBKDF2(string Password)
        {
            byte[] salt;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            //generate a salt and a number of random bytes
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, 32, ApplicationSettings.Default.PBKDF2IterationCount.ToInt()))
            {
                salt = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(salt, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash.ConvertToString();
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string and salt using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Password to be converted into a HMAC
        /// </param>
        /// <param name="Salt">
        /// The salt, a unique salt means a unique PBKDF2 string
        /// </param>
        /// <returns>
        /// A HMAC converted to a string
        /// </returns>
        public static string Hash_PBKDF2(string Password, string Salt)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            if (Salt == null || Salt.Length == 0)
                throw new ArgumentNullException("Missing Salt");

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, Salt.ToBytes(), ApplicationSettings.Default.PBKDF2IterationCount.ToInt()))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash.ConvertToString();
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Byte array of Password chars to be converted into a HMAC
        /// </param>
        /// <returns>
        /// A HMAC converted to a string
        /// </returns>
        public static string Hash_PBKDF2(byte[] Password)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");
        
            saltbytes = Generate_RandomBytes(32);

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, saltbytes, ApplicationSettings.Default.PBKDF2IterationCount.ToInt()))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash.ConvertToString();
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string and salt using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Byte array of Password chars to be converted into a HMAC
        /// </param>
        /// <param name="Salt">
        /// The salt, a unique salt means a unique PBKDF2 string
        /// </param>
        /// <returns>
        /// A HMAC converted to a string
        /// </returns>
        public static string Hash_PBKDF2(byte[] Password, string Salt)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            if (Salt == null || Salt == String.Empty)
                throw new ArgumentNullException("Missing Salt");

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, Salt.ToBytes(), ApplicationSettings.Default.PBKDF2IterationCount.ToInt()))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash.ConvertToString();
        }
        
        /// <summary>
        /// Generates and returns a salted HMAC of the provided string using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Password to be converted into a HMAC
        /// </param>
        /// <returns>
        /// A HMAC converted to Byte array
        /// </returns>
        public static byte[] Hash_PBKDF2_To_Bytes(string Password)
        {
            byte[] salt;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null)
                throw new ArgumentNullException("Missing Password");

            //generate a salt and a number of random bytes
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, 32, ApplicationSettings.Default.PBKDF2IterationCount.ToInt()))
            {
                salt = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(salt, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash;
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string and salt using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Password to be converted into a HMAC
        /// </param>
        /// <param name="Salt">
        /// The salt, a unique salt means a unique PBKDF2 string
        /// </param>  
        /// <returns>
        /// A HMAC converted to a Byte array
        /// </returns>
        public static byte[] Hash_PBKDF2_To_Bytes(string Password, string Salt)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password,  Salt.ToBytes(), ApplicationSettings.Default.PBKDF2IterationCount.ToInt()))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash;
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Byte array of Password chars to be converted into a HMAC
        /// </param>
        /// <returns>
        /// A hashed byte array
        /// </returns>
        public static byte[] Hash_PBKDF2_To_Bytes(byte[] Password)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            //generate a random salt
            saltbytes = Generate_RandomBytes(32);

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, saltbytes, int.Parse(ApplicationSettings.Default.PBKDF2IterationCount)))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash;
        }

        /// <summary>
        /// Generates and returns a salted  HMAC of the provided string and salt using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Byte array of Password chars to be converted into a HMAC
        /// </param>
        /// <param name="Salt">
        /// The salt, a unique salt means a unique PBKDF2 string
        /// </param>
        /// <returns>
        /// A hashed byte array
        /// </returns>
        public static byte[] Hash_PBKDF2_To_Bytes(byte[] Password, string Salt)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password.Length==0 || Password == null)
                throw new ArgumentNullException("Missing Password");

            if(Salt == string.Empty || Salt == null)
                throw new ArgumentNullException("Missing Salt");

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, Salt.ToBytes(), ApplicationSettings.Default.PBKDF2IterationCount.ToInt()))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash;
        }

        /// <summary>
        /// Gets the salt from a PBKDF2 generated HMAC
        /// </summary>
        /// <param name="HashedPassword">
        /// The full HMAC generated with the Hash_PBK2DF function
        /// </param>
        /// <returns>
        /// A string containing salt orginally used to generate the HMAC
        /// </returns>
        /// <remarks>
        /// Grabs the first 32 bytes from the string (the Hash_PBKDF2 implementations above use a 32 byte salt)
        /// </remarks>
        public static string Get_PBKDF2Salt(string HashedPassword)
        {
            //convert the string to bytes
            byte[] hash = HashedPassword.ToBytes();
            byte[] salt = new byte[32];

            //grab first 32 bytes
            Buffer.BlockCopy(hash, 0, salt, 0, 32);

            //return just the salt
            return salt.ConvertToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlainText"></param>
        /// <returns></returns>
        public static string Hash_SHA1(string PlainText)
        {
            string sha1hash = string.Empty;

            using (SHA1 sha = new SHA1CryptoServiceProvider())
            {
                sha1hash = sha.ComputeHash(PlainText.ToBytes()).ConvertToString();
            }

            return sha1hash;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlainText"></param>
        /// <returns></returns>
        public static byte[] Hash_SHA1_To_Bytes(string PlainText)
        {
            byte[] sha1hash;

            using (SHA1 sha = new SHA1CryptoServiceProvider())
            {
                sha1hash = sha.ComputeHash(PlainText.ToBytes());
            }

            return sha1hash;
        }

        ///<summary>
        ///Generates a number random bytes using cryptographically secure RNG
        ///</summary>
        ///<param name="NumberOfBytes">
        ///The number of bytes to return
        ///</param>
        ///<returns>
        ///An array of random bytes
        ///</returns>
        public static byte[] Generate_RandomBytes(int NumberOfBytes)
        {
            byte[] randombytes = new byte[NumberOfBytes];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(randombytes);
            }

            return randombytes;
        }

        ///<summary>
        ///Generates a number random bytes using cryptographically secure RNG
        ///</summary>
        ///<param name="NumberOfBytes">
        ///The number of bytes to return
        ///</param>
        ///<returns>
        ///An string of random bytes
        ///</returns>
        public static string Generate_RandomString(int NumberOfChars)
        {
            byte[] randombytes = new byte[NumberOfChars];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(randombytes);
            }

            return randombytes.ConvertToString();
        }

        ///<summary>
        ///Generates a number random string of readable chars using the cryptographically secure RNG
        ///</summary>
        ///<param name="NumberOfBytes">
        ///The number of chars to return
        ///</param>
        ///<returns>
        ///An string of random bytes
        ///</returns>
        public static string Generate_Random_ReadableString(int NumberOfChars)
        {
            string characterSet = "0123456789ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz";
            var buffer = new char[NumberOfChars];
            var usableChars = characterSet.ToCharArray();
            var usableLength = usableChars.Length;

            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {

                var random = new byte[NumberOfChars];
                rngCsp.GetNonZeroBytes(random);

                for (int index = 0; index < NumberOfChars; index++)
                {
                    buffer[index] = usableChars[random[index] % usableLength];
                }

            }

            return new string(buffer);
        }

        /// <summary>
        /// Pads the string with more chars from the original string
        /// </summary>
        /// <param name="OriginalString">
        /// The string to add padding to
        /// </param>
        /// <param name="NumberOfCharsToAdd">
        /// The number of chars to add to the string
        /// </param>
        public static void Add_StringPadding(ref string OriginalString, int NumberOfCharsToAdd)
        {
            //if the number of chars to add is greater then the length of the data
            if (NumberOfCharsToAdd > OriginalString.Length)
            {
                string CopyOfString = OriginalString;

                //keep appending data until chars to add is not longer greater then the length of the data
                while (NumberOfCharsToAdd > CopyOfString.Length)
                {
                    NumberOfCharsToAdd -= CopyOfString.Length;
                    OriginalString += CopyOfString;
                }

                //any remaining chars
                OriginalString += CopyOfString.Substring(0, NumberOfCharsToAdd);
            }
            //number of chars to add was less than the length of the data, so just append what is needed.
            else
                OriginalString += OriginalString.Substring(0, NumberOfCharsToAdd);

        }

        /// <summary>
        /// Pads the byte array with more chars from the original array
        /// </summary>
        /// <param name="OriginalBytes">
        /// The byte array to add padding to
        /// </param>
        /// <param name="NumberOfBytesToAdd">
        /// The number of chars to add to the string
        /// </param>
        public static void Add_BytePadding(ref byte[] OriginalBytes, int NumberOfBytesToAdd)
        {
            //if the number of chars to add is greater then the length of the data
            if (NumberOfBytesToAdd > OriginalBytes.Length)
            {
                //clone the original array
                byte[] CopyOfBytes = new byte[OriginalBytes.Length];
                OriginalBytes.CopyTo(CopyOfBytes, 0);

                //keep appending data until bytes to add is not longer greater then the length of the data
                while (NumberOfBytesToAdd > CopyOfBytes.Length)
                {
                    Buffer.BlockCopy(CopyOfBytes, 0, OriginalBytes, OriginalBytes.Length, CopyOfBytes.Length);
                    NumberOfBytesToAdd -= CopyOfBytes.Length;
                }

                //any remaining chars
                Buffer.BlockCopy(CopyOfBytes, 0, OriginalBytes, OriginalBytes.Length, NumberOfBytesToAdd);

                //clear the array (for security)
                Array.Clear(CopyOfBytes, 0, CopyOfBytes.Length);
            }
            //number of chars to add was less than the length of the data, so just append what is needed.
            else
            {
                //clone the original array
                byte[] CopyOfBytes = new byte[OriginalBytes.Length];
                OriginalBytes.CopyTo(CopyOfBytes, 0);

                //resize the original array
                Array.Resize(ref OriginalBytes, OriginalBytes.Length + NumberOfBytesToAdd);

                //append bytes
                Buffer.BlockCopy(CopyOfBytes, 0, OriginalBytes, CopyOfBytes.Length, NumberOfBytesToAdd);
                
                //clear the array (for security)
                Array.Clear(CopyOfBytes, 0, CopyOfBytes.Length);
            }


        }

    }
}
