using Base.Application.Profiles;
using Base.Domain.Entities.userInf;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Base.Application.Dtos.Auth
{
    public class UserCreateDto : ICreateMapper<User>
    {
        /// <example>Amirfoad</example>
        [Required(ErrorMessage = "وارد کردن نام کاربری  الزامی است")]
        public string UserName { get; set; }
        /// <example>AmirfoadAhmadi@gmail.com</example>
        [Required(ErrorMessage = "وارد کردن ایمیل  الزامی است")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نیست")]
        public string Email { get; set; }
        [Required(ErrorMessage = "وارد کردن نام  الزامی است")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "وارد کردن نام خوانوادگی الزامی است")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "وارد کردن رمز عبور  الزامی است")]
        public string Password { get; set; }
        [Required(ErrorMessage = "وارد کردن تأیید رمز عبور  الزامی است")]
        [Compare("Password", ErrorMessage = "رمز عبور و  تأیید رمز عبور باید مطابقت داشته باشند")]
        public string RePassword { get; set; }

    }
}
