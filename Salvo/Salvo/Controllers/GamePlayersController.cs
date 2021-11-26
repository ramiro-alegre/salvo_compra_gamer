using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Salvo.Models;
using Salvo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/gamePlayers")]
    [ApiController]
    [Authorize("PlayerOnly")]
    public class GamePlayersController : ControllerBase
    {
        private IGamePlayerRepository _repository;
        private IPlayerRepository _playerRepository;
        public GamePlayersController(IGamePlayerRepository repository, IPlayerRepository playerRepository)
        {
            _repository = repository;
            _playerRepository = playerRepository;
        }


        // GET api/<GamePlayersController>/5
        [HttpGet("{id}", Name = "GetGameView")]
        public IActionResult GetGameView(long id)
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                var gp = _repository.GetGamePlayerView(id);

                if (gp.Player.Email != email)
                    return Forbid();

                var gameView = new GameViewDTO
                {
                    Id = gp.Id,
                    CreationDate = gp.Game.CreationDate,
                    Ships = gp.Ships.Select(ship => new ShipDTO
                    {
                        Id = ship.Id,
                        Type = ship.Type,
                        Locations = ship.Locations.Select(shipLocation => new ShipLocationDTO
                        {
                            Id = shipLocation.Id,
                            Location = shipLocation.Location
                        }).ToList()
                    }).ToList(),
                    GamePlayers = gp.Game.GamePlayers.Select(gps => new GamePlayerDTO
                    {
                        Id = gps.Id,
                        JoinDate = gps.JoinDate,
                        Player = new PlayerDTO
                        {
                            Id = gps.Player.Id,
                            Email = gps.Player.Email,
                        }
                    }).ToList(),
                    Salvos = gp.Game.GamePlayers.SelectMany(gps => gps.Salvos.Select(salvo => new SalvoDTO
                    {
                        Id = salvo.Id,
                        Turn = salvo.Turn,
                        Player = new PlayerDTO
                        {
                            Id = gps.Player.Id,
                            Email = gps.Player.Email
                        },
                        Locations = salvo.Locations.Select(salvoLocation => new SalvoLocationDTO
                        {
                            Id = salvoLocation.Id,
                            Location = salvoLocation.Location
                        }).ToList()
                    })).ToList()
                };

                return Ok(gameView);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/ships")]
        public IActionResult Post(long id, [FromBody] List<ShipDTO> ships)
        {
            try
            {
                var gp = _repository.FindById(id);
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";
                if (gp == null)
                {
                    return StatusCode(403, "No existe el juego");
                }
                if (email != gp.Player.Email)
                {
                    return StatusCode(403, "El usuario no se encuentra en el juego");
                }
                if (gp.Ships.Count == 5)
                {
                    return StatusCode(403, "Ya se han posicionado los barcos");
                }

                foreach (ShipDTO shipdto in ships)
                {
                    gp.Ships.Add(new Ship
                    {
                        
                        Locations = shipdto.Locations.Select(location => new ShipLocation
                        {

                            Location = location.Location
                        }).ToList(),
                        Type = shipdto.Type
                    });
                }

                _repository.Save(gp);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/salvos")]
        public IActionResult Post(long id, [FromBody] SalvoDTO salvodto)
        {
            try
            {

                var gp = _repository.FindById(id);
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";
                if (gp == null)
                {
                    return StatusCode(403, "No existe el juego");
                }
                if (email != gp.Player.Email)
                {
                    return StatusCode(403, "El usuario no se encuentra en el juego");
                }

                GamePlayer gameplayer = gp.GetOpponent();

                if(gameplayer == null)
                {
                    return StatusCode(403, "No tiene oponente");
                }

                gameplayer = _repository.FindById(gameplayer.Id);
                if(gameplayer.Ships.Count() == 0)
                {
                    return StatusCode(403, "No posiciono los barcos el oponente");
                }

                if(gp.Salvos.Count() > gameplayer.Salvos.Count())
                {
                    return StatusCode(403, "No es su turno");
                }
                
                if ((gp.Salvos.Count == gameplayer.Salvos.Count) && gp.JoinDate > gameplayer.JoinDate)
                    return StatusCode(403, "This is not your turn");
 
                //gp = Usuario actual autenticado
                //gameplayer = Oponente

                gp.Salvos.Add(new Models.Salvo
                {
                    GamePlayerId = id,
                    Turn = gp.Salvos.Count() + 1,
                    Locations = salvodto.Locations.Select(location => new SalvoLocation
                    {
                        //SalvoId = salvodto.Id,
                        Location = location.Location
                    }).ToList()
                });

                _repository.Save(gp);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
