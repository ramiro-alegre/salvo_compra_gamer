using Microsoft.AspNetCore.Mvc;
using Salvo.Models;
using Salvo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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


        // POST api/<PlayersController>
        [HttpPost]
        public IActionResult Post([FromBody] PlayerDTO player)
        {
            try
            {
                //El mail debe contener un letras + @ + letras + . + com/otro
                //No acepta cualquier signo
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(player.Email);
                //Sí el match es false, es porque el email no esta bien, sino se pregunta si esta el mail, la password o el nombre vacios, en ese caso,
                //devuelve datos invalidos. En el caso de no cumplir ninguno devuelve ""
                string mensaje = !match.Success ? "Email invalido" : String.IsNullOrEmpty(player.Email) || String.IsNullOrEmpty(player.Password) ? "Datos invalidos" : "";
                
                if (mensaje != "")
                    return StatusCode(403, mensaje);
               
                Player dbPlayer = _repository.FindByMail(player.Email);
                if(dbPlayer != null) 
                {
                    return StatusCode(403, "Email en uso");
                }

                byte[] data = Encoding.ASCII.GetBytes(player.Password);
                data = new SHA256Managed().ComputeHash(data);
                string passwordHash = Encoding.ASCII.GetString(data);

                Player newPlayer = new Player
                {
                    Email = player.Email,
                    Password = passwordHash,
                    Name = player.Name,
                    Avatar = "1.jpg"
                };

                _repository.Save(newPlayer);
                return StatusCode(201, newPlayer);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
           
        }

       

    }
}
