using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;
using WFConFin.Models;

namespace WFConFin.Servicos
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(Usuario usuario) //Recebo os dados do usuario pelo parametro
        {
            var tokenHandler = new JwtSecurityTokenHandler(); //Crio a estrutura JWT do tokem

            var chave = Encoding.ASCII.GetBytes(_configuration.GetSection("Chave").Get<string>()); //Pego a informação do appSetting com a chave e transformo em bytes

            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, usuario.Login.ToString()),
                        new Claim(ClaimTypes.Role, usuario.Funcao.ToString()),

                    }),

                Expires = DateTime.UtcNow.AddHours(2), // Aqui seria o tempo de expiração do token, no caso 2 horas

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature) //Aqui estou criando as assinaturas e passando o algoritimo de criptografia

            }; //Aqui são os dados que farão parte do meu tokem a ser gerado, (Usuario, 
             
            var token = tokenHandler.CreateToken(tokenDescriptor); //Aqui estou gerando o token efetivamente com os dados acima

            return tokenHandler.WriteToken(token); // Aqui estou retornando o token para o metodo que fez a chamada.
        }

    }
}
