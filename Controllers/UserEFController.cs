using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using DotnetAPI.Data;
using AutoMapper;
namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
  IUserRepository _userRepository;
  IMapper _mapper;

  public UserEFController(IConfiguration config, IUserRepository userRepository)
  {
    _userRepository = userRepository;
    
    _mapper = new Mapper(new MapperConfiguration(cfg => {
      cfg.CreateMap<UserToAddDto, User>();
    }));
  } 

  [HttpGet("GetUsers")]
  public IEnumerable<User> GetUsers()
  {
    IEnumerable<User> users = _userRepository.GetUsers();
    return users;
  }

  [HttpGet("GetSingleUser/{userId}")]
  public User GetSingleUser(int userId)
  {
    return _userRepository.GetSingleUser(userId);
  }

  [HttpPut("EditUser")]
  public IActionResult EditUser(User user) 
  {
    User? userDb = _userRepository.GetSingleUser(user.UserId);

    if (userDb != null)
    {
      userDb.Active = user.Active;
      userDb.FirstName = user.FirstName;
      userDb.LastName = user.LastName;
      userDb.Email = user.Email;
      userDb.Gender = user.Gender;
      if(_userRepository.SaveChanges())
      {
        return Ok();
      }

      throw new Exception("Failed to update user");
    }

    throw new Exception("Failed to update user");
  }

  [HttpPost]
  public IActionResult AddUser(UserToAddDto user) 
  {
    User userDb = _mapper.Map<User>(user);

    _userRepository.AddEntity<User>(userDb);
    if(_userRepository.SaveChanges())
    {
      return Ok();
    }

    throw new Exception("Failed to add user");
  }

  [HttpDelete("DeleteUser/{userId}")]
  public IActionResult DeleteUser(int userId)
  {
    User? userDb = _userRepository.GetSingleUser(userId);
    
    if(userDb != null) 
    {
      _userRepository.RemoveEntity<User>(userDb);
      if(_userRepository.SaveChanges())
      {
        return Ok();
      }
      throw new Exception("Failed to Delete user");
    }

    throw new Exception("Failed to Delete user");
  }

  [HttpGet("UserSalary/{userId}")]
  public UserSalary GetUserSalaryEF(int userId)
  {
    return _userRepository.GetSingelUserSalary(userId);
  }

  [HttpPost("UserSalary")]
  public IActionResult PostUserSalaryEF(UserSalary userSalaryForInsert)
  { 
    _userRepository.AddEntity<UserSalary>(userSalaryForInsert);
    if(_userRepository.SaveChanges())
    {
      return Ok();
    }
    throw new Exception("Adding user salary failed on save");
  }

  [HttpPut("UserSalary")]
  public IActionResult PutUserSalaryEF(UserSalary userSalaryForUpdate)
  {
    UserSalary? userSalaryToUpdate = _userRepository.GetSingelUserSalary(userSalaryForUpdate.UserId);

    if(userSalaryToUpdate != null)
    {
      _mapper.Map(userSalaryForUpdate, userSalaryToUpdate);
      if(_userRepository.SaveChanges())
      {
        return Ok();
      }
      throw new Exception("Failed to update user salary on save");
    }

    throw new Exception("Failed to find user salary to update");
  }

  [HttpDelete("UserSalary/{userId}")]
  public IActionResult DeleteUserSalaryEF(int userId)
  {
    UserSalary? userSalaryToDelete = _userRepository.GetSingelUserSalary(userId);

    if(userSalaryToDelete != null)
    {
      _userRepository.RemoveEntity<UserSalary>(userSalaryToDelete);
      if(_userRepository.SaveChanges())
      {
        return Ok();
      }
      throw new Exception("Failed to delete user salary on save");
    }

    throw new Exception("Failed to find user salary to delete");
  }

  [HttpGet("UserJobInfo/{userId}")]
  public UserJobInfo GetUserJobInfoEF(int userId)
  {
    return _userRepository.GetSingleUserJobInfo(userId);
  }

  [HttpPost("UserJobInfo")]
  public IActionResult PostUserJobInfoEF(UserJobInfo userJobInfoForInsert)
  {
    _userRepository.AddEntity<UserJobInfo>(userJobInfoForInsert);
    if(_userRepository.SaveChanges())
    {
      return Ok();
    }
    throw new Exception("Adding user job info failed on save");
  }

  [HttpPut("UserJobInfo")]
  public IActionResult PutUserJobInfoEF(UserJobInfo userJobInfoForUpdate)
  {
    UserJobInfo? userJobInfoToUpdate = _userRepository.GetSingleUserJobInfo(userJobInfoForUpdate.UserId);

    if(userJobInfoToUpdate != null)
    {
      _mapper.Map(userJobInfoForUpdate, userJobInfoToUpdate);
      if(_userRepository.SaveChanges())
      {
        return Ok();
      }
      throw new Exception("Failed to update user job info on save");
    }

    throw new Exception("Failed to find user job info to update");
  }

  [HttpDelete("UserJobInfo/{userId}")]
  public IActionResult DeleteUserJobInfoEF(int userId)
  {
    UserJobInfo? userJobInfoToDelete = _userRepository.GetSingleUserJobInfo(userId);

    if(userJobInfoToDelete != null)
    {
      _userRepository.RemoveEntity<UserJobInfo>(userJobInfoToDelete);
      if(_userRepository.SaveChanges())
      {
        return Ok();
      }
      throw new Exception("Failed to delete user job info on save");
    }

    throw new Exception("Failed to find user job info to delete");
  }
}
