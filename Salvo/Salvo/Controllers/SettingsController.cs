using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Salvo.Models;
using Salvo.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Salvo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("PlayerOnly")]
    public class SettingsController : ControllerBase
    {

        private IPlayerRepository _repository;
        public SettingsController(IPlayerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("permisse")]
        [AllowAnonymous]
        public IActionResult GetPermisses()
        {
            if(User.FindFirst("Player") != null)
            {
                return Ok("Usuario permitido");
            }
            else
            {
                //Unauthorized()
                return Forbid("Usuario denegado");
            }

            
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                if (email == "Guest")
                {
                    return StatusCode(403, "No tiene permisos");
                }

                Player dbPlayer = _repository.FindByMail(email);

                if (dbPlayer == null)
                {
                    return StatusCode(403, "Ocurrio un error");
                }

                PlayerDTO player = new PlayerDTO
                {
                    Name = dbPlayer.Name,
                    Email = email,
                    Avatar = dbPlayer.Avatar
                };

                return Ok(player);
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex.Message);
            }



        }

        [HttpPost("name")]
        public IActionResult ChangeName([FromBody] PlayerDTO player)
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                if (email == "Guest")
                    return StatusCode(403, "Ocurrio un error");

                Player dbPlayer = _repository.FindByMail(email);

                if (dbPlayer == null)
                    return StatusCode(403, "Ocurrio un error");

                if (dbPlayer.Password != player.Password)
                    return StatusCode(403, "Contraseña Incorrecta");


                Player newPlayer = new Player
                {
                    Id = dbPlayer.Id,
                    Name = player.Name,
                    Email = email,
                    Password = dbPlayer.Password
                };

                _repository.Save(newPlayer);

                return StatusCode(201, newPlayer);
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpPost("mail")]
        public async Task<IActionResult> ChangeMail([FromBody] PlayerDTO player)
        {
            try
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(player.Email);
            
                string mensaje = !match.Success ? "Email invalido" : String.IsNullOrEmpty(player.Email) || String.IsNullOrEmpty(player.Password) ? "Datos invalidos" : "";

                if (mensaje != "")
                    return StatusCode(403, mensaje);

                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                if (email == "Guest")
                    return StatusCode(403, "Ocurrio un error");

                Player dbPlayer = _repository.FindByMail(email);

                if (dbPlayer == null)
                    return StatusCode(403, "Ocurrio un error");

                if (dbPlayer.Password != player.Password)
                    return StatusCode(403, "Contraseña Incorrecta");


                Player newPlayer = new Player
                {
                    Id = dbPlayer.Id,
                    Name = dbPlayer.Name,
                    Email = player.Email,
                    Password = dbPlayer.Password,
                    Avatar = dbPlayer.Avatar
                };

                var claims = new List<Claim>
                {
                    new Claim("Player", newPlayer.Email),
                    new Claim("Avatar", dbPlayer.Avatar)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme
                );

                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                );

                _repository.Save(newPlayer);

                return StatusCode(201, newPlayer);
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpPost("password")]
        public IActionResult ChangePassword([FromBody] PasswordPlayerDTO player)
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                if (email == "Guest")
                    return StatusCode(403, "Ocurrio un error");

                Player dbPlayer = _repository.FindByMail(email);

                if (dbPlayer == null)
                    return StatusCode(403, "Ocurrio un error");

                if (dbPlayer.Password != player.Password)
                    return StatusCode(403, "Contraseña Incorrecta");

                if(player.NewPassword != player.NewPasswordRepeat)
                    return StatusCode(403, "Las contraseñas no coinciden");



                Player newPlayer = new Player
                {
                    Id = dbPlayer.Id,
                    Name = dbPlayer.Name,
                    Email = email,
                    Password = player.NewPassword
                };

                _repository.Save(newPlayer);

                return StatusCode(201, newPlayer);

            }catch(Exception ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpPut("avatar")]
        public async Task<IActionResult> ChangeAvatar([FromBody] PlayerDTO player)
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                if (email == "Guest")
                    return StatusCode(403, "Ocurrio un error");

                Player dbPlayer = _repository.FindByMail(email);

                if (dbPlayer == null)
                    return StatusCode(403, "Ocurrio un error");


                Player newPlayer = new Player
                {
                    Id = dbPlayer.Id,
                    Name = dbPlayer.Name,
                    Email = email,
                    Password = dbPlayer.Password,
                    Avatar = player.Avatar
                };
                var claims = new List<Claim>
                {
                    new Claim("Avatar", newPlayer.Avatar),
                    new Claim("Player", email)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme
                );

                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                );

                _repository.Save(newPlayer);

                return StatusCode(201, newPlayer.Avatar);
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex.Message);
            }
        }
    }
}
