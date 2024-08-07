using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTO;

public class RegisterDTO
{
    [Required] public string PersonName { get; set; }

    [Required]
    [EmailAddress(ErrorMessage =
        "E-mail should be in a proper email address format")]
    public string Email { get; set; }

    [Required]
    [RegularExpression("^[0-9]*$",
        ErrorMessage = "Phone number should contain numbers only")]
    public string Phone { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password",
        ErrorMessage = "Password and confirm password don't match!")]
    public string ConfirmPassword { get; set; }
}
