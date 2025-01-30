#if Authentication || DEBUG
using GenerateTemplate.Domain.Common;
using GenerateTemplate.Domain.Entity.UserEntity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
#endif
using GenerateTemplate.Domain.Interface.Utils;
using System.Security.Cryptography;
using System.Text;

namespace GenerateTemplate.Domain.Utils;

public class GenerateHash : IGenerateHash
{
    public int GenerateRandomNumber()
    {
        Random random = new Random();
        return random.Next(100000, 1000000);
    }

    public string GenerateHashRandom()
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string randomValue = Guid.NewGuid().ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(randomValue);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hash;
        }
    }

    public string GenerateHashParameters(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            // Converte a senha em bytes
            byte[] bytes = Encoding.UTF8.GetBytes(password);

            // Calcula o hash
            byte[] hashBytes = sha256.ComputeHash(bytes);

            // Converte o hash para uma string hexadecimal
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        // Gera o hash da senha fornecida pelo usuário
        string hashInputPassword = GenerateHashParameters(password);

        // Compara os hashes de forma segura, evitando ataques de tempo
        // Verifica se os comprimentos dos hashes são iguais antes de fazer a comparação
        // Isso evita ataques de tempo em que um invasor pode inferir a senha através do tempo de comparação
        return hashInputPassword == hashedPassword;
    }

#if Authentication || DEBUG
    public string GenerateToken(UserModel userModel)
    {
        Claim[] claims =
        [
          new Claim("username", userModel.UserName),
          new Claim("email", userModel.Email),
          new Claim("cpf", userModel.Cpf),
          new Claim("role", userModel.Role.ToString()),
          new Claim("dataCreated", userModel.DateCreated.ToString()),
          new Claim("id", userModel.Id),
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.Key));

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
            (
                expires: DateTime.Now.AddMinutes(10),
                claims: claims,
                signingCredentials: signingCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
#endif
}
