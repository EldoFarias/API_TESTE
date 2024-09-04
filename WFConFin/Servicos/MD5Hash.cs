using System.Security.Cryptography;
using System.Text;

namespace WFConFin.Servicos
{
    public class MD5Hash
    {

        public static string  CalcularHash(string valor)
        {

            try
            {
                MD5 md5 = MD5.Create(); //Criação do objeto do tipo MD5
                byte[] bytesEntrada = Encoding.ASCII.GetBytes(valor);  //Extrai os bytes do valor recebido
                byte[] hash = md5.ComputeHash(bytesEntrada);  // Gera um hash á partir dos bytes extraídos
                StringBuilder stringBuilder = new StringBuilder();   // Cria um objeto do tipo StringBuilder para retornar a string depois
                for (int i = 0; i < hash.Length; i++)
                {
                    stringBuilder.Append(hash[i].ToString("X2"));  // Gero a string de saída
                }

                return stringBuilder.ToString();  // Retorna a string para o método que chamou
            }
            catch ( Exception ex )
            {
                return null;  // Caso dê algum erro, retorno null
            }

        }

    }
}
