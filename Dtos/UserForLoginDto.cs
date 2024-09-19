namespace DotnetAPI.Dtos
{
  partial class UserForLoginDto
  {
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }

    public UserForLoginDto()
    {
      if (Email == null)
      {
        Email = "";
      }
      if (Password == null)
      {
        Password = "";
      }
      if (PasswordConfirm == null)
      {
        PasswordConfirm = "";
      }
    }
  }
}