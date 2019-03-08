using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Common.Security
{
    public class DES
    {
        private static string DESKey = "oEp*Sj2p";
        public static string bohuijinrongDESKey = "oEW*Jo2x";
        public static string ChinaPayDESKey = "EOTYF68H5DYQW69A3F2DEI54WBCJSJ1Y";

        public static string Encrypt(string pToEncrypt)
        {
            return Encrypt(pToEncrypt, DESKey);
        }

        /// <summary>
        /// 进行DES加密。
        /// </summary>
        /// <param name="pToEncrypt">要加密的字符串。</param>
        /// <param name="sKey">密钥，且必须为8位。</param>
        /// <returns>以Base64格式返回的加密字符串。</returns>
        public static string Encrypt(string pToEncrypt, string sKey, bool needBase64 = true)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = needBase64 ? Convert.ToBase64String(ms.ToArray()) : Signature.GenBytes2Hex(ms.ToArray(), null);
                ms.Close();
                return str;
            }
        }

        public static string DES3Decrypt(string data, string key)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DES.Mode = CipherMode.CBC;
            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            string result = "";
            try
            {
                byte[] Buffer = Encoding.UTF8.GetBytes(data);
                byte[] TempBuffer = DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length);
                result = Convert.ToBase64String(TempBuffer);
            }
            catch (Exception e)
            {

            }
            return result;
        }

        public static string Decrypt(string pToDecrypt)
        {
            return Decrypt(pToDecrypt, DESKey);
        }

        /// <summary>
        /// 进行DES解密。
        /// </summary>
        /// <param name="pToDecrypt">要解密的以Base64</param>
        /// <param name="sKey">密钥，且必须为8位。</param>
        /// <returns>已解密的字符串。</returns>
        public static string Decrypt(string pToDecrypt, string sKey, bool needBase64 = true)
        {
            byte[] inputByteArray = needBase64 ? Convert.FromBase64String(pToDecrypt) : Encoding.UTF8.GetBytes(pToDecrypt);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="key">Key键</param>
        /// <param name="iv">向量</param>
        /// <param name="value">要解密的串</param>
        /// <returns>解密后的字串</returns>
        public static string DecryptString(string key,  string value)
        {
            string iv = "";
            using (SymmetricAlgorithm desCSP = new TripleDESCryptoServiceProvider())
            {
                desCSP.Key = Convert.FromBase64String(key);
              //  desCSP.IV = Convert.FromBase64String(iv);    //指定加密的运算模式 
                desCSP.Mode = System.Security.Cryptography.CipherMode.ECB;    //获取或设置加密算法的填充模式 
                desCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                ICryptoTransform ctTrans = desCSP.CreateDecryptor(desCSP.Key, desCSP.IV);
                byte[] bytes = Convert.FromBase64String(value);
                byte[] bytesOut = new byte[10240];
                int iReadLen = 0;
                using (MemoryStream memStream = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(memStream, ctTrans, CryptoStreamMode.Read))
                    {
                        using (MemoryStream outStream = new MemoryStream())
                        {
                            while ((iReadLen = csDecrypt.Read(bytesOut, 0, bytesOut.Length)) > 0)
                            {
                                outStream.Write(bytesOut, 0, iReadLen);
                            }
                            bytes = null;
                            bytesOut = null;//清理内存
                            return System.Text.Encoding.UTF8.GetString(outStream.ToArray());
                        }
                    }
                }
            }
        }

        #region  DecryptString 对应的Java加密代码

        //        import java.io.UnsupportedEncodingException;
//import java.security.InvalidKeyException;
//import java.security.NoSuchAlgorithmException;
//import java.util.Base64;

//import javax.crypto.BadPaddingException;
//import javax.crypto.Cipher;
//import javax.crypto.IllegalBlockSizeException;
//import javax.crypto.NoSuchPaddingException;
//import javax.crypto.SecretKey;
//import javax.crypto.spec.SecretKeySpec;

//public class HelloWorld {

//    public static void main(String[] args) {
//         try{
//             String toSign = "CID=150&CUserID=12312312&TimeStamp=100000";
//             String strEncode = encryptString(toSign);
//             System.out.println("加密返回："+strEncode);
//             String strOrg = decryptString(strEncode);
//             System.out.println("解密返回："+strOrg);
//         }
//         catch(Exception e){
//             e.printStackTrace();
//         }
//    }

//    private static String Algorithm = "DESede";
	   
//    private static final String Default_Key = "EOTYF68H5DYQW69A3F2DEI54WBCJSJ1Y";
   
//    public static String encryptString(String value) throws InvalidKeyException, NoSuchAlgorithmException, NoSuchPaddingException, IllegalBlockSizeException, BadPaddingException, UnsupportedEncodingException{
//        return encryptString(Default_Key, value);
//    }
   
   
//    public static String encryptString(String key, String value) throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, IllegalBlockSizeException, BadPaddingException, UnsupportedEncodingException{
//        byte[] bytesKey = null, bytes = null, bytesCipher = null;
//        SecretKey deskey = null;
//        if (value == null)
//            new IllegalArgumentException("待加密字串不允许为空");
//        //密码器
//        Cipher desCipher = Cipher.getInstance(Algorithm);
//        try{
//            bytesKey =Base64.getDecoder().decode(key);//Base64.decodeBase64(key);
//            deskey = new SecretKeySpec(bytesKey, Algorithm);
//            bytes = value.getBytes();//待解密的字串
//            desCipher.init(Cipher.ENCRYPT_MODE, deskey);//初始化密码器，用密钥deskey,进入解密模式 
//            bytesCipher = desCipher.doFinal(bytes);
//            return Base64.getEncoder().encodeToString(bytesCipher).trim();
//        }
//        finally{
//            bytesKey = null;
//            bytes = null;
//            bytesCipher = null;
//        }
//     }
   
//    public static String decryptString(String value) throws InvalidKeyException, NoSuchAlgorithmException, NoSuchPaddingException, IllegalBlockSizeException, BadPaddingException, UnsupportedEncodingException{
//        return decryptString(Default_Key, value);
//    }
   
//    public static String decryptString(String key, String value) throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, IllegalBlockSizeException, BadPaddingException, UnsupportedEncodingException{
//        byte[] bytesKey = null, bytes = null, bytesCipher = null;
//        SecretKey deskey = null;
//        if (value == null)
//            new IllegalArgumentException("待解密字串不允许为空");
//        //密码器
//        Cipher desCipher = Cipher.getInstance(Algorithm);
//        try{
//            bytesKey =Base64.getDecoder().decode(key); // Base64.decodeBase64(key);
//            deskey = new SecretKeySpec(bytesKey, Algorithm);
//            bytes = Base64.getDecoder().decode(value); // Base64.decodeBase64(value);//加密后的字串
//            desCipher.init(Cipher.DECRYPT_MODE, deskey);//初始化密码器，用密钥deskey,进入解密模式 
//            bytesCipher = desCipher.doFinal(bytes);
//            return (new String(bytesCipher,"UTF-8"));
//        }
//        finally{
//            bytesKey = null;
//            bytes = null;
//            bytesCipher = null;
//        }
//     } 
     
//}


        #endregion

    }
}