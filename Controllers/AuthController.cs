using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;
    public AuthController(IConfiguration config)
    {
      _dapper = new DataContextDapper(config);
      _authHelper = new AuthHelper(config);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
      if(userForRegistration.Password == userForRegistration.PasswordConfirm)
      {
        string sqlCheckUserExists = "SELECT * FROM TutorialAppSchema.Auth WHERE Email = '"
          + userForRegistration.Email + "'";
        
        IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

        if(existingUsers.Count() == 0)
        {
          byte[] passwordSalt = new byte[128 /8];
          using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
          {
            rng.GetNonZeroBytes(passwordSalt);
          }
          
          byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

          string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert
              @Email = @EmailParam, 
              @PasswordHash = @PasswordHashParam, 
              @PasswordSalt = @PasswordSaltParam";
          
          List<SqlParameter> sqlParameters = new List<SqlParameter>();

          SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
          emailParameter.Value = userForRegistration.Email;
          sqlParameters.Add(emailParameter);

          SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary);
          passwordHashParameter.Value = passwordHash;
          sqlParameters.Add(passwordHashParameter);

          SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary);
          passwordSaltParameter.Value = passwordSalt;
          sqlParameters.Add(passwordSaltParameter);

          if(_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
          {

            string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                @FirstName = '" + userForRegistration.FirstName + 
                "', @LastName = '" + userForRegistration.LastName + 
                "', @Email = '" + userForRegistration.Email + 
                "', @Gender = '" + userForRegistration.Gender + 
                "', @Active = 1" + 
                ", @JobTitle = '" + userForRegistration.JobTitle + 
                "', @Department = '" + userForRegistration.Department + 
                "', @Salary= '" + userForRegistration.Salary + "'";

            if (_dapper.ExecuteSql(sqlAddUser))
            {
              return Ok();
            }

            return Ok();
          }
          throw new Exception("Failed to register user");
        }
          throw new Exception("User with this email already exists");
      }
      throw new Exception("Passwords do not match");
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
      string sqlForHashAndSalt = @"SELECT
              [PasswordHash], 
              [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" +
              userForLogin.Email + "'";

      UserForLoginConfirmationDto userForConfirmation = 
            _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt); 

      byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

      for(int i = 0; i < passwordHash.Length; i++)
      {
        if(passwordHash[i] != userForConfirmation.PasswordHash[i])
        {
         return StatusCode(401, "Incorrect password");
        }
      }

      string userIdSql = @"
          SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" +
          userForLogin.Email + "'";

      int userId = _dapper.LoadDataSingle<int>(userIdSql);

      return Ok(new Dictionary<string, string> {
        {"token", _authHelper.CreateToken(userId)}
      });
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
      string userId = User.FindFirst("userId")?.Value + "";

      string userIdSql = "SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = " + userId;

      int userIdFromDb = _dapper.LoadDataSingle<int>(userIdSql);

      return Ok(new Dictionary<string, string> {
        {"token", _authHelper.CreateToken(userIdFromDb)}
      });
    }
  }
}