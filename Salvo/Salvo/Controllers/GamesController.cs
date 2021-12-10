using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Salvo.Models;
using Salvo.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private IGameRepository _repository;
        private IPlayerRepository _playerRepository;
        private IGamePlayerRepository _gpRepository;

        public GamesController(IGameRepository repository, IPlayerRepository playerRepository, IGamePlayerRepository gpRepository)
        {
            _repository = repository;
            _playerRepository = playerRepository;
            _gpRepository = gpRepository;
        }
        // GET: api/<GamesController>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                GameListDTO gameList = new GameListDTO
                {
                    Email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest",
                    Avatar = User.FindFirst("Avatar") != null ? User.FindFirst("Avatar").Value : "Images/1.jpg",
                    Games = _repository.GetAllGamesWithPlayers()
                    .Select(game => new GameDTO
                    {
                        Id = game.Id,
                        CreationDate = game.CreationDate,
                        GamePlayers = game.GamePlayers.Select(gp => new GamePlayerDTO
                        {
                            Id = gp.Id,
                            JoinDate = gp.JoinDate,
                            Player = new PlayerDTO
                            {
                                Id = gp.Player.Id,
                                Name = (gp.Player.Name.Length > 10) ? gp.Player.Name.Substring(0, 10) + "..." : gp.Player.Name,
                                Email = gp.Player.Email,
                                Avatar = gp.Player.Avatar
                            },
                            Point = gp.GetScore() != null ? (double?)gp.GetScore().Point : null
                        }).ToList()
                    }).ToList()
            };

                return Ok(gameList);
            } catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }

        [HttpGet("topTypes")]
        [AllowAnonymous]
        public IActionResult GetTopTypeDestroyed()
        {
            try
            {
                List<String> Sunks = new List<String>();
                var games = _repository.GetAllGamesWithPlayersAndSalvos();
                 foreach(var game in games)
                  {
                      foreach(var gp in game.GamePlayers)
                      {
                          Sunks.AddRange(gp.GetSunks());
                      }
                  }
                var types = Sunks
                     .GroupBy(i => i)
                     .OrderByDescending(g => g.Count());

                IEnumerable sunksTop5 = types.Take(5).Select(type => new
                {
                    type = type.First(),
                    quantity = type.Count()
                });


                return Ok(sunksTop5);
            }catch(Exception ex)
            {
                return StatusCode(403, ex.Message);
            }
        }


        [HttpGet("topLocations")]
        [AllowAnonymous]
        public IActionResult GetTopLocationsDestroyed()
        {
            try
            {   
                IEnumerable<String> salvoLocations = _repository.GetAllSalvoLocations()
                .SelectMany(game => game.GamePlayers)
                    .SelectMany(gp => gp.Salvos)
                        .SelectMany(salvo => salvo.Locations)
                            .Select(location => location.Location);

                var locations = salvoLocations
                     .GroupBy(i => i)
                     .OrderByDescending(g => g.Count());

                IEnumerable mayores = locations.Take(5).Select(location => new
                {
                    position = location.First(),
                    quantity = location.Count()
                }) ;
                return Ok(mayores);
            }catch(Exception ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";
                Player player = _playerRepository.FindByMail(email);

                DateTime fechaActual = DateTime.Now;

                GamePlayer gamePlayer = new GamePlayer
                {
                    Game = new Game
                    {
                        CreationDate = fechaActual
                    },
                    PlayerId = player.Id,
                    JoinDate = fechaActual
                };
                _gpRepository.Save(gamePlayer);

                return StatusCode(201, gamePlayer.Id);
            }
            catch(Exception ex)
            {
               return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/players", Name="Join")]
        public IActionResult Join(long id)
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";
                Player player = _playerRepository.FindByMail(email);

                Game game = _repository.FindById(id);

                //Validaciones
                if(game == null)
                {
                    return StatusCode(403, "No existe el juego");
                }
                if (game.GamePlayers.Where(gp => gp.Player.Id == player.Id).FirstOrDefault() != null)
                {
                    return StatusCode(403, "Ya se encuentra el jugador en el juego");
                }
                if(game.GamePlayers.Count > 1)
                {
                    return StatusCode(403, "Juego lleno");
                }
                GamePlayer gamePlayer = new GamePlayer
                {
                    GameId = game.Id,
                    PlayerId = player.Id,
                    JoinDate = DateTime.Now
                };
                _gpRepository.Save(gamePlayer);
                return StatusCode(201, gamePlayer.Id);
                
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /*[HttpGet ("/estadistica", Name = "GetSunkedShips")]
        [AllowAnonymous]
        public IActionResult GetSunkedShips()
        {

        }*/
    }
}
