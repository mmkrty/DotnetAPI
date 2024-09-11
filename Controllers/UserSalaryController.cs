using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using DotnetAPI.Data;
namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class UserSalaryController : ControllerBase
{
  DataContextDapper _dapper;

  public UserSalaryController(IConfiguration config)
  {
    Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    _dapper = new DataContextDapper(config);
  }

  [HttpGet("GetUserSalaries")]
  public IEnumerable<UserSalary> GetUserSalaries()
  {
    string sql = @"
          SELECT [UserId],
            [Salary]
          FROM TutorialAppSchema.UserSalary";
    IEnumerable<UserSalary> userSalaries = _dapper.LoadData<UserSalary>(sql);
    return userSalaries;
  }
}