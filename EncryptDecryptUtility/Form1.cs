using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncryptDecryptUtility
{
    public partial class Form1 : Form
    {
        public string myx509CertificatePath = @"C:\My509Certificate";
        public Form1()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string encryptDecryptValue = string.Empty;
            try
            {
                // Check for null/empty data
                if (richTextBox1.Text == "")
                {
                    MessageBox.Show("Please enter data to encrypt/decrypt", "Validation",
       MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (checkBox1.Checked)
                {
                    // Get publick key pem

                    string textToEncrypt = richTextBox1.Text;
                    // Encrypt the string
                    encryptDecryptValue = Encrypt(textToEncrypt);

                }
                else
                {
                    string textToDecryptBase64 = richTextBox1.Text;
                    // Decrypt the string
                    encryptDecryptValue = Decrypt(textToDecryptBase64);

                }

                richTextBox2.Text = encryptDecryptValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Please enter data to encrypt/decrypt. Message: {ex.Message}", "Exception",
       MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private string Decrypt(string textToDecryptBase64)
        {
            // Convert from base64 to byte data
            byte[] byteData = Convert.FromBase64String(textToDecryptBase64);
            string path = Path.Combine(myx509CertificatePath, "test-cert.pfx");

            //Note This Password is That Password That We Have Put On Generate Keys
            var Password = "844644";   
            var collection = new X509Certificate2Collection();
            collection.Import(System.IO.File.ReadAllBytes(path), Password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            X509Certificate2 certificate = new X509Certificate2();
            certificate = collection[0];

            if (certificate.HasPrivateKey)
            {
                RSA csp = (RSA)certificate.PrivateKey;
                var privateKey = certificate.PrivateKey as RSACryptoServiceProvider;
                var keys = Encoding.UTF8.GetString(csp.Decrypt(byteData, RSAEncryptionPadding.OaepSHA1));
                return keys;
            }
            return null;
        }

        private string Encrypt(string textToEncrypt)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(textToEncrypt);
            string path = Path.Combine(myx509CertificatePath, "test-cert-public.pem");
            var collection = new X509Certificate2Collection();
            collection.Import(path);
            var certificate = collection[0];
            var output = "";
            using (RSA csp = (RSA)certificate.PublicKey.Key)
            {
                byte[] bytesEncrypted = csp.Encrypt(byteData, RSAEncryptionPadding.OaepSHA1);
                output = Convert.ToBase64String(bytesEncrypted);
            }
            return output;


        }
    }
}
