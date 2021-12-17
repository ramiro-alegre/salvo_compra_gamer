using Microsoft.AspNetCore.Mvc;
using Salvo.Models;
using Salvo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private IPlayerRepository _repository;

        public PlayersController(IPlayerRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Post([FromBody] PlayerDTO playerDTO)
        {
            bool CamposInvalidos = false;
            string MensajeError = "", mensajeErrorPassword = "";

            #region Validaciones
            if (string.IsNullOrEmpty(playerDTO.Name))
            {
                CamposInvalidos = true;
                MensajeError = (string.IsNullOrEmpty(MensajeError)) ? "The Name cannot be Empty" : MensajeError + " - The Name cannot be Empty";
            }
            if (!isValidName(playerDTO.Name))
            {
                CamposInvalidos = true;
                MensajeError = (string.IsNullOrEmpty(MensajeError)) ? "The Name is not valid" : MensajeError + " - The Name is not valid";
            }
            if (string.IsNullOrEmpty(playerDTO.Email))
            {
                CamposInvalidos = true;
                MensajeError = (string.IsNullOrEmpty(MensajeError)) ? "The Email cannot be Empty" : MensajeError + " - The Email cannot be Empty";
            }
            if (!isValidEmail(playerDTO.Email))
            {
                CamposInvalidos = true;
                MensajeError = (string.IsNullOrEmpty(MensajeError)) ? "The Email is not valid" : MensajeError + " - The Email is not valid";
            }
            if (string.IsNullOrEmpty(playerDTO.Password))
            {
                CamposInvalidos = true;
                MensajeError = (string.IsNullOrEmpty(MensajeError)) ? "The Password cannot be Empty" : MensajeError + " - The Password cannot be Empty";
            }
            if (!IsValidPassword(playerDTO.Password, out mensajeErrorPassword))
            {
                CamposInvalidos = true;
                MensajeError = (string.IsNullOrEmpty(MensajeError)) ? mensajeErrorPassword : MensajeError + " - " + mensajeErrorPassword;
            }
            #endregion

            if (!CamposInvalidos)
            {
                Player newPlayer = _repository.FindByMail(playerDTO.Email);
                if (newPlayer == null)
                {
                    byte[] data = Encoding.ASCII.GetBytes(playerDTO.Password);
                    data = new SHA256Managed().ComputeHash(data);
                    string passwordHash = Encoding.ASCII.GetString(data);

                    newPlayer = new Player
                    {
                        Name = playerDTO.Name,
                        Email = playerDTO.Email,
                        Password = passwordHash,
                        Avatar = "1.jpg"
                    };

                    _repository.Save(newPlayer);

                    return StatusCode(201, "Creado");
                }
                else
                {
                    return StatusCode(403, "Email en uso");
                }
            }
            else
            {
                return StatusCode(403, MensajeError);
            }
        }

        public bool isValidName(string Name)
        {
            var hasBetween3And30Chars = new Regex(@".{3,30}");

            return hasBetween3And30Chars.IsMatch(Name);
        }
        public bool isValidEmail(string Email)
        {
            var hasBetween10And30Chars = new Regex(@".{10,30}");

            bool hasValidFormat = true;

            if(Email == null || Email == "")
            {
                hasValidFormat = false;
                return hasValidFormat;
            }

            try
            {
                MailAddress address = new MailAddress(Email);
                hasValidFormat = (address.Address == Email);
                // or
                // isValid = string.IsNullOrEmpty(address.DisplayName);
            }
            catch (FormatException)
            {
                hasValidFormat = false;
            }

            return hasValidFormat && hasBetween10And30Chars.IsMatch(Email);
        }
        public bool IsValidPassword(string password, out string ErrorMessage)
        {
            var minLength = 8;
            var numUpper = 1;
            var numLower = 1;
            var numNumbers = 1;
            var numSpecial = 1;

            var upper = new Regex("[A-Z]");
            var lower = new Regex("[a-z]");
            var number = new Regex("[0-9]");
            var special = new Regex("[^a-zA-Z0-9]");

            if (password.Length < minLength)
            {
                ErrorMessage = "Password should not be less than or greater than 8 characters";
                return false;
            }
            if (upper.Matches(password).Count < numUpper)
            {
                ErrorMessage = "Password should contain at least one upper case letter";
                return false;
            }
            if (lower.Matches(password).Count < numLower)
            {
                ErrorMessage = "Password should contain at least one lower case letter";
                return false;
            }
            if (number.Matches(password).Count < numNumbers)
            {
                ErrorMessage = "Password should contain At least one numeric value";
                return false;
            }
            if (special.Matches(password).Count < numSpecial)
            {
                ErrorMessage = "Password should contain at least one special case character";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }
    }
}
